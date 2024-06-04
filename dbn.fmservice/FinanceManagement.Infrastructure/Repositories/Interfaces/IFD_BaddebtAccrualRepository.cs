using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BaddebtAccrualRepository : IRepository<FD_BaddebtAccrual, string>
    {

    }



    public interface IFD_BaddebtAccrualDetailRepository : IRepository<FD_BaddebtAccrualDetail, string>
    {
    }


    public interface IFD_BaddebtAccrualExtRepository : IRepository<FD_BaddebtAccrualExt, string>
    {
    }
    
}
