namespace FreeDB.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class DependencyResolver
    {
        public static Func<Type, object> CreateDependency = type => Activator.CreateInstance(type);
        public static Func<Type, string, object> CreateNamedDependency = (type, key) => Activator.CreateInstance(type);
        public static Action<string> RegisterPlugins = siteKey => { };
        public static Func<object, Type, Type, object> Mapper;
        public static Func<object, object, Type, Type, object> MapperWithDestination;
        public static Func<Type, IList> CreateAllDependencies = type => type.IsInterface ? new List<object>() : new List<object> { Activator.CreateInstance(type) };

        public static T Resolve<T>()
        {
            return (T)CreateDependency(typeof(T));
        }

        public static T ResolveNamed<T>(string key)
        {
            return (T)CreateNamedDependency(typeof(T), key);
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return CreateAllDependencies(typeof(T)).OfType<T>();
        }

        public static TProjection Map<TModel, TProjection>(TModel model)
        {
            return (TProjection)Mapper(model, typeof(TModel), typeof(TProjection));
        }

        public static TProjection Map<TModel, TProjection>(TModel model, TProjection destination)
        {
            return (TProjection)MapperWithDestination(model, destination, typeof(TModel), typeof(TProjection));
        }

        public static TProjection Map<TProjection>(object source)
            where TProjection : class
        {
            if (source == null)
            {
                return null;
            }

            var sourceType = source.GetType();
            return (TProjection)Mapper(source, sourceType, typeof(TProjection));
        }
    }
}
