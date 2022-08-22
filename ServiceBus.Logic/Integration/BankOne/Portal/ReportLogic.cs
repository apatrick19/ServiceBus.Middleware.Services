using ServiceBus.Data.Implementation.DataAccess;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration.Portal
{
   public  class ReportLogic
    {
        public static List<DataPoint> GetReferalReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select ReferralName as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by ReferralName", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetAccountOfficerReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select AccountOfficerCode as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by AccountOfficerCode", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetProductReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select ProductCode as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by ProductCode", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetTierReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select AccountTier as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by AccountTier", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetAccountStatusReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select StatusName as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by StatusName", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetDeviceUsageReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select DeviceName as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by DeviceName", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetGenderReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("select Gender as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by Gender", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    foreach (var item in DataPointList)
                    {
                        if (item.Label=="0")
                        {
                            item.Label = "Male";
                        }
                        else
                        {
                            item.Label = "Female";
                        }
                    }
                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetAccountByMonthReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                //SqlCommand cmd = new SqlCommand("select FORMAT(DateCreated, 'MMMM') as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by FORMAT(DateCreated, 'MMMM') order by label", conn);
                SqlCommand cmd = new SqlCommand("select MONTH(DateCreated) as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by MONTH(DateCreated) ", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    foreach (var item in DataPointList)
                    {
                        item.Label= CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(item.Label));
                    }
                    return DataPointList;
                }
            }
        }


        public static List<DataPoint> GetBillsPaymentTopCustomersReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                //SqlCommand cmd = new SqlCommand("select FORMAT(DateCreated, 'MMMM') as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by FORMAT(DateCreated, 'MMMM') order by label", conn);
                SqlCommand cmd = new SqlCommand("select SourceAccount as Label, sum(Amount) as Y from [AiroPay].[dbo].[BillsPaymentTransaction] group by SourceAccount order by Y desc", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    foreach (var item in DataPointList)
                    {
                        item.Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(item.Label));
                    }
                    return DataPointList;
                }
            }
        }

        public static List<DataPoint> GetBillsPaymentSuccessRateReports()
        {
            List<DataPoint> DataPointList = new List<DataPoint>();
            using (var conn = SqlConn.GetAppConnection("AiroPayContext"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                //SqlCommand cmd = new SqlCommand("select FORMAT(DateCreated, 'MMMM') as label, count(ID) as Y from [AiroPay].[dbo].[Account] group by FORMAT(DateCreated, 'MMMM') order by label", conn);
                SqlCommand cmd = new SqlCommand("select Status as Label, count(ID) as Y from [AiroPay].[dbo].[BillsPaymentTransaction] group by Status order by Y desc", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.Text;

                // 3. add parameter to command, which will be passed to the stored procedure

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        var account = new DataPoint();
                        account.Label = rdr[0].ToString();
                        account.Y = Convert.ToInt32(rdr[1].ToString());
                        DataPointList.Add(account);
                    }

                    foreach (var item in DataPointList)
                    {
                        item.Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(item.Label));
                    }
                    return DataPointList;
                }
            }
        }
    }
}
