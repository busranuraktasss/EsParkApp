using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Helper.DTO.UpdateRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class LocationsController : Controller
    {
        private readonly EntitiesContext _ctx;


        public LocationsController(EntitiesContext ctx)
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
                var recordsTotal = _ctx.MP_LOCATIONs.Where(w => w.ISDELETED == 0 && w.ISACTIVE == 1).Count();

                //TOTAL HESAPLARKEN ısdeleted larıda sayıyor.

                var LocationsListCount = _ctx.MP_LOCATIONs.Where(w => w.LOCNAME.Contains(searchValue) && w.ISDELETED == 0 && w.ISACTIVE == 1)
                    .Select(s => new showLocationRequest()
                    {
                        DT_LocId = "id_" + s.LOCID.ToString().Trim(),
                        LocId = s.LOCID,
                        Locname = s.LOCNAME,
                        Locaddress = s.LOCADDRESS,
                        Capacity = s.CAPACITY,
                        Phone = s.PHONE,
                        Muhkod = s.MUHKOD,   
                        

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "LocId":
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.LocId descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.LocId ascending select o).ToList();
                            break;
                        case "Locname":
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.Locname descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.Locname ascending select o).ToList();
                            break;
                        case "Locaddress":
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.Locaddress descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.Locaddress ascending select o).ToList();
                            break;
                        case "Capacity":
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.Capacity descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.Capacity ascending select o).ToList();
                            break;
                        case "Phone":
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.Phone descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.Phone ascending select o).ToList();
                            break;
                        case "Muhkod":
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.Muhkod descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.Muhkod ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                LocationsListCount = (from o in LocationsListCount orderby o.LocId descending select o).ToList();
                            else
                                LocationsListCount = (from o in LocationsListCount orderby o.LocId ascending select o).ToList();
                            break;
                    }
                }
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = LocationsListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddLocation(addLocationRequest request)
        {
            try
            {
                _ctx.MP_LOCATIONs.Add(new MP_LOCATIONS()
                {
                    LOCNAME = request.locname,
                    LOCADDRESS = request.locaddress,
                    CAPACITY = request.capacity,
                    PHONE = request.phone,
                    MUHKOD = request.muhkod,
                    ISACTIVE =request.isactive,
                    LOCCODE = request.locname,
                    RECDATE = DateTime.Now,
                    LOCTYPE = request.loctype,
                    
                });
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, Messages = "Ekleme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteLocation(int pr)
        {
            try
            {
                var current = await _ctx.MP_LOCATIONs.Where(w => w.LOCID == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.ISDELETED = 1;//Soft Delete
                _ctx.MP_LOCATIONs.Update(current);
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = $"{current.LOCNAME} isimli Kullanıcı Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> getLocation(int sId)
        {
            var currentData = await _ctx.MP_LOCATIONs.Where(f => f.LOCID == sId).Select(s=> new { s.LOCID ,s.LOCCODE , s.LOCNAME , s.LOCADDRESS , s.PHONE , s.MUHKOD , s.CAPACITY ,s.ISACTIVE , s.LOCTYPE ,s.CENTERLOCID ,s.RECDATE }).FirstOrDefaultAsync();
            return Json(new { Status = true, data = currentData, Messages = "Success", Code=200 } );

         }

        [HttpPost]
        public async Task<JsonResult> UpdateLocation(updateLocationRequest pr)
        {
            try
            {
                var current = await _ctx.MP_LOCATIONs.Where(w => w.LOCID == pr.updateId).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.LOCNAME = pr.locname;
                current.LOCADDRESS = pr.locaddress;
                current.CAPACITY = pr.capacity;
                current.PHONE = pr.phone;
                current.MUHKOD = pr.muhkod;
                current.ISACTIVE = pr.isactive;
                current.RECDATE = DateTime.Now;
                current.CENTERLOCID = 1;
                current.LOCTYPE = 1;
                _ctx.MP_LOCATIONs.Update(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }
    }
}
