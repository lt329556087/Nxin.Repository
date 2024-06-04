using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_settletypesetRepository : IRepository<fd_settletypeset, string>
    {
        
    }
    public interface Ifd_settletypeRepository : IRepository<biz_datadict, string>
    {

    }
}
