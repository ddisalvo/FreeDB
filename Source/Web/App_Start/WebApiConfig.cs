namespace FreeDB.Web.App_Start
{
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Routing;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{id}",
                                       new {id = RouteParameter.Optional}, new {id = @"\d+"});

            config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}");

            config.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}", new {action = "Get"},
                                       new {httpMethod = new HttpMethodConstraint(HttpMethod.Get)});

            config.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new {action = "Post"},
                                       new {httpMethod = new HttpMethodConstraint(HttpMethod.Post)});

            config.Routes.MapHttpRoute("DefaultApiPut", "api/{controller}", new {action = "Put"},
                                       new {httpMethod = new HttpMethodConstraint(HttpMethod.Put)});

            config.Routes.MapHttpRoute("DefaultApiDelete", "api/{controller}", new {action = "Delete"},
                                       new {httpMethod = new HttpMethodConstraint(HttpMethod.Delete)});

            config.EnableQuerySupport();
        }
    }
}
