using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BaddebtWriteOffRepository : IRepository<FD_BaddebtWriteOff, string>
    {

    }



    public interface IFD_BaddebtWriteOffDetailRepository : IRepository<FD_BaddebtWriteOffDetail, string>
    {
    }

}
