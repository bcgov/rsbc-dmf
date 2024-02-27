namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public interface IMapper<TSource, TDest>
    {
        TDest Map(TSource source);
    }
}
