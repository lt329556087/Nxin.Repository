using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_paymentreceivablesvoucherRepository : IRepository<fd_paymentreceivablesvoucher, string>
    {
        Task<fd_paymentreceivablesvoucher> GetDataAsync(string id, CancellationToken cancellationToken = default);
        /// <summary>
        /// 凭证整理专用
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        List<fd_paymentreceivablesvoucher> GetDataList(string beginDate = "", string endDate = "", string enterpriseId = "", CancellationToken cancellationToken = default);
    }

    public interface Ifd_paymentreceivablesvoucherDetailRepository : IRepository<fd_paymentreceivablesvoucherdetail, string>
    {
        List<fd_paymentreceivablesvoucherdetail> GetDetailByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}
