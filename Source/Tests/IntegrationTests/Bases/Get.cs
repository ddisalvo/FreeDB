namespace FreeDB.IntegrationTests.Bases
{
    using System;
    using System.Linq;
    using Core.Bases;
    using Core.Model;
    using FreeDB.Infrastructure.EntityFramework;
    using NUnit.Framework;

    public class Get : UnitTests.Helpers.Get
    {
        public static T Any<T>() where T : PersistentObject
        {
            using (var ctxt = new FreeDbDataContext())
            {
                var first = ctxt.Set<T>().FirstOrDefault();

                if (first == null)
                {
                    first = New<T>(true);
                    ctxt.Set<T>().Add(first);
                    ctxt.SaveChanges();
                }

                return first;
            }
        }

        public static Disc NewDiscWithArtistAndGenre(bool generateRandom)
        {
            var disc = New<Disc>();
            disc.Id = generateRandom ? new Randomizer().Next() : 100000000000;
            disc.Artist = New<Artist>(generateRandom);
            disc.Genre = New<Genre>(generateRandom);

            return disc;
        }
    }
}
