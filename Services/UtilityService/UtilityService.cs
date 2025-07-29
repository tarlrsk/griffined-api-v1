using Firebase.Auth;
using FirebaseAdmin;

namespace griffined_api.Services.UtilityService
{
    public class UtilityService : IUtilityService
    {
        private readonly string? API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        private readonly string? PROJECT_ID = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        private readonly FirebaseApp _firebaseApp;
        private readonly DataContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IAsyncRepository<StudyCourse> _studyCourseRepo;
        private readonly IAsyncRepository<StudySubject> _studySubjectRepo;
        private readonly IAsyncRepository<StudyClass> _studyClassRepo;

        public UtilityService(DataContext context,
                              FirebaseApp firebaseApp,
                              IUnitOfWork uow,
                              IAsyncRepository<StudyCourse> studyCourseRepo,
                              IAsyncRepository<StudySubject> studySubjectRepo,
                              IAsyncRepository<StudyClass> studyClassRepo)
        {
            _context = context;
            _firebaseApp = firebaseApp;
            _uow = uow;
            _studyCourseRepo = studyCourseRepo;
            _studySubjectRepo = studySubjectRepo;
            _studyClassRepo = studyClassRepo;
        }

        public async Task AddStudentFirebaseId()
        {
            var student = _context.Students.FirstOrDefault(x => x.FirebaseId == null
                                                           && x.Email != "-");

            if (student is null)
            {
                return;
            }

            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            var studentPassword = $"Hog{student.Phone}";

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(student.Email, studentPassword);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    throw new ConflictException("Email Exists");
                else if (ex.Message.Contains("INVALID_EMAIL"))
                    throw new ConflictException("Invalid Email Format");
                else
                    throw new InternalServerException($"Something went wrong. {ex.Message}");
            }

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            student.FirebaseId = firebaseId;

            await AddStudentFireStoreAsync(student);

            await _context.SaveChangesAsync();
        }

        public async Task AddStudentCode()
        {
            var students = _context.Students.Where(x => x.StudentCode == null)
                                           .ToList();

            // INITIALIZE CURRENT DATE, MONTH, AND RUNNING NUMBER.
            DateTime now = DateTime.Now;
            int currentMonth = DateTime.Now.Month;
            int runningNumber = 0;

            foreach (var student in students)
            {
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

                student.StudentCode = studentCode;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddTeacherFirebaseId()
        {
            var teacher = _context.Teachers.FirstOrDefault(x => x.FirebaseId == null
                                                           && x.Email != "-");

            if (teacher is null)
            {
                return;
            }

            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            var teacherPassword = $"Hog{teacher.Phone}";

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(teacher.Email, teacherPassword);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    throw new ConflictException("Email Exists");
                else if (ex.Message.Contains("INVALID_EMAIL"))
                    throw new ConflictException("Invalid Email Format");
                else
                    throw new InternalServerException($"Something went wrong. {ex.Message}");
            }

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            teacher.FirebaseId = firebaseId;

            await AddTeacherFireStoreAsync(teacher);

            await _context.SaveChangesAsync();
        }

        public async Task AddStaffFirebaseId()
        {
            var staff = _context.Staff.FirstOrDefault(x => x.FirebaseId == null
                                                      && x.Email != "-");

            if (staff is null)
            {
                return;
            }

            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            var staffPassword = $"hog{staff.Phone}";

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(staff.Email, staffPassword);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    throw new ConflictException("Email Exists");
                else if (ex.Message.Contains("INVALID_EMAIL"))
                    throw new ConflictException("Invalid Email Format");
                else
                    throw new InternalServerException($"Something went wrong. {ex.Message}");
            }

            var token = new JwtSecurityToken(jwtEncodedString: firebaseAuthLink.FirebaseToken);
            string firebaseId = token.Claims.First(c => c.Type == "user_id").Value;

            staff.FirebaseId = firebaseId;

            await AddStaffFireStoreAsync(staff);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteFirebaseAuthentication()
        {
            var auth = FirebaseAdmin.Auth.FirebaseAuth.GetAuth(_firebaseApp);

            var users = auth.ListUsersAsync(null);

            await foreach (var user in users)
            {
                await auth.DeleteUserAsync(user.Uid);
            }
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

        private async Task AddTeacherFireStoreAsync(Teacher staff)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(staff.FirebaseId);
            Dictionary<string, object> staffDoc = new()
                {
                    { "displayName", staff.FullName },
                    { "email", staff.Email },
                    { "id", staff.Id },
                    { "role", "teacher" },
                    { "uid", staff.FirebaseId!}

                };
            await docRef.SetAsync(staffDoc);
        }

        private async Task AddStaffFireStoreAsync(Staff staff)
        {
            FirestoreDb db = FirestoreDb.Create(PROJECT_ID);
            DocumentReference docRef = db.Collection("users").Document(staff.FirebaseId);
            Dictionary<string, object> staffDoc = new()
                {
                    { "displayName", staff.FullName },
                    { "email", staff.Email },
                    { "id", staff.Id },
                    { "role", staff.Role },
                    { "uid", staff.FirebaseId!}
                };
            await docRef.SetAsync(staffDoc);
        }

        public void UpdateStudyClassNumber()
        {
            var studySubjects = _studySubjectRepo.Query()
                                                 .Include(x => x.StudyClasses)
                                                 .ToList();

            if (!studySubjects.Any())
            {
                return;
            }

            _uow.BeginTran();

            int classNo = 0;

            foreach (var studySubject in studySubjects)
            {
                classNo = 0;

                var studyClasses = studySubject.StudyClasses;

                foreach (var studyClass in studyClasses)
                {
                    classNo++;
                    studyClass.ClassNumber = classNo;
                    _studyClassRepo.Update(studyClass);
                }
            }

            _uow.Complete();
            _uow.CommitTran();
        }

        public async Task UpdateStudyCourseStatus()
        {
            var studyCourses = await _context.StudyCourses
                                             .Include(sc => sc.StudySubjects)
                                                 .ThenInclude(ss => ss.StudyClasses)
                                             .Where(x => x.Status != StudyCourseStatus.NotStarted
                                                      && x.Status != StudyCourseStatus.Finished
                                                      && x.Status != StudyCourseStatus.Cancelled)
                                             .ToListAsync();

            _uow.BeginTran();

            foreach (var studyCourse in studyCourses)
            {
                bool allClassesCheckedOrUnchecked = studyCourse.StudySubjects
                                                               .All(subject => subject.StudyClasses
                                                               .All(studyClass => studyClass.Status == ClassStatus.CHECKED
                                                                               || studyClass.Status == ClassStatus.UNCHECKED
                                                                               || studyClass.Status == ClassStatus.CANCELLED
                                                                               || studyClass.Status == ClassStatus.DELETED));

                if (allClassesCheckedOrUnchecked)
                {
                    studyCourse.Status = StudyCourseStatus.Finished;

                    await _uow.CompleteAsync();
                }
            }

            _uow.CommitTran();
        }

        public async Task UpdateStudentExpiryDate()
        {
            // Get current date
            DateTime today = DateTime.UtcNow;

            // Fetch all students along with their associated study subject memberships and classes
            var students = await _context.Students.AsNoTracking()
                                                  .Include(s => s.StudySubjectMember)
                                                      .ThenInclude(ssm => ssm.StudySubject)
                                                        .ThenInclude(ss => ss.StudyClasses)
                                                            .ThenInclude(sc => sc.Schedule)
                                                  .AsSplitQuery()
                                                  .ToListAsync();

            foreach (var student in students)
            {
                if (student.StudySubjectMember is null || !student.StudySubjectMember.Any())
                {
                    student.ExpiryDate = today;
                    student.Status = StudentStatus.Inactive;
                    continue;
                }

                foreach (var studySubjectMember in student.StudySubjectMember)
                {
                    DateTime classEndDateTime = studySubjectMember.StudySubject
                                              .StudyClasses
                                              .OrderByDescending(sc => sc.Schedule.Date)
                                              .Select(sc => sc.Schedule.Date)
                                              .FirstOrDefault();

                    if (student.ExpiryDate <= classEndDateTime.AddDays(14) || student.ExpiryDate is null)
                    {
                        student.ExpiryDate = classEndDateTime.AddDays(14);
                    }
                }

                if (student.ExpiryDate <= DateTime.UtcNow)
                {
                    student.Status = StudentStatus.Inactive;
                }
            }

            // Save changes
            _uow.BeginTran();
            foreach (var student in students)
            {
                _context.Students.Entry(student).State = EntityState.Modified;
            }
            await _uow.CompleteAsync();
            _uow.CommitTran();
        }

        public async Task AddStudentAttendence()
        {
            var allStudyClasses = await _context.StudyClasses.Include(x => x.StudyCourse)
                                                             .Where(x => x.StudyCourse.Status != StudyCourseStatus.Pending)
                                                             .AsNoTracking()
                                                             .AsSplitQuery()
                                                             .ToListAsync();

            List<int> studyClassIds = new();

            foreach (var cls in allStudyClasses)
            {
                var attendences = await _context.StudentAttendances.AsNoTracking()
                                                                   .Where(x => x.StudyClassId == cls.Id)
                                                                   .ToListAsync();

                if (attendences is null || !attendences.Any())
                {
                    studyClassIds.Add(cls.Id);
                }
            }

            var studyClasses = await _context.StudyClasses.Include(x => x.StudySubject)
                                                            .ThenInclude(x => x.StudySubjectMember)
                                                          .Where(x => studyClassIds.Contains(x.Id))
                                                          .AsNoTracking()
                                                          .AsSplitQuery()
                                                          .ToListAsync();

            List<StudentAttendance> attendances = new();

            foreach (var cls in studyClasses)
            {
                foreach (var member in cls.StudySubject.StudySubjectMember)
                {
                    var attendence = new StudentAttendance
                    {
                        StudentId = member.StudentId,
                        StudyClassId = cls.Id,
                        Attendance = Attendance.None
                    };

                    attendances.Add(attendence);
                }
            }

            _uow.BeginTran();
            await _context.StudentAttendances.AddRangeAsync(attendances);
            await _uow.CompleteAsync();
            _uow.CommitTran();
        }
    }
}