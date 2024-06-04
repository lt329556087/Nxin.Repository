using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_ExpenseRepository : Repository<FD_Expense, string, Qlw_Nxin_ComContext>, IFD_ExpenseRepository
    {

        public FD_ExpenseRepository(Qlw_Nxin_ComContext context) : base(context)
        {
        }

        public override Task<FD_Expense> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_Expense>().FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_ExpenseDetailRepository : Repository<FD_ExpenseDetail, string, Qlw_Nxin_ComContext>, IFD_ExpenseDetailRepository
    {
        public FD_ExpenseDetailRepository(Qlw_Nxin_ComContext context) : base(context)
        {
        }
    }
    public class FD_ExpenseExtRepository : Repository<FD_ExpenseExt, string, Qlw_Nxin_ComContext>, IFD_ExpenseExtRepository
    {
        public FD_ExpenseExtRepository(Qlw_Nxin_ComContext context) : base(context)
        {
        }
    }
}
