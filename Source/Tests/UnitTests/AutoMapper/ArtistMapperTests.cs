namespace FreeDB.UnitTests.AutoMapper
{
    using Bases;
    using Infrastructure.AutoMapper.ConfigurationProfiles;
    using NUnit.Framework;
    using global::AutoMapper;

    [TestFixture]
    public class ArtistMapperTests : BaseTestFixture
    {
        [SetUp]
        protected override void SetUp()
        {
            Mapper.AddProfile<TrackMapper>();
            Mapper.AddProfile<DiscMapper>();
            Mapper.AddProfile<ArtistMapper>();
        }

        [Test]
        public void Configuration_Is_Valid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
