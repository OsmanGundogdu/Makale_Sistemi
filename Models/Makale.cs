using System;
using System.ComponentModel.DataAnnotations;

namespace MakaleSistemi.Models
{
    public class Makale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public string YazarEmail { get; set; } = string.Empty;

        public string DosyaYolu { get; set; } = string.Empty;

        public string TakipNumarasi { get; set; } = string.Empty;

        public string Durum { get; set; } = "Yükleme Başarılı";

        public DateTime YuklemeTarihi { get; set; } = DateTime.Now;

        public string Icerik { get; set; } = string.Empty;

        public string Konu { get; set; } = string.Empty;
    }
}
