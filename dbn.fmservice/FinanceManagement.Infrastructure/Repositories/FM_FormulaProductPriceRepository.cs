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
    public class MS_FormulaProductPriceRepository : Repository<MS_FormulaProductPrice, string, Nxin_Qlw_BusinessContext>, IMS_FormulaProductPriceRepository
    {

        public MS_FormulaProductPriceRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<MS_FormulaProductPrice> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<MS_FormulaProductPrice>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class MS_FormulaProductPriceDetailRepository : Repository<MS_FormulaProductPriceDetail, string, Nxin_Qlw_BusinessContext>, IMS_FormulaProductPriceDetailRepository
    {
        public MS_FormulaProductPriceDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
       
    }

    public class MS_FormulaProductPriceExtRepository : Repository<MS_FormulaProductPriceExt, string, Nxin_Qlw_BusinessContext>, IMS_FormulaProductPriceExtRepository
    {
        public MS_FormulaProductPriceExtRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<MS_FormulaProductPriceExt>> GetExtAsync(string id, string detailID, int BusiType)
        {
            return DbContext.Set<MS_FormulaProductPriceExt>().Where(o =>o.NumericalOrder==id)?.ToListAsync();
        }
    }

}
