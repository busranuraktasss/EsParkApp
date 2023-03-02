using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]
    public class ZReportController : Controller
    {
        private readonly EntitiesContext _ctx;


        public ZReportController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowOpenShifts()
        {
            try
            {
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
                var recordsTotal = _ctx.MP_ZREPORTs.Where(t => t.REPORTTAKENDATE == null).Count();

                var OpenShiftListCount = _ctx.MP_ZREPORTs.Where(w => w.ID.ToString().Contains(searchValue) && w.REPORTTAKENDATE == null)
                    .Select(s => new showOpenShiftsRequest()
                    {
                        DT_ShiftId = "id_" + s.ID.ToString().Trim(),
                        ShiftId = s.ID,
                        Vardiya = s.CREATEDDATE.ToString("dd-MM-yyyy"),
                        
                        Lokasyon = _ctx.MP_LOCATIONs.Where(w => w.LOCID == _ctx.MP_JOBROTATIONHISTORYs.Where(t => t.ID == s.JOBROTATIONHISTORYID).Select(u => u.PARKID).FirstOrDefault()).Select(s => s.LOCNAME).FirstOrDefault(),
                        
                     
                        Kullanici = (from y in _ctx.MP_USERs
                                     join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                     where z.ID == s.JOBROTATIONHISTORYID
                                     select y.USERNAME).FirstOrDefault(),
                    }).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "ShiftId":
                            if (sortColumnDir == "desc")
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.ShiftId descending select o).ToList();
                            else
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.ShiftId ascending select o).ToList();
                            break;
                        case "Vardiya":
                            if (sortColumnDir == "desc")
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.Vardiya descending select o).ToList();
                            else
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.Vardiya ascending select o).ToList();
                            break;
                        case "Lokasyon":
                            if (sortColumnDir == "desc")
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.Lokasyon descending select o).ToList();
                            else
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.Lokasyon ascending select o).ToList();
                            break;
                        case "Kullanici":
                            if (sortColumnDir == "desc")
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.Kullanici descending select o).ToList();
                            else
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.Kullanici ascending select o).ToList();
                            break;

                        default:
                            if (sortColumnDir == "desc")
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.ShiftId descending select o).ToList();
                            else
                                OpenShiftListCount = (from o in OpenShiftListCount orderby o.ShiftId ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = OpenShiftListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> CheckShift(int[] pr)
        {
            try
            {

                var current = await _ctx.MP_ZREPORTs.Where(w => pr.Contains(w.ID)).ToListAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "" });


                foreach (var debt in current)
                {
                debt.REPORTTAKENDATE = DateTime.Now;//Soft Delete
                }

                _ctx.MP_ZREPORTs.UpdateRange(current);

                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "", Messages = "" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

    }
}
