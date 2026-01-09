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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            IQueryable<Processo> query = _context.Processos;

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(p => p.Setor == user.Setor);
            }

            var processos = await query
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return View(processos);
        }

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
    }
}
