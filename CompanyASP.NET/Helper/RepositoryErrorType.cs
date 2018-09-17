﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Helper
{
    public enum RepositoryErrorType
    {
        INVALID_ARGUMENT,
        NOT_FOUND,
        SQL_EXCEPTION,
        NOT_INSERTED
    }
}