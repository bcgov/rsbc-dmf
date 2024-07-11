namespace RSBC.DMF.MedicalPortal.API
{
    public static class DmerUtilities
    {
        public static string TranslateDmerStatus(string dmerStatus, string loginId)
        {
            if (dmerStatus == "Open-Required")
            {
                if (string.IsNullOrEmpty(loginId))
                {
                    dmerStatus = "Required - Unclaimed";
                }
                else
                {
                    dmerStatus = "Required - Claimed";
                }
            }

            if (dmerStatus == "Non-Comply")
            {
                if (string.IsNullOrEmpty(loginId))
                {
                    dmerStatus = "Non-Comply - Unclaimed";
                }
                else
                {
                    dmerStatus = "Non-Comply - Claimed";
                }
            }

            return dmerStatus;
        }
    }
}
