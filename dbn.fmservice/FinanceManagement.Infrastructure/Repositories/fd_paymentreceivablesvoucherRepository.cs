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
    public class fd_paymentreceivablesvoucherRepository : Repository<fd_paymentreceivablesvoucher, string, Nxin_Qlw_BusinessContext>, Ifd_paymentreceivablesvoucherRepository
    {
        public fd_paymentreceivablesvoucherRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<fd_paymentreceivablesvoucher> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_paymentreceivablesvoucher>().Include(o => o.details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
        public Task<fd_paymentreceivablesvoucher> GetDataAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_paymentreceivablesvoucher>().FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
        public List<fd_paymentreceivablesvoucher> GetDataList(string beginDate = "",string endDate = "",string enterpriseId = "", CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_paymentreceivablesvoucher>().Where(m=>m.DataDate >= Convert.ToDateTime(beginDate) && m.DataDate <= Convert.ToDateTime(endDate) && m.EnterpriseID == enterpriseId).ToList();
        }
    }

    public class fd_paymentreceivablesvoucherDetailRepository : Repository<fd_paymentreceivablesvoucherdetail, string, Nxin_Qlw_BusinessContext>, Ifd_paymentreceivablesvoucherDetailRepository
    {
        public fd_paymentreceivablesvoucherDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {            
        }
        public List<fd_paymentreceivablesvoucherdetail> GetDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_paymentreceivablesvoucherdetail>().Where(o => o.NumericalOrder == id).ToList();
        }
    }
}
