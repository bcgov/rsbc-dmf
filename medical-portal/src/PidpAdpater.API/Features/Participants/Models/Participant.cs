namespace pdipadapter.Features.Participants.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Address
    {
        public string addressTypeCd { get; set; } = string.Empty;
        public string addressLine1 { get; set; } = string.Empty;
        public string addressLine2 { get; set; } = string.Empty;
        public string addressLine3 { get; set; } = string.Empty;
        public string postalCode { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string province { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
    }

    public class AssignedAgency
    {
        public string agenId { get; set; } = string.Empty;
        public string agencyName { get; set; } = string.Empty;
        public string agencyType { get; set; } = string.Empty;
        public string startDate { get; set; } = string.Empty;
    }

    public class GrantedRole
    {
        public string role { get; set; } = string.Empty;
    }

    public class Participant
    {
        public List<ParticipantDetail> participantDetails { get; set; } = new List<ParticipantDetail>();
    }

    public class ParticipantDetail
    {
        public string surname { get; set; } = string.Empty;
        public string firstGivenNm { get; set; } = string.Empty;
        public string secondGivenNm { get; set; } = string.Empty;
        public string thirdGivenNm { get; set; } = string.Empty;
        public string birthDate { get; set; } = string.Empty;
        public string emailAddress { get; set; } = string.Empty;
        public string partId { get; set; } = string.Empty;
        public string partUserId { get; set; } = string.Empty;
        public List<Address> addresses { get; set; } = new List<Address>();
        public List<AssignedAgency> assignedAgencies { get; set; } = new List<AssignedAgency>();
        public List<GrantedRole> GrantedRoles { get; set; } = new List<GrantedRole>();
    }

    public class Party
    {
        public Participant participant { get; set; } = new Participant();
    }


}
