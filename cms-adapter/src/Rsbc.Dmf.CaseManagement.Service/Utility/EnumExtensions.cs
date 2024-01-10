using System;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public static class EnumEx
    {
        public static TEnum Convert<T, TEnum>(this T n) where T : struct, Enum where TEnum : struct, Enum
        {
            return Enum.Parse<TEnum>(n.ToString());
        }
    }
}
