﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services.Api
{
    public interface IApiService
    {
        Task<string> Ping();
    }
}
