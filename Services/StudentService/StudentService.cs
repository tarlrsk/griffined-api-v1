using Firebase.Auth;
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
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<StudentResponseDto>> AddStudent(AddStudentRequestDto newStudent)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            string password = newStudent.nickname.ToLower() +
                        DateTime.ParseExact(newStudent.dob, "dd-MMMM-yyyy HH:mm:ss", null).ToString("dd/MM/yyyy");

            FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));

            FirebaseAuthLink firebaseAuthLink;

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newStudent.email, password);
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
            _student.firebaseId = firebaseId;
            _student.CreatedBy = id;
            _student.LastUpdatedBy = id;

            _context.Students.Add(_student);

            if (newStudent.additionalFiles != null)
            {
                _student.additionalFiles = new List<StudentAdditionalFile>();
                foreach (var additionalFile in newStudent.additionalFiles)
                {
                    var file = _mapper.Map<StudentAdditionalFile>(additionalFile);
                    _student.additionalFiles.Add(file);
                }
            }

            await _context.SaveChangesAsync();

            string studentId = DateTime.Now.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (_student.id % 10000).ToString("0000");

            _student.studentId = studentId;

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(_student);

            return response;
        }

        public async Task<ServiceResponse<List<StudentResponseDto>>> DeleteStudent(int id)
        {
            var response = new ServiceResponse<List<StudentResponseDto>>();

            var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.id == id);
            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

            _context.Students.Remove(dbStudent);

            if (dbStudent.parent != null)
            {
                var dbParent = await _context.Parents.FirstOrDefaultAsync(p => p.studentId == id);
                if (dbParent is null)
                    throw new NotFoundException("Parent not found.");
                _context.Parents.Remove(dbParent);
            }

            if (dbStudent.address != null)
            {
                var dbAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.studentId == id);
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
                .Include(s => s.parent)
                .Include(s => s.address)
                .Include(s => s.additionalFiles)
                .Select(s => _mapper.Map<StudentResponseDto>(s))
                .ToListAsync();

            if (dbStudents is null)
                throw new NotFoundException("No students found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = dbStudents;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> GetStudentByStudentId(string studentId)
        {
            var response = new ServiceResponse<StudentResponseDto>();

            var dbStudent = await _context.Students
                .Include(s => s.parent)
                .Include(s => s.address)
                .Include(s => s.additionalFiles)
                .FirstOrDefaultAsync(s => s.studentId == studentId);
            if (dbStudent is null)
                throw new NotFoundException($"Student with ID '{studentId}' not found.");

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(dbStudent);

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> GetStudentByToken()
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<StudentResponseDto>();

            var dbStudent = await _context.Students
                .Include(s => s.parent)
                .Include(s => s.address)
                .Include(s => s.additionalFiles)
                .FirstOrDefaultAsync(s => s.id == id);

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

            var student = await _context.Students.Include(s => s.additionalFiles).FirstOrDefaultAsync(s => s.id == updatedStudent.id);
            if (student is null)
                throw new NotFoundException($"Student with ID '{updatedStudent.id}' not found.");

            // Update the student entity
            _mapper.Map(updatedStudent, student);

            student.title = updatedStudent.title;
            student.fName = updatedStudent.fName;
            student.lName = updatedStudent.lName;
            student.nickname = updatedStudent.nickname;
            student.profilePicture = updatedStudent.profilePicture;
            student.phone = updatedStudent.phone;
            student.line = updatedStudent.line;
            student.email = updatedStudent.email;
            student.school = updatedStudent.school;
            student.countryOfSchool = updatedStudent.countryOfSchool;
            student.levelOfStudy = updatedStudent.levelOfStudy;
            student.program = updatedStudent.program;
            student.targetUni = updatedStudent.targetUni;
            student.targetScore = updatedStudent.targetScore;
            student.hogInfo = updatedStudent.hogInfo;
            student.healthInfo = updatedStudent.healthInfo;
            student.LastUpdatedBy = id;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = updatedStudent.firebaseId,
                Email = updatedStudent.email
            });

            if (updatedStudent.parent != null)
            {
                var _parent = await _context.Parents.FirstOrDefaultAsync(p => p.studentId == updatedStudent.id);
                if (_parent is null)
                {
                    var parent = new Parent();
                    parent.fName = updatedStudent.parent.fName;
                    parent.lName = updatedStudent.parent.lName;
                    parent.relationship = updatedStudent.parent.relationship;
                    parent.email = updatedStudent.parent.email;
                    parent.line = updatedStudent.parent.line;
                    parent.phone = updatedStudent.parent.phone;
                    parent.student = student;
                    await _context.AddAsync(parent);
                }
                else
                {
                    _mapper.Map(updatedStudent.parent, _parent);

                    _parent.fName = updatedStudent.parent.fName;
                    _parent.lName = updatedStudent.parent.lName;
                    _parent.relationship = updatedStudent.parent.relationship;
                    _parent.email = updatedStudent.parent.email;
                    _parent.line = updatedStudent.parent.line;
                }

            }

            if (updatedStudent.address != null)
            {
                var _address = await _context.Addresses.FirstOrDefaultAsync(a => a.studentId == updatedStudent.id);
                if (_address is null)
                {
                    var address = new Address();
                    address.address = updatedStudent.address.address;
                    address.subdistrict = updatedStudent.address.subdistrict;
                    address.district = updatedStudent.address.district;
                    address.province = updatedStudent.address.province;
                    address.zipcode = updatedStudent.address.zipcode;
                    address.student = student;
                    await _context.AddAsync(address);
                }
                else
                {
                    _mapper.Map(updatedStudent.address, _address);

                    _address.address = updatedStudent.address.address;
                    _address.subdistrict = updatedStudent.address.subdistrict;
                    _address.district = updatedStudent.address.district;
                    _address.province = updatedStudent.address.province;
                    _address.zipcode = updatedStudent.address.zipcode;
                }
            }

            if (updatedStudent.additionalFiles is not null)
            {
                var existingFileIds = student.additionalFiles?.Select(f => f.Id).ToList();
                var updatedFileIds = updatedStudent.additionalFiles.Select(f => f.id).ToList();

                // Remove any files that were not included in the updated DTO
                var filesToRemove = student.additionalFiles?.Where(f => !updatedFileIds.Contains(f.Id)).ToList();
                if (filesToRemove != null)
                {
                    foreach (var file in filesToRemove)
                    {
                        student.additionalFiles?.Remove(file);
                    }

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
                }
            }

            await _context.SaveChangesAsync();

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = _mapper.Map<StudentResponseDto>(student);

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> DisableStudent(int id)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.id == id);

            if (student is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

            student.status = StudentStatus.Inactive;
            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = student.firebaseId,
                Disabled = true
            });

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }

        public async Task<ServiceResponse<StudentResponseDto>> EnableStudent(int id)
        {
            var response = new ServiceResponse<StudentResponseDto>();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.id == id);

            if (student is null)
                throw new NotFoundException($"Student with ID '{id}' not found.");

            student.status = StudentStatus.Active;
            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = student.firebaseId,
                Disabled = false
            });

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }
    }
}
