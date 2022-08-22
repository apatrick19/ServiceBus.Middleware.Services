using ServiceBus.Web.Models;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Custom.Contract;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ServiceBus.Logic.Model;

namespace ServiceBus.Web.Controllers
{
    /// <summary>
    /// bills payment controller class for managing cable tv, water, electicity payment
    /// </summary>
    [RoutePrefix("billspayment")]
    public class BillPaymentController : ApiController
    {
        IBillingService billingService;
        public BillPaymentController(IBillingService billing)
        {
            billingService = billing;
        }


        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("getcategory")]
        public IHttpActionResult GetBillingCategory()
        {
            var IPcheck = ApiHelper.TrackIP();
            if (IPcheck.ResponseCode != "00")
            {
                return Ok(ResponseDictionary.GetCodeDescription("105", "Ip address not registered, please contact admin"));
            }        
            return Ok(billingService.GetBillingCategory());
        }

        /// <summary>
        /// this is to get all merchants under a category (dstv under cable, IKEDC under electricity)
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("getbillers")]
        public IHttpActionResult GetBillers(string CategoryCode)
        {
            var IPcheck = ApiHelper.TrackIP();
            if (IPcheck.ResponseCode != "00")
            {
                return Ok(ResponseDictionary.GetCodeDescription("105", "Ip address not registered, please contact admin"));
            }           
            return Ok(billingService.GetBillingMerchants(CategoryCode));
        }

        /// <summary>
        /// this is to get all products under a merchant(dstv, gotv)
        /// </summary>
        /// <returns></returns>
           [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("getproducts")]
        public IHttpActionResult GetProductsByMerchant(string billercode)
        {
            var IPcheck = ApiHelper.TrackIP();
            if (IPcheck.ResponseCode != "00")
            {
                return Ok(ResponseDictionary.GetCodeDescription("105", "Ip address not registered, please contact admin"));
            }           
            return Ok(billingService.GetPaymentItems(billercode));
        }

        /// <summary>
        /// this is to get all cable menus ()
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("validatecustomer")]
        public IHttpActionResult ValidateCustomer(CustomerValidation request)
        {
            var IPcheck = ApiHelper.TrackIP();
            if (IPcheck.ResponseCode != "00")
            {
                return Ok(ResponseDictionary.GetCodeDescription("105", "Ip address not registered, please contact admin"));
            }
            return Ok(billingService.CustomerValidation(request));
        }

         [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("payment")]
        public IHttpActionResult CablePayment(BillsPaymentRequest requestModel)
        {
            var IPcheck = ApiHelper.TrackIP();
            if (IPcheck.ResponseCode != "00")
            {
                return Ok(ResponseDictionary.GetCodeDescription("105", "Ip address not registered, please contact admin"));
            }          
            return Ok(billingService.SendPayment(requestModel));
        }
    }
}
