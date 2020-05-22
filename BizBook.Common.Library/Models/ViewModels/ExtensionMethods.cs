using System;
using System.Linq;
using System.Reflection;
using BizBook.Common.Library.Models.Entities;

namespace BizBook.Common.Library.Models.ViewModels
{
    public static class ExtensionMethods
    {
        public static IViewModel ToViewModel<T, TV>(this T entity) where TV : IViewModel
        {
            var viewModel = Activator.CreateInstance<TV>();
            var viewModelType = viewModel.GetType();
            var entityType = entity.GetType();
            var viewModelTypes = viewModelType.GetProperties();
            var entityProperties = entityType.GetProperties();

            foreach (PropertyInfo viewModelProperty in viewModelTypes)
            {
                var entityProperty = entityProperties.FirstOrDefault(x => x.Name == viewModelProperty.Name);
                if (entityProperty != null)
                {
                    var value = entityProperty.GetValue(entity);
                    viewModelProperty.SetValue(viewModel, value);
                }
            }

            return viewModel;
        }
    }
}