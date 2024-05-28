namespace RSBC.DMF.MedicalPortal.API.Auth;
public class AuthConstant
{
    public static class Claims
    {
        public const string Address = "address";
        public const string AssuranceLevel = "identity_assurance_level";
        public const string Birthdate = "birthdate";
        public const string Gender = "gender";
        public const string Email = "pidp_email";
        public const string FamilyName = "family_name";
        public const string GivenName = "given_name";
        public const string GivenNames = "given_names";
        public const string IdentityProvider = "identity_provider";
        public const string PreferredUsername = "preferred_username";
        public const string ResourceAccess = "resource_access";
        public const string Subject = "sub";
        public const string Roles = "roles";
        public const string LoginIds = "login_ids";
        public const string Scope = "scope";
    }
    public static class Roles
    {
        // PIdP Role Placeholders
        public const string Practitoner = "PRACTITIONER";
        public const string Moa = "MOA";
        public const string Dmft = "DMFT_ENROLLED";
    }
    public static class Policies
    {
        public const string Oidc = "oidc";
        public const string MedicalPractitioner = "medical-practitioner";
        public const string Enrolled = "dmft-enrolled";
    }
    public static class Clients
    {
        public const string License = "LICENCE-STATUS";
        public const string DmftStatus = "DMFT-WEBAPP";
    }
}
