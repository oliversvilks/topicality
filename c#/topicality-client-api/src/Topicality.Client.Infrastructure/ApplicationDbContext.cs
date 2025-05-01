using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sentry;
using Topicality.Domain.Entities;

namespace Topicality.Client.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
       // this.Database.SetConnectionString("Server=45.42.197.224\\MSSQLSERVER2022;Database=topicality;Persist Security Info=False;User ID=topicality;Password=CeturtaisMaijs2025;MultipleActiveResultSets=true;TrustServerCertificate=true;");
    }
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<CategoryDocument> CategoryDocuments { get; set; }
    public DbSet<Comparison> Comparisons { get; set; }
    public DbSet<ComparisonSet> ComparisonSets { get; set; }
    public DbSet<DocumentMetadata> Documents { get; set; }
    public DbSet<DocumentShare> DocumentShares { get; set; }
    public DbSet<SchemaDefinition> SchemaDefinitions { get; set; }
    public DbSet<SharedDocument> SharedDocuments { get; set; }
    public DbSet<UserCategory> UserCategories { get; set; }
    public DbSet<KnowledgeFlow> KnowledgeFLows { get; set; }
    public DbSet<KnowledgeFlowStep> KnowledgeFLowSteps { get; set; }
}