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
    public class FD_BadDebtRecoverRepository : Repository<FD_BadDebtRecover, string, Nxin_Qlw_BusinessContext>, IFD_BadDebtRecoverRepository
    {
        public FD_BadDebtRecoverRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BadDebtRecover> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BadDebtRecover>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_BadDebtRecoverDetailRepository : Repository<FD_BadDebtRecoverDetail, string, Nxin_Qlw_BusinessContext>, IFD_BadDebtRecoverDetailRepository
    {
        public FD_BadDebtRecoverDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
