using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BizBook.Common.Library.Models.RequestModels
{
    public class AbstractModelBinderProvider<T> : IModelBinderProvider where T : class
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType != typeof(T))
                return null;

            var binders = new Dictionary<string, IModelBinder>();
            foreach (var type in typeof(AbstractModelBinderProvider<>).GetTypeInfo().Assembly.GetTypes())
            {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsAbstract || typeInfo.IsNested)
                    continue;

                if (!(typeInfo.IsClass && typeInfo.IsPublic))
                    continue;

                if (!typeof(T).IsAssignableFrom(type))
                    continue;

                var metadata = context.MetadataProvider.GetMetadataForType(type);
                var binder = context.CreateBinder(metadata);
                binders.Add(type.FullName, binder);
            }

            return new AbstractModelBinder(context.MetadataProvider, binders);
        }
    }
}