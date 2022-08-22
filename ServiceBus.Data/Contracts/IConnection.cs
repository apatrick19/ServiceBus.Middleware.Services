using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.Contracts
{
   public  interface IConnection
    {
        //T GetAppConection<T>();

        SqlConnection GetAppConnection();
    }
}
