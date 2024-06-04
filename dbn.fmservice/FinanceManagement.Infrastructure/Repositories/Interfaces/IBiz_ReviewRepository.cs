using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IBiz_ReviewRepository : IRepository<Biz_Review, int>
    {
        /// <summary>
        /// 删除审核，checkmark=16
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task DeleteAudit(string numericalOrder);
        /// <summary>
        /// 删除所有审核
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task DeleteByNumericalOrder(string numericalOrder);
        /// <summary>
        /// 删除财务审核，checkmark=2048
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task DeleteFinanceAudit(string numericalOrder);
        /// <summary>
        /// 删除制单，checkmark=65536
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task DeleteMaking(string numericalOrder);
        /// <summary>
        /// 删除仓库审核，checkmark=4096
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task DeleteWarehouseAudit(string numericalOrder);
        /// <summary>
        /// 审核，checkmark=16
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task<Biz_Review> GetAudit(string numericalOrder);
        /// <summary>
        /// 所有审核
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        IQueryable<Biz_Review> GetByNumericalOrder(string numericalOrder);
        /// <summary>
        /// 财务审核，checkmark=4096
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task<Biz_Review> GetFinanceAudit(string numericalOrder);
        /// <summary>
        /// 制单，checkmark=65536
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task<Biz_Review> GetMaking(string numericalOrder);
        /// <summary>
        /// 仓库审核，checkmark=4096
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        Task<Biz_Review> GetWarehouseAudit(string numericalOrder);
    }
}
