using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.CheckAvailableService
{
    public class CheckAvailableService : ICheckAvailableService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CheckAvailableService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetAvailableTeacherDto>>> GetAvailableTeacher(string fromTime, string toTime, string date, int classId)
        {
            var response = new ServiceResponse<List<GetAvailableTeacherDto>>();
            DateTime dateParse = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
            date = dateParse.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan duration = DateTime.Parse(toTime).Subtract(DateTime.Parse(fromTime));
            string _day = dateParse.DayOfWeek.ToString();

            var dbTeachers = await _context.Teachers
                .Include(t => t.workTimes)
                .Select(t => _mapper.Map<GetTeacherDto>(t))
                .ToListAsync();

            var availableTeacher = new List<GetAvailableTeacherDto>();

            var preferFromTime = TimeOnly.Parse(fromTime);
            var preferToTime = TimeOnly.Parse(toTime);

            foreach (var teacher in dbTeachers)
            {
                var conflictStatus = false;
                var _currentClass = false;
                var cls = await _context.PrivateClasses.Where(pc => pc.teacherPrivateClass.teacherId == teacher.id
                        && pc.date == date && pc.isActive == true).ToListAsync();
                if (cls != null)
                {
                    foreach (var c in cls)
                    {
                        TimeSpan c_duration = DateTime.Parse(c.toTime).Subtract(DateTime.Parse(c.fromTime));
                        var c_fromTime = TimeOnly.Parse(c.fromTime);
                        var c_toTime = TimeOnly.Parse(c.toTime);
                        if (c.id == classId && duration == c_duration)
                        {
                            _currentClass = true;
                        }
                        else if (c.id != classId)
                        {
                            if (!(preferFromTime >= c_toTime || c_fromTime >= preferToTime))
                            {
                                conflictStatus = true;
                                break;
                            }
                        }
                    }
                }
                var workDay = teacher.workTimes.FirstOrDefault(w => w.day == _day.ToLower());

                if (conflictStatus != true && teacher.isActive == true)
                {
                    if (workDay == null)
                    {
                        availableTeacher.Add(new GetAvailableTeacherDto
                        {
                            id = teacher.id,
                            firebaseId = teacher.firebaseId,
                            fName = teacher.fName,
                            lName = teacher.lName,
                            nickname = teacher.nickname,
                            workType = "SP",
                            currentClass = _currentClass
                        });
                    }
                    else if ((preferFromTime >= TimeOnly.Parse(workDay.toTime) || TimeOnly.Parse(workDay.fromTime) >= preferToTime))
                    {
                        availableTeacher.Add(new GetAvailableTeacherDto
                        {
                            id = teacher.id,
                            firebaseId = teacher.firebaseId,
                            fName = teacher.fName,
                            lName = teacher.lName,
                            nickname = teacher.nickname,
                            workType = "OT",
                            currentClass = _currentClass
                        });
                    }
                    else
                    {
                        availableTeacher.Add(new GetAvailableTeacherDto
                        {
                            id = teacher.id,
                            firebaseId = teacher.firebaseId,
                            fName = teacher.fName,
                            lName = teacher.lName,
                            nickname = teacher.nickname,
                            workType = "Normal",
                            currentClass = _currentClass
                        });
                    }

                };
            }
            response.Data = availableTeacher;
            return response;
        }

        public async Task<ServiceResponse<List<GetAvailableTimeDto>>> GetAvailableTime([FromQuery] int[] listOfStudentId, string date, int hour, int classId)
        {
            var response = new ServiceResponse<List<GetAvailableTimeDto>>();
            var dateParse = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
            date = dateParse.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var fixPeriod = CreateFixPeriod(hour);
            var conflicts = new List<GetConflictTimeDto>();
            foreach (var sId in listOfStudentId)
            {
                var student = await _context.Students.FindAsync(sId);
                if (student is null)
                    throw new Exception($"Student with ID {sId} not found.");

                var cls = await _context.PrivateClasses.Where(pc => pc.isActive == true && pc.studentPrivateClasses!
                        .Any(spc => spc.studentId == sId)
                        && pc.date == date).ToListAsync();
                if (cls != null)
                {
                    foreach (var c in cls)
                    {
                        TimeSpan duration = DateTime.Parse(c.toTime).Subtract(DateTime.Parse(c.fromTime));
                        if (c.id == classId && duration.Hours == hour)
                        {
                            conflicts.Add(new GetConflictTimeDto
                            {
                                id = c.id,
                                fromTime = c.fromTime,
                                toTime = c.toTime,
                                currentClass = true
                            });
                        }
                        else if (c.id != classId)
                        {
                            conflicts.Add(new GetConflictTimeDto
                            {
                                id = c.id,
                                fromTime = c.fromTime,
                                toTime = c.toTime,
                                currentClass = false
                            });

                        }
                    }
                }
            };
            conflicts = conflicts.DistinctBy(i => i.id).ToList();

            var availableTimes = new List<GetAvailableTimeDto>();
            foreach (var f in fixPeriod)
            {
                var f_fromTime = TimeOnly.Parse(f.fromTime);
                var f_toTime = TimeOnly.Parse(f.toTime);
                var _currentClass = false;
                var conflictStatus = false;
                foreach (var c in conflicts)
                {
                    var c_fromTime = TimeOnly.Parse(c.fromTime);
                    var c_toTime = TimeOnly.Parse(c.toTime);

                    if (!(f_fromTime >= c_toTime || c_fromTime >= f_toTime) && c.currentClass == false)
                    {
                        conflictStatus = true;
                        break;
                    }
                    else if (!(f_fromTime >= c_toTime || c_fromTime >= f_toTime) && c.currentClass == true)
                    {
                        _currentClass = true;
                    }
                }
                if (conflictStatus == false)
                {
                    availableTimes.Add(new GetAvailableTimeDto
                    {
                        fromTime = f.fromTime,
                        toTime = f.toTime,
                        currentClass = _currentClass
                    });
                }
            }
            response.Data = availableTimes;

            return response;
        }
        private List<GetAvailableTimeDto> CreateFixPeriod(int hour)
        {
            var fixPeriod = new List<GetAvailableTimeDto>();
            var startTime = TimeOnly.Parse("10:00");
            var endTime = TimeOnly.Parse("20:00").AddHours(-(hour));
            while (startTime <= endTime)
            {
                fixPeriod.Add(new GetAvailableTimeDto { fromTime = startTime.ToString(), toTime = (startTime.AddHours(hour)).ToString() });
                startTime = startTime.AddHours(1);
            }
            return fixPeriod;
        }
    }
}