namespace FreeDB.UnitTests.Helpers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.Model;
    using NUnit.Framework;

    public class Get
    {
        protected readonly Randomizer Randomizer = new Randomizer(Randomizer.RandomSeed);
        protected static Get Instance { get; set; }

        protected Get()
        {
        }

        public static T New<T>(bool generateRandom = false) where T : class
        {
            return (Instance ?? (Instance = new Get())).CreateNew<T>(generateRandom);
        }

        protected T CreateNew<T>(bool generateRandom) where T : class
        {
            var type = typeof (T);
            var factories = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "New" + type.Name)
                .Select(m => new Func<T>(() => m.Invoke(this, new object[] {generateRandom}) as T))
                .ToList();

            var createNewT = factories.FirstOrDefault();
            if (createNewT == null)
                throw new Exception("No method to create instance of type " + type.Name);

            return createNewT();
        }

        protected virtual Artist NewArtist(bool generateRandom)
        {
            return new Artist
                {
                    Name = generateRandom ? "TestArtist" + Randomizer.Next() : "TestArtist"
                };
        }

        protected virtual Disc NewDisc(bool generateRandom)
        {
            return new Disc
                {
                    LengthInSeconds = generateRandom ? Randomizer.Next() : 100,
                    Released =
                        generateRandom ? DateTime.Now.Subtract(TimeSpan.FromDays(Randomizer.Next(10000))) : DateTime.Now,
                    Title = generateRandom ? "TestTitle" + Randomizer.Next() : "TestTitle"
                };
        }

        protected virtual Genre NewGenre(bool generateRandom)
        {
            return new Genre
            {
                Title = generateRandom ? "TestGenre" + Randomizer.Next() : "TestGenre"
            };
        }

        protected virtual Track NewTrack(bool generateRandom)
        {
            return new Track
            {
                Title = generateRandom ? "TestTitle" + Randomizer.Next() : "TestTitle"
            };
        }
    }
}
