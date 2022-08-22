using ServiceBus.Core.Model.CRM;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace ServiceBus.Web.Controllers
{
    /// <summary>
    /// initiate the generic entity controller
    /// </summary>
    [RoutePrefix("generic/entity")]

    public class GenericEntityController : ApiController
    {
        IGenericBaseService genericBaseService;

        /// <summary>
        /// initiate the generic base service
        /// </summary>
        /// <param name="generic"></param>
        public GenericEntityController(IGenericBaseService generic)
        {
            genericBaseService = generic;
        }


        /// <summary>
        /// this is to get all gender in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("raisedispute")]
        public IHttpActionResult LogComplaints(Complaints issue)
        {
            return Ok(genericBaseService.LogComplaints(issue));
        }

        /// <summary>
        /// this is to get all gender in the database
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("gender/getall")]
        public IHttpActionResult GetAllGender()
        {
            return Ok(genericBaseService.GetAllGender());
        }


        /// <summary>
        /// this is to get all states in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("state/getall")]
        public IHttpActionResult GetAllState()
        {
            return Ok(genericBaseService.GetAllState());
        }

        /// <summary>
        /// this is to get all countries in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("country/getall")]
        public IHttpActionResult GetAllCountry()
        {
            return Ok(genericBaseService.GetAllCountry());
        }


        /// <summary>
        /// this is to get all countries in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("response/getall")]
        public IHttpActionResult GetAllResponse()
        {
            return Ok(genericBaseService.GetAllResponseDecription());
        }




        /// <summary>
        /// this is to get all local government areas by state in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("lga/getbystate")]
        public IHttpActionResult GetLGAByState(string stateCode)
        {
            return Ok(genericBaseService.GetLGAByState(stateCode));
        }

        /// <summary>
        /// this is to get all local government areas 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("lga/getall")]
        public IHttpActionResult GetAllLGA()
        {
            return Ok(genericBaseService.GetLGA());
        }

        /// <summary>
        /// this is to get all titles in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("title/getall")]
        public IHttpActionResult GetAllTitle()
        {
            return Ok(genericBaseService.GetAllTitle());
        }

        /// <summary>
        /// this is to get all marital statuses in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("maritalstatus/getall")]
        public IHttpActionResult GetAllMaritalStatus()
        {
            return Ok(genericBaseService.GetAllMaritalStatus());
        }

        /// <summary>
        /// this is to get all relationships in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("relationship/getall")]
        public IHttpActionResult GetAllRelationship()
        {
            return Ok(genericBaseService.GetAllRelationship());
        }

        /// <summary>
        /// this is to get all banks in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("bank/getall")]
        public IHttpActionResult GetAllBank()
        {
            return Ok(genericBaseService.GetAllBank());
        }

        /// <summary>
        /// this is to get all business branches in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("branch/getbystate")]
        public IHttpActionResult GetBranchByState(string StateCode)
        {
            return Ok(genericBaseService.GetBranchByState(StateCode));
        }

       
        /// <summary>
        /// this is to get all statement options in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("statementoption/getall")]
        public IHttpActionResult GetAllStatementOption()
        {
            return Ok(genericBaseService.GetAllStatementOption());
        }

        /// <summary>
        /// this is to get all proof of identity options in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("proofofidentity/getall")]
        public IHttpActionResult GetAllProofOfIdentity()
        {
            return Ok(genericBaseService.GetAllProofOfIdentity());
        }

      
        /// <summary>
        /// This is to get all nationalities in the database
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("nationality/getall")]
        public IHttpActionResult GetAllNationality()
        {
            return Ok(genericBaseService.GetAllNationality());
        }


        /// <summary>
        /// This is to get faqs
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("faqs")]
        public IHttpActionResult GetFAQS()
        {
            return Ok(genericBaseService.GetFAQs());
        }


        /// <summary>
        /// This is to get faqs
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("getbankone/products")]
        public IHttpActionResult GetBankOneProducts()
        {
            return Ok(genericBaseService.GetProducts());
        }


    }
}
