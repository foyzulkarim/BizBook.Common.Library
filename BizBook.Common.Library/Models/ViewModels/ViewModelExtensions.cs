using System;
using System.Linq;
using System.Reflection;

namespace BizBook.Common.Library.Models.ViewModels
{
    public static class ViewModelExtensions
    {
        public static TV ToViewModel<T, TV>(this T entity) where TV : class
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