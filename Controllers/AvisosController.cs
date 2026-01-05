using IntraNet.Data;
using IntraNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AvisosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AvisosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Todos veem (geral + setor do usuário)
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var role = (await _userManager.GetRolesAsync(user!)).FirstOrDefault();

        var avisos = await _context.Avisos
            .Where(a => a.Setor == null || a.Setor == role)
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();

        return View(avisos);
    }

    // Apenas Admin cria
    [Authorize(Roles = "Admin")]
    public IActionResult Criar()
    {
        return View("CriarAvisos");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar(Avisos aviso)
    {
        if (!ModelState.IsValid)
            return View(aviso);

        aviso.AutorId = _userManager.GetUserId(User)!;
        aviso.DataCriacao = DateTime.Now;

        _context.Avisos.Add(aviso);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
