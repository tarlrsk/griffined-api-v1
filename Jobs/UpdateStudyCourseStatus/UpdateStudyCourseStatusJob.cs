using Quartz;

namespace griffined_api.Jobs.UpdateStudyCourseStatus
{
    [DisallowConcurrentExecution]
    public class UpdateStudyCourseStatusJob : IJob
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _uow;
        public UpdateStudyCourseStatusJob(DataContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

        public async Task Execute(IJobExecutionContext context)
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
    }
}