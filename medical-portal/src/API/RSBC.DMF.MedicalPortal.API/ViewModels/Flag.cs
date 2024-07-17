namespace RSBC.DMF.MedicalPortal.API.ViewModels;

/// <summary>
/// Enum for Flag Type Option Set
/// </summary>
/// NOTE that the dynamic values here should only be used in cms-adapter only and they are only here for reference and ease of mapping values
public enum FlagTypeOptionSet
{
    Submittal = 100000000,
    Review = 100000001,
    FollowUp = 100000002,
    Message = 100000003
}

public class Flag
{
    public string Question { get; set; }
    public string Identifier { get; set; }
    public FlagTypeOptionSet? FlagType { get; set; }
    public string FormId { get; set; }
}