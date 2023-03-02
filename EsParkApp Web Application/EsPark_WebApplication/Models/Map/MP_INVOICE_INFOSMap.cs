using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_INVOICE_INFOSMap : IEntityTypeConfiguration<MP_INVOICE_INFOS>
    {
        public void Configure(EntityTypeBuilder<MP_INVOICE_INFOS> builder)
        {
            builder.ToTable("MP_INVOICE_INFOS");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.CreateUserId).HasColumnType("int").IsRequired();
            builder.Property(p => p.CreateDate).HasColumnType("date").IsRequired();
            builder.Property(p => p.InvoiceText).HasColumnType("string").IsRequired();
            builder.Property(p => p.InvoiceType).HasColumnType("int").IsRequired();
           
    }
    }
}
