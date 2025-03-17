using System;
using System.ComponentModel.DataAnnotations;

namespace MakaleSistemi.Models
{
    public class Makale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Baslik { get; set; }

        [Required]
        public string YazarEmail { get; set; }

        public string DosyaYolu { get; set; }

        public string TakipNumarasi { get; set; }

        public string Durum { get; set; } = "Beklemede";

        public DateTime YuklemeTarihi { get; set; } = DateTime.Now;
    }
}
