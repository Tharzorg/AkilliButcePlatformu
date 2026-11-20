using ButcePlatformu.Data;
using ButcePlatformu.Models;
using ButcePlatformu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ButcePlatformu.Controllers;

[ApiController]
[Route("api/harcamalar")]
public class HarcamalarApiController : ControllerBase
{
    private readonly UygulamaDbContext _context;
    private readonly ExifTemizlemeServisi _exifServisi;

    public HarcamalarApiController(UygulamaDbContext context, ExifTemizlemeServisi exifServisi)
    {
        _context = context;
        _exifServisi = exifServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var liste = await _context.Harcamalar
            .Include(h => h.Kategori)
            .OrderByDescending(h => h.Tarih)
            .Select(h => new
            {
                h.Id,
                h.Baslik,
                h.Tutar,
                h.Tarih,
                h.KategoriId,
                KategoriAdi = h.Kategori != null ? h.Kategori.Ad : "Diğer",
                h.ExifTemizlendiMi
            })
            .ToListAsync();

        return Ok(liste);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> TekGetir(int id)
    {
        var harcama = await _context.Harcamalar.FindAsync(id);
        return harcama == null ? NotFound() : Ok(harcama);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] Harcama yeniHarcama)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _context.Harcamalar.Add(yeniHarcama);
        _context.DenetimKayitlari.Add(new DenetimKaydi
        {
            KullaniciEposta = yeniHarcama.KullaniciId ?? "MobilAPI",
            IslemTuru = "API Harcama Ekleme",
            Detay = $"{yeniHarcama.Baslik} - {yeniHarcama.Tutar:N2} TL"
        });
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(TekGetir), new { id = yeniHarcama.Id }, yeniHarcama);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Guncelle(int id, [FromBody] Harcama guncel)
    {
        var mevcut = await _context.Harcamalar.FindAsync(id);
        if (mevcut == null) return NotFound();

        mevcut.Baslik = guncel.Baslik;
        mevcut.Tutar = guncel.Tutar;
        mevcut.Tarih = guncel.Tarih;
        mevcut.KategoriId = guncel.KategoriId;

        await _context.SaveChangesAsync();
        return Ok(mevcut);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Sil(int id)
    {
        var mevcut = await _context.Harcamalar.FindAsync(id);
        if (mevcut == null) return NotFound();

        _context.Harcamalar.Remove(mevcut);
        _context.DenetimKayitlari.Add(new DenetimKaydi
        {
            KullaniciEposta = "MobilAPI",
            IslemTuru = "API Harcama Silme",
            Detay = $"Silinen kayıt: {mevcut.Baslik}"
        });
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("fis-temizle")]
    public async Task<IActionResult> FisExifTemizle(IFormFile dosya)
    {
        if (dosya == null || dosya.Length == 0)
            return BadRequest("Lütfen geçerli bir fiş görseli yükleyin.");

        var sonuc = await _exifServisi.TemizleVeKaydetAsync(dosya, "MobilKullanici");
        return Ok(new
        {
            dosyaYolu = sonuc.DosyaYolu,
            exifTemizlendi = true,
            exifTespitEdildiMi = sonuc.ExifVardi,
            mesaj = sonuc.Ozet
        });
    }
}