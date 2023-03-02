using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_INVOICESMap : IEntityTypeConfiguration<MP_INVOICES>
    {
        public void Configure(EntityTypeBuilder<MP_INVOICES> builder)
        {
            builder.ToTable("MP_INVOICES");
            builder.HasKey(e => e.Id);


            builder.Property(p => p.InvoiceNumber).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.TotalAmount).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.TotalVat).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.InvoiceTime).HasColumnType("date").IsRequired();
            builder.Property(p => p.JobId).HasColumnType("int").IsRequired();
            builder.Property(p => p.LicensePlate).HasColumnType("string").IsRequired();
            builder.Property(p => p.InvoiceInfo).HasColumnType("string").IsRequired();
            builder.Property(p => p.Ettn).HasColumnType("guid").IsRequired();
            builder.Property(p => p.AmountText).HasColumnType("string").IsRequired();
            builder.Property(p => p.RecordId).HasColumnType("string").IsRequired();
            builder.Property(p => p.Cancel).HasColumnType("bit").IsRequired(true);
            builder.Property(p => p.CancelTime).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.Ccart).HasColumnType("bit").IsRequired(true);

        }
    }
}
