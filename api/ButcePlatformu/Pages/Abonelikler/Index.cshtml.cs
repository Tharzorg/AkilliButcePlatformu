using ButcePlatformu.Data;
using ButcePlatformu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ButcePlatformu.Pages.Abonelikler;

[Authorize]
public class IndexModel : PageModel
{
    private readonly UygulamaDbContext _context;

    public IndexModel(UygulamaDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Abonelik YeniAbonelik { get; set; } = new();

    public SelectList KategoriListesi { get; set; } = default!;
    public List<Abonelik> Abonelikler { get; set; } = new();

    public async Task OnGetAsync()
    {
        KategoriListesi = new SelectList(await _context.Kategoriler.ToListAsync(), "Id", "Ad");
        Abonelikler = await _context.Abonelikler
            .Include(a => a.Kategori)
            .OrderBy(a => a.OdemeGunu)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEkleAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        YeniAbonelik.KullaniciId = User.Identity?.Name;
        _context.Abonelikler.Add(YeniAbonelik);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}