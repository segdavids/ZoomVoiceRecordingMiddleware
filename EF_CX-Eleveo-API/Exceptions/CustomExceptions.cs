namespace Eleveo_EFCX_Connector_API.Exceptions
{
    public class CustomExceptions: Exception
    {
        public CustomExceptions()
        { }
        public CustomExceptions(string message) : base(message)
        {
        }
        public CustomExceptions(string message, Exception inner) : base(message, inner) { }
    }
}
