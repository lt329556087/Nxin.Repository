using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System; 
using System.Linq; 
using System.Threading.Tasks; 

namespace FinanceManagement.Infrastructure.Repositories
{
    public class Biz_ReviewRepository : Repository<Biz_Review, int, Nxin_Qlw_BusinessContext>, IBiz_ReviewRepository
    {
        public Biz_ReviewRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public IQueryable<Biz_Review> GetByNumericalOrder(string numericalOrder)
        {
            if (numericalOrder.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(numericalOrder));
            }

            return Set.Where(o => o.NumericalOrder == numericalOrder);
        }
        public Task<Biz_Review> GetMaking(string numericalOrder)
        {
            return GetByNumericalOrder(numericalOrder).Where(o => o.CheckMark == ReviewCode.制单.GetIntValue()).FirstOrDefaultAsync();
        }
        public Task<Biz_Review> GetAudit(string numericalOrder)
        {
            return GetByNumericalOrder(numericalOrder).Where(o => o.CheckMark == ReviewCode.审核.GetIntValue()).FirstOrDefaultAsync();
        }
        public Task<Biz_Review> GetFinanceAudit(string numericalOrder)
        {
            return GetByNumericalOrder(numericalOrder).Where(o => o.CheckMark == ReviewCode.财务审核.GetIntValue()).FirstOrDefaultAsync();
        }
        public Task<Biz_Review> GetWarehouseAudit(string numericalOrder)
        {
            return GetByNumericalOrder(numericalOrder).Where(o => o.CheckMark == ReviewCode.仓库审核.GetIntValue()).FirstOrDefaultAsync();
        }

        public Task DeleteByNumericalOrder(string numericalOrder)
        {
            if (numericalOrder.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(numericalOrder));
            }
            return RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
        }
        public Task DeleteMaking(string numericalOrder)
        {
            return RemoveRangeAsync(o => o.NumericalOrder == numericalOrder && o.CheckMark == ReviewCode.制单.GetIntValue());
        }
        public Task DeleteAudit(string numericalOrder)
        {
            return RemoveRangeAsync(o => o.NumericalOrder == numericalOrder && o.CheckMark == ReviewCode.审核.GetIntValue());
        }
        public Task DeleteFinanceAudit(string numericalOrder)
        {
            return RemoveRangeAsync(o => o.NumericalOrder == numericalOrder && o.CheckMark == ReviewCode.财务审核.GetIntValue());
        }
        public Task DeleteWarehouseAudit(string numericalOrder)
        {
            return RemoveRangeAsync(o => o.NumericalOrder == numericalOrder && o.CheckMark == ReviewCode.仓库审核.GetIntValue());
        }
    }
}
