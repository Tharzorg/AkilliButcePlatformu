using ButcePlatformu.Data;
using ButcePlatformu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ButcePlatformu.Pages.Yonetim;

[Authorize(Roles = "Admin")]
public class DenetimIzleriModel : PageModel
{
    private readonly UygulamaDbContext _context;

    public DenetimIzleriModel(UygulamaDbContext context)
    {
        _context = context;
    }

    public List<DenetimKaydi> Kayitlar { get; set; } = new();

    public async Task OnGetAsync()
    {
        Kayitlar = await _context.DenetimKayitlari
            .OrderByDescending(d => d.IslemTarihi)
            .ToListAsync();
    }
}