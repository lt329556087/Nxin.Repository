using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BadDebtExecutionRepository : IRepository<FD_BadDebtExecution, string>
    {
        bool IsExist(string numericalOrder);
    }
}
