using System.Xml;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels.Interfaces
{
    public interface IExcelExportable
    {
        string[] GetHeaders();
        string[] GetRowValues();
    }

}