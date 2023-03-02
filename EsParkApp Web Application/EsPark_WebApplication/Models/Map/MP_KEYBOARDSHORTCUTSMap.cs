using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_KEYBOARDSHORTCUTSMap : IEntityTypeConfiguration<MP_KEYBOARDSHORTCUTS>
    {
        public void Configure(EntityTypeBuilder<MP_KEYBOARDSHORTCUTS> builder)
        {
            builder.ToTable("MP_KEYBOARDSHORTCUTS");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.Tus1).HasColumnType("string").IsRequired();
            builder.Property(p => p.Tus2).HasColumnType("string").IsRequired();
            builder.Property(p => p.Tus3).HasColumnType("string").IsRequired();
            builder.Property(p => p.Tus4).HasColumnType("string").IsRequired();
            builder.Property(p => p.Tus5).HasColumnType("string").IsRequired();
            builder.Property(p => p.Tus6).HasColumnType("string").IsRequired();
        }
    }
}
