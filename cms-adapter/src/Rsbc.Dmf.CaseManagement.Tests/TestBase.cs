namespace Rsbc.Dmf.CaseManagement.Tests
{
    public class TestBase
    {
        //set to null to run tests in this class, requires to be on VPN and Dynamics params configured in secrets.xml
#if RELEASE
        protected const string RequiresDynamics = "Integration tests that requires Dynamics connection via VPN";
#else
        protected const string RequiresDynamics = null;
#endif
    }
}