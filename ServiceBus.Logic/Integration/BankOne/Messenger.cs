using ServicBus.Logic.Contracts;
using ServiceBus.Core;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class Messenger : IMessenger
    {
        IApiPostAndGet apiService;
        public Messenger(IApiPostAndGet service)
        {
            apiService = service;
        }
        string Classname = "Messenger";
        public ResponseModel SendSMS(BankOneSMSRequest request)
        {
            string method = "SendSMS";
            try
            {
                string Url = string.Concat(BaseService.GetAppSetting("SMSBaseUrl"), "Messaging/SaveBulkSms/2?authtoken=",BaseService.GetAppSetting("AuthToken"),"&institutionCode=",BaseService.GetAppSetting("BankOneCode"));
                var response = apiService.UrlPost<BankoneSMSResponse>(Url,request);
                if (response.Status==false)
                {
                    return ResponseDictionary.GetCodeDescription("06", response.ErrorMessage);
                }
                return ResponseDictionary.GetCodeDescription("00", response.ErrorMessage);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }
    }
}
