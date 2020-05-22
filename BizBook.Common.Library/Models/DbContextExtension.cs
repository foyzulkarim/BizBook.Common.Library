using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizBook.Common.Library.Models
{

    public class IsIndex : Attribute
    {

    }

    public static class DbContextExtension
    {
        //public static void Build(this ModelBuilder modelBuilder, Type dbContextType)
        //{
        //    var assembly = Assembly.GetAssembly(dbContextType);
        //    if (assembly != null)
        //    {
        //        var allTypes = assembly.GetTypes().ToList();
        //        var types = allTypes.Where(x => x.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IIndexBuilder<>))).ToList();
        //        foreach (Type type in types)
        //        {
        //            var methodInfo = type.GetMethods().FirstOrDefault(x => x.Name == "BuildIndices");
        //            var classInstance = Activator.CreateInstance(type, null);
        //            if (methodInfo != null)
        //            {
        //                methodInfo.Invoke(classInstance, new[] { modelBuilder });
        //            }
        //        }

        //        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        //        {
        //            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        //        }
        //    }
        //}

        public static ModelBuilder BuildIndex<T>(this ModelBuilder builder) where T : class
        {
            var propertyInfos = typeof(T).GetProperties().ToList().Where(x => x.GetCustomAttributes(typeof(IsIndex), false).Length > 0);
            foreach (var propertyInfo in propertyInfos)
            {
                //var propertyInfo = typeof(T).GetType().GetProperty("PhoneNumber");
                var parameterExpression = Expression.Parameter(typeof(T), "x");
                var memberExpression = Expression.PropertyOrField(parameterExpression, propertyInfo.Name);
                var lambda = Expression.Lambda<Func<T, object>>(memberExpression, parameterExpression);
                string indexName = $"IX_{propertyInfo.Name}";
                builder.Entity<T>().HasIndex(lambda).HasName(indexName);
            }

            return builder;
        }
    }
}