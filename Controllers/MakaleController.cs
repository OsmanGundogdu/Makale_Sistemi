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

        // [HttpPost]
        // [Route("makalesistemi")]
        // public IActionResult MakaleSistemi(Makale model, Microsoft.AspNetCore.Http.IFormFile makale)
        // {
        //     if (makale != null && makale.Length > 0)
        //     {
        //         var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        //         Directory.CreateDirectory(uploadsFolder);
        //         var filePath = Path.Combine(uploadsFolder, makale.FileName);

        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             makale.CopyTo(stream);
        //         }

        //         var yeniMakale = new Makale
        //         {
        //             Baslik = model.Baslik,
        //             YazarEmail = model.YazarEmail,
        //             DosyaYolu = "/uploads/" + makale.FileName,
        //             TakipNumarasi = Guid.NewGuid().ToString(),
        //             Durum = "Yükleme Başarılı",
        //             YuklemeTarihi = DateTime.Now
        //         };

        //         _context.Makaleler.Add(yeniMakale);
        //         _context.SaveChanges();

        //         TempData["BasariMesaji"] = "Makale başarıyla yüklendi!";
        //         return RedirectToAction("MakaleSistemi");
        //     }
        //     else
        //     {
        //         TempData["Mesaj"] = "Makale yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";
        //         return View();
        //     }
        // }

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
                    Icerik = icerik
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
                ViewBag.Mesaj = "Makale bulunamadı!";
                return View("MakaleTakip");
            }

            ViewBag.TakipNo = takipNo;
            ViewBag.Durum = makale.Durum;

            return View("MakaleTakip");
        }

        [HttpPost]
        [Route("makale/mesajgonder")]
        public IActionResult MesajGonder(string yazarEmail, string aliciEmail, string icerik)
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

            _context.Mesajlar.Add(mesaj);
            _context.SaveChanges();

            TempData["MesajBasari"] = "Mesaj başarıyla gönderildi!";
            return RedirectToAction("MakaleTakip");
        }


    }
}
