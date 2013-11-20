namespace FreeDB.Web.Queries
{
    using System.Web.Http.OData.Query;
    using Infrastructure.EntityFramework;

    public interface IDataContextAware
    {
        FreeDbDataContext DataContext { get; set; }
        ODataQueryOptions ODataQueryOptions { get; set; }
        int MaxPageSize { get; set; }
    }
}