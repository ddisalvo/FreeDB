namespace FreeDB.IntegrationTests.Infrastructure
{
    using Bases;
    using Core.Model;
    using FreeDB.Infrastructure;
    using NUnit.Framework;

    [TestFixture]
    public class IndexServiceTests : BaseTestFixture
    {
        [Test]
        public void Add_To_Index_Should_Succeed()
        {
            var service = new IndexService();
            var disc = Get.New<Disc>();

            Expect(() => service.Add(new[] { disc }), Throws.Nothing);
        }
    }
}
