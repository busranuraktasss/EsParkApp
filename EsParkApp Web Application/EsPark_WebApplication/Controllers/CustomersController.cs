using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Helper.DTO.getRequest;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Helper.DTO.UpdateRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EsPark_WebApplication.Contrsollers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly EntitiesContext _ctx;

        public CustomersController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowCustomers(int cusId)
        {
            if (cusId == 0)
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
            else
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
                var recordsTotal = _ctx.MP_CUSTOMERs.Where(w => w.Id == cusId).Count();

                var CustomersListCount = _ctx.MP_CUSTOMERs.Where(w => w.Adisoyadi.Contains(searchValue) && w.Id == cusId)
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
        }

        [HttpPost]
        public JsonResult ShowOPlates(int sId , string _plate)
        {
            try
            {
                if(sId == 0)
                {
                    sId = _ctx.MP_OFFICIALPLATEs.Where(w => w.LICENSEPLATE == _plate).Select(s => s.CUSTOMERBAG).FirstOrDefault();

                }

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
        public async Task<JsonResult> ShowOPLocations(int sId)
        {
            try
            {
                var OPLocationListCount = await _ctx.MP_OFFICIALPLATESLOCATIONs.Where(t => t.PlatesId == sId)
                    .Select(s => new showLocTariffAssignRequest()
                    {
                        LocTariffAssignId = s.LocId,
                        LocId = _ctx.MP_LOCATIONs.Where(t => t.LOCID == s.LocId).Select(t => t.LOCNAME).FirstOrDefault(),
                        opl = s.Id,
                        plateId = s.PlatesId

                    }).ToListAsync();

                return Json(new { Status = true, data = OPLocationListCount });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomer(int sId)
        {
            var currentData = await _ctx.MP_CUSTOMERs.Where(f => f.Id == sId).Select(s => new { s.Id, s.Adisoyadi, s.Adres, s.Meslek, s.İsyeri, s.Tckimlik, s.Dtarihi, s.Gorevi, s.Tel1, s.Tel2, s.Mail, s.Ogretim, s.Medeni, s.Tipi, s.Vergidairesi }).FirstOrDefaultAsync();
            return Json(new { Status = true, data = currentData, Messages = "Success", Code = 200 });
        }

        [HttpGet]
        public async Task<JsonResult> GetOPlate(int sId)
        {
            var currentData = await _ctx.MP_OFFICIALPLATEs.Where(f => f.PID == sId).Select(s => new { s.PID, s.LICENSEPLATE, s.FEE, s.FREETIME, s.CUSTOMERBAG, s.GRUP }).FirstOrDefaultAsync();

            return Json(new { Status = true, data = currentData, Messages = "Success", Code = 200 });
        }
        
        [HttpGet]
        public async Task<JsonResult> GetOPlateCus(int sId)
        {
            var currentData = await _ctx.MP_CUSTOMERs.Where(f => f.Id == sId).Select(s => new { s.Id }).FirstOrDefaultAsync();

            return Json(new { Status = true, data = currentData, Messages = "Success", Code = 200 });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCustomer(updateCustomerRequest pr)
        {
            try
            {
                var current = await _ctx.MP_CUSTOMERs.Where(w => w.Id == pr.updateId1).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.Adisoyadi = pr.adsoyad;
                current.Tel1 = pr.tel1;
                current.Tel2 = pr.tel2;
                current.Vergidairesi = pr.vergidairesi;
                current.Mail = pr.mail;
                current.Tckimlik = pr.tckimlik;
                current.Adres = pr.adres;
                current.Meslek = pr.meslek;
                current.Ogretim = pr.ogretim;
                current.İsyeri = pr.isyeri;
                current.Gorevi = pr.gorevi;
                current.Medeni = pr.medenihali;
                current.Tipi = pr.tip;
                current.Dtarihi = pr.tarih;

                _ctx.MP_CUSTOMERs.Update(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateOPlate(updateOfficialPlatesRequest pr)
        {
            try
            {
                var current = await _ctx.MP_OFFICIALPLATEs.Where(w => w.PID == pr.updateId2).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.LICENSEPLATE = pr.licenseplate;
                current.FEE = pr.fee;
                current.FINISHDATE = DateTime.Now;
                current.FREETIME = pr.freetime;
                current.CUSTOMERBAG = pr.cusId;
                current.GRUP = pr.grup;


                _ctx.MP_OFFICIALPLATEs.Update(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddCustomer(addCustomerRequest request)
        {
            try
            {
                _ctx.MP_CUSTOMERs.Add(new MP_CUSTOMERS()
                {
                    Adisoyadi = request.adsoyad,
                    Tel1 = request.tel1,
                    Tel2 = request.tel2,
                    Vergidairesi = request.vergidairesi,
                    Mail = request.mail,
                    Tckimlik = request.tckimlik,
                    Adres = request.adres,
                    Meslek = request.meslek,
                    Ogretim = request.ogretim,
                    İsyeri = request.isyeri,
                    Gorevi = request.gorevi,
                    Medeni = request.medenihali,
                    Tipi = request.tip,
                    Dtarihi = request.tarih,
                });
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, Messages = "Ekleme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddOPlate(addOfficialPlatesRequest request)
        {
            try
            {
                var existingOP = await _ctx.MP_OFFICIALPLATEs.Where(t => t.CUSTOMERBAG == request.cusId && t.LICENSEPLATE == request.licenseplate).FirstOrDefaultAsync();

                if (existingOP != null)
                    return Json(new { Status = false, data = "", Messages = "Ekleme Başarısız" });

                var insertedPlate = new MP_OFFICIALPLATES()
                {
                    LICENSEPLATE = request.licenseplate,
                    FEE = request.fee,
                    FINISHDATE = DateTime.Now,
                    FREETIME = request.freetime,
                    CUSTOMERBAG = request.cusId,
                    GRUP = request.grup

                };

                _ctx.MP_OFFICIALPLATEs.Add(insertedPlate);
                await _ctx.SaveChangesAsync();


                return Json(new { Status = true, Messages = "Ekleme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddOPLocation(addOfficialPlatesLocationRequest request)
        {
            if (request.locId != null)
            {
                for (var i = 0; i < request.locId.Length; i++)
                {
                    var insertedOPL = new MP_OFFICIALPLATESLOCATIONS()
                    {
                        LocId = request.locId[i],
                        PlatesId = request.plateId,
                    };
                    _ctx.MP_OFFICIALPLATESLOCATIONs.Add(insertedOPL);
                    await _ctx.SaveChangesAsync();

                    if (request.locId.Length - 1 == i)
                    {
                        return Json(new { Status = true, data = insertedOPL, Messages = "Success", Code = 200 });
                    }
                }
            }
            return Json(new { Status = false, data = "" });
        }

        [HttpGet]
        public async Task<JsonResult> DeleteCustomer(int pr)
        {
            try
            {
                var currnetOPlate = await _ctx.MP_OFFICIALPLATEs.Where(w => w.CUSTOMERBAG == pr).FirstOrDefaultAsync();

                while (currnetOPlate != null)
                {
                    var currnetLoc = await _ctx.MP_OFFICIALPLATEs.Where(w => w.CUSTOMERBAG == pr).Select(s => s.PID).FirstOrDefaultAsync();

                    if (currnetLoc != null)
                    {
                        var currnetOPLocation = _ctx.MP_OFFICIALPLATESLOCATIONs.Where(w => w.PlatesId == currnetLoc).FirstOrDefault();
                        while (currnetOPLocation != null)
                        {
                            _ctx.MP_OFFICIALPLATESLOCATIONs.Remove(currnetOPLocation);//Hard Delete
                            await _ctx.SaveChangesAsync();
                            currnetLoc = await _ctx.MP_OFFICIALPLATEs.Where(w => w.CUSTOMERBAG == pr).Select(s => s.PID).FirstOrDefaultAsync();
                            currnetOPLocation = _ctx.MP_OFFICIALPLATESLOCATIONs.Where(w => w.PlatesId == currnetLoc).FirstOrDefault();
                        }
                    }

                    if (currnetOPlate != null)
                    {
                        _ctx.MP_OFFICIALPLATEs.Remove(currnetOPlate);//Hard Delete
                        await _ctx.SaveChangesAsync();
                        currnetOPlate = await _ctx.MP_OFFICIALPLATEs.Where(w => w.CUSTOMERBAG == pr).FirstOrDefaultAsync();
                    }
                }

                var current = await _ctx.MP_CUSTOMERs.Where(w => w.Id == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                _ctx.MP_CUSTOMERs.Remove(current);//Hard Delete

                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = $"{current.Adisoyadi} isimli Kullanıcı Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteOPlate(int pr)
        {
            try
            {
                var currnetOPLoc = await _ctx.MP_OFFICIALPLATESLOCATIONs.Where(w => w.PlatesId == pr).FirstOrDefaultAsync();

                while (currnetOPLoc != null)
                {
                    _ctx.MP_OFFICIALPLATESLOCATIONs.Remove(currnetOPLoc);//Hard Delete
                    await _ctx.SaveChangesAsync();
                    currnetOPLoc = await _ctx.MP_OFFICIALPLATESLOCATIONs.Where(w => w.PlatesId == pr).FirstOrDefaultAsync();

                }

                var currentOPlate = await _ctx.MP_OFFICIALPLATEs.Where(w => w.PID == pr).FirstOrDefaultAsync();
                if (currentOPlate == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                _ctx.MP_OFFICIALPLATEs.Remove(currentOPlate);//Hard Delete
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "", Messages = $"{currentOPlate.LICENSEPLATE} isimli Kullanıcı Silme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteOPLocation(int pr)
        {
            try
            {
                var current = await _ctx.MP_OFFICIALPLATESLOCATIONs.Where(w => w.Id == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                _ctx.MP_OFFICIALPLATESLOCATIONs.Remove(current);//Hard Delete
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "", Messages = " Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpGet]
        public JsonResult Dropdown2()
        {
            var ListCount = _ctx.MP_CUSTOMERs.Where(t => t.Adisoyadi != null)
                .Select(s => new showCustomerResponse()
                {
                    cusId = s.Id,
                    adsoyad = _ctx.MP_CUSTOMERs.Where(t => t.Id == s.Id).Select(s => s.Adisoyadi).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }

        [HttpGet]
        public JsonResult Dropdown3()
        {
            var ListCount = _ctx.MP_OFFICIALPLATEs.Where(t => t.LICENSEPLATE != null)
                .Select(s => new showCustomerResponse()
                {
                    cusId = s.PID,
                    adsoyad = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == s.PID).Select(s => s.LICENSEPLATE).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }

        [HttpGet]
        public async Task<JsonResult> GetSelect()
        {
            var ListCount = _ctx.MP_LOCATIONs.Where(t => t.LOCNAME != null)
                 .Select(s => new getSelectRequest()
                 {
                     LocTariffAssignId = s.LOCID,
                     LocId = _ctx.MP_LOCATIONs.Where(t => t.LOCID == s.LOCID).Select(s => s.LOCNAME).FirstOrDefault(),

                 }).ToList();

            return Json(new { data = ListCount });


        }
    }
}
