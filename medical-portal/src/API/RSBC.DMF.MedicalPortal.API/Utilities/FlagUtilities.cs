using RSBC.DMF.MedicalPortal.API.ViewModels;

namespace RSBC.DMF.MedicalPortal.API
{
    public static class FlagUtilities
    {
        public static Flag[] MapChefsFlagsToCaseFlags(IEnumerable<Flag> allCaseFlags,
            Dictionary<string, object> chefsFlags)
        {
            var matchedFlags = allCaseFlags
                .Where(flag => chefsFlags.ContainsKey(flag.FormId) && (bool)chefsFlags[flag.FormId])
                .ToArray();

            return matchedFlags;
        }
    }
}