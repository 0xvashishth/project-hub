using Microsoft.EntityFrameworkCore;

namespace projecthub.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions <ProjectContext> options) : base(options)
        {

        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<User> Users => Set<User> ();
        
    }
}
