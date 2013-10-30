namespace FreeDB.Web.Queries
{
    using System.Linq;
    using Core.Model;

    public class GetArtists : BaseODataQuery<Artist>, IFreeDbQuery<Artist>
    {
        public FreeDbQueryResults<Artist> GetResults()
        {
            return new FreeDbQueryResults<Artist>(ApplyODataQuery(Context.Set<Artist>(), SearchParameters),
                                                  Context.Set<Artist>().LongCount());
        }
    }
}