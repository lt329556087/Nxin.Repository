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
    public class FM_CarryForwardVoucherRepository : Repository<FM_CarryForwardVoucher, string, Nxin_Qlw_BusinessContext>, IFM_CarryForwardVoucherRepository
    {
        public FM_CarryForwardVoucherRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FM_CarryForwardVoucher> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CarryForwardVoucher>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FM_CarryForwardVoucherDetailRepository : Repository<FM_CarryForwardVoucherDetail, string, Nxin_Qlw_BusinessContext>, IFM_CarryForwardVoucherDetailRepository
    {
        public FM_CarryForwardVoucherDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
    public class FM_CarryForwardVoucherExtendRepository : Repository<FM_CarryForwardVoucherExtend, string, Nxin_Qlw_BusinessContext>, IFM_CarryForwardVoucherExtendRepository
    {
        public FM_CarryForwardVoucherExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<FM_CarryForwardVoucherExtend>> GetExtends(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CarryForwardVoucherExtend>().Where(s => s.NumericalOrder == id).ToListAsync();
        }
    }
    public class FM_CarryForwardVoucherFormulaRepository : Repository<FM_CarryForwardVoucherFormula, string, Nxin_Qlw_BusinessContext>, IFM_CarryForwardVoucherFormulaRepository
    {
        public FM_CarryForwardVoucherFormulaRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<FM_CarryForwardVoucherFormula>> GetExtends(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CarryForwardVoucherFormula>().Where(s => s.NumericalOrder == id).ToListAsync();
        }
    }
}
