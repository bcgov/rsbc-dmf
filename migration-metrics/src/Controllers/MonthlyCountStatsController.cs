namespace MigrationMetrics.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using MigrationMetrics.Models.MonthlyCountStat;

using MigrationMetrics.Services;


[ApiController]
[Route("api/[controller]")]

public class MonthlyCountStatsController : ControllerBase
{
   

    private IMonthlyCountStatService _monthlyCountStatService;
    private IMapper _mapper;

    public MonthlyCountStatsController(
        IMonthlyCountStatService userService,
        IMapper mapper)
    {
        _monthlyCountStatService = userService;
        _mapper = mapper;
    }

    [HttpGet()]
    public IActionResult GetAll()
    {
        var users = _monthlyCountStatService.GetAll();
        return Ok(users);
    }

    [HttpGet("{category}")]
    public IActionResult GetByCategory(string category)
    {
        var data = _monthlyCountStatService.GetByCategory(category);
        return Ok(data);
    }

    
    [HttpGet("Categories")]
    public IActionResult GetCategories()
    {
        var user = _monthlyCountStatService.GetCategories();
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create(CreateRequest model)
    {
        _monthlyCountStatService.Create(model);
        return Ok(new { message = "MonthlyCountStat created" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRequest model)
    {
        _monthlyCountStatService.Update(id, model);
        return Ok(new { message = "MonthlyCountStat updated" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _monthlyCountStatService.Delete(id);
        return Ok(new { message = "MonthlyCountStat deleted" });
    }

    [HttpGet("CaseProgress")]
    public IActionResult GetCaseProgress()
    {

        var data = _monthlyCountStatService.GetCaseProgress();        
        return Ok(data);
    }

}



