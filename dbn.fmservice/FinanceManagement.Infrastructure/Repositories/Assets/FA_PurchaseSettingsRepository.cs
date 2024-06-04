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
    public class FA_PurchaseSettingsRepository : Repository<FA_PurchaseSettings, string, Nxin_Qlw_BusinessContext>, IFA_PurchaseSettingsRepository
    {

        public FA_PurchaseSettingsRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_PurchaseSettings> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_PurchaseSettings>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_PurchaseSettingsDetailRepository : Repository<FA_PurchaseSettingsDetail, string, Nxin_Qlw_BusinessContext>, IFA_PurchaseSettingsDetailRepository
    {
        public FA_PurchaseSettingsDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
