using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_USERSMap : IEntityTypeConfiguration<MP_USERS>
    {
        public void Configure(EntityTypeBuilder<MP_USERS> builder)
        {
            builder.ToTable("MP_USERS");
            builder.HasKey(e => e.ROWID);


            builder.Property(p => p.USERNAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.PASSWORD).HasColumnType("string").IsRequired();
            builder.Property(p => p.REALNAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.ISDELETED).HasColumnType("int").IsRequired();
            builder.Property(p => p.STATUS).HasColumnType("int").IsRequired();
            builder.Property(p => p.AUTHORITY).HasColumnType("int").IsRequired();
            builder.Property(p => p.PHONE).HasColumnType("string").IsRequired();
        }
    }
}
