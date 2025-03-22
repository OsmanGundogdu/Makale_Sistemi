using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakaleSistemi.Models
{
    public class LogKayit
    {
        public int Id { get; set; }
        public int MakaleId { get; set; }
        public int KullaniciId { get; set; }
        public string Islem { get; set; } = string.Empty;
        public DateTime Tarih { get; set; } = DateTime.Now;

        public Makale? Makale { get; set; }
        public Kullanici? Kullanici { get; set; }
    }

}