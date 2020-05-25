using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BizBook.Common.Library.Models.RequestModels
{
    public class AbstractModelBinder : IModelBinder
    {
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly Dictionary<string, IModelBinder> _binders;

        public AbstractModelBinder(IModelMetadataProvider metadataProvider, Dictionary<string, IModelBinder> binders)
        {
            _metadataProvider = metadataProvider;
            _binders = binders;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //var messageTypeModelName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, "Type");
            var messageTypeModelName = ModelNames.CreatePropertyModelName(bindingContext.ModelMetadata.ModelType.Name, "Type");
            var typeResult = bindingContext.ValueProvider.GetValue(messageTypeModelName);
            if (typeResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            IModelBinder binder;
            if (!_binders.TryGetValue(typeResult.FirstValue, out binder))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var type = Type.GetType(typeResult.FirstValue);

            var metadata = _metadataProvider.GetMetadataForType(type);

            ModelBindingResult result;
            using (bindingContext.EnterNestedScope(metadata, bindingContext.FieldName, bindingContext.ModelName, model: null))
            {
                await binder.BindModelAsync(bindingContext);
                result = bindingContext.Result;
            }

            bindingContext.Result = result;

            return;
        }
    }
}