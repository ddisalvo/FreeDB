namespace FreeDB.Web.Queries
{
    using System.Linq;
    using System.Web.Http.OData.Query;
    using Core.Bases;
    using Core.Model;
    using Infrastructure.EntityFramework;

    public class Query<TEntity> where TEntity : PersistentObject
    {
        private readonly FreeDbDataContext _dataContext;

        public Query(FreeDbDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public FreeDbQueryResults<TEntity> Create<TQuery>(ODataQueryOptions<TEntity> options)
            where TQuery : IFreeDbQuery<TEntity>, new()
        {
            return
                new TQuery {SearchParameters = options, Context = _dataContext}.GetResults();
        }

        public TEntity Create<TQuery>(object id)
            where TQuery : IFreeDbScalarQuery<TEntity>, new()
        {
            return
                new TQuery { Context = _dataContext }.GetResult(id);
        }

        public FreeDbQueryResults<Disc> Create<TQuery>(int[] artistIds, ODataQueryOptions<TEntity> options)
            where TQuery : GetDiscsForArtists, new()
        {
            return
                new TQuery {SearchParameters = options, Context = _dataContext}.GetResults(
                    d => artistIds.Contains(d.Artist.Id));
        }
    }
}