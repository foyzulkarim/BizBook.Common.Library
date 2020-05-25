using BizBook.Common.Library.Models.Entities;

namespace BizBook.Common.Library.Models.RequestModels
{
    public class RequestModelWrapper<T> where T : Entity
    {
        public RequestModel<T> Request { get; set; }

        public RequestModelWrapper()
        {

        }

        public RequestModelWrapper(RequestModel<T> request)
        {
            this.Request = request;
        }
    }
}