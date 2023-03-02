using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_PARKTARIFFSMap : IEntityTypeConfiguration<MP_PARKTARIFFS>
    {
        public void Configure(EntityTypeBuilder<MP_PARKTARIFFS> builder)
        {
            builder.ToTable("MP_PARKTARIFFS");
            builder.HasKey(e => e.TARIFFID);


            builder.Property(p => p.TARIFFNAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.TOLERANCE).HasColumnType("int").IsRequired();
            builder.Property(p => p.FIXEDENTRYFEE).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.FIXEDENTRYDURATION).HasColumnType("int").IsRequired();
            builder.Property(p => p.VALIDDAYS).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.MONDAY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.TUESDAY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.WEDNESDAY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.THURSDAY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.FRIDAY).HasColumnType("tinyint").IsRequired(); 
			builder.Property(p => p.SATURDAY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.SUNDAY).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.H00000030).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.H00300100).HasColumnType("decimal").IsRequired();
            builder.Property(p => p.H01000130).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H01300200).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H02000230).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H02300300).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H03000330).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H03300400).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H04000430).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H04300500).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H05000530).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H05300600).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H06000630).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H06300700).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H07000730).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H07300800).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H08000830).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H08300900).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H09000930).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H09301000).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H10001030).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H10301100).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H11001130).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H11301200).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H12001230).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H12301300).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H13001330).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H13301400).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H14001430).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H14301500).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H15001530).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H15301600).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H16001630).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H16301700).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H17001730).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H17301800).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H18001830).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H18301900).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H19001930).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H19302000).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H20002030).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H20302100).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H21002130).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H21302200).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H22002230).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H22302300).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H23002330).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.H23302400).HasColumnType("decimal").IsRequired();
			builder.Property(p => p.ISACTIVE).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("tinyint").IsRequired();
            builder.Property(p => p.RECBY).HasColumnType("int").IsRequired();
            builder.Property(p => p.RECDATE).HasColumnType("date").IsRequired();

	}
    }
}
