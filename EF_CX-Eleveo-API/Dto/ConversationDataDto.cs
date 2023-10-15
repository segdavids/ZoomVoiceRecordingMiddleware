namespace Eleveo_EFCX_Connector_API.Dto
{
    public class ConversationDataDto
    {
        public int Id { get; set; }

        public string conversation_Id { get; set; }
        public string? Resource_Uri { get; set; }
        public string? Jtapi_Id { get; set; }
        public string? CallRec_Groupid { get; set; }
        public string? CallRec_Callid { get; set; }
        public string? CallRec_Sid { get; set; }
        public string? Jsession_Id { get; set; }
        public DateTime Start_Time { get; set; }
        public string? Event_Type { get; set; }
        public DateTime? Created_At { get; set; }
        public string? Event_ID { get; set; }
        public string Direction { get; set; }
        public DateTime? Date_Fetched { get; set; }
        public DateTime? Date_Requested { get; set; }
        public int Status { get; set; }
    }
}
