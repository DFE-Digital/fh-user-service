using FamilyHub.IdentityServerHost.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.IdentityServerHost.Persistence.Repository;

public interface IApplicationDbContext
{
    //DbSet<Organisation> Organisations { get; }
    //DbSet<OrganisationType> OrganisationTypes { get; }
    //DbSet<OrganisationMapping> OrganisationMappings { get; }

    public DbSet<UserOrganisation> UserOrganisations { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}

public class ApplicationDbContext : IdentityDbContext<ApplicationIdentityUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    //public DbSet<Organisation> Organisations => Set<Organisation>();
    //public DbSet<OrganisationType> OrganisationTypes => Set<OrganisationType>();
    //public DbSet<OrganisationMapping> OrganisationMappings => Set<OrganisationMapping>();

    public DbSet<UserOrganisation> UserOrganisations => Set<UserOrganisation>();
    
}

