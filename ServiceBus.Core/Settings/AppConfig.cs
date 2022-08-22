using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ServiceBus.Core.Settings
{
    public static class AppConfig
    {
        public static string NIPBaseUrl = ConfigurationManager.AppSettings["NIPBaseUrl"].ToString();
        public static string QuicktellerBaseUrl = ConfigurationManager.AppSettings["QuicktellerBaseUrl"].ToString();
        public static string CoreBankingBaseUrl = ConfigurationManager.AppSettings["CoreBankingBaseUrl"].ToString();
        public static string HashKey = ConfigurationManager.AppSettings["HashKey"].ToString();
        public static string EmailTemplate = ConfigurationManager.AppSettings["EmailTemplate"].ToString();
        public static string ServiceContext = ConfigurationManager.ConnectionStrings["ServiceContext"].ConnectionString;
        public static string AuthUsername = ConfigurationManager.AppSettings["AuthUsername"].ToString();
        public static string AuthPassword = ConfigurationManager.AppSettings["AuthPassword"].ToString();
        public static string AppMode = ConfigurationManager.AppSettings["AppMode"].ToString();
        public static string BranchId = ConfigurationManager.AppSettings["BranchId"].ToString();
        public static string AuthKey = ConfigurationManager.AppSettings["AuthKey"].ToString();
        public static string OfferingId = ConfigurationManager.AppSettings["OfferingId"].ToString();
        public static decimal IntraFee = decimal.Parse(ConfigurationManager.AppSettings["IntraFee"].ToString());
        public static decimal DefaultFee = decimal.Parse(ConfigurationManager.AppSettings["DefaultFee"].ToString());

    }
}
