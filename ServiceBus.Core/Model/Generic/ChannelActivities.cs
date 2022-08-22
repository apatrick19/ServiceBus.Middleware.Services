using ServiceBus.Core.Contracts;
using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class ChannelActivities : Entity,IChannelActivities
    {
        public string RSAPIN { get; set; }
        public string Channel { get; set; }
        public string Activities { get; set; }
        public DateTime ActionDate { get; set; }
    }



    public class ChannelActivitiesMapping :  Dropdown, IChannelActivitiesMapping
    {

    }
}
