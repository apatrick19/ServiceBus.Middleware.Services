using ServiceBus.Core.Model.Generic;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http;

namespace ServiceBus.Custom.Contract
{
   public interface ITransactionService
    {
        ResponseModel CreditAccount(PostingModel request);
        ResponseModel DebitAccount(PostingModel request);
        ReversalResponse Reversal(ReversalRequest request);
        TransactionQueryResponse TransactionQuery(TransactionQueryRequest request);


    }
}
