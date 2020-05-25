using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizBook.Common.Library.Models.Entities;
using BizBook.Common.Library.Models.RequestModels;
using BizBook.Common.Library.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BizBook.Common.Library.Services
{
    public abstract class BaseService<T, TR, TV, TV2> : IBaseService<T, TR, TV, TV2> where T : Entity where TR : RequestModel<T> where TV : BaseViewModel<T> 
        where TV2: BaseBasicViewModel<T>
    {
        protected DbContext Db;

        protected BaseService(DbContext db)
        {
            Db = db;
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            var entityEntry = await Db.Set<T>().AddAsync(entity);
            var i = await Db.SaveChangesAsync();
            return i > 0;
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            var entity = await Db.Set<T>().FindAsync(id);
            var deleted = Db.Set<T>().Remove(entity);
            var i = await Db.SaveChangesAsync();
            return i > 0;
        }

        public virtual async Task<bool> EditAsync(T entity)
        {
            var entityEntry = Db.Set<T>().Update(entity);
            var saveChanges = await Db.SaveChangesAsync();
            return saveChanges > 0;
        }

        public virtual async Task<T> GetEntityAsync(string id)
        {
            T entity = await Db.Set<T>().FindAsync(id);
            return entity;
        }

        public virtual async Task<List<TV>> GetAllAsync()
        {
            var queryable = await Db.Set<T>().AsNoTracking().ToListAsync();
            var vms = queryable.Select(x => (TV)Activator.CreateInstance(typeof(TV), new object[] { x }));
            return vms.ToList();
        }

        public async Task<List<DropdownViewModel<T, TV2>>> GetDropdownListAsync(TR request) 
        {
            var queryable = Db.Set<T>().AsNoTracking();
            queryable = request.GetOrderedData(queryable);
            var list = await queryable.Select(request.Dropdown<TV2>()).ToListAsync();
            return new List<DropdownViewModel<T, TV2>>(list);
        }

        public virtual async Task<Tuple<List<TV>, int>> SearchAsync(TR request)
        {
            var (list, count) = await GetEntityListAsync(request);
            var vms = list.ConvertAll(x => x.ToViewModel<T, TV>()).ToList();
            return new Tuple<List<TV>, int>(vms, count);
        }

        public virtual async Task<Tuple<List<TV3>, int>> SearchAsync<TV3>(TR request) where TV3 : BaseViewModel<T>
        {
            var (list, count) = await GetEntityListAsync(request);
            var vms = list.ConvertAll(x => x.ToViewModel<T, TV3>() as TV3).ToList();
            return new Tuple<List<TV3>, int>(vms, count);
        }

        public async Task<Tuple<List<TV2>, int>> BasicSearchAsync(TR request) 
        {
            var (list, count) = await GetEntityListAsync(request);
            var vms = list.ConvertAll(x => x.ToViewModel<T, TV2>()).ToList();
            return new Tuple<List<TV2>, int>(vms, count);
        }

        public virtual async Task<TV> GetAsync(string id)
        {
            var entity = await GetEntityAsync(id);
            var viewModel = entity.ToViewModel<T, TV>() as TV;
            return viewModel;
        }

        public async Task<T> GetFirstEntityAsync(TR request)
        {
            T entity = await request.GetFirstData(Db.Set<T>().AsNoTracking());
            return entity;
        }

        public async Task<TV> GetFirstAsync(TR request)
        {
            var entity = await GetFirstEntityAsync(request);
            var viewModel = entity.ToViewModel<T, TV>() as TV;
            return viewModel;
        }

        protected Entity AddNewCommonValues(Entity fromEntity, Entity toEntity, string createdBy = null, string createdFrom = null)
        {
            toEntity.Id = Guid.NewGuid().ToString();
            toEntity.Created = DateTime.Now;
            toEntity.CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? fromEntity.ModifiedBy : createdBy;
            toEntity.CreatedFrom = string.IsNullOrWhiteSpace(createdFrom) ? fromEntity.CreatedFrom : createdFrom;
            toEntity.Modified = DateTime.Now;
            toEntity.ModifiedBy = toEntity.CreatedBy;
            toEntity.IsActive = true;
            return toEntity;
        }

        private async Task<Tuple<List<T>, int>> GetEntityListAsync(TR request)
        {
            var queryable = request.GetOrderedData(Db.Set<T>().AsNoTracking());
            int count = queryable.Count();
            queryable = request.SkipAndTake(queryable);
            if (request.IsIncludeParents)
            {
                queryable = request.IncludeParents(queryable);
            }

            var list = await queryable.ToListAsync();
            return new Tuple<List<T>, int>(list, count);
        }

        private static TV CreateVmInstance(T x)
        {
            return (TV)Activator.CreateInstance(typeof(TV));
        }

    }

}
