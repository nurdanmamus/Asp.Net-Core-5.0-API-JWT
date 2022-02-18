using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>(); 
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
           return await _dbSet.ToListAsync(); 
            //tüm veri db den geldikten sonra memoride query yaparsın
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity!=null)  
            {
                //memoriden ayırmak 
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);  
            //memoriden bu alanı sildi. daha veri tabanına yansımadı
        }

        public TEntity Update(TEntity entity)
        {
            //repository patternin dezavantajlarından biri. 
            //sadece bir sütun güncellemek için tüm entry güncelleniyor. 
            //büyük projeler için generic repo yerine domain design pattern  kullanılmalıdır.
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
            //IQueryable da tüm sorgu koşulları çalıştıktan sonra tolist denildiğinde db'ye yansır.

        }
    }
}
