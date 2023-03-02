using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Models.Map
{
    public class AracDetayliRaporEsParkMap : IEntityTypeConfiguration<AracDetayliRaporEsPark>
    {
        public void Configure(EntityTypeBuilder<AracDetayliRaporEsPark> builder)
        {
            builder.ToTable("AracDetayliRaporEsPark");
            builder.HasKey(e => e.Plaka);


            builder.Property(p => p.Baslangic).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.GiristeOdenen).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.GirisOdemeTipi).HasColumnType("decimal").IsRequired(true);
            builder.Property(p => p.Bitis).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.BitisteOdenen).HasColumnType("date").IsRequired(false);
            builder.Property(p => p.BitisOdemeTipi).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Borcmu).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Otopark).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Kullanici).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Yon).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Tip).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Resim).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Lokasyon).HasColumnType("string").IsRequired(true);

        }
    }
}
