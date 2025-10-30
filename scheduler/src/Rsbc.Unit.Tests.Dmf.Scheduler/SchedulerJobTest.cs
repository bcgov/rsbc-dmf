using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.Scheduler;
using Xunit;
using static Rsbc.Dmf.BcMailAdapter.BcMailAdapter;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Rsbc.Unit.Tests.Dmf.Scheduler
{
    public class SchedulerJobTest
    {
        IConfiguration _configuration;
        ScheduledJobs _scheduledJobs;
        private IcbcAdapterClient _icbcAdapterClient;
        private CaseManagerClient _caseManagerClient;
        private BcMailAdapterClient _bcMailAdapterClient;



        public SchedulerJobTest()
        {

            _scheduledJobs = new ScheduledJobs(_configuration, _scheduledJobs, _icbcAdapterClient, _caseManagerClient, _bcMailAdapterClient); ;
            
        }

        [Fact]
        public async void TestToSentToBcMail()
        {
            await _scheduledJobs.SendToBcMail(null);
        }

        [Fact]
        public async void TestToUpdateBirthDate()
        {
            await _scheduledJobs.UpdateBirthdate(null);
        }

        [Fact]
        public async void TestGetIcbcNotifications()
        {
            await _scheduledJobs.GetIcbcNotifications(null);
        }
    }
}
