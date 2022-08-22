using ServiceBus.Core.Model.Generic;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Model.Portal
{
    public class UserBaseResponse: BaseResponse
    {
        public List<Core.Model.Generic.User> users { get; set; }
        public List<DropdownResponse> usertype { get; set; }
        public List<DropdownResponse> userdropdown { get; set; }
        public List<DropdownResponse> Region { get; set; }
    }
}