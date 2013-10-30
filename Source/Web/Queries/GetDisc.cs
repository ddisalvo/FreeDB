namespace FreeDB.Web.Queries
{
    using Core.Model;

    public class GetDisc : BaseODataQuery<Disc>, IFreeDbScalarQuery<Disc>
    {
        public Disc GetResult(object id)
        {
            return Context.Set<Disc>().Find(id);
        }
    }
}