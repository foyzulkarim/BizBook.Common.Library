namespace BizBook.Common.Library.Models.ViewModels
{
    public class DropdownViewModel<TV> where TV : IViewModel
    {
        public string Id { get; set; }
        public string Text { get; set; }

        public TV Data { get; set; }
    }
}