namespace FreeDB.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Bases;
    using Core;
    using Core.Common;
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
            if (searchParameters == null)
                throw new ArgumentNullException("searchParameters");

            if (String.IsNullOrWhiteSpace(searchParameters.SearchTerm))
                throw new ArgumentException("A search term is required");

            if (!IndexReader.IndexExists(Directory))
                throw new Exception("No index present in: " + Settings.Default.IndexDirectory);

            using (var searcher = new IndexSearcher(Directory, true))
            {
                using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
                {
                    var pageSize = searchParameters.PageSize > MaxPageSize ? MaxPageSize : searchParameters.PageSize;
                    var parser = new MultiFieldQueryParser(Version.LUCENE_30,
                                                           new[] {"ArtistName", "DiscTitle", "Tracks", "Genre"},
                                                           analyzer);
                    var query = ParseQuery(searchParameters.SearchTerm, parser);
                    var start = (searchParameters.CurrentPage - 1) * pageSize; 
                    var hits = searcher.Search(query, null, int.MaxValue);
                    var hitDocs = new List<Document>();
                    for (var i = start; i < start + pageSize && i < hits.TotalHits; i++)
                    {
                        hitDocs.Add(searcher.Doc(hits.ScoreDocs[i].Doc));
                    }

                    return new SearchResults(searchParameters, DependencyResolver.Map<FreeDbSearchResult[]>(hitDocs), hits.TotalHits);
                }
            }
        }

        private static Query ParseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        } 
    }
}
