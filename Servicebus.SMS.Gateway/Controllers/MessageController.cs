using Servicebus.SMS.Gateway.Models;
using Servicebus.SMS.Gateway.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Servicebus.SMS.Gateway.Controllers
{
    [RoutePrefix("messaging")]
    public class MessageController : ApiController
    {

        string ClassName = "MessageController";

        /// <summary>
        /// this is to send an sms
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("sendsms")]
        public IHttpActionResult SendSMS(SMSRequest request)
        {
            string method = "SendSMS";
            Trace.TraceInformation(ClassName+ method+ "entered the send sms controller service");
            return Ok(MessageService.SendSMSTermii(request));
        }
    }
}
