namespace FreeDB.IntegrationTests.Web.Api
{
    using Bases;
    using Core.Model;
    using NUnit.Framework;

    [TestFixture]
    public class ArtistsApiTests : ApiTester<Artist>
    {
        public ArtistsApiTests()
            : base("Artists")
        {
        }
    }
}
