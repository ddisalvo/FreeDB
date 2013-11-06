namespace FreeDB.Core.Services
{
    using System.Collections.Generic;
    using Common;
    using Model.Enumerations;

    public interface ISearchService
    {
        SearchResults Search(SearchParameters searchParameters);
        IEnumerable<string> Suggest(string search, SearchField suggestedField);
    }
}
