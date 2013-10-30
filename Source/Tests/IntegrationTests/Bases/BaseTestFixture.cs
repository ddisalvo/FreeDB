namespace FreeDB.IntegrationTests.Bases
{
    using System.Data.Entity;
    using DependencyResolution;
    using FreeDB.Infrastructure.AutoMapper;
    using NUnit.Framework;

    public abstract class BaseTestFixture : UnitTests.Bases.BaseTestFixture
    {
        [SetUp]
        protected override void SetUp()
        {
            DependencyRegistrar.EnsureDependenciesRegistered();
            AutoMapperConfiguration.Configure();
        }

        [TearDown]
        protected override void TearDown()
        {
            CleanStore();
        }

        private static void CleanStore()
        {
            using (var context = new DbContext("name=FreeDBConnectionString"))
            {
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.Tracks");
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.Discs");
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.Genres");
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.Artists");
            }
        }
    }
}
