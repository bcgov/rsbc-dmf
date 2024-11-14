using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.DriverPortal.ViewModels
{
    public class UserRegistration
    {
        public string DriverLicenseNumber { get; set; }
        public string Email { get; set; }
        public bool NotifyByMail { get; set; }
        public bool NotifyByEmail { get; set; }
        public FullAddress? Address { get; set; }

    }
}
