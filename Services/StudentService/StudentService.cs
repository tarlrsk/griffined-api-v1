using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

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

        public async Task<ServiceResponse<GetStudentDto>> AddStudent(AddStudentDto newStudent)
        {
            var response = new ServiceResponse<GetStudentDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            try
            {
                string password = newStudent.nickname.ToLower() +
                            DateTime.ParseExact(newStudent.dob, "dd-MMMM-yyyy HH:mm:ss", null).ToString("dd/MM/yyyy");

                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newStudent.email, password);
                var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
                string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

                var _student = _mapper.Map<Student>(newStudent);
                _student.firebaseId = firebaseId;
                _student.CreatedBy = id;
                _student.LastUpdatedBy = id;

                _context.Students.Add(_student);

                if (newStudent.additionalFiles != null)
                {
                    _student.additionalFiles = new List<StudentAdditionalFiles>();
                    foreach (var additionalFile in newStudent.additionalFiles)
                    {
                        var file = _mapper.Map<StudentAdditionalFiles>(additionalFile);
                        _student.additionalFiles.Add(file);
                    }
                }

                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetStudentDto>(_student);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }


            return response;
        }

        public async Task<ServiceResponse<List<GetStudentDto>>> DeleteStudent(int id)
        {
            var response = new ServiceResponse<List<GetStudentDto>>();
            try
            {
                var dbStudent = await _context.Students.FirstAsync(s => s.id == id);
                if (dbStudent is null)
                    throw new Exception($"Student with ID '{id}' not found.");

                _context.Students.Remove(dbStudent);

                if (dbStudent.parent != null)
                {
                    var dbParent = await _context.Parents.FirstAsync(p => p.studentId == id);
                    _context.Parents.Remove(dbParent);
                }

                if (dbStudent.address != null)
                {
                    var dbAddress = await _context.Addresses.FirstAsync(a => a.studentId == id);
                    _context.Addresses.Remove(dbAddress);
                }

                var dbAdditionalFiles = await _context.StudentAdditionalFiles.Where(f => f.studentId == id).ToListAsync();
                if (dbAdditionalFiles is null)
                    throw new Exception($"No additional files found.");
                _context.StudentAdditionalFiles.RemoveRange(dbAdditionalFiles);

                await _context.SaveChangesAsync();
                response.Data = _context.Students.Select(s => _mapper.Map<GetStudentDto>(s)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetStudentDto>>> GetStudent()
        {
            var response = new ServiceResponse<List<GetStudentDto>>();
            var dbStudents = await _context.Students
                .Include(s => s.parent)
                .Include(s => s.address)
                .Include(s => s.additionalFiles)
                .Select(s => _mapper.Map<GetStudentDto>(s))
                .ToListAsync();
            response.Data = dbStudents;
            return response;
        }

        public async Task<ServiceResponse<GetStudentDto>> GetStudentById(int id)
        {
            var response = new ServiceResponse<GetStudentDto>();
            try
            {
                var dbStudent = await _context.Students
                    .Include(s => s.parent)
                    .Include(s => s.address)
                    .Include(s => s.additionalFiles)
                    .FirstOrDefaultAsync(s => s.id == id);
                if (dbStudent is null)
                    throw new Exception($"Student with ID '{id}' not found.");
                response.Data = _mapper.Map<GetStudentDto>(dbStudent);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<GetStudentDto>> GetStudentByMe()
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<GetStudentDto>();
            try
            {
                var dbStudent = await _context.Students
                    .Include(s => s.parent)
                    .Include(s => s.address)
                    .Include(s => s.additionalFiles)
                    .FirstOrDefaultAsync(s => s.id == id);
                if (dbStudent is null)
                    throw new Exception($"Student with ID '{id}' not found.");
                response.Data = _mapper.Map<GetStudentDto>(dbStudent);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<GetStudentDto>> UpdateStudent(UpdateStudentDto updatedStudent)
        {
            var response = new ServiceResponse<GetStudentDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            try
            {
                var student = await _context.Students.Include(s => s.additionalFiles).FirstOrDefaultAsync(s => s.id == updatedStudent.id);
                if (student is null)
                    throw new Exception($"Student with ID '{updatedStudent.id}' not found.");

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
                    var existingFileIds = student.additionalFiles?.Select(f => f.id).ToList();
                    var updatedFileIds = updatedStudent.additionalFiles.Select(f => f.id).ToList();

                    // Remove any files that were not included in the updated DTO
                    var filesToRemove = student.additionalFiles?.Where(f => !updatedFileIds.Contains(f.id)).ToList();
                    if (filesToRemove != null)
                    {
                        foreach (var file in filesToRemove)
                        {
                            student.additionalFiles?.Remove(file);
                        }

                        // Update or add any files that were included in the updated DTO
                        foreach (var updatedFile in updatedStudent.additionalFiles)
                        {
                            var existingFile = student.additionalFiles?.FirstOrDefault(f => f.id == updatedFile.id);
                            if (existingFile is null)
                            {
                                existingFile = new StudentAdditionalFiles();
                                student.additionalFiles?.Add(existingFile);
                            }

                            _mapper.Map(updatedFile, existingFile);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetStudentDto>(student);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetStudentDto>> DisableStudent(int id)
        {
            var response = new ServiceResponse<GetStudentDto>();
            var student = await _context.Students.FirstAsync(s => s.id == id);
            if (student is null)
                throw new Exception($"Student with ID '{id}' not found.");

            student.status = _StudentStatusEnum.Inactive;
            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = student.firebaseId,
                Disabled = true
            });
            return response;
        }

        public async Task<ServiceResponse<GetStudentDto>> EnableStudent(int id)
        {
            var response = new ServiceResponse<GetStudentDto>();
            var student = await _context.Students.FirstAsync(s => s.id == id);
            if (student is null)
                throw new Exception($"Student with ID '{id}' not found.");

            student.status = _StudentStatusEnum.Active;
            await _context.SaveChangesAsync();

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
            {
                Uid = student.firebaseId,
                Disabled = false
            });
            return response;
        }
    }
}
