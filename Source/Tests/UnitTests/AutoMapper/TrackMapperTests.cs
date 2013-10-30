namespace FreeDB.UnitTests.AutoMapper
{
    using Bases;
    using Infrastructure.AutoMapper.ConfigurationProfiles;
    using NUnit.Framework;
    using global::AutoMapper;

    [TestFixture]
    public class TrackMapperTests : BaseTestFixture
    {
        [SetUp]
        protected override void SetUp()
        {
            Mapper.AddProfile<TrackMapper>();
        }

        [Test]
        public void Configuration_Is_Valid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
