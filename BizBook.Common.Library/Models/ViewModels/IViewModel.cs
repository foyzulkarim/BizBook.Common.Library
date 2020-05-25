using BizBook.Common.Library.Models.Entities;

namespace BizBook.Common.Library.Models.ViewModels
{
    public interface IViewModel
    {
    }

    public interface IBasicViewModel<T> where T:Entity
    {
    }
}