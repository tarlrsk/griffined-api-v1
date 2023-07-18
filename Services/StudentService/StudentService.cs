using Firebase.Auth;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace griffined_api.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private string? FIREBASE_BUCKET = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
        private string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageClient _storageClient;

        public StudentService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, StorageClient storageClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            _storageClient = storageClient;
        }

        public async Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent, IFormFile newProfilePicture, ICollection<IFormFile> newFiles)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = newStudent.Nickname.ToLower() +
                        DateTime.ParseExact(newStudent.DOB, "dd-MMMM-yyyy HH:mm:ss", null).ToString("dd/MM/yyyy");

            FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
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

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            var _student = _mapper.Map<Student>(newStudent);
            _student.FirebaseId = firebaseId;
            _student.CreatedBy = id;
            _student.LastUpdatedBy = id;

            _context.Students.Add(_student);

            await _context.SaveChangesAsync();

            string studentCode = DateTime.Now.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (_student.Id % 10000).ToString("0000");

            _student.StudentCode = studentCode;

            if (newProfilePicture != null)
            {
                _student.ProfilePicture = new ProfilePicture();

                var pictureRequestDto = new AddProfilePictureRequestDto
                {
                    PictureData = newProfilePicture
                };

                var pictureEntity = _mapper.Map<ProfilePicture>(pictureRequestDto);
                var fileName = newProfilePicture.FileName;

                using (var stream = pictureRequestDto.PictureData.OpenReadStream())
                {
                    var storageObject = await _storageClient.UploadObjectAsync(
                        FIREBASE_BUCKET,
                        $"students/{studentCode}/profile/{fileName}",
                        null,
                        stream
                    );

                    pictureEntity.FileName = fileName;
                    pictureEntity.URL = storageObject.MediaLink;
                }
                _student.ProfilePicture = pictureEntity;
            }

            if (newFiles != null && newFiles.Count() > 0)
            {
                _student.AdditionalFiles = new List<StudentAdditionalFile>();

                foreach (var file in newFiles)
                {
                    var fileRequestDto = new AddStudentAdditionalFilesRequestDto
                    {
                        FileData = file
                    };

                    var fileEntity = _mapper.Map<StudentAdditionalFile>(fileRequestDto);
                    var fileName = file.FileName;

                    using (var stream = fileRequestDto.FileData.OpenReadStream())
                    {
                        var storageObject = await _storageClient.UploadObjectAsync(
                            FIREBASE_BUCKET,
                            $"students/{studentCode}/documents/{fileName}",
                            null,
                            stream
                        );

                        fileEntity.FileName = fileName;
                        fileEntity.URL = storageObject.MediaLink;
                    }
                    _student.AdditionalFiles.Add(fileEntity);
                }
            }

            await _context.SaveChangesAsync();
            await AddStudentFireStoreAsync(_student);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(_student);

            return response;
        }

        public async Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int id)
        {
            var response = new ServiceResponse<List<StudentResponseDto>>();

            var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

            _context.Students.Remove(dbStudent);

            if (dbStudent.Parent != null)
            {
                var dbParent = await _context.Parents.FirstOrDefaultAsync(p => p.StudentId == id);
                if (dbParent is null)
                    throw new NotFoundException("Parent not found.");
                _context.Parents.Remove(dbParent);
            }

            if (dbStudent.Address != null)
            {
                var dbAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == id);
                if (dbAddress is null)
                    throw new NotFoundException("Address not found.");
                _context.Addresses.Remove(dbAddress);
            }

            var dbAdditionalFiles = await _context.StudentAdditionalFiles.Where(f => f.StudentId == id).ToListAsync();
            if (dbAdditionalFiles is null)
                throw new NotFoundException($"No additional files found.");
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
                .Include(s => s.AdditionalFiles)
                .Select(s => _mapper.Map<StudentResponseDto>(s))
                .ToListAsync();

            if (dbStudents is null)
                throw new NotFoundException("No students found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = dbStudents;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> GetStudentByStudentId(string studentCode)
        {
            var response = new ServiceResponse<StudentResponseDto>();

            var dbStudent = await _context.Students
                .Include(s => s.Parent)
                .Include(s => s.Address)
                .Include(s => s.AdditionalFiles)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{studentCode}' not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(dbStudent);

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
                .FirstOrDefaultAsync(s => s.Id == id);

            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(dbStudent);

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> UpdateStudent(UpdateStudentRequestDto updatedStudent, IFormFile updatedProfilePicture, ICollection<IFormFile> updatedFiles)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var student = await _context.Students
                            .Include(s => s.ProfilePicture)
                            .Include(s => s.AdditionalFiles)
                            .FirstOrDefaultAsync(s => s.Id == updatedStudent.Id);
            if (student is null)
                throw new NotFoundException($"Student with ID '{updatedStudent.Id}' not found.");

            // Update the student entity
            _mapper.Map(updatedStudent, student);

            student.Title = updatedStudent.Title;
            student.FirstName = updatedStudent.FirstName;
            student.LastName = updatedStudent.LastName;
            student.Nickname = updatedStudent.Nickname;
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
            student.LastUpdatedBy = id;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = updatedStudent.FirebaseId,
                Email = updatedStudent.Email
            });

            if (updatedStudent.Parent != null)
            {
                var _parent = await _context.Parents.FirstOrDefaultAsync(p => p.StudentId == updatedStudent.Id);
                if (_parent is null)
                {
                    var parent = new Parent();
                    parent.FirstName = updatedStudent.Parent.FirstName;
                    parent.LastName = updatedStudent.Parent.LastName;
                    parent.Relationship = updatedStudent.Parent.Relationship;
                    parent.Email = updatedStudent.Parent.Email;
                    parent.Line = updatedStudent.Parent.Line;
                    parent.Phone = updatedStudent.Parent.Phone;
                    parent.Student = student;
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

            if (updatedStudent.Address != null)
            {
                var _address = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == updatedStudent.Id);
                if (_address is null)
                {
                    var address = new Address();
                    address.address = updatedStudent.Address.Address;
                    address.Subdistrict = updatedStudent.Address.Subdistrict;
                    address.District = updatedStudent.Address.District;
                    address.Province = updatedStudent.Address.Province;
                    address.Zipcode = updatedStudent.Address.Zipcode;
                    address.Student = student;
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

            // if (updatedProfilePicture != null)
            // {
            //     var fileName = Path.GetFileName(updatedProfilePicture.FileName);

            //     using (var stream = updatedProfilePicture.OpenReadStream())
            //     {
            //         var storageObject = await _storageClient.UploadObjectAsync(
            //             FIREBASE_BUCKET,
            //             $"students/{student.StudentCode}/profile/{fileName}",
            //             null,
            //             stream
            //         );

            //         if (student.ProfilePicture != null)
            //             student.ProfilePicture.URL = storageObject.MediaLink;
            //     }
            // }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(student);

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> DisableStudent(int id)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

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
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

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

        private async Task AddStudentFireStoreAsync(Student student)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(student.FirebaseId);
            Dictionary<string, object> studentDoc = new Dictionary<string, object>()
                {
                    { "displayName", student.FullName },
                    { "email", student.Email },
                    { "id", student.StudentCode },
                    { "role", "student" },
                    { "uid", student.FirebaseId }
                };
            await docRef.SetAsync(studentDoc);
        }

        private async Task DeleteFileFromStorage(string fileUrl)
        {
            var storageObject = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, fileUrl, null);

            if (storageObject != null)
            {
                await _storageClient.DeleteObjectAsync(storageObject);
            }
        }
    }
}
