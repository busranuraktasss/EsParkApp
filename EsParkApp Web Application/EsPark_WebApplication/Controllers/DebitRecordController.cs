using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Helper.DTO.ShowRequest;
using EsPark_WebApplication.Helper.DTO.UpdateRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class DebitRecordController : Controller
    {
        private readonly EntitiesContext _ctx;

        public DebitRecordController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowParkings(string sId)
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
                var recordsTotal = _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE == sId && w.DEBTPAYMENTID == null).Count();

                var ParkingListCount = _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE.Contains(searchValue) && w.LICENSEPLATE == sId && w.DEBTPAYMENTID == null)
                    .Select(s => new showParkingsRequest()
                    {
                        parkingDebtId = s.PARKINGDEBTID,
                        licenseplate = s.LICENSEPLATE,
                        rowId = (from y in _ctx.MP_USERs
                                    join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                    join a in _ctx.MP_PARKINGDEBTs on z.ID equals a.PARKINGJRID
                                    where a.PARKINGDEBTID == s.PARKINGDEBTID
                                    select y.USERNAME).FirstOrDefault(),
                        locId = (from y in _ctx.MP_LOCATIONs
                                   join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                   join a in _ctx.MP_PARKINGDEBTs on z.ID equals a.PARKINGJRID
                                   where a.PARKINGDEBTID == s.PARKINGDEBTID
                                   select y.LOCNAME).FirstOrDefault(),
                        enddate = _ctx.MP_PARKINGs.Where(w => w.PARKINGID == s.PARKINGID).Select(s => s.ENDDATE.ToString("dd-MM-yyyy HH:mm:ss")).FirstOrDefault(),
                        startdate = _ctx.MP_PARKINGs.Where(w => w.PARKINGID == s.PARKINGID).Select(s => s.STARTDATE.ToString("dd-MM-yyyy HH:mm:ss")).FirstOrDefault(),
                        parkingdurationforovertime = _ctx.MP_PARKINGs.Where(w => w.PARKINGID == s.PARKINGID).Select(s => s.PARKINGFEEFOROVERTIME - s.PAIDFEEFOROVERTIME).FirstOrDefault(),
                        paidfeeforovertime = s.DEBTAMOUNT,
                        lawyer = s.LAWYER,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "REALNAME":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                        case "LOCNAME":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.rowId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.rowId ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = ParkingListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    data = "",
                    messages = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetTET(string sId)
        {
            var currentTET = Convert.ToInt32((from x in _ctx.MP_PARKINGDEBTs join y in _ctx.MP_DEBTPAYMENTs on x.DEBTPAYMENTID equals y.DEBTPAYMENTID where x.LICENSEPLATE == sId select y.DEBTPAYMENTAMOUNT).Sum());

            return Json(new { Status = true, data = currentTET, Messages = "Success", Code = 200 });
        }

        [HttpPost]
        public async Task<JsonResult> GetTB(string sId)
        {
            int currentTB = Convert.ToInt32((from t in _ctx.MP_PARKINGDEBTs where t.LICENSEPLATE == sId select t.DEBTAMOUNT).Sum());

            return Json(new { Status = true, data = currentTB, Messages = "Success", Code = 200 });
        }

        [HttpPost]
        public async Task<JsonResult> GetKTB(string sId)
        {
            int currentTB = Convert.ToInt32((from t in _ctx.MP_PARKINGDEBTs where t.LICENSEPLATE == sId && t.DEBTPAYMENTID == null select t.DEBTAMOUNT).Sum());

            return Json(new { Status = true, data = currentTB, Messages = "Success", Code = 200 });
        }

        [HttpPost]
        public async Task<JsonResult> GetSBT(int sId)
        {
            int currentSBT = Convert.ToInt32((from t in _ctx.MP_PARKINGDEBTs where t.PARKINGDEBTID == sId && t.DEBTPAYMENTID == null select t.DEBTAMOUNT).Sum());

            return Json(new { Status = true, data = currentSBT, Messages = "Success", Code = 200 });
        }

        [HttpPost]
        public async Task<JsonResult> AddDebtpayment(addDebtpaymentSelectRequest request)
        {
            try
            {
                var current = await _ctx.MP_PARKINGDEBTs.Where(w => w.PARKINGDEBTID == request.parkingId).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.EXPLANATION = request.aciklama;

                var elementToInsert = new MP_DEBTPAYMENTS()
                {
                    DEBTPAYMENTAMOUNT = request.silinecekBorcTutarı,
                    COLLECTIONJRID = 0,
                    DEBTPAYMENTDATE = DateTime.Now,
                    EXPLANATION = request.aciklama,
                    COLLECTIONTYPE = 2
                };
                _ctx.MP_DEBTPAYMENTs.Add(elementToInsert);
                await _ctx.SaveChangesAsync();
                current.DEBTPAYMENTID = elementToInsert.DEBTPAYMENTID;

                _ctx.MP_PARKINGDEBTs.Update(current);
                await _ctx.SaveChangesAsync();



                return Json(new { Status = true, Messages = "Success", Code = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }

        }
        [HttpPost]
        public async Task<JsonResult> AddDebtpaymentArchieve(addDebtpaymentSelectRequest request)
        {
            try
            {
                var current = await _ctx.MP_PARKINGDEBTs.Where(w => w.PARKINGDEBTID == request.parkingId).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.EXPLANATION = request.aciklama;

                var elementToInsert = new MP_DEBTPAYMENTS()
                {
                    DEBTPAYMENTAMOUNT = request.silinecekBorcTutarı,
                    COLLECTIONJRID = 0,
                    DEBTPAYMENTDATE = DateTime.Now,
                    EXPLANATION = request.aciklama,
                    COLLECTIONTYPE = 2,
                    ETTN = Guid.NewGuid(),

                };
                _ctx.MP_DEBTPAYMENTs.Add(elementToInsert);
                await _ctx.SaveChangesAsync();
                current.DEBTPAYMENTID = elementToInsert.DEBTPAYMENTID;

                _ctx.MP_PARKINGDEBTs.Update(current);
                await _ctx.SaveChangesAsync();



                return Json(new { Status = true, Messages = "Success", Code = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }

        }

        [HttpPost]
        public async Task<JsonResult> AddDebtpaymentAll(addDebtpaymentRequest request)
        {
            try
            {
                var elementToInsert = new MP_DEBTPAYMENTS()
                {
                    DEBTPAYMENTAMOUNT = request.silinecekBorcTutarı,
                    COLLECTIONJRID = 0,
                    DEBTPAYMENTDATE = DateTime.Now,
                    EXPLANATION = request.aciklama,
                    COLLECTIONTYPE = 2
                };
                _ctx.MP_DEBTPAYMENTs.Add(elementToInsert);
                await _ctx.SaveChangesAsync();



                return Json(new { Status = true, data = elementToInsert.DEBTPAYMENTID, Messages = "Success", Code = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }

        }
        [HttpPost]
        public async Task<JsonResult> AddDebtpaymentAllArchieve(addDebtpaymentRequest request)
        {
            try
            {
                var elementToInsert = new MP_DEBTPAYMENTS()
                {
                    DEBTPAYMENTAMOUNT = request.silinecekBorcTutarı,
                    COLLECTIONJRID = 0,
                    DEBTPAYMENTDATE = DateTime.Now,
                    EXPLANATION = request.aciklama,
                    COLLECTIONTYPE = 2,
                    ETTN = Guid.NewGuid(),
                };
                _ctx.MP_DEBTPAYMENTs.Add(elementToInsert);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = elementToInsert.DEBTPAYMENTID, Messages = "Success", Code = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }

        }

        [HttpPost]
        public async Task<JsonResult> UpdateParkingDebt(addDebtpaymentRequest request)
        {
            try
            {

                var current = await _ctx.MP_PARKINGDEBTs.Where(w => request.parkingIds.Contains(w.PARKINGDEBTID)).ToListAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                foreach (var debt in current)
                {
                    debt.EXPLANATION = request.aciklama;
                    debt.DEBTPAYMENTID = request.deptpaymentId;
                }

                _ctx.MP_PARKINGDEBTs.UpdateRange(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteParking(int pr)
        {
            try
            {
                var current =  _ctx.MP_PARKINGDEBTs.Where(w => w.PARKINGDEBTID == pr).FirstOrDefault();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                _ctx.MP_PARKINGDEBTs.Remove(current);//Hard Delete

                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = " Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAllParking(int[] pr)
        {
            try
            {
                var current = await _ctx.MP_PARKINGDEBTs.Where(w => pr.Contains(w.PARKINGDEBTID)).ToListAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });

                _ctx.MP_PARKINGDEBTs.RemoveRange(current);//Hard Delete
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = " Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpPost]
        public async Task<JsonResult> TransferSelect(updateParkingDebtsTransferRequest request)
        {
            try
            {

                var currentAktaran = await _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE == request.aktaranPlaka && request.plakaIds.Contains(w.PARKINGDEBTID)).ToListAsync();
                if (currentAktaran == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });


                var currentAktarılan = await _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE == request.aktarılanPlaka).FirstOrDefaultAsync();
                if (currentAktarılan == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                foreach (var debt in currentAktaran)
                {
                    debt.LICENSEPLATE = request.aktarılanPlaka;
                }

                _ctx.MP_PARKINGDEBTs.UpdateRange(currentAktaran);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "", Messages = " Güncelleme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> TransferPlakaSelect(updateParkingDebtsTransferRequest request)
        {
            try
            {

                var currentAktaran = await _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE == request.aktaranPlaka && request.plakaIds.Contains(w.PARKINGDEBTID)).ToListAsync();
                if (currentAktaran == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });


                var Plaka = await _ctx.MP_OFFICIALPLATEs.Where(w => w.PID == request.aktarılanPlakaId).Select(s => s.LICENSEPLATE).FirstOrDefaultAsync();

                foreach (var debt in currentAktaran)
                {
                    debt.LICENSEPLATE = Plaka;
                }

                _ctx.MP_PARKINGDEBTs.UpdateRange(currentAktaran);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "", Messages = " Güncelleme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> Dropdown3(int pr)
        {
            var ListCount = _ctx.MP_OFFICIALPLATEs.Where(t => t.LICENSEPLATE != null && t.CUSTOMERBAG == pr)
                .Select(s => new showCustomerResponse()
                {
                    cusId = s.PID,
                    adsoyad = _ctx.MP_OFFICIALPLATEs.Where(t => t.PID == s.PID).Select(s => s.LICENSEPLATE).FirstOrDefault(),

                }).ToList();

            return Json(new { Status = true, data = ListCount });
        }

        [HttpGet]
        public async Task<JsonResult> JobRotationControl(controlJobRotationRequest request)
        {

            var currentJob =  _ctx.MP_JOBROTATIONHISTORYs.Where(w => w.USERID == request.userId && w.BEGINDATE.Date == request.date.Date).Select(s => s.ID).FirstOrDefault();

            if (currentJob == 0)
            {
                return Json(new { Status = false, data = "" });
            }

            return Json(new { Status = true, data = currentJob });
        }

        [HttpPost]
        public async Task<JsonResult> AddDebts(controlJobRotationRequest request)
        {
            try
            {

                var currentParking = new MP_PARKINGS()
                {
                    SESSIONID = "EsParkApp",
                    JOBROTATIONHISTORYID = request.jobId,
                    PARKINGSTATUS = 3,
                    TARIFFID = 4,
                    LICENSEPLATE = request.plaka,
                    STARTDATE = request.date,
                    ENDDATE = request.date,
                    PERON = 1,
                    PARKINGDURATION = 15,
                    PARKINGFEE = 0,
                    PAIDFEE = 0,
                    PARKINGDURATIONFOROVERTIME = 0,
                    PARKINGFEEFOROVERTIME = request.ucret,
                    PAIDFEEFOROVERTIME = 0,
                    ENDDATEFOROVERTIME = request.date,
                    OUTDATE = request.date,
                    INDEBTED = 1,
                    GBORC = 0,
                    LocaliD = 0,
                    VerifyEnrollment_In = null,
                    VerifyEnrollment_Out = null,
                    OUT_F_NO = null,
                    IN_F_NO = null, 
                    OUT_ETTN = null,
                    IN_ETTN = null,

                };
                _ctx.MP_PARKINGs.AddAsync(currentParking);
                await _ctx.SaveChangesAsync();

                var parkingId = currentParking.PARKINGID;

                var currentPDebt = new MP_PARKINGDEBTS()
                {
                    PARKINGID = parkingId,
                    PARKINGJRID = request.jobId,
                    LICENSEPLATE = request.plaka,
                    DEBTAMOUNT = request.ucret,
                    LAWYER = false

                };

                _ctx.MP_PARKINGDEBTs.Add(currentPDebt);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "" });

            }
            catch
            {
                return Json(new { Status = false, data = "" });

            }
        }
        
        [HttpPost]
        public async Task<JsonResult> KAddDebts(controlJobRotationRequest request)
        {
            try
            {

                var currentParking = new MP_PARKINGS()
                {
                    SESSIONID = "EsParkApp",
                    JOBROTATIONHISTORYID = request.jobId,
                    PARKINGSTATUS = 3,
                    TARIFFID = 4,
                    LICENSEPLATE = _ctx.MP_OFFICIALPLATEs.Where(w => w.PID == request.plakaId).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                    STARTDATE = request.date,
                    ENDDATE = request.date,
                    PERON = 1,
                    PARKINGDURATION = 15,
                    PARKINGFEE = 0,
                    PAIDFEE = 0,
                    PARKINGDURATIONFOROVERTIME = 0,
                    PARKINGFEEFOROVERTIME = request.ucret,
                    PAIDFEEFOROVERTIME = 0,
                    ENDDATEFOROVERTIME = request.date,
                    OUTDATE = request.date,
                    INDEBTED = 1,
                    GBORC = 0,
                    LocaliD = 0,
                    VerifyEnrollment_In = null,
                    VerifyEnrollment_Out = null,
                    OUT_F_NO = null,
                    IN_F_NO = null, 
                    OUT_ETTN = null,
                    IN_ETTN = null,

                };
                _ctx.MP_PARKINGs.AddAsync(currentParking);
                await _ctx.SaveChangesAsync();

                var parkingId = currentParking.PARKINGID;

                var currentPDebt = new MP_PARKINGDEBTS()
                {
                    PARKINGID = parkingId,
                    PARKINGJRID = request.jobId,
                    LICENSEPLATE = _ctx.MP_OFFICIALPLATEs.Where(w => w.PID == request.plakaId).Select(s => s.LICENSEPLATE).FirstOrDefault(),
                    DEBTAMOUNT = request.ucret,
                    LAWYER = false

                };

                _ctx.MP_PARKINGDEBTs.Add(currentPDebt);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "" });

            }
            catch
            {
                return Json(new { Status = false, data = "" });

            }
        }

        [HttpPost]
        public JsonResult ShowParkRecord(showParkingsRequest request)
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

                //Paging Size 
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var recordsTotal = _ctx.MP_PARKINGs.Where(w => w.LICENSEPLATE == request.licenseplate && w.STARTDATE >= request.date && w.PAIDFEE == 0 && w.PARKINGFEE == 0 && w.INDEBTED == null).Count();

                var ParkingListCount = _ctx.MP_PARKINGs.Where(w => w.LICENSEPLATE == request.licenseplate && w.STARTDATE >= request.date && w.PAIDFEE == 0 && w.PARKINGFEE == 0 && w.INDEBTED == null)
                    .Select(s => new showParkingsRequest()
                    {
                        DT_ParkingsId = "id_" + s.PARKINGID.ToString().Trim(),
                        parkingDebtId = s.PARKINGID,

                        licenseplate = s.LICENSEPLATE,
                        locId = (from y in _ctx.MP_LOCATIONs
                                 join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                 where z.ID == s.JOBROTATIONHISTORYID
                                 select y.LOCNAME).FirstOrDefault(),
                        rowId = (from y in _ctx.MP_USERs
                                 join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                 where z.ID == s.JOBROTATIONHISTORYID
                                 select y.USERNAME).FirstOrDefault(),
                        startdate = s.STARTDATE.ToString("dd-MM-yyyy HH:mm:ss"),
                        enddate = s.ENDDATE.ToString("dd-MM-yyyy HH:mm:ss"),
                        parkingdurationforovertime = s.PARKINGDURATIONFOROVERTIME,
                        parkingfeeforovertime = s.PARKINGFEEFOROVERTIME,
                        paidfeeforovertime = s.PAIDFEEFOROVERTIME,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "licenseplate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                        case "startdate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate ascending select o).ToList();
                            break;
                        case "enddate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.enddate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.enddate ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = ParkingListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddDebtRecord(int sId)
        {
            try
            {

                var currentPark = _ctx.MP_PARKINGs.Where(w => w.PARKINGID == sId).FirstOrDefault();
                if (currentPark == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                currentPark.PAIDFEE = 1;
                currentPark.PARKINGFEE = 1;
                currentPark.INDEBTED = 1;

                _ctx.MP_PARKINGs.Update(currentPark);
                await _ctx.SaveChangesAsync();

                var currentParkDebt = new MP_PARKINGDEBTS()
                {
                    PARKINGID = currentPark.PARKINGID,
                    PARKINGJRID = currentPark.JOBROTATIONHISTORYID,
                    LICENSEPLATE = currentPark.LICENSEPLATE,
                    DEBTAMOUNT = (decimal)currentPark.PARKINGFEEFOROVERTIME,
                };
                _ctx.MP_PARKINGDEBTs.Add(currentParkDebt);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, data = "" });

            }
            catch
            {
                return Json(new { Status = false, data = "" });

            }
        }

        [HttpGet]
        public async Task<JsonResult> DebtControl(string plaka)
        {
            try
            {
                var current = await _ctx.MP_PARKINGDEBTs.Where(w => plaka.Contains(w.LICENSEPLATE) && w.DEBTPAYMENTID != null).ToArrayAsync();

                return Json(new { Status = true, data = current });
            }
            catch
            {
                return Json(new { Status = false, data = "" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ShowDeletedDebts(showDeleteDebtsRequest request)
        {
            try
            {
                var current = _ctx.MP_PARKINGDEBTs.Where(w => request.plaka.Contains(w.LICENSEPLATE) && w.DEBTPAYMENTID != null).Select(s => s.PARKINGID).ToArray();


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

                //Paging Size 
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                if (request.startdate == request.enddate)
                {
                    var recordsTotal1 = _ctx.MP_PARKINGs.Where(w => current.Contains(w.PARKINGID)).Count();

                    var ParkingListCount1 = _ctx.MP_PARKINGs.Where(w => current.Contains(w.PARKINGID))
                        .Select(s => new showParkingsRequest()
                        {
                            parkingId = s.PARKINGID,
                            licenseplate = s.LICENSEPLATE,
                            rowId = (from y in _ctx.MP_USERs
                                     join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                     where z.ID == s.JOBROTATIONHISTORYID
                                     select y.USERNAME).FirstOrDefault(),
                            locId = (from y in _ctx.MP_LOCATIONs
                                     join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                     where z.ID == s.JOBROTATIONHISTORYID
                                     select y.LOCNAME).FirstOrDefault(),
                            startdate = s.STARTDATE.ToString("dd-MM-yyyy HH:mm:ss"),
                            enddate = s.ENDDATE.ToString("dd-MM-yyyy HH:mm:ss"),
                            parkingdurationforovertime = s.PARKINGDURATIONFOROVERTIME,
                            parkingfeeforovertime = s.PARKINGFEEFOROVERTIME,

                        }).Skip(skip).Take(pageSize).ToList();

                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        switch (sortColumn)
                        {
                            case "licenseplate":
                                if (sortColumnDir == "desc")
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.licenseplate descending select o).ToList();
                                else
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.licenseplate ascending select o).ToList();
                                break;
                            case "startdate":
                                if (sortColumnDir == "desc")
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.startdate descending select o).ToList();
                                else
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.startdate ascending select o).ToList();
                                break;
                            case "enddate":
                                if (sortColumnDir == "desc")
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.enddate descending select o).ToList();
                                else
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.enddate ascending select o).ToList();
                                break;
                            default:
                                if (sortColumnDir == "desc")
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.licenseplate descending select o).ToList();
                                else
                                    ParkingListCount1 = (from o in ParkingListCount1 orderby o.licenseplate ascending select o).ToList();
                                break;
                        }
                    }

                    return Json(new
                    {
                        draw = draw,
                        recordsFiltered = recordsTotal1,
                        recordsTotal = recordsTotal1,
                        data = ParkingListCount1
                    });
                }
                else
                {
                    var ParkingIds = _ctx.MP_PARKINGs.Where(w => w.STARTDATE >= request.startdate && request.enddate >= w.ENDDATE).Select(s => s.PARKINGID).ToList();
                    var ParkingDebts = _ctx.MP_PARKINGDEBTs.Where(t => ParkingIds.Contains(t.PARKINGID)).Select(v => v.PARKINGID).ToList();
                    var total = _ctx.MP_PARKINGDEBTs.Where(t => ParkingIds.Contains(t.PARKINGID)).Select(v => v.PARKINGID).Count();



                    var ParkingListCount = _ctx.MP_PARKINGs.Where(w => ParkingDebts.Contains(w.PARKINGID) && w.STARTDATE > request.startdate && request.enddate >= w.ENDDATE)
                        .Select(s => new showParkingsRequest()
                        {
                            licenseplate = s.LICENSEPLATE,
                            rowId = (from y in _ctx.MP_USERs
                                     join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                     where z.ID == s.JOBROTATIONHISTORYID
                                     select y.USERNAME).FirstOrDefault(),
                            locId = (from y in _ctx.MP_LOCATIONs
                                     join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                     where z.ID == s.JOBROTATIONHISTORYID
                                     select y.LOCNAME).FirstOrDefault(),
                            startdate = s.STARTDATE.ToString("dd-MM-yyyy HH:mm:ss"),
                            enddate = s.ENDDATE.ToString("dd-MM-yyyy HH:mm:ss"),
                            parkingdurationforovertime = s.PARKINGDURATIONFOROVERTIME,
                            parkingfeeforovertime = s.PARKINGFEEFOROVERTIME,

                        }).Skip(skip).Take(pageSize).ToList();

                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        switch (sortColumn)
                        {
                            case "licenseplate":
                                if (sortColumnDir == "desc")
                                    ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                                else
                                    ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                                break;
                            case "startdate":
                                if (sortColumnDir == "desc")
                                    ParkingListCount = (from o in ParkingListCount orderby o.startdate descending select o).ToList();
                                else
                                    ParkingListCount = (from o in ParkingListCount orderby o.startdate ascending select o).ToList();
                                break;
                            case "enddate":
                                if (sortColumnDir == "desc")
                                    ParkingListCount = (from o in ParkingListCount orderby o.enddate descending select o).ToList();
                                else
                                    ParkingListCount = (from o in ParkingListCount orderby o.enddate ascending select o).ToList();
                                break;
                            default:
                                if (sortColumnDir == "desc")
                                    ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                                else
                                    ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                                break;
                        }
                    }

                    return Json(new
                    {
                        draw = draw,
                        recordsFiltered = total,
                        recordsTotal = total,
                        data = ParkingListCount
                    });
                }



            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> TransferDeletedDebts(int[] parkingId)
        {
            try
            {
                var parkDebtPrice = _ctx.MP_PARKINGDEBTs.Where(w => parkingId.Contains(w.PARKINGID)).Select(s => s.DEBTAMOUNT).ToList();
                var parkDebtId = _ctx.MP_PARKINGDEBTs.Where(w => parkingId.Contains(w.PARKINGID)).Select(s => s.DEBTPAYMENTID).ToList();

                var debtpayment = _ctx.MP_DEBTPAYMENTs.Where(w => parkDebtId.Contains(w.DEBTPAYMENTID)).ToList();

                var currentPrice = _ctx.MP_DEBTPAYMENTs.Where(w => parkDebtId.Contains(w.DEBTPAYMENTID)).Select(s => s.DEBTPAYMENTAMOUNT).FirstOrDefault();

                var currentEqual = Convert.ToDecimal((parkDebtPrice).Sum());

                var equal = currentPrice - currentEqual;

                var current = _ctx.MP_DEBTPAYMENTs.Where(w => parkDebtId.Contains(w.DEBTPAYMENTID)).FirstOrDefault();

                current.DEBTPAYMENTAMOUNT = equal;

                if (equal == 0)
                {
                    _ctx.MP_DEBTPAYMENTs.RemoveRange(current);
                }
                else
                {
                    _ctx.MP_DEBTPAYMENTs.Update(current);
                }

                await _ctx.SaveChangesAsync();


                var parkingDebt = _ctx.MP_PARKINGDEBTs.Where(w => parkingId.Contains(w.PARKINGID)).ToList();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });

                foreach (var debt in parkingDebt)
                {
                    debt.EXPLANATION = null;
                    debt.DEBTPAYMENTID = null;
                }

                _ctx.MP_PARKINGDEBTs.UpdateRange(parkingDebt);
                await _ctx.SaveChangesAsync();



                return Json(new { Status = true, data = "" });

            }
            catch
            {
                return Json(new { Status = false, data = "" });

            }
        }

        [HttpPost]
        public JsonResult CheckedExecutionDebts(int[] sId , string sPlaka)
        {
            try
            {
                var executionDebtsId = _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE == sPlaka && sId.Contains(w.PARKINGDEBTID)).ToList();
                if (executionDebtsId == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });

                foreach (var debt in executionDebtsId)
                {
                    debt.LAWYER = true;
                }

                _ctx.MP_PARKINGDEBTs.UpdateRange(executionDebtsId);
                _ctx.SaveChanges();

                return Json(new { Status = true, data = "", Messages = " Güncelleme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CheckedNotExecutionDebts(int[] sId , string sPlaka)
        {
            try
            {
                var notExecutionDebtsId = _ctx.MP_PARKINGDEBTs.Where(w => w.LICENSEPLATE == sPlaka && sId.Contains(w.PARKINGDEBTID)).ToList();
                if (notExecutionDebtsId == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });

                foreach (var debt in notExecutionDebtsId)
                {
                    debt.LAWYER = false;
                }

                _ctx.MP_PARKINGDEBTs.UpdateRange(notExecutionDebtsId);
                _ctx.SaveChanges();

                return Json(new { Status = true, data = "", Messages = " Güncelleme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }
    }
}