// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IoTCenter.Data.Database
{
    public interface IRepository<TEntity>
        where TEntity:class
    {
        int Max(Expression<Func<TEntity, int>> query);
        bool Any(Expression<Func<TEntity, bool>> query);
        IQueryable<TEntity> GetAll();
        List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllListAsync();
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> query);
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> query);
        List<TEntity> GetAllListAsTracking(Expression<Func<TEntity, bool>> query);
        Task<List<TEntity>> GetAllListAsTrackingAsync(Expression<Func<TEntity, bool>> query);

        Task<TEntity> GetAsync(object key);
        TEntity GetByKey(object key);
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void AddRange(List<TEntity>entities);
        Task AddRangeAsync(List<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        Task<int> SaveChangesAsync();
        int SaveChanges();
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> query);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> query);
        Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> query, int pageNo, int pageSize);
        List<TEntity> GetPageList(Expression<Func<TEntity, bool>> query, int pageNo, int pageSize);

        (List<TEntity>, int, int) GetPageList(Expression<Func<TEntity, bool>> query, int pageNo, int pageSize, Expression<Func<TEntity, DateTime?>> orderByExpression = null, bool isAsc = true);
    }
}
