using System.ComponentModel.DataAnnotations;

namespace ButcePlatformu.Models;

public class Harcama
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Harcama başlığı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Başlık 2-100 karakter olmalıdır.")]
    [Display(Name = "Harcama / İşletme Adı")]
    public string Baslik { get; set; } = "";

    [Required(ErrorMessage = "Tutar zorunludur.")]
    [Range(0.01, 1000000, ErrorMessage = "Tutar 0,01 ile 1.000.000 TL arasında olmalıdır.")]
    [Display(Name = "Tutar (TL)")]
    public decimal Tutar { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur.")]
    [Display(Name = "Harcama Tarihi")]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Kategori seçiniz.")]
    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }
    public Kategori? Kategori { get; set; }

    public string? KullaniciId { get; set; }
    public string? FisGorselYolu { get; set; }
    public bool ExifTemizlendiMi { get; set; }
}