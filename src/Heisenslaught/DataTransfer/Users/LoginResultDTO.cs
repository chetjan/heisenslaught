﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.DataTransfer.Users
{
    public class LoginResultDTO<TData>
    {
        public bool success;
        public TData data;

        public LoginResultDTO(bool success, TData data)
        {
            this.data = data;
            this.success = success;
        }
    }
}
