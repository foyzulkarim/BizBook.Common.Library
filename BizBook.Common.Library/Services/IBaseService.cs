using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BizBook.Common.Library.Models.Entities;
using BizBook.Common.Library.Models.RequestModels;
using BizBook.Common.Library.Models.ViewModels;

namespace BizBook.Common.Library.Services
{
    public interface IBaseService<T, in TR, TV> where T : Entity where TR : RequestModel<T> where TV : BaseViewModel<T>
    {
        Task<bool> AddAsync(T entity);
        Task<bool> EditAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<T> GetAsync(string id);
        Task<TV> GetDetailAsync(string id);
        Task<List<TV>> GetAllAsync();
        Task<List<DropdownViewModel<T>>> GetDropdownListAsync(TR request);
        Task<Tuple<List<TV>, int>> SearchAsync(TR request);
        Task<Tuple<List<TV2>, int>> SearchAsync<TV2>(TR request) where TV2 : BaseViewModel<T>;
    }
}
