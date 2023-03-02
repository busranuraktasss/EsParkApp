using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_NOTIFICATIONSMap : IEntityTypeConfiguration<MP_NOTIFICATIONS>
    {
        public void Configure(EntityTypeBuilder<MP_NOTIFICATIONS> builder)
        {
            builder.ToTable("MP_NOTIFICATIONS");
            builder.HasKey(e => e.ID);


            builder.Property(p => p.NF_DATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.NF_MESSAGE).HasColumnType("string").IsRequired();
            builder.Property(p => p.JOBID).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISREADED).HasColumnType("bit").IsRequired();
            builder.Property(p => p.ISALLSEND).HasColumnType("bit").IsRequired();
            builder.Property(p => p.TITLE).HasColumnType("string").IsRequired();
            builder.Property(p => p.SENDUSERID).HasColumnType("int").IsRequired();

        }
    }
}
