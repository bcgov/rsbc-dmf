using pdipadapter.Data.ef;
using System.Diagnostics.CodeAnalysis;

namespace pdipadapter.Data.Comparer
{
    public class UserRoleRoleIdComparer : IEqualityComparer<JustinUserRole>
    {
        public bool Equals([AllowNull]  JustinUserRole? x, [AllowNull]  JustinUserRole? y)
        {
            return x != null && y != null && GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode([DisallowNull] JustinUserRole obj)
        {
            var hash = new HashCode();
            hash.Add(obj.RoleId);
            return hash.ToHashCode();
        }
    }
}
