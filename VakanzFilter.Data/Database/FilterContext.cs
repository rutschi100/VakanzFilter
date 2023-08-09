using Microsoft.EntityFrameworkCore;
using VakanzFilter.Data.Entities;

namespace VakanzFilter.Data.Database;

public class FilterContext : DbContext  
{
    public FilterContext(DbContextOptions<FilterContext> options) : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  
    {  
        // optionsBuilder.UseNpgsql();  
        if (!optionsBuilder.IsConfigured)  
        {  
            optionsBuilder.UseNpgsql();  
        }  
    }

    public DbSet<Filters> Filters { get; set; }
}