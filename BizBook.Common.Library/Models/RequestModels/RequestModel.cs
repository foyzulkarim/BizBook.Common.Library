using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BizBook.Common.Library.Models.Entities;
using BizBook.Common.Library.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BizBook.Common.Library.Models.RequestModels
{
    [ModelBinder(BinderType = typeof(AbstractModelBinder))]
    public abstract class RequestModel<TModel> where TModel : Entity
    {
        protected Expression<Func<TModel, bool>> ExpressionObj = e => true;


        protected RequestModel(string keyword, string orderBy = "Modified", string isAscending = "False")
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "";
            }

            Page = 1;
            Keyword = keyword.ToLower();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy;
            }

            if (!string.IsNullOrWhiteSpace(isAscending))
            {
                IsAscending = isAscending;
            }

            Request = new OrderByRequest
            {
                PropertyName = string.IsNullOrWhiteSpace(OrderBy) ? "Modified" : OrderBy,
                IsAscending = !string.IsNullOrWhiteSpace(isAscending) && bool.Parse(isAscending)
            };
        }

        public string OrderBy { get; set; }
        public string IsAscending { get; set; }
        public string DateColumn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PerPageCount { get; set; } = 10;
        public int Page { get; set; }
        public string Id { get; set; }
        public OrderByRequest Request { get; }
        public string Keyword { get; set; }
        public string ParentId { get; set; }
        public string ShopId { get; set; }

        public bool IsIncludeParents { get; set; }


        protected Func<IQueryable<TSource>, IOrderedQueryable<TSource>> OrderByFunc<TSource>()
        {
            string propertyName = Request.PropertyName;
            bool ascending = Request.IsAscending;
            var source = Expression.Parameter(typeof(IQueryable<TSource>), "source");
            var item = Expression.Parameter(typeof(TSource), "item");
            var member = Expression.Property(item, propertyName);
            var selector = Expression.Quote(Expression.Lambda(member, item));
            var body = Expression.Call(
                typeof(Queryable), @ascending ? "OrderBy" : "OrderByDescending",
                new[] { item.Type, member.Type },
                source, selector);
            var expr = Expression.Lambda<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>>(body, source);
            var func = expr.Compile();
            return func;
        }
        protected abstract Expression<Func<TModel, bool>> GetExpression();
        //public abstract IQueryable<TModel> IncludeParents(IQueryable<TModel> queryable);

        public virtual IQueryable<TModel> IncludeParents(IQueryable<TModel> queryable)
        {
            return queryable;
        }

        public abstract Expression<Func<TModel, DropdownViewModel<TModel,TV>>> Dropdown<TV>() where TV : BaseBasicViewModel<TModel>;

        public IQueryable<TModel> GetOrderedData(IQueryable<TModel> queryable)
        {
            queryable = queryable.Where(GetExpression());
            queryable = OrderByFunc<TModel>()(queryable);
            return queryable;
        }

        public IQueryable<TModel> SkipAndTake(IQueryable<TModel> queryable)
        {
            if (Page != -1)
            {
                queryable = queryable.Skip((Page - 1) * PerPageCount).Take(PerPageCount);
            }

            return queryable;
        }

        public IQueryable<TModel> GetData(IQueryable<TModel> queryable)
        {
            return queryable.Where(GetExpression());
        }

        public async Task<TModel> GetFirstData(IQueryable<TModel> queryable)
        {
            return await queryable.FirstOrDefaultAsync(GetExpression());
        }

        protected Expression<Func<TModel, bool>> GenerateBaseEntityExpression()
        {
            if (Id.IdIsOk())
            {
                ExpressionObj = ExpressionObj.And(x => x.Id == Id);
            }

            if (StartDate != new DateTime())
            {
                StartDate = StartDate.Date;
                if (EndDate != new DateTime())
                {
                    EndDate = EndDate.Date.AddDays(1).AddMinutes(-1);
                }
                else
                {
                    EndDate = DateTime.Today.Date.AddDays(1).AddMinutes(-1);
                }

                if (string.IsNullOrWhiteSpace(DateColumn)  || DateColumn == "Created")
                {
                    ExpressionObj = ExpressionObj.And(x => x.Created.Date >= StartDate && x.Created.Date <= EndDate);
                }
                else
                {
                    ExpressionObj = ExpressionObj.And(x => x.Modified.Date >= StartDate && x.Modified.Date <= EndDate);
                }
            }

            return ExpressionObj;
        }
    }
}