namespace FreeDB.Web.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Query;
    using Core.Model;
    using Infrastructure.EntityFramework;
    using Models.Dto;
    using Queries;

    public class ArtistsController : BaseApiController
    {
        public ArtistsController(FreeDbDataContext dataContext)
            : base(dataContext)
        {
        }

        [HttpGet]
        public PageResult<ArtistSummaryDto> Get(ODataQueryOptions<Artist> options)
        {
            var results = GetQueryHelper<Artist>()
                .Create<GetArtists>(options);

            var model = Map<IEnumerable<ArtistSummaryDto>>(results.Results).ToArray();
            CalculateArtistHRefs(model);
            QueryNumberOfDiscs(model);

            return new PageResult<ArtistSummaryDto>(model, null, results.TotalCount);
        }

        [HttpGet]
        public HttpResponseMessage Get(int id, ODataQueryOptions<Disc> discOptions)
        {
            var artist = GetQueryHelper<Artist>().Create<GetArtist>(id);
            if (artist == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "No artist found");

            var model = Map<ArtistDto>(artist);
            LoadPagedDiscs(model, discOptions);

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        private void LoadPagedDiscs(ArtistDto model, ODataQueryOptions<Disc> options)
        {
            var results = GetQueryHelper<Disc>().Create<GetDiscsForArtists>(new[] {model.Id}, options);
            model.Discs = new PageResult<DiscDto>(results.Results.Select(Map<DiscDto>), null, results.TotalCount);
            CalculateDiscHRefs(model.Discs);
        }

        private void QueryNumberOfDiscs(ArtistSummaryDto[] model)
        {
            var artistIds = model.Select(a => a.Id).ToArray();
            var artistsDiscs = GetQueryHelper<Disc>().Create<GetDiscsForArtists>(artistIds, null)
                .Results.GroupBy(d => d.Artist)
                .Select(g => new {Artist = g.Key, NumberOfDiscs = g.Count()});

            foreach (var discGrouping in artistsDiscs)
            {
                var summary = model.Single(a => a.Id == discGrouping.Artist.Id);
                summary.NumberOfDiscs = discGrouping.NumberOfDiscs;
            }
        }
    }
}
