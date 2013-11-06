namespace FreeDB.IntegrationTests.Infrastructure
{
    using System;
    using System.Linq;
    using Bases;
    using Core.Common;
    using Core.Model;
    using Core.Model.Enumerations;
    using FreeDB.Infrastructure;
    using NUnit.Framework;
    using UnitTests.Helpers;

    [TestFixture]
    public class SearchServiceTests : BaseTestFixture
    {
        [Test]
        public void Search_Disc_Exact_Should_Succeed()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();
            var searchParameters = new SearchParameters {CurrentPage = 1, PageSize = 10, SearchTerm = disc.Title};

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
            Expect(results.Results.Select(r => r.DiscTitle).Contains(disc.Title));
        }

        [Test]
        public void Search_Disc_Fuzzy_Should_Succeed()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

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
            Expect(results.Results.Select(r => r.DiscTitle).Contains(disc.Title));
        }

        [Test]
        public void Search_Artist_Exact_Should_Succeed()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();
            var searchParameters = new SearchParameters {CurrentPage = 1, PageSize = 10, SearchTerm = disc.Artist.Name};

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
            Expect(results.Results.Select(r => r.ArtistName).Contains(disc.Artist.Name));
        }

        [Test]
        public void Suggest_Disc_Should_Succeed()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();

            var suggestions = service.Suggest(disc.Title.Substring(0, 4), SearchField.DiscTitle).ToArray();

            Expect(suggestions, Is.Not.Null.And.Not.Empty);
            Expect(suggestions.All(s => s.IndexOf(disc.Title, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private void AddDiscToIndex(Disc disc)
        {
            var indexService = new IndexService();
            disc.Artist = Get.New<Artist>(true);

            Expect(() => indexService.Add(new[] { new { ArtistName = disc.Artist.Name, DiscTitle = disc.Title } }),
                   Throws.Nothing);
        }
    }
}
