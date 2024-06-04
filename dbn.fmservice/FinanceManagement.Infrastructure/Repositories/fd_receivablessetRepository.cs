using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class fd_receivablessetRepository : Repository<fd_receivablesset, string, Nxin_Qlw_BusinessContext>, Ifd_receivablessetRepository
    {
        public fd_receivablessetRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {

        }

        public fd_receivablesset GetDataByEnterpriseId(string enterpriseId)
        {
            return Set.Where(m => m.EnterpriseID == enterpriseId).FirstOrDefault();
        }
        public List<fd_receivablesset> GetListByEnterpriseId(string enterpriseId)
        {
            return Set.Where(m => m.EnterpriseID == enterpriseId).ToList();
        }
    }
}
