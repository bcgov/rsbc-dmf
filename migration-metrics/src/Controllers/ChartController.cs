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

    [HttpGet()]
    public IActionResult GetChart()
    {
        var recordedDates = _monthlyCountStatService.GetRecordedDates();

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
            
            var theData = _monthlyCountStatService.GetDataByRecordedDate(recordedDate);

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

            if (l == 0)
            {
                newItem.PointHoverRadius = 2;
                newItem.BorderColor = "green";  
                newItem.BorderWidth = 2;
                newItem.Fill = false;
                newItem.DrawActiveElementsOnTop = false;
            }
            else
            {
                int greyScale = l / recordedDates.Count();
                newItem.PointHoverRadius = 1;
                newItem.BorderColor = $"#{greyScale.ToString("X")}{greyScale.ToString("X")}{greyScale.ToString("X")}";
                newItem.BorderWidth = 1;
                newItem.Fill = false;
                newItem.DrawActiveElementsOnTop = true;
            }


            datasets.Add(newItem);
            l++;
        }

        var chartWrapper = new ChartWrapper { 
             Type = "line",
             Data = new ChartData {  Labels = labels, Datasets = datasets  },
             Options = new ChartOptions {  AspectRatio = 1.7 }
            };

        return Ok(chartWrapper);
    }

}



