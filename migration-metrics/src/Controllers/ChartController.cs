namespace MigrationMetrics.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using MigrationMetrics.Models;
using MigrationMetrics.Models.MonthlyCountStat;

using MigrationMetrics.Services;
using System.Diagnostics.Eventing.Reader;

[ApiController]
[Route("api/[controller]")]

public class ChartController : ControllerBase
{
   

    private IMonthlyCountStatService _monthlyCountStatService;
    private IMapper _mapper;

    public ChartController(
        IMonthlyCountStatService dataService,
        IMapper mapper)
    {
        _monthlyCountStatService = dataService;
        _mapper = mapper;
    }

    [HttpGet("{chartId}")]
    public IActionResult GetChart(string chartId, bool showRed = false)
    {
        var recordedDates = _monthlyCountStatService.GetRecordedDatesByCategory(chartId);

        List<string> labels = new List<string>();

        var startDates = new List<DateTime>();// _monthlyCountStatService.GetStartDates();

        var loopDate = new DateTime(2001,01,01);
        while (loopDate < DateTime.Now)
        {
            startDates.Add(loopDate);

            loopDate = loopDate.AddMonths(1);
        }

        var startDatesArray = startDates.ToArray();

        foreach (var startDate in startDates)
        {
            labels.Add(startDate.ToShortDateString());
        }


        List<ChartDataSet> datasets = new List<ChartDataSet>();

        int l = 0;
        foreach (var recordedDate in recordedDates)
        {
            
            var theData = _monthlyCountStatService.GetDataByRecordedDateCategory(recordedDate, chartId);

            var normalizedData = new int[startDatesArray.Length];

           for (int i = 0; i < startDatesArray.Length; i++)
        {
            int newValue = 0;
                
            var matchRecord =  theData.Where(x => x.Start.Year == startDatesArray[i].Year && x.Start.Month == startDatesArray[i].Month).FirstOrDefault();

            if (matchRecord != null)
            {
                newValue = matchRecord.DestinationCount;
            }
                normalizedData[i] = newValue;

        }


        ChartDataSet newItem = new ChartDataSet { Label = recordedDate.ToShortDateString(),                
                Data = normalizedData
            };

            if (l == 0 && showRed)
            {
                newItem.PointHoverRadius = 1;
                newItem.BorderColor = "red";  
                newItem.BackgroundColor = "red";
                newItem.BorderWidth = 1;
                newItem.Fill = true; 
                //newItem.DrawActiveElementsOnTop = true;
            }
            else
            {
                int greyScale = ((int)Math.Round (((l * 1.0) / 10.0) * 255.0));
                newItem.PointHoverRadius = 1;
                newItem.BorderColor = $"#00{greyScale.ToString("X")}00";
                newItem.BackgroundColor = $"#00{greyScale.ToString("X")}00";
                newItem.BorderWidth = 1;
                newItem.Fill = true;
                //newItem.DrawActiveElementsOnTop = true;
            }


            datasets.Add(newItem);
            l++;

            // only need a certain number of lines.
            if (l > 10)
            {
                break;
            }
        }

        var chartWrapper = new ChartWrapper { 
             Type = "line",
             Data = new ChartData {  Labels = labels, Datasets = datasets  },
             Options = new ChartOptions {  AspectRatio = 1.7 }
            };

        return Ok(chartWrapper);
    }

}



