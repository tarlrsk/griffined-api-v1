using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;

namespace griffined_api.Services.TeacherService
{
    public class TeacherService : ITeacherService
    {
        private string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TeacherService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<GetTeacherDto>> AddTeacher(AddTeacherDto newTeacher)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            try
            {
                string password = "hog" + newTeacher.phone;
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(newTeacher.email, password);

                var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
                string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

                var teacher = _mapper.Map<Teacher>(newTeacher);
                teacher.firebaseId = firebaseId;
                teacher.CreatedBy = id;
                teacher.LastUpdatedBy = id;
                await addStaffFireStoreAsync(teacher);
                _context.Teachers.Add(teacher);

                if (newTeacher.workTimes != null)
                {
                    teacher.workTimes = new List<WorkTime>();
                    foreach (var workTime in newTeacher.workTimes)
                    {
                        var day = _mapper.Map<WorkTime>(workTime);
                        teacher.workTimes.Add(day);
                    }
                }

                await _context.SaveChangesAsync();
                await addStaffFireStoreAsync(teacher);

                response.Data = _mapper.Map<GetTeacherDto>(teacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherDto>>> DeleteTeacher(int id)
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();

            try
            {
                var dbTeacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstAsync(t => t.id == id);
                if (dbTeacher is null)
                    throw new Exception($"Teacher with ID {id} not found.");

                _context.Teachers.Remove(dbTeacher);
                _context.WorkTimes.RemoveRange(dbTeacher.workTimes);

                await _context.SaveChangesAsync();

                response.Data = _context.Teachers.Select(t => _mapper.Map<GetTeacherDto>(t)).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherCourseWithClassesDto>>> GetTeacherCourseWithClassesByTeacherId(int teacherId)
        {
            var response = new ServiceResponse<List<GetTeacherCourseWithClassesDto>>();
            try
            {
                var courses = await _context.PrivateCourses
                .Include(c => c.privateClasses)
                    .ThenInclude(pc => pc.studentPrivateClasses)!
                        .ThenInclude(spc => spc.student)
                .Include(c => c.privateClasses)
                    .ThenInclude(pc => pc.teacherPrivateClass)
                        .ThenInclude(tpc => tpc!.teacher)
                .Include(c => c.request)
                .Where(c => c.privateClasses.Any(pc => pc.teacherPrivateClass != null && pc.teacherPrivateClass.teacherId == teacherId))
                .ToListAsync();

                response.Data = courses.Select(c => new GetTeacherCourseWithClassesDto
                {
                    request = new GetPrivateRegistrationRequestDto
                    {
                        id = c.request.id,
                        section = c.request.section,
                        courseType = c.request.courseType,
                        status = c.request.status,
                        EAStatus = c.request.EAStatus,
                        paymentStatus = c.request.paymentStatus,
                        dateCreated = c.request.dateCreated,
                        EPRemark1 = c.request.EPRemark1,
                        EPRemark2 = c.request.EPRemark2,
                        EARemark = c.request.EARemark,
                        takenByEPId = c.request.takenByEAId ?? 0,
                        takenByEAId = c.request.takenByEAId ?? 0,
                        takenByOAId = c.request.takenByOAId ?? 0
                    },
                    course = new GetPrivateCourseDto
                    {
                        id = c.id,
                        course = c.course,
                        section = c.section,
                        subject = c.subject ?? "",
                        level = c.level ?? "",
                        method = c.method,
                        totalHour = c.totalHour,
                        hourPerClass = c.hourPerClass,
                        fromDate = c.fromDate,
                        toDate = c.toDate,
                        isActive = c.isActive
                    },
                    classes = c.privateClasses.Select(pc => new GetPrivateClassWithNameDto
                    {
                        id = pc.id,
                        room = pc.room,
                        method = pc.method,
                        date = pc.date,
                        fromTime = pc.fromTime,
                        toTime = pc.toTime,
                        studentPrivateClasses = pc.studentPrivateClasses?.Select(sp => new GetStudentPrivateClassWithNameDto
                        {
                            id = sp.id,
                            studentId = sp.studentId,
                            fullName = sp.student.fullName,
                            nickname = sp.student.nickname,
                            attendance = sp.attendance
                        }).ToList(),
                        teacherPrivateClass = pc.teacherPrivateClass == null ? null : new GetTeacherPrivateClassWithNameDto
                        {
                            id = pc.id,
                            teacherId = pc.teacherPrivateClass.teacherId,
                            fullName = pc.teacherPrivateClass.teacher.fullName,
                            nickname = pc.teacherPrivateClass.teacher.nickname,
                            status = pc.teacherPrivateClass.status,
                            workType = pc.teacherPrivateClass.workType
                        }
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherCourseWithClassesDto>>> GetTeacherCourseWithClassesByMe()
        {
            int teacherId = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<List<GetTeacherCourseWithClassesDto>>();
            try
            {
                var courses = await _context.PrivateCourses
                .Include(c => c.privateClasses)
                    .ThenInclude(pc => pc.studentPrivateClasses)!
                        .ThenInclude(spc => spc.student)
                .Include(c => c.privateClasses)
                    .ThenInclude(pc => pc.teacherPrivateClass)
                        .ThenInclude(tpc => tpc!.teacher)
                .Include(c => c.request)
                .Where(c => c.privateClasses.Any(pc => pc.teacherPrivateClass != null && pc.teacherPrivateClass.teacherId == teacherId))
                .ToListAsync();

                response.Data = courses.Select(c => new GetTeacherCourseWithClassesDto
                {
                    request = new GetPrivateRegistrationRequestDto
                    {
                        id = c.request.id,
                        section = c.request.section,
                        courseType = c.request.courseType,
                        status = c.request.status,
                        EAStatus = c.request.EAStatus,
                        paymentStatus = c.request.paymentStatus,
                        dateCreated = c.request.dateCreated,
                        EPRemark1 = c.request.EPRemark1,
                        EPRemark2 = c.request.EPRemark2,
                        EARemark = c.request.EARemark,
                        takenByEPId = c.request.takenByEAId ?? 0,
                        takenByEAId = c.request.takenByEAId ?? 0,
                        takenByOAId = c.request.takenByOAId ?? 0
                    },
                    course = new GetPrivateCourseDto
                    {
                        id = c.id,
                        course = c.course,
                        section = c.section,
                        subject = c.subject ?? "",
                        level = c.level ?? "",
                        method = c.method,
                        totalHour = c.totalHour,
                        hourPerClass = c.hourPerClass,
                        fromDate = c.fromDate,
                        toDate = c.toDate,
                        isActive = c.isActive
                    },
                    classes = c.privateClasses.Select(pc => new GetPrivateClassWithNameDto
                    {
                        id = pc.id,
                        room = pc.room,
                        method = pc.method,
                        date = pc.date,
                        fromTime = pc.fromTime,
                        toTime = pc.toTime,
                        studentPrivateClasses = pc.studentPrivateClasses?.Select(sp => new GetStudentPrivateClassWithNameDto
                        {
                            id = sp.id,
                            studentId = sp.studentId,
                            fullName = sp.student.fullName,
                            nickname = sp.student.nickname,
                            attendance = sp.attendance
                        }).ToList(),
                        teacherPrivateClass = pc.teacherPrivateClass == null ? null : new GetTeacherPrivateClassWithNameDto
                        {
                            id = pc.id,
                            teacherId = pc.teacherPrivateClass.teacherId,
                            fullName = pc.teacherPrivateClass.teacher.fullName,
                            nickname = pc.teacherPrivateClass.teacher.nickname,
                            status = pc.teacherPrivateClass.status,
                            workType = pc.teacherPrivateClass.workType
                        }
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherDto>>> GetTeacher()
        {
            var response = new ServiceResponse<List<GetTeacherDto>>();
            var dbTeachers = await _context.Teachers
                .Include(t => t.workTimes)
                .ToListAsync();
            response.Data = dbTeachers.Select(t => _mapper.Map<GetTeacherDto>(t)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> GetTeacherById(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var dbTeacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == id);
                if (dbTeacher is null)
                    throw new Exception($"Teacher with ID {id} not found.");
                response.Data = _mapper.Map<GetTeacherDto>(dbTeacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }



        public async Task<ServiceResponse<GetTeacherDto>> GetTeacherByMe()
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var dbTeacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == id);
                if (dbTeacher is null)
                    throw new Exception($"Teacher with ID {id} not found.");
                response.Data = _mapper.Map<GetTeacherDto>(dbTeacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> UpdateTeacher(UpdateTeacherDto updatedTeacher)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.workTimes)
                    .FirstOrDefaultAsync(t => t.id == updatedTeacher.id);
                if (teacher is null)
                    throw new Exception($"Teacher with ID {updatedTeacher.id} not found.");

                _mapper.Map(updatedTeacher, teacher);

                teacher.fName = updatedTeacher.fName;
                teacher.lName = updatedTeacher.lName;
                teacher.nickname = updatedTeacher.nickname;
                teacher.email = updatedTeacher.email;
                teacher.line = updatedTeacher.line;
                teacher.isActive = updatedTeacher.isActive;
                teacher.LastUpdatedBy = id;

                if (updatedTeacher.workTimes != null)
                {
                    teacher.workTimes.Clear();
                    foreach (var updatedWorkTime in updatedTeacher.workTimes)
                    {
                        var workTime = _mapper.Map<WorkTime>(updatedWorkTime);
                        teacher.workTimes.Add(workTime);
                    }
                }

                await _context.SaveChangesAsync();

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = teacher.firebaseId,
                    Email = updatedTeacher.email
                });

                await addStaffFireStoreAsync(teacher);

                response.Data = _mapper.Map<GetTeacherDto>(teacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // public Task<ServiceResponse<GetTeacherLeavingRequestDto>> UpdateTeacherLeavingRequest(UpdateTeacherLeavingRequestDto updatedRequest)
        // {
        //     throw new NotImplementedException();
        // }

        private async Task addStaffFireStoreAsync(Teacher staff)
        {
            FirestoreDb db = FirestoreDb.Create("myproject-f44ec");
            DocumentReference docRef = db.Collection("users").Document(staff.firebaseId);
            Dictionary<string, object> staffDoc = new Dictionary<string, object>()
                {
                    { "displayName", staff.fullName },
                    { "email", staff.email },
                    { "id", staff.id },
                    { "role", "Teacher" },
                    { "uid", staff.firebaseId}

                };
            await docRef.SetAsync(staffDoc);
        }

        public async Task<ServiceResponse<GetStudentAttendanceDto>> GetStudentAttendanceByClassId(int classId)
        {
            var response = new ServiceResponse<GetStudentAttendanceDto>();
            var course = await _context.PrivateCourses
                .Include(c => c.privateClasses)
                    .ThenInclude(pc => pc.studentPrivateClasses)!
                        .ThenInclude(spc => spc.student)
                .Include(c => c.privateClasses)
                    .ThenInclude(pc => pc.teacherPrivateClass)
                        .ThenInclude(tpc => tpc!.teacher)
                .FirstOrDefaultAsync(c => c.privateClasses.Any(pc => pc.id == classId));

            if (course is null)
                throw new Exception($"Course with class ID {classId} not found.");

            response.Data = new GetStudentAttendanceDto
            {
                course = new GetPrivateCourseDto
                {
                    id = course.id,
                    course = course.course,
                    section = course.section,
                    subject = course.subject ?? "",
                    level = course.level ?? "",
                    method = course.method,
                    totalHour = course.totalHour,
                    hourPerClass = course.hourPerClass,
                    fromDate = course.fromDate,
                    toDate = course.toDate
                },
                cls = new GetPrivateClassWithNameDto
                {
                    id = classId,
                    room = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.room ?? " ",
                    method = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.method ?? StudyMethod.onsite,
                    date = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.date ?? " ",
                    fromTime = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.fromTime ?? " ",
                    toTime = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.toTime ?? " ",
                    studentPrivateClasses = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.studentPrivateClasses?
                    .Select(spc => new GetStudentPrivateClassWithNameDto
                    {
                        id = spc.id,
                        studentId = spc.studentId,
                        fullName = spc.student.fullName,
                        nickname = spc.student.nickname,
                        attendance = spc.attendance
                    }).ToList() ?? new List<GetStudentPrivateClassWithNameDto>(),
                    teacherPrivateClass = new GetTeacherPrivateClassWithNameDto
                    {
                        id = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.teacherPrivateClass?.id ?? 0,
                        teacherId = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.teacherPrivateClass?.teacherId ?? 0,
                        fullName = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.teacherPrivateClass?.teacher.fullName ?? " ",
                        nickname = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.teacherPrivateClass?.teacher.nickname ?? " ",
                        status = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.teacherPrivateClass?.status ?? TeacherClassStatus.Incomplete,
                        workType = course.privateClasses.FirstOrDefault(pc => pc.id == classId)?.teacherPrivateClass?.workType ?? " "
                    }
                }
            };
            return response;
        }

        public async Task<ServiceResponse<GetStudentPrivateClassDto>> UpdateStudentAttendance(UpdateStudentPrivateClassDto updatedStudentAttendance)
        {
            var response = new ServiceResponse<GetStudentPrivateClassDto>();
            try
            {
                var studentPrivateClass = await _context.StudentPrivateClasses
                    .Include(spc => spc.student)
                    .FirstOrDefaultAsync(spc => spc.id == updatedStudentAttendance.id);

                if (studentPrivateClass is null)
                    throw new Exception($"Student private class with ID {updatedStudentAttendance.id} not found.");

                _mapper.Map(updatedStudentAttendance, studentPrivateClass);

                studentPrivateClass.attendance = updatedStudentAttendance.attendance;

                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetStudentPrivateClassDto>(studentPrivateClass);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherPrivateClassDto>> UpdateClassStatus(UpdateTeacherPrivateClassDto updatedClassStatus)
        {
            var response = new ServiceResponse<GetTeacherPrivateClassDto>();
            try
            {
                var teacherPrivateClass = await _context.TeacherPrivateClasses
                    .Include(tpc => tpc.teacher)
                    .FirstOrDefaultAsync(tpc => tpc.id == updatedClassStatus.id);

                if (teacherPrivateClass is null)
                    throw new Exception($"Teacher with private class ID {updatedClassStatus.id} not found.");

                _mapper.Map(updatedClassStatus, teacherPrivateClass);

                teacherPrivateClass.status = updatedClassStatus.status;

                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetTeacherPrivateClassDto>(teacherPrivateClass);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetTeacherWithCourseCountDto>>> GetTeacherWithCourseCount()
        {
            var response = new ServiceResponse<List<GetTeacherWithCourseCountDto>>();
            try
            {
                var teachers = await _context.Teachers
                    .Include(t => t.privateClasses)!
                        .ThenInclude(pc => pc.privateClass)
                            .ThenInclude(pc => pc.privateCourse)
                    .ToListAsync();

                var teacherWithCourseCount = teachers.Select(t => new GetTeacherWithCourseCountDto
                {
                    id = t.id,
                    fullName = t.fullName,
                    nickname = t.nickname,
                    courseCount = t.privateClasses!
                        .Select(tpc => tpc.privateClass.privateCourse)
                        .Where(c => c?.isActive == true && c?.request.status != RegistrationRequestStatus.Reject)
                        .Distinct()
                        .Count()
                }).ToList();

                response.Data = teacherWithCourseCount;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> DisableTeacher(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var staff = await _context.Teachers.FirstOrDefaultAsync(o => o.id == id);
                if (staff is null)
                    throw new Exception($"Teacher with ID '{id}' not found.");

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = staff.firebaseId,
                    Disabled = true
                });


                staff.isActive = false;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetTeacherDto>> EnableTeacher(int id)
        {
            var response = new ServiceResponse<GetTeacherDto>();
            try
            {
                var staff = await _context.Teachers.FirstOrDefaultAsync(o => o.id == id);
                if (staff is null)
                    throw new Exception($"Teacher with ID '{id}' not found.");

                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = staff.firebaseId,
                    Disabled = false
                });


                staff.isActive = true;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

    }
}
