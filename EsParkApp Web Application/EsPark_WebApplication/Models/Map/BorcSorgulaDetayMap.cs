using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class BorcSorgulaDetayMap : IEntityTypeConfiguration<BorcSorgulaDetays>
    {
        public void Configure(EntityTypeBuilder<BorcSorgulaDetays> builder)
        {
            builder.ToTable("BorcSorgulaDetays");
            builder.HasKey(e => e.Plaka);


            builder.Property(p => p.Borc).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Alinan).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.PARKINGDEBTID).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.STARTDATE).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.OUTDATE).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.BORC).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.LOCNAME).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.REALNAME).HasColumnType("string").IsRequired(true);

        }
    }
}
