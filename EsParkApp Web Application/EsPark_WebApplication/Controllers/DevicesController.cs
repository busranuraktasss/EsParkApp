using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class DevicesController : Controller
    {

        private readonly EntitiesContext _ctx;


        public DevicesController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowDevices()
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
                var recordsTotal = _ctx.MP_DEVICEs.Where(w => w.ISDELETED == 0).Count();

                var DevicesListCount = _ctx.MP_DEVICEs.Where(w => w.SERIALNO.Contains(searchValue) && w.ISDELETED == 0)
                    .Select(s => new Cihazlar()
                    {
                        DT_RowId = "id_" + s.SERIALNO.ToString().Trim(),
                        SerialNo = s.SERIALNO,
                        BatteryStatus = s.BATTERY_STATUS,
                       
                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "SerialNo":
                            if (sortColumnDir == "desc")
                                DevicesListCount = (from o in DevicesListCount orderby o.SerialNo descending select o).ToList();
                            else
                                DevicesListCount = (from o in DevicesListCount orderby o.SerialNo ascending select o).ToList();
                            break;
                        case "BatteryStatus":
                            if (sortColumnDir == "desc")
                                DevicesListCount = (from o in DevicesListCount orderby o.BatteryStatus descending select o).ToList();
                            else
                                DevicesListCount = (from o in DevicesListCount orderby o.BatteryStatus ascending select o).ToList();
                            break;
                       
                        default:
                            if (sortColumnDir == "desc")
                                DevicesListCount = (from o in DevicesListCount orderby o.SerialNo descending select o).ToList();
                            else
                                DevicesListCount = (from o in DevicesListCount orderby o.SerialNo ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = DevicesListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, data = "", messages = ex.Message });
            }
        }
    }
}

public class Cihazlar
{
    public string DT_RowId { get; set; }
    public string SerialNo { get; set; }
    public int BatteryStatus { get; set; }

}