using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_CAMPAIGNSMap : IEntityTypeConfiguration<MP_CAMPAIGNS>
    {
        public void Configure(EntityTypeBuilder<MP_CAMPAIGNS> builder)
        {
            builder.ToTable("MP_CAMPAIGNS");
            builder.HasKey(e => e.CAMPAIGNID);


            builder.Property(p => p.CAMPAIGNNAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.CAMPAIGNFEE).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.CAMPAIGNTYPE).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.CAMPAIGNFREEDURATION).HasColumnType("int").IsRequired();
            builder.Property(p => p.CAMPAIGNDISCOUNTAMOUNT).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.ISACTIVE).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.RECBY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.RECDATE).HasColumnType("date").IsRequired();

        }

    }
}
