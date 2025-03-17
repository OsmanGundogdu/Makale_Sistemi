using Microsoft.AspNetCore.Mvc;
using MakaleSistemi.Data;
using MakaleSistemi.Models;
using System.Linq;

namespace MakaleSistemi.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KayitOl(Kullanici model)
        {
            if (ModelState.IsValid)
            {
                // Aynı email ile kayıtlı kullanıcı var mı kontrol et
                if (_context.Kullanicilar.Any(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", "Bu e-posta zaten kayıtlı!");
                    return View();
                }

                model.Sifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre); // Şifreyi hashle
                _context.Kullanicilar.Add(model);
                _context.SaveChanges();

                return RedirectToAction("GirisYap");
            }
            return View();
        }

        public IActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GirisYap(string email, string sifre)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(x => x.Email == email);

            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(sifre, kullanici.Sifre))
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre!");
                return View();
            }

            HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
            HttpContext.Session.SetString("KullaniciRol", kullanici.Rol);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CikisYap()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("GirisYap");
        }
    }
}
