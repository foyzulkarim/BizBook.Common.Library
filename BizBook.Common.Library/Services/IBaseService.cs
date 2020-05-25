using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BizBook.Common.Library.Models.Entities;
using BizBook.Common.Library.Models.RequestModels;
using BizBook.Common.Library.Models.ViewModels;

namespace BizBook.Common.Library.Services
{
    public interface IBaseService<T, in TR, TV, TV2> where T : Entity where TR : RequestModel<T> where TV : BaseViewModel<T> where TV2 : BaseBasicViewModel<T>
    {
        Task<bool> AddAsync(T entity);
        Task<bool> EditAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<T> GetEntityAsync(string id);
        Task<TV> GetAsync(string id);
        Task<List<TV>> GetAllAsync();
        Task<T> GetFirstEntityAsync(TR request);
        Task<TV> GetFirstAsync(TR request);
        Task<List<DropdownViewModel<T, TV2>>> GetDropdownListAsync(TR request);
        Task<Tuple<List<TV>, int>> SearchAsync(TR request);
        Task<Tuple<List<TV3>, int>> SearchAsync<TV3>(TR request) where TV3 : BaseViewModel<T>;
        Task<Tuple<List<TV2>, int>> BasicSearchAsync(TR request);
    }
}
