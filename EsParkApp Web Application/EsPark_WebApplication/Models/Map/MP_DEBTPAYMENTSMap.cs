using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EsPark_WebApplication.Models.Map
{
    public class MP_DEBTPAYMENTSMap : IEntityTypeConfiguration<MP_DEBTPAYMENTS>
    {
        public void Configure(EntityTypeBuilder<MP_DEBTPAYMENTS> builder)
        {
            builder.ToTable("MP_DEBTPAYMENTS");
            builder.HasKey(e => e.DEBTPAYMENTID);


            builder.Property(p => p.DEBTPAYMENTAMOUNT).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.COLLECTIONTYPE).HasColumnType("int").IsRequired();
            builder.Property(p => p.COLLECTIONJRID).HasColumnType("int").IsRequired();
            builder.Property(p => p.DEBTPAYMENTDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.EXPLANATION).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.CCART).HasColumnType("bit").IsRequired(false);
            builder.Property(p => p.ETTN).HasColumnType("guid").IsRequired(false);


        }
    }
}
