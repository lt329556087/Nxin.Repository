using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FM_PerformanceIncomeRepository : Repository<FM_PerformanceIncome, string, Nxin_Qlw_BusinessContext>, IFM_PerformanceIncomeRepository
    {

        public FM_PerformanceIncomeRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

      
    }
}
