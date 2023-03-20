namespace RSBC.DMF.MedicalPortal.API.Auth;
public class AuthConstant
{
    public static class Claims
    {
        public const string Address = "address";
        public const string AssuranceLevel = "identity_assurance_level";
        public const string Birthdate = "birthdate";
        public const string Gender = "gender";
        public const string Email = "email";
        public const string FamilyName = "family_name";
        public const string GivenName = "given_name";
        public const string GivenNames = "given_names";
        public const string IdentityProvider = "identity_provider";
        public const string PreferredUsername = "preferred_username";
        public const string ResourceAccess = "resource_access";
        public const string Subject = "sub";
        public const string Roles = "roles";
    }
    public static class Roles
    {
        // PIdP Role Placeholders
        public const string Practitoner = "PRACTITIONER";
        public const string Moa = "MOA";
    }
}

