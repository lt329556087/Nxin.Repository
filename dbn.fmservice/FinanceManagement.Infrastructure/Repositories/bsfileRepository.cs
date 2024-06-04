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
    public class bsfileRepository : Repository<bsfile, string, Qlw_Nxin_ComContext>, IbsfileRepository
    {
        public bsfileRepository(Qlw_Nxin_ComContext context) : base(context)
        {
        }

        public List<bsfile> GetBsfiles(string nums)
        {
            return Set.Where(m => m.NumericalOrder == nums).ToList();
        }
    }
}
