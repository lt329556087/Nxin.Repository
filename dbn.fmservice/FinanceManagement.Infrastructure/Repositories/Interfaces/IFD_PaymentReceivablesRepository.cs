using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_PaymentReceivablesRepository : IRepository<FD_PaymentReceivables, string>
    {
        public string ExceptionMessageHandle(string title, string msg);
        public List<FD_PaymentReceivables> GetList(string enterpriseId, string beginDate, string endDate);
    }
    
    public interface IFD_PaymentReceivablesDetailRepository : IRepository<FD_PaymentReceivablesDetail, string>
    {
        List<FD_PaymentReceivablesDetail> GetNums(string nums);
    }
    public interface IFD_PaymentExtendRepository : IRepository<FD_PaymentExtend, string>
    {
        public void SaveChange(FD_PaymentExtend model);
        public Task<FD_PaymentExtend> GetAsyncByRecordId(string id, CancellationToken cancellationToken = default);
        List<FD_PaymentExtend> GetDetails(string id, CancellationToken cancellationToken = default);

    }
}
