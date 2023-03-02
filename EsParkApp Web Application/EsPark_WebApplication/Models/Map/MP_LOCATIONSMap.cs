using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_LOCATIONSMap : IEntityTypeConfiguration<MP_LOCATIONS>
    {
        public void Configure(EntityTypeBuilder<MP_LOCATIONS> builder)
        {
            builder.ToTable("MP_LOCATIONS");
            builder.HasKey(e => e.LOCID);


            builder.Property(p => p.LOCNAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.LOCCODE).HasColumnType("string").IsRequired();
            builder.Property(p => p.LOCADDRESS).HasColumnType("string").IsRequired();
            builder.Property(p => p.CAPACITY).HasColumnType("int").IsRequired();
            builder.Property(p => p.LOCTYPE).HasColumnType("int").IsRequired();
            builder.Property(p => p.CENTERLOCID).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISACTIVE).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("int").IsRequired();
            builder.Property(p => p.RECBY).HasColumnType("int").IsRequired();
            builder.Property(p => p.RECDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.PHONE).HasColumnType("string").IsRequired();
            builder.Property(p => p.MUHKOD).HasColumnType("string").IsRequired();
           
    }
    }
}
