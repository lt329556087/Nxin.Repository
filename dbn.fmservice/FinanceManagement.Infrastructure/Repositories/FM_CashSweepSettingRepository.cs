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
    public class FM_CashSweepSettingRepository : Repository<FM_CashSweepSetting, string, Nxin_Qlw_BusinessContext>, IFM_CashSweepSettingRepository
    {

        public FM_CashSweepSettingRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FM_CashSweepSetting> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CashSweepSetting>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FM_CashSweepSettingDetailRepository : Repository<FM_CashSweepSettingDetail, string, Nxin_Qlw_BusinessContext>, IFM_CashSweepSettingDetailRepository
    {
        public FM_CashSweepSettingDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
           
        }
    }

    public class FM_CashSweepSettingExtRepository : Repository<FM_CashSweepSettingExt, string, Nxin_Qlw_BusinessContext>, IFM_CashSweepSettingExtRepository
    {
        public FM_CashSweepSettingExtRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
