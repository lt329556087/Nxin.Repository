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
    public class FA_PigAssetsResetRepository : Repository<FA_PigAssetsReset, string, Nxin_Qlw_BusinessContext>, IFA_PigAssetsResetRepository
    {

        public FA_PigAssetsResetRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_PigAssetsReset> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_PigAssetsReset>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_PigAssetsResetDetailRepository : Repository<FA_PigAssetsResetDetail, string, Nxin_Qlw_BusinessContext>, IFA_PigAssetsResetDetailRepository
    {
        public FA_PigAssetsResetDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
