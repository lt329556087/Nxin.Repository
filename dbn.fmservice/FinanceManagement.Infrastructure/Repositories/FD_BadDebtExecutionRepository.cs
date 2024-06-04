using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_BadDebtExecutionRepository : Repository<FD_BadDebtExecution, string, Nxin_Qlw_BusinessContext>, IFD_BadDebtExecutionRepository
    {
        public FD_BadDebtExecutionRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public bool IsExist(string numericalOrder)
        {
            return DbContext.Set<FD_BadDebtExecution>().Where(o => o.NumericalOrder == numericalOrder && o.State ).FirstOrDefault() != null;
        }
    }
}
