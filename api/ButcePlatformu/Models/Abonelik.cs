using System.ComponentModel.DataAnnotations;

namespace ButcePlatformu.Models;

public class Abonelik
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    [StringLength(80)]
    [Display(Name = "Platform / Hizmet")]
    public string HizmetAdi { get; set; } = "";

    [Required(ErrorMessage = "Aylık tutar zorunludur.")]
    [Range(1, 50000, ErrorMessage = "Tutar 1 ile 50.000 TL arasında olmalıdır.")]
    [Display(Name = "Aylık Ücret (TL)")]
    public decimal AylikTutar { get; set; }

    [Required(ErrorMessage = "Ödeme günü zorunludur.")]
    [Range(1, 31, ErrorMessage = "Ödeme günü 1 ile 31 arasında olmalıdır.")]
    [Display(Name = "Her Ayın Kaçında?")]
    public int OdemeGunu { get; set; } = 1;

    public bool AktifMi { get; set; } = true;

    [Required(ErrorMessage = "Kategori seçiniz.")]
    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }
    public Kategori? Kategori { get; set; }

    public string? KullaniciId { get; set; }

    public int KalanGunHesapla()
    {
        var bugun = DateTime.Today;
        int gun = Math.Min(OdemeGunu, DateTime.DaysInMonth(bugun.Year, bugun.Month));
        var buAyOdeme = new DateTime(bugun.Year, bugun.Month, gun);
        if (buAyOdeme < bugun)
            buAyOdeme = buAyOdeme.AddMonths(1);
        return (buAyOdeme - bugun).Days;
    }
}