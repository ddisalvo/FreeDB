namespace FreeDB.Web.Controllers.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.OData.Query;
    using Core.Common;
    using Models.Dto;
    
    public class BaseApiController : ApiController
    {
        protected TDest Map<TDest>(object source)
            where TDest : class
        {
            return Core.DependencyResolver.Map<TDest>(source);
        }

        protected SearchParameters ConvertODataToSearchParameters(string search, ODataQueryOptions options, int maxPageSize)
        {
            var top = maxPageSize;
            if (options == null)
                return new SearchParameters {PageSize = top, CurrentPage = 1, SearchTerm = search};

            if (options.Top != null)
            {
                int parsedTopValue;
                if (int.TryParse(options.Top.RawValue, out parsedTopValue))
                    top = parsedTopValue > maxPageSize
                              ? maxPageSize
                              : parsedTopValue;
            }

            var orderBy = new string[0];
            var descending = false;
            if (options.OrderBy != null)
            {
                orderBy = options.OrderBy.RawValue.Split(',');
                if (options.OrderBy.RawValue.Substring(options.OrderBy.RawValue.IndexOf(' ')).Contains("desc"))
                    descending = true;
            }

            var skip = 0;
            if (options.Skip != null)
                skip = int.Parse(options.Skip.RawValue);

            var currentPage = (skip/top) + 1;

            return new SearchParameters
                {
                    PageSize = top,
                    SortBy = orderBy,
                    SortDescending = descending,
                    CurrentPage = currentPage,
                    SearchTerm = search
                };
        }

        protected void CalculateDiscHRefs(IEnumerable<DiscSummaryDto> model)
        {
            foreach (var discSummaryDto in model)
            {
                discSummaryDto.HRef = GenerateApiHRef("Discs", discSummaryDto.Id);
                discSummaryDto.ArtistHRef = GenerateApiHRef("Artists", discSummaryDto.ArtistId);
            }
        }

        protected void CalculateArtistHRefs(IEnumerable<ArtistSummaryDto> model)
        {
            foreach (var artistSummaryDto in model)
            {
                artistSummaryDto.HRef = GenerateApiHRef("Artists", artistSummaryDto.Id);
            }
        }

        protected string GenerateApiHRef(string controller, object id)
        {
            if (id == null)
                return null;

            return Url.Route("DefaultApiWithId",
                             new
                                 {
                                     controller,
                                     id
                                 });
        }
    }
}
