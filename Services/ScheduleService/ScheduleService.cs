using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ScheduleService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetScheduleDto>> AddSchedule(AddScheduleDto newSchedule)
        {
            var response = new ServiceResponse<GetScheduleDto>();
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");

            try
            {
                var course = _mapper.Map<PrivateCourse>(newSchedule.course);

                var request = await _context.PrivateRegistrationRequests.FindAsync(newSchedule.requestId);
                if (request is null)
                    throw new Exception($"Request with ID {newSchedule.requestId} not found.");

                course.request = request;
                course.CreatedBy = id;
                course.LastUpdatedBy = id;

                foreach (var cls in newSchedule.classes)
                {
                    var privateClass = _mapper.Map<PrivateClass>(cls);
                    privateClass.privateCourse = course;
                    privateClass.CreatedBy = id;
                    privateClass.LastUpdatedBy = id;
                    privateClass.studentPrivateClasses = null;
                    privateClass.teacherPrivateClass = null;

                    foreach (var spc in cls.studentPrivateClasses)
                    {
                        var student = await _context.Students.FindAsync(spc.studentId);
                        if (student is null)
                            throw new Exception($"Student with ID {spc.studentId} not found.");
                        var studentPrivateClass = _mapper.Map<StudentPrivateClass>(spc);
                        studentPrivateClass.privateClass = privateClass;
                        studentPrivateClass.studentId = student.id;
                        studentPrivateClass.student = student;
                        _context.StudentPrivateClasses.Add(studentPrivateClass);
                    }

                    var tpc = cls.teacherPrivateClass;
                    var teacher = await _context.Teachers.FindAsync(tpc.teacherId);
                    if (teacher is null)
                        throw new Exception($"Teacher with ID {tpc.teacherId} not found.");
                    var teacherPrivateClass = _mapper.Map<TeacherPrivateClass>(tpc);
                    teacherPrivateClass.privateClass = privateClass;
                    teacherPrivateClass.teacherId = teacher.id;
                    teacherPrivateClass.teacher = teacher;
                    _context.TeacherPrivateClasses.Add(teacherPrivateClass);

                    _context.PrivateClasses.Add(privateClass);
                }

                await _context.SaveChangesAsync();

                response.Data = new GetScheduleDto
                {
                    requestId = request.id,
                    requestStatus = request.status,
                    course = new GetPrivateCourseDto
                    {
                        id = course.id,
                        course = course.course,
                        subject = course.subject ?? "",
                        level = course.level ?? "",
                        section = course.section,
                        method = course.method,
                        totalHour = course.totalHour,
                        hourPerClass = course.hourPerClass,
                        fromDate = course.fromDate,
                        toDate = course.toDate
                    },
                    classes = course.privateClasses.Select(privateClass => new GetPrivateClassWithNameDto
                    {
                        id = privateClass.id,
                        room = privateClass.room,
                        method = privateClass.method,
                        date = privateClass.date,
                        fromTime = privateClass.fromTime,
                        toTime = privateClass.toTime,
                        studentPrivateClasses = privateClass.studentPrivateClasses?.Select(spc => new GetStudentPrivateClassWithNameDto
                        {
                            id = spc.id,
                            studentId = spc.studentId,
                            fullName = spc.student.fullName,
                            nickname = spc.student.nickname,
                            attendance = spc.attendance
                        }).ToList() ?? new List<GetStudentPrivateClassWithNameDto>(),
                        teacherPrivateClass = new GetTeacherPrivateClassWithNameDto
                        {
                            id = privateClass.teacherPrivateClass?.id ?? 0,
                            teacherId = privateClass.teacherPrivateClass?.teacherId ?? 0,
                            fullName = privateClass.teacherPrivateClass?.teacher.fullName ?? " ",
                            nickname = privateClass.teacherPrivateClass?.teacher.nickname ?? " ",
                            workType = privateClass.teacherPrivateClass?.workType ?? " ",
                            status = privateClass.teacherPrivateClass?.status ?? TeacherClassStatus.Incomplete
                        }
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetPrivateClassDto>>> DeletePrivateClass(int classId)
        {
            var response = new ServiceResponse<List<GetPrivateClassDto>>();
            try
            {
                await DeleteClass(classId);
                response.Data = new List<GetPrivateClassDto>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<List<GetPrivateClassDto>>> DeleteListOfPrivateClass([FromQuery] int[] listOfClassId)
        {
            var response = new ServiceResponse<List<GetPrivateClassDto>>();
            try
            {
                foreach (var classId in listOfClassId)
                {
                    await DeleteClass(classId);
                }
                response.Data = new List<GetPrivateClassDto>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetScheduleDto>>> DeleteSchedule(int courseId)
        {
            var response = new ServiceResponse<List<GetScheduleDto>>();
            try
            {
                var schedule = await _context.PrivateCourses
                    .Include(c => c.privateClasses)
                        .ThenInclude(pc => pc.studentPrivateClasses)
                            .ThenInclude(spc => spc.student)
                    .Include(c => c.privateClasses)
                        .ThenInclude(pc => pc.teacherPrivateClass)
                            .ThenInclude(tpc => tpc.teacher)
                    .FirstOrDefaultAsync(c => c.id == courseId);

                if (schedule is null)
                    throw new Exception($"Course with ID {courseId} not found.");

                foreach (var privateClass in schedule.privateClasses)
                {
                    if (privateClass.studentPrivateClasses != null)
                        _context.RemoveRange(privateClass.studentPrivateClasses);
                    if (privateClass.teacherPrivateClass != null)
                        _context.Remove(privateClass.teacherPrivateClass);
                }
                _context.RemoveRange(schedule.privateClasses);
                _context.Remove(schedule);

                await _context.SaveChangesAsync();

                response.Data = new List<GetScheduleDto>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetScheduleDto>>> SoftDeleteSchedule(int courseId)
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var response = new ServiceResponse<List<GetScheduleDto>>();
            try
            {
                var schedule = await _context.PrivateCourses
                    .Include(c => c.privateClasses)
                        .ThenInclude(pc => pc.studentPrivateClasses)
                            .ThenInclude(spc => spc.student)
                    .Include(c => c.privateClasses)
                        .ThenInclude(pc => pc.teacherPrivateClass)
                            .ThenInclude(tpc => tpc.teacher)
                    .FirstOrDefaultAsync(c => c.id == courseId);
                if (schedule is null)
                    throw new Exception($"Course with ID {courseId} not found.");

                schedule.LastUpdatedBy = id;
                schedule.isActive = false;

                foreach (var privateClass in schedule.privateClasses)
                {
                    privateClass.isActive = false;
                }

                await _context.SaveChangesAsync();

                response.Data = new List<GetScheduleDto>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetScheduleDto>>> GetSchedule()
        {
            var response = new ServiceResponse<List<GetScheduleDto>>();
            try
            {
                var schedules = await _context.PrivateCourses
                    .Where(c => c.isActive == true)
                    .Include(c => c.request)
                    .Include(c => c.privateClasses)
                        .ThenInclude(pc => pc.studentPrivateClasses)
                            .ThenInclude(spc => spc.student)
                    .Include(c => c.privateClasses)
                        .ThenInclude(pc => pc.teacherPrivateClass)
                            .ThenInclude(tpc => tpc.teacher)
                    .ToListAsync();

                response.Data = schedules.Select(s => new GetScheduleDto
                {
                    id = s.id,
                    requestId = s.request.id,
                    requestStatus = s.request.status,
                    course = new GetPrivateCourseDto
                    {
                        id = s.id,
                        course = s.course,
                        section = s.section,
                        subject = s.subject ?? "",
                        level = s.level ?? "",
                        method = s.method,
                        totalHour = s.totalHour,
                        hourPerClass = s.hourPerClass,
                        fromDate = s.fromDate,
                        toDate = s.toDate
                    },
                    classes = s.privateClasses.Select(c => new GetPrivateClassWithNameDto
                    {
                        id = c.id,
                        room = c.room,
                        method = c.method,
                        date = c.date,
                        fromTime = c.fromTime,
                        toTime = c.toTime,
                        studentPrivateClasses = c.studentPrivateClasses?.Select(sp => new GetStudentPrivateClassWithNameDto
                        {
                            id = sp.id,
                            studentId = sp.studentId,
                            fullName = sp.student.fullName,
                            nickname = sp.student.nickname,
                            attendance = sp.attendance
                        }).ToList() ?? new List<GetStudentPrivateClassWithNameDto>(),
                        teacherPrivateClass = c.teacherPrivateClass == null ? null : new GetTeacherPrivateClassWithNameDto
                        {
                            id = c.teacherPrivateClass.id,
                            teacherId = c.teacherPrivateClass.teacherId,
                            fullName = c.teacherPrivateClass.teacher.fullName,
                            nickname = c.teacherPrivateClass.teacher.nickname,
                            workType = c.teacherPrivateClass.workType,
                            status = c.teacherPrivateClass.status
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

        public async Task<ServiceResponse<List<GetScheduleDto>>> GetScheduleByRequestId(int requestId)
        {
            var response = new ServiceResponse<List<GetScheduleDto>>();
            try
            {
                var schedules = await _context.PrivateCourses
                    .Include(p => p.request)
                    .Include(p => p.privateClasses)
                        .ThenInclude(pc => pc.studentPrivateClasses)
                            .ThenInclude(spc => spc.student)
                    .Include(p => p.privateClasses)
                        .ThenInclude(pc => pc.teacherPrivateClass)
                            .ThenInclude(tpc => tpc.teacher)
                    .Where(p => p.request.id == requestId)
                    .ToListAsync();

                if (schedules is null)
                    throw new Exception($"Schedule with request ID {requestId} not found.");

                response.Data = schedules.Select(s => new GetScheduleDto
                {
                    id = s.id,
                    requestId = s.request.id,
                    requestStatus = s.request.status,
                    course = new GetPrivateCourseDto
                    {
                        id = s.id,
                        course = s.course,
                        section = s.section,
                        subject = s.subject ?? "",
                        level = s.level ?? "",
                        method = s.method,
                        totalHour = s.totalHour,
                        hourPerClass = s.hourPerClass,
                        fromDate = s.fromDate,
                        toDate = s.toDate
                    },
                    classes = s.privateClasses.Select(c => new GetPrivateClassWithNameDto
                    {
                        id = c.id,
                        room = c.room,
                        method = c.method,
                        date = c.date,
                        fromTime = c.fromTime,
                        toTime = c.toTime,
                        studentPrivateClasses = c.studentPrivateClasses?.Select(sp => new GetStudentPrivateClassWithNameDto
                        {
                            id = sp.id,
                            studentId = sp.studentId,
                            fullName = sp.student.fullName,
                            nickname = sp.student.nickname,
                            attendance = sp.attendance
                        }).ToList() ?? new List<GetStudentPrivateClassWithNameDto>(),
                        teacherPrivateClass = c.teacherPrivateClass == null ? null : new GetTeacherPrivateClassWithNameDto
                        {
                            id = c.teacherPrivateClass.id,
                            teacherId = c.teacherPrivateClass.teacherId,
                            fullName = c.teacherPrivateClass.teacher.fullName,
                            nickname = c.teacherPrivateClass.teacher.nickname,
                            workType = c.teacherPrivateClass.workType,
                            status = c.teacherPrivateClass.status
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

        public async Task<ServiceResponse<UpdatePrivateClassDto>> UpdatePrivateClass(UpdatePrivateClassDto updatedClass)
        {
            var response = new ServiceResponse<UpdatePrivateClassDto>();
            try
            {
                var result = await UpdateClass(updatedClass);
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<List<UpdatePrivateClassDto>>> UpdateListOfPrivateClass(List<UpdatePrivateClassDto> updatedClasses)
        {
            var response = new ServiceResponse<List<UpdatePrivateClassDto>>();
            try
            {
                foreach (var c in updatedClasses)
                {
                    var result = await UpdateClass(c);
                    response?.Data?.Add(result);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetSinglePrivateClassDto>> AddPrivateClass(AddSinglePrivateClassDto newClass)
        {
            var response = new ServiceResponse<GetSinglePrivateClassDto>();
            try
            {
                var result = await AddClass(newClass);
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetSinglePrivateClassDto>>> AddListOfPrivateClass(List<AddSinglePrivateClassDto> newClasses)
        {
            var response = new ServiceResponse<List<GetSinglePrivateClassDto>>();
            try
            {
                foreach (var c in newClasses)
                {
                    var result = await AddClass(c);
                    response?.Data?.Add(result);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }




        private async Task<GetSinglePrivateClassDto> AddClass(AddSinglePrivateClassDto newClass)
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var course = await _context.PrivateCourses.FindAsync(newClass.courseId);

            if (course is null)
                throw new Exception($"Course with ID {newClass.courseId} not found.");

            var privateClass = _mapper.Map<PrivateClass>(newClass.privateClass);
            privateClass.privateCourse = course;
            privateClass.CreatedBy = id;
            privateClass.LastUpdatedBy = id;

            privateClass.studentPrivateClasses = null;
            privateClass.teacherPrivateClass = null;

            foreach (var spc in newClass.privateClass.studentPrivateClasses)
            {
                var student = await _context.Students.FindAsync(spc.studentId);
                if (student is null)
                    throw new Exception($"Student with ID {spc.studentId} not found.");
                var studentPrivateClass = _mapper.Map<StudentPrivateClass>(spc);
                studentPrivateClass.privateClass = privateClass;
                studentPrivateClass.studentId = student.id;
                studentPrivateClass.student = student;
                _context.StudentPrivateClasses.Add(studentPrivateClass);
            }

            var tpc = newClass.privateClass.teacherPrivateClass;
            var teacher = await _context.Teachers.FindAsync(tpc.teacherId);
            if (teacher is null)
                throw new Exception($"Teacher with ID {tpc.teacherId} not found.");
            var teacherPrivateClass = _mapper.Map<TeacherPrivateClass>(tpc);
            teacherPrivateClass.privateClass = privateClass;
            teacherPrivateClass.teacherId = teacher.id;
            teacherPrivateClass.teacher = teacher;
            privateClass.teacherPrivateClass = teacherPrivateClass;
            _context.TeacherPrivateClasses.Add(teacherPrivateClass);

            _context.PrivateClasses.Add(privateClass);

            await _context.SaveChangesAsync();

            return new GetSinglePrivateClassDto
            {
                courseId = course.id,
                privateClass = new GetPrivateClassDto
                {
                    id = privateClass.id,
                    room = privateClass.room,
                    method = privateClass.method,
                    date = privateClass.date,
                    fromTime = privateClass.fromTime,
                    toTime = privateClass.toTime,
                    studentPrivateClasses = privateClass.studentPrivateClasses?.Select(spc => new GetStudentPrivateClassDto
                    {
                        id = spc.id,
                        studentId = spc.student.id,
                        attendance = spc.attendance
                    }).ToList(),
                    teacherPrivateClass = new GetTeacherPrivateClassDto
                    {
                        id = privateClass.teacherPrivateClass?.id ?? 0,
                        teacherId = privateClass.teacherPrivateClass?.teacher.id ?? 0,
                        status = privateClass.teacherPrivateClass?.status ?? TeacherClassStatus.Incomplete
                    }
                }
            };
        }


        private async Task DeleteClass(int classId)
        {
            var cls = await _context.PrivateClasses
                    .Include(pc => pc.studentPrivateClasses)
                    .Include(pc => pc.teacherPrivateClass)
                    .FirstOrDefaultAsync(pc => pc.id == classId);

            if (cls is null)
                throw new Exception($"Class with ID {classId} not found.");

            if (cls.studentPrivateClasses != null)
                _context.RemoveRange(cls.studentPrivateClasses);

            if (cls.teacherPrivateClass != null)
                _context.Remove(cls.teacherPrivateClass);

            _context.Remove(cls);

            await _context.SaveChangesAsync();
        }

        private async Task<UpdatePrivateClassDto> UpdateClass(UpdatePrivateClassDto updatedClass)
        {
            int id = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            var cls = await _context.PrivateClasses
                    .FirstOrDefaultAsync(pc => pc.id == updatedClass.id);

            if (cls is null)
                throw new Exception($"Class with ID {updatedClass.id} not found.");

            _mapper.Map(updatedClass, cls);

            cls.room = updatedClass.room;
            cls.method = updatedClass.method;
            cls.date = updatedClass.date;
            cls.fromTime = updatedClass.fromTime;
            cls.toTime = updatedClass.toTime;
            cls.LastUpdatedBy = id;

            var teacherCls = await _context.TeacherPrivateClasses
                .FirstOrDefaultAsync(tpc => tpc.id == updatedClass.teacherPrivateClass.id);

            if (teacherCls is null)
                throw new Exception($"Teacher private class with ID {updatedClass.teacherPrivateClass.id} not found.");

            _mapper.Map(updatedClass.teacherPrivateClass, teacherCls);

            if (teacherCls.teacher != null)
                teacherCls.teacherId = updatedClass.teacherPrivateClass.teacherId;

            var updatedTeacher = await _context.Teachers
                .Include(t => t.privateClasses)
                .FirstOrDefaultAsync(tpc => tpc.id == updatedClass.teacherPrivateClass.teacherId);

            if (updatedTeacher is null)
                throw new Exception($"Teacher with ID {updatedClass.teacherPrivateClass.teacherId} not found.");

            teacherCls.teacher = updatedTeacher;

            teacherCls.workType = updatedClass.teacherPrivateClass.workType;

            await _context.SaveChangesAsync();

            return new UpdatePrivateClassDto
            {
                id = updatedClass.id,
                room = updatedClass.room,
                method = updatedClass.method,
                fromTime = updatedClass.fromTime,
                toTime = updatedClass.toTime,
                teacherPrivateClass = new UpdateTeacherPrivateClassDto
                {
                    id = teacherCls.id,
                    teacherId = teacherCls.id,
                    workType = teacherCls.workType
                }
            };
        }
    }
}