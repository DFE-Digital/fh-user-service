using FamilyHub.IdentityServerHost.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FamilyHub.IdentityServerHost.Persistence.Repository;

public interface IApplicationDbContext
{
    //DbSet<Organisation> Organisations { get; }
    //DbSet<OrganisationType> OrganisationTypes { get; }
    //DbSet<OrganisationMapping> OrganisationMappings { get; }

    public DbSet<UserOrganisation> UserOrganisations { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    string? GetFullName(string email);
    Task<bool> SetFullNameAsync(string email, string fullName);
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

    public string? GetFullName(string email)
    {
        var user = Users.FirstOrDefault(x => x.Email == email);
        if (user != null)
        {
            return user.FullName;
        }

        return string.Empty;
    }

    public async Task<bool> SetFullNameAsync(string email, string fullName)
    {
        var user = Users.FirstOrDefault(x => x.Email == email);
        if (user != null) 
        {
            user.FullName = fullName;
            Entry(user).State = EntityState.Modified;

            await SaveChangesAsync();

            return true;
        }

        return false;
    }
    
}

