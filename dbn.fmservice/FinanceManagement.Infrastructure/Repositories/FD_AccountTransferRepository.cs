using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_AccountTransferRepository : Repository<FD_AccountTransfer, string, Nxin_Qlw_BusinessContext>, IFD_AccountTransferRepository
    {

        public FD_AccountTransferRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_AccountTransfer> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_AccountTransfer>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_AccountTransferDetailRepository : Repository<FD_AccountTransferDetail, string, Nxin_Qlw_BusinessContext>, IFD_AccountTransferDetailRepository
    {
        public FD_AccountTransferDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public FD_AccountTransferDetail GetSingleData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return DbContext.Set<FD_AccountTransferDetail>().FirstOrDefault(o => o.NumericalOrder == id && o.IsIn == false);
        }
        public FD_AccountTransferDetail GetSingleDataIsIn(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return DbContext.Set<FD_AccountTransferDetail>().FirstOrDefault(o => o.NumericalOrder == id && o.IsIn == true);
        }
    }
}
