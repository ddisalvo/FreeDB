namespace FreeDB.Web.Queries
{
    using System.Linq;
    using Core.Model;

    public class GetDiscs : BaseODataQuery<Disc>, IFreeDbQuery<Disc>
    {
        public FreeDbQueryResults<Disc> GetResults()
        {
            return new FreeDbQueryResults<Disc>(ApplyODataQuery(Context.Set<Disc>(), SearchParameters),
                                                Context.Set<Disc>().LongCount());
        }
    }
}