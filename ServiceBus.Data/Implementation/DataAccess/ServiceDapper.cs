using Dapper;
using ServiceBus.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.Implementation.DataAccess
{
    public class ServiceDapper : SqlConn, IServiceDapper
    {

        IConnection connection;
        public ServiceDapper(IConnection conn)
        {
            this.connection = conn;
        }

        public ServiceDapper()
        {

        }

        public IEnumerable<T> GetAllRecords<T>()
        {
            try
            {
                using (IDbConnection db = GetAppConnection())
                {
                    db.Open();
                    return db.Query<T>($"Select * From {typeof(T).Name}");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}; {ex.Message}; {ex?.InnerException}; {ex?.StackTrace}");
                throw;
            }
        }

        public IEnumerable<T> GetAllRecordsByKey<T>(string key, string value)
        {
            try
            {
                using (IDbConnection db = GetAppConnection())
                {
                    db.Open();
                    return db.Query<T>($"Select * From {typeof(T).Name} where {key} = {value}");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}; {ex.Message}; {ex?.InnerException}; {ex?.StackTrace}");
                throw;
            }
        }

        public IEnumerable<T> GetAllGenericEnitities<T>()
        {
            try
            {
                using (IDbConnection db = GetAppConnection())
                {
                    db.Open();
                    return db.Query<T>($"Select Name, Reference From {typeof(T).Name}");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}; {ex.Message}; {ex?.InnerException}; {ex?.StackTrace}");
                throw;
            }
        }
        
        public IEnumerable<T> GetAllRecordsByCount<T>(int count)
        {
            try
            {
                using (IDbConnection db = connection.GetAppConnection())
                {
                    db.Open();
                    return db.Query<T>($"Select top {(count > 0 ? count : 10)} * From {typeof(T).Name}  ");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}; {ex.Message}; {ex?.InnerException}; {ex?.StackTrace}");
                throw ex;
            }
        }

        public IEnumerable<T> GetAllRecordsByKeys<T>(string Status)
        {
            try
            {
                try
                {
                    using (IDbConnection db = connection.GetAppConnection())
                    {
                        db.Open();
                        return db.Query<T>($"Select top * From {typeof(T).Name} where ");
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation($"An error occurred {ex}; {ex.Message}; {ex?.InnerException}; {ex?.StackTrace}");
                    throw ex;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<T> GetAllTableSchema<T>()
        {
            try
            {
                using (IDbConnection db = GetAppConnection())
                {
                    db.Open();
                    return db.Query<T>($"SELECT 	TABLE_NAME as Name,count (COLUMN_NAME) Attributes, 'ID' as UniqueID FROM INFORMATION_SCHEMA.COLUMNS	group by TABLE_NAME	order by TABLE_NAME").ToList();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}; {ex.Message}; {ex?.InnerException}; {ex?.StackTrace}");
                throw ex;
            }
        }


    }
}
