using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts.BankOne
{
    public interface IBankOneTransactionIntegration
    {
        GetTransactionHistoryResponse GetTransaction(GetTransactionHistoryRequest request);

        GetMiniStatementResponse GetMiniStatement(GetMiniStatementRequest request);
    }
}
