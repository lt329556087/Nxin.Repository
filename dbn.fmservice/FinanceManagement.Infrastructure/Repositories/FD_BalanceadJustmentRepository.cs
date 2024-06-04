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
    public class FD_BalanceadJustmentRepository : Repository<FD_BalanceadJustment, string, Nxin_Qlw_BusinessContext>, IFD_BalanceadJustmentRepository
    {
        public FD_BalanceadJustmentRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BalanceadJustment> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BalanceadJustment>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_BalanceadJustmentDetailRepository : Repository<FD_BalanceadJustmentDetail, string, Nxin_Qlw_BusinessContext>, IFD_BalanceadJustmentDetailRepository
    {
        public FD_BalanceadJustmentDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
