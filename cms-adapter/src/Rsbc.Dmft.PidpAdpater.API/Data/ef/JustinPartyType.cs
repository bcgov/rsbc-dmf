using pdipadapter.Models.Lookups;

namespace pdipadapter.Data.ef;

[Table("PartyTypeLookup")]
public class JustinPartyType
{
    [Key]
    public PartyTypeCode Code { get; set; }
    public string Name { get; set; } = string.Empty;
}
public enum PartyTypeCode
{
    Organization = 1,
    Individual = 2,
    Staff = 3,
}
public class PartyTypeDataGenerator : ILookupDataGenerator<JustinPartyType>
{
    public IEnumerable<JustinPartyType> Generate() => new[]
    {
        new JustinPartyType { Code = PartyTypeCode.Organization, Name = "Organization"},
        new JustinPartyType { Code = PartyTypeCode.Individual, Name = "Individual"},
        new JustinPartyType { Code = PartyTypeCode.Staff, Name = "Staff"},
    };

    public Task<IEnumerable<JustinPartyType>> GenerateAsync()
    {
        throw new NotImplementedException();
    }
}

