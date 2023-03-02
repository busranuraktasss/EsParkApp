using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_OFFICIALPLATESLOCATIONSMap : IEntityTypeConfiguration<MP_OFFICIALPLATESLOCATIONS>
    {
        public void Configure(EntityTypeBuilder<MP_OFFICIALPLATESLOCATIONS> builder)
        {
            builder.ToTable("MP_OFFICIALPLATESLOCATIONS");
            builder.HasKey(e => e.Id);

            builder.Property(p => p.LocId).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.PlatesId).HasColumnType("int").IsRequired(true);


        }
    }
}
