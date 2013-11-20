namespace FreeDB.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Query;
    using Core.Common;
    using Core.Model;
    using Core.Model.Enumerations;
    using Core.Services;
    using Models.Dto;
    using Queries;

    public class ArtistsController : BaseApiController
    {
        private readonly ISearchService _searchService;
        private readonly QueryFactory _queryFactory;

        public ArtistsController(QueryFactory queryFactory, ISearchService searchService)
        {
            _searchService = searchService;
            _queryFactory = queryFactory;
        }

        [HttpGet]
        public PageResult<ArtistSummaryDto> Get(ODataQueryOptions<Artist> options)
        {
            var query = _queryFactory.CreateQuery<GetArtists>(options);
            var model = Map<IEnumerable<ArtistSummaryDto>>(query.Results()).ToArray();
            CalculateArtistHRefs(model);
            QueryNumberOfDiscs(model);

            return new PageResult<ArtistSummaryDto>(model, null, query.TotalCount);
        }

        [HttpGet]
        public HttpResponseMessage Get(int id, ODataQueryOptions<Disc> discOptions)
        {
            var artist = _queryFactory
                        .CreateQuery<GetArtist>()
                        .WhereIdIs(id)
                        .Result();

            if (artist == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "No artist found");

            var model = Map<ArtistDto>(artist);
            LoadPagedDiscs(model, discOptions);
            QueryYearsActive(model);

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        public PageResult<ArtistSummaryDto> Search(string search, ODataQueryOptions<Artist> options)
        {
            var searchParameters = ConvertODataToSearchParameters(search, options, QueryFactory.MaxPageSize);
            searchParameters.GroupBy = SearchField.ArtistName.ToString();

            var results = _searchService.Search(searchParameters);
            var model = Map<IEnumerable<ArtistSummaryDto>>(results.Results).ToArray();
            CalculateArtistHRefs(model);
            QueryNumberOfDiscs(model);

            return new PageResult<ArtistSummaryDto>(model, null, results.TotalCount);
        }

        [HttpGet]
        public IEnumerable<string> Suggest(string search)
        {
            var results = _searchService.Suggest(search, SearchField.ArtistName);
            return results;
        }

        private void LoadPagedDiscs(ArtistDto model, ODataQueryOptions<Disc> options)
        {
            var query = _queryFactory
                .CreateQuery<GetDiscsForArtists>(options)
                .Include(d => d.Tracks)
                .WhereCriteria(d => d.Artist.Id == model.Id);

            var discs = Map<IEnumerable<DiscDto>>(query.Results());
            model.Discs = new PageResult<DiscDto>(discs, null, query.TotalCount);
            model.NumberOfDiscs = (int)query.TotalCount;
            CalculateDiscHRefs(model.Discs);
        }

        private void QueryYearsActive(ArtistDto model)
        {
            var results = _queryFactory
                .CreateQuery<GetFirstAndLastDiscForArtist>()
                .WhereCriteria(d => d.Artist.Id == model.Id && d.Released.HasValue)
                .Results();

            model.YearsActive =
                StringExtensions.ToYearRange(Tuple.Create(results.FirstOrDefault(), results.LastOrDefault()));
        }

        private void QueryNumberOfDiscs(ArtistSummaryDto[] model)
        {
            var artistIds = model.Select(a => a.Id).ToArray();
            var query = _queryFactory
                .CreateQuery<GetDiscsForArtists>()
                .WhereCriteria(d => artistIds.Contains(d.Artist.Id))
                .Results()
                .GroupBy(d => d.Artist)
                .Select(g => new {Artist = g.Key, NumberOfDiscs = g.Count()});

            foreach (var discGrouping in query)
            {
                var summary = model.Single(a => a.Id == discGrouping.Artist.Id);
                summary.NumberOfDiscs = discGrouping.NumberOfDiscs;
            }
        }
    }
}
