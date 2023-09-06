using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Api;

namespace griffined_api.Services.ClassCancellationRequestService
{
    public class ClassCancellationRequestService : IClassCancellationRequestService
    {
        private readonly DataContext _context;
        private readonly IFirebaseService _firebaseService;

        public ClassCancellationRequestService(DataContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }

        public async Task<ServiceResponse<string>> AddClassCancellationRequest(int studyClassId)
        {
            var dbStudyClass = await _context.StudyClasses
                            .Include(c => c.StudyCourse)
                            .Include(c => c.StudySubject)
                            .FirstOrDefaultAsync(c => c.Id == studyClassId && c.Status == ClassStatus.None)
                            ?? throw new NotFoundException($"StudyClass that can cancel with ID {studyClassId} is not found.");

            var classCancellationRequest = new ClassCancellationRequest
            {
                RequestedDate = DateTime.Now,
                StudyClass = dbStudyClass,
                StudyCourse = dbStudyClass.StudyCourse,
                StudySubject = dbStudyClass.StudySubject,
            };


            var role = _firebaseService.GetRoleWithToken();
            var userId = _firebaseService.GetAzureIdWithToken();

            if (role == "student")
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == userId)
                                    ?? throw new NotFoundException($"Student with ID {userId} is not found.");

                classCancellationRequest.Student = dbStudent;
                classCancellationRequest.RequestedRole = CancellationRole.Student;
            }
            else
            {
                var dbTeacher = await _context.Teachers.FirstOrDefaultAsync(s => s.Id == userId)
                                    ?? throw new NotFoundException($"Teacher with ID {userId} is not found.");

                classCancellationRequest.Teacher = dbTeacher;
                classCancellationRequest.RequestedRole = CancellationRole.Teacher;
            }

            dbStudyClass.Status = ClassStatus.PendingCancellation;

            _context.ClassCancellationRequests.Add(classCancellationRequest);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            return response;
        }
    }
}