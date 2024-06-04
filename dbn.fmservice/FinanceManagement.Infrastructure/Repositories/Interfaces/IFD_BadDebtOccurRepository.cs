using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BadDebtOccurRepository : IRepository<FD_BadDebtOccur, string>
    {
    }

    public interface IFD_BadDebtOccurDetailRepository : IRepository<FD_BadDebtOccurDetail, string>
    {
    }

}
