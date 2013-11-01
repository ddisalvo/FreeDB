namespace FreeDB.Web.Queries
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Core.Model;

    public class GetDiscsForArtists : BaseODataQuery<Disc>, IFreeDbCriteriaQuery<Disc>
    {
        public FreeDbQueryResults<Disc> GetResults(Expression<Func<Disc, bool>> criteria)
        {
            var discs = Context.Set<Disc>()
                               .Include(d => d.Tracks)
                               .Where(criteria)
                               .OrderBy(d => d.Released);
            var results = ApplyODataQuery(discs, SearchParameters);
            return
                new FreeDbQueryResults<Disc>(results, discs.LongCount());
        }
    }
}