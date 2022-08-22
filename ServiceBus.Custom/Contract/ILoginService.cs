using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Contract
{
   public interface ILoginService
    {
        ResponseModel AuthenticateUser(string username, string password);
        ResponseModel AuthenticateAccount(string username, string password,string DeviceId);

        ResponseModel ChangePassword(ChangePassword model);

        ResponseModel ChangePin(ChangePinModel model);

        ResponseModel ForgotPassword(ForgotPasswordModel model);
        ResponseModel PasswordUpdate(PasswordUpdateModel model);

        ResponseModel RefreshAccount(string accountNo);
    }
}
