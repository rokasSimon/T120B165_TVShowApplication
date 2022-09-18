using Microsoft.EntityFrameworkCore;
using TVShowApplication.Models;

namespace TVShowApplication.Data
{
    public class TVShowContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Poster> Posters { get; set; }
        public DbSet<Administrator> Admins { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public TVShowContext(DbContextOptions<TVShowContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Series>()
                .Property(x => x.Directors)
                .HasConversion(
                    v => v == null ? null : string.Join(',', v),
                    v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<Series>()
                .Property(x => x.StarringCast)
                .HasConversion(
                    v => v == null ? null : string.Join(',', v),
                    v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.Reviewer)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Poster>()
                .HasMany(p => p.PostedSeries)
                .WithOne(s => s.Poster)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
