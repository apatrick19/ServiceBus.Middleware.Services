using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ServiceBus.Web.Models.Enum;

namespace ServiceBus.Web.Controllers
{
    public abstract class BaseController : Controller
    {

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "swal('" + notificationType.ToString().ToUpper() + "', '" + message + "','" + notificationType + "')" + "";
            TempData["notification"] = msg;
        }

        public string Alert(string title,string message, NotificationType notificationType)
        {
            var msg = $"\"{title}!!\", \"{message}\", \"{notificationType}\"";
            return msg;
        }
    }



}