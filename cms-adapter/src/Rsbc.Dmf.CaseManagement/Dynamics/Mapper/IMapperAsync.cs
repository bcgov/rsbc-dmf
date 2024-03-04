using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IMapperAsync<TSource, TDest>
    {
        Task<TDest> Map(TSource source);
    }
}
