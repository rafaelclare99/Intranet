using IntraNet.Data;
using IntraNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProcessosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ProcessosController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    // 📌 LISTAGEM
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        IQueryable<Processo> query = _context.Processos;

        // 🔴 Admin vê tudo
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(p => p.Setor == role);
        }

        var processos = await query
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();

        return View(processos);
    }

    // ➕ CRIAR (GET)
    [Authorize(Roles = "Admin")]
    public IActionResult Criar()
    {
        return View("Criar");
    }

    // ➕ CRIAR (POST)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Processo processo, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            ModelState.AddModelError("", "Arquivo obrigatório");

        if (!ModelState.IsValid)
            return View("Criar", processo);

        var pasta = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(pasta);

        var nomeArquivo = $"{Guid.NewGuid()}_{Path.GetFileName(arquivo.FileName)}";
        var caminho = Path.Combine(pasta, nomeArquivo);

        using (var stream = new FileStream(caminho, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        processo.ArquivoPath = "/uploads/" + nomeArquivo;
        processo.DataCriacao = DateTime.Now;

        _context.Processos.Add(processo);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ✏️ EDITAR (GET)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Editar(int id)
    {
        var processo = await _context.Processos.FindAsync(id);
        if (processo == null)
            return NotFound();

        return View(processo);
    }

    // ❌ EXCLUIR
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var processo = await _context.Processos.FindAsync(id);
        if (processo == null)
            return NotFound();

        // 🗑️ Remove arquivo físico
        if (!string.IsNullOrEmpty(processo.ArquivoPath))
        {
            var caminho = Path.Combine(
                _env.WebRootPath,
                processo.ArquivoPath.TrimStart('/'));

            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);
        }

        _context.Processos.Remove(processo);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
