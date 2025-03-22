using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MakaleSistemi.Data;
using MakaleSistemi.Models;

namespace MakaleSistemi.Services
{
    public class LogKayitService
    {
        private readonly ApplicationDbContext _context;

        public LogKayitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogEkle(int makaleId, int kullaniciId, string islem)
        {
            var log = new LogKayit
            {
                MakaleId = makaleId,
                KullaniciId = kullaniciId,
                Islem = islem,
                Tarih = DateTime.Now
            };

            _context.LogKayitlari.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}