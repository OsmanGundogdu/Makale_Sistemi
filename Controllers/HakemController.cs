using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Diagnostics;

namespace MakaleSistemi.Controllers
{
    [Authorize(Roles = "Hakem")]
    public class HakemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HakemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("makalesistemi/hakem")]
        public IActionResult Index()
        {
            Debug.WriteLine("Index metodu çalıştı.");

            var hakemId = GetCurrentUserId();

            Debug.WriteLine($"GetCurrentUserId çağrıldı. Hakem ID: {hakemId}");

            var makaleler = _context.MakaleHakemler
                                    .Where(mh => mh.HakemId == hakemId && mh.Durum == "İnceleniyor")
                                    .Select(mh => mh.Makale)
                                    .ToList();

            return View(makaleler);
        }


        [Route("makalesistemi/hakem/makale-detay/{id}")]
        public IActionResult MakaleDetay(int id)
        {
            var makale = _context.Makaleler.FirstOrDefault(m => m.Id == id);
            if (makale == null) return NotFound();

            return View(makale);
        }

        [HttpPost]
        [Route("makalesistemi/hakem/degerlendir")]
        public IActionResult Degerlendir(int makaleId, string yorum, int puan)
        {
            var hakemId = GetCurrentUserId();
            var atama = _context.MakaleHakemler.FirstOrDefault(mh => mh.MakaleId == makaleId && mh.HakemId == hakemId);
            if (atama == null) return NotFound();

            // burası değişmiş olması lazım. default değer değil de inputta girdiğim değer alınmalı.
            if (string.IsNullOrWhiteSpace(yorum))
            {
                yorum = "Hakem yorumu eklenmemiş.";
            }

            var degerlendirme = new MakaleHakem
            {
                // durum düzgün gelmiyor
                Durum = atama.Durum,
                MakaleId = makaleId,
                HakemId = hakemId,
                // yorum düzgün gelmiyor
                Yorum = yorum,
                Puan = puan,
                Tarih = DateTime.Now
            };

            _context.MakaleHakemler.Add(degerlendirme);
            // atama.Durum = "Tamamlandı";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        private int GetCurrentUserId()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return 0;
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }


        [Route("makalesistemi/hakem/atanmis-makaleler")]
        public IActionResult AtanmisMakaleler()
        {
            var hakemId = GetCurrentUserId();

            var degerlendirdigiMakaleler = _context.MakaleHakemler
                .Where(d => d.HakemId == hakemId)
                .Select(d => new
                {
                    Id = d.MakaleId,
                    Baslik = d.Makale.Baslik,
                    Durum = d.Makale.Durum
                })
                .ToList<dynamic>();

            return View(degerlendirdigiMakaleler);
        }

    }
}
