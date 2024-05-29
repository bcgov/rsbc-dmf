using Microsoft.AspNetCore.Authorization;

namespace pdipadapter.Infrastructure.Auth
{
    public class RealmAccessRoleRequirement : IAuthorizationRequirement
    {
        #region Properties
        /// <summary>
        /// get - The role to validate.
        /// </summary>
        /// <value></value>
        public string Role { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a RealmAccessRoleRequirment class.
        /// </summary>
        /// <param name="role"></param>
        public RealmAccessRoleRequirement(string role)
        {
            this.Role = role;
        }
        #endregion
    }
}
