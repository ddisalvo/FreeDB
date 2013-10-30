namespace FreeDB.Web.Queries
{
    using System.Linq;
    using System.Web.Http.OData.Query;
    using Helpers;
    using Infrastructure.EntityFramework;

    public abstract class BaseODataQuery<T> : IODataQueryContext
    {
        public ODataQueryOptions SearchParameters { get; set; }
        public FreeDbDataContext Context { get; set; }
        public const int MaxPageSize = 25;

        protected IQueryable<T> ApplyODataQuery(IQueryable<T> source, ODataQueryOptions options)
        {
            if (options == null)
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

            if (options.OrderBy != null)
            {
                //orderby Id seems to be the cause of
                //AmbiguousMatchException due to base class.

                //handle nested orderby
                if (ODataOrderByLambda.ContainsNestedOrderBy(options.OrderBy.RawValue))
                {
                    //generates terrible sql queries
                    var lambda = new ODataOrderByLambda(options.OrderBy.RawValue);
                    source = lambda.ApplyTo(source);
                }
                else
                {
                    source = options.OrderBy.ApplyTo(source);
                }
            }

            if (options.Skip != null)
                source = options.Skip.ApplyTo(source, settings);

            //always limit, manually
            var top = MaxPageSize;
            if (options.Top != null)
            {
                int parsedTopValue;
                if (int.TryParse(options.Top.RawValue, out parsedTopValue))
                    top = parsedTopValue > MaxPageSize ? MaxPageSize : parsedTopValue;
            }

            source = new TopQueryOption(top.ToString(), options.Context).ApplyTo(source, settings);
            return source;
        }
    }
}