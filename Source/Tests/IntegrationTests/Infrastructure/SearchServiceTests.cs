namespace FreeDB.IntegrationTests.Infrastructure
{
    using Bases;
    using Core.Common;
    using Core.Model;
    using FreeDB.Infrastructure;
    using NUnit.Framework;
    using UnitTests.Helpers;

    [TestFixture]
    public class SearchServiceTests : BaseTestFixture
    {
        [Test]
        public void Search_Disc_Exact_Should_Succeed()
        {
            var indexService = new IndexService();
            var disc = Get.New<Disc>();

            Expect(() => indexService.Add(new[] {disc}), Throws.Nothing);

            var service = new SearchService();
            var searchParameters = new SearchParameters {CurrentPage = 1, PageSize = 10, SearchTerm = disc.Title};

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Search_Disc_Fuzzy_Should_Succeed()
        {
            var indexService = new IndexService();
            var disc = Get.New<Disc>();

            Expect(() => indexService.Add(new[] {disc}), Throws.Nothing);

            var service = new SearchService();
            var searchParameters = new SearchParameters
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    SearchTerm = disc.Title.Substring(0, 6)
                };

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Search_Artist_Exact_Should_Succeed()
        {
            var indexService = new IndexService();
            var disc = Get.New<Disc>();
            disc.Artist = Get.New<Artist>(true);

            Expect(() => indexService.Add(new[] {new {ArtistName = disc.Artist.Name, DiscTitle = disc.Title}}),
                   Throws.Nothing);

            var service = new SearchService();
            var searchParameters = new SearchParameters {CurrentPage = 1, PageSize = 10, SearchTerm = disc.Artist.Name};

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
        }
    }
}
