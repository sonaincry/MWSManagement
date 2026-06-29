using Indotalent.Data;
using Indotalent.Models.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Indotalent.Applications.AX
{
    public class AxCrudService<TEntity> where TEntity : class, IAxEntity, new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public AxCrudService(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<TEntity?> GetByRecIdAsync(long recId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.RecId == recId);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity input)
        {
            await NormalizeAsync(input);
            await ValidateBeforeCreateAsync(input);

            if (input.RecId <= 0)
            {
                input.RecId = await GenerateRecIdAsync();
            }

            await _dbSet.AddAsync(input);
            await _context.SaveChangesAsync();

            return input;
        }

        public virtual async Task UpdateAsync(TEntity input)
        {
            await NormalizeAsync(input);
            await ValidateBeforeUpdateAsync(input);

            var existing = await GetByRecIdAsync(input.RecId);

            if (existing == null)
            {
                throw new Exception($"Unable to load data. RecId={input.RecId}");
            }

            _context.Entry(existing).CurrentValues.SetValues(input);

            IgnoreRowVersionIfExists(existing);

            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(long recId)
        {
            var existing = await GetByRecIdAsync(recId);

            if (existing == null)
            {
                throw new Exception($"Unable to load data. RecId={recId}");
            }

            _dbSet.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<int> DeleteManyAsync(List<TEntity> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return 0;
            }

            var recIds = rows
                .Where(x => x.RecId > 0)
                .Select(x => x.RecId)
                .Distinct()
                .ToList();

            if (recIds.Count == 0)
            {
                return 0;
            }

            var data = await _dbSet
                .Where(x => recIds.Contains(x.RecId))
                .ToListAsync();

            _dbSet.RemoveRange(data);
            await _context.SaveChangesAsync();

            return data.Count;
        }

        public virtual async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        protected virtual Task NormalizeAsync(TEntity input)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ValidateBeforeCreateAsync(TEntity input)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ValidateBeforeUpdateAsync(TEntity input)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task<long> GenerateRecIdAsync()
        {
            long recId;

            do
            {
                recId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000
                        + Random.Shared.Next(100, 999);
            }
            while (await _dbSet.AnyAsync(x => x.RecId == recId));

            return recId;
        }

        private void IgnoreRowVersionIfExists(TEntity entity)
        {
            var property = _context.Entry(entity).Properties
                .FirstOrDefault(x => x.Metadata.Name.Equals("RowVersion", StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                property.IsModified = false;
            }
        }
    }
}