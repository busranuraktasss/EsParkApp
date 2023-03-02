using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_SUB_INVOICESMap : IEntityTypeConfiguration<MP_SUB_INVOICES>
    {

        public void Configure(EntityTypeBuilder<MP_SUB_INVOICES> builder)
        {
            builder.ToTable("MP_SUB_INVOICES");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.RecordId).HasColumnType("long").IsRequired();
            builder.Property(p => p.InvoiceId).HasColumnType("long").IsRequired();
           

    }
    }
}
