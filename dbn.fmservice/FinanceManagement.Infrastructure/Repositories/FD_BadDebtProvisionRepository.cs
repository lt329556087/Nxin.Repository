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
    public class FD_BadDebtProvisionRepository : Repository<FD_BadDebtProvision, string, Nxin_Qlw_BusinessContext>, IFD_BadDebtProvisionRepository
    {
        public FD_BadDebtProvisionRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public Task<FD_BadDebtProvision> GetLastest(string enterpriseId, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BadDebtProvision>().Where(o => o.EnterpriseID == enterpriseId).OrderByDescending(o => o.DataDate).FirstOrDefaultAsync();
        }

        public override Task<FD_BadDebtProvision> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BadDebtProvision>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

        public bool IsSettingDataExist(string id, string enterpriseId)
        {
            var modelProvision = DbContext.Set<FD_BadDebtProvision>().Where(o => o.EnterpriseID == enterpriseId && o.NumericalOrderSetting == id);
            var modelOccur = DbContext.Set<FD_BadDebtOccur>().Where(o => o.EnterpriseID == enterpriseId && o.NumericalOrderSetting == id);
            var modelRecover = DbContext.Set<FD_BadDebtRecover>().Where(o => o.EnterpriseID == enterpriseId && o.NumericalOrderSetting == id);
            return modelProvision.Any() && modelOccur.Any() && modelRecover.Any();
        }

    }


    public class FD_BadDebtProvisionDetailRepository : Repository<FD_BadDebtProvisionDetail, string, Nxin_Qlw_BusinessContext>, IFD_BadDebtProvisionDetailRepository
    {
        public FD_BadDebtProvisionDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BadDebtProvisionDetail> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BadDebtProvisionDetail>().Include(o => o.AgingList).FirstOrDefaultAsync(o => o.NumericalOrderDetail == id);
        }

        public bool IsSpecificDataExist(string id, string enterpriseId)
        {
            var data = DbContext.Set<FD_BadDebtProvision>().Include(o => o.Lines).Where(o => o.EnterpriseID == enterpriseId).ToListAsync().Result;
            var flag = false;
            flag = data.Exists(o => o.Lines.Exists(o => o.NumericalOrderSpecific == id));
            return flag;
        }
    }

    public class FD_BadDebtProvisionExtRepository : Repository<FD_BadDebtProvisionExt, string, Nxin_Qlw_BusinessContext>, IFD_BadDebtProvisionExtRepository
    {
        public FD_BadDebtProvisionExtRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
