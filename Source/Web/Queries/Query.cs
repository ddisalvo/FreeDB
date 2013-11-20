namespace FreeDB.Web.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http.OData.Query;
    using Core.Bases;
    using Helpers;
    using Infrastructure.EntityFramework;

    public class Query<T> : CriteriaQuery<T> where T : PersistentObject
    {
        public Query<T> Include(Expression<Func<T, object>> include)
        {
            AddInclude(include);
            return this;
        }

        public Query<T> WhereIdIs(object id)
        {
            AddCriteria(x => x.Id == id);
            return this;
        }

        public Query<T> WhereCriteria(Expression<Func<T, bool>> criteria)
        {
            AddCriteria(criteria);
            return this;
        }
    }

    public abstract class CriteriaQuery<T> : IDataContextAware where T : PersistentObject
    {
        public FreeDbDataContext DataContext { get; set; }
        public ODataQueryOptions ODataQueryOptions { get; set; }
        public int MaxPageSize { get; set; }
        public long TotalCount { get; private set; }
        private readonly IList<Expression<Func<T, bool>>> _criteria;
        private readonly IList<Expression<Func<T, object>>> _includes;

        public CriteriaQuery()
        {
            _criteria = new List<Expression<Func<T, bool>>>();
            _includes = new List<Expression<Func<T, object>>>();
        }

        public virtual IQueryable<T> Results()
        {
            IQueryable<T> set = DataContext.Set<T>().AsNoTracking();

            foreach (var expression in _includes)
            {
                set = set.Include(expression);
            }

            foreach (var expression in _criteria)
            {
                set = set.Where(expression);
            }

            var query = ApplyODataQuery(set);

            TotalCount = set.LongCount();

            return query;
        }

        public virtual T Result()
        {
            return Results().FirstOrDefault();
        }

        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            _criteria.Add(criteria);
        }

        protected void AddInclude(Expression<Func<T, object>> include)
        {
            _includes.Add(include);
        }

        private IQueryable<T> ApplyODataQuery(IQueryable<T> source)
        {
            if (ODataQueryOptions == null)
                return source;

            //this is throwing an AmbiguousMatchException, so do manually
            //results = options.ApplyTo(Discs.AsQueryable(), new ODataQuerySettings {PageSize = MaxPageSize});
            var settings =
                new ODataQuerySettings
                {
                    EnableConstantParameterization
                        = true,
                    PageSize = MaxPageSize
                };

            if (ODataQueryOptions.OrderBy != null)
            {
                //orderby Id seems to be the cause of
                //AmbiguousMatchException due to base class.

                //handle nested orderby
                if (ODataOrderByLambda.ContainsNestedOrderBy(ODataQueryOptions.OrderBy.RawValue))
                {
                    //generates terrible sql queries
                    var lambda = new ODataOrderByLambda(ODataQueryOptions.OrderBy.RawValue);
                    source = lambda.ApplyTo(source);
                }
                else
                {
                    source = ODataQueryOptions.OrderBy.ApplyTo(source);
                }
            }

            if (ODataQueryOptions.Skip != null)
                source = ODataQueryOptions.Skip.ApplyTo(source, settings);

            //always limit, manually
            var top = MaxPageSize;
            if (ODataQueryOptions.Top != null)
            {
                int parsedTopValue;
                if (int.TryParse(ODataQueryOptions.Top.RawValue, out parsedTopValue))
                    top = parsedTopValue > MaxPageSize ? MaxPageSize : parsedTopValue;
            }

            source = new TopQueryOption(top.ToString(), ODataQueryOptions.Context).ApplyTo(source, settings);
            return source;
        }
    }
}