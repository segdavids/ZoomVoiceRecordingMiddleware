using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleveo_EFCX_Connector_API.Dto
{
    public class ConversationsDto
    {
        public int Id { get; set; }
        public int JtapiId { get; set; }
        public string conversation_Id { get; set; }
        public string? Event_Id { get; set; }
        public string? Event_Resource_Uri { get; set; }
        public int? Event_Count { get; set; }
        public DateTime Start_Time { get; set; }
        public string? Duration { get; set; }
        public string? Direction { get; set; }
        public DateTime? Date_Fetched { get; set; }
        public DateTime? Date_Requested { get; set; }
    }
}
