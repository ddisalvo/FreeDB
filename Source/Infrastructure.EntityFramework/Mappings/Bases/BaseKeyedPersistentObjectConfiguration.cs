namespace FreeDB.Infrastructure.EntityFramework.Mappings.Bases
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Core.Bases;

    public abstract class BaseKeyedPersistentObjectConfiguration<T, TKey> : BaseEntityTypeConfiguration<T>
        where T : PersistentObject<TKey>
        where TKey : struct
    {
        protected BaseKeyedPersistentObjectConfiguration(string tableName = null, string keyName = null)
            : base(tableName)
        {
            if (keyName == null)
            {
                keyName = typeof(T).Name + PersistentObject.ID;
            }

            HasKey(p => p.Id).Property(p => p.Id).HasColumnName(keyName).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
        }
    }
}
