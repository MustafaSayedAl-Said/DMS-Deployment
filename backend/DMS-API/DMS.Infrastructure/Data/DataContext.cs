using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<MyDirectory> Directories { get; set; }

        public virtual DbSet<Workspace> Workspaces { get; set; }

        public virtual DbSet<ActionLog> ActionLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .ToTable("Users");

            modelBuilder.Entity<User>()
            .HasOne(u => u.Workspace)
            .WithOne(w => w.User)
            .HasForeignKey<Workspace>(w => w.UserId);

            modelBuilder.Entity<MyDirectory>()
            .HasOne(d => d.Workspace)
            .WithMany(w => w.Directories)
            .HasForeignKey(d => d.WorkspaceId);

            modelBuilder.Entity<Document>()
            .HasOne(d => d.MyDirectory)
            .WithMany(dir => dir.Documents)
            .HasForeignKey(d => d.DirectoryId);

            // ActionLogs configuration
            modelBuilder.Entity<ActionLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete on UserId

            modelBuilder.Entity<ActionLog>()
                .HasOne(al => al.Document)
                .WithMany()
                .HasForeignKey(al => al.DocumentId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
