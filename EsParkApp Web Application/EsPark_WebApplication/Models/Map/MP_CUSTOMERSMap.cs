using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_CUSTOMERSMap : IEntityTypeConfiguration<MP_CUSTOMERS>   
    {
        public void Configure(EntityTypeBuilder<MP_CUSTOMERS> builder)
        {
            builder.ToTable("MP_CUSTOMERS");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.Adisoyadi).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.Tel1).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Tel2).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Adres).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Mail).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Tckimlik).HasColumnType("decimal").IsRequired(true);
            builder.Property(p => p.Meslek).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Ogretim).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.İsyeri).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Gorevi).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Dtarihi).HasColumnType("datetime").IsRequired(false);
            builder.Property(p => p.Medeni).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.Tipi).HasColumnType("sort").IsRequired(false);
            builder.Property(p => p.Vergidairesi).HasColumnType("string").IsRequired(false);

	    }
    }
}
