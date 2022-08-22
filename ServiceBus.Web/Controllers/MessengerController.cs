using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Custom.Model;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace ServiceBus.API.Controllers
{
   // [EnableCors(origins: "*", headers: "*", methods: "*")]
    /// <summary>
    /// This controller handles sms processes
    /// </summary>
    [RoutePrefix("messenger")]
    public class MessengerController : ApiController
    {
        IMessengerService messenger;

        /// <summary>
        /// intiializing the messenger controller
        /// </summary>
        /// <param name="service"></param>
        public MessengerController(IMessengerService service)
        {
            messenger = service;
        }

        /// <summary>
        /// endpoint to send sms messages to users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("sms/send")]
        public IHttpActionResult SendSMS(SMSRequestModel model)
        {
            return Ok(messenger.SendSMS(model));
        }

      

      
    }
}
