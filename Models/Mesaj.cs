using System;

namespace MakaleSistemi.Models
{
    public class Mesaj
    {
        public int Id { get; set; }
        public string GonderenEmail { get; set; } = string.Empty;
        public string AliciEmail { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public DateTime GonderimTarihi { get; set; } = DateTime.Now;
    }
}
