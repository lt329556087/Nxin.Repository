using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_ReceivablesRepository : Repository<FD_PaymentReceivables, string, Nxin_Qlw_BusinessContext>, IFD_ReceivablesRepository
    {
        public FD_ReceivablesRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public string ExceptionMessageHandle(string title, string msg)
        {
            return $@"
                            <details>
                            <p>{title}</p>
                            </details>";
        }
        public override Task<FD_PaymentReceivables> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_PaymentReceivables>().Include(o => o.details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FD_ReceivablesDetailRepository : Repository<FD_PaymentReceivablesDetail, string, Nxin_Qlw_BusinessContext>, IFD_ReceivablesDetailRepository
    {
        public FD_ReceivablesDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public List<FD_PaymentReceivablesDetail> GetDetails(string id)
        {
            return Set.Where(m => m.NumericalOrder == id).ToList();
        }
    }
    public class FD_ReceivablesExtendRepository : Repository<FD_PaymentExtend, string, Nxin_Qlw_BusinessContext>, IFD_ReceivablesExtendRepository
    {
        public FD_ReceivablesExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FD_PaymentExtend> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_PaymentExtend>().FirstOrDefaultAsync(o => o.TradeNo == id);
        }
        public Task<FD_PaymentExtend> GetAsyncByRecordId(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_PaymentExtend>().FirstOrDefaultAsync(o => o.RecordID == Convert.ToInt32(id));
        }
        public void SaveChange(FD_PaymentExtend model)
        {
            DbContext.Set<FD_PaymentExtend>().Update(model);
            DbContext.SaveChanges();
        }
    }
}
