namespace FreeDB.IntegrationTests.Infrastructure.EntityFramework
{
    using System.Linq;
    using Bases;
    using Core.Model;
    using FreeDB.Infrastructure.EntityFramework;
    using NUnit.Framework;

    [TestFixture]
    public class DiscPersistenceTests : PersistenceTester<Disc>
    {
        protected override Disc GetNew(bool generateRandom = false)
        {
            return Get.NewDiscWithArtistAndGenre(generateRandom);
        }

        [Test]
        public void Persist_With_Tracks_Should_Succeed()
        {
            var obj = GetNew();
            obj.Tracks.Add(Get.New<Track>());
            obj.Tracks.Add(Get.New<Track>(true));

            foreach (var track in obj.Tracks)
            {
                track.TrackNumber = obj.Tracks.ToList().IndexOf(track) + 1;
            }

            using (var context = new FreeDbDataContext())
            {
                context.Set<Disc>().Add(obj);
                context.SaveChanges();
            }

            Expect(obj.IsPersistent);

        }
    }
}
