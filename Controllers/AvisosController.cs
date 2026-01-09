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
    private readonly UserManager<ApplicationUser> _userManager;

    public AvisosController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // 📌 LISTAGEM
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        // Segurança extra
        if (user == null)
            return Challenge();

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        IQueryable<Avisos> query = _context.Avisos
            .Include(a => a.Autor);

        // 🔴 Admin vê tudo
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(a => a.Setor == null || a.Setor == role);
        }

        var avisos = await query
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();

        return View(avisos);
    }

    // ➕ CRIAR (GET)
    [Authorize(Roles = "Admin")]
    public IActionResult Criar()
    {
        return View("CriarAvisos");
    }

    // ➕ CRIAR (POST)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Avisos aviso)
    {
        if (!ModelState.IsValid)
            return View("CriarAvisos", aviso);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        aviso.AutorId = user.Id;
        aviso.DataCriacao = DateTime.Now;

        _context.Avisos.Add(aviso);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ✏️ EDITAR (GET)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Editar(int id)
    {
        var aviso = await _context.Avisos.FindAsync(id);
        if (aviso == null)
            return NotFound();

        return View(aviso);
    }

    // ✏️ EDITAR (POST)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Avisos aviso)
    {
        if (!ModelState.IsValid)
            return View(aviso);

        var avisoDb = await _context.Avisos.FindAsync(aviso.AvisosId);
        if (avisoDb == null)
            return NotFound();

        avisoDb.Titulo = aviso.Titulo;
        avisoDb.Mensagem = aviso.Mensagem;
        avisoDb.Setor = aviso.Setor;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ❌ EXCLUIR
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var aviso = await _context.Avisos.FindAsync(id);
        if (aviso == null)
            return NotFound();

        _context.Avisos.Remove(aviso);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
