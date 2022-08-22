using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts.Service_Contracts
{
   public  interface ITransactionGenericService
    {
        GetTransactionHistoryResponse GetTransaction(GetTransactionHistoryRequest request);

        GetMiniStatementResponse GetMiniStatetment(GetMiniStatementRequest request);
    }
}
