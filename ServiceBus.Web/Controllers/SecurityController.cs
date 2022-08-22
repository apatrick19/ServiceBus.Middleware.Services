//using ServiceBus.Core.DataTransferObject;
//using ServiceBus.Logic.Service;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.Http.Description;

//namespace ServiceBus.Web.Controllers
//{

//    /// <summary>
//    /// security controller class for managing authentication
//    /// </summary>
//    [RoutePrefix("security")]
//    public class SecurityController : ApiController
//    {

//        string className = "SecurityController";


//        /// <summary>
//        /// this is to create token foe authorization 
//        /// </summary>
//        /// <returns></returns>
//        [Authorize]
//        [HttpPost]
//        [ResponseType(typeof(object))]
//        [Route("create-token")]
//        public IHttpActionResult GenerateToken(SecurityRequest request)
//        {
//            string methodName = "GenerateToken";
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                LogService.LogError(request.OperatorId, className, methodName, ex);
//            }
//        }
//    }
//}
