using ButcePlatformu.Data;
using ButcePlatformu.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ButcePlatformu.Pages;

public class IndexModel : PageModel
{
    private readonly UygulamaDbContext _context;

    public IndexModel(UygulamaDbContext context)
    {
        _context = context;
    }

    public decimal ToplamHarcama { get; set; }
    public decimal AylikAbonelikToplami { get; set; }
    public List<Abonelik> YaklasanAbonelikler { get; set; } = new();
    public List<Harcama> SonHarcamalar { get; set; } = new();

    public async Task OnGetAsync()
    {
        var harcamalar = await _context.Harcamalar
            .Include(h => h.Kategori)
            .OrderByDescending(h => h.Tarih)
            .ToListAsync();

        ToplamHarcama = harcamalar.Sum(h => h.Tutar);
        SonHarcamalar = harcamalar.Take(5).ToList();

        var abonelikler = await _context.Abonelikler
            .Where(a => a.AktifMi)
            .ToListAsync();

        AylikAbonelikToplami = abonelikler.Sum(a => a.AylikTutar);
        YaklasanAbonelikler = abonelikler
            .Where(a => a.KalanGunHesapla() <= 3)
            .OrderBy(a => a.KalanGunHesapla())
            .ToList();
    }
}