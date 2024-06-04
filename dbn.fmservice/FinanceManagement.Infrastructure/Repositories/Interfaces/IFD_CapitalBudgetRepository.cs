using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_CapitalBudgetRepository : IRepository<FD_CapitalBudget, string>
    {
        Task<bool> ExistAsync(string enterpriseId, string numericalorder);
    }



    public interface IFD_CapitalBudgetDetailRepository : IRepository<FD_CapitalBudgetDetail, string>
    {
    }
}
