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
    public class FA_AssetsApplyRepository : Repository<FA_AssetsApply, string, Nxin_Qlw_BusinessContext>, IFA_AssetsApplyRepository
    {

        public FA_AssetsApplyRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_AssetsApply> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_AssetsApply>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_AssetsApplyDetailRepository : Repository<FA_AssetsApplyDetail, string, Nxin_Qlw_BusinessContext>, IFA_AssetsApplyDetailRepository
    {
        public FA_AssetsApplyDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FA_AssetsApplyDetail> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_AssetsApplyDetail>().FirstOrDefaultAsync(o => o.NumericalOrderDetail == id);
        }
    }
}
