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
        private readonly IAsyncRepository<Staff> _staffRepo;

        public UtilityService(DataContext context,
                              FirebaseApp firebaseApp,
                              IUnitOfWork uow,
                              IAsyncRepository<Staff> staffRepo)
        {
            _context = context;
            _firebaseApp = firebaseApp;
            _uow = uow;
            _staffRepo = staffRepo;
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

            foreach (var student in students)
            {
                string studentCode = DateTime.Now.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (student.Id % 10000).ToString("0000");
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
            var staff = _staffRepo.Query()
                                  .FirstOrDefault(x => x.FirebaseId == null
                                                  && x.Email != "-");

            if (staff is null)
            {
                return;
            }

            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            var teacherPassword = $"hog{staff.Phone}";

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(staff.Email, teacherPassword);
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

            _uow.BeginTran();
            _staffRepo.Update(staff);
            _uow.Complete();
            _uow.CommitTran();

            await AddStaffFireStoreAsync(staff);
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
    }
}