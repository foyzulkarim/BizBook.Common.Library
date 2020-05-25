using System;
using BizBook.Common.Library.Models.Entities;

namespace BizBook.Common.Library.Models.ViewModels
{
    public abstract class BaseViewModel<T> : IViewModel where T : Entity
    {
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedFrom { get; set; }

        public DateTime Modified { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }
    }

    public abstract class BaseBasicViewModel<T> : IBasicViewModel<T> where T : Entity
    {
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}