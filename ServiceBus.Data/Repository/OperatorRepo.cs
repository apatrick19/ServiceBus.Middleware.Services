using Dapper;
using ServiceBus.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.Repository
{
    public class OperatorRepo
    {
        public static Operator GetOperatorById (string OperatorId)
        {
           
            try
            {
                using (IDbConnection db = BaseConnection.GetAppConnection())
                {
                    db.Open();
                    string query = "Select * From Operator where OperatorId = @OperatorId";
                    return db.Query<Operator>(query, new { OperatorId = OperatorId }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
