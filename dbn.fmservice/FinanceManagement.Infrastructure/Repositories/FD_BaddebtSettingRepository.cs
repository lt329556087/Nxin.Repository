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
    public class FD_BaddebtSettingRepository : Repository<FD_BaddebtSetting, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtSettingRepository
    {

        public FD_BaddebtSettingRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BaddebtSetting> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BaddebtSetting>().FirstOrDefaultAsync(o => o.NumericalOrder == id);
            //return DbContext.Set<FD_BaddebtSetting>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_BaddebtSettingDetailRepository : Repository<FD_BaddebtSettingDetail, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtSettingDetailRepository
    {
        public FD_BaddebtSettingDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
