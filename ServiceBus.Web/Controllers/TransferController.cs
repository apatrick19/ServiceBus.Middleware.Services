using ServiceBus.Core.DataTransferObject;
using ServiceBus.Custom.Contract;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PortalModel;
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
    /// initiate the generic entity controller
    /// </summary>
    [RoutePrefix("transfer")]
    public class TransferController : ApiController
    {
        ITransferBaseService transferService;

        public TransferController(ITransferBaseService transfer)
        {
            transferService = transfer;
        }


        /// <summary>
        /// this is to get name of beneficiary
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpPost]
        [ResponseType(typeof(NameEnquiryResponse))]
        [Route("name-enquiry")]
        public IHttpActionResult GetBeneficiaryName(NameEnquiryRequest request)
        {
            return Ok(transferService.GetBeneficiaryName(request));
        }

        /// <summary>
        /// this is to get name of beneficiary
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(FundTransferResponse))]
        [Route("interbank")]
        public IHttpActionResult InterBank(FundsTransferRequest request)
        {
            return Ok(transferService.InterBank(request));
        }

        /// <summary>
        /// this is to transfer to manny account
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpPost]
        [ResponseType(typeof(LocalTransferResponse))]
        [Route("intrabank")]
        public IHttpActionResult IntraBank(LocalFundTransferRequest request)
        {
            return Ok(transferService.IntraBank(request));
        }


      
        ///// <summary>
        ///// this is to add a beneficiary 
        ///// </summary>
        ///// <returns></returns>
        // [Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("addbeneficiary")]
        //public IHttpActionResult AddBeneficiary(BeneficiaryModel model)
        //{
        //    return Ok(transferService.AddBeneficiary(model));
        //}


        ///// <summary>
        ///// this is to get list of beneficiaries
        ///// </summary>
        ///// <returns></returns>
        // [Authorize]
        //[HttpGet]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("getbeneficiaries")]
        //public IHttpActionResult GetBeneficiaryList(string InitiatorAccountNo, bool IsMannyBeneficiary)
        //{
        //    return Ok(transferService.GetBeneficiary(InitiatorAccountNo, IsMannyBeneficiary));
        //}
    }
}
