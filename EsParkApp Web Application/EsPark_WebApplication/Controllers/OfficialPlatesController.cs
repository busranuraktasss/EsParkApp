using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class OfficialPlatesController : Controller
    {
        private readonly EntitiesContext _ctx;

        public OfficialPlatesController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowOfficialPlates()
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
                var recordsTotal = _ctx.MP_OFFICIALPLATEs.Count();

                var oPlateListCount = _ctx.MP_OFFICIALPLATEs.Where(w => w.LICENSEPLATE.Contains(searchValue))
                    .Select(s => new showOfficialPlatesRequest()
                    {
                        DT_OffId = "id_" + s.PID.ToString().Trim(),
                        pId = s.PID,
                        licenseplate = s.LICENSEPLATE,
                        freetime = s.FREETIME,
                        grup = s.GRUP,
                        finishdate = s.FINISHDATE.ToString("dd-MM-yyyy"),
                        fee = s.FEE,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "pId":
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.pId descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.pId ascending select o).ToList();
                            break;
                        case "licenseplate":
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.licenseplate descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                        case "freetime":
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.freetime descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.freetime ascending select o).ToList();
                            break;
                        case "grup":
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.grup descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.grup ascending select o).ToList();
                            break;
                        case "finishdate":
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.finishdate descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.finishdate ascending select o).ToList();
                            break;
                        case "fee":
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.fee descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.fee ascending select o).ToList();
                            break;

                        default:
                            if (sortColumnDir == "desc")
                                oPlateListCount = (from o in oPlateListCount orderby o.pId descending select o).ToList();
                            else
                                oPlateListCount = (from o in oPlateListCount orderby o.pId ascending select o).ToList();
                            break;
                    }
                }
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = oPlateListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, data = "", messages = ex.Message });
            }
        }

    }
}
