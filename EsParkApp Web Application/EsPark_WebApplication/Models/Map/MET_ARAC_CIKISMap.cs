using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Models.Map
{
    public class MET_ARAC_CIKISMap : IEntityTypeConfiguration<MET_ARAC_CIKIS>
    {


        public void Configure(EntityTypeBuilder<MET_ARAC_CIKIS> builder)
        {
            builder.ToTable("MET_ARAC_CIKIS");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.CIKIS_ID).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.PLAKA).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.GIRIS_PLAKA_ILK).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.CIKIS_PLAKA_ILK).HasColumnType("string");
            builder.Property(p => p.GIRIS_ZAMANI).HasColumnType("date").IsRequired(false);
            builder.Property(p => p.CIKIS_ZAMANI).HasColumnType("date").IsRequired(false);
            builder.Property(p => p.ABONE_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.SURE).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.UCRET).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.KULLANICI_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.ACIKLAMA).HasColumnType("string");
            builder.Property(p => p.CIKIS_INCELENEN_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.BAKIYE).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.ODEME_SEKLI_ID).HasColumnType("byte").IsRequired(false);
            builder.Property(p => p.SABIT_UCRET).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.TOKEN).HasColumnType("string").IsRequired(true);


        }
    }
}
