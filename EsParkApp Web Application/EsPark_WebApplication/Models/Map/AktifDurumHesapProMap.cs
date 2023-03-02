using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Models.Map
{
    public class AktifDurumHesapProMap : IEntityTypeConfiguration<AktifDurumHesapPros>
    {
        public void Configure(EntityTypeBuilder<AktifDurumHesapPros> builder)
        {
            builder.ToTable("AktifDurumHesapPros");
            builder.HasKey(e => e.ID);


            builder.Property(p => p.trh).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.USERNAME).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.LOCNAME).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.SERIALNO).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.CREATEDDATE).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.ARAC).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.TAHSIL).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.TAHSIL_KART).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.BORC).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.BORC_KART).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.TOPLAM).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.TOPLANMASI_GEREKEN).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.BATTERY).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.DOLULUK).HasColumnType("string").IsRequired(true);

        }

    }
}
