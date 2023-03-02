using EsPark_WebApplication.Helper.DTO;
using EsPark_WebApplication.Helper.DTO.AddRequest;
using EsPark_WebApplication.Helper.DTO.UpdateRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EsPark_WebApplication.Contrsollers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly EntitiesContext _ctx;

        public UsersController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }
                
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShowUsers()
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
                var recordsTotal = _ctx.MP_USERs.Where(w => w.ISDELETED == 0).Count();

                var UsersListCount = _ctx.MP_USERs.Where(w => w.USERNAME.Contains(searchValue) && w.ISDELETED == 0 )
                    .Select(s => new showUserRequest()
                    {
                        DT_RowId = "id_" + s.ROWID.ToString().Trim(),
                        RowId = s.ROWID,
                        Username = s.USERNAME,
                        Realname = s.REALNAME,
                        Authority = s.AUTHORITY,
                        Statuss = s.STATUS,
                        Phone = s.PHONE,

                    }).Skip(skip).Take(pageSize).ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "RowId":
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.RowId descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.RowId ascending select o).ToList();
                            break;
                        case "Username":
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.Username descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.Username ascending select o).ToList();
                            break;
                        case "Realname":
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.Realname descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.Realname ascending select o).ToList();
                            break;
                        case "Authority":
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.Authority descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.Authority ascending select o).ToList();
                            break;
                        case "Statuss":
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.Statuss descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.Statuss ascending select o).ToList();
                            break;
                        case "Phone":
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.Phone descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.Phone ascending select o).ToList();
                            break;

                        default:
                            if (sortColumnDir == "desc")
                                UsersListCount = (from o in UsersListCount orderby o.RowId descending select o).ToList();
                            else
                                UsersListCount = (from o in UsersListCount orderby o.RowId ascending select o).ToList();
                            break;
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = UsersListCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, data = "", messages = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> CheckUsers(int request)
        {
            try
            {
                var current = await _ctx.MP_USERs.Where(w => w.ROWID == request).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "" });

                if(current.STATUS == 1)
                {
                    current.STATUS = -1;
                }
                else
                {
                    current.STATUS = 1;
                }
                _ctx.MP_USERs.Update(current);

                await _ctx.SaveChangesAsync();
                
                return Json(new { Status = true, data = "", Messages = ""});

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUser(int sId)
        {
            var currentData = await _ctx.MP_USERs.FirstOrDefaultAsync(f => f.ROWID == sId);
            return Json(currentData);

        }

        [HttpPost]
        public async Task<JsonResult> UpdateUser(updateUserRequest pr)
        {
            try
            {
                var current = await _ctx.MP_USERs.Where(w => w.ROWID == pr.updateId).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.USERNAME = pr.Username;
                current.PASSWORD = pr.Password;
                current.PHONE = pr.Phone;
                current.REALNAME = pr.Realname;
                current.STATUS = pr.Durum;
                current.AUTHORITY = pr.Authority;
              
                _ctx.MP_USERs.Update(current);
                await _ctx.SaveChangesAsync();

                return Json(new { Status = true, Messages = "Değiştirme işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", Messages = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddUser(addUserRequest request)
        {
            try
            {
                _ctx.MP_USERs.Add(new MP_USERS()
                {
                    USERNAME = request.UserName,
                    PASSWORD = request.Password,
                    PHONE = request.Phone,
                    REALNAME = request.RealName,
                    AUTHORITY = request.Authority,
                    STATUS = request.Durum,
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
        public async Task<JsonResult> DeleteUser(int pr)
        {
            try
            {
                var current = await _ctx.MP_USERs.Where(w => w.ROWID == pr).FirstOrDefaultAsync();
                if (current == null) return Json(new { Status = false, data = "", Messages = "Ürün bulunamadı." });
                current.ISDELETED = 1;//Soft Delete
                _ctx.MP_USERs.Update(current);
                await _ctx.SaveChangesAsync();
                return Json(new { Status = true, data = "", Messages = $"{current.USERNAME} isimli Kullanıcı Silme işlemi başarılı" });

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, data = "", messages = ex.Message });

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
