using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ServiceBus.Web.Controllers
{
    public class PaymentController : ApiController
    {
        /// <summary>
        /// this is to deactivate pnd
        /// </summary>
        /// <returns></returns>
       
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("Process")]
        public IHttpActionResult Process(object request)
        {           
            return Ok(ResponseDictionary.GetCodeDescription("00","Acknowledged"));
        }
    }
}
