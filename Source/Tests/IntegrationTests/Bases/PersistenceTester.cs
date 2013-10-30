namespace FreeDB.IntegrationTests.Bases
{
    using System.Linq;
    using Core.Bases;
    using FreeDB.Infrastructure.EntityFramework;
    using NUnit.Framework;
    using UnitTests.Helpers;

    [TestFixture]
    public abstract class PersistenceTester<T> : BaseTestFixture where T : PersistentObject
    {
        [Test]
        public virtual void Persist_Should_Succeed()
        {
            var obj = GetNew();

            using (var context = new FreeDbDataContext())
            {
                context.Set<T>().Add(obj);
                context.SaveChanges();
            }

            Expect(obj.IsPersistent);
        }

        [Test]
        public virtual void Get_All_Should_Succeed()
        {
            var obj1 = GetNew();
            var obj2 = GetNew(true);

            using (var context = new FreeDbDataContext())
            {
                context.Set<T>().Add(obj1);
                context.Set<T>().Add(obj2);
                context.SaveChanges();
            }

            using (var context = new FreeDbDataContext())
            {
                var all = context.Set<T>().ToList();

                Expect(all.Contains(obj1));
                Expect(all.Contains(obj2));
            }
        }

        [Test]
        public virtual void Delete_Should_Succeed()
        {
            var obj = GetNew();

            using (var context = new FreeDbDataContext())
            {
                context.Set<T>().Add(obj);
                context.SaveChanges();
            }

            Expect(obj.IsPersistent);

            using (var context = new FreeDbDataContext())
            {
                context.Set<T>().Attach(obj);
                context.Set<T>().Remove(obj);
                context.SaveChanges();
            }

            using (var context = new FreeDbDataContext())
            {
                Expect(context.Set<T>().ToArray(), Is.Empty);
            }
        }

        protected virtual T GetNew(bool generateRandom = false)
        {
            return Get.New<T>(generateRandom);
        }

        protected virtual TK AnyFromDb<TK>() where TK : PersistentObject
        {
            using (var context = new FreeDbDataContext())
            {
                var fromDb = context.Set<TK>().FirstOrDefault() ?? Get.New<TK>();

                if (!fromDb.IsPersistent)
                {
                    context.Set<TK>().Add(fromDb);
                    context.SaveChanges();
                }

                return fromDb;
            }
        }
    }
}
