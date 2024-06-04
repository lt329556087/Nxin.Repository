using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_BadDebtProvisionRepository : IRepository<FD_BadDebtProvision, string>
    {


        public bool IsSettingDataExist(string id, string enterpriseId);

        Task<FD_BadDebtProvision> GetLastest(string enterpriseId, CancellationToken cancellationToken = default);


    }

    public interface IFD_BadDebtProvisionDetailRepository : IRepository<FD_BadDebtProvisionDetail, string>
    {

        public bool IsSpecificDataExist(string id, string enterpriseId);
    }


    public interface IFD_BadDebtProvisionExtRepository : IRepository<FD_BadDebtProvisionExt, string>
    {
    }

}
