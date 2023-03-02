using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_ZREPORTSMap : IEntityTypeConfiguration<MP_ZREPORTS>
    {
        public void Configure(EntityTypeBuilder<MP_ZREPORTS> builder)
        {
            builder.ToTable("MP_ZREPORTS");
            builder.HasKey(e => e.ID);


            builder.Property(p => p.JOBROTATIONHISTORYID).HasColumnType("int").IsRequired();
            builder.Property(p => p.CREATEDDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.REPORTTAKENDATE).HasColumnType("date").IsRequired(false);
            builder.Property(p => p.ZREPORTNUMBER).HasColumnType("int").IsRequired();
            builder.Property(p => p.CUMULATIVESUM).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.CUMULATIVESUMVAT).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.TOTALMONEYMUSTCOLLECTED).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.TOTALMONEYCOLLECTED).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.TOTALMONEYCOLLECTEDVAT).HasColumnType("decimal").IsRequired(false);

        }
    }
}
