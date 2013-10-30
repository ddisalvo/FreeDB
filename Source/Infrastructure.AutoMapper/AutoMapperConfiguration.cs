namespace FreeDB.Infrastructure.AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using global::AutoMapper;

    public class AutoMapperConfiguration : IRequiresConfigurationOnStartup
    {
        public static void Configure()
        {
            DependencyResolver.Mapper = Mapper.Map;
            DependencyResolver.MapperWithDestination = Mapper.Map;

            Mapper.Initialize(x =>
                                  {
                                      x.ConstructServicesUsing(type => DependencyResolver.CreateDependency(type));
                                      GetProfiles().ToList().ForEach(
                                          type => x.AddProfile((Profile) DependencyResolver.CreateDependency(type)));
                                  });
        }

        private static IEnumerable<Type> GetProfiles()
        {
            return
                typeof(AutoMapperConfiguration).Assembly.GetTypes().Where(
                    type => !type.IsAbstract && typeof(Profile).IsAssignableFrom(type));
        }

        void IRequiresConfigurationOnStartup.Configure()
        {
            Configure();
        }
    }
}
