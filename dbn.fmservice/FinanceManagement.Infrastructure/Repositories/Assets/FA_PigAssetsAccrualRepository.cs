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
    public class FA_PigAssetsAccrualRepository : Repository<FA_PigAssetsAccrual, string, Nxin_Qlw_BusinessContext>, IFA_PigAssetsAccrualRepository
    {

        public FA_PigAssetsAccrualRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_PigAssetsAccrual> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_PigAssetsAccrual>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_PigAssetsAccrualDetailRepository : Repository<FA_PigAssetsAccrualDetail, string, Nxin_Qlw_BusinessContext>, IFA_PigAssetsAccrualDetailRepository
    {
        public FA_PigAssetsAccrualDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
