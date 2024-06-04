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
    public class fm_transferrealtimeRepository : Repository<fm_transferrealtime, string, Nxin_Qlw_BusinessContext>, Ifm_transferrealtimeRepository
    {
        public fm_transferrealtimeRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
    public class fm_transferrealtimedetailRepository : Repository<fm_transferrealtimedetail, string, Nxin_Qlw_BusinessContext>, Ifm_transferrealtimedetailRepository
    {
        public fm_transferrealtimedetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
    public class fm_voucheramortizationrelatedRepository : Repository<fm_voucheramortizationrelated, string, Nxin_Qlw_BusinessContext>, Ifm_voucheramortizationrelatedRepository
    {
        public fm_voucheramortizationrelatedRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        /// <summary>
        /// 根据会计凭证流水号获取摊销数据
        /// 决定是否取消摊销
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<fm_voucheramortizationrelated> GetBySettle(long num = 0)
        {
            return this.DbContext.Set<fm_voucheramortizationrelated>().Where(m=>m.NumericalOrderSettl == num).ToList();
        }
    }
}
