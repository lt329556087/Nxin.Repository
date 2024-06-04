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
    public class fd_bankreceivableRepository : Repository<fd_bankreceivable, string, Nxin_Qlw_BusinessContext>, Ifd_bankreceivableRepository
    {
        public fd_bankreceivableRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {

        }

        public fd_bankreceivable GetDataByIndex(string Index)
        {
            return Set.Where(m => m.transIndex == Index).FirstOrDefault();
        }

        public fd_bankreceivable GetDataBySourceNum(string Num)
        {
            return Set.Where(m => m.SourceNum == Num).FirstOrDefault();
        }
    }
}
