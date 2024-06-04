using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFA_AssetsContractRepository : IRepository<FA_AssetsContract, string>
    {
    }


    public interface IFA_AssetsContractDetailRepository : IRepository<FA_AssetsContractDetail, string>
    {
    }
}
