namespace FreeDB.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Data.OData.Query;

    public class ODataOrderByLambda
    {
        /// <summary>
        /// OrderBy string split(',') nodes
        /// </summary>
        private readonly IList<string> _nodes;

        /// <summary>
        /// Constructor initialized with the raw orderby value
        /// </summary>
        /// <param name="orderByRawValue">Raw orderby value</param>
        public ODataOrderByLambda(string orderByRawValue)
        {
            _nodes = orderByRawValue.Split(',');
        }

        /// <summary>
        /// Determines if the orderby raw value contains a nested property expression
        /// </summary>
        /// <param name="rawValue">Raw orderby to check</param>
        /// <returns>True if the orderby contains a nested property expression false otherwise</returns>
        public static bool ContainsNestedOrderBy(string rawValue)
        {
            return rawValue != null && rawValue.Contains('/');
        }

        /// <summary>
        /// Apply the order by query options to the given query
        /// </summary>
        /// <typeparam name="T">Queryable type</typeparam>
        /// <param name="query">Queryable instance</param>
        /// <returns>Query with orderby applied</returns>
        public IOrderedQueryable<T> ApplyTo<T>(IQueryable<T> query)
        {
            return _nodes.Aggregate((IOrderedQueryable<T>) null, (a, n) =>
                {
                    var oParamT = Expression.Parameter(typeof (T), "x");
                    var parsedNode = GetNodeProperties(n);

                    var oExpression = Expression.Lambda(parsedNode.Item1
                                                                  .Aggregate((MemberExpression) null,
                                                                             (me, s) =>
                                                                                 {
                                                                                     if (me == null)
                                                                                         return
                                                                                             Expression.Property(
                                                                                                 oParamT, s);

                                                                                     return
                                                                                         Expression.Property(me, s);
                                                                                 }), oParamT);

                    var oOrderByMethod = GetOrderingMethod<T>(parsedNode.Item2, a != null, oExpression);
                    return oOrderByMethod.Invoke(null, new object[]
                        {
                            a ?? query,
                            oExpression
                        }) as IOrderedQueryable<T>;
                });
        }

        /// <summary>
        /// Get the paresd order by expression for the property
        /// </summary>
        /// <param name="node">Node to split</param>
        /// <returns>Order by expression split by '/'</returns>
        private static Tuple<string[], OrderByDirection> GetNodeProperties(string node)
        {
            if (!node.Contains(' '))
                return new Tuple<string[], OrderByDirection>(node.Split('/'), OrderByDirection.Ascending);

            if (!node.Substring(node.IndexOf(' ')).Contains("desc"))
                return new Tuple<string[], OrderByDirection>(
                    node
                        .Substring(0, node.IndexOf(' '))
                        .Split('/'),
                    OrderByDirection.Descending);

            return new Tuple<string[], OrderByDirection>(
                node
                    .Substring(0, node.IndexOf(' '))
                    .Split('/'),
                OrderByDirection.Ascending);
        }

        /// <summary>
        /// Gets the ordering method for based on the current direction and ordering state
        /// </summary>
        /// <typeparam name="T">Type being queried</typeparam>
        /// <param name="orderByDirection">Direction to the order</param>
        /// <param name="isOrdered">Flag determining if the query is already ordered</param>
        /// <param name="expression">Lambda expression for orderby method</param>
        /// <returns>Ordering method</returns>
        private static MethodInfo GetOrderingMethod<T>(OrderByDirection orderByDirection,
                                                       bool isOrdered, LambdaExpression expression)
        {
            MethodInfo oResult;
            if (orderByDirection == OrderByDirection.Ascending)
            {
                oResult = isOrdered
                              ? ThenByMethod.MakeGenericMethod(typeof (T), expression.Body.Type)
                              : OrderByMethod.MakeGenericMethod(typeof (T), expression.Body.Type);

                return oResult;
            }

            oResult = isOrdered
                          ? ThenByDescendingMethod.MakeGenericMethod(typeof (T), expression.Body.Type)
                          : OrderByDescendingMethod.MakeGenericMethod(typeof (T), expression.Body.Type);

            return oResult;
        }

        /// <summary>
        /// OrderBy method info
        /// </summary>
        /// <remarks>
        /// Copied from
        /// http://aspnetwebstack.codeplex.com/SourceControl/changeset/view/3283da822ade#src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        /// </remarks>
        private static readonly MethodInfo OrderByMethod =
            GenericMethodOf(_ => default(IQueryable<int>).OrderBy(default(Expression<Func<int, int>>)));

        /// <summary>
        /// OrderByDecending method info
        /// </summary>
        /// <remarks>
        /// Copied from
        /// http://aspnetwebstack.codeplex.com/SourceControl/changeset/view/3283da822ade#src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        /// </remarks> 
        private static readonly MethodInfo OrderByDescendingMethod =
            GenericMethodOf(_ => default(IQueryable<int>).OrderByDescending(default(Expression<Func<int, int>>)));

        /// <summary>
        /// ThenBy method info
        /// </summary>
        /// <remarks>
        /// Copied from
        /// http://aspnetwebstack.codeplex.com/SourceControl/changeset/view/3283da822ade#src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        /// </remarks>
        private static readonly MethodInfo ThenByMethod = GenericMethodOf(_ => default(IOrderedQueryable<int>).ThenBy(default(Expression<Func<int, int>>)));

        /// <summary>
        /// ThenByDescending method info
        /// </summary>
        /// <remarks>
        /// Copied from
        /// http://aspnetwebstack.codeplex.com/SourceControl/changeset/view/3283da822ade#src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        /// </remarks>
        private static readonly MethodInfo ThenByDescendingMethod = GenericMethodOf(_ => default(IOrderedQueryable<int>).ThenByDescending(default(Expression<Func<int, int>>)));

        /// <summary>
        /// Generate a generic method from the given expression
        /// </summary>
        /// <typeparam name="TReturn">Method return type</typeparam>
        /// <param name="expression">Expression to make generic</param>
        /// <returns>Generic verions of the given expression</returns>
        /// <remarks>
        /// Copied from
        /// http://aspnetwebstack.codeplex.com/SourceControl/changeset/view/3283da822ade#src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        /// </remarks>
        private static MethodInfo GenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            LambdaExpression lambdaExpression = expression;
            return ((MethodCallExpression)lambdaExpression.Body).Method.GetGenericMethodDefinition();
        }
    }
}