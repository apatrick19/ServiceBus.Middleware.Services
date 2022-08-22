using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Model;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Contract
{
    public interface IMessengerService
    {
        ResponseModel SendSMS(SMSRequestModel model);

       // ResponseModel LogSMS(SMSModel model);

        //ResponseModel SendEmail(EmailModel model);

       // ResponseModel LogMail(EmailModel model);

      //  ResponseModel SMS_Status(string Smskey);


    }
}
