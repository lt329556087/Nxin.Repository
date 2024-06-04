using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_ReceivablesRepository : IRepository<FD_PaymentReceivables, string>
    {
        public string ExceptionMessageHandle(string title, string msg);
    }
    
    public interface IFD_ReceivablesDetailRepository : IRepository<FD_PaymentReceivablesDetail, string>
    {
        /// <summary>
        /// 获取收付款单明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<FD_PaymentReceivablesDetail> GetDetails(string id);
    }
    public interface IFD_ReceivablesExtendRepository : IRepository<FD_PaymentExtend, string>
    {
        public void SaveChange(FD_PaymentExtend model);
        public Task<FD_PaymentExtend> GetAsyncByRecordId(string id, CancellationToken cancellationToken = default);

    }
}
