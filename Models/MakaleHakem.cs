using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakaleSistemi.Models
{
    public class MakaleHakem
    {
        public int Id { get; set; }
        public int MakaleId { get; set; }
        public int HakemId { get; set; }
        public string Yorum { get; set; } = string.Empty;
        public int Puan { get; set; }
        public DateTime Tarih { get; set; }
        public string Durum { get; set; } = string.Empty;
        public Makale Makale { get; set; } = null!;
    }

}