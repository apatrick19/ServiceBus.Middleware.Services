using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    interface IECRSResponse : IEntity
    {
        string SetID { get; set; }
        string ReferenceNumber { get; set; }
        string RSAPIN { get; set; }
        string ResponseCode { get; set; }
        string ResponseMessage { get; set; }
        string RequestType { get; set; }
        DateTime DateSent { get; set; }
        DateTime DateReceived { get; set; }

    }
}
