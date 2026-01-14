using IntraNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AgenteController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AgenteController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // LISTAGEM
    public IActionResult Index()
    {
        var agentes = _userManager.Users.ToList();
        return View(agentes);
    }

    // ====== CRIAR ======
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ApplicationUser model, string senha, bool isAdmin)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Nome = model.Nome,
            Setor = model.Setor,
            Ativo = model.Ativo,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, senha);

        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(model);
        }

        // 🔑 ROLE
        if (isAdmin)
            await _userManager.AddToRoleAsync(user, "Admin");
        else
            await _userManager.AddToRoleAsync(user, "Agente");

        return RedirectToAction(nameof(Index));
    }



    // ====== EDITAR ======
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ApplicationUser model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.Nome = model.Nome;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.Setor = model.Setor;
        user.Ativo = model.Ativo;

        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    // ====== EXCLUIR ======
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
            await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }
}
