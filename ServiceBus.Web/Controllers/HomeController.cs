using Newtonsoft.Json;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Custom.Implementation;
using ServiceBus.Custom.Model;
using ServiceBus.Data.Implementation.DataAccess;
using ServiceBus.Logic.Integration.Portal;
using ServiceBus.Logic.Model.PortalModel;
using ServiceBus.Logic.Portal;
using ServiceBus.Web.Controllers;
using ServiceBus.Web.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using static ServiceBus.Web.Models.Enum;

namespace ServiceBus.API.Controllers
{
    [SessionState(SessionStateBehavior.Default)]
    public class HomeController : BaseController
    {
        ILoginService loginservice;
        public HomeController(ILoginService service)
        {
            loginservice = service;
            if (MemoryManager.AccountOfficer==null)
            {
                MemoryManager.FetchLga();
                MemoryManager.FetchState();
                MemoryManager.FetchNationality();
                MemoryManager.FetchProducts();
                MemoryManager.FetchOfficer();
                MemoryManager.FetchBanks();
                MemoryManager.FetchBillerCategories();
                MemoryManager.FetchBillers();
                MemoryManager.FetchPaymentItems();
            }
        }

        public HomeController()
        {

        }
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string password)
        {
            var result = new LoginService().AuthenticateUser(email, password);
            if (result.ResponseCode!="00")
            {
                ViewBag.Message = "invalid input / no record found";
                return View();
            }
            User user = (User)result.ResultObject;
            Session["userid"] = user.ID;
            Session["email"] = user.Email;
            Session["name"] = user.FirstName+" "+user.LastName ;
            Session["passportUrl"] = user.PassportUrl;
            Session["Role"] = user.Role;

            //get user roles 
            var userroles = UserLogic.FetchUserRoleByUserId(user.Role);
            Session["userroles"] = userroles;

            if (user.IsFirstLogon==true && user.IsPasswordChanged==false)
            {
                return RedirectToAction("PasswordChange");
            }
            return RedirectToAction("Landing");           
        }

        public ActionResult Landing()
        {
            if (Session["userid"]==null)
            {
                return RedirectToAction("Index","Home");
            }

            //monthly report
            var dataPoints = ReportLogic.GetAccountByMonthReports();
            ViewBag.MonthlyReport = JsonConvert.SerializeObject(dataPoints);

            //status reports
            var statusPoints = ReportLogic.GetAccountStatusReports();
            ViewBag.StatusReport = JsonConvert.SerializeObject(statusPoints);

            //device reports
            var devicePoints = ReportLogic.GetDeviceUsageReports();
            ViewBag.deviceReport = JsonConvert.SerializeObject(devicePoints);

            return View(new BaseResponse() { ResponseCode = "", ResponseMessage = "" });
        }


        public ActionResult PasswordChange()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(  new BaseResponse() { ResponseCode = "", ResponseMessage = "" });
        }

        [HttpPost]
        public ActionResult PasswordChange(PasswordChangeRequest request)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var result = UserLogic.PasswordChange(request);
            return View("Landing",result);
        }
    }
}
