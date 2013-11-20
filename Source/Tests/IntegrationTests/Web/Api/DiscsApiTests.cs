namespace FreeDB.IntegrationTests.Web.Api
{
    using Bases;
    using Core.Model;
    using NUnit.Framework;

    [TestFixture]
    public class DiscsApiTests : ApiTester<Disc>
    {
        public DiscsApiTests()
            : base("Discs")
        {
        }
    }
}
