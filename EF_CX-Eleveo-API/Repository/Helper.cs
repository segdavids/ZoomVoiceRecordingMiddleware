using Eleveo_EFCX_Connector_API.Contracts;
using Eleveo_EFCX_Connector_API.Data;

namespace Eleveo_EFCX_Connector_API.Repository
{
    public class Helper:IHelper
    {
        public string GetConnectionString()
        {
            //DEV
            var connstr = string.Empty;
            //var host = ".\\SQLEXPRESS";// Environment.GetEnvironmentVariable("DB_HOST");
            //var dbname = "EleveoConnector";// Environment.GetEnvironmentVariable("DB_NAME");
            //var dbusername = "sa"; // Environment.GetEnvironmentVariable("DB_USERNAME");
            //var dbpw = "password1$"; // Environment.GetEnvironmentVariable("DB_PASSWORD");
            //var dbport = "1433"; /// Environment.GetEnvironmentVariable("DB_PORT");
            //var part = "Integrated Security=SSPI; persist security info=True;TrustServerCertificate=True; MultipleActiveResultSets=true";

            //PROD
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var dbname = Environment.GetEnvironmentVariable("DB_NAME");
            var dbusername = Environment.GetEnvironmentVariable("DB_USERNAME");
            var dbpw = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbport = Environment.GetEnvironmentVariable("DB_PORT");
            var part = Environment.GetEnvironmentVariable("DB_DRIVER");

            if ((host is null) && (dbname is null) && (dbusername is null) && (dbpw is null) && (dbport is null))
            {
                //connstr = $"Data Source={host};Initial Catalog={dbname};Integrated Security=false;persist security info=True;TrustServerCertificate=True; User ID={dbusername};pwd={dbpw}; MultipleActiveResultSets=true";
                connstr = $"Data Source={host},{dbport};Initial Catalog={dbname};{part}";
            }
            return connstr;
        }

        public EleveoUser GetEleveoUser()
        {
            var user = new EleveoUser();

            //DEV
            //var elwveoun = "david"; // Environment.GetEnvironmentVariable("ELEVEO_USERNAME");
            //var eleveopw = "zoomcallrec1234"; // Environment.GetEnvironmentVariable("ELEVEO_PW");



            //PROD
            var elwveoun = Environment.GetEnvironmentVariable("ELEVEO_USERNAME");
            var eleveopw = Environment.GetEnvironmentVariable("ELEVEO_PW");
            user.password = eleveopw;
            user.username = elwveoun;

            return user;
        }

       

        public Env GetEnv()
        {
            //DEV 
            //var eleveoAuthURL = "http://192.168.1.237";//  Environment.GetEnvironmentVariable("ELEVEO_AUTHENTICATION_URL");
            //var eleveoSearchURL = "http://192.168.1.237";// Environment.GetEnvironmentVariable("ELEVEO_RECORDERQM_URL");
            //var interval = 10; // Environment.GetEnvironmentVariable("INTERVAL");
            //var timeZone = "CET";
            //PROD
            var eleveoAuthURL = Environment.GetEnvironmentVariable("ELEVEO_AUTHENTICATION_URL");
            var eleveoSearchURL = Environment.GetEnvironmentVariable("ELEVEO_RECORDERQM_URL");
            var interval = Environment.GetEnvironmentVariable("INTERVAL");
            var timeZone = Environment.GetEnvironmentVariable("TIMEZONE");
            var env = new Env();
            try
            {
                env.eleveoAuthURL = eleveoAuthURL;
                env.eleveoSearchURL = eleveoSearchURL;
                env.interval = Convert.ToInt32(interval);
                env.timeZone = timeZone;
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
            return env;
        }

        public void Logit(string message)
        {
            string logpath = Environment.GetEnvironmentVariable("LOG_PATH");//"C:\\EF\\"; //
            var timezonetime = DateTime.Now; ///.ToUniversalTime(); //.AddHours(2);
            string reportpath = $"{logpath}{timezonetime:dd_MM_yyyy}_log.txt";
            try
            {
                Console.WriteLine($"{timezonetime}: {message}");
                if (!string.IsNullOrEmpty(logpath))
                {
                    using (StreamWriter logitwriter = new(reportpath, true))
                    {
                        logitwriter.WriteLine($"{timezonetime}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{timezonetime}: {ex.Message}");
            }
        }

    }
}
