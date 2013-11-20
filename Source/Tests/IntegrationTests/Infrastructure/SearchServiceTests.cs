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
        public void Search_Disc_Not_Found()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();
            var searchParameters = new SearchParameters
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    SearchTerm = new string(disc.Title.Reverse().ToArray())
                };

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Empty);
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
        public void Search_Artist_GroupBy_Name_Should_Succeed()
        {
            var artist = Get.New<Artist>(true);
            var disc1 = Get.New<Disc>(true);
            var disc2 = Get.New<Disc>(true);
            AddDiscToIndex(disc1, artist);
            AddDiscToIndex(disc2, artist);

            var service = new SearchService();
            var searchParameters = new SearchParameters
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    SearchTerm = artist.Name,
                    GroupBy = SearchField.ArtistName.ToString()
                };

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results.Length, Is.EqualTo(1));
            Expect(results.Results.Single().ArtistName, Is.EqualTo(artist.Name));
        }

        [Test]
        public void Search_Artist_Fuzzy_Should_Succeed()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();
            var searchParameters = new SearchParameters
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    SearchTerm = disc.Artist.Name.Substring(0, 6)
                };

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Not.Null.And.Not.Empty);
            Expect(results.Results.Select(r => r.ArtistName).Contains(disc.Artist.Name));
        }

        [Test]
        public void Search_Artist_Not_Found()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();
            var searchParameters = new SearchParameters
                {
                    CurrentPage = 1,
                    PageSize = 10,
                    SearchTerm = new string(disc.Artist.Name.Reverse().ToArray())
                };

            var results = service.Search(searchParameters);

            Expect(results, Is.Not.Null);
            Expect(results.Results, Is.Empty);
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

        [Test]
        public void Suggest_Artist_Should_Succeed()
        {
            var disc = Get.New<Disc>(true);
            AddDiscToIndex(disc);

            var service = new SearchService();

            var suggestions = service.Suggest(disc.Artist.Name.Substring(0, 4), SearchField.ArtistName).ToArray();

            Expect(suggestions, Is.Not.Null.And.Not.Empty);
            Expect(suggestions.All(s => s.IndexOf(disc.Artist.Name, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private void AddDiscToIndex(Disc disc, Artist artist = null)
        {
            var indexService = new IndexService();
            disc.Artist = artist ?? Get.New<Artist>(true);

            Expect(() => indexService.Add(new[] { new { ArtistName = disc.Artist.Name, DiscTitle = disc.Title } }),
                   Throws.Nothing);
        }
    }
}
