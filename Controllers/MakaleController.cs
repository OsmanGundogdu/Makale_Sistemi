using Microsoft.AspNetCore.Mvc;

namespace MakaleSistemi.Controllers
{
    public class MakaleController : Controller
    {
        public IActionResult Yukle()
        {
            return View();
        }

        public IActionResult MakaleTakip()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MakaleDurumuSorgula(string takipNo, string email)
        {
            // Burada makale veritabanından sorgulanacak (şu an mock veri)
            var durum = "İnceleme Aşamasında"; // Gerçek veritabanı bağlantısı eklenmeli

            ViewBag.TakipNo = takipNo;
            ViewBag.Durum = durum;

            return View("MakaleTakip");
        }
    }
}
