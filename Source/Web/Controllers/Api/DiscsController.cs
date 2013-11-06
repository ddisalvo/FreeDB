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
    using Core.Model.Enumerations;
    using Core.Services;
    using Infrastructure.EntityFramework;
    using Models.Dto;
    using Queries;

    public class DiscsController : BaseApiController
    {
        private readonly ISearchService _searchService;

        public DiscsController(ISearchService searchService, FreeDbDataContext dataContext)
            : base(dataContext)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public PageResult<DiscSummaryDto> Get(ODataQueryOptions<Disc> options)
        {
            var results =
                GetQueryHelper<Disc>()
                    .Create<GetDiscs>(options);

            var withArtist = results.Results.Select(d => new { Disc = d, d.Artist, d.Genre });
            var model = Map<IEnumerable<DiscSummaryDto>>(withArtist.Select(x => x.Disc)).ToArray();
            CalculateDiscHRefs(model);

            return new PageResult<DiscSummaryDto>(model, null, results.TotalCount);
        }

        [HttpGet]
        public HttpResponseMessage Get(long id)
        {
            var disc = GetQueryHelper<Disc>().Create<GetDisc>(id);
            if (disc == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "No disc found");

            var model = Map<DiscDto>(disc);
            model.ArtistHRef = GenerateApiHRef("Artists", model.ArtistId);
            model.CalculateTrackTimes();

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        public PageResult<DiscSummaryDto> Search(string search, ODataQueryOptions<Disc> options)
        {
            var results = _searchService.Search(ConvertODataToSearchParameters(search, options));
            var model = Map<IEnumerable<DiscSummaryDto>>(results.Results).ToArray();
            CalculateDiscHRefs(model);

            return new PageResult<DiscSummaryDto>(model, null, results.TotalCount);
        }

        [HttpGet]
        public IEnumerable<string> Suggest(string search)
        {
            var results = _searchService.Suggest(search, SearchField.DiscTitle);
            return results;
        }
    }
}
