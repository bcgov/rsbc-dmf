namespace MigrationMetrics.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using MigrationMetrics.Models;
using MigrationMetrics.Models.MonthlyCountStat;

using MigrationMetrics.Services;
using System.Diagnostics.Eventing.Reader;

[ApiController]
[Route("api/[controller]")]

public class VelocityController : ControllerBase
{
   

    private IMonthlyCountStatService _monthlyCountStatService;
    private IMapper _mapper;

    public VelocityController(
        IMonthlyCountStatService dataService,
        IMapper mapper)
    {
        _monthlyCountStatService = dataService;
        _mapper = mapper;
    }

    [HttpGet("{category}")]
    public IActionResult GetChart(string category)
    {
        var recordedDates = _monthlyCountStatService.GetRecordedDatesByCategory(category).OrderBy(x => x);

        List<VelocityData> data = new List<VelocityData>();

        

        int previousTotal = 0;
        DateTime previousDate = DateTime.MinValue;

        bool first = true;

        foreach (var recordedDate in recordedDates)
        {
            VelocityData velocityData = new VelocityData() { Label = recordedDate.ToShortDateString() };

            var theData = _monthlyCountStatService.GetDataByRecordedDateCategory(recordedDate,category);
            int total = 0;
            foreach (var item in theData)
            {
                total += item.DestinationCount;
            }

            velocityData.Total = total;

            if (first)
            {
                velocityData.Improvement = 0;
                velocityData.Time = 0.0;
                velocityData.Velocity = 0.0;
                velocityData.ProjectedEnd = "N/A";

                first = false;
            }
            else
            {
                velocityData.Improvement = previousTotal - total;
                
                double elapsedTime = (recordedDate -previousDate).TotalMinutes / 60.0;
                velocityData.Time = elapsedTime;
                if (velocityData.Improvement > 0)
                {
                    velocityData.Velocity = (velocityData.Improvement * 1.0) / (elapsedTime * 1.0);
                    double estimatedHours = (total * 1.0) / velocityData.Velocity;

                    if (estimatedHours > 24) { 
                        estimatedHours = estimatedHours / 24;
                        velocityData.ProjectedEnd = estimatedHours.ToString("0.#") + " Days";
                        }
                    else
                    {
                        velocityData.ProjectedEnd = estimatedHours.ToString("0.#") + " Hours";
                    }

                }
                else
                {
                    velocityData.Velocity = 0.0  ;
                    velocityData.ProjectedEnd = "N/A";
                }
                
            }
            previousDate = recordedDate;
            previousTotal = total;


            data.Add(velocityData);
        }

        
        return Ok(data);
    }

}



