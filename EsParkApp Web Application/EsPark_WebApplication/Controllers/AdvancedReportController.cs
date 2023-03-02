using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace EsPark_WebApplication.Controllers
{
    public class AdvancedReportController : Controller
    {
        private readonly EntitiesContext _ctx;

        public AdvancedReportController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelectFilter(selectFilterAdvance sf)
        {
            try
            {
                int[] loc_current = _ctx.MP_JOBROTATIONHISTORYs.Where(w => w.PARKID == sf.loc).Select(s => s.ID).ToArray();
                int[] row_currnet = _ctx.MP_JOBROTATIONHISTORYs.Where(w => w.USERID == sf.row).Select(s => s.ID).ToArray();
                int[] plate_current = _ctx.MP_PARKINGs.Where(w => w.LICENSEPLATE == sf.plate).Select(s => s.JOBROTATIONHISTORYID).ToArray();
                int[] device_current = _ctx.MP_JOBROTATIONHISTORYs.Where(w => w.DEVICEID == sf.device).Select(s => s.ID).ToArray();
                int[] fee_plate = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                   join y in _ctx.MP_PARKINGs on x.ID equals y.JOBROTATIONHISTORYID
                                   join z in _ctx.MP_OFFICIALPLATEs on y.LICENSEPLATE equals z.LICENSEPLATE
                                   where z.PID == sf.fee_plate
                                   select x.ID).ToArray();
                int[] check = _ctx.MP_PARKINGs.Where(w => w.INDEBTED == 1).Select(s => s.JOBROTATIONHISTORYID).ToArray();

                return Json(new { status = false, data = "" });
            }
            catch (Exception ex) { return Json(new { status = false, data = "", messages = ex.Message }); }
        }

        [HttpPost]
        public JsonResult GetAdvanceReport(advanceReport pr)
        {
            pr.date2 = pr.date2.AddDays(1);

            Request.Form.TryGetValue("draw", out Microsoft.Extensions.Primitives.StringValues drawOut);

            var draw = drawOut.FirstOrDefault();
            Request.Form.TryGetValue("order[0][column]", out Microsoft.Extensions.Primitives.StringValues orderColumnOut);
            Request.Form.TryGetValue("columns[" + orderColumnOut.FirstOrDefault() + "][name]", out Microsoft.Extensions.Primitives.StringValues columnsNameOut);
            var sortColumn = columnsNameOut.FirstOrDefault();

            Request.Form.TryGetValue("order[0][dir]", out Microsoft.Extensions.Primitives.StringValues sortColumnDirOut);
            var sortColumnDir = sortColumnDirOut.FirstOrDefault();

            Request.Form.TryGetValue("search[value]", out Microsoft.Extensions.Primitives.StringValues searchValueOut);
            var searchValue = searchValueOut.FirstOrDefault() ?? "";

            var recordsTotal = 0;

            if (pr.locId != -1)
            {
                if (pr.rowId != -1)
                {
                    if (pr.deviceId != -1)
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }

                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }
                        }

                    }
                    else
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs 
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs 
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs 
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs 
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs 
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }

                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs 
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where z.ROWID == pr.rowId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =   s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (pr.deviceId != -1)
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                     
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                          
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                     
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                          
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                         {
                                                             DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                             reportId = s.PARKINGID,
                                                             locname = (from y in _ctx.MP_LOCATIONs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.LOCNAME).FirstOrDefault(),
                                                             username = (from y in _ctx.MP_USERs
                                                                         join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                         where z.ID == s.JOBROTATIONHISTORYID
                                                                         select y.USERNAME).FirstOrDefault(),
                                                             device = (from y in _ctx.MP_DEVICEs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.SERIALNO).FirstOrDefault(),
                                                             plaka = s.LICENSEPLATE,
                                                             startdate =  s.STARTDATE.ToString("G"),
                                                             parking_duration = s.PARKINGDURATION,
                                                             parking_fee = s.PARKINGFEE,
                                                             parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                             parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                             paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                             outdate =  s.STARTDATE.ToString("G"),
                                                             indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                         }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                          
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where t.LOCID == pr.locId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                     
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         
                                                        join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                     
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                          
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where t.LOCID == pr.locId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                     
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                          
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                     
                                                    join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                          
                                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where t.LOCID == pr.locId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (pr.rowId != -1)
                {
                    if (pr.deviceId != -1)
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1 select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }

                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }
                        }

                    }
                    else
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                        join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where z.ROWID == pr.rowId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where z.ROWID == pr.rowId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (pr.deviceId != -1)
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1
                                    });
                                }
                            }
                            else
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2
                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from x in _ctx.MP_DEVICEs
                                                        join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where x.DEVICEID == pr.deviceId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where x.DEVICEID == pr.deviceId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where x.DEVICEID == pr.deviceId && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from x in _ctx.MP_DEVICEs
                                                    join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where x.DEVICEID == pr.deviceId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from x in _ctx.MP_DEVICEs
                                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where x.DEVICEID == pr.deviceId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }
                        }

                    }
                    else
                    {
                        if (pr.plate != null)
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where   v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where   v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where   v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where   v.LICENSEPLATE == pr.plate && u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where   v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where   v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where   v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where   v.LICENSEPLATE == pr.plate && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }

                        }
                        else
                        {
                            if (pr.freePlateId != -1)
                            {
                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs

                                                     
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where   u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount = (from y in _ctx.MP_JOBROTATIONHISTORYs

                                                         
                                                        join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                        join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                        where   u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                        select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount = (from o in AdvanceCount orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                    where   u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount1 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         join u in _ctx.MP_OFFICIALPLATEs on v.LICENSEPLATE equals u.LICENSEPLATE
                                                         where   u.PID == pr.freePlateId && v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount1 = (from o in AdvanceCount1 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount1

                                    });
                                }


                            }
                            else
                            {

                                if (pr.check == 1)
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where   v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount2 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where   v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2 && v.INDEBTED == 1
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount2 = (from o in AdvanceCount2 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount2

                                    });
                                }
                                else
                                {
                                    recordsTotal = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                    join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                    where   v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                    select v.PARKINGID
                                                    ).Count();

                                    var AdvanceCount3 = (from y in _ctx.MP_JOBROTATIONHISTORYs
                                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                                         where   v.STARTDATE > pr.date1 && v.STARTDATE < pr.date2
                                                         select v)
                                                        .Select(s => new showAdvanceReport()
                                                        {
                                                            DT_ReportId = "id_" + s.PARKINGID.ToString().Trim(),
                                                            reportId = s.PARKINGID,
                                                            locname = (from y in _ctx.MP_LOCATIONs
                                                                       join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                                       where z.ID == s.JOBROTATIONHISTORYID
                                                                       select y.LOCNAME).FirstOrDefault(),
                                                            username = (from y in _ctx.MP_USERs
                                                                        join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                                        where z.ID == s.JOBROTATIONHISTORYID
                                                                        select y.USERNAME).FirstOrDefault(),
                                                            device = (from y in _ctx.MP_DEVICEs
                                                                      join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                                                      where z.ID == s.JOBROTATIONHISTORYID
                                                                      select y.SERIALNO).FirstOrDefault(),
                                                            plaka = s.LICENSEPLATE,
                                                            startdate =  s.STARTDATE.ToString("G"),
                                                            parking_duration = s.PARKINGDURATION,
                                                            parking_fee = s.PARKINGFEE,
                                                            parking_duration_forovertime = s.PARKINGDURATIONFOROVERTIME,
                                                            parking_fee_forovertime = s.PARKINGFEEFOROVERTIME,
                                                            paid_fee_forovertime = s.PAIDFEEFOROVERTIME,
                                                            outdate =  s.STARTDATE.ToString("G"),
                                                            indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                                                        }).ToList();


                                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                                    {
                                        switch (sortColumn)
                                        {
                                            case "reportId":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                            case "plaka":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.plaka ascending select o).ToList();
                                                break;
                                            case "locname":
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.locname ascending select o).ToList();
                                                break;
                                            default:
                                                if (sortColumnDir == "desc")
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId descending select o).ToList();
                                                else
                                                    AdvanceCount3 = (from o in AdvanceCount3 orderby o.reportId ascending select o).ToList();
                                                break;
                                        }
                                    }

                                    return Json(new
                                    {
                                        draw = draw,
                                        recordsFiltered = recordsTotal,
                                        recordsTotal = recordsTotal,
                                        data = AdvanceCount3

                                    });
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
