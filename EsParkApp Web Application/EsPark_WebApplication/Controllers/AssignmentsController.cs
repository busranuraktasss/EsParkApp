using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsPark_WebApplication.Controllers
{
    [Authorize]

    public class AssignmentsController : Controller
    {
        private readonly EntitiesContext _ctx;

        public AssignmentsController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowAssignments()
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
                var recordsTotal = _ctx.MP_ASSIGNMENTs.Where(w => w.ISDELETED == 0 && w.ISDELETED == 0).Count();

                var AssignmentListCount = _ctx.MP_ASSIGNMENTs.Where(w => w.LOCID.ToString().Contains(searchValue) && w.ISDELETED == 0 && w.ISDELETED == 0)
                    .Select(s => new showAssignmentRequest()
                    {
                        DT_AssignId = "id_" + s.ASSIGNMENTID.ToString().Trim(),
                        AssignId = s.ASSIGNMENTID,
                        lokasyonAdi = _ctx.MP_LOCATIONs.Where(t => t.LOCID == s.LOCID).Select(s => s.LOCNAME).FirstOrDefault(),
                        cihazNo = _ctx.MP_DEVICEs.Where(t => t.DEVICEID == s.TERMID).Select(s => s.SERIALNO).FirstOrDefault(),
                        kullaniciAdi = _ctx.MP_USERs.Where(t => t.ROWID == s.USERID).Select(s => s.USERNAME).FirstOrDefault(),


                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "AssignId":
                            if (sortColumnDir == "desc")
                                AssignmentListCount = (from o in AssignmentListCount orderby o.AssignId descending select o).ToList();
                            else
                                AssignmentListCount = (from o in AssignmentListCount orderby o.AssignId ascending select o).ToList();
                            break;
                        case "kullaniciAdi":
                            if (sortColumnDir == "desc")
                                AssignmentListCount = (from o in AssignmentListCount orderby o.kullaniciAdi descending select o).ToList();
                            else
                                AssignmentListCount = (from o in AssignmentListCount orderby o.kullaniciAdi ascending select o).ToList();
                            break;
                        case "cihazNo":
                            if (sortColumnDir == "desc")
                                AssignmentListCount = (from o in AssignmentListCount orderby o.cihazNo descending select o).ToList();
                            else
                                AssignmentListCount = (from o in AssignmentListCount orderby o.cihazNo ascending select o).ToList();
                            break;
                        default:
                            if (sortColumnDir == "desc")
                                AssignmentListCount = (from o in AssignmentListCount orderby o.lokasyonAdi descending select o).ToList();
                            else
                                AssignmentListCount = (from o in AssignmentListCount orderby o.lokasyonAdi ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = AssignmentListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, data = "", messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddAssignment(addAssignmentRequest request)
        {
            try
            {                
                _ctx.MP_ASSIGNMENTs.Add(new MP_ASSIGNMENTS()
                {
                    LOCID = request.lokasyonAdi,
                    TERMID = request.cihazNo,
                    USERID = request.kullaniciAdi,
                    RECDATE = DateTime.Now,
                    ISACTIVE = request.isactive,
                    ISDELETED = 0
                });
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, Messages = "Ekleme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteAssignmnet(int pr)
        {
            try
            {
                var current = await _ctx.MP_ASSIGNMENTs.Where(w => w.ASSIGNMENTID == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.ISDELETED = 1;//Soft Delete
                _ctx.MP_ASSIGNMENTs.Update(current);
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = $"{current.ASSIGNMENTID} isimli Kullanıcı Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAssignment(int sId)
        {
            var currentData = await _ctx.MP_ASSIGNMENTs.Where(f => f.ASSIGNMENTID == sId).Select(s => new { s.LOCID , s.USERID, s.TERMID, s.ASSIGNMENTID, s.ISACTIVE }).FirstOrDefaultAsync();

            return Json(new { Status = true, data = currentData, Messages = "Success" });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAssignment(addAssignmentRequest pr)
        {
            try
            {
                var current = await _ctx.MP_ASSIGNMENTs.Where(w => w.ASSIGNMENTID == pr.assignId).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.USERID = pr.kullaniciAdi;
                current.TERMID = pr.cihazNo;
                current.LOCID = pr.lokasyonAdi;
                //current.RECDATE = DateTime.Now;
                current.ISACTIVE = pr.isactive;

                _ctx.MP_ASSIGNMENTs.Update(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Dropdown1()
        {
            var ListCount = _ctx.MP_LOCATIONs.Where(t => t.LOCNAME != null)
                .Select(s => new showAssignmentRequest()
                {
                    AssignId = s.LOCID,
                    lokasyonAdi = _ctx.MP_LOCATIONs.Where(t => t.LOCID == s.LOCID).Select(s => s.LOCNAME).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }

        [HttpGet]
        public JsonResult Dropdown2()
        {
            var ListCount = _ctx.MP_DEVICEs.Where(t => t.SERIALNO != null)
                .Select(s => new showAssignmentRequest()
                {
                    AssignId = s.DEVICEID,
                    cihazNo = _ctx.MP_DEVICEs.Where(t => t.DEVICEID == s.DEVICEID).Select(s => s.SERIALNO).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }
        
        [HttpGet]
        public JsonResult Dropdown3()
        {
            var ListCount = _ctx.MP_USERs.Where(t => t.USERNAME != null)
                .Select(s => new showAssignmentRequest()
                {
                    AssignId = s.ROWID,
                    kullaniciAdi = _ctx.MP_USERs.Where(t => t.ROWID == s.ROWID).Select(s => s.USERNAME).FirstOrDefault(),

                }).ToList();

            return Json(new { data = ListCount });
        }

    }
}
