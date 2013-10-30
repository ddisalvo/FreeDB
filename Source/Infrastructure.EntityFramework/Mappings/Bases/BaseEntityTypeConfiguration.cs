namespace FreeDB.Infrastructure.EntityFramework.Mappings.Bases
{
    using System;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class BaseEntityTypeConfiguration<T> : EntityTypeConfiguration<T>,
                                                           IEntityFrameworkConfiguration
        where T : class
    {
        protected BaseEntityTypeConfiguration(string tableName = null)
        {
            if (tableName != null)
            {
                ToTable(tableName);
            }
        }

        protected Expression<Func<T, TResult>> GetExpression<TResult>(string propertyName)
        {
            return GetExpression<T, TResult>(propertyName);
        }

        protected Expression<Func<TSource, TResult>> GetExpression<TSource, TResult>(string propertyName)
        {
            var prop = typeof (TSource).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            var x = Expression.Parameter(typeof (TSource), "x");
            var body = Expression.Property(x, prop);
            return (Expression<Func<TSource, TResult>>) Expression.Lambda(body, x);
        }
    }
}
