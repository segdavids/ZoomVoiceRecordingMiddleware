namespace Eleveo_EFCX_Connector_API.Data
{
    public class Resource
    {
        public string uri { get; set; }
        public List<string> flags { get; set; }
        public int? channel { get; set; }
        public Parent parent { get; set; }
    }
}
