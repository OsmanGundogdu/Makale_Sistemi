// using MakaleSistemi.Data;
// using Microsoft.AspNetCore.Mvc;

// namespace MakaleSistemi.Controllers
// {
//     public class MakaleController : Controller
//     {
//         private readonly ApplicationDbContext _context;

//         public MakaleController(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         [Route("makalesistemi")]
//         public IActionResult MakaleSistemi()
//         {
//             return View();
//         }

//         [Route("makaledurumsorgulama")]
//         public IActionResult MakaleTakip()
//         {
//             return View();
//         }

//         [HttpPost]
//         [Route("makaledurumsorgulama")]
//         public IActionResult MakaleDurumuSorgula(string takipNo, string email)
//         {
//             Console.WriteLine($"Gelen Takip No: {takipNo}, Email: {email}");

//             var makale = _context.Makaleler
//                 .FirstOrDefault(m => m.TakipNumarasi == takipNo && m.YazarEmail == email);

//             if (makale == null)
//             {
//                 Console.WriteLine("Makale bulunamadı!");
//                 ViewBag.Mesaj = "Makale bulunamadı!";
//                 return View("MakaleTakip");
//             }

//             Console.WriteLine($"Makale bulundu! Durum: {makale.Durum}");

//             ViewBag.TakipNo = takipNo;
//             ViewBag.Durum = makale.Durum;

//             return View("MakaleTakip");
//         }

//     }

// }



using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

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
                // Dosya yolu belirleme
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads"); // WebRoot altındaki uploads klasörüne kaydedilecektir.
                Directory.CreateDirectory(uploadsFolder); // Eğer klasör yoksa oluştur
                var filePath = Path.Combine(uploadsFolder, makale.FileName);

                // Dosyayı kaydetme
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    makale.CopyTo(stream);
                }

                // Makale bilgilerini veritabanına ekleme
                var yeniMakale = new Makale
                {
                    Baslik = model.Baslik,
                    YazarEmail = model.YazarEmail,
                    DosyaYolu = filePath,  // Dosya yolunu kaydet
                    TakipNumarasi = Guid.NewGuid().ToString(),  // Her makale için benzersiz bir takip numarası
                    Durum = "Yükleme Başarılı",  // İlk durum olarak "Yükleme Başarılı" atandı
                    YuklemeTarihi = DateTime.Now  // Yükleme tarihi olarak şu anki zamanı alıyoruz
                };

                // Veritabanına kaydetme
                _context.Makaleler.Add(yeniMakale);
                _context.SaveChanges();

                // Kullanıcıya başarılı bir şekilde kaydedildiğini bildiren mesaj
                TempData["Mesaj"] = "Makale başarıyla yüklendi!";
                return RedirectToAction("MakaleSistemi");
            }
            else
            {
                TempData["Mesaj"] = "Lütfen geçerli bir dosya seçin!";
                return View();
            }
        }

        // Makale Takip Durumu
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
