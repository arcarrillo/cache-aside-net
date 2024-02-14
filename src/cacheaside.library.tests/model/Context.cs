using Microsoft.EntityFrameworkCore;

namespace cacheaside.library.tests.model
{
    public class Context : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=sample;Uid=root;Pwd=secretpassword123;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
