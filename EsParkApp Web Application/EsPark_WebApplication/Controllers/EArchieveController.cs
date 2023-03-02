using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]
    public class EArchieveController : Controller
    {
        private readonly EntitiesContext _ctx;

        public EArchieveController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowEArchieve(controlDate request)
        {
            try
            {
                request.dateEnd = request.dateEnd.AddDays(1);

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
                var recordsTotal = _ctx.MP_INVOICEs.Where(w => w.InvoiceTime >= request.dateStart && request.dateEnd >= w.InvoiceTime).Count();

                var InvoiceListCount = _ctx.MP_INVOICEs.Where(w => w.LicensePlate.Contains(searchValue) && w.InvoiceTime >= request.dateStart && request.dateEnd >= w.InvoiceTime)
                    .Select(s => new showInvoicesRequest()
                    {
                        DT_InvoiceId = "id_" + s.Id.ToString().Trim(),
                        invoiceId = s.Id,
                        invoiceNumber = s.InvoiceNumber,
                        invoiceTime = s.InvoiceTime.ToString("G"),
                        licensePlate = s.LicensePlate,
                        invoiceInfo = s.InvoiceInfo,
                        totalAmount = s.TotalAmount,
                        totalVat = s.TotalVat,
                        totalSum = Convert.ToDecimal(s.TotalAmount) + Convert.ToDecimal(s.TotalVat),
                        checkCcart = s.Ccart,
                        checkCancel = s.Cancel,
                        cancelTime = (s.CancelTime == null) ? s.CancelTime : s.CancelTime,
                        ettn = s.Ettn,
                        locId = (from y in _ctx.MP_LOCATIONs
                                 join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                 join x in _ctx.MP_INVOICEs on z.ID equals x.JobId
                                 where x.Id == s.Id
                                 select y.LOCNAME).FirstOrDefault(),
                        rowId = (from y in _ctx.MP_USERs
                                 join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                 join x in _ctx.MP_INVOICEs on z.ID equals x.JobId
                                 where x.Id == s.Id
                                 select y.REALNAME).FirstOrDefault(),
                        deviceId = (from y in _ctx.MP_DEVICEs
                                    join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                    join x in _ctx.MP_INVOICEs on z.ID equals x.JobId
                                    where x.Id == s.Id
                                    select y.SERIALNO).FirstOrDefault()

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "licensePlate":
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate ascending select o).ToList();
                            break;
                        case "invoiceNumber":
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.invoiceNumber descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.invoiceNumber ascending select o).ToList();
                            break;
                        case "invoiceTime":
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.invoiceTime descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.invoiceTime ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = InvoiceListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ShowEArchieve2(controlDate request)
        {
            try
            {
                request.dateEnd = request.dateEnd.AddDays(1);

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

                var InvoiceListCount = _ctx.MP_INVOICEs.Where(w => w.LicensePlate.Contains(searchValue) && w.InvoiceTime >= request.dateStart && request.dateEnd >= w.InvoiceTime)
                    .Select(s => new showInvoicesRequest()
                    {


                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "licensePlate":
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate ascending select o).ToList();
                            break;
                        case "invoiceNumber":
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.invoiceNumber descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.invoiceNumber ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate descending select o).ToList();
                            else
                                InvoiceListCount = (from o in InvoiceListCount orderby o.licensePlate ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    data = InvoiceListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }
    }
}
