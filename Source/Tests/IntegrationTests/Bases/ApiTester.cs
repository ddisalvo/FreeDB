namespace FreeDB.IntegrationTests.Bases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.OData;
    using System.Web.Http.SelfHost;
    using Core;
    using Core.Bases;
    using FreeDB.Web;
    using FreeDB.Web.App_Start;
    using FreeDB.Web.Controllers.Api;
    using NUnit.Framework;
    using Newtonsoft.Json;

    [TestFixture]
    public abstract class ApiTester<T> : BaseTestFixture where T : PersistentObject
    {
        protected const string BaseAddress = "http://localhost:19830";
        protected string Controller { get; private set; }
        private HttpSelfHostServer _server;

        protected ApiTester(string controller)
        {
            Controller = controller;
        }

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            var configuration = new HttpSelfHostConfiguration(BaseAddress)
                {
                    IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
                };
            configuration.Services.Replace(typeof (IAssembliesResolver),
                                           new TestAssemblyResolver(GetControllerType()));
            configuration.DependencyResolver = new CustomWebApiDependencyResolver();

            WebApiConfig.Register(configuration);

            _server = new HttpSelfHostServer(configuration);
            _server.OpenAsync().Wait();

            Console.WriteLine("Listening on " + BaseAddress);
        }

        private Type GetControllerType()
        {
            return DependencyResolver.ResolveNamed<BaseApiController>(Controller + "Controller").GetType();
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
        }

        [Test]
        public virtual void Get_Should_Succeed()
        {
            var id1 = Get.Any<T>().Id;

            var client = new HttpClient();
            client.GetStringAsync(BaseAddress + "/api/" + Controller).ContinueWith(getTask =>
                {
                    var collection = JsonConvert.DeserializeObject<PageResult<dynamic>>(getTask.Result);
                    Expect(collection, Is.Not.Null.And.Not.Empty);
                    Expect(collection.Select(p => p.Id.ToString()), Contains(id1.ToString()));
                }).Wait();
        }

        [Test]
        public void Get_By_Id_Should_Succeed()
        {
            var id = Get.Any<T>().Id;

            var client = new HttpClient();
            client.GetStringAsync(BaseAddress + "/api/" + Controller + "/" + id).ContinueWith(getTask =>
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(getTask.Result);
                    Expect(result, Is.Not.Null);
                    Expect(result.Id.ToString(), Is.EqualTo(id.ToString()));
                }).Wait();
        }
    }

    public class TestAssemblyResolver : IAssembliesResolver
    {
        private readonly Type _currentController;
        public TestAssemblyResolver(Type controller)
        {
            _currentController = controller;
        }

        public ICollection<Assembly> GetAssemblies()
        {
            return new[] { _currentController.Assembly };
        }
    }
}
