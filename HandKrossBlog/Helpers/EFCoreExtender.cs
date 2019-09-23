using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HandKrossBlog.Helpers
{
    public static class EFCoreExtender
    {
        public static IQueryable<T> WhereAny_OR<T>(this IQueryable<T> queryable, params Expression<Func<T, bool>>[] predicates)
        {
            var parameter = Expression.Parameter(typeof(T));
            return queryable.Where(Expression.Lambda<Func<T, bool>>(predicates.Aggregate<Expression<Func<T, bool>>, Expression>(null, (current, predicate) => {
                var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
                return current != null ? Expression.OrElse(current, visitor.Visit(predicate.Body)) : visitor.Visit(predicate.Body);
            }), parameter));
        }

        public static IQueryable<T> WhereAll_AND<T>(this IQueryable<T> queryable, params Expression<Func<T, bool>>[] predicates)
        {
            var parameter = Expression.Parameter(typeof(T));
            return queryable.Where(Expression.Lambda<Func<T, bool>>(predicates.Aggregate<Expression<Func<T, bool>>, Expression>(null, (current, predicate) => {
                var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
                return current != null ? Expression.AndAlso(current, visitor.Visit(predicate.Body)) : visitor.Visit(predicate.Body);
            }), parameter));
        }

        private class ParameterSubstitutionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _destination;
            private readonly ParameterExpression _source;

            public ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination)
            {
                _source = source;
                _destination = destination;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return ReferenceEquals(node, _source) ? _destination : base.VisitParameter(node);
            }
        }
    }
}
