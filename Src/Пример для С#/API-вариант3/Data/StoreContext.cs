using Microsoft.EntityFrameworkCore;
using StorageApi.Models;

namespace StorageApi.Data;

public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

    // Таблица с файлами
    public DbSet<FileRecord> Files => Set<FileRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FileRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(255);
            e.Property(x => x.Path).IsRequired().HasMaxLength(1024);
            e.Property(x => x.Format).IsRequired().HasMaxLength(10);
        });
    }
}
