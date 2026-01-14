using IntraNet.Data;
using IntraNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntraNet.Controllers
{
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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            IQueryable<Avisos> query = _context.Avisos;

            // Admin vê tudo
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(a =>
                    a.Setor == null || a.Setor == user.Setor);
            }

            var avisos = await query
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();

            return View(avisos);
        }

        // =======================
        // CRIAR
        // =======================
        [Authorize(Roles = "Admin")]
        public IActionResult Criar()
        {
            return View();
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

        // =======================
        // EDITAR
        // =======================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(int id)
        {
            var aviso = await _context.Avisos.FindAsync(id);
            if (aviso == null)
                return NotFound();

            return View(aviso);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(Avisos aviso)
        {
            if (!ModelState.IsValid)
                return View(aviso);

            _context.Avisos.Update(aviso);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =======================
        // EXCLUIR
        // =======================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Excluir(int id)
        {
            var aviso = await _context.Avisos.FindAsync(id);
            if (aviso != null)
            {
                _context.Avisos.Remove(aviso);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
