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
    public class FD_SettleReceiptRepository : Repository<FD_SettleReceipt, string, Nxin_Qlw_BusinessContext>, IFD_SettleReceiptRepository
    {
        public FD_SettleReceiptRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_SettleReceipt> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SettleReceipt>().Include(o => o.details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

        public List<FD_SettleReceipt> GetByNums(string nums)
        {
            return DbContext.Set<FD_SettleReceipt>().Where(o => nums.Contains(o.NumericalOrder)).ToList();
        }

        public Task<FD_SettleReceipt> GetDataAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SettleReceipt>().FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
        public List<FD_SettleReceipt> GetDataList(string beginDate = "",string endDate = "",string enterpriseId = "", CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SettleReceipt>().Where(m=>m.DataDate >= Convert.ToDateTime(beginDate) && m.DataDate <= Convert.ToDateTime(endDate) && m.EnterpriseID == enterpriseId
            && m.SettleReceipType != "201610220104402204"
            && m.SettleReceipType != "201610220104402205"
            && m.SettleReceipType != "201610220104402206").ToList();
        }
    }

    public class FD_SettleReceiptDetailRepository : Repository<FD_SettleReceiptDetail, string, Nxin_Qlw_BusinessContext>, IFD_SettleReceiptDetailRepository
    {
        public FD_SettleReceiptDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {            
        }

        public List<FD_SettleReceiptDetail> GetByNums(string nums)
        {
            return DbContext.Set<FD_SettleReceiptDetail>().Where(o => nums.Contains(o.NumericalOrder)).ToList();
        }

        public Task<FD_SettleReceiptDetail> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SettleReceiptDetail>().FirstOrDefaultAsync(o => o.RecordID == id);
        }
    }
}
