using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MakaleSistemi.Controllers
{
    [Authorize(Roles = "Editor")]
    public class EditorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EditorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("makalesistemi/editor")]
        public IActionResult Index()
        {
            var makaleler = _context.Makaleler.ToList();
            return View(makaleler);
        }

        [Route("makalesistemi/editor/makale-detay/{id}")]
        public IActionResult MakaleDetay(int id)
        {
            var makale = _context.Makaleler.FirstOrDefault(m => m.Id == id);
            if (makale == null)
            {
                return NotFound();
            }
            return View(makale);
        }

        [HttpPost]
        [Route("makalesistemi/editor/anonimlestir")]
        public IActionResult Anonimlestir(int id)
        {
            var makale = _context.Makaleler.FirstOrDefault(m => m.Id == id);
            if (makale == null) return NotFound();

            // Burada makale anonimleştirme işlemi yapılır (örneğin isimler vs. kaldırılır)
            makale.Icerik = AnonimlestirMetin(makale.Icerik);

            _context.SaveChanges();
            TempData["BasariMesaji"] = "Makale anonimleştirildi.";
            return RedirectToAction("Index");
        }

        [Route("makalesistemi/editor/loglar")]
        public IActionResult Loglar()
        {
            var loglar = _context.LogKayitlari.OrderByDescending(l => l.Tarih).ToList();
            return View(loglar);
        }

        private string AnonimlestirMetin(string icerik)
        {
            // Örneğin, yazar isimlerini ve kurum bilgilerini anonim hale getirme işlemi
            return icerik.Replace("Dr.", "[Anonim]").Replace("Üniversitesi", "[Kurum]");
        }

        [Route("makalesistemi/editor/hakem-ata/{id}")]
        public IActionResult HakemAta(int id)
        {
            var makale = _context.Makaleler.FirstOrDefault(m => m.Id == id);
            if (makale == null)
            {
                return NotFound();
            }

            var hakemler = _context.Kullanicilar
                .Where(u => u.Rol == "Hakem")
                .Select(h => new SelectListItem { Value = h.Id.ToString(), Text = h.AdSoyad })
                .ToList();

            ViewBag.Hakemler = hakemler.Count > 0 ? new SelectList(hakemler, "Value", "Text") : null;

            return View(makale);
        }

        [HttpPost]
        [Route("makalesistemi/editor/hakem-ata")]
        public IActionResult HakemAtaIslem(int makaleId, int hakemId)
        {
            if (!_context.Kullanicilar.Any(u => u.Id == hakemId && u.Rol == "Hakem"))
            {
                TempData["HataMesaji"] = "Seçilen hakem sistemde bulunamadı.";
                return RedirectToAction("HakemAta", new { id = makaleId });
            }

            var atama = new MakaleHakem
            {
                MakaleId = makaleId,
                HakemId = hakemId,
                Durum = "Hakeme Atandı",
                Tarih = DateTime.Now
            };

            _context.MakaleHakemler.Add(atama);
            _context.SaveChanges();

            var hakem = _context.Kullanicilar.FirstOrDefault(u => u.Id == hakemId);
            if (hakem == null)
            {
                TempData["HataMesaji"] = "Hakem bulunamadı.";
                return RedirectToAction("HakemAta", new { id = makaleId });
            }

            var logKayit = new LogKayit
            {
                Islem = $"Makale ID: {makaleId} hakeme atandı. Hakem: {hakem.AdSoyad} ({hakem.Email}), Durum: İnceleniyor",
                Tarih = DateTime.Now
            };

            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
            _context.LogKayitlari.Add(logKayit);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

            TempData["BasariMesaji"] = "Makale başarıyla hakeme atandı.";
            return RedirectToAction("Index");
        }




    }
}
