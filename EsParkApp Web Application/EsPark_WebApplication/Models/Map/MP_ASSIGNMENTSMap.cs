using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_ASSIHNMENTSMap : IEntityTypeConfiguration<MP_ASSIGNMENTS>
    {
        public void Configure(EntityTypeBuilder<MP_ASSIGNMENTS> builder)
        {
            builder.ToTable("MP_ASSIGNMENTS");
            builder.HasKey(e => e.ASSIGNMENTID);


            builder.Property(p => p.LOCID).HasColumnType("int").IsRequired();
            builder.Property(p => p.TERMID).HasColumnType("int").IsRequired();
            builder.Property(p => p.USERID).HasColumnType("int").IsRequired();
            builder.Property(p => p.RECDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.RECBY).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISACTIVE).HasColumnType("int").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("int").IsRequired();
           

        }

    }
}
