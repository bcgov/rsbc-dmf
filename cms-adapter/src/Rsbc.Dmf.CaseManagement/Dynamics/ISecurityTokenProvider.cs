using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public interface ISecurityTokenProvider
    {
        Task<string> AcquireToken();
    }
}
