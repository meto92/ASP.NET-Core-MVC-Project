using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Metomarket.Common.Extensions
{
    public static class EnumerableExtensions
    {
        private const string ParamName = "x";

        public static IEnumerable<TElement> Order<TElement>(
            this IEnumerable<TElement> source,
            string propName,
            bool ascending = true)
        {
            Validate(source, propName);

            PropertyInfo propInfo = typeof(TElement).GetProperties()
                .Where(p => p.Name.ToUpper() == propName.ToUpper())
                .FirstOrDefault();

            if (propInfo == null)
            {
                return source;
            }

            Type propType = propInfo.PropertyType;

            bool isPropComparable = typeof(IComparable<>)
                .MakeGenericType(propType)
                .IsAssignableFrom(propType);

            if (!isPropComparable)
            {
                return source;
            }

            ParameterExpression sourceParam = Expression.Parameter(
                typeof(IEnumerable<TElement>),
                nameof(source));
            ParameterExpression param = Expression.Parameter(typeof(TElement), ParamName);
            Expression propLambda = Expression.Lambda(
                Expression.Property(param, propInfo.Name),
                param);

            Expression orderBy = Expression.Call(
                typeof(Enumerable),
                ascending ? nameof(Enumerable.OrderBy) : nameof(Enumerable.OrderByDescending),
                new Type[] { typeof(TElement), propType },
                sourceParam,
                propLambda);

            var orderFunc = Expression.Lambda<Func<IEnumerable<TElement>, IEnumerable<TElement>>>(
                orderBy,
                sourceParam);

            return orderFunc.Compile()(source);
        }

        private static void Validate<TElement>(IEnumerable<TElement> source, string propName)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrWhiteSpace(propName))
            {
                throw new ArgumentException(nameof(propName));
            }
        }
    }
}