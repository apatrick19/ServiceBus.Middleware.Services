﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Contracts
{
    public interface IDuplicateDetection
    {
        dynamic GetDuplicateRule(string key);
    }
}