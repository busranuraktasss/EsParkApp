using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_PARKINGDEBTSMap : IEntityTypeConfiguration<MP_PARKINGDEBTS>
    {
        public void Configure(EntityTypeBuilder<MP_PARKINGDEBTS> builder)
        {
            builder.ToTable("MP_PARKINGDEBTS");
            builder.HasKey(e => e.PARKINGDEBTID);


            builder.Property(p => p.PARKINGID).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.PARKINGJRID).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.LICENSEPLATE).HasColumnType("string").IsRequired(true);
            builder.Property(p => p.DEBTAMOUNT).HasColumnType("decimal").IsRequired(true);
            builder.Property(p => p.DEBTPAYMENTID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.EXPLANATION).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.FINISHDATE).HasColumnType("date").IsRequired(false);
            builder.Property(p => p.FEE).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.LAWYER).HasColumnType("bool").IsRequired(false);


        }
    }
}
