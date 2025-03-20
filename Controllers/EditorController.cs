using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

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

        private string PdfDosyasindanMetinOku(Stream pdfStream)
        {
            using (var reader = new PdfReader(pdfStream))
            using (var pdfDocument = new PdfDocument(reader))
            {
                string text = "";
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    text += PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i));
                }
                return text;
            }
        }
    }
}
