using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_PigAssetsResetRepository : IRepository<FA_PigAssetsReset, string>
    {
    }


    public interface IFA_PigAssetsResetDetailRepository : IRepository<FA_PigAssetsResetDetail, string>
    {
    }
}
