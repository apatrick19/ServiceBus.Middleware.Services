using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts
{
    public interface IDropdown : IEntity
    {
        string Name { get; set; }
        string GUID { get; set; }
        string Reference { get; set; }
    }
}
