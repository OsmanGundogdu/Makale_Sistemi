using Microsoft.EntityFrameworkCore;
using MakaleSistemi.Models;

namespace MakaleSistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Makale> Makaleler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<LogKayit> LogKayitlari { get; set; }
        public DbSet<MakaleHakem> MakaleHakemler { get; set; }

    }
}
