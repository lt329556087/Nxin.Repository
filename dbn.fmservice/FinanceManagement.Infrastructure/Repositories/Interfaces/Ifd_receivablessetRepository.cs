using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_receivablessetRepository : IRepository<fd_receivablesset, string>
    {
        fd_receivablesset GetDataByEnterpriseId(string enterpriseId);
        List<fd_receivablesset> GetListByEnterpriseId(string enterpriseId);
    }
}
