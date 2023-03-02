using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_JOBROTATIONHISTORYMap : IEntityTypeConfiguration<MP_JOBROTATIONHISTORY>
    {
        public void Configure(EntityTypeBuilder<MP_JOBROTATIONHISTORY> builder)
        {
            builder.ToTable("MP_JOBROTATIONHISTORY");
            builder.HasKey(e => e.ID);


            builder.Property(p => p.CREATEDDATE).HasColumnType("datetime").IsRequired();
            builder.Property(p => p.BEGINDATE).HasColumnType("datetime").IsRequired();
            builder.Property(p => p.FINISHDATE).HasColumnType("datetime").IsRequired();
            builder.Property(p => p.OLD_ID).HasColumnType("int").IsRequired();
            builder.Property(p => p.PARKID).HasColumnType("int").IsRequired();
            builder.Property(p => p.USERID).HasColumnType("int").IsRequired();
            builder.Property(p => p.DEVICEID).HasColumnType("int").IsRequired();
            builder.Property(p => p.LOCATION).HasColumnType("string").IsRequired();

        }
    }
}
