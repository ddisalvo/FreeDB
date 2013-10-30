namespace FreeDB.DependencyResolution
{
    using System;
    using System.Linq;
    using Core;
    using Infrastructure.EntityFramework;
    using Infrastructure.EntityFramework.Mappings.Bases;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;

    public class DependencyRegistry : Registry
    {
        public DependencyRegistry()
        {
            var assemblyPrefix = GetThisAssembliesPrefix();

            Scan(x =>
                {
                    GetType()
                        .Assembly.GetReferencedAssemblies()
                        .Where(name => name.Name.StartsWith(assemblyPrefix, StringComparison.CurrentCultureIgnoreCase))
                        .ToList()
                        .ForEach(name => x.Assembly(name.Name));

                         x.Convention<DefaultConventionScanner>();
                         x.AssembliesFromApplicationBaseDirectory(
                             assembly => assembly.GetName().Name.StartsWith(assemblyPrefix, StringComparison.CurrentCultureIgnoreCase));
                         x.LookForRegistries();
                         x.AddAllTypesOf<IRequiresConfigurationOnStartup>();
                         x.AddAllTypesOf<IEntityFrameworkConfiguration>();
                     });

            For<FreeDbDataContext>().HybridHttpOrThreadLocalScoped();
        }

        private string GetThisAssembliesPrefix()
        {
            var name = GetType().Assembly.GetName().Name;
            name = name.Substring(0, name.LastIndexOf(".", StringComparison.Ordinal));

            return name;
        }
    }
}
