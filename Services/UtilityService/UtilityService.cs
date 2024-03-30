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

        public UtilityService(DataContext context, FirebaseApp firebaseApp)
        {
            _context = context;
            _firebaseApp = firebaseApp;
        }

        public async Task AddStudentFirebaseId(int studentId)
        {
            var student = _context.Students.FirstOrDefault(x => x.Id == studentId
                                                           && x.FirebaseId == null
                                                           && x.Email != "-")
                                                           ?? throw new NotFoundException($"student with id {studentId} not found.");

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

        public async Task AddTeacherFirebaseId(int teacherId)
        {
            var teacher = _context.Teachers.FirstOrDefault(x => x.Id == teacherId
                                               && x.FirebaseId == null
                                               && x.Email != "-")
                                               ?? throw new NotFoundException($"student with id {teacherId} not found.");

            FirebaseAuthProvider firebaseAuthProvider = new(new FirebaseConfig(API_KEY));
            FirebaseAuthLink firebaseAuthLink;

            var studentPassword = $"Hog{teacher.Phone}";

            try
            {
                firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(teacher.Email, studentPassword);
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
    }
}