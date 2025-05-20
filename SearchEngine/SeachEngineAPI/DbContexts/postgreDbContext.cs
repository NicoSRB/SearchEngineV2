using Microsoft.EntityFrameworkCore;
using Shared.Model;


namespace SeachEngineAPI.DbContexts
{
    public class postgreDbContext : DbContext
    {
        public DbSet<Word> word { get; set; }
        public DbSet<BEDocument> document { get; set; }
        public DbSet<Occ> occ { get; set; }

        private readonly string _connectionString;

        public postgreDbContext(DbContextOptions<postgreDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Word>(entity =>
            {
                entity.HasKey(w => w.Id); // Primary Key
                entity.Property(w => w.Id).HasColumnName("id");
                entity.Property(w => w.Name).HasColumnName("name");
            });

            modelBuilder.Entity<BEDocument>(entity =>
            {
                entity.HasKey(d => d.mId); // Ensure mId is a primary key
                entity.Property(d => d.mId).HasColumnName("id"); // Explicitly map mId
                entity.Property(d => d.mUrl).HasColumnName("url"); // Explicitly map mUrl
                entity.Property(d => d.mIdxTime).HasColumnName("idxtime");
                entity.Property(d => d.mCreationTime).HasColumnName("creationtime");
            });

            modelBuilder.Entity<Occ>(entity =>
            {
                entity.HasKey(o => new { o.WordId, o.DocumentId }); // Composite Key
                entity.Property(o => o.DocumentId).HasColumnName("docid");
                entity.Property(o => o.WordId).HasColumnName("wordid");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
