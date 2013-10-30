namespace FreeDB.UnitTests.Bases
{
    using NUnit.Framework;

    [TestFixture]
    public abstract class BaseTestFixture : AssertionHelper
    {
        [SetUp]
        protected virtual void SetUp()
        {
        }

        [TearDown]
        protected virtual void TearDown()
        {
        }
    }
}
