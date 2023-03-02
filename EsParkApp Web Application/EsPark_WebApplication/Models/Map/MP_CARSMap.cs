using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_CARSMap : IEntityTypeConfiguration<MP_CARS>
    {
        public void Configure(EntityTypeBuilder<MP_CARS> builder)
        {
            builder.ToTable("MP_CARS");
            builder.HasKey(e => e.ID);


            builder.Property(p => p.CUSTOMERBAG).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.PLATE).HasColumnType("string").IsRequired();
            builder.Property(p => p.MARKA).HasColumnType("string").IsRequired();
            builder.Property(p => p.MODEL).HasColumnType("string").IsRequired();
            builder.Property(p => p.TIP).HasColumnType("string").IsRequired();
            builder.Property(p => p.YIL).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.ACIKLAMA).HasColumnType("byte").IsRequired();
            builder.Property(p => p.BITISTARIHI).HasColumnType("datetime").IsRequired();
            builder.Property(p => p.UCRET).HasColumnType("decimal").IsRequired();

            
        }
    }
}
