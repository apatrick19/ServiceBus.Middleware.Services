using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Contracts
{
    public interface ICryptography
    {
         string Encrypt(string text);

        string Decrypt(string cipher);

    }
}
