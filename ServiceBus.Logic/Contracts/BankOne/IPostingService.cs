using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
   public  interface IPostingIntegrationService
    {
        ResponseModel DebitAccount(PostingModel model);
        ResponseModel CreditAccount(PostingModel model);
        ReversalResponse Reversal(ReversalRequest request);
        TransactionQueryResponse TransactionQuery(TransactionQueryRequest request);
    }
}
