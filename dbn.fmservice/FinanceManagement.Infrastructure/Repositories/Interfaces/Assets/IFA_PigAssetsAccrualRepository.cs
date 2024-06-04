using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_PigAssetsAccrualRepository : IRepository<FA_PigAssetsAccrual, string>
    {
    }


    public interface IFA_PigAssetsAccrualDetailRepository : IRepository<FA_PigAssetsAccrualDetail, string>
    {
    }
}
