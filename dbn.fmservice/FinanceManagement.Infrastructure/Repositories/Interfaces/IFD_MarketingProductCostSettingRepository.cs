using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_MarketingProductCostSettingRepository : IRepository<FD_MarketingProductCostSetting>
    {
        Task<FD_MarketingProductCostSetting> GetAsync(string id, CancellationToken cancellationToken = default);
    }

    public interface IFD_MarketingProductCostSettingDetailRepository : IRepository<FD_MarketingProductCostSettingDetail>
    {
        Task<List<FD_MarketingProductCostSettingDetail>> GetByNumericalOrderListAsync(string numericalOrder, CancellationToken cancellationToken = default);

        void UpdateRange(IEnumerable<FD_MarketingProductCostSettingDetail> entities);
    }
}
