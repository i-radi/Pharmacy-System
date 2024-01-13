using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PharmacyWebAPI.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.dbSet = _context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public Task<int> CountAsync() => _context.Set<T>().CountAsync();

        public Task<int> CountAsync(Expression<Func<T, bool>> filter) => _context.Set<T>().CountAsync(filter);

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? includeProperty)
        {
            IQueryable<T> query = _context.Set<T>();
            query = _context.Set<T>();
            if (includeProperty != null)
                query = includeProperty.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllFilterAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[]? includeProperty)
        {
            IQueryable<T> query;
            if (filter != null)
                query = _context.Set<T>().Where(filter);
            else
                query = _context.Set<T>();
            if (includeProperty != null)
                query = includeProperty.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return await query.ToListAsync();
        }

        public async Task<T>? GetAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[]? includeProperty)
        {
            IQueryable<T> query;
            if (filter != null)
                query = _context.Set<T>().Where(filter);
            else
                query = _context.Set<T>();
            if (includeProperty != null)
                query = includeProperty.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return await query.FirstOrDefaultAsync();
        }

        public T Update(T entity)
        {
            _context.Update(entity);
            return entity;
        }

        public IEnumerable<T> UpdateRange(IEnumerable<T> entity)
        {
            _context.UpdateRange(entity);
            return entity;
        }
    }
}