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
    public class FA_UseStateTransferRepository : Repository<FA_UseStateTransfer, string, Nxin_Qlw_BusinessContext>, IFA_UseStateTransferRepository
    {

        public FA_UseStateTransferRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_UseStateTransfer> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_UseStateTransfer>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_UseStateTransferDetailRepository : Repository<FA_UseStateTransferDetail, string, Nxin_Qlw_BusinessContext>, IFA_UseStateTransferDetailRepository
    {
        public FA_UseStateTransferDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
