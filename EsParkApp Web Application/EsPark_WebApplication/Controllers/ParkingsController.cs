using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]
    public class ParkingsController : Controller
    {
        private readonly EntitiesContext _ctx;


        public ParkingsController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowParkings()
        {
            try
            {
                Request.Form.TryGetValue("draw", out Microsoft.Extensions.Primitives.StringValues drawOut);

                var draw = drawOut.FirstOrDefault();

                Request.Form.TryGetValue("start", out Microsoft.Extensions.Primitives.StringValues startOut);
                var start = startOut.FirstOrDefault();

                Request.Form.TryGetValue("length", out Microsoft.Extensions.Primitives.StringValues lengthOut);
                var length = lengthOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][column]", out Microsoft.Extensions.Primitives.StringValues orderColumnOut);
                Request.Form.TryGetValue("columns[" + orderColumnOut.FirstOrDefault() + "][name]", out Microsoft.Extensions.Primitives.StringValues columnsNameOut);
                var sortColumn = columnsNameOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][dir]", out Microsoft.Extensions.Primitives.StringValues sortColumnDirOut);
                var sortColumnDir = sortColumnDirOut.FirstOrDefault();

                Request.Form.TryGetValue("search[value]", out Microsoft.Extensions.Primitives.StringValues searchValueOut);
                var searchValue = searchValueOut.FirstOrDefault() ?? "";

                //Paging Size 
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var recordsTotal = _ctx.MP_PARKINGs.Where(t => t.JOB_OUT == null).Count();

                var ParkingListCount = _ctx.MP_PARKINGs.Where(w => w.LICENSEPLATE.Contains(searchValue) )
                    .Select(s => new showParkingsRequest()
                    {
                        DT_ParkingsId = "id_" + s.PARKINGID.ToString().Trim(),
                        parkingId = s.PARKINGID,
                        licenseplate = s.LICENSEPLATE,
                        startdate = s.STARTDATE.ToString("dd-MM-yyyy"),
                        parkingduration = s.PARKINGDURATION,
                        parkingfee = s.PARKINGFEE,
                        parkingdurationforovertime = s.PARKINGDURATIONFOROVERTIME,
                        parkingfeeforovertime = s.PARKINGFEEFOROVERTIME,
                        locId = (from y in _ctx.MP_LOCATIONs
                                    join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                    where z.ID == s.JOBROTATIONHISTORYID
                                    select y.LOCNAME).FirstOrDefault(),

                        deviceId = (from y in _ctx.MP_DEVICEs
                                   join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                   where z.ID == s.JOBROTATIONHISTORYID
                                   select y.SERIALNO).FirstOrDefault(),

                        rowId = (from y in _ctx.MP_USERs
                                     join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                     where z.ID == s.JOBROTATIONHISTORYID
                                     select y.USERNAME).FirstOrDefault(),
                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "parkingId":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.parkingId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.parkingId ascending select o).ToList();
                            break;
                        case "licenseplate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                        case "startdate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate ascending select o).ToList();
                            break;
                        case "rowId":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.rowId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.rowId ascending select o).ToList();
                            break;
                        case "locId":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.locId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.locId ascending select o).ToList();
                            break;

                        default:
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.parkingId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.parkingId ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = ParkingListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ParkOut(int sId)
        {
            try
            {
                var current = await _ctx.MP_PARKINGs.Where(w => w.TARIFFID == sId).FirstOrDefaultAsync();
                if (current == null) return Json(new { status = false, data = "", Messages = "Ürün bulunamadı." });


               

                return Json(new { Status = true, Messages = "Online araç çıkış işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }
        

    }
}


