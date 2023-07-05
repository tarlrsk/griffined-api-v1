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

        public async Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent, ICollection<IFormFile> files)
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

            string studentCode = DateTime.Now.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (_student.StudentId % 10000).ToString("0000");

            _student.StudentCode = studentCode;

            // if (newStudent.additionalFiles != null && newStudent.additionalFiles.Count > 0)
            // {
            //     _student.additionalFiles = new List<StudentAdditionalFile>();

            //     foreach (var fileRequestDto in newStudent.additionalFiles)
            //     {
            //         var file = _mapper.Map<StudentAdditionalFile>(fileRequestDto);
            //         var fileName = Path.GetFileName(fileRequestDto.FileData.Name);

            //         using (var stream = fileRequestDto.FileData.OpenReadStream())
            //         {
            //             var storageObject = await _storageClient.UploadObjectAsync(
            //                 FIREBASE_BUCKET,
            //                 $"students/{studentId}/{fileName}",
            //                 null,
            //                 stream
            //             );

            //             file.URL = storageObject.MediaLink;
            //         }

            //         _student.additionalFiles.Add(file);
            //     }
            // }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(_student);

            return response;
        }

        public async Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int StudentId)
        {
            var response = new ServiceResponse<List<StudentResponseDto>>();

            var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == StudentId);
            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{StudentId}' not found.");

            _context.Students.Remove(dbStudent);

            if (dbStudent.Parent != null)
            {
                var dbParent = await _context.Parents.FirstOrDefaultAsync(p => p.StudentId == StudentId);
                if (dbParent is null)
                    throw new NotFoundException("Parent not found.");
                _context.Parents.Remove(dbParent);
            }

            if (dbStudent.Address != null)
            {
                var dbAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == StudentId);
                if (dbAddress is null)
                    throw new NotFoundException("Address not found.");
                _context.Addresses.Remove(dbAddress);
            }

            var dbAdditionalFiles = await _context.StudentAdditionalFiles.Where(f => f.StudentId == StudentId).ToListAsync();
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
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(dbStudent);

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> UpdateStudent(UpdateStudentRequestDto updatedStudent)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            var student = await _context.Students.Include(s => s.AdditionalFiles).FirstOrDefaultAsync(s => s.StudentId == updatedStudent.StudentId);
            if (student is null)
                throw new NotFoundException($"Student with ID '{updatedStudent.StudentId}' not found.");

            // Update the student entity
            _mapper.Map(updatedStudent, student);

            student.Title = updatedStudent.Title;
            student.FirstName = updatedStudent.FirstName;
            student.LastName = updatedStudent.LastName;
            student.Nickname = updatedStudent.Nickname;
            student.ProfilePicture = updatedStudent.ProfilePicture;
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
                var _parent = await _context.Parents.FirstOrDefaultAsync(p => p.StudentId == updatedStudent.StudentId);
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
                var _address = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == updatedStudent.StudentId);
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

            // if (updatedStudent.AdditionalFiles is not null)
            // {
            //     var existingFileIds = student.AdditionalFiles?.Select(f => f.Id).ToList();
            //     var updatedFileIds = updatedStudent.AdditionalFiles.Select(f => f.StudentId).ToList();

            //     // Remove any files that were not included in the updated DTO
            //     var filesToRemove = student.AdditionalFiles?.Where(f => !updatedFileIds.Contains(f.Id)).ToList();
            //     if (filesToRemove != null)
            //     {
            //         foreach (var file in filesToRemove)
            //         {
            //             student.additionalFiles?.Remove(file);
            //         }

            // // Update or add any files that were included in the updated DTO
            // foreach (var updatedFile in updatedStudent.additionalFiles)
            // {
            //     var existingFile = student.additionalFiles?.FirstOrDefault(f => f.id == updatedFile.id);
            //     if (existingFile is null)
            //     {
            //         existingFile = new StudentAdditionalFile();
            //         student.additionalFiles?.Add(existingFile);
            //     }

            //     _mapper.Map(updatedFile, existingFile);
            // }
            //     }
            // }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(student);

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> DisableStudent(int studentId)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student is null)
                throw new NotFoundException($"Student with ID '{studentId}' not found.");

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

        public async Task<ServiceResponse<StudentResponseDto>> EnableStudent(int studentId)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student is null)
                throw new NotFoundException($"Student with ID '{studentId}' not found.");

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
    }
}
