using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly HttpContext _context;
    private readonly IConfiguration _config;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _context = httpContextAccessor.HttpContext;
        _config = config;
        HasContext = _context != null;
        HasClaims = HasContext && _context?.User.Identity is ClaimsIdentity claims && claims.Claims.Any();

        if (HasContext & HasClaims)
        {
            ClaimsId = _context?.User?.FindFirstValue("id");
            FirstName = _context?.User?.FindFirstValue("firstName") ?? "Anonymous";
            LastName = _context?.User?.FindFirstValue("lastName");
            UserEmail = _context?.User?.FindFirstValue(ClaimTypes.Email);
            //ERole = Enum.Parse<ERoleByName>(_context.User.Claims
            //                        .Where(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)
            //                        .FirstOrDefault(x => Enum.IsDefined(typeof(ERoleByName), x.Value)).Value);

            Token = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString().Split(' ').LastOrDefault();
        }
    }

    public bool HasClaims { get; }
    public bool HasContext { get; }
    public int? UserId
    {
        get
        {
            if (!HasClaims)
            {
                if (!HasContext)
                {
                    return GetSystemId();
                }
                else
                {
                    return GetAnonymousId();
                }
            }
            else return null;
        }
    }

    private string ClaimsId { get; }
    public string Token { get; }
    public string UserEmail { get; }
    public EUserType EUserType { get; }
    public int BranchId { get; }
    public string FirstName { get; }
    public string LastName { get; }
   
    /// <summary>
    /// make different validations separating the calims by commas
    /// <code>HasScope("scope1, scope2, etc...") </code>
    /// </summary>
    public bool HasScope(string scope)
    {
        if (!_context.User.HasClaim(c => c.Type == "scope"))
            return false;

        // Split the scopes string into an array
        var scopes = _context.User.FindFirst(c => c.Type == "scope").Value.Split(' ');

        // Succeed if the scope array contains the required scope
        return scopes.Any(s => scope.Split(',').Contains(s));

    }

    public int? GetAnonymousId()
    {
        return GetUsersFromConfig().Where(x => x.Name == "Anonymous").FirstOrDefault()?.AspNetId;
    }
    public int? GetSystemId()
    {
        return GetUsersFromConfig().Where(x => x.Name == "System").FirstOrDefault()?.AspNetId;
    }

    public List<UserDetail> GetUsersFromConfig()
    {
        var result = JsonConvert.DeserializeObject<UserDetail[]>(_config.GetValue<string>("KV-SystemUsers-Users"));
        if (result != null && result.Length > 0)
            return result.ToList();
        else return new List<UserDetail>();
    }
}