using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Quando possui mais de um context na app, é bom informar explicitamente qual vc quer referenciar
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }                            
    }
}