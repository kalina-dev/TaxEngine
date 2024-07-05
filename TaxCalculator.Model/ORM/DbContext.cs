using Microsoft.EntityFrameworkCore;
using TaxCalculator.Model.Entities;
public class BoundaryContext : DbContext
{
    public DbSet<Tax> Boundaries { get; set; }

    public string DbPath { get; }

    public BoundaryContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "data.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Boundary
{
    public int BoundaryId { get; set; }
    public decimal MinSocialTax { get; set; }
    public decimal MaxSocialTax { get; set; }
    public decimal SocialTaxRate { get; set; }
    public decimal MinIncomeTax { get; set; }
    public decimal IncomeTaxRate { get; set; }
    public decimal CharitySpentMaxRate { get; set; }
    public string BaseCurrency { get; set; } = "IDR";

}

