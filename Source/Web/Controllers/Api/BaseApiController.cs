namespace FreeDB.Web.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.OData.Query;
    using Core.Bases;
    using Core.Common;
    using Core.Model;
    using Infrastructure.EntityFramework;
    using Models.Dto;
    using Queries;

    public class BaseApiController : ApiController
    {
        private readonly FreeDbDataContext _dataContext;

        protected BaseApiController(FreeDbDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        protected TDest Map<TDest>(object source)
            where TDest : class
        {
            return Core.DependencyResolver.Map<TDest>(source);
        }

        protected IQueryable<Disc> Discs
        {
            get { return _dataContext.Set<Disc>(); }
        }

        protected IQueryable<Artist> Artists
        {
            get { return _dataContext.Set<Artist>(); }
        }

        public Query<T> GetQueryHelper<T>() where T : PersistentObject
        {
            return new Query<T>(_dataContext);
        }

        protected SearchParameters ConvertODataToSearchParameters(string search, ODataQueryOptions options)
        {
            var top = BaseODataQuery<object>.MaxPageSize;
            if (options == null)
                return new SearchParameters {PageSize = top, CurrentPage = 1, SearchTerm = search};

            if (options.Top != null)
            {
                int parsedTopValue;
                if (int.TryParse(options.Top.RawValue, out parsedTopValue))
                    top = parsedTopValue > BaseODataQuery<object>.MaxPageSize
                              ? BaseODataQuery<object>.MaxPageSize
                              : parsedTopValue;
            }

            var orderBy = new string[0];
            var descending = false;
            if (options.OrderBy != null)
            {
                orderBy = options.OrderBy.RawValue.Split('/');
                if (!options.OrderBy.RawValue.Substring(options.OrderBy.RawValue.IndexOf(' ')).Contains("desc"))
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
