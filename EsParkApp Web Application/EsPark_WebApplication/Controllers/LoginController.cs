using EsPark_WebApplication.Helper.DTO.getRequest;
using EsPark_WebApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EsPark_WebApplication.Controllers
{
    public class LoginController : Controller
    {

        private readonly EntitiesContext _ctx;

        public LoginController(EntitiesContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(getLoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _ctx.MP_USERs.Where(w => w.USERNAME == request.UserName && w.PASSWORD == request.Password).FirstOrDefaultAsync();

                if (response != null)
                {
                    string JsonToString = JsonConvert.SerializeObject(response);

                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, JsonToString)
                    };
                    var userIdentity = new ClaimsIdentity(claims, "login");

                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    ViewBag.ErrorMessage = "";
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "";
                    ModelState.AddModelError("Error", "");
                }
            }

            return View();
        }
    }
}
