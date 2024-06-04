using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFM_PerformanceIncomeRepository : IRepository<FM_PerformanceIncome, string>
    {
    }
}
