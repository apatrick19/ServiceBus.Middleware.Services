using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Custom.Contract;
using ServiceBus.Logic.Contracts.Service_Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace ServiceBus.Web.Controllers
{
    /// <summary>
    /// account controller class for managing account creation, login , user settings etc
    /// </summary>
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        string ClassName = "AccountController";
        IAccountService accountService;
        ILoginService loginService;
        IAccountGenericService accountGenericService;

        public AccountController(IAccountService service, ILoginService login, IAccountGenericService accountGenericService)
        {
            accountService = service;
            loginService = login;
            this.accountGenericService = accountGenericService;
        }

        /// <summary>
        /// this is to create an account for a customer 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(GetAccountBalanceResponse))]
        [Route("getbalance")]
        public IHttpActionResult GetBalance(GetAccountBalanceRequest request)
        {
            string method = "GetBalance";
            LogMachine.LogInformation(ClassName, method, "entered the create get balance service");
            return Ok(accountGenericService.GetBalance(request));
        }

        /// <summary>
        /// this is to create an account for a customer 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(AccountCreationResponse))]
        [Route("create")]
        public IHttpActionResult CreateAccount(AccountRequest account)
        {
            string method = "CreateAccount";
            LogMachine.LogInformation(ClassName,method,"entered the create account endpoint about calling the account creation base service");
            return Ok(accountGenericService.CreateAccount(account));
        }
              

        /// <summary>
        /// this is to register an exisitng account 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(BVNValidationResponse))]
        [Route("bvnvalidation")]
        public IHttpActionResult ValidateBVN(BVNValidationRequest request)
        {
            string method = "ValidateBVN";
            LogMachine.LogInformation(ClassName, method, "BVN Validation service");
            return Ok(accountGenericService.ValidateBVN(request));
        }


        /// <summary>
        /// this is to fetch account details  
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(GetAccountByAccountNoResponse))]
        [Route("getaccountbyaccountno")]
        public IHttpActionResult GetAccountByAccountNo(GetAccountByAccountNoRequest request)
        {
            string method = "GetAccountByAccountNo";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountGenericService.GetAccountByAccountNo(request));
        }

        /// <summary>
        /// this is to fetch account details  
        /// </summary>
        /// <returns></returns>
      //  [Authorize]
        [HttpPost]
        [ResponseType(typeof(AccountEnquiryResponse))]
        [Route("accountenquiry")]
        public IHttpActionResult AccountEnquiry(GetAccountByAccountNoRequest request)
        {
            string method = "GetAccountByAccountNo";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountGenericService.AccountEnquiry(request));
        }


        #region No inuse
        /// <summary>
        /// this is to register an exisitng account 
        /// </summary>
        /// <returns></returns>
        // [Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("existing/register")]
        //public IHttpActionResult RegisterExistingAccount(ExistingAccountModel account)
        //{
        //    string method = "RegisterExistingAccount";
        //    LogMachine.LogInformation(ClassName, method, "entered the register existing account endpoint about calling the account creation base service");
        //    return Ok(accountService.RegisterExistingAccount(account));
        //}


        /// <summary>
        /// this is to create an account for a customer 
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("login")]
        //public IHttpActionResult Login(LoginModel model)
        //{
        //    string methodname = "Login";
        //    LogMachine.LogInformation(ClassName, methodname, $"entered login method");
        //    return Ok(loginService.AuthenticateAccount(model.Username,model.Password, model.DeviceId));
        //}



        ///// <summary>
        ///// this is to create an account for a customer 
        ///// </summary>
        ///// <returns></returns>
        //// [Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("refreshaccount")]
        //public IHttpActionResult AccountReferesh(string AccountNo)
        //{
        //    string methodname = "AccountReferesh";
        //    LogMachine.LogInformation(ClassName, methodname, $"entered login method");
        //    return Ok(loginService.RefreshAccount(AccountNo));
        //}




        /// <summary>
        /// this is to change password
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("changePassword")]
        //public IHttpActionResult ChangePassword(ChangePassword model)
        //{
        //    return Ok(loginService.ChangePassword(model));
        //}

        /// <summary>
        /// this is to change pin
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("changePin")]
        //public IHttpActionResult ChangePIN(ChangePinModel model)
        //{
        //    return Ok(loginService.ChangePin(model));
        //}

        /// <summary>
        /// this is to test forgot password 
        /// </summary>
        /// <returns></returns>

        //[Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("forgotPassword")]
        //public IHttpActionResult ForgotPassword(ForgotPasswordModel model)
        //{
        //    return Ok(loginService.ForgotPassword(model));
        //}

        /// <summary>
        /// this is to test forgot password 
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[ResponseType(typeof(ResponseModel))]
        //[Route("passwordUpdate")]
        //public IHttpActionResult PasswordUpdate(PasswordUpdateModel model)
        //{
        //    return Ok(loginService.PasswordUpdate(model));
        //}
        #endregion


        /// <summary>
        /// this is to freeze an account   
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("freezeAccount")]
        public IHttpActionResult FreezeAccount(FreezeAccountRequest request)
        {
            string method = "FreezeAccount";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.FreezeAccount(request));
        }


        /// <summary>
        /// this is to unfreeze an account   
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("un-FreezeAccount")]
        public IHttpActionResult UnFreezeAccount(FreezeAccountRequest request)
        {
            string method = "FreezeAccount";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.UnFreezeAccount(request));
        }

        /// <summary>
        /// this is to check account freeze status  
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("checkFreeze")]
        public IHttpActionResult CheckFreezeStatus(FreezeStatus request)
        {
            string method = "CheckFreezeStatus";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.CheckFreeze(request));
        }

        /// <summary>
        /// this is to lien an account 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("placeLien")]
        public IHttpActionResult PlaceLien(PlaceLienModel request)
        {
            string method = "placeLien";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.PlaceLien(request));
        }


        /// <summary>
        /// this is to unlien an account 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("unplaceLien")]
        public IHttpActionResult UnPlaceLien(FreezeAccountRequest request)
        {
            string method = "PlaceLien";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.UnPlaceLien(request));
        }


        /// <summary>
        /// this is to check a iien status
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("checkLienStatus")]
        public IHttpActionResult CheckLienStatus(FreezeStatus request)
        {
            string method = "CheckLienStatus";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.CheckLienStatus(request));
        }


        /// <summary>
        /// this is to activate pnd
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("activatePND")]
        public IHttpActionResult activatePND(FreezeStatus request)
        {
            string method = "activatePND";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.ActivatePND(request));
        }


        /// <summary>
        /// this is to deactivate pnd
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("deactivatePND")]
        public IHttpActionResult deactivatePND(FreezeStatus request)
        {
            string method = "activatePND";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.DeactivatePND(request));
        }

        /// <summary>
        /// this is to check pnd status 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ResponseModel))]
        [Route("checkPND")]
        public IHttpActionResult checkpnd(FreezeStatus request)
        {
            string method = "checkPND";
            LogMachine.LogInformation(ClassName, method, "entered the get account endpoint");
            return Ok(accountService.CheckPNDStatus(request));
        }
    }
}
