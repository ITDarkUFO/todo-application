using Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext
            (DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
            Database.EnsureCreated();
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Priority> Priorities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));

            //TODO Удалить EnableSensitiveDataLogging
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.Id).HasName("user_pkey");

                entity.HasIndex(e => e.Name).IsUnique();
            });

            builder.Entity<Priority>(entity =>
            {
                entity.ToTable("priority");
                entity.HasKey(e => e.Id).HasName("priority_pkey");

                entity.HasIndex(e => e.Level).IsUnique();
            });

            builder.Entity<ToDoItem>(entity =>
            {
                entity.ToTable("todo-item");
                entity.HasKey(e => e.Id).HasName("todo-item_pkey");

                entity.HasIndex(e => e.Title);

                entity.Property(e => e.Title).IsRequired();
            });
        }
    }

}
