using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.getRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Diagnostics;


namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EntitiesContext _ctx;
        public HomeController(ILogger<HomeController> logger, EntitiesContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        [HttpPost]
        public JsonResult GetShowParkings()
        {
            try
            {
                Request.Form.TryGetValue("draw", out Microsoft.Extensions.Primitives.StringValues drawOut);

                var draw = drawOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][column]", out Microsoft.Extensions.Primitives.StringValues orderColumnOut);
                Request.Form.TryGetValue("columns[" + orderColumnOut.FirstOrDefault() + "][name]", out Microsoft.Extensions.Primitives.StringValues columnsNameOut);
                var sortColumn = columnsNameOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][dir]", out Microsoft.Extensions.Primitives.StringValues sortColumnDirOut);
                var sortColumnDir = sortColumnDirOut.FirstOrDefault();

                Request.Form.TryGetValue("search[value]", out Microsoft.Extensions.Primitives.StringValues searchValueOut);
                var searchValue = searchValueOut.FirstOrDefault() ?? "";

                var recordsTotal = _ctx.MP_PARKINGs.Where(t => t.JOB_OUT != null && t.STARTDATE >= DateTime.Today).Count();

                var ParkingListCount = _ctx.MP_PARKINGs.Where(t => t.JOB_OUT != null && t.STARTDATE >= DateTime.Today)
                    .Select(s => new getParkingsRequest()
                    {
                        DT_ParkingsId = "id_" + s.PARKINGID.ToString().Trim(),
                        parkingId = s.PARKINGID,
                        licenseplate = s.LICENSEPLATE,
                        startdate = s.STARTDATE,
                        parkingduration = s.PARKINGDURATION,
                        parkingdurationforovertime = s.PARKINGDURATIONFOROVERTIME,
                        paidfeeforovertime = s.PARKINGFEEFOROVERTIME,
                        paidfee = s.PAIDFEE,
                        enddate = s.ENDDATE,
                        locId = (from y in _ctx.MP_LOCATIONs
                                 join z in _ctx.MP_JOBROTATIONHISTORYs on y.LOCID equals z.PARKID
                                 where z.ID == s.JOBROTATIONHISTORYID
                                 select y.LOCNAME).FirstOrDefault(),
                        rowId = (from y in _ctx.MP_USERs
                                 join z in _ctx.MP_JOBROTATIONHISTORYs on y.ROWID equals z.USERID
                                 where z.ID == s.JOBROTATIONHISTORYID
                                 select y.USERNAME).FirstOrDefault(),
                        deviceId = (from y in _ctx.MP_DEVICEs
                                    join z in _ctx.MP_JOBROTATIONHISTORYs on y.DEVICEID equals z.DEVICEID
                                    where z.ID == s.JOBROTATIONHISTORYID
                                    select y.SERIALNO).FirstOrDefault(),
                        indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                    }).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "startdate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate ascending select o).ToList();
                            break;
                        case "licenseplate":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                        case "parkıngId":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.parkingId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.parkingId ascending select o).ToList();
                            break;
                        case "rowId":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.rowId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.rowId ascending select o).ToList();
                            break;
                        case "locId":
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.locId descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.locId ascending select o).ToList();
                            break;

                        default:
                            if (sortColumnDir == "desc")
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate descending select o).ToList();
                            else
                                ParkingListCount = (from o in ParkingListCount orderby o.startdate ascending select o).ToList();
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
        public JsonResult getActiveShift()
        {

            DataTable dt = new DataTable();
            IDbConnection connection = _ctx.Database.GetDbConnection();
            connection.Open();
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory((DbConnection)connection);
            var rec = new List<AktifHesapDurumProRequest>();

            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = (DbConnection?)connection;
                cmd.CommandTimeout = 360;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AktifDurumHesapPro";
                SqlParameter _trh = new SqlParameter("@trh", SqlDbType.DateTime);
                _trh.Value = DateTime.Today;
                cmd.Parameters.Add(_trh);

                List<List<String>> ResultSet = new List<List<String>>();

                SqlDataReader reader = (SqlDataReader)cmd.ExecuteReader();
                while (reader.Read())
                {
                    var strList = new List<String>();

                    for (int i = 0; i <= reader.FieldCount - 1; i++)
                    {
                        if (reader.GetDataTypeName(i).Contains("decimal"))
                            strList.Add(reader.IsDBNull(i) ? null : reader.GetDecimal(i).ToString());

                        else if (reader.GetDataTypeName(i).Contains("datetime"))
                            strList.Add(reader.IsDBNull(i) ? null : reader.GetDateTime(i).ToString());

                        else if (reader.GetDataTypeName(i) == "int")
                            strList.Add(reader.IsDBNull(i) ? null : reader.GetInt32(i).ToString());

                        else
                            strList.Add(reader.IsDBNull(i) ? null : reader.GetString(i));
                    }

                    rec.Add(new AktifHesapDurumProRequest()
                    {
                        ID = strList[0],
                        USERNAME = strList[1],
                        LOCNAME = strList[2],
                        SERIALNO = strList[3],
                        CREATEDDATE = strList[4],
                        ARAC = strList[5],
                        TAHSIL = strList[6],
                        TAHSIL_KART = strList[7],
                        BORC = strList[8],
                        BORC_KART = strList[9],
                        TOPLAM = strList[10],
                        TOPLANMASI_GEREKEN = strList[11],
                        BATTERY = strList[12],
                        DOLULUK = strList[13]
                    });
                }
            }

            return Json(new { Status = true, Data = rec });

        }

        [HttpPost]
        public JsonResult CardOpenShow(int Id, string serialno, string username, string locname)
        {
            try
            {
                Request.Form.TryGetValue("draw", out Microsoft.Extensions.Primitives.StringValues drawOut);

                var draw = drawOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][column]", out Microsoft.Extensions.Primitives.StringValues orderColumnOut);
                Request.Form.TryGetValue("columns[" + orderColumnOut.FirstOrDefault() + "][name]", out Microsoft.Extensions.Primitives.StringValues columnsNameOut);
                var sortColumn = columnsNameOut.FirstOrDefault();

                Request.Form.TryGetValue("order[0][dir]", out Microsoft.Extensions.Primitives.StringValues sortColumnDirOut);
                var sortColumnDir = sortColumnDirOut.FirstOrDefault();

                Request.Form.TryGetValue("search[value]", out Microsoft.Extensions.Primitives.StringValues searchValueOut);
                var searchValue = searchValueOut.FirstOrDefault() ?? "";

                var recordsTotal = (from x in _ctx.MP_DEVICEs
                                   join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                   join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                   join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                   join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                   where x.SERIALNO == serialno && z.USERNAME == username && t.LOCNAME == locname && v.STARTDATE >= DateTime.Today
                                   select v.PARKINGID).Count();

                var cardOpenListCount = (from x in _ctx.MP_DEVICEs
                                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.DEVICEID equals y.DEVICEID
                                         join z in _ctx.MP_USERs on y.USERID equals z.ROWID
                                         join t in _ctx.MP_LOCATIONs on y.PARKID equals t.LOCID
                                         join v in _ctx.MP_PARKINGs on y.ID equals v.JOBROTATIONHISTORYID
                                         where x.SERIALNO == serialno && z.USERNAME == username && t.LOCNAME == locname && v.STARTDATE >= DateTime.Today
                                         select v
                                        )
                    .Select(s => new getParkingsRequest()
                    {
                        DT_ParkingsId = "id_" + s.PARKINGID.ToString().Trim(),
                        parkingId = s.PARKINGID,
                        licenseplate = s.LICENSEPLATE,
                        startdate = s.STARTDATE,
                        enddate = s.ENDDATE,
                        parkingduration = s.PARKINGDURATION,
                        parkingfee = s.PARKINGFEE,
                        paidfee = s.PAIDFEE,
                        parkingdurationforovertime = s.PARKINGDURATIONFOROVERTIME,
                        parkingfeeforovertime = s.PARKINGFEEFOROVERTIME,
                        paidfeeforovertime = s.PARKINGFEEFOROVERTIME,
                        outdate = s.OUTDATE,
                        indebted = s.INDEBTED == 1 ? "EVET" : "HAYIR",
                    }).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "startdate":
                            if (sortColumnDir == "desc")
                                cardOpenListCount = (from o in cardOpenListCount orderby o.startdate descending select o).ToList();
                            else
                                cardOpenListCount = (from o in cardOpenListCount orderby o.startdate ascending select o).ToList();
                            break;
                        case "licenseplate":
                            if (sortColumnDir == "desc")
                                cardOpenListCount = (from o in cardOpenListCount orderby o.licenseplate descending select o).ToList();
                            else
                                cardOpenListCount = (from o in cardOpenListCount orderby o.licenseplate ascending select o).ToList();
                            break;
                        case "parkıngId":
                            if (sortColumnDir == "desc")
                                cardOpenListCount = (from o in cardOpenListCount orderby o.parkingId descending select o).ToList();
                            else
                                cardOpenListCount = (from o in cardOpenListCount orderby o.parkingId ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                cardOpenListCount = (from o in cardOpenListCount orderby o.startdate descending select o).ToList();
                            else
                                cardOpenListCount = (from o in cardOpenListCount orderby o.startdate ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = cardOpenListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetAL()
        {
            var currentAL = _ctx.MP_MAPLOCATIONs.Where(w => w.CreatedTime >= DateTime.Today).Select(s => s.JobId).Distinct().ToList();
            return Json(new { Status = true, data = currentAL, });
        }

        [HttpPost]
        public async Task<JsonResult> GetAP()
        {
            var currentAP = _ctx.MP_MAPLOCATIONs.Where(w => w.CreatedTime >= DateTime.Today).Select(s => s.JobId).Distinct().ToList();
            return Json(new { Status = true, data = currentAP, });
        }

        [HttpPost]
        public async Task<JsonResult> GetPEA()
        {
            //parkEdenArac
            var currnetPEA = _ctx.AracDetayliRaporEsPark.Where(w => w.Baslangic >= DateTime.Today).Count();
            return Json(new { Status = true, data = currnetPEA, });
        }

        [HttpPost]
        public async Task<JsonResult> GetIA()
        {
            //icerdekiArac
            var currentIA = _ctx.AracDetayliRaporEsPark.Where(w => w.Baslangic >= DateTime.Today && w.Borcmu == "" && w.Bitis == null).Count();
            return Json(new { Status = true, data = currentIA, });
        }

        [HttpPost]
        public async Task<JsonResult> GetTYA()
        {
            //TahsilatliArac
            var currentTYA = _ctx.AracDetayliRaporEsPark.Where(w => w.Baslangic >= DateTime.Today && w.Borcmu == "" && w.Bitis != null).Count();
            return Json(new { Status = true, data = currentTYA, });
        }

        [HttpPost]
        public async Task<JsonResult> GetBCA()
        {
            //BorcluArac
            var currentBCA = _ctx.AracDetayliRaporEsPark.Where(w => w.Baslangic >= DateTime.Today && w.Borcmu == "EVET").Count();
            return Json(new { Status = true, data = currentBCA, });
        }

        [HttpPost]
        public JsonResult ShiftMapId( )
        {
            var shiftMapId = _ctx.MP_MAPLOCATIONs.Where(w => w.CreatedTime >= DateTime.Today).Select(s => s.JobId).Distinct().ToList();
        
            return Json(new { Status = true, data = shiftMapId });
          
        }

        [HttpPost]
        public JsonResult ShiftMap(int sId)
        {
            var shiftMap = _ctx.MP_MAPLOCATIONs.Where(t => t.JobId == sId).OrderByDescending(o => o.CreatedTime).Select(s => new shiftMapRequest
            {
                Lat = s.Lat,
                Lng = s.Lng,
                JobId = (from x in _ctx.MP_USERs 
                         join y in _ctx.MP_JOBROTATIONHISTORYs on x.ROWID equals y.USERID
                         where y.ID == s.JobId
                         select x.REALNAME ).FirstOrDefault(),
                CreatedTime = s.CreatedTime
            }).Take(1);

            return Json(new { Status = true, data = shiftMap });
           
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}