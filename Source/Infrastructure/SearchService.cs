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
    using Lucene;
    using Properties;
    using global::Lucene.Net.Analysis.Standard;
    using global::Lucene.Net.Documents;
    using global::Lucene.Net.Index;
    using global::Lucene.Net.QueryParsers;
    using global::Lucene.Net.Search;
    using Version = global::Lucene.Net.Util.Version;

    public class SearchService : BaseLuceneService, ISearchService
    {
        private const int MaxPageSize = 25;

        public SearchResults Search(SearchParameters searchParameters)
        {
            if (!IndexReader.IndexExists(Directory))
                throw new Exception("No index present in: " + Settings.Default.IndexDirectory);

            using (var searcher = new IndexSearcher(Directory, true))
            {
                searchParameters.PageSize = searchParameters.PageSize > MaxPageSize ? MaxPageSize : searchParameters.PageSize;

                var hits = GetQueryHits(searcher, searchParameters);
                var hitDocs = new List<Document>();

                var start = (searchParameters.CurrentPage - 1) * searchParameters.PageSize;
                for (var i = start; i < start + searchParameters.PageSize && i < hits.TotalHits; i++)
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
            if (!String.IsNullOrWhiteSpace(searchParameters.GroupBy) && searchParameters.SortBy != null && searchParameters.SortBy.Any())
                throw new NotSupportedException("Cannot sort AND group by search query.");

            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, Enum.GetNames(typeof (SearchField)), analyzer);
                var query = ParseQuery(searchParameters.SearchTerm, parser);
                var sort = ParseSorting(searchParameters);
                var filter = new QueryWrapperFilter(query);

                if (sort != null && sort.GetSort().Any())
                    return searcher.Search(query, filter, searchParameters.PageSize, sort);

                if (!String.IsNullOrWhiteSpace(searchParameters.GroupBy))
                {
                    var groupedValues = new Dictionary<string, int>();
                    searcher.Search(query, filter, new DelegatingCollector((reader, doc) =>
                        {
                            var groupByValue = searcher.Doc(doc).Get(searchParameters.GroupBy);
                            if (String.IsNullOrWhiteSpace(groupByValue))
                                return;

                            if (!groupedValues.ContainsKey(groupByValue))
                                groupedValues.Add(groupByValue, doc);
                        }));
                    return new TopDocs(groupedValues.Count,
                                       groupedValues.Select(g => new ScoreDoc(g.Value, 0)).ToArray(), 0);
                }

                return searcher.Search(query, filter, searchParameters.PageSize);
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
