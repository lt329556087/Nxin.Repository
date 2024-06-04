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
    public class FA_AssetsMaintainRepository : Repository<FA_AssetsMaintain, string, Nxin_Qlw_BusinessContext>, IFA_AssetsMaintainRepository
    {

        public FA_AssetsMaintainRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_AssetsMaintain> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_AssetsMaintain>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_AssetsMaintainDetailRepository : Repository<FA_AssetsMaintainDetail, string, Nxin_Qlw_BusinessContext>, IFA_AssetsMaintainDetailRepository
    {
        public FA_AssetsMaintainDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
