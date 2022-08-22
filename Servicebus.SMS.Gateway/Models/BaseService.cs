using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.SMS.Gateway.Models
{
    public class BaseService
    {
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        //public static IDatabase SqlConnection()
        //{
        //    string connectionString = GetConnectionString("RIAConnection");
        //    return new Database(connectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance);
        //}
    }
}
