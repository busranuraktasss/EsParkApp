using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Models.Map
{
    public class Isletme_100Map : IEntityTypeConfiguration<Isletme_100>
    {
        public void Configure(EntityTypeBuilder<Isletme_100> builder)
        {
            builder.ToTable("AktifDurumHesapPros");
            builder.HasKey(e => e.ID);


            //builder.Property(p => p._backingForCREATEDDATE).HasColumnType("date").IsRequired(true);
            //builder.Property(p => p._backingForLOCNAME).HasColumnType("string").IsRequired(true);
            //builder.Property(p => p._backingForSERIALNO).HasColumnType("string").IsRequired(true);
            //builder.Property(p => p._backingForREALNAME).HasColumnType("string").IsRequired(true);
            //builder.Property(p => p._backingForBEGINDATE).HasColumnType("date").IsRequired(true);
            //builder.Property(p => p._backingForGereken).HasColumnType("decimal").IsRequired(false);
            //builder.Property(p => p._backingForToplanan).HasColumnType("decimal").IsRequired(false);
            //builder.Property(p => p._backingForKacanPara).HasColumnType("decimal").IsRequired(false);
            //builder.Property(p => p._backingForID).HasColumnType("int").IsRequired(true);
            //builder.Property(p => p._backingForBorc).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.CREATEDDATE).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.LOCNAME).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.SERIALNO).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.REALNAME).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.BEGINDATE).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.Gereken).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Toplanan).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.KacanPara).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.Borc).HasColumnType("decimal").IsRequired(false);


        }
    }

}
