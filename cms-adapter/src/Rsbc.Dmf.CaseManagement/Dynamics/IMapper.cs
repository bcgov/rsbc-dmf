using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IMapper<TSource, TDest>
    {
        Task<TDest> Map(TSource source);
    }
}
