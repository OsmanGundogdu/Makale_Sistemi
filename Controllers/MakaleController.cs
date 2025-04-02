using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.EntityFrameworkCore;

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
            var editorler = _context.Kullanicilar
                .Where(u => u.Rol == "Editor")
                .Select(u => new EditorViewModel
                {
                    Email = u.Email,
                    AdSoyad = u.AdSoyad
                })
                .ToList();

            ViewBag.Editorler = editorler;
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

                string icerik = "";

                try
                {
                    using (PdfReader pdfReader = new PdfReader(filePath))
                    using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                    {
                        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                        {
                            icerik += PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i)) + "\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Mesaj"] = "PDF içeriği okunamadı: " + ex.Message;
                    return View();
                }

                var yeniMakale = new Makale
                {
                    Baslik = model.Baslik,
                    YazarEmail = model.YazarEmail,
                    DosyaYolu = "/uploads/" + makale.FileName,
                    TakipNumarasi = Guid.NewGuid().ToString(),
                    Durum = "Yükleme Başarılı",
                    YuklemeTarihi = DateTime.Now,
                    Icerik = icerik,
                    Konu = model.Konu
                };

                _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
                _context.Makaleler.Add(yeniMakale);
                _context.SaveChanges();
                _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

                var logKayit = new LogKayit
                {
                    MakaleId = yeniMakale.Id,
                    Islem = $"Makale yüklendi: {yeniMakale.Baslik}",
                    Tarih = DateTime.Now
                };

                _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
                _context.LogKayitlari.Add(logKayit);
                _context.SaveChanges();
                _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

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
            var editorler = _context.Kullanicilar
                .Where(u => u.Rol == "Editor")
                .Select(u => new EditorViewModel
                {
                    Email = u.Email,
                    AdSoyad = u.AdSoyad
                })
                .ToList();

            ViewBag.Editorler = editorler;
            return View();
        }

        [HttpPost]
        [Route("makaledurumsorgulama")]
        public IActionResult MakaleDurumuSorgula(string takipNo, string email)
        {
            var editorler = _context.Kullanicilar
                .Where(u => u.Rol == "Editor")
                .Select(u => new EditorViewModel
                {
                    Email = u.Email,
                    AdSoyad = u.AdSoyad
                })
                .ToList();

            ViewBag.Editorler = editorler;

            var makale = _context.Makaleler
                .FirstOrDefault(m => m.TakipNumarasi == takipNo && m.YazarEmail == email);

            if (makale == null)
            {
                ViewBag.HataMesaji = "Makale bulunamadı!";
                return View("DurumveMesajGonderme");
            }

            // Makale bilgilerini ViewModel üzerinden View'a taşıyoruz
            ViewData["Makale"] = makale;  // ViewData kullanarak makale bilgilerini aktarıyoruz
            ViewBag.TakipNo = takipNo;
            ViewBag.Durum = makale.Durum;

            return View("DurumveMesajGonderme");
        }


        [HttpPost]
        [Route("makale/mesajgonder")]
        public IActionResult MesajGonder(string yazarEmail, string aliciEmail, string icerik, string? takipNo = null)
        {
            if (string.IsNullOrEmpty(yazarEmail) || string.IsNullOrEmpty(aliciEmail) || string.IsNullOrEmpty(icerik))
            {
                TempData["MesajHata"] = "Lütfen tüm alanları doldurun!";
                return RedirectToAction("MakaleTakip");
            }

            var mesaj = new Mesaj
            {
                GonderenEmail = yazarEmail,
                AliciEmail = aliciEmail,
                Icerik = icerik,
                GonderimTarihi = DateTime.Now
            };

            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
            _context.Mesajlar.Add(mesaj);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

            int kullaniciId = _context.Kullanicilar
                .Where(u => u.Email == yazarEmail)
                .Select(u => u.Id)
                .FirstOrDefault();

            int makaleId = _context.Makaleler
                .Where(m => m.TakipNumarasi == takipNo)
                .Select(m => m.Id)
                .FirstOrDefault();

            var logKayit = new LogKayit
            {
                Islem = $"Mesaj gönderildi: {yazarEmail} → {aliciEmail} | İçerik: {icerik.Substring(0, Math.Min(20, icerik.Length))}...",
                KullaniciId = kullaniciId,
                MakaleId = makaleId,
                Tarih = DateTime.Now,

            };

            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
            _context.LogKayitlari.Add(logKayit);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

            TempData["MesajBasari"] = "Mesaj başarıyla gönderildi!";
            return RedirectToAction("MakaleTakip");
        }



    }
}
