namespace FreeDB.Infrastructure.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;
    using Mappings.Bases;

    public class FreeDbDataContext : DbContext
    {
        public bool IsDisposed { get; private set; }

        public FreeDbDataContext()
            : base("name=FreeDBConnectionString")
        {
            Database.SetInitializer<FreeDbDataContext>(null);
            IsDisposed = false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            IsDisposed = true;
        }

        public IEnumerable<T> Execute<T>(string functionName, object arguments)
        {
            return Execute(functionName, arguments, Database.SqlQuery<T>);
        }

        public int Execute(string functionName, object arguments)
        {
            return Execute(functionName, arguments, Database.ExecuteSqlCommand);
        }

        public int Execute(string functionName, SqlParameter[] arguments)
        {
            return Execute(functionName, arguments, Database.ExecuteSqlCommand);
        }
        
        private static T Execute<T>(string functionName, object arguments, Func<string, object[], T> sqlQuery)
        {
            var sqlParameters = (from PropertyDescriptor descriptor in TypeDescriptor.GetProperties(arguments)
                                 select new SqlParameter(descriptor.Name, descriptor.GetValue(arguments))).ToArray();

            var sql = functionName;
            if (sqlParameters.Any())
            {
                sql += " @" + string.Join(", @", sqlParameters.Select(p => p.ParameterName));
            }
            return sqlQuery(sql, sqlParameters);
        }

        private static T Execute<T>(string functionName, SqlParameter[] arguments, Func<string, object[], T> sqlQuery)
        {
            var sql = functionName;
            if (arguments.Any())
            {
                sql += " @" + string.Join(", @", arguments.Select(p => p.ParameterName));
            }
            return sqlQuery(sql, arguments);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var configurations =
                GetType().Assembly.GetExportedTypes().Where(
                    x => typeof(IEntityFrameworkConfiguration).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).
                    Select(Activator.CreateInstance);

            foreach (dynamic configuration in configurations)
            {
                modelBuilder.Configurations.Add(configuration);
            }
        }
    }
}
