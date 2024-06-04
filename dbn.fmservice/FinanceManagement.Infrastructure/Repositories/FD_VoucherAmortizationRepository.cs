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
    public class FD_VoucherAmortizationRepository : Repository<FD_VoucherAmortization, string, Nxin_Qlw_BusinessContext>, IFD_VoucherAmortizationRepository
    {
        public FD_VoucherAmortizationRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_VoucherAmortization> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_VoucherAmortization>().Include(o => o.Details).Include(o=>o.PeriodDetails).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_VoucherAmortizationDetailRepository : Repository<FD_VoucherAmortizationDetail, string, Nxin_Qlw_BusinessContext>, IFD_VoucherAmortizationDetailRepository
    {
        public FD_VoucherAmortizationDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
    public class FD_VoucherAmortizationPeriodDetailRepository : Repository<FD_VoucherAmortizationPeriodDetail, string, Nxin_Qlw_BusinessContext>, IFD_VoucherAmortizationPeriodDetailRepository
    {
        public FD_VoucherAmortizationPeriodDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<FD_VoucherAmortizationPeriodDetail>> GetPeriods(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_VoucherAmortizationPeriodDetail>().Where(s => s.NumericalOrder == id).ToListAsync();
        }
    }
}
