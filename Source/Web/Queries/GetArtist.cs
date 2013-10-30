namespace FreeDB.Web.Queries
{
    using Core.Model;

    public class GetArtist : BaseODataQuery<Artist>, IFreeDbScalarQuery<Artist>
    {
        public Artist GetResult(object id)
        {
            return Context.Set<Artist>().Find(id);
        }
    }
}