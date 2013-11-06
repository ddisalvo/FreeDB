namespace FreeDB.IntegrationTests.Bases
{
    using System.Configuration;
    using System.Data.Entity;
    using System.IO;
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
            CleanIndex();
            CleanStore();
        }

        private static void CleanIndex()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = (ClientSettingsSection)config.GetSection("applicationSettings/FreeDB.Infrastructure.Properties.Settings");
            var indexDirectory = section.Settings.Get("IndexDirectory").Value.ValueXml.InnerText;

            foreach (var file in Directory.GetFiles(indexDirectory))
            {
                File.Delete(file);
            }
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
