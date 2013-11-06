namespace FreeDB.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bases;
    using Core;
    using Core.Common;
    using Core.Model.Enumerations;
    using Core.Services;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Properties;
    using Version = Lucene.Net.Util.Version;

    public class SearchService : BaseLuceneService, ISearchService
    {
        private const int MaxPageSize = 25;

        public SearchResults Search(SearchParameters searchParameters)
        {
            if (!IndexReader.IndexExists(Directory))
                throw new Exception("No index present in: " + Settings.Default.IndexDirectory);

            using (var searcher = new IndexSearcher(Directory, true))
            {
                var hits = GetQueryHits(searcher, searchParameters);
                var hitDocs = new List<Document>();

                var pageSize = searchParameters.PageSize > MaxPageSize ? MaxPageSize : searchParameters.PageSize;
                var start = (searchParameters.CurrentPage - 1) * pageSize;
                for (var i = start; i < start + pageSize && i < hits.TotalHits; i++)
                {
                    hitDocs.Add(searcher.Doc(hits.ScoreDocs[i].Doc));
                }

                return new SearchResults(searchParameters, DependencyResolver.Map<FreeDbSearchResult[]>(hitDocs),
                                         hits.TotalHits);
            }
        }

        public IEnumerable<string> Suggest(string search, SearchField suggestedField)
        {
            using (var searcher = new IndexSearcher(Directory, true))
            {
                var terms = searcher.IndexReader.Terms(new Term(suggestedField.ToString(), search));
                var cnt = 0;
                do
                {
                    var currentTerm = terms.Term;

                    if (currentTerm.Field != suggestedField.ToString())
                        yield break;

                    yield return currentTerm.Text;
                    cnt++;
                } while (terms.Next() && cnt < 10);
            }
        }

        private static TopDocs GetQueryHits(Searcher searcher, SearchParameters searchParameters)
        {
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, Enum.GetNames(typeof (SearchField)), analyzer);
                var query = ParseQuery(searchParameters.SearchTerm, parser);
                var sort = ParseSorting(searchParameters);

                var filter = new QueryWrapperFilter(query);
                return sort != null
                           ? searcher.Search(query, filter, int.MaxValue, sort)
                           : searcher.Search(query, filter, int.MaxValue);
            }
        }

        private static BooleanQuery ParseQuery(string searchTerm, QueryParser parser)
        {
            var query = new BooleanQuery();
            searchTerm = searchTerm
                .Replace("+", "")
                .Replace("\"", "")
                .Replace("\'", "");

            var terms = searchTerm.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                query.Add(parser.Parse(term.Replace("*", "") + "*"),
                          searchTerm.Contains("+") ? Occur.MUST : Occur.SHOULD);
            }
            return query;
        }

        private static Sort ParseSorting(SearchParameters searchParameters)
        {
            if (searchParameters.SortBy == null)
                return null;

            return new Sort(
                searchParameters.SortBy.Select(
                    s => new SortField(s.Replace("/", ""), SortField.STRING, searchParameters.SortDescending))
                                .ToArray());
        }
    }
}
