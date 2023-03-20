using pdipadapter.Core.Extension;
using pdipadapter.Data;
using pdipadapter.Data.Comparer;
using pdipadapter.Data.ef;
using pdipadapter.Extensions;
using pdipadapter.Features.Users.Queries;
using Microsoft.EntityFrameworkCore;

namespace pdipadapter.Features.Users.Services;

public class UserService : IUserService
{
    private readonly JumDbContext _context;
    private readonly IHttpContextAccessor _accessor;
    private readonly ILogger<UserService> _logger;
    public UserService(JumDbContext context, IHttpContextAccessor accessor, ILogger<UserService> logger)
    {
        _context = context;
        _accessor = accessor;
        _logger = logger;
    }
    public async Task<JustinUser> AddUser(JustinUser user)
    {
        var u = await AddWithoutSave(user);
        _context.Add(u);
        _context.SaveChanges();
        return await GetUserById(user.UserId);
    }

    public async Task<JustinUser> AddWithoutSave(JustinUser user)
    {
        var agency = await _context.Agencies.FirstOrDefaultAsync(n => n.AgencyId == user.AgencyId);
        if (agency == null) throw new ArgumentException(nameof(user), "User must belong to a valid agency.");

        var userId = _accessor?.HttpContext?.User.GetUserId();
        var idp = _accessor?.HttpContext?.User.GetIdentityProvider();
        if (idp == null) _logger.LogInformation("No Identity Provider associated with this user.");
        JustinIdentityProvider? userIdp = new JustinIdentityProvider();

        user.Agency = agency;

        user.PartyType = await _context.PartyTypes.FirstAsync(n => n.Code == user.PartyType.Code);

        if (idp != null && userId != null)
        {
            if (idp == "idr") idp = "oidazure";
            else if (idp == "bcsc") idp = "oidc";
            userIdp = await _context.IdentityProviders.FirstOrDefaultAsync(n => n.Alias == idp);
            user.IdentityProvider = userIdp;
            user.DigitalIdentifier = userId;
        }

        return user;
    }
    public async Task<IEnumerable<JustinUser>> AllUsersList()
    {
        return await _context.Users
            .Include(p => p.Person)
            .Include(pt => pt.PartyType)
            .Include(i => i.IdentityProvider)
            .Include(a => a.Agency)
            .ThenInclude(a => a.AgencyAssignments)
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .AsSplitQuery()
            .ToListAsync();
    }

    public Task<long> DeleteUser(JustinUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<JustinUser> GetUserById(long id)
    {
        return await _context.Users
            .Include(p => p.Person)
            .Include(pt => pt.PartyType)
            .Include(i => i.IdentityProvider)
            .Include(a => a.Agency)
            .ThenInclude(a => a.AgencyAssignments)
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .AsSplitQuery()
            .SingleOrDefaultAsync(u => u.UserId == id) ?? throw new KeyNotFoundException();

    }

    public async Task<JustinUser> GetUserByUserName(string userName)
    {
        return await _context.Users
            .Include(p => p.Person)
            .Include(pt => pt.PartyType)
            .Include(i => i.IdentityProvider)
            .Include(a => a.Agency)
            .ThenInclude(a => a.AgencyAssignments)
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.UserName == userName) ?? throw new KeyNotFoundException($"username {userName} not found");
    }

    public async Task<JustinUser> GetUserByPartId(long partId)
    {
        return await _context.Users
            .Include(p => p.Person)
            //.Include(pt => pt.PartyType)
            //.Include(i => i.IdentityProvider)
            //.Include(a => a.Agency)
            //.ThenInclude(a => a.AgencyAssignments)
            //.Include(r => r.UserRoles)
            //.ThenInclude(r => r.Role)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.ParticipantId == partId) ?? throw new KeyNotFoundException($"Participant with id {partId} not found");
    }

    public async Task<JustinUser> UpdateUser(JustinUser update)
    {
        var user = await UpdateWithoutSave(update);

        _context.SaveChanges();

        return update;
    }

    private async Task<JustinUser> UpdateWithoutSave(JustinUser update)
    {
        var user = await _context.Users
                 .Include(u => u.Person)
                 .ThenInclude(p => p.Address)
                 .Include(u => u.UserRoles)
                 .ThenInclude(r => r.Role)
                 .Include(u => u.Agency)
                 .Include(pt => pt.PartyType)
                 .AsSplitQuery()
                 .FirstOrDefaultAsync(u => u.ParticipantId == update.ParticipantId) ?? throw new KeyNotFoundException();
        
        var agency = await _context.Agencies.FirstOrDefaultAsync(n => n.AgencyId == update.AgencyId);
        if (agency == null) throw new ArgumentException(nameof(user), "User must belong to a valid agency.");


        var addRoles = update.UserRoles.Except(user.UserRoles, new UserRoleRoleIdComparer());

        addRoles.ForEach(r => user.UserRoles.Add(new JustinUserRole() { UserId = user.UserId, RoleId = r.RoleId }));
        var removeRoles = user.UserRoles.Except(update.UserRoles, new UserRoleRoleIdComparer());
        removeRoles.ForEach(r =>
        {
            var remove = user.UserRoles.FirstOrDefault(r2 => r2.RoleId == r.RoleId);
            if (remove != null)
                _context.Entry(remove).State = EntityState.Deleted;
        });

        var userId = _accessor?.HttpContext?.User.GetUserId();
        var idp = _accessor?.HttpContext?.User.GetIdentityProvider();
        if (idp == null) _logger.LogInformation("No Identity Provider associated with this user.");
        JustinIdentityProvider? userIdp = new JustinIdentityProvider();

        if (idp != null && userId != null)
        {
            if (idp == "idr") idp = "oidazure";
            else if (idp == "bcsc") idp = "oidc";
            userIdp = await _context.IdentityProviders.FirstOrDefaultAsync(n => n.Alias == idp);
            user.IdentityProvider = userIdp;
            user.DigitalIdentifier = userId;
        }

        user.Agency = agency;
        user.UserName = update.UserName;
        user.PartyType = await _context.PartyTypes.FirstAsync(n => n.Code == update.PartyType.Code); ;
        user.Person = update.Person;
        //user.IdentityProvider = update.IdentityProvider;

        _context.Users.Update(user);

        return user;
    }
}

