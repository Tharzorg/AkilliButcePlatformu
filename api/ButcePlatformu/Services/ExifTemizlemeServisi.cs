using ButcePlatformu.Data;
using ButcePlatformu.Models;
using SixLabors.ImageSharp;

namespace ButcePlatformu.Services;

public class ExifTemizlemeServisi
{
    private readonly UygulamaDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ExifTemizlemeServisi(UygulamaDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<(string DosyaYolu, bool ExifVardi, string Ozet)> TemizleVeKaydetAsync(
        IFormFile dosya, string kullaniciEposta)
    {
        string yuklemeKlasoru = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(yuklemeKlasoru);

        string yeniAd = $"{Guid.NewGuid()}.jpg";
        string tamYol = Path.Combine(yuklemeKlasoru, yeniAd);

        bool exifBulundu = false;
        using (var stream = dosya.OpenReadStream())
        using (var image = await Image.LoadAsync(stream))
        {
            if (image.Metadata.ExifProfile != null && image.Metadata.ExifProfile.Values.Any())
            {
                exifBulundu = true;
                image.Metadata.ExifProfile = null;
            }
            image.Metadata.XmpProfile = null;
            image.Metadata.IptcProfile = null;

            await image.SaveAsJpegAsync(tamYol);
        }

        string ozet = exifBulundu
            ? "Görseldeki EXIF/GPS meta verileri tespit edildi ve temizlendi."
            : "Görsel temiz olarak yeniden kodlandı (EXIF bulunmadı).";

        _context.DenetimKayitlari.Add(new DenetimKaydi
        {
            KullaniciEposta = kullaniciEposta,
            IslemTuru = "EXIF Gizlilik Temizliği",
            Detay = $"{dosya.FileName} -> {yeniAd} | {ozet}"
        });
        await _context.SaveChangesAsync();

        return ($"/uploads/{yeniAd}", exifBulundu, ozet);
    }
}