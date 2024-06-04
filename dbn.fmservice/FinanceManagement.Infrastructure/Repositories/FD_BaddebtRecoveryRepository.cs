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
    public class FD_BaddebtRecoveryRepository : Repository<FD_BaddebtRecovery, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtRecoveryRepository
    {

        public FD_BaddebtRecoveryRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FD_BaddebtRecovery> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BaddebtRecovery>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

    }

    public class FD_BaddebtRecoveryDetailRepository : Repository<FD_BaddebtRecoveryDetail, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtRecoveryDetailRepository
    {
        public FD_BaddebtRecoveryDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }        
    }

}
