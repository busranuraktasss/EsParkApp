using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_OFFICIALPLATESMap : IEntityTypeConfiguration<MP_OFFICIALPLATES>
    {
        public void Configure(EntityTypeBuilder<MP_OFFICIALPLATES> builder)
        {
            builder.ToTable("MP_OFFICIALPLATES");
            builder.HasKey(e => e.PID);


            builder.Property(p => p.LICENSEPLATE).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.FREETIME).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.GRUP).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.CUSTOMERBAG).HasColumnType("int");
            builder.Property(p => p.FINISHDATE).HasColumnType("date");
            builder.Property(p => p.FEE).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.IsCCard).HasColumnType("bit").HasDefaultValue<bool>(false);
            builder.Property(p => p.ETTN).HasColumnType("guid").IsRequired(false);

        }

    }
}
