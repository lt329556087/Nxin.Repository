using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_AccountRepository : IRepository<FD_Account, string>
    {
        FD_Account GetAccountByAccountNumber(string AccountNumber);
        List<FD_Account> GetEnterpriseId(string EnterpriseId);
    }
}
