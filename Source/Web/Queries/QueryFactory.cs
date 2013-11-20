namespace FreeDB.Web.Queries
{
    using System.Web.Http.OData.Query;
    using Infrastructure.EntityFramework;

    public class QueryFactory
    {
        public const int MaxPageSize = 25;
        private readonly FreeDbDataContext _dataContext;

        public QueryFactory(FreeDbDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public T CreateQuery<T>(ODataQueryOptions options = null) where T : IDataContextAware, new()
        {
            return new T { DataContext = _dataContext, ODataQueryOptions = options, MaxPageSize = MaxPageSize };
        }
    }
}