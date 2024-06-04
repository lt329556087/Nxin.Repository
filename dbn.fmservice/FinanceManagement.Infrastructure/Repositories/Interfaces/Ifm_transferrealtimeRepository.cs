using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifm_transferrealtimeRepository : IRepository<fm_transferrealtime, string>
    {
    }
    public interface Ifm_transferrealtimedetailRepository : IRepository<fm_transferrealtimedetail, string>
    {
    }
    public interface Ifm_voucheramortizationrelatedRepository : IRepository<fm_voucheramortizationrelated, string>
    {
        /// <summary>
        /// 根据凭证流水号获取摊销信息
        /// 是否需要执行删除
        /// </summary>
        /// <returns></returns>
        public List<fm_voucheramortizationrelated> GetBySettle(long num = 0);
    }
}
