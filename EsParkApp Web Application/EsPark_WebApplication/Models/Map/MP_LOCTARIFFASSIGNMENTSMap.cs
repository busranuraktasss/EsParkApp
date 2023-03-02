using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_LOCTARIFFASSIGNMENTSMap : IEntityTypeConfiguration<MP_LOCTARIFFASSIGNMENTS>
    {
        public void Configure(EntityTypeBuilder<MP_LOCTARIFFASSIGNMENTS> builder)
        {
            builder.ToTable("MP_LOCTARIFFASSIGNMENTS");
            builder.HasKey(e => e.ASSIGNID);


            builder.Property(p => p.LOCID).HasColumnType("int").IsRequired();
            builder.Property(p => p.TARIFFID).HasColumnType("int").IsRequired();
            builder.Property(p => p.CAMPAIGNID).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISACTIVE).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("int").IsRequired();
            builder.Property(p => p.RECBY).HasColumnType("int").IsRequired();
            builder.Property(p => p.RECDATE).HasColumnType("date").IsRequired();
           
    }
    }
}
