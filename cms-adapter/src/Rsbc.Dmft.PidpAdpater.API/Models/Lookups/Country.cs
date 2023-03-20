namespace pdipadapter.Models.Lookups;

[Table("CountryLookup")]
public class Country
{
    [Key]
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}

public class CountryDataGenerator : ILookupDataGenerator<Country>
{
    public IEnumerable<Country> Generate() => new[]
    {
        new Country { Code = "CA", Name = "Canada"        },
        new Country { Code = "US", Name = "United States" }
    };

    public Task<IEnumerable<Country>> GenerateAsync()
    {
        throw new NotImplementedException();
    }
}
