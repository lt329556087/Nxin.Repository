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
    public class FD_PaymentReceivablesRepository : Repository<FD_PaymentReceivables, string, Nxin_Qlw_BusinessContext>, IFD_PaymentReceivablesRepository
    {
        public FD_PaymentReceivablesRepository(Nxin_Qlw_BusinessContext context) : base(context)
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
        /// <summary>
        /// 获取非集中支付 收付款单表头数据
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<FD_PaymentReceivables> GetList(string enterpriseId, string beginDate, string endDate)
        {
            return DbContext.Set<FD_PaymentReceivables>().Where(o => o.EnterpriseID == enterpriseId && o.DataDate >= Convert.ToDateTime(beginDate) && o.DataDate <= Convert.ToDateTime(endDate) && !o.IsGroupPay).ToList();
        }
    }

    public class FD_PaymentReceivablesDetailRepository : Repository<FD_PaymentReceivablesDetail, string, Nxin_Qlw_BusinessContext>, IFD_PaymentReceivablesDetailRepository
    {
        public FD_PaymentReceivablesDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public List<FD_PaymentReceivablesDetail> GetNums(string nums)
        {
            return DbContext.Set<FD_PaymentReceivablesDetail>().Where(o => o.NumericalOrder == nums).ToList();
        }
    }
    public class FD_PaymentExtendRepository : Repository<FD_PaymentExtend, string, Nxin_Qlw_BusinessContext>, IFD_PaymentExtendRepository
    {
        public FD_PaymentExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
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
        public List<FD_PaymentExtend> GetDetails(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_PaymentExtend>().Where(o => o.NumericalOrder == id).ToList();
        }
        public void SaveChange(FD_PaymentExtend model)
        {
            DbContext.Set<FD_PaymentExtend>().Update(model);
            DbContext.SaveChanges();
        }
    }
}
