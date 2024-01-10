using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IMapper<TSource, TDest>
    {
        TDest Map(TSource source);
    }
    public interface IMapperAsync<TSource, TDest>
    {
        Task<TDest> Map(TSource source);
    }
}
