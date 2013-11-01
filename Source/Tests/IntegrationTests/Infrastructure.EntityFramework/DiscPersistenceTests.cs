namespace FreeDB.IntegrationTests.Infrastructure.EntityFramework
{
    using System.Linq;
    using Bases;
    using Core.Model;
    using FreeDB.Infrastructure.EntityFramework;
    using NUnit.Framework;
    using UnitTests.Helpers;

    [TestFixture]
    public class DiscPersistenceTests : PersistenceTester<Disc>
    {
        protected override Disc GetNew(bool generateRandom = false)
        {
            var disc = Get.New<Disc>();
            disc.Id = generateRandom ? new Randomizer().Next() : 100000000000;
            disc.Artist = Get.New<Artist>(generateRandom);
            disc.Genre = Get.New<Genre>(generateRandom);

            return disc;
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
