using Microsoft.EntityFrameworkCore;

class FruitDb : DbContext
{
    public FruitDb(DbContextOptions<FruitDb> options)
        : base(options) { }

    public DbSet<Fruit> Fruits => Set<Fruit>();

    public DbSet<Color> Colors => Set<Color>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Color>(entity =>
        { 
            // entity.HasMany(c => c.Fruits)
            //     .WithOne(f => f.ColorNavigation)
            //     .HasForeignKey(f => f.ColorId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("FK_Fruit_Colors_ColorId");
            entity.HasData(
                new Color { Id = 1, Name = "Orange" },
                new Color { Id = 2, Name = "Red" },
                new Color { Id = 3, Name = "Yellow" }
            );
        });
        modelBuilder.Entity<Fruit>(entity =>
        { 
            entity.HasData(
                new Fruit { Id = 1, Name = "Apple", Instock = true, ColorId = 2 },
                new Fruit { Id = 2, Name = "Banana", Instock = false, ColorId = 3  },
                new Fruit { Id = 3, Name = "Orange", Instock = true, ColorId = 1  }
                );
        }); 
    }
}