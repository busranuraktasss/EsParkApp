using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_ZREPORTCOUNTERMap : IEntityTypeConfiguration<MP_ZREPORTCOUNTER>
    {
        public void Configure(EntityTypeBuilder<MP_ZREPORTCOUNTER> builder)
        {
            builder.ToTable("MP_ZREPORTCOUNTER");
            builder.HasKey(e => e.ID);


            builder.Property(p => p.DEVICEID).HasColumnType("int").IsRequired();
            builder.Property(p => p.LASTCOUNT).HasColumnType("int").IsRequired();
            builder.Property(p => p.LASTUPDATETIME).HasColumnType("date").IsRequired();
        }
    }
}
