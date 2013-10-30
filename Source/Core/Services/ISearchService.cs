namespace FreeDB.Core.Services
{
    using Common;

    public interface ISearchService
    {
        SearchResults Search(SearchParameters searchParameters);
    }
}
