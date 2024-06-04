using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFM_CashSweepRepository : IRepository<FM_CashSweep, string>
    {
        void SaveChange(FM_CashSweep model);
    }

    public interface IFM_CashSweepDetailRepository : IRepository<FM_CashSweepDetail, string>
    {
        FM_CashSweepDetail GetDetailByRecordID(int id, CancellationToken cancellationToken = default);
        List<FM_CashSweepDetail> GetDetailByID(string id, CancellationToken cancellationToken = default);
        Task<FM_CashSweepDetail> GetDetailByRecordIDAsc(int id, CancellationToken cancellationToken = default);
    }

    public interface IFD_PayextendRepository : IRepository<FD_Payextend, string>
    {
        List<FD_Payextend> GetByDetailId(string id, CancellationToken cancellationToken = default);
        List<FD_Payextend> GetByOrderNo(string id, CancellationToken cancellationToken = default);
        List<FD_Payextend> GetById(string id, CancellationToken cancellationToken = default);
        void SaveChange(FD_Payextend model);
    }
    public interface IFD_settlereceiptextendRepository : IRepository<fd_settlereceiptextend, string>
    {
        List<fd_settlereceiptextend> GetById(string id, CancellationToken cancellationToken = default);
        List<fd_settlereceiptextend> GetNums(string nums);
    }
    public interface IFM_CashSweepLogRepository : IRepository<FM_CashSweepLog, string>
    {
        List<FM_CashSweepLog> GetByIdAndBatchNo(string id,string batchNo, CancellationToken cancellationToken = default);
    }
}
