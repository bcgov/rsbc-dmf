namespace pdipadapter.Features.Persons.Models;
public class Person
{
    public string Surname { get; set; } = String.Empty;
    public string FirstName { get; set; } = String.Empty;
    public string MiddleNames { get; set; } = String.Empty;
    public string NameSuffix { get; set; } = String.Empty;
    public DateTime BirthDate { get; set; } 
    public string PreferredName { get; set; } = String.Empty;
    public string? Comment { get; set; }
    public string? AddressComment { get; set; }
    public bool? IsDisabled { get; set; }

}

