namespace FreeDB.Web.Queries
{
    using System.Linq;
    using Core.Model;

    public class GetFirstAndLastDiscForArtist : Query<Disc>
    {
        public override IQueryable<Disc> Results()
        {
            return
                new EnumerableQuery<Disc>(new[]
                    {
                        base.Results().OrderBy(d => d.Released).FirstOrDefault(),
                        base.Results().OrderByDescending(d => d.Released).FirstOrDefault()
                    });
        }
    }
}