using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Custom.Contract;
using ServiceBus.Logic.Contracts.Service_Contracts;
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
    /// initiate the transaction entity controller
    /// </summary>
    [RoutePrefix("transaction")]
    public class TransactionController : ApiController
    {
        IAccountService accountService;
        ITransactionService transactionService;
        ITransactionGenericService transactionGenericService;
        public TransactionController(IAccountService service, ITransactionService tService, ITransactionGenericService transactionGeneric)
        {
            accountService = service;
            transactionService = tService;
            this.transactionGenericService = transactionGeneric;
        }
        /// <summary>
        /// this is to get all transaction in the database
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpPost]
        [ResponseType(typeof(GetTransactionHistoryResponse))]
        [Route("get-transaction-history-by-date-range")]
        public IHttpActionResult GetTransaction(GetTransactionHistoryRequest request)
        {            
            return Ok(transactionGenericService.GetTransaction(request));
        }


        /// <summary>
        /// this is to get mini statement 
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpPost]
        [ResponseType(typeof(GetMiniStatementResponse))]
        [Route("mini-statement")]
        public IHttpActionResult GetMinistatement(GetMiniStatementRequest request)
        {
            return Ok(transactionGenericService.GetMiniStatetment(request));
        }


        /// <summary>
        /// this is to get all transaction in the database
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpPost]
        [ResponseType(typeof(StatementBaseResponse))]
        [Route("get-statement-report-via-email")]
        public IHttpActionResult GetStatement(TransactionRequest request)
        {
            return Ok(accountService.GetStatement(request));
        }


        /// <summary>
        /// this is to get all transaction in the database
        /// </summary>
        /// <returns></returns>
         [Authorize]
        [HttpPost]
        [ResponseType(typeof(TransactionQueryResponse))]
        [Route("transactionStatusQuery")]
        public IHttpActionResult TransactionQuery(TransactionQueryRequest request)
        {
            return Ok(transactionService.TransactionQuery(request));
        }

        /// <summary>
        /// this is to debit a customer
        /// </summary>
        /// <returns></returns>
       // [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("debit")]
        public IHttpActionResult Debit(PostingModel request)
        {
            return Ok(transactionService.DebitAccount(request));
        }

        /// <summary>
        /// this is to credit a customer
        /// </summary>
        /// <returns></returns>
       // [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("credit")]
        public IHttpActionResult Credit(PostingModel request)
        {
            return Ok(transactionService.CreditAccount(request));
        }

        /// <summary>
        /// this is to perform reversal
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ReversalResponse))]
        [Route("reversal")]
        public IHttpActionResult Reversal(ReversalRequest request)
        {
            return Ok(transactionService.Reversal(request));
        }
    }
}
