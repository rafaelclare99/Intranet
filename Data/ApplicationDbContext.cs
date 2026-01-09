using IntraNet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntraNet.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Avisos> Avisos { get; set; }
        public DbSet<Processo> Processos { get; set; }
        public DbSet<ChatMensagem> ChatMensagens { get; set; }
    }
}
