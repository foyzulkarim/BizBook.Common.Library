namespace BizBook.Common.Library.Models.ViewModels
{
    public class DropdownViewModel<T>
    {
        public string Id { get; set; }
        public string Text { get; set; }

        public T Data { get; set; }
    }
}