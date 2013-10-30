namespace FreeDB.Core.Common
{
    public class SearchResults
    {
        public FreeDbSearchResult[] Results { get; private set; }
        public long TotalCount { get; private set; }
        public SearchParameters SearchParameters { get; private set; }

        public SearchResults(SearchParameters search, FreeDbSearchResult[] results, long totalCount)
        {
            SearchParameters = search;
            Results = results;
            TotalCount = totalCount;
        }
    }
}
