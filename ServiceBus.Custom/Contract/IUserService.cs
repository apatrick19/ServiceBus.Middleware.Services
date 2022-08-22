using ServiceBus.Core.Model.Generic;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Contract
{
    public interface IUserService
    {
        ResponseModel CreateUser(User model);
    }
}
