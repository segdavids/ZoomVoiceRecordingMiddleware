using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eleveo_EFCX_Connector_API.Data
{
    public class JSession
    {
        [Key]
        public string conversationId { get; set; }
        public List<object>? previousIds { get; set; }
        public List<Event>? events { get; set; }
        public DateTime? start { get; set; }
        public string? duration { get; set; }
        public string? direction { get; set; }
        public List<object>? communicationTypes { get; set; }
        public List<object>? participants { get; set; }
    }
}
