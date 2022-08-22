using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IChannelActivities:IEntity
    {
         string RSAPIN { get; set; }
         string Channel { get; set; }
         string Activities { get; set; }
         DateTime ActionDate { get; set; }
    }


    public interface IChannelActivitiesMapping : IDropdown
    {

    }
}
