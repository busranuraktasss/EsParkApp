using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_ISLETME_REPORTMap : IEntityTypeConfiguration<MP_ISLETME_REPORT>
    {

        public void Configure(EntityTypeBuilder<MP_ISLETME_REPORT> builder)
        {
            builder.ToTable("MP_ISLETME_REPORT");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.Toplanmasi_gereken).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Toplanan).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Toplanan_nakit).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Toplanan_kkarti).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Borc).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Borc_nakit).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Borc_kkarti).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Genel_nakit).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Genel_kkart).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Toplanan_genel).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Firma).HasColumnType("string");
            builder.Property(p => p.Tarih).HasColumnType("date");
            builder.Property(p => p.Locname).HasColumnType("string");
            builder.Property(p => p.Username).HasColumnType("string");

        }
    }
}
