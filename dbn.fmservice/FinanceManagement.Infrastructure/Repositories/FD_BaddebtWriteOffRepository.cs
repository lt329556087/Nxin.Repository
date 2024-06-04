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
    public class FD_BaddebtWriteOffRepository : Repository<FD_BaddebtWriteOff, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtWriteOffRepository
    {

        public FD_BaddebtWriteOffRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FD_BaddebtWriteOff> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_BaddebtWriteOff>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

    }

    public class FD_BaddebtWriteOffDetailRepository : Repository<FD_BaddebtWriteOffDetail, string, Nxin_Qlw_BusinessContext>, IFD_BaddebtWriteOffDetailRepository
    {
        public FD_BaddebtWriteOffDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }        
    }

}
