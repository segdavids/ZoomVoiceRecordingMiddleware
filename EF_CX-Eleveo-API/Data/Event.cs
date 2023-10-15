namespace Eleveo_EFCX_Connector_API.Data
{
    public class Event
    {
        public List<string> correlationIds { get; set; }
        public DateTime start { get; set; }
        public Resource resource { get; set; }
        public string type { get; set; }
        public DateTime created { get; set; }
        public string eventId { get; set; }
        public string direction { get; set; }
        public Data data { get; set; }
        public Participant participant { get; set; }
    }
}
