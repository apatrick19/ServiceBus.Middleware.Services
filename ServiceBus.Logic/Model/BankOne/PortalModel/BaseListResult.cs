using ServiceBus.Core.Model.Generic;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Model.Portal
{
    public class BaseListResult:BaseResponse
    {
        public List<DropdownResponse> result { get; set; }

        public UserRoleModules UserRoleModules { get; set; }
    }
}