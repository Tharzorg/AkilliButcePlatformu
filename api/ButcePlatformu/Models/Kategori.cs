using System.ComponentModel.DataAnnotations;

namespace ButcePlatformu.Models;

public class Kategori
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
    [Display(Name = "Kategori Adı")]
    public string Ad { get; set; } = "";

    [Display(Name = "İkon Kodu")]
    public string Ikon { get; set; } = "receipt";

    public List<Harcama> Harcamalar { get; set; } = new();
    public List<Abonelik> Abonelikler { get; set; } = new();
}