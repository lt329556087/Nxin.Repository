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
    public class FD_CapitalBudgetRepository : Repository<FD_CapitalBudget, string, Qlw_Nxin_ComContext>, IFD_CapitalBudgetRepository
    {

        public FD_CapitalBudgetRepository(Qlw_Nxin_ComContext context) : base(context)
        {
        }
        public Task<bool> ExistAsync(string enterpriseId, string NumericalOrder)
        {
            return Set.AnyAsync(o => o.EnterpriseID == enterpriseId && o.NumericalOrder == NumericalOrder);
        }
        public override Task<FD_CapitalBudget> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_CapitalBudget>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_CapitalBudgetDetailRepository : Repository<FD_CapitalBudgetDetail, string, Qlw_Nxin_ComContext>, IFD_CapitalBudgetDetailRepository
    {
        public FD_CapitalBudgetDetailRepository(Qlw_Nxin_ComContext context) : base(context)
        {
        }
    }
}
