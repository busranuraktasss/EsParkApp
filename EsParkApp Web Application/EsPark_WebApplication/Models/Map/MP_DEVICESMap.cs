using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_DEVICESMap : IEntityTypeConfiguration<MP_DEVICES>
    {
        public void Configure(EntityTypeBuilder<MP_DEVICES> builder)
        {
            builder.ToTable("MP_DEVICES");
            builder.HasKey(e => e.DEVICEID);


            builder.Property(p => p.SERIALNO).HasColumnType("string").IsRequired();
            builder.Property(p => p.SIMID).HasColumnType("int").IsRequired();
            builder.Property(p => p.ICCID).HasColumnType("string").IsRequired();
            builder.Property(p => p.ISACTIVE).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("int").IsRequired();
            builder.Property(p => p.TERMTYPE).HasColumnType("int").IsRequired();
            builder.Property(p => p.BATTERY_STATUS).HasColumnType("int").IsRequired();
            builder.Property(p => p.DEVICETYPE).HasColumnType("string").IsRequired();
           
    }
    }
}
