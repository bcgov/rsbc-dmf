namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public static class Translate
    {
        public static string DmerType(int? optionSetValue)
        {
            string result = null;
            switch (optionSetValue)
            {
                case 100000000:
                    result = "Commercial/NSC";
                    break;
                case 100000001:
                    result = "Age";
                    break;
                case 100000002:
                    result = "Industrial Road";
                    break;
                case 100000003:
                    result = "Known Medical";
                    break;
                case 100000006:
                    result = "Suspected Medical";
                    break;
                case 100000005:
                    result = "No DMER";
                    break;
            }
            return result;
        }
    }
}
