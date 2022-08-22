using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts.BankOne
{
  public   interface IBankOneAccountCreationIntegration
    {
        AccountCreationResponse CreateNewAccount(Account request);
    }
}
