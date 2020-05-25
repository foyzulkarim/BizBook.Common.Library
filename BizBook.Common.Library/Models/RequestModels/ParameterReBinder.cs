using System.Collections.Generic;
using System.Linq.Expressions;

namespace BizBook.Common.Library.Models.RequestModels
{
    public class ParameterReBinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterReBinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
            Expression exp)
        {
            return new ParameterReBinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}