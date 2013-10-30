namespace FreeDB.Infrastructure.EntityFramework.Mappings
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Bases;
    using Core.Model;

    public class DiscConfiguration : BaseEntityTypeConfiguration<Disc>
    {
        public DiscConfiguration()
        {
            //override sql identity with freedb id
            HasKey(p => p.Id)
                .Property(p => p.Id)
                .HasColumnName("DiscId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasOptional(d => d.Artist).WithMany().Map(mc => mc.MapKey("ArtistId"));
            HasOptional(d => d.Genre).WithMany().Map(mc => mc.MapKey("GenreId"));
            HasMany(d => d.Tracks).WithRequired().Map(mc => mc.MapKey("DiscId"));
        }
    }
}
