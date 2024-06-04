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
    public class FD_BadDebtOccurRepository : Repository<FD_BadDebtOccur, string, Nxin_Qlw_BusinessContext>,    IFD_BadDebtOccurRepository
    {
        public FD_BadDebtOccurRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BadDebtOccur> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BadDebtOccur>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }


    public class FD_BadDebtOccurDetailRepository : Repository<FD_BadDebtOccurDetail, string, Nxin_Qlw_BusinessContext>,IFD_BadDebtOccurDetailRepository
    {
        public FD_BadDebtOccurDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
