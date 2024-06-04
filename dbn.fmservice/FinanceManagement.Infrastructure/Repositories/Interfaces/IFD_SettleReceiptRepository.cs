using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_SettleReceiptRepository : IRepository<FD_SettleReceipt, string>
    {
        Task<FD_SettleReceipt> GetDataAsync(string id, CancellationToken cancellationToken = default);
        /// <summary>
        /// 凭证整理专用
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="enterpriseId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        List<FD_SettleReceipt> GetDataList(string beginDate = "", string endDate = "", string enterpriseId = "", CancellationToken cancellationToken = default);
        /// <summary>
        /// 根据流水号获取数据
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        List<FD_SettleReceipt> GetByNums(string nums);
    }

    public interface IFD_SettleReceiptDetailRepository : IRepository<FD_SettleReceiptDetail, string>
    {
        Task<FD_SettleReceiptDetail> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);
        List<FD_SettleReceiptDetail> GetByNums(string nums);
    }
}
