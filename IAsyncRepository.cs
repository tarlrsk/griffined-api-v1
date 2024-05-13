namespace griffined_api
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        T Add(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(ICollection<T> entities);

        int Count();
        Task<int> CountAsync();

        IQueryable<T> Query();
    }
}