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
    public class FD_AccountRepository : Repository<FD_Account, string, Nxin_Qlw_BusinessContext>, IFD_AccountRepository
    {

        public FD_AccountRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public FD_Account GetAccountByAccountNumber(string AccountNumber)
        {
            return DbContext.Set<FD_Account>().FirstOrDefault(o => o.AccountNumber == AccountNumber);
        }

        public override Task<FD_Account> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_Account>().FirstOrDefaultAsync(o => o.AccountID == id);
        }

        public List<FD_Account> GetEnterpriseId(string EnterpriseId)
        {
            return DbContext.Set<FD_Account>().Where(m => m.EnterpriseID == EnterpriseId).ToList();
        }
    }
}
