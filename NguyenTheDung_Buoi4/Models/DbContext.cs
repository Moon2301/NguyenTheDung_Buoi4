using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NguyenTheDung_Buoi4.Models;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : IdentityDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

}
