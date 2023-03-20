//namespace pdipadapter.Data.ef
//{
//    public partial class JustinUser
//    {
//        [NotMapped]
//        public long Id { get => UserId; set => UserId = value; }
//        public ICollection<JustinRole> GetRoles() => UserRoles?.Select(r => r.Role).ToArray();

//        public JustinPartyType GetPartyType() => PartyType.
//        public JustinUser(JustinPerson person, JustinPartyType partyType) : this()
//        {
//            Person = person ?? throw new ArgumentNullException(nameof(person)); 
//            PartyType = partyType ?? throw new ArgumentNullException(nameof(partyType));
//            PersonId = person.PersonId;
//            PartyType.Code = partyType.Code; 
//        }
//    }
//}
