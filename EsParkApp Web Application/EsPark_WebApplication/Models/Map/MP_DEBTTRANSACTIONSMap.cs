using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_DEBTTRANSACTIONSMap : IEntityTypeConfiguration<MP_DEBTTRANSACTIONS>
    {
        public void Configure(EntityTypeBuilder<MP_DEBTTRANSACTIONS> builder)
        {
            builder.ToTable("MP_DEBTTRANSACTIONS");
            builder.HasKey(e => e.DEBTTRANSACTIONID);


            builder.Property(p => p.RECEIVEDDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.CREATEDDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.JOBROTATIONHISTORYID).HasColumnType("int").IsRequired();
            builder.Property(p => p.DATA).HasColumnType("string").IsRequired();

    }
    }
}
