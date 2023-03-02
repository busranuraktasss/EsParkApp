using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]
    public class CustomerCollectionController : Controller
    {
        private readonly EntitiesContext _ctx;

        public CustomerCollectionController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowCustomers()
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
                var recordsTotal = _ctx.MP_CUSTOMERs.Count();

                var CustomersListCount = _ctx.MP_CUSTOMERs.Where(w => w.Adisoyadi.Contains(searchValue))
                    .Select(s => new showCustomerResponse()
                    {
                        DT_CusId = "id_" + s.Id.ToString().Trim(),
                        cusId = s.Id,
                        adsoyad = s.Adisoyadi,
                        tckimlik = s.Tckimlik,
                        tel1 = s.Tel1,
                        mail = s.Mail,
                        vergidairesi = s.Vergidairesi,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "cusId":
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.cusId descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.cusId ascending select o).ToList();
                            break;
                        case "adsoyad":
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.adsoyad descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.adsoyad ascending select o).ToList();
                            break;
                        case "tckimlik":
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.tckimlik descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.tckimlik ascending select o).ToList();
                            break;
                        case "tel1":
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.tel1 descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.tel1 ascending select o).ToList();
                            break;
                        case "mail":
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.mail descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.mail ascending select o).ToList();
                            break;
                        case "vergidairesi":
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.vergidairesi descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.vergidairesi ascending select o).ToList();
                            break;

                        default:
                            if (sortColumnDir == "desc")
                                CustomersListCount = (from o in CustomersListCount orderby o.cusId descending select o).ToList();
                            else
                                CustomersListCount = (from o in CustomersListCount orderby o.cusId ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = CustomersListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ShowOPlates(int sId)
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
                var recordsTotal = _ctx.MP_OFFICIALPLATEs.Where(w => w.CUSTOMERBAG == sId).Count();

                var OPlateListCount = _ctx.MP_OFFICIALPLATEs.Where(w => w.LICENSEPLATE.Contains(searchValue) && w.CUSTOMERBAG == sId)
                    .Select(s => new showOfficialPlatesRequest()
                    {
                        DT_OffId = "id_" + s.PID.ToString().Trim(),
                        pId = s.PID,
                        cusId = s.CUSTOMERBAG,
                        freetime = s.FREETIME,
                        fee = s.FEE,
                        finishdate = s.FINISHDATE.ToString("dd-MM-yyyy"),
                        licenseplate = s.LICENSEPLATE,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "pId":
                            if (sortColumnDir == "desc")
                                OPlateListCount = (from o in OPlateListCount orderby o.pId descending select o).ToList();
                            else
                                OPlateListCount = (from o in OPlateListCount orderby o.pId ascending select o).ToList();
                            break;
                        case "cusId":
                            if (sortColumnDir == "desc")
                                OPlateListCount = (from o in OPlateListCount orderby o.cusId descending select o).ToList();
                            else
                                OPlateListCount = (from o in OPlateListCount orderby o.cusId ascending select o).ToList();
                            break;
                        case "freetime":
                            if (sortColumnDir == "desc")
                                OPlateListCount = (from o in OPlateListCount orderby o.freetime descending select o).ToList();
                            else
                                OPlateListCount = (from o in OPlateListCount orderby o.freetime ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                OPlateListCount = (from o in OPlateListCount orderby o.pId descending select o).ToList();
                            else
                                OPlateListCount = (from o in OPlateListCount orderby o.pId ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = OPlateListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOPlate(int sId)
        {
            var currentData = await _ctx.MP_OFFICIALPLATEs.Where(f => f.PID == sId).Select(s => new { s.PID, s.LICENSEPLATE, s.FEE, s.FREETIME, s.CUSTOMERBAG, s.GRUP }).FirstOrDefaultAsync();

            return Json(new { Status = true, data = currentData, Messages = "Success", Code = 200 });
        }

        [HttpGet]
        public JsonResult Dropdown()
        {
            var ListCount = _ctx.MP_OFFICIALPLATEs.Where(t => t.LICENSEPLATE != null)
                .Select(s => new showCustomerResponse()
                {
                    cusId = s.PID,
                    adsoyad = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == s.PID).Select(s => s.LICENSEPLATE).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateOPlates(addOfficialPlatesRequest request)
        {
            try
            {
                var current = await _ctx.MP_OFFICIALPLATEs.Where(w => w.PID == request.plateId).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.LICENSEPLATE = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == request.plateId).Select(s => s.LICENSEPLATE).FirstOrDefault();
                current.CUSTOMERBAG = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == request.plateId).Select(s => s.CUSTOMERBAG).FirstOrDefault();
                current.FREETIME = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == request.plateId).Select(s => s.FREETIME).FirstOrDefault();
                current.GRUP = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == request.plateId).Select(s => s.GRUP).FirstOrDefault();
                current.FEE = request.fee;
                current.FINISHDATE = request.startdate.AddMonths(request.month);

                _ctx.MP_OFFICIALPLATEs.Update(current);
                await _ctx.SaveChangesAsync();


                return Json(new { Status = true, Messages = "Güncelleme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }
    }
}
