using System.ComponentModel.DataAnnotations;

namespace MakaleSistemi.Models
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AdSoyad { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Sifre { get; set; }  // Şifreleme ekleyebiliriz.

        [Required]
        public string Rol { get; set; } // "Yazar", "Editör", "Hakem"
    }
}
