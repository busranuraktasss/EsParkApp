namespace EsPark_WebApplication.Models
{
    public class AracDetayliRaporEsPark
    {

        public string Plaka { get; set; }
        public DateTime Baslangic { get; set; }
        public decimal GiristeOdenen { get; set; }
        public string GirisOdemeTipi { get; set; }
        public DateTime? Bitis { get; set; }
        public decimal? BitisteOdenen { get; set; }
        public string BitisOdemeTipi { get; set; }
        public string Borcmu { get; set; }
        public string Otopark { get; set; }
        public string Kullanici { get; set; }
        public string Yon { get; set; }
        public string Tip { get; set; }
        public string Resim { get; set; }
        public string Lokasyon { get; set; }
    }
}
