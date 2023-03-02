using Microsoft.VisualBasic;
using System.Globalization;

namespace EsPark_WebApplication.Helper.DTO
{
    public class showDebtCollectionReport
    {
        public string DT_ReportId { get; set; }
        public int reportId { get; set; }
        public string plaka { get; set; }
        public string locname { get; set; }
        public string username { get; set; }
        public string debtpaymentdate { get; set; }
        public string explanation { get; set; }
        public decimal debtamount { get; set; }
    }
}
