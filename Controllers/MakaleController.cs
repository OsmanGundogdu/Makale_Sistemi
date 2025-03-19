using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace MakaleSistemi.Controllers
{
    public class MakaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MakaleController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Route("makalesistemi")]
        public IActionResult MakaleSistemi()
        {
            return View();
        }

        [HttpPost]
        [Route("makalesistemi")]
        public IActionResult MakaleSistemi(Makale model, Microsoft.AspNetCore.Http.IFormFile makale)
        {
            if (makale != null && makale.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, makale.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    makale.CopyTo(stream);
                }

                var yeniMakale = new Makale
                {
                    Baslik = model.Baslik,
                    YazarEmail = model.YazarEmail,
                    DosyaYolu = "/uploads/" + makale.FileName,
                    TakipNumarasi = Guid.NewGuid().ToString(),
                    Durum = "Yükleme Başarılı",
                    YuklemeTarihi = DateTime.Now
                };

                _context.Makaleler.Add(yeniMakale);
                _context.SaveChanges();

                TempData["BasariMesaji"] = "Makale başarıyla yüklendi!";
                return RedirectToAction("MakaleSistemi");
            }
            else
            {
                TempData["Mesaj"] = "Makale yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";
                return View();
            }
        }

        [Route("makaledurumsorgulama")]
        public IActionResult MakaleTakip()
        {
            return View();
        }

        [HttpPost]
        [Route("makaledurumsorgulama")]
        public IActionResult MakaleDurumuSorgula(string takipNo, string email)
        {
            var makale = _context.Makaleler
                .FirstOrDefault(m => m.TakipNumarasi == takipNo && m.YazarEmail == email);

            if (makale == null)
            {
                ViewBag.Mesaj = "Makale bulunamadı!";
                return View("MakaleTakip");
            }

            ViewBag.TakipNo = takipNo;
            ViewBag.Durum = makale.Durum;

            return View("MakaleTakip");
        }
    }
}
