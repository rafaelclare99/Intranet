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
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ProcessosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var role = (await _userManager.GetRolesAsync(user!)).FirstOrDefault();

        IQueryable<Processo> query = _context.Processos;

        if (!User.IsInRole("Admin"))
        {
            query = query.Where(p => p.Setor == role);
        }

        var processos = await query
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();

        return View(processos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar(Processo processo, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            ModelState.AddModelError("", "Arquivo obrigatório");

        if (!ModelState.IsValid)
            return View(processo);

        var pasta = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(pasta);

        var nomeArquivo = $"{Guid.NewGuid()}_{arquivo.FileName}";
        var caminho = Path.Combine(pasta, nomeArquivo);

        using var stream = new FileStream(caminho, FileMode.Create);
        await arquivo.CopyToAsync(stream);

        processo.ArquivoPath = "/uploads/" + nomeArquivo;
        processo.DataCriacao = DateTime.Now;

        _context.Processos.Add(processo);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Editar(int id)
    {
        var processo = await _context.Processos.FindAsync(id);
        if (processo == null)
            return NotFound();

        return View(processo);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var processo = await _context.Processos.FindAsync(id);
        if (processo == null)
            return NotFound();

        // apaga arquivo físico
        if (!string.IsNullOrEmpty(processo.ArquivoPath))
        {
            var caminho = Path.Combine(_env.WebRootPath, processo.ArquivoPath.TrimStart('/'));
            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);
        }

        _context.Processos.Remove(processo);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


}
