using EsPark_WebApplication.Controllers;
using EsPark_WebApplication.Models.Map;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EsPark_WebApplication.Helper.DTO;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

namespace EsPark_WebApplication.Models
{
    public class EntitiesContext : DbContext
    {
        public EntitiesContext(DbContextOptions<EntitiesContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MP_ASSIHNMENTSMap());
            modelBuilder.ApplyConfiguration(new MP_CAMPAIGNSMap());
            modelBuilder.ApplyConfiguration(new MP_CARSMap());
            modelBuilder.ApplyConfiguration(new MP_COLLECTIONTYPESMap());
            modelBuilder.ApplyConfiguration(new MP_CUSTOMERSMap());
            modelBuilder.ApplyConfiguration(new MP_DEBTPAYMENTSMap());
            modelBuilder.ApplyConfiguration(new MP_DEBTTRANSACTIONSMap());
            modelBuilder.ApplyConfiguration(new MP_DEVICESMap());
            modelBuilder.ApplyConfiguration(new MP_INVOICESMap());
            modelBuilder.ApplyConfiguration(new MP_INVOICE_INFOSMap());
            modelBuilder.ApplyConfiguration(new MP_JOBROTATIONHISTORYMap());
            modelBuilder.ApplyConfiguration(new MP_KEYBOARDSHORTCUTSMap());
            modelBuilder.ApplyConfiguration(new MP_LOCATIONSMap());
            modelBuilder.ApplyConfiguration(new MP_LOCTARIFFASSIGNMENTSMap());
            modelBuilder.ApplyConfiguration(new MP_MAPLOCATIONSMap());
            modelBuilder.ApplyConfiguration(new MP_NOTIFICATIONSMap());
            modelBuilder.ApplyConfiguration(new MP_OFFICIALPLATESMap());
            modelBuilder.ApplyConfiguration(new MP_OFFICIALPLATESLOCATIONSMap());
            modelBuilder.ApplyConfiguration(new MP_PARKINGDEBTSMap());
            modelBuilder.ApplyConfiguration(new MP_PARKINGSMap());
            modelBuilder.ApplyConfiguration(new MP_PARKTARIFFSMap());
            modelBuilder.ApplyConfiguration(new MP_SUB_INVOICESMap());
            modelBuilder.ApplyConfiguration(new MP_SYSTEMLOGMap());
            modelBuilder.ApplyConfiguration(new MP_TRANSACTIONSMap());
            modelBuilder.ApplyConfiguration(new MP_USERSMap());
            modelBuilder.ApplyConfiguration(new MP_ZREPORTCOUNTERMap());
            modelBuilder.ApplyConfiguration(new MP_ZREPORTSMap());
            modelBuilder.ApplyConfiguration(new AktifDurumHesapProMap());
            modelBuilder.ApplyConfiguration(new MET_INCELENENMap());
            modelBuilder.ApplyConfiguration(new MET_ARAC_CIKISMap());
            modelBuilder.ApplyConfiguration(new MP_ISLETME_REPORTMap());
            //modelBuilder.ApplyConfiguration(new BorcSorgulaDetayMap());
            //modelBuilder.Entity<BorcSorgulaDetays>().ToView(nameof(BorcSorgulaDetays)).HasKey(s => s.ID);
            modelBuilder.Entity<BorcSorgulaDetays>(e => e.HasNoKey());
            modelBuilder.Entity<AracDetayliRaporEsPark>(e => e.HasNoKey());
            modelBuilder.Entity<Isletme_100>(e => e.HasNoKey());

            base.OnModelCreating(modelBuilder);
        }

        //....
        public DbSet<MP_ASSIGNMENTS> MP_ASSIGNMENTs { get; set; }
        public DbSet<MP_CAMPAIGNS> MP_CAMPAIGNs { get; set; }
        public DbSet<MP_CARS> MP_CARs { get; set; }
        public DbSet<MP_COLLECTIONTYPES> MP_COLLECTIONTYPEs { get; set; }
        public DbSet<MP_CUSTOMERS> MP_CUSTOMERs { get; set; }
        public DbSet<MP_DEBTPAYMENTS> MP_DEBTPAYMENTs { get; set; }
        public DbSet<MP_DEBTTRANSACTIONS> MP_DEBTTRANSACTIONs { get; set; }
        public DbSet<MP_DEVICES> MP_DEVICEs { get; set; }
        public DbSet<MP_INVOICES> MP_INVOICEs { get; set; }
        public DbSet<MP_INVOICE_INFOS> MP_INVOICE_INFOs { get; set; }
        public DbSet<MP_JOBROTATIONHISTORY> MP_JOBROTATIONHISTORYs { get; set; }
        public DbSet<MP_KEYBOARDSHORTCUTS> MP_KEYBOARDSHORTCUTs { get; set; }
        public DbSet<MP_LOCATIONS> MP_LOCATIONs { get; set; }
        public DbSet<MP_LOCTARIFFASSIGNMENTS> MP_LOCTARIFFASSIGNMENTs { get; set; }
        public DbSet<MP_MAPLOCATIONS> MP_MAPLOCATIONs { get; set; }
        public DbSet<MP_NOTIFICATIONS> MP_NOTIFICATIONs { get; set; }
        public DbSet<MP_OFFICIALPLATES> MP_OFFICIALPLATEs { get; set; }
        public DbSet<MP_OFFICIALPLATESLOCATIONS> MP_OFFICIALPLATESLOCATIONs { get; set; }
        public DbSet<MP_PARKINGDEBTS> MP_PARKINGDEBTs { get; set; }
        public DbSet<MP_PARKINGS> MP_PARKINGs { get; set; }
        public DbSet<MP_PARKTARIFFS> MP_PARKTARIFFs { get; set; }
        public DbSet<MP_SUB_INVOICES> MP_SUB_INVOICEs { get; set; }
        public DbSet<MP_SYSTEMLOG> MP_SYSTEMLOGs { get; set; }
        public DbSet<MP_TRANSACTIONS> MP_TRANSACTIONs { get; set; }
        public DbSet<MP_USERS> MP_USERs { get; set; }
        public DbSet<MP_ZREPORTCOUNTER> MP_ZREPORTCOUNTERs { get; set; }
        public DbSet<MP_ZREPORTS> MP_ZREPORTs { get; set; }
        public DbSet<MET_INCELENEN> MET_INCELENENs { get; set; }
        public DbSet<MET_ARAC_CIKIS> MET_ARAC_CIKISs { get; set; }
        public DbSet<MP_ISLETME_REPORT> MP_ISLETME_REPORTs { get; set; }
        public DbSet<AktifDurumHesapPros> AktifDurumHesapPro { get; set; }
        public DbSet<BorcSorgulaDetays> BorcSorgulaDetays { get; set; }
        public DbSet<AracDetayliRaporEsPark> AracDetayliRaporEsPark { get; set; }
        public DbSet<Isletme_100> Isletme_100 { get; set; }

       
    }
}
