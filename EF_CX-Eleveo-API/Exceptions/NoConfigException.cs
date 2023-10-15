namespace Eleveo_EFCX_Connector_API.Exceptions
{
    public class NoConfigException:Exception
    {
        public NoConfigException()
        {}
        public NoConfigException(string message): base(message)
        {      
        }
        public NoConfigException(string message, Exception inner): base(message, inner) { }

    }
}
