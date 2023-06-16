using MigrationMetrics.Helpers;
using AutoMapper;

using MigrationMetrics.Entities;
using MigrationMetrics.Models.MonthlyCountStat;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MigrationMetrics.Services;



public interface IMonthlyCountStatService
{
IEnumerable<MonthlyCountStat> GetAll();
IEnumerable<MonthlyCountStat> GetByCategory(string category);

    IEnumerable<string> GetCategories();
    void Create(CreateRequest model);
void Update(int id, UpdateRequest model);
void Delete(int id);

    IEnumerable<DateTime> GetRecordedDates();

    IEnumerable<DateTime> GetRecordedDatesByCategory(string category);

    IEnumerable<DateTime> GetStartDates();

    IEnumerable<MonthlyCountStat> GetDataByRecordedDate(DateTime recordedDate);

    IEnumerable<MonthlyCountStat> GetDataByRecordedDateCategory(DateTime recordedDate, string category);

}

public class MonthlyCountStatService : IMonthlyCountStatService
{
private DataContext _context;
private readonly IMapper _mapper;

public MonthlyCountStatService(
    DataContext context,
    IMapper mapper)
{
    _context = context;
    _mapper = mapper;
}

public IEnumerable<MonthlyCountStat> GetAll()
{
    return _context.MonthlyCountStats;
}

public IEnumerable<MonthlyCountStat> GetByCategory(string category)
{
    return _context.MonthlyCountStats.Where(x => x.Category == category);
}

    public IEnumerable<DateTime> GetRecordedDates()
{
        return _context.MonthlyCountStats.Select(x => x.RecordedTime).Distinct().OrderByDescending(x => x);
}


    public IEnumerable<DateTime> GetRecordedDatesByCategory(string category)
    {
        return _context.MonthlyCountStats.Where(x => x.Category == category).Select(x => x.RecordedTime).Distinct().OrderByDescending(x => x);
    }

    public IEnumerable<string> GetCategories()
    {
        return _context.MonthlyCountStats.Select(x => x.Category).Distinct().OrderBy(x => x);
    }

    public IEnumerable<DateTime> GetStartDates()
{
    return _context.MonthlyCountStats.Select(x => x.Start).Distinct().OrderBy(x => x);
}

public   IEnumerable<MonthlyCountStat> GetDataByRecordedDate(DateTime recordedDate)
{

        // trim the ticks as sqlite lacks precision
        //recordedDate = recordedDate.Subtract(TimeSpan.FromTicks(recordedDate.Ticks % 1000));

        var rawData = _context.MonthlyCountStats.ToList(); 
        /*
        var filtered = new List<MonthlyCountStat>();

        foreach (var data in rawData)
        {
            var ticks = data.RecordedTime.Ticks;

            if (ticks == recordedDate.Ticks)
            {
                filtered.Add(data);
            }
        }
        */
        var filtered = rawData.Where(x => x.RecordedTime.Ticks == recordedDate.Ticks).ToList();

        var ordered = filtered.OrderBy(x => x.Start);

        

        return ordered;
}


    public IEnumerable<MonthlyCountStat> GetDataByRecordedDateCategory(DateTime recordedDate, string category)
    {

        // trim the ticks as sqlite lacks precision
        //recordedDate = recordedDate.Subtract(TimeSpan.FromTicks(recordedDate.Ticks % 1000));

        var rawData = _context.MonthlyCountStats.ToList();
        /*
        var filtered = new List<MonthlyCountStat>();

        foreach (var data in rawData)
        {
            var ticks = data.RecordedTime.Ticks;

            if (ticks == recordedDate.Ticks)
            {
                filtered.Add(data);
            }
        }
        */
        var filtered = rawData.Where(x => x.Category == category && x.RecordedTime.Ticks == recordedDate.Ticks).ToList();

        var ordered = filtered.OrderBy(x => x.Start);



        return ordered;
    }

    public MonthlyCountStat GetById(int id)
{
    return getUser(id);
}

public void Create(CreateRequest model)
{
    // map model to new user object
    var monthlyCountStats = _mapper.Map<MonthlyCountStat>(model);

    // save user
    _context.MonthlyCountStats.Add(monthlyCountStats);
    _context.SaveChanges();
}

public void Update(int id, UpdateRequest model)
{
    var user = getUser(id);

    // copy model to user and save
    _mapper.Map(model, user);
    _context.MonthlyCountStats.Update(user);
    _context.SaveChanges();
}

public void Delete(int id)
{
    var user = getUser(id);
    _context.MonthlyCountStats.Remove(user);
    _context.SaveChanges();
}

// helper methods

private MonthlyCountStat getUser(int id)
{
    var monthlyCountStat = _context.MonthlyCountStats.Find(id);
    if (monthlyCountStat == null) throw new KeyNotFoundException("User not found");
    return monthlyCountStat;
}
}
