using System.Xml.Serialization;

namespace Eleveo_EFCX_Connector_API.Data
{
    public class DownloadToken
    {
        // using System.Xml.Serialization;
        // XmlSerializer serializer = new XmlSerializer(typeof(Reply));
        // using (StringReader reader = new StringReader(xml))
        // {
        //    var test = (Reply)serializer.Deserialize(reader);
        // }

        [XmlRoot(ElementName = "reply")]
        public class Reply
        {
        }
    }
}
