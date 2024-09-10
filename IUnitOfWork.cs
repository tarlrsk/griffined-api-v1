namespace griffined_api
{
    public interface IUnitOfWork
    {
        void BeginTran();
        void RollBackTran();
        void CommitTran();

        void Complete();
        Task CompleteAsync();
    }
}