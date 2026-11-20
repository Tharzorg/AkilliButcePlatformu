using ButcePlatformu.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ButcePlatformu.Data;

public class UygulamaDbContext : IdentityDbContext
{
    public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Kategori> Kategoriler { get; set; } = default!;
    public DbSet<Harcama> Harcamalar { get; set; } = default!;
    public DbSet<Abonelik> Abonelikler { get; set; } = default!;
    public DbSet<DenetimKaydi> DenetimKayitlari { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Harcama>().Property(h => h.Tutar).HasPrecision(18, 2);
        builder.Entity<Abonelik>().Property(a => a.AylikTutar).HasPrecision(18, 2);

        builder.Entity<Kategori>().HasData(
            new Kategori { Id = 1, Ad = "Market & Gıda", Ikon = "shopping_cart" },
            new Kategori { Id = 2, Ad = "Fatura & Kira", Ikon = "receipt_long" },
            new Kategori { Id = 3, Ad = "Ulaşım", Ikon = "directions_bus" },
            new Kategori { Id = 4, Ad = "Dijital Abonelik", Ikon = "subscriptions" },
            new Kategori { Id = 5, Ad = "Eğitim & Teknoloji", Ikon = "laptop" }
        );
    }
}