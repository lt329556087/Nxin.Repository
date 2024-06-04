using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_MarketingProductCostSettingRepository : Repository<FD_MarketingProductCostSetting, string, Nxin_Qlw_BusinessContext>, IFD_MarketingProductCostSettingRepository
    {
        public FD_MarketingProductCostSettingRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override async Task<FD_MarketingProductCostSetting> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<FD_MarketingProductCostSetting>().FirstOrDefaultAsync(c => c.NumericalOrder == id);
        }
    }

    public class FD_MarketingProductCostSettingDetailRepository : Repository<FD_MarketingProductCostSettingDetail, string, Nxin_Qlw_BusinessContext>, IFD_MarketingProductCostSettingDetailRepository
    {
        public FD_MarketingProductCostSettingDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public async Task<List<FD_MarketingProductCostSettingDetail>> GetByNumericalOrderListAsync(string numericalOrder, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<FD_MarketingProductCostSettingDetail>().Where(c => c.NumericalOrder == numericalOrder).ToListAsync(cancellationToken);
        }

        public void UpdateRange(IEnumerable<FD_MarketingProductCostSettingDetail> entities)
        {
            DbContext.Set<FD_MarketingProductCostSettingDetail>().UpdateRange(entities.ToArray());
        }
    }
}
