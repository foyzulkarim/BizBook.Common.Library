using BizBook.Common.Library.Models.Entities;

namespace BizBook.Common.Library.Models.ViewModels
{
    public class DropdownViewModel<T,TV> where T: Entity where TV : BaseBasicViewModel<T>
    {
        public string Id { get; set; }
        public string Text { get; set; }

        public TV Data { get; set; }
    }
}