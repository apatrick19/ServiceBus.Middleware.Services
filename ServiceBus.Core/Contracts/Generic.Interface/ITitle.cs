using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts
{
    public interface ITitle : IDropdown
    {
        string NavID { get; set; }
    }
}
