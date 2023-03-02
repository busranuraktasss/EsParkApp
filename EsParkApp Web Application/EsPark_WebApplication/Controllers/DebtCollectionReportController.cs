using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace EsPark_WebApplication.Controllers
{
    public class DebtCollectionReportController : Controller
    {
        private readonly EntitiesContext _ctx;

        public DebtCollectionReportController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DebtCollectionReport(debtCollectionReport pr)
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
                    if (pr.plate != null)
                    {
                        recordsTotal = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                        join y in _ctx.MP_LOCATIONs on x.PARKID equals y.LOCID
                                        join z in _ctx.MP_USERs on x.USERID equals z.ROWID
                                        join t in _ctx.MP_DEBTPAYMENTs on x.ID equals t.COLLECTIONJRID
                                        join v in _ctx.MP_PARKINGDEBTs on t.DEBTPAYMENTID equals v.DEBTPAYMENTID 
                                        where t.DEBTPAYMENTDATE > pr.date1 && t.DEBTPAYMENTDATE < pr.date2 && y.LOCID == pr.locId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate
                                        select v.PARKINGID).Count();

                        var DebtCollectionCount1 = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                                    join y in _ctx.MP_LOCATIONs on x.PARKID equals y.LOCID
                                                    join z in _ctx.MP_USERs on x.USERID equals z.ROWID
                                                    join t in _ctx.MP_DEBTPAYMENTs on x.ID equals t.COLLECTIONJRID
                                                    join v in _ctx.MP_PARKINGDEBTs on t.DEBTPAYMENTID equals v.DEBTPAYMENTID
                                                    where t.DEBTPAYMENTDATE > pr.date1 && t.DEBTPAYMENTDATE < pr.date2 && y.LOCID == pr.locId && z.ROWID == pr.rowId && v.LICENSEPLATE == pr.plate
                                                    select t)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount1 = (from o in DebtCollectionCount1 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount1
                        });
                    }
                    else
                    {
                        recordsTotal = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                              join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                              where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.PARKID == pr.locId && x.USERID == pr.rowId
                                              select y.DEBTPAYMENTID).Count();

                        var DebtCollectionCount2 = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                                     join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                                     where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.PARKID == pr.locId && x.USERID == pr.rowId
                                                     select y)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount2 = (from o in DebtCollectionCount2 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount2
                        });
                    }
                }
                else
                {
                    if (pr.plate != null)
                    {
                        var aa = (from z in _ctx.MP_PARKINGs
                                  join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                  where  z.LICENSEPLATE == pr.plate
                                  select z.PARKINGID).Count();

                        recordsTotal = (from z in _ctx.MP_PARKINGs
                                              join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                              join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                              where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.PARKID == pr.locId && z.LICENSEPLATE == pr.plate
                                              select y.DEBTPAYMENTID).Count();

                        var DebtCollectionCount3 = (from z in _ctx.MP_PARKINGs
                                                     join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                                     join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                                     where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.PARKID == pr.locId && z.LICENSEPLATE == pr.plate
                                                     select y)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount3 = (from o in DebtCollectionCount3 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount3
                        });
                    }
                    else
                    {
                        recordsTotal = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                              join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                              where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.PARKID == pr.locId
                                              select y.DEBTPAYMENTID).Count();

                        var DebtCollectionCount4 = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                                     join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                                     where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.PARKID == pr.locId
                                                     select y)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount4 = (from o in DebtCollectionCount4 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount4
                        });
                    }
                }

            }
            else
            {
                if (pr.rowId != -1)
                {
                    if (pr.plate != null)
                    {
                        recordsTotal = (from z in _ctx.MP_PARKINGs
                                              join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                              join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                              where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.USERID == pr.rowId && z.LICENSEPLATE == pr.plate
                                              select y.DEBTPAYMENTID).Count();

                        var DebtCollectionCount5 = (from z in _ctx.MP_PARKINGs
                                                     join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                                     join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                                     where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.USERID == pr.rowId && z.LICENSEPLATE == pr.plate
                                                     select y)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount5 = (from o in DebtCollectionCount5 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount5
                        });
                    }
                    else
                    {
                        recordsTotal = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                              join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                              where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.USERID == pr.rowId
                                              select y.DEBTPAYMENTID).Count();

                        var DebtCollectionCount6 = (from x in _ctx.MP_JOBROTATIONHISTORYs
                                                     join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                                     where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && x.USERID == pr.rowId
                                                     select y)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount6 = (from o in DebtCollectionCount6 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount6
                        });
                    }
                }
                else
                {
                    if (pr.plate != null)
                    {
                        recordsTotal = (from z in _ctx.MP_PARKINGs
                                              join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                              join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                              where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && z.LICENSEPLATE == pr.plate
                                              select y).Count();

                        var DebtCollectionCount7 = (from z in _ctx.MP_PARKINGs
                                                     join x in _ctx.MP_JOBROTATIONHISTORYs on z.JOBROTATIONHISTORYID equals x.ID
                                                     join y in _ctx.MP_DEBTPAYMENTs on x.ID equals y.COLLECTIONJRID
                                                     where y.DEBTPAYMENTDATE > pr.date1 && y.DEBTPAYMENTDATE < pr.date2 && z.LICENSEPLATE == pr.plate
                                                     select y)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount7 = (from o in DebtCollectionCount7 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount7
                        });
                    }
                    else
                    {
                        recordsTotal = _ctx.MP_DEBTPAYMENTs.Where(w => w.DEBTPAYMENTDATE > pr.date1 && w.DEBTPAYMENTDATE < pr.date2).Select(s => s.DEBTPAYMENTID).Count();

                        var DebtCollectionCount8 = _ctx.MP_DEBTPAYMENTs.Where(w => w.DEBTPAYMENTDATE > pr.date1 && w.DEBTPAYMENTDATE < pr.date2)
                                               .Select(s => new showDebtCollectionReport()
                                               {
                                                   DT_ReportId = "id_" + s.DEBTPAYMENTID.ToString().Trim(),
                                                   reportId = s.DEBTPAYMENTID,
                                                   plaka = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                                                   locname = (from y in _ctx.MP_LOCATIONs
                                                              join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                                              where z.ID == s.COLLECTIONJRID
                                                              select y.LOCNAME).FirstOrDefault(),
                                                   username = (from y in _ctx.MP_USERs
                                                               join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                                               where z.ID == s.COLLECTIONJRID
                                                               select y.USERNAME).FirstOrDefault(),
                                                   debtpaymentdate = s.DEBTPAYMENTDATE.ToString("G"),
                                                   explanation = s.EXPLANATION,
                                                   debtamount = _ctx.MP_PARKINGDEBTs.Where(w => w.DEBTPAYMENTID == s.DEBTPAYMENTID).Select(s => s.DEBTAMOUNT).FirstOrDefault()

                                               }).ToList();


                        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                        {
                            switch (sortColumn)
                            {
                                case "reportId":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.reportId ascending select o).ToList();
                                    break;
                                case "plaka":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.plaka descending select o).ToList();
                                    else
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.plaka ascending select o).ToList();
                                    break;
                                case "locname":
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.locname descending select o).ToList();
                                    else
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.locname ascending select o).ToList();
                                    break;
                                default:
                                    if (sortColumnDir == "desc")
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.reportId descending select o).ToList();
                                    else
                                        DebtCollectionCount8 = (from o in DebtCollectionCount8 orderby o.reportId ascending select o).ToList();
                                    break;
                            }
                        }

                        return Json(new
                        {
                            draw = draw,
                            recordsFiltered = recordsTotal,
                            recordsTotal = recordsTotal,
                            data = DebtCollectionCount8
                        });
                    }
                }
            }


        }
    }
}

