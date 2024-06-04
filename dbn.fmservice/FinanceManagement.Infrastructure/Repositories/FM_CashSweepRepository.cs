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
    public class FM_CashSweepRepository : Repository<FM_CashSweep, string, Nxin_Qlw_BusinessContext>, IFM_CashSweepRepository
    {

        public FM_CashSweepRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FM_CashSweep> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CashSweep>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
        public void SaveChange(FM_CashSweep model)
        {
            DbContext.Set<FM_CashSweep>().Update(model);
            DbContext.SaveChanges();
        }
    }

    public class FM_CashSweepDetailRepository : Repository<FM_CashSweepDetail, string, Nxin_Qlw_BusinessContext>, IFM_CashSweepDetailRepository
    {
        public FM_CashSweepDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<FM_CashSweepDetail> GetDetailByRecordIDAsc(int id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CashSweepDetail>().FirstOrDefaultAsync(o => o.RecordID == id);
        }
        public FM_CashSweepDetail GetDetailByRecordID(int id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CashSweepDetail>().FirstOrDefault(o => o.RecordID == id);
        }
        public List<FM_CashSweepDetail> GetDetailByID(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CashSweepDetail>().Where(o => o.NumericalOrder == id).ToList();
        }
    }

    public class FD_PayextendRepository : Repository<FD_Payextend, string, Nxin_Qlw_BusinessContext>, IFD_PayextendRepository
    {

        public FD_PayextendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public List<FD_Payextend> GetByDetailId(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_Payextend>().Where(o => o.NumericalOrderDetail == id)?.ToList();
        }
        public List<FD_Payextend> GetByOrderNo(string orderNo, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_Payextend>().Where(o => o.OrderNo == orderNo)?.ToList();
        }
        public List<FD_Payextend> GetById(string id,  CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_Payextend>().Where(o => o.NumericalOrder == id)?.ToList();
        }
        public void SaveChange(FD_Payextend model)
        {
            DbContext.Set<FD_Payextend>().Update(model);
            DbContext.SaveChanges();
        }
    }

    public class FD_settlereceiptextendRepository : Repository<fd_settlereceiptextend, string, Nxin_Qlw_BusinessContext>, IFD_settlereceiptextendRepository
    {

        public FD_settlereceiptextendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public List<fd_settlereceiptextend> GetById(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<fd_settlereceiptextend>().Where(o => o.NumericalOrder == id)?.ToList();
        }

        public List<fd_settlereceiptextend> GetNums(string nums)
        {
            return DbContext.Set<fd_settlereceiptextend>().Where(o => o.NumericalOrder.Contains(nums))?.ToList();
        }
    }

    public class FM_CashSweepLogRepository : Repository<FM_CashSweepLog, string, Nxin_Qlw_BusinessContext>, IFM_CashSweepLogRepository
    {

        public FM_CashSweepLogRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public List<FM_CashSweepLog> GetByIdAndBatchNo(string id, string batchNo, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_CashSweepLog>().Where(o => o.NumericalOrder == id&&o.BatchNo== batchNo)?.ToList();
        }

    }
}
