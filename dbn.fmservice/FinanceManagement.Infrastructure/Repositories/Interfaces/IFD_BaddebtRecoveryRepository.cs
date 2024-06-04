using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BaddebtRecoveryRepository : IRepository<FD_BaddebtRecovery, string>
    {

    }



    public interface IFD_BaddebtRecoveryDetailRepository : IRepository<FD_BaddebtRecoveryDetail, string>
    {
    }

}
