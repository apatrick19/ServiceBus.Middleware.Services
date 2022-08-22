using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Web.Models
{
    public class ApiHelper
    {
       static string ClassName = "ApiHelper";
        public static string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public static ResponseModel TrackIP() {
            string method = "TrackIP";            
            try
            {
                string IPAddress = GetIp();
                if (string.IsNullOrEmpty(IPAddress))
                {
                    return ResponseDictionary.GetCodeDescription("06", "invalid Ip addr in header");
                }
                using (AiroPayContext context =new AiroPayContext())
                {
                    var IP = context.IPAddresses.Where(x => x.IP == IPAddress).FirstOrDefault();
                    if (IP==null)
                    {
                        return ResponseDictionary.GetCodeDescription("00", "ip addr not profiled for transaction please contact admin");
                      //  return ResponseDictionary.GetCodeDescription("04", "ip addr not profiled for transaction please contact admin");
                    }
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
               
                LogMachine.LogInformation(ClassName, method, "entered the create account endpoint about calling the account creation base service");
                return ResponseDictionary.GetCodeDescription("00", "an error occurred, please contact admin");
            }
        }
    }
}