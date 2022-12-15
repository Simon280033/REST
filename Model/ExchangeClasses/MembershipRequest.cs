namespace REST.Model.ExchangeClasses
{
    public class MembershipRequest
    {
        public string userId { get; set; }
        public int teamId { get; set; }
        public string newRole { get; set; }
    }
}
