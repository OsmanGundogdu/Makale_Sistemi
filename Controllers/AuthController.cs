using Microsoft.AspNetCore.Mvc;
using MakaleSistemi.Data;
using MakaleSistemi.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace MakaleSistemi.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("kaydol")]
        public IActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        [Route("kaydol")]
        public IActionResult KayitOl(Kullanici model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (_context.Kullanicilar.Any(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu e-posta zaten kayıtlı!");
                    return View(model);
                }

                if (model.Rol == "Hakem" && string.IsNullOrWhiteSpace(model.IlgiAlani))
                {
                    ModelState.AddModelError("IlgiAlani", "Hakem olarak kayıt olurken ilgi alanı girmeniz zorunludur!");
                    return View(model);
                }

                _context.Kullanicilar.Add(model);
                _context.SaveChanges();

                TempData["BasariMesaji"] = "Kayıt işlemi başarılı! Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("GirisYap");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bir hata oluştu, lütfen tekrar deneyin.");
                Console.WriteLine($"Kayıt hatası: {ex.Message}");
                return View(model);
            }
        }

        [Route("girisyap")]
        public IActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        [Route("girisyap")]
        public async Task<IActionResult> GirisYap(string email)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(x => x.Email == email);

            if (kullanici == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta!");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kullanici.Email),
                new Claim(ClaimTypes.Role, kullanici.Rol),
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
            HttpContext.Session.SetString("KullaniciRol", kullanici.Rol);

            return RedirectToAction("Index", "Home");
        }

        [Route("cikisyap")]
        public async Task<IActionResult> CikisYap()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("GirisYap");
        }
    }
}