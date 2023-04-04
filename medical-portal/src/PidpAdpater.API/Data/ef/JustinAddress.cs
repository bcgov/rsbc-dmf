using pdipadapter.Models.Lookups;

namespace pdipadapter.Data.ef;

public class JustinAddress
{
    [Key]
    public int Id { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public Country? Country { get; set; }

    public string ProvinceCode { get; set; } = string.Empty;

    public Province? Province { get; set; }

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Postal { get; set; } = string.Empty;
}
