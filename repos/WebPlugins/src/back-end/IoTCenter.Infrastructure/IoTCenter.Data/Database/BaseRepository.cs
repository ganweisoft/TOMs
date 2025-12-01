// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenter.Data.Database
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public readonly GWDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(GWDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public int Max(Expression<Func<TEntity, int>> query)
        {
            return _dbSet.Max(query);
        }

        public bool Any(Expression<Func<TEntity, bool>> query)
        {
            return _dbSet.Any(query);
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void AddRange(List<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }

        public List<TEntity> GetAllList()
        {
            return _dbSet.AsNoTracking().ToList();
        }


        public List<TEntity> GetAllListAsTracking(Expression<Func<TEntity, bool>> query)
        {
            return _dbSet.Where(query).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsTrackingAsync(Expression<Func<TEntity, bool>> query)
        {
            return await _dbSet.Where(query).ToListAsync();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> query)
        {
            return _dbSet.AsNoTracking().Where(query).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> query)
        {
            return await _dbSet.AsNoTracking().Where(query).ToListAsync();
        }


        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public List<TEntity> GetPageList(Expression<Func<TEntity, bool>> query, int pageNo, int pageSize)
        {
            return _dbSet.AsNoTracking().Where(query).Skip(pageNo * pageSize).Take(pageSize).ToList();
        }

        public async Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> query, int pageNo, int pageSize)
        {
            return await _dbSet.AsNoTracking().Where(query).Skip(pageNo * pageSize).Take(pageSize).ToListAsync();
        }

        public (List<TEntity>, int, int) GetPageList(Expression<Func<TEntity, bool>> query, int pageNo, int pageSize, Expression<Func<TEntity, DateTime?>> orderByExpression = null, bool isAsc = true)
        {
            int total = _dbSet.AsNoTracking().Count(query);
            int totalPage = (int)Math.Ceiling((double)(total / pageSize));
            var iqueryable = _dbSet.AsNoTracking().Where(query);
            if (orderByExpression != null)
            {
                if (isAsc)
                    iqueryable = iqueryable.OrderBy(orderByExpression);
                else
                    iqueryable = iqueryable.OrderByDescending(orderByExpression);
            }

            return (iqueryable.Skip(pageNo * pageSize).Take(pageSize).ToList(), total, totalPage);
        }

        public async Task<TEntity> GetAsync(object key)
        {
            return await _dbSet.FindAsync(key);
        }

        public TEntity GetByKey(object key)
        {
            return _dbSet.Find(key);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(List<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(List<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> query)
        {
            return _dbSet.FirstOrDefault(query);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> query)
        {
            return await _dbSet.FirstOrDefaultAsync(query);
        }
    }
}
