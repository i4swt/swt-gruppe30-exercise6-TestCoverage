﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DoorControl
{
    public interface IUserValidation
    {
        bool ValidateEntryRequest(int id);
    }
}
