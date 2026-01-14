using IntraNet.Data;
using IntraNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntraNet.Controllers
{
    [Authorize]
    public class ProcessosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProcessosController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= LISTAGEM =================
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            IQueryable<Processo> query = _context.Processos;

            // 🔒 Agente só vê do próprio setor
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(p => p.Setor == user.Setor);
            }

            var processos = await query
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return View(processos);
        }

        // ================= CRIAR =================
        [Authorize(Roles = "Admin")]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Criar(Processo processo)
        {
            if (!ModelState.IsValid)
                return View(processo);

            var user = await _userManager.GetUserAsync(User);

            processo.AutorId = user!.Id;
            processo.DataCriacao = DateTime.Now;

            _context.Processos.Add(processo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= EDITAR =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(int id)
        {
            var processo = await _context.Processos.FindAsync(id);
            if (processo == null)
                return NotFound();

            return View(processo);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(Processo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var processo = await _context.Processos.FindAsync(model.ProcessoId);
            if (processo == null)
                return NotFound();

            processo.Titulo = model.Titulo;
            processo.Descricao = model.Descricao;
            processo.Setor = model.Setor;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= EXCLUIR =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Excluir(int id)
        {
            var processo = await _context.Processos.FindAsync(id);
            if (processo != null)
            {
                _context.Processos.Remove(processo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
