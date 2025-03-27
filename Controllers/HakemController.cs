using MakaleSistemi.Data;
using MakaleSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

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
            var hakemId = GetCurrentUserId();

            var makaleler = _context.MakaleHakemler
                                    .Where(mh => mh.HakemId == hakemId && mh.Durum == "Hakeme Atandı")
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
        public IActionResult Degerlendir(int makaleId, string yorum, string durum)
        {
            var hakemId = GetCurrentUserId();
            var atama = _context.MakaleHakemler.FirstOrDefault(mh => mh.MakaleId == makaleId && mh.HakemId == hakemId);
            if (atama == null) return NotFound();

            if (string.IsNullOrWhiteSpace(yorum))
            {
                yorum = "Hakem yorumu eklenmemiş.";
            }

            var degerlendirme = new MakaleHakem
            {
                Durum = durum,
                MakaleId = makaleId,
                HakemId = hakemId,
                Yorum = yorum,
                Tarih = DateTime.Now
            };

            _context.MakaleHakemler.Add(degerlendirme);
            var makale = _context.Makaleler.FirstOrDefaultAsync(m => m.Id == makaleId).Result;
            if (makale != null)
            {
                makale.Durum = durum;
            }
            _context.SaveChanges();

            var logKayit = new LogKayit
            {
                Islem = $"Makale ID: {makaleId} hakem değerlendirilmesi yapıldı. Hakem: {hakemId}, Durum: {durum}",
                Tarih = DateTime.Now
            };

            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
            _context.LogKayitlari.Add(logKayit);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

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


        [Route("makalesistemi/hakem/degerlendirdigim-makaleler")]
        public IActionResult DegerlendirdigimMakaleler()
        {
            var hakemId = GetCurrentUserId();

            var degerlendirdigiMakaleler = _context.MakaleHakemler
                .Where(d => d.HakemId == hakemId && d.Durum != "Hakeme Atandı")
                .Select(d => new
                {
                    Id = d.MakaleId,
                    Baslik = d.Makale.Baslik,
                    Durum = d.Durum
                })
                .ToList<dynamic>();

            return View(degerlendirdigiMakaleler);
        }

    }
}
