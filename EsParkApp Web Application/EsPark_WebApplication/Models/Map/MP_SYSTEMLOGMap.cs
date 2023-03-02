using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_SYSTEMLOGMap : IEntityTypeConfiguration<MP_SYSTEMLOG>
    {
        public void Configure(EntityTypeBuilder<MP_SYSTEMLOG> builder)
        {
            builder.ToTable("MP_SYSTEMLOG");
            builder.HasKey(e => e.ROWID);


            builder.Property(p => p.OPTYPE).HasColumnType("string").IsRequired();
            builder.Property(p => p.OPDATE).HasColumnType("date").IsRequired();
            builder.Property(p => p.STNAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.RECBY).HasColumnType("int").IsRequired();
            builder.Property(p => p.CARDNO).HasColumnType("string").IsRequired();
            builder.Property(p => p.EXP).HasColumnType("string").IsRequired();
           


    }

    }
}
