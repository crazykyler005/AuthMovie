using Microsoft.EntityFrameworkCore;

namespace AuthMovie {

    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<Movie> Movies {get;set;}
        public DbSet<Playlist> Playlists{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Playlist>()
                .Property<string>("MovieCollection")
                .HasField("_MovieNames");
        }
    }
}