namespace ButcePlatformu.Models;

public class DenetimKaydi
{
    public int Id { get; set; }
    public string KullaniciEposta { get; set; } = "Sistem";
    public string IslemTuru { get; set; } = "";
    public string Detay { get; set; } = "";
    public DateTime IslemTarihi { get; set; } = DateTime.Now;
}