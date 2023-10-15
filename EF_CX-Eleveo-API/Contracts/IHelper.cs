using Eleveo_EFCX_Connector_API.Data;

namespace Eleveo_EFCX_Connector_API.Contracts
{
    public interface IHelper
    {
        EleveoUser GetEleveoUser();
        string GetConnectionString();
        void Logit(string message);
        Env GetEnv();
    }
}
