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

            Expect(() => indexService.Add(new[] { disc }), Throws.Nothing);

            var service = new SearchService();
            var searchParameters = new SearchParameters {PageSize = 10, SearchTerm = disc.Title};

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
        }
    }
}
