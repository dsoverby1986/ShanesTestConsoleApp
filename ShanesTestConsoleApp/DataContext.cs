using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ShanesTestConsoleApp
{
    class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<UserMenu> UserMenus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Database=ShanesTestConsoleApp;Trusted_Connection=True;");
        }
    }
}
