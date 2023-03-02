using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{  
    public class MP_MAPLOCATIONSMap : IEntityTypeConfiguration<MP_MAPLOCATIONS>
    {
        public void Configure(EntityTypeBuilder<MP_MAPLOCATIONS> builder)
        {
            builder.ToTable("MP_MAPLOCATIONS");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.Lat).HasColumnType("string").IsRequired();
            builder.Property(p => p.Lng).HasColumnType("string").IsRequired();
            builder.Property(p => p.JobId).HasColumnType("int").IsRequired();
            builder.Property(p => p.BatteryStatus).HasColumnType("int").IsRequired();
            builder.Property(p => p.CreatedTime).HasColumnType("date").IsRequired();
          
    }

    }
}
