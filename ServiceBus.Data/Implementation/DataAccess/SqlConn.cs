using ServiceBus.Core;
using ServiceBus.Core.Settings;
using ServiceBus.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.Implementation.DataAccess
{
    public class SqlConn:IConnection
    {        
        public SqlConnection GetAppConnection()
        {
            return new SqlConnection(BaseService.GetConnectionString("ServiceContext"));
        }

        public static SqlConnection GetAppConnection(string key)
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[key].ConnectionString);
        }

        public SqlConn()
        {

        }
        
    }
}
