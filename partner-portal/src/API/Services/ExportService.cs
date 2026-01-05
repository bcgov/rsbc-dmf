using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.PartnerPortal.Api.ViewModels.Interfaces;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace Rsbc.Dmf.PartnerPortal.Api.Services;

public interface IExportService
{
    byte[] ExportToExcel<T>(IEnumerable<T> items) where T : IExcelExportable;
}

public class ExportService : IExportService
{

    public byte[] ExportToExcel<T>(IEnumerable<T> items)
       where T : IExcelExportable
    {
        var list = items.ToList();
        var csv = new StringBuilder();

        if (!list.Any())
            return Encoding.UTF8.GetBytes(string.Empty);

        csv.AppendLine(string.Join(",", list.First().GetHeaders()));

        foreach (var item in list)
        {
            csv.AppendLine(string.Join(",", item.GetRowValues().Select(EscapeCsv)));
        }

        var csvBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return csvBytes;
    }


    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}