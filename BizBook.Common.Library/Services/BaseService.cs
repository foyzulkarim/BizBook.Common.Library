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
    public abstract class BaseService<T, TR, TV> : IBaseService<T, TR, TV> where T : Entity where TR : RequestModel<T> where TV : BaseViewModel<T>
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

        public virtual async Task<T> GetAsync(string id)
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

        public virtual async Task<List<DropdownViewModel<TV>>> GetDropdownListAsync(TR request)
        {
            var queryable = Db.Set<T>().AsNoTracking();
            queryable = request.GetOrderedData(queryable);
            var list = await queryable.Select(request.Dropdown<TV>()).ToListAsync();
            return new List<DropdownViewModel<TV>>(list);
        }

        public virtual async Task<Tuple<List<TV>, int>> SearchAsync(TR request)
        {
            var queryable = request.GetOrderedData(Db.Set<T>().AsNoTracking());
            int count = queryable.Count();
            queryable = request.SkipAndTake(queryable);
            if (request.IsIncludeParents)
            {
                queryable = request.IncludeParents(queryable);
            }

            var list = await queryable.ToListAsync();
            var vms = list.ConvertAll(x => x.ToViewModel<T,TV>() as TV).ToList();
            return new Tuple<List<TV>, int>(vms, count);
        }

        public virtual async Task<Tuple<List<TV2>, int>> SearchAsync<TV2>(TR request) where TV2 : BaseViewModel<T>
        {
            var queryable = request.GetOrderedData(Db.Set<T>().AsNoTracking());
            int count = queryable.Count();
            queryable = request.SkipAndTake(queryable);
            if (request.IsIncludeParents)
            {
                queryable = request.IncludeParents(queryable);
            }

            var list = await queryable.ToListAsync();
            var vms = list.ConvertAll(x => x.ToViewModel<T,TV2>() as TV2).ToList();
            return new Tuple<List<TV2>, int>(vms, count);
        }

        public virtual async Task<TV> GetDetailAsync(string id)
        {
            var entity = await GetAsync(id);
            var viewModel = entity.ToViewModel<T,TV>() as TV;
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


        private static TV CreateVmInstance(T x)
        {
            return (TV)Activator.CreateInstance(typeof(TV));
        }

    }

}
