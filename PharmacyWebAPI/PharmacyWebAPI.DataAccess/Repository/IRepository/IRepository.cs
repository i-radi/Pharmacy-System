namespace PharmacyWebAPI.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T>? GetAsync(int id);

        Task<T> AddAsync(T entity);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        T Update(T entity);

        IEnumerable<T> UpdateRange(IEnumerable<T> entity);

        T Delete(T entity);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<T, bool>> filter);

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[]? includeProperty);

        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? includeProperty);

        Task<IEnumerable<T>> GetAllFilterAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[]? includeProperty);
    }
}