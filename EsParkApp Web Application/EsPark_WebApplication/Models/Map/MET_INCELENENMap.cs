using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Models.Map
{
    public class MET_INCELENENMap : IEntityTypeConfiguration<MET_INCELENEN>
    {
        public void Configure(EntityTypeBuilder<MET_INCELENEN> builder)
        {
            builder.ToTable("MET_INCELENEN");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.TOKEN).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.PLAKA).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.PLAKA_ILK).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.ZAMAN).HasColumnType("date");
            builder.Property(p => p.YON).HasColumnType("byte");
            builder.Property(p => p.SAHIP_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.ARKA_INCELENEN_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.KULLANICI_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.TIP).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.ARAC_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.PLAKALAR).HasColumnType("string");
            builder.Property(p => p.CIKIS_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.PARKLANMA).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.MET_ID).HasColumnType("int").IsRequired(true);
        }
    }
}
