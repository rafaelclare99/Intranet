using Microsoft.AspNetCore.Identity;

namespace IntraNet.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = "";
        public string Setor { get; set; } = "";
        public bool Ativo { get; set; } = true;
    }
}
