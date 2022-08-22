using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
    public interface IAccountCreationService
    {
        ResponseModel CreateNewAccount(Account account);
    }
}
