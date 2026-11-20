using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ButcePlatformu.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthApiController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public record GirisIstegi(string Eposta, string Sifre);

    [HttpPost("giris")]
    public async Task<IActionResult> Giris([FromBody] GirisIstegi istek)
    {
        var kullanici = await _userManager.FindByEmailAsync(istek.Eposta);
        if (kullanici == null) return Unauthorized("E-posta veya şifre hatalı.");

        var sonuc = await _signInManager.CheckPasswordSignInAsync(kullanici, istek.Sifre, false);
        if (!sonuc.Succeeded) return Unauthorized("E-posta veya şifre hatalı.");

        var roller = await _userManager.GetRolesAsync(kullanici);
        return Ok(new
        {
            kullaniciId = kullanici.Id,
            eposta = kullanici.Email,
            rol = roller.FirstOrDefault() ?? "Kullanici"
        });
    }
}