using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class LocTariffAssignmentsController : Controller
    {
        private readonly EntitiesContext _ctx;

        public LocTariffAssignmentsController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowLocations()
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
                var recordsTotal = _ctx.MP_LOCTARIFFASSIGNMENTs.Where(t => t.ISDELETED == 0).Count();

                var LocationListCount = _ctx.MP_LOCTARIFFASSIGNMENTs.Where(w => w.ASSIGNID.ToString().Contains(searchValue) && w.ISDELETED == 0)
                    .Select(s => new showLocTariffAssignRequest()
                    {
                        DT_LocTariffAssignId = "id_" + s.ASSIGNID.ToString().Trim(),
                        LocTariffAssignId = s.ASSIGNID,
                        LocId = _ctx.MP_LOCATIONs.Where(t => t.LOCID == s.LOCID).Select(s => s.LOCNAME).FirstOrDefault(),
                        plateId = s.LOCID,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {

                    if (sortColumnDir == "desc")
                        LocationListCount = (from o in LocationListCount orderby o.LocId descending select o).ToList();
                    else
                        LocationListCount = (from o in LocationListCount orderby o.LocId ascending select o).ToList();


                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = LocationListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, data = "", messages = ex.Message });
            }
        }
        
        [HttpPost]
        public JsonResult ShowParkTariff(int sId)
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
                var recordsTotal = _ctx.MP_LOCTARIFFASSIGNMENTs.Where(t =>  t.ISDELETED == 0  && t.LOCID == sId).Count();

                var TariffListCount = _ctx.MP_LOCTARIFFASSIGNMENTs.Where(w => w.ASSIGNID.ToString().Contains(searchValue) && w.ISDELETED == 0  && w.LOCID == sId)
                    .Select(s => new showLocTariffAssignRequest()
                    {
                        DT_LocTariffAssignId = "id_" + s.ASSIGNID.ToString().Trim(),
                        LocTariffAssignId = s.ASSIGNID,
                        TariffId = _ctx.MP_PARKTARIFFs.Where(t => t.TARIFFID == s.TARIFFID ).Select(s => s.TARIFFNAME).FirstOrDefault(),

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {

                    if (sortColumnDir == "desc")
                        TariffListCount = (from o in TariffListCount orderby o.LocId descending select o).ToList();
                    else
                        TariffListCount = (from o in TariffListCount orderby o.LocId ascending select o).ToList();


                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = TariffListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteLocation(int pr)
        {
            try
            {
                var current = await _ctx.MP_LOCTARIFFASSIGNMENTs.Where(w => w.ASSIGNID == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                _ctx.MP_LOCTARIFFASSIGNMENTs.Remove(current);//Hard Delete
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "", Messages = " Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpGet]
        public async Task<JsonResult> GetParkTariff(int sId)
        {
            var currentData = await _ctx.MP_LOCATIONs.Where(f => f.LOCID == sId).Select(s => new { s.LOCID, s.LOCNAME }).FirstOrDefaultAsync();

            return Json(new { Status = true, data = currentData, Messages = "Success", Code = 200 });

        }

        [HttpGet]
        public JsonResult Dropdown1()
        {
            var ListCount = _ctx.MP_LOCATIONs.Where(t => t.LOCNAME != null)
                .Select(s => new showLocTariffAssignRequest()
                {
                    LocTariffAssignId = s.LOCID,
                    LocId = _ctx.MP_LOCATIONs.Where(t => t.LOCID == s.LOCID).Select(s => s.LOCNAME).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }
       
        [HttpGet]
        public JsonResult Dropdown2()
        {
            var ListCount = _ctx.MP_PARKTARIFFs.Where(t => t.TARIFFNAME != null && t.ISDELETED == 0)
                .Select(s => new showLocTariffAssignRequest()
                {
                    LocTariffAssignId = s.TARIFFID,
                    TariffId = _ctx.MP_PARKTARIFFs.Where(t => t.TARIFFID == s.TARIFFID).Select(s => s.TARIFFNAME).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }

        [HttpPost]
        public async Task<JsonResult> AddParkTariff(addOfficialPlatesRequest request)
        {

            try
            {  if (request.plateId == -1) return Json(new { Status = false, data = ""});
                var existingLTA = await _ctx.MP_LOCTARIFFASSIGNMENTs.Where(t => t.LOCID == request.locId && t.TARIFFID == request.plateId).FirstOrDefaultAsync();

                if (existingLTA != null)
                    return Json(new { Status = false, data = "", Messages = "Ekleme Başarısız" });

                var insertedLTA = new MP_LOCTARIFFASSIGNMENTS()
                {
                    LOCID = request.locId,
                    TARIFFID = request.plateId,
                    RECDATE = DateTime.Now,
                };

                _ctx.MP_LOCTARIFFASSIGNMENTs.Add(insertedLTA);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = insertedLTA, Messages = "Success", Code = 200 });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }

        }

        [HttpGet]
        public async Task<JsonResult> DeleteTariff(int pr)
        {
            try
            {
                var current = await _ctx.MP_LOCTARIFFASSIGNMENTs.Where(w => w.ASSIGNID == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.ISDELETED = 1;//Soft Delete
                _ctx.MP_LOCTARIFFASSIGNMENTs.Update(current);
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = " Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = true, data = "", messages = ex.Message });

            }
        }

    }
}
