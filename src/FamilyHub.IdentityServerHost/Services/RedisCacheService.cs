using FamilyHub.IdentityServerHost.Models;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.Services;

public interface IRedisCacheService
{
    string RetrieveLastPageName();
    void StoreCurrentPageName(string? currPage);
    NewUser? RetrieveNewUser();
    void StoreNewUser(NewUser? vm);
    void ResetNewUser();
    NewOrganisation? RetrieveNewOrganisation();
    void StoreNewOrganisation(NewOrganisation? vm);
    void ResetLastPageName();
}

public class RedisCacheService : IRedisCacheService
{
    private readonly string? UserId;
    public string KeyCurrentPage
    {
        get 
        {
            if (UserId != null)
            {
                return $"_CurrentPage{UserId}";
            }
            return "_CurrentPage";  
        }
    }

    public string KeyNewUser
    {
        get 
        {
            if (UserId != null) 
            {
                return $"_NewUser{UserId}";
            }           
            return "_NewUser"; 
        }
    }

    public string KeyOrganisation
    {
        get
        {
            if (UserId != null)
            {
                return $"_NewOrganisation{UserId}";
            }
            return "_NewOrganisation";
        }
    }

    private readonly IRedisCache _redisCache;
    private readonly int _timespanMinites;

    public RedisCacheService(IRedisCache redisCache, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _redisCache = redisCache;
        _timespanMinites = configuration.GetValue<int>("SessionTimeOutMinutes");
        UserId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public NewOrganisation? RetrieveNewOrganisation()
    {
        return _redisCache.GetValue<NewOrganisation>(KeyOrganisation);
    }
    public void StoreNewOrganisation(NewOrganisation? vm)
    {
        if (vm != null)
            _redisCache.SetValue(KeyOrganisation, vm, _timespanMinites);
    }

    public NewUser? RetrieveNewUser()
    {
        return _redisCache.GetValue<NewUser>(KeyNewUser);
    }
    public void StoreNewUser(NewUser? vm)
    {
        if (vm != null)
            _redisCache.SetValue(KeyNewUser, vm, _timespanMinites);
    }

    public void ResetNewUser()
    {
        NewUser newUser = new();
        _redisCache.SetValue(KeyNewUser, newUser, _timespanMinites);
    }

    public string RetrieveLastPageName()
    {
        return _redisCache.GetStringValue(KeyCurrentPage) ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _redisCache.SetStringValue(KeyCurrentPage, currPage, _timespanMinites);
    }

    public void ResetLastPageName()
    {
        _redisCache.SetStringValue(KeyCurrentPage, String.Empty, _timespanMinites);
    }
}
