namespace FreeDB.Web.Queries
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http.OData.Query;
    using Core.Bases;
    using Infrastructure.EntityFramework;

    public interface IODataQueryContext
    {
        ODataQueryOptions SearchParameters { get; set; }
        FreeDbDataContext Context { get; set; }
    }

    public interface IFreeDbQuery<T> : IODataQueryContext where T : PersistentObject
    {
        FreeDbQueryResults<T> GetResults();
    }

    public interface IFreeDbCriteriaQuery<T> : IODataQueryContext where T : PersistentObject
    {
        FreeDbQueryResults<T> GetResults(Expression<Func<T, bool>> criteria);
    }

    public interface IFreeDbScalarQuery<out T> : IODataQueryContext where T : PersistentObject
    {
        T GetResult(object id);
    }

    public class FreeDbQueryResults<T>
    {
        public IQueryable<T> Results { get; private set; }
        public long TotalCount { get; private set; }

        public FreeDbQueryResults(IQueryable<T> results, long totalCount)
        {
            Results = results;
            TotalCount = totalCount;
        }
    }
}
