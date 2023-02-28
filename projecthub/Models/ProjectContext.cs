using Microsoft.EntityFrameworkCore;

namespace projecthub.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions <ProjectContext> options) : base(options)
        {

        }

        public DbSet<Projects> Projects => Set<Projects>();
        public DbSet<User> Users => Set<User> ();
        
    }
}
