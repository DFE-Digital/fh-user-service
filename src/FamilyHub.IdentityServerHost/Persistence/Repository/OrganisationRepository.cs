using FamilyHub.IdentityServerHost.Models.Entities;
using System.Threading;

namespace FamilyHub.IdentityServerHost.Persistence.Repository;

public interface IOrganisationRepository
{
    Task AddUserOrganisationAsync(UserOrganisation userOrganisation, CancellationToken cancellationToken = new CancellationToken());
    string GetUserOrganisationIdByUserId(string userId);
    List<UserOrganisation> GetUserOrganisations();
    Task DeleteUserByUserIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken());
}

public class OrganisationRepository : IOrganisationRepository
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ILogger<OrganisationRepository> _logger;
    public OrganisationRepository(IApplicationDbContext applicationDbContext, ILogger<OrganisationRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }
    public async Task AddUserOrganisationAsync(UserOrganisation userOrganisation, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            _applicationDbContext.UserOrganisations.Add(userOrganisation);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating organisation. {exceptionMessage}", ex.Message);
            throw new Exception(ex.Message, ex);
        }
        
    }

    public string GetUserOrganisationIdByUserId(string userId)
    {
        var userOrganisation = _applicationDbContext.UserOrganisations.FirstOrDefault(x => x.UserId == userId);
        if (userOrganisation != null)
        {
            return userOrganisation.OrganisationId;
        }

        return string.Empty;
    }

    public List<UserOrganisation> GetUserOrganisations()
    {
        return _applicationDbContext.UserOrganisations.ToList();
    }

    public async Task DeleteUserByUserIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
    {
        var userOrganisations = _applicationDbContext.UserOrganisations.Where(x => x.UserId == userId);
        if (userOrganisations != null && userOrganisations.Any())
        {
            foreach(var item in userOrganisations)
            {
                _applicationDbContext.UserOrganisations.Remove(item);
            }
            
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
