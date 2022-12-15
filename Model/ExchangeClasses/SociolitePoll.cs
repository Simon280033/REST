namespace REST.Model.ExchangeClasses
{
    public class SociolitePoll
    {
        public string Id { get; set; }
        public string CreatedById { get; set; }
        public string CreationTime { get; set; }
        public string Question { get; set; }
        public List<string> Answers { get; set; }
    }
}
