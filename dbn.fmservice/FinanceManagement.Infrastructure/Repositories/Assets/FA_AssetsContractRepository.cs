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
    public class FA_AssetsContractRepository : Repository<FA_AssetsContract, string, Nxin_Qlw_BusinessContext>, IFA_AssetsContractRepository
    {

        public FA_AssetsContractRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_AssetsContract> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_AssetsContract>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_AssetsContractDetailRepository : Repository<FA_AssetsContractDetail, string, Nxin_Qlw_BusinessContext>, IFA_AssetsContractDetailRepository
    {
        public FA_AssetsContractDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FA_AssetsContractDetail> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_AssetsContractDetail>().FirstOrDefaultAsync(o => o.NumericalOrderDetail == id);
        }
    }
}
