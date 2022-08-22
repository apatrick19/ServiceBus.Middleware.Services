using ServiceBus.Custom.Contract;
using ServiceBus.Logic.Implementations;
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
    /// cards controller
    /// </summary>
    [RoutePrefix("cards")]
    public class CardsController : ApiController
    {
        ICardService cardService;
        public CardsController(ICardService service)
        {
            cardService = service;
        }

        /// <summary>
        /// this is to get card delivery option 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("cardsDeliveryOption")]
        public IHttpActionResult GetDeliveryOption()
        {
            return Ok(cardService.GetCardDeliveryOptions());
        }

        /// <summary>
        /// this is to get card configration for the bank 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(ResponseModel))]
        [Route("getCardsConfiguration")]
        public IHttpActionResult RetrieveInstitutionConfig()
        {
            return Ok(cardService.RetrieveInstitutionConfig());
        }

        /// <summary>
        /// this is to get request for a card
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("cardRequest")]
        public IHttpActionResult CardRequest(CardRequestModel request)
        {
            return Ok(cardService.CardRequest(request));
        }


        /// <summary>
        /// this is to get a customer cards 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("getCustomerCards")]
        public IHttpActionResult GetCustomerCards(CustomerCardRequestModel request)
        {
            return Ok(cardService.GetCustomerCards(request));
        }


        /// <summary>
        /// this is to hotlist cards 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("hotlistCard")]
        public IHttpActionResult HotlistCard(HotlistRequestModel request)
        {
            return Ok(cardService.HotlistCard(request));
        }
    }
}
