using ServiceBus.Custom.Contract;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ServiceBus.Web.Controllers
{
    /// <summary>
    /// device controller class 
    /// </summary>
    [RoutePrefix("device")]
    public class DeviceController : ApiController
    {

        public static string ClassName = "DeviceController";
        IAccountService accountService;

        public DeviceController(IAccountService service)
        {
            accountService = service;
        }
        /// <summary>
        /// this is to create an account for a customer 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("update")]
        public IHttpActionResult DeviceUpdate(DeviceUpdate account)
        {
            string method = "CreateAccount";
            LogMachine.LogInformation(ClassName, method, "entered service");
            return Ok(accountService.UpdateDevice(account));
        }
    }
}
