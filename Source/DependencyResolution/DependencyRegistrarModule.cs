namespace FreeDB.DependencyResolution
{
    using System.Web;
    using StructureMap;

    public class DependencyRegistrarModule : IHttpModule
    {
        private static bool _dependenciesRegistered;
        private static readonly object Lock = new object();

        public void Init(HttpApplication context)
        {
            EnsureDependenciesRegistered();
        }

        public void Dispose() { }

        private static void EnsureDependenciesRegistered()
        {
            if (_dependenciesRegistered) return;
            lock (Lock)
            {
                if (_dependenciesRegistered) return;
                ObjectFactory.ResetDefaults();
                DependencyRegistrar.ConfigureOnStartup();
                _dependenciesRegistered = true;
            }
        }
    }
}
