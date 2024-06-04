using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_bankreceivableRepository : IRepository<fd_bankreceivable, string>
    {
        fd_bankreceivable GetDataByIndex(string Index);
        fd_bankreceivable GetDataBySourceNum(string Num);
    }
}
