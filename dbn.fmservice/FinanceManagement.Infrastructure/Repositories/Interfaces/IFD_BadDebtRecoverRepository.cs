using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BadDebtRecoverRepository : IRepository<FD_BadDebtRecover, string>
    {
    }

    public interface IFD_BadDebtRecoverDetailRepository : IRepository<FD_BadDebtRecoverDetail, string>
    {
    }
}
