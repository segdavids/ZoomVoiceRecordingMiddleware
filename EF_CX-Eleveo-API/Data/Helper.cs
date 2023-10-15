using Eleveo_EFCX_Connector_API.Exceptions;

namespace Eleveo_EFCX_Connector_API.Data
{
    public class Helper
    {
        public static string GetConnectionString()
        {
            var connstr = "";
            try
            {
                //DEV
                //var host = ".\\SQLEXPRESS";// Environment.GetEnvironmentVariable("DB_HOST");
                //var dbname = "EleveoConnector";// Environment.GetEnvironmentVariable("DB_NAME");
                //var dbusername = "sa"; // Environment.GetEnvironmentVariable("DB_USERNAME");
                //var dbpw = "password1$"; // Environment.GetEnvironmentVariable("DB_PASSWORD");
                //var dbport = "1433"; /// Environment.GetEnvironmentVariable("DB_PORT");

                //PROD
                var host = Environment.GetEnvironmentVariable("DB_HOST");
                var dbname = Environment.GetEnvironmentVariable("DB_NAME");
                var dbusername = Environment.GetEnvironmentVariable("DB_USERNAME");
                var dbpw = Environment.GetEnvironmentVariable("DB_PASSWORD");
                var dbport = Environment.GetEnvironmentVariable("DB_PORT");

                if ((host is null) && (dbname is null) && (dbusername is null) && (dbpw is null) && (dbport is null))
                {
                    throw new CustomExceptions($"Env Variables not found. Please check config file");
                }
                else
                {
                    connstr = $"Data Source={host};Initial Catalog={dbname};Integrated Security=false;TrustServerCertificate=True;persist security info=True; User ID={dbusername};pwd={dbpw}; MultipleActiveResultSets=true";
                }
            }
            catch (CustomExceptions e)
            {
                Console.WriteLine(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return connstr;
        }

        public static string gethost()
        {
            string hosturl = string.Empty;
            //DEV
            hosturl = "http://*:90;http://localhost:90;https://hostname:90"; // zoomcallrec1234"; // Environment.GetEnvironmentVariable("ELEVEO_PW");

            //PROD
            //hosturl = Environment.GetEnvironmentVariable("DOMAINS");

            return hosturl;
        }
    }
    
}
