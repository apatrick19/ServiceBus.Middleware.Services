using ServicBus.Logic.Contracts;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Custom.Model;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMSModel = ServiceBus.Logic.Model.SMSModel;

namespace ServiceBus.Custom.Implementation
{
    public class MessengerService : IMessengerService
    {
        IMessenger messenger;
        public MessengerService(IMessenger messengerService)
        {
            messenger = messengerService;
        }


        

        public ResponseModel SendSMS(SMSRequestModel requestModel)
        {
            try
            {
                //pass the model
                var request = new BankOneSMSRequest();
                var smsModel = new Logic.Model.SMSModel() {
                    AccountId = "",
                    AccountNumber = requestModel.AccountNo,
                    Body = requestModel.MessageToSend,
                    ReferenceNo = requestModel.TrackingNo,
                    To = requestModel.RecipientMobileNo                     
                 };               
                request.SMSModels.Add(smsModel);
                return messenger.SendSMS(request);
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06","System Error");
            }
        }
    }
}
