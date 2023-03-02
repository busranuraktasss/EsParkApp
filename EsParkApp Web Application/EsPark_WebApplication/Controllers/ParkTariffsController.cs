using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Helper.DTO.UpdateRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class ParkTariffsController : Controller
    {
        private readonly EntitiesContext _ctx;

        public ParkTariffsController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowTariffs()
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
                var recordsTotal = _ctx.MP_PARKTARIFFs.Where(w => w.ISDELETED == 0).Count();

                var TariffsListCount = _ctx.MP_PARKTARIFFs.Where(w => w.TARIFFNAME.Contains(searchValue) && w.ISDELETED == 0)
                    .Select(s => new showTariffRequest()
                    {
                        DT_TariffId = "id_" + s.TARIFFID.ToString().Trim(),
                        tariffid = s.TARIFFID,
                        tariffname = s.TARIFFNAME,
                        tolerance = s.TOLERANCE,
                        fixedentryfee = s.FIXEDENTRYFEE,
                        fixedentryduration = s.FIXEDENTRYDURATION,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "tariffid":
                            if (sortColumnDir == "desc")
                                TariffsListCount = (from o in TariffsListCount orderby o.tariffid descending select o).ToList();
                            else
                                TariffsListCount = (from o in TariffsListCount orderby o.tariffid ascending select o).ToList();
                            break;
                        case "tariffname":
                            if (sortColumnDir == "desc")
                                TariffsListCount = (from o in TariffsListCount orderby o.tariffname descending select o).ToList();
                            else
                                TariffsListCount = (from o in TariffsListCount orderby o.tariffname ascending select o).ToList();
                            break;
                        case "tolerance":
                            if (sortColumnDir == "desc")
                                TariffsListCount = (from o in TariffsListCount orderby o.tolerance descending select o).ToList();
                            else
                                TariffsListCount = (from o in TariffsListCount orderby o.tolerance ascending select o).ToList();
                            break;
                        case "fixedentryfee":
                            if (sortColumnDir == "desc")
                                TariffsListCount = (from o in TariffsListCount orderby o.fixedentryfee descending select o).ToList();
                            else
                                TariffsListCount = (from o in TariffsListCount orderby o.fixedentryfee ascending select o).ToList();
                            break;
                        case "fixedentryduration":
                            if (sortColumnDir == "desc")
                                TariffsListCount = (from o in TariffsListCount orderby o.fixedentryduration descending select o).ToList();
                            else
                                TariffsListCount = (from o in TariffsListCount orderby o.fixedentryduration ascending select o).ToList();
                            break;
                      
                        default:
                            if (sortColumnDir == "desc")
                                TariffsListCount = (from o in TariffsListCount orderby o.tariffid descending select o).ToList();
                            else
                                TariffsListCount = (from o in TariffsListCount orderby o.tariffid ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = TariffsListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddTariff(addTariffRequest request)
        {
            try
            {
                _ctx.MP_PARKTARIFFs.Add(new MP_PARKTARIFFS()
                {
                    TARIFFNAME = request.tariffname,
                    TOLERANCE = request.tolerance,
                    FIXEDENTRYFEE = request.fixedentryfee,
                    FIXEDENTRYDURATION = request.fixedentryduration,
                    RECDATE = DateTime.Now,
                    H00000030 = request.H00000030,
                    H00300100 = request.H00300100,
                    H01000130 = request.H01000130,
                    H01300200 = request.H01300200,
                    H02000230 = request.H02000230,
                    H02300300 = request.H02300300,
                    H03000330 = request.H03000330,
                    H03300400 = request.H03300400,
                    H04000430 = request.H04000430,
                    H04300500 = request.H04300500,
                    H05000530 = request.H05000530,
                    H05300600 = request.H05300600,
                    H06000630 = request.H06000630,
                    H06300700 = request.H06300700,
                    H07000730 = request.H07000730,
                    H07300800 = request.H07300800,
                    H08000830 = request.H08000830,
                    H08300900 = request.H08300900,
                    H09000930 = request.H09000930,
                    H09301000 = request.H09301000,
                    H10001030 = request.H10001030,
                    H10301100 = request.H10301100,
                    H11001130 = request.H11001130,
                    H11301200 = request.H11301200,
                    H12001230 = request.H12001230,
                    H12301300 = request.H12301300,
                    H13001330 = request.H13001330,
                    H13301400 = request.H13301400,
                    H14001430 = request.H14001430,
                    H14301500 = request.H14301500,
                    H15001530 = request.H15001530,
                    H15301600 = request.H15301600,
                    H16001630 = request.H16001630,
                    H16301700 = request.H16301700,
                    H17001730 = request.H17001730,
                    H17301800 = request.H17301800,
                    H18001830 = request.H18001830,
                    H18301900 = request.H18301900,
                    H19001930 = request.H19001930,
                    H19302000 = request.H19302000,
                    H20002030 = request.H20002030,
                    H20302100 = request.H20302100,
                    H21002130 = request.H21002130,
                    H21302200 = request.H21302200,
                    H22002230 = request.H22002230,
                    H22302300 = request.H22302300,
                    H23002330 = request.H23002330,
                    H23302400 = request.H23302400,

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
        public async Task<JsonResult> GetTariff(int sId)
        {
            var currentData = await _ctx.MP_PARKTARIFFs.Where(f => f.TARIFFID == sId).Select(s => new { s.TARIFFID, s.TARIFFNAME, s.RECDATE ,s.TOLERANCE ,s.FIXEDENTRYDURATION , s.FIXEDENTRYFEE, s.H00000030 , s.H00300100 , s.H01000130 , s.H01300200 , s.H02000230 ,s.H02300300 ,   s.H03000330 , s.H03300400 , s.H04000430 , s.H04300500 , s.H05000530 , s.H05300600 , s.H06000630 , s.H06300700 , s.H07000730 , s.H07300800 , s.H08000830 ,  s.H08300900 , s.H09000930 , s.H09301000 , s.H10001030 , s.H10301100 , s.H11001130 , s.H11301200 , s.H12001230 , s.H12301300 , s.H13001330 , s.H13301400 , s.H14001430 , s.H14301500 , s.H15001530 , s.H15301600 , s.H16001630 , s.H16301700 , s.H17001730 , s.H17301800 , s.H18001830 , s.H18301900 , s.H19001930 , s.H19302000 , s.H20002030 , s.H20302100 , s.H21002130 , s.H21302200 , s.H22002230 , s.H22302300 , s.H23002330 , s.H23302400 }).FirstOrDefaultAsync();
            return Json(new { Status = true, data = currentData, Messages = "Success", Code = 200 });

        }

        [HttpPost]
        public async Task<JsonResult> UpdateTariff(updateTariffRequest pr)
        {
            try
            {
                var current = _ctx.MP_PARKTARIFFs.Where(w => w.TARIFFID == pr.AddOrUpdateId).FirstOrDefault();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
            
                current.TARIFFNAME = pr.tariffname;
                current.TOLERANCE = pr.tolerance ;
                current.FIXEDENTRYFEE = pr.fixedentryfee;
                current.FIXEDENTRYDURATION = pr.fixedentryduration;
                current.RECDATE = DateTime.Now;
                current.H00000030 = pr.H00000030;
                current.H00300100 = pr.H00300100;
                current.H01000130 = pr.H01000130;
                current.H01300200 = pr.H01300200;
                current.H02000230 = pr.H02000230;
                current.H02300300 = pr.H02300300;
                current.H03000330 = pr.H03000330;
                current.H03300400 = pr.H03300400;
                current.H04000430 = pr.H04000430;
                current.H04300500 = pr.H04300500;
                current.H05000530 = pr.H05000530;
                current.H05300600 = pr.H05300600;
                current.H06000630 = pr.H06000630;
                current.H06300700 = pr.H06300700;
                current.H07000730 = pr.H07000730;
                current.H07300800 = pr.H07300800;
                current.H08000830 = pr.H08000830;
                current.H08300900 = pr.H08300900;
                current.H09000930 = pr.H09000930;
                current.H09301000 = pr.H09301000;
                current.H10001030 = pr.H10001030;
                current.H10301100 = pr.H10301100;
                current.H11001130 = pr.H11001130;
                current.H11301200 = pr.H11301200;
                current.H12001230 = pr.H12001230;
                current.H12301300 = pr.H12301300;
                current.H13001330 = pr.H13001330;
                current.H13301400 = pr.H13301400;
                current.H14001430 = pr.H14001430;
                current.H14301500 = pr.H14301500;
                current.H15001530 = pr.H15001530;
                current.H15301600 = pr.H15301600;
                current.H16001630 = pr.H16001630;
                current.H16301700 = pr.H16301700;
                current.H17001730 = pr.H17001730;
                current.H17301800 = pr.H17301800;
                current.H18001830 = pr.H18001830;
                current.H18301900 = pr.H18301900;
                current.H19001930 = pr.H19001930;
                current.H19302000 = pr.H19302000;
                current.H20002030 = pr.H20002030;
                current.H20302100 = pr.H20302100;
                current.H21002130 = pr.H21002130;
                current.H21302200 = pr.H21302200;
                current.H22002230 = pr.H22002230;
                current.H22302300 = pr.H22302300;
                current.H23002330 = pr.H23002330;
                current.H23302400 = pr.H23302400;

                _ctx.MP_PARKTARIFFs.Update(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
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
                var current = await _ctx.MP_PARKTARIFFs.Where(w => w.TARIFFID == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                _ctx.MP_PARKTARIFFs.Remove(current);//Hard Delete
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = "Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

    }
}


