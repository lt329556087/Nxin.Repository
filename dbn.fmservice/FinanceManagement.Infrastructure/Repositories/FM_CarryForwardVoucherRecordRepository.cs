using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace FinanceManagement.Infrastructure.Repositories
{
    public class FM_CarryForwardVoucherRecordRepository : Repository<FM_CarryForwardVoucherRecord, string, Nxin_Qlw_BusinessContext>, IFM_CarryForwardVoucherRecordRepository
    {
        public FM_CarryForwardVoucherRecordRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
