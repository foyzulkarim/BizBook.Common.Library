using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BizBook.Common.Library.EntityFrameworkCore
{

    public class IsIndex : Attribute
    {

    }

    public static class DbContextExtension
    {
        public static ModelBuilder BuildIndex<T>(this ModelBuilder builder) where T : class
        {
            var propertyInfos = typeof(T).GetProperties().ToList().Where(x => x.GetCustomAttributes(typeof(IsIndex), false).Length > 0);
            foreach (var propertyInfo in propertyInfos)
            {
                var parameterExpression = Expression.Parameter(typeof(T), "x");
                var memberExpression = Expression.Convert(Expression.Property(parameterExpression, propertyInfo.Name), typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(memberExpression, parameterExpression);
                string indexName = $"IX_{propertyInfo.Name}";
                builder.Entity<T>().HasIndex(lambda).HasName(indexName);
            }

            return builder;
        }
    }
}