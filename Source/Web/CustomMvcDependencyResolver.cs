namespace FreeDB.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class CustomMvcDependencyResolver : IDependencyResolver
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
    }
}