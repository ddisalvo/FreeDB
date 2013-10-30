namespace FreeDB.UnitTests.AutoMapper
{
    using Bases;
    using Infrastructure.AutoMapper.ConfigurationProfiles;
    using NUnit.Framework;
    using global::AutoMapper;

    [TestFixture]
    public class DiscMapperTests : BaseTestFixture
    {
        [SetUp]
        protected override void SetUp()
        {
            Mapper.AddProfile<TrackMapper>();
            Mapper.AddProfile<ArtistMapper>();
            Mapper.AddProfile<DiscMapper>();
        }

        [Test]
        public void Configuration_Is_Valid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
