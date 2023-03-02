using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_PARKINGSMap : IEntityTypeConfiguration<MP_PARKINGS>
    {
        public void Configure(EntityTypeBuilder<MP_PARKINGS> builder)
        {
            builder.ToTable("MP_PARKINGS");
            builder.HasKey(e => e.PARKINGID);


            builder.Property(p => p.SESSIONID).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.JOBROTATIONHISTORYID).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.PARKINGSTATUS).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.TARIFFID).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.LICENSEPLATE).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.STARTDATE).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.ENDDATE).HasColumnType("date").IsRequired(true);
            builder.Property(p => p.PERON).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.PARKINGDURATION).HasColumnType("int").IsRequired(true);
            builder.Property(p => p.PARKINGFEE).HasColumnType("decimal").IsRequired(true);
            builder.Property(p => p.PAIDFEE).HasColumnType("decimal").IsRequired(true);
            builder.Property(p => p.PARKINGDURATIONFOROVERTIME).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.PARKINGFEEFOROVERTIME).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.PAIDFEEFOROVERTIME).HasColumnType("decimal").IsRequired(false); 
            builder.Property(p => p.ENDDATEFOROVERTIME).HasColumnType("datetime").IsRequired(false);
            builder.Property(p => p.OUTDATE).HasColumnType("datetime").IsRequired(false);
            builder.Property(p => p.INDEBTED).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.GBORC).HasColumnType("decimal").IsRequired(false);
            builder.Property(p => p.JOB_OUT).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.CCart).HasColumnType("bit").HasDefaultValue<bool>(false);
            builder.Property(p => p.LocaliD).HasColumnType("int").IsRequired(false); 
            builder.Property(p => p.ImageName).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.OLD_ID).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.CCartEntry).HasColumnType("bit").HasDefaultValue<bool>(false);
            builder.Property(p => p.LocLink).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.IN_ETTN).HasColumnType("guid").IsRequired(false);
            builder.Property(p => p.OUT_ETTN).HasColumnType("guid").IsRequired(false);
            builder.Property(p => p.IN_F_NO).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.OUT_F_NO).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.VerifyEnrollment_Out).HasColumnType("string").IsRequired(false);
            builder.Property(p => p.VerifyEnrollment_In).HasColumnType("string").IsRequired(false);

          

    }
    }
}
