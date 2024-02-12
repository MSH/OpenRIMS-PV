using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OpenRIMS.PV.Main.Core.Utilities
{
    public static class Extensions
    {
        /// <summary>
        /// Concatenates the Dictionary into a delimited String.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public static string ToKeyValueString(this Dictionary<string, decimal> dictionary)
        {
            if (dictionary == null) return string.Empty;

            var stringBuilder = new StringBuilder();

            foreach (var item in dictionary)
            {
                stringBuilder.Append(string.Format("{0}:{1},", item.Key, item.Value.ToString(CultureInfo.InvariantCulture)));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Spilts the delimited string into a dictionary
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Dictionary<string, decimal> ToDictionary(this string source)
        {
            var returnDictionary = new Dictionary<string, decimal>();

            if (string.IsNullOrWhiteSpace(source)) return returnDictionary;

            foreach (var pair in source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var items = pair.Split(new[] { ':' });

                decimal decimalValue;

                Decimal.TryParse(items[1], out decimalValue);

                returnDictionary.Add(items[0], decimalValue);
            }

            return returnDictionary;
        }

        /// <summary>
        /// Contatenates the items in the specified list into a string delimited by the character specified as the delimiter.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string ToDelimitedString(this List<string> list, char delimiter)
        {
            if (list == null) return string.Empty;

            var stringBuilder = new StringBuilder();

            foreach (var item in list)
            {
                stringBuilder.Append(string.Format("{0}{1}", item, delimiter));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Splits the specified string by the specified delimiter into a list of items.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static List<string> ToList(this string source, char delimiter)
        {
            var returnList = new List<string>();

            if (string.IsNullOrWhiteSpace(source)) return returnList;

            returnList.AddRange(source.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries));

            return returnList;
        }

        /// <summary>
        /// Generate a functional expression for creating an IOrderedQueryable
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being ordered</typeparam>
        /// <param name="orderColumn">The column to be ordered</param>
        /// <param name="orderType">Asc or desc</param>
        /// <returns></returns>
        public static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderBy<TEntity>(string orderColumn, string orderType)
        {
            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            string[] props = orderColumn.Split('.');
            IQueryable<TEntity> query = new List<TEntity>().AsQueryable<TEntity>();
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);
            string methodName = orderType == "asc" ? "OrderBy" : "OrderByDescending";

            MethodCallExpression resultExp =
                Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TEntity), type }, outerExpression.Body, Expression.Quote(lambda));
            var finalLambda = Expression.Lambda(resultExp, argQueryable);
            return (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
        }
    }
}
