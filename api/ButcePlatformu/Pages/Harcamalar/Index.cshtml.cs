using ButcePlatformu.Data;
using ButcePlatformu.Models;
using ButcePlatformu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ButcePlatformu.Pages.Harcamalar;

[Authorize]
public class IndexModel : PageModel
{
    private readonly UygulamaDbContext _context;
    private readonly ExifTemizlemeServisi _exifServisi;

    public IndexModel(UygulamaDbContext context, ExifTemizlemeServisi exifServisi)
    {
        _context = context;
        _exifServisi = exifServisi;
    }

    [BindProperty]
    public Harcama YeniHarcama { get; set; } = new();

    [BindProperty]
    public IFormFile? FisDosyasi { get; set; }

    public SelectList KategoriListesi { get; set; } = default!;
    public List<Harcama> Harcamalar { get; set; } = new();

    [TempData]
    public string? BilgiMesaji { get; set; }

    public async Task OnGetAsync()
    {
        KategoriListesi = new SelectList(await _context.Kategoriler.ToListAsync(), "Id", "Ad");
        Harcamalar = await _context.Harcamalar
            .Include(h => h.Kategori)
            .OrderByDescending(h => h.Tarih)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEkleAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        string kullanici = User.Identity?.Name ?? "WebKullanici";
        YeniHarcama.KullaniciId = kullanici;

        if (FisDosyasi != null && FisDosyasi.Length > 0)
        {
            var sonuc = await _exifServisi.TemizleVeKaydetAsync(FisDosyasi, kullanici);
            YeniHarcama.FisGorselYolu = sonuc.DosyaYolu;
            YeniHarcama.ExifTemizlendiMi = true;
            BilgiMesaji = $"Harcama kaydedildi. {sonuc.Ozet}";
        }
        else
        {
            BilgiMesaji = "Harcama başarıyla kaydedildi.";
        }

        _context.Harcamalar.Add(YeniHarcama);
        _context.DenetimKayitlari.Add(new DenetimKaydi
        {
            KullaniciEposta = kullanici,
            IslemTuru = "Web Harcama Ekleme",
            Detay = $"{YeniHarcama.Baslik} - {YeniHarcama.Tutar:N2} TL"
        });
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSilAsync(int id)
    {
        var kayit = await _context.Harcamalar.FindAsync(id);
        if (kayit != null)
        {
            _context.Harcamalar.Remove(kayit);
            _context.DenetimKayitlari.Add(new DenetimKaydi
            {
                KullaniciEposta = User.Identity?.Name ?? "WebKullanici",
                IslemTuru = "Web Harcama Silme",
                Detay = $"Silinen: {kayit.Baslik}"
            });
            await _context.SaveChangesAsync();
            BilgiMesaji = "Harcama kaydı silindi.";
        }
        return RedirectToPage();
    }
}