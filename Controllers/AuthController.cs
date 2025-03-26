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

                model.Sifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre);

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
        public async Task<IActionResult> GirisYap(string email, string sifre)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(x => x.Email == email);

            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(sifre, kullanici.Sifre))
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre!");
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

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

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
