using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsPark_WebApplication.Models.Map
{
    public class MP_COLLECTIONTYPESMap : IEntityTypeConfiguration<MP_COLLECTIONTYPES>
    {

        public void Configure(EntityTypeBuilder<MP_COLLECTIONTYPES> builder)
        {
            builder.ToTable("MP_COLLECTIONTYPES");
            builder.HasKey(e => e.COLLECTIONTYPEID);


            builder.Property(p => p.COLLECTIONTYPENAME).HasColumnType("string").IsRequired();
            builder.Property(p => p.COLLECTIONTYPEDESCRIPTION).HasColumnType("string").IsRequired();


        }
    }

}
