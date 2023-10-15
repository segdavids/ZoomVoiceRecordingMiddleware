namespace Eleveo_EFCX_Connector_API.Exceptions
{
    public class ApiError: Exception
    {
        public ApiError()
        { }
        public ApiError(string message) : base(message)
        {
        }
        public ApiError(string message, Exception inner) : base(message, inner) { }
    }
}
