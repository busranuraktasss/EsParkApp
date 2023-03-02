using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using EsPark_WebApplication.Helper.DTO.ShowRequest;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class BusinessReportController : Controller
    {
        private readonly EntitiesContext _ctx;

        public BusinessReportController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult ShowLocationReport(DateTime sDate)
        {
            try
            {
                var bas_tar = sDate;
                var bit_tar = Convert.ToDateTime(sDate.ToShortDateString() + " 23:59:59");

                Request.Form.TryGetValue("draw", out Microsoft.Extensions.Primitives.StringValues drawOut);

                var draw = drawOut.FirstOrDefault();

                Request.Form.TryGetValue("start", out Microsoft.Extensions.Primitives.StringValues startOut);
                var start = startOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][column]", out Microsoft.Extensions.Primitives.StringValues orderColumnOut);
                Request.Form.TryGetValue("columns[" + orderColumnOut.FirstOrDefault() + "][name]", out Microsoft.Extensions.Primitives.StringValues columnsNameOut);
                var sortColumn = columnsNameOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][dir]", out Microsoft.Extensions.Primitives.StringValues sortColumnDirOut);
                var sortColumnDir = sortColumnDirOut.FirstOrDefault();

                Request.Form.TryGetValue("search[value]", out Microsoft.Extensions.Primitives.StringValues searchValueOut);
                var searchValue = searchValueOut.FirstOrDefault() ?? "";

                //Paging Size 
                var recordsTotal = _ctx.MP_ISLETME_REPORTs.Where(t => t.Tarih >= bas_tar && t.Tarih <= bit_tar).Count();

                var LocReportList = _ctx.MP_ISLETME_REPORTs.Where(t => t.Tarih >= bas_tar && t.Tarih <= bit_tar)
                    .Select(s => new showLocReportRequest()
                    {
                        DT_ShiftId = "id_" + s.Id.ToString().Trim(),
                        _Id = s.Id,
                       _locname = s.Locname,
                       _username = s.Username,
                       _toplanmasi_gereken = s.Toplanmasi_gereken,
                       _toplanan = s.Toplanan,
                       _toplanan_nakit = s.Toplanan_nakit,
                       _toplanan_kkarti = s.Toplanan_kkarti,
                       _borc = s.Borc,
                       _borc_nakit = s.Borc_nakit,
                       _borc_kkarti = s.Borc_kkarti,
                       _toplanan_genel = s.Toplanan_genel,
                       _genel_nakit = s.Genel_nakit,
                       _genel_kkart = s.Genel_kkart
                    }).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "_locname":
                            if (sortColumnDir == "desc")
                                LocReportList = (from o in LocReportList orderby o._locname descending select o).ToList();
                            else
                                LocReportList = (from o in LocReportList orderby o._locname ascending select o).ToList();
                            break;
                        case "Vardiya":
                            if (sortColumnDir == "desc")
                                LocReportList = (from o in LocReportList orderby o._username descending select o).ToList();
                            else
                                LocReportList = (from o in LocReportList orderby o._username ascending select o).ToList();
                            break;
                        case "Lokasyon":
                            if (sortColumnDir == "desc")
                                LocReportList = (from o in LocReportList orderby o._toplanan_genel descending select o).ToList();
                            else
                                LocReportList = (from o in LocReportList orderby o._toplanan_genel ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                LocReportList = (from o in LocReportList orderby o._locname descending select o).ToList();
                            else
                                LocReportList = (from o in LocReportList orderby o._locname ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = LocReportList
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        public JsonResult getIsletmeRaporu(DateTime trh)
        {


            var bas_tar = trh;
            var bit_tar = Convert.ToDateTime(trh.ToShortDateString() + " 23:59:59");
            var query = new isletme();

            query.user_count = _ctx.Isletme_100.Where(w => w.BEGINDATE == trh).Count();
            query.lok_count = _ctx.Isletme_100.Where(w => w.BEGINDATE == trh).GroupBy(g => g.LOCNAME).Count();
            query.arac_count = (from z in _ctx.MP_PARKINGs join j in _ctx.Isletme_100 on z.JOBROTATIONHISTORYID equals j.ID where j.BEGINDATE == trh && z.PARKINGDURATIONFOROVERTIME != null select new { z.PARKINGID }).Count();
            query.odeyenler_count = (from z in _ctx.MP_PARKINGs
                                     join j in _ctx.Isletme_100 on z.JOBROTATIONHISTORYID equals j.ID
                                     where j.BEGINDATE == trh && z.PARKINGDURATIONFOROVERTIME != null && z.PARKINGFEEFOROVERTIME == z.PAIDFEEFOROVERTIME
                                     select new
                                     {
                                         z.PARKINGID
                                     }).Count();

            query.odemeyenler_count = (from z in _ctx.MP_PARKINGs
                                       join j in _ctx.Isletme_100 on z.JOBROTATIONHISTORYID equals j.ID
                                       where j.BEGINDATE == trh && z.PARKINGDURATIONFOROVERTIME != null && z.PARKINGFEEFOROVERTIME > z.PAIDFEEFOROVERTIME && z.PAIDFEEFOROVERTIME > 0
                                       select new
                                       {
                                           z.PARKINGID
                                       }).Count();

            query.borclu_count = (from z in _ctx.MP_PARKINGs
                                  join j in _ctx.Isletme_100 on z.JOBROTATIONHISTORYID equals j.ID
                                  where j.BEGINDATE == trh && z.PARKINGDURATIONFOROVERTIME != null && z.INDEBTED != null
                                  select new
                                  {
                                      z.PARKINGID
                                  }).Count();


            query.toplanmasi_gereken = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Toplanmasi_gereken);

            query.toplanan = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Toplanan);

            query.toplanan_nakit = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Toplanan_nakit);

            query.toplanan_kkarti = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Toplanan_kkarti);

            query.borc = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Borc);

            query.borc_nakit = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Borc_nakit);

            query.borc_kkarti = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Borc_kkarti);

            query.toplanan_genel = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Toplanan_genel);
            query.genel_nakit = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Genel_nakit);
            query.genel_kkart = _ctx.MP_ISLETME_REPORTs.Where(w => w.Tarih >= bas_tar && w.Tarih <= bit_tar).Sum(s => s.Genel_kkart);

            query.firma = "Kentas Ltd. Sti.";
            query.tarih = bas_tar.ToShortDateString();
            query.tarih2 = bas_tar.ToString("D");
            query.Id = 1;

            return Json(new { Status = true, Data = query });

        }
    }
}
