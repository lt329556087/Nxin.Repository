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
    public class FA_AssetsInspectRepository : Repository<FA_AssetsInspect, string, Nxin_Qlw_BusinessContext>, IFA_AssetsInspectRepository
    {

        public FA_AssetsInspectRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_AssetsInspect> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_AssetsInspect>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_AssetsInspectDetailRepository : Repository<FA_AssetsInspectDetail, string, Nxin_Qlw_BusinessContext>, IFA_AssetsInspectDetailRepository
    {
        public FA_AssetsInspectDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
