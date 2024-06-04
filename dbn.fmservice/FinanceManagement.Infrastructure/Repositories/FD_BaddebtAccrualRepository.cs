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
    public class FD_BaddebtAccrualRepository : Repository<FD_BaddebtAccrual, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtAccrualRepository
    {

        public FD_BaddebtAccrualRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FD_BaddebtAccrual> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BaddebtAccrual>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

    }

    public class FD_BaddebtAccrualDetailRepository : Repository<FD_BaddebtAccrualDetail, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtAccrualDetailRepository
    {
        public FD_BaddebtAccrualDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_BaddebtAccrualDetail> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BaddebtAccrualDetail>().Include(o => o.AgingList).FirstOrDefaultAsync(o => o.NumericalOrderDetail == id);
        }
    }

    public class FD_BaddebtAccrualExtRepository : Repository<FD_BaddebtAccrualExt, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtAccrualExtRepository
    {
        public FD_BaddebtAccrualExtRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

}
