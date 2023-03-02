using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_TRANSACTIONSMap : IEntityTypeConfiguration<MP_TRANSACTIONS>
    {
        public void Configure(EntityTypeBuilder<MP_TRANSACTIONS> builder)
        {
            builder.ToTable("MP_TRANSACTIONS");
            builder.HasKey(e => e.TRANSACTIONID);


            builder.Property(p => p.RECEIVEDDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.CREATEDDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.JOBROTATIONHISTORYID).HasColumnType("int").IsRequired();
            builder.Property(p => p.TRANSACTIONSTATUS).HasColumnType("int").IsRequired();
            builder.Property(p => p.TRANSACTIONTYPE).HasColumnType("int").IsRequired();
            builder.Property(p => p.DATA).HasColumnType("string").IsRequired();
    }
}
}
