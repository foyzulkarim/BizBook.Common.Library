using System;
using System.Collections.Generic;
using System.Reflection;
using BizBook.Common.Library.Models.RequestModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace BizBook.Common.Library.ApiExtensions
{
    public static class StartupExtension
    {
        public static void RegisterCommonServicesAndModels(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                if (!type.IsAbstract && !type.IsInterface)
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        serviceCollection.AddTransient(interfaceType, type);
                    }
                }
            }

            List<IModelBinderProvider> providers = new List<IModelBinderProvider>();
            var requestModelType = typeof(RequestModel<>);
            for (var index = 0; index < types.Length; index++)
            {
                Type type1 = types[index];
                if (type1.BaseType != null)
                {
                    if (type1.BaseType.Name == requestModelType.Name)
                    {
                        //var instance = Activator.CreateInstance(type1);
                        var d1 = typeof(AbstractModelBinderProvider<>);
                        Type[] args = new[] { type1 };
                        var makeName = d1.MakeGenericType(args);
                        var instance = Activator.CreateInstance(makeName);
                        if (instance != null)
                        {
                            providers.Add((IModelBinderProvider)instance);
                        }
                    }
                }
            }

            serviceCollection.AddMvc(options =>
            {
                for (int i = 0; i < providers.Count; i++)
                {
                    options.ModelBinderProviders.Insert(0, providers[i]);
                }

                options.EnableEndpointRouting = false;
            });
        }
    }
}
