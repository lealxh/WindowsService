﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Infrastructure
{
    public interface ILogService
    {
        void Log(LogLevel LogLevel, object Message);
    }
}
