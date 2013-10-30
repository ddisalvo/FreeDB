namespace FreeDB.DependencyResolution
{
    using System;
    using Core;
    using StructureMap;

    public class DependencyRegistrar
    {
        private static bool _dependenciesRegistered;
        private static readonly object Sync = new object();

        private static void RegisterDependencies()
        {
            ObjectFactory.Initialize(x => x.Scan(y =>
                {
                    y.AssemblyContainingType<DependencyRegistry>();
                    y.LookForRegistries();
                    y.AddAllTypesOf<IRequiresConfigurationOnStartup>();
                }));

            DependencyResolver.CreateDependency = ObjectFactory.GetInstance;
            DependencyResolver.CreateAllDependencies = ObjectFactory.GetAllInstances;
            DependencyResolver.Mapper = AutoMapper.Mapper.Map;
            DependencyResolver.MapperWithDestination = AutoMapper.Mapper.Map;
        }

        internal static void ConfigureOnStartup()
        {
            RegisterDependencies();
            var dependenciesToInitialized = ObjectFactory.GetAllInstances<IRequiresConfigurationOnStartup>();
            foreach (var dependency in dependenciesToInitialized)
            {
                dependency.Configure();
            }
        }

        public static T Resolve<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }

        public static object Resolve(Type modelType)
        {
            return ObjectFactory.GetInstance(modelType);
        }

        public static bool Registered(Type type)
        {
            EnsureDependenciesRegistered();
            return ObjectFactory.GetInstance(type) != null;
        }

        public static void EnsureDependenciesRegistered()
        {
            lock (Sync)
            {
                if (!_dependenciesRegistered)
                {
                    ConfigureOnStartup();
                    _dependenciesRegistered = true;
                }
            }
        }
    }
}
