using Microsoft.EntityFrameworkCore;
using NabsGroup.Models;

namespace NabsGroup.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option) { }

        public DbSet<Job>Staging_NABSGROUP { get; set; }
    }
  
}
