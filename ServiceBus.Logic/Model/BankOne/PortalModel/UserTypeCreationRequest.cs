using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class UserTypeCreationRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public string IsAdmin { get; set; }
    }
}