namespace FreeDB.Core.Common
{
    public class SearchParameters
    {
        public string SearchTerm { get; set; }
        public string[] SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
