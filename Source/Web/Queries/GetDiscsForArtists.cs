namespace FreeDB.Web.Queries
{
    using System.Linq;
    using Core.Model;

    public class GetDiscsForArtists : Query<Disc>
    {
        public override IQueryable<Disc> Results()
        {
            return base.Results().OrderBy(d => d.Released);
        }
    }
}