using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IntraNet.Models
{
    public class Avisos
    {
        [Key]
        public int AvisosId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public string? Setor { get; set; }

        // 🔑 FK para Identity
        [Required]
        public string AutorId { get; set; }

        // 🔗 Navegação
        public ApplicationUser Autor { get; set; }
    }
}
