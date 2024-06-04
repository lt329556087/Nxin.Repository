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

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FA_InventoryRepository : Repository<FA_Inventory, string, Nxin_Qlw_BusinessContext>, IFA_InventoryRepository
    {

        public FA_InventoryRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FA_Inventory> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_Inventory>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_InventoryDetailRepository : Repository<FA_InventoryDetail, string, Nxin_Qlw_BusinessContext>, IFA_InventoryDetailRepository
    {
        public FA_InventoryDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        
    }
}
