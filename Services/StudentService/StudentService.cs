using Firebase.Auth;
using Google.Cloud.Storage.V1;
using griffined_api.Extensions.DateTimeExtensions;
using System.Net;

namespace griffined_api.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private readonly string? FIREBASE_BUCKET = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
        private readonly string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageClient _storageClient;
        private readonly IFirebaseService _firebaseService;

        public StudentService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, StorageClient storageClient, IFirebaseService firebaseService)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            _storageClient = storageClient;
            _firebaseService = firebaseService;
        }

        public async Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent, IFormFile? newProfilePicture, List<IFormFile>? filesToUpload)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = newStudent.Nickname.ToLower() + newStudent.DOB.ToDateTime().ToString("dd/MM/yyyy");

            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newStudent.Email, password);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    throw new ConflictException("Email Exists");
                else if (ex.Message.Contains("INVALID_EMAIL"))
                    throw new ConflictException("Invalid Email Format");
                else
                    throw new InternalServerException("Something went wrong.");
            }

            var existedStudent = await _context.Students.FirstOrDefaultAsync(x => x.FirstName == newStudent.FirstName
                                                                               && x.LastName == newStudent.LastName);

            if (existedStudent is not null)
            {
                throw new ConflictException($"Student with first name ({newStudent.FirstName}) and last name ({newStudent.LastName}) already exists.");
            }

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            var _student = _mapper.Map<Student>(newStudent);
            _student.DOB = newStudent.DOB.ToDateTime();
            _student.FirebaseId = firebaseId;
            _student.CreatedBy = id;
            _student.LastUpdatedBy = id;
            _student.ExpiryDate = DateTime.Now;
            _student.Status = StudentStatus.Inactive;

            _context.Students.Add(_student);

            await _context.SaveChangesAsync();

            // INITIALIZE CURRENT DATE, MONTH, AND RUNNING NUMBER.
            DateTime now = DateTime.Now;
            int currentMonth = DateTime.Now.Month;
            int runningNumber = 0;

            // GET LAST STUDENT CODE.      
            var latestStudentCode = _context.Students.Where(s => s.DateCreated.Year == now.Year
                                                              && s.DateCreated.Month == currentMonth)
                                                     .OrderByDescending(s => s.StudentCode)
                                                     .Select(s => s.StudentCode)
                                                     .FirstOrDefault();

            // IF A STUDENT CODE EXISTS FOR THE CURRENT MONTH, EXTRACT AND INCREMENT THE RUNNING NUMBER.
            if (!string.IsNullOrEmpty(latestStudentCode))
            {
                string runningNumberStr = latestStudentCode.Substring(6, 3);
                runningNumber = int.Parse(runningNumberStr);
            }

            runningNumber++;

            // GENERATE STUDENT CODE
            string studentCode = "GF" +
                                 DateTime.Now.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) +
                                 DateTime.Now.ToString("MM", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) +
                                 runningNumber.ToString("D3");

            _student.StudentCode = studentCode;

            var data = _mapper.Map<StudentResponseDto>(_student);

            data.DOB = _student.DOB.ToDateString();

            if (newProfilePicture != null)
            {
                _student.ProfilePicture = new ProfilePicture();

                var pictureRequestDto = new AddProfilePictureRequestDto
                {
                    PictureData = newProfilePicture
                };

                var pictureEntity = _mapper.Map<ProfilePicture>(pictureRequestDto);
                var fileName = newProfilePicture.FileName;
                var objectName = $"students/{studentCode}/profile/{fileName}";

                using (var stream = pictureRequestDto.PictureData.OpenReadStream())
                {
                    var storageObject = await _storageClient.UploadObjectAsync(
                        FIREBASE_BUCKET,
                        objectName,
                        newProfilePicture.ContentType,
                        stream
                    );

                    pictureEntity.FileName = fileName;
                    pictureEntity.ObjectName = objectName;
                }
                string url = await _firebaseService.GetUrlByObjectName(objectName);

                var pictureResponseDto = new FilesResponseDto
                {
                    FileName = pictureEntity.FileName,
                    ContentType = newProfilePicture.ContentType,
                    URL = url
                };

                _student.ProfilePicture = pictureEntity;
                data.ProfilePicture = pictureResponseDto;
            }

            if (filesToUpload != null && filesToUpload.Count > 0)
            {
                _student.AdditionalFiles = new List<StudentAdditionalFile>();

                foreach (var file in filesToUpload)
                {
                    var fileRequestDto = new AddStudentAdditionalFilesRequestDto
                    {
                        FileData = file
                    };

                    var fileEntity = _mapper.Map<StudentAdditionalFile>(fileRequestDto);
                    var fileName = file.FileName;
                    var objectName = $"students/{studentCode}/documents/{fileName}";

                    using (var stream = fileRequestDto.FileData.OpenReadStream())
                    {
                        var storageObject = await _storageClient.UploadObjectAsync(
                            FIREBASE_BUCKET,
                            objectName,
                            file.ContentType,
                            stream
                        );

                        fileEntity.FileName = fileName;
                        fileEntity.ObjectName = objectName;
                    }
                    string url = await _firebaseService.GetUrlByObjectName(objectName);

                    var fileResponseDto = new FilesResponseDto
                    {
                        FileName = fileEntity.FileName,
                        ContentType = file.ContentType,
                        URL = url
                    };

                    _student.AdditionalFiles.Add(fileEntity);
                    data.AdditionalFiles?.Add(fileResponseDto);
                }
            }

            await _context.SaveChangesAsync();
            await AddStudentFireStoreAsync(_student);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int id)
        {
            var response = new ServiceResponse<List<StudentResponseDto>>();

            var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"Student with ID '{id}' not found.");

            _context.Students.Remove(dbStudent);

            if (dbStudent.Parent != null)
            {
                var dbParent = await _context.Parents.FirstOrDefaultAsync(p => p.StudentId == id) ?? throw new NotFoundException("Parent not found.");
                _context.Parents.Remove(dbParent);
            }

            if (dbStudent.Address != null)
            {
                var dbAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == id) ?? throw new NotFoundException("Address not found.");
                _context.Addresses.Remove(dbAddress);
            }

            var dbAdditionalFiles = await _context.StudentAdditionalFiles.Where(f => f.StudentId == id).ToListAsync() ?? throw new NotFoundException($"No additional files found.");
            _context.StudentAdditionalFiles.RemoveRange(dbAdditionalFiles);

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _context.Students.Select(s => _mapper.Map<StudentResponseDto>(s)).ToList();

            return response;
        }

        public async Task<ServiceResponse<List<StudentResponseDto>>> GetStudent()
        {
            var response = new ServiceResponse<List<StudentResponseDto>>();

            var dbStudents = await _context.Students
                .Include(s => s.Parent)
                .Include(s => s.Address)
                .ToListAsync() ?? throw new NotFoundException("No students found.");

            var data = dbStudents.Select(s =>
            {
                var studentDto = _mapper.Map<StudentResponseDto>(s);
                studentDto.StudentId = s.Id;
                studentDto.DOB = s.DOB.ToDateString();
                return studentDto;
            }).ToList();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> GetStudentByStudentId(string studentCode)
        {
            var response = new ServiceResponse<StudentResponseDto>();

            var dbStudent = await _context.Students
                .Include(s => s.ProfilePicture)
                .Include(s => s.Parent)
                .Include(s => s.Address)
                .Include(s => s.AdditionalFiles)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode) ?? throw new NotFoundException($"Student with ID '{studentCode}' not found.");

            var data = _mapper.Map<StudentResponseDto>(dbStudent);

            data.StudentId = dbStudent.Id;
            data.DOB = dbStudent.DOB.ToDateString();

            if (dbStudent.ProfilePicture != null)
            {
                string objectName = dbStudent.ProfilePicture.ObjectName;

                var objectMetaData = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);
                string url = await _firebaseService.GetUrlByObjectName(objectName);
                ulong? size = objectMetaData.Size;

                var pictureResponseDto = new FilesResponseDto
                {
                    FileName = dbStudent.ProfilePicture.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                };

                data.ProfilePicture = pictureResponseDto;
            }

            if (dbStudent.AdditionalFiles != null && dbStudent.AdditionalFiles.Count != 0)
            {
                data.AdditionalFiles = new List<FilesResponseDto>();

                foreach (var file in dbStudent.AdditionalFiles)
                {
                    string objectName = file.ObjectName;

                    var objectMetaData = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);
                    string url = await _firebaseService.GetUrlByObjectName(objectName);
                    ulong? size = objectMetaData.Size;

                    var fileResponseDto = new FilesResponseDto
                    {
                        FileName = file.FileName,
                        ContentType = objectMetaData.ContentType,
                        URL = url
                    };

                    data.AdditionalFiles.Add(fileResponseDto);
                }
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> GetStudentByToken()
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<StudentResponseDto>();

            var dbStudent = await _context.Students
                .Include(s => s.Parent)
                .Include(s => s.Address)
                .Include(s => s.AdditionalFiles)
                .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"Student with ID '{id}' not found.");

            var data = _mapper.Map<StudentResponseDto>(dbStudent);

            data.StudentId = dbStudent.Id;
            data.DOB = dbStudent.DOB.ToDateString();

            if (dbStudent.ProfilePicture != null)
            {
                string objectName = dbStudent.ProfilePicture.ObjectName;

                var objectMetaData = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);
                string url = await _firebaseService.GetUrlByObjectName(objectName);
                ulong? size = objectMetaData.Size;

                var pictureResponseDto = new FilesResponseDto
                {
                    FileName = dbStudent.ProfilePicture.FileName,
                    ContentType = objectMetaData.ContentType,
                    URL = url
                };

                data.ProfilePicture = pictureResponseDto;
            }

            if (dbStudent.AdditionalFiles != null && dbStudent.AdditionalFiles.Count != 0)
            {
                data.AdditionalFiles = new List<FilesResponseDto>();

                foreach (var file in dbStudent.AdditionalFiles)
                {
                    string objectName = file.ObjectName;

                    var objectMetaData = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);
                    string url = await _firebaseService.GetUrlByObjectName(objectName);
                    ulong? size = objectMetaData.Size;

                    var fileResponseDto = new FilesResponseDto
                    {
                        FileName = file.FileName,
                        ContentType = objectMetaData.ContentType,
                        URL = url
                    };

                    data.AdditionalFiles.Add(fileResponseDto);
                }
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> UpdateStudent(UpdateStudentRequestDto updatedStudent, IFormFile? updatedProfilePicture, List<IFormFile>? filesToUpload)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var student = await _context.Students
                            .Include(s => s.ProfilePicture)
                            .Include(s => s.Parent)
                            .Include(s => s.Address)
                            .Include(s => s.AdditionalFiles)
                            .FirstOrDefaultAsync(s => s.StudentCode == updatedStudent.StudentCode) ?? throw new NotFoundException($"Student with ID '{updatedStudent.StudentCode}' not found.");

            var data = _mapper.Map<StudentResponseDto>(student);

            // Update student's information
            _mapper.Map(updatedStudent, student);

            student.Title = updatedStudent.Title;
            student.FirstName = updatedStudent.FirstName;
            student.LastName = updatedStudent.LastName;
            student.Nickname = updatedStudent.Nickname;
            student.DOB = updatedStudent.DOB.ToDateTime();
            student.Phone = updatedStudent.Phone;
            student.Line = updatedStudent.Line;
            student.Email = updatedStudent.Email;
            student.School = updatedStudent.School;
            student.CountryOfSchool = updatedStudent.CountryOfSchool;
            student.LevelOfStudy = updatedStudent.LevelOfStudy;
            student.Program = updatedStudent.Program;
            student.TargetUniversity = updatedStudent.TargetUniversity;
            student.TargetScore = updatedStudent.TargetScore;
            student.HogInformation = updatedStudent.HogInformation;
            student.HealthInformation = updatedStudent.HealthInformation;
            student.Remark = updatedStudent.Remark;
            student.LastUpdatedBy = id;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = updatedStudent.FirebaseId,
                Email = updatedStudent.Email
            });

            data.DOB = student.DOB.ToDateString();

            // Update parent's information
            if (updatedStudent.Parent != null)
            {
                var _parent = await _context.Parents.FirstOrDefaultAsync(p => p.Student != null && p.Student.StudentCode == updatedStudent.StudentCode);
                if (_parent is null)
                {
                    var parent = new Parent
                    {
                        FirstName = updatedStudent.Parent.FirstName,
                        LastName = updatedStudent.Parent.LastName,
                        Relationship = updatedStudent.Parent.Relationship,
                        Email = updatedStudent.Parent.Email,
                        Line = updatedStudent.Parent.Line,
                        Phone = updatedStudent.Parent.Phone,
                        Student = student
                    };
                    await _context.AddAsync(parent);
                }
                else
                {
                    _mapper.Map(updatedStudent.Parent, _parent);

                    _parent.FirstName = updatedStudent.Parent.FirstName;
                    _parent.LastName = updatedStudent.Parent.LastName;
                    _parent.Relationship = updatedStudent.Parent.Relationship;
                    _parent.Email = updatedStudent.Parent.Email;
                    _parent.Line = updatedStudent.Parent.Line;
                }

            }

            // Update address's information
            if (updatedStudent.Address != null)
            {
                var _address = await _context.Addresses.FirstOrDefaultAsync(a => a.Student != null && a.Student.StudentCode == updatedStudent.StudentCode);
                if (_address is null)
                {
                    var address = new Address
                    {
                        address = updatedStudent.Address.Address,
                        Subdistrict = updatedStudent.Address.Subdistrict,
                        District = updatedStudent.Address.District,
                        Province = updatedStudent.Address.Province,
                        Zipcode = updatedStudent.Address.Zipcode,
                        Student = student
                    };
                    await _context.AddAsync(address);
                }
                else
                {
                    _mapper.Map(updatedStudent.Address, _address);

                    _address.address = updatedStudent.Address.Address;
                    _address.Subdistrict = updatedStudent.Address.Subdistrict;
                    _address.District = updatedStudent.Address.District;
                    _address.Province = updatedStudent.Address.Province;
                    _address.Zipcode = updatedStudent.Address.Zipcode;
                }
            }

            // Update student's profile picture
            if (student.ProfilePicture != null)
            {
                if (updatedProfilePicture != null)
                {
                    var incomingFileName = updatedProfilePicture.FileName;
                    var oldProfilePicture = student.ProfilePicture;

                    var pictureRequestDto = new AddProfilePictureRequestDto
                    {
                        PictureData = updatedProfilePicture
                    };

                    var pictureEntity = _mapper.Map<ProfilePicture>(pictureRequestDto);
                    var fileName = updatedProfilePicture.FileName;
                    var objectName = $"students/{student.StudentCode}/profile/{fileName}";

                    using (var stream = pictureRequestDto.PictureData.OpenReadStream())
                    {
                        var storageObject = await _storageClient.UploadObjectAsync(
                            FIREBASE_BUCKET,
                            objectName,
                            updatedProfilePicture.ContentType,
                            stream
                        );

                        pictureEntity.FileName = fileName;
                        pictureEntity.ObjectName = objectName;
                    }

                    string url = await _firebaseService.GetUrlByObjectName(objectName);

                    var pictureResponseDto = new FilesResponseDto
                    {
                        FileName = pictureEntity.FileName,
                        ContentType = updatedProfilePicture.ContentType,
                        URL = url
                    };

                    if (incomingFileName != oldProfilePicture.FileName)
                    {
                        await _firebaseService.DeleteStorageFileByObjectName(oldProfilePicture.ObjectName);
                        _context.ProfilePictures.Remove(oldProfilePicture);
                        student.ProfilePicture = pictureEntity;
                    }
                    else
                    {
                        student.ProfilePicture.FileName = pictureEntity.FileName;
                        student.ProfilePicture.ObjectName = pictureEntity.ObjectName;
                    }

                    data.ProfilePicture = pictureResponseDto;
                }
            }
            else
            {
                if (updatedProfilePicture != null)
                {
                    var incomingFileName = updatedProfilePicture.FileName;

                    var pictureRequestDto = new AddProfilePictureRequestDto
                    {
                        PictureData = updatedProfilePicture
                    };

                    var pictureEntity = _mapper.Map<ProfilePicture>(pictureRequestDto);
                    var fileName = updatedProfilePicture.FileName;
                    var objectName = $"students/{student.StudentCode}/profile/{fileName}";

                    using (var stream = pictureRequestDto.PictureData.OpenReadStream())
                    {
                        var storageObject = await _storageClient.UploadObjectAsync(
                            FIREBASE_BUCKET,
                            objectName,
                            updatedProfilePicture.ContentType,
                            stream
                        );

                        pictureEntity.FileName = fileName;
                        pictureEntity.ObjectName = objectName;
                    }

                    string url = await _firebaseService.GetUrlByObjectName(objectName);

                    var pictureResponseDto = new FilesResponseDto
                    {
                        FileName = pictureEntity.FileName,
                        ContentType = updatedProfilePicture.ContentType,
                        URL = url
                    };

                    student.ProfilePicture = pictureEntity;

                    data.ProfilePicture = pictureResponseDto;
                }
            }

            // Update student additional files
            if (student.AdditionalFiles != null)
            {
                if (updatedStudent.FilesToDelete != null && updatedStudent.FilesToDelete.Count > 0)
                {
                    var firebaseFiles = student.AdditionalFiles.ToList();

                    foreach (var file in firebaseFiles)
                    {
                        foreach (var fileToDelete in updatedStudent.FilesToDelete)
                        {
                            try
                            {
                                if (fileToDelete == file.FileName)
                                {
                                    await _firebaseService.DeleteStorageFileByObjectName(file.ObjectName);
                                    _context.StudentAdditionalFiles.Remove(file);
                                }
                            }
                            catch
                            {
                                throw new NotFoundException($"No files called '{file}' found.");
                            }
                        }
                    }
                }

                if (filesToUpload != null && filesToUpload.Count > 0)
                {
                    foreach (var file in filesToUpload)
                    {

                        var fileRequestDto = new AddStudentAdditionalFilesRequestDto
                        {
                            FileData = file
                        };

                        var fileEntity = _mapper.Map<StudentAdditionalFile>(fileRequestDto);
                        var fileName = file.FileName;
                        var objectName = $"students/{student.StudentCode}/documents/{fileName}";

                        using (var stream = fileRequestDto.FileData.OpenReadStream())
                        {
                            var storageObject = await _storageClient.UploadObjectAsync(
                                FIREBASE_BUCKET,
                                objectName,
                                file.ContentType,
                                stream
                            );

                            fileEntity.FileName = fileName;
                            fileEntity.ObjectName = objectName;
                        }

                        var existingFile = student.AdditionalFiles?.FirstOrDefault(f => f.FileName == fileName);

                        if (existingFile != null)
                        {
                            // Update the existing file's properties
                            existingFile.FileName = fileEntity.FileName;
                            existingFile.ObjectName = fileEntity.ObjectName;
                        }
                        else
                        {
                            student.AdditionalFiles?.Add(fileEntity);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = data;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> DisableStudent(int id)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"Student with ID '{id}' not found.");

            student.Status = StudentStatus.Inactive;
            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = student.FirebaseId,
                Disabled = true
            });

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> EnableStudent(int id)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"Student with ID '{id}' not found.");

            student.Status = StudentStatus.Active;
            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = student.FirebaseId,
                Disabled = false
            });

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }

        public async Task<ServiceResponse<string>> ChangePasswordWithFirebaseId(string uid, ChangeUserPasswordDto password)
        {
            if (password.Password != password.VerifyPassword)
                throw new BadRequestException("Both Password must be the same");

            await _firebaseService.ChangePasswordWithUid(uid, password.Password);

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;

        }

        private async Task AddStudentFireStoreAsync(Student student)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(student.FirebaseId);
            Dictionary<string, object> studentDoc = new()
                {
                    { "displayName", student.FullName },
                    { "email", student.Email },
                    { "id", student.StudentCode! },
                    { "role", "student" },
                    { "uid", student.FirebaseId! }
                };
            await docRef.SetAsync(studentDoc);
        }
    }
}
