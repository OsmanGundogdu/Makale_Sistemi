using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakaleSistemi.Models
{
    public class LogKayit
    {
        public int Id { get; set; }
        public string Aciklama { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
    }

}