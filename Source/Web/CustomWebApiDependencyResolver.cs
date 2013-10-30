namespace FreeDB.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;


    public class CustomWebApiDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            try
            {
                return Core.DependencyResolver.CreateDependency(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return Core.DependencyResolver.CreateAllDependencies(serviceType).OfType<object>();
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}