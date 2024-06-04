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
    public class FD_AccountInventoryRepository : Repository<FD_AccountInventory, string, Nxin_Qlw_BusinessContext>, IFD_AccountInventoryRepository
    {
        public FD_AccountInventoryRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_AccountInventory> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_AccountInventory>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_AccountInventoryDetailRepository : Repository<FD_AccountInventoryDetail, string, Nxin_Qlw_BusinessContext>, IFD_AccountInventoryDetailRepository
    {
        public FD_AccountInventoryDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
