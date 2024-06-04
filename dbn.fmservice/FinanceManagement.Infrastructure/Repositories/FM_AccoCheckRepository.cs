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
    public class FM_AccoCheckRepository : Repository<FM_AccoCheck, string, Nxin_Qlw_BusinessContext>, IFM_AccoCheckRepository
    {
        public FM_AccoCheckRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FM_AccoCheck> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_AccoCheck>().FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
        public Task<List<FM_AccoCheck>> GetListByYear(int year,string enterpriseID)
        {
            return DbContext.Set<FM_AccoCheck>().Where(s => s.DataDate.Year == year&&s.EnterpriseID== enterpriseID).ToListAsync();
        }
    }

    public class FM_AccoCheckDetailRepository : Repository<FM_AccoCheckDetail, string, Nxin_Qlw_BusinessContext>, IFM_AccoCheckDetailRepository
    {
        public FM_AccoCheckDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public  Task<List< FM_AccoCheckDetail>> GetListAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_AccoCheckDetail>().Where(o => o.NumericalOrder == id).ToListAsync();
        }

    }
    public class FM_AccoCheckExtendRepository : Repository<FM_AccoCheckExtend, string, Nxin_Qlw_BusinessContext>, IFM_AccoCheckExtendRepository
    {
        public FM_AccoCheckExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<FM_AccoCheckExtend>> GetExtends(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_AccoCheckExtend>().Where(s => s.NumericalOrder == id).ToListAsync();
        }
    }
    public class FM_AccoCheckRuleRepository : Repository<FM_AccoCheckRule, string, Nxin_Qlw_BusinessContext>, IFM_AccoCheckRuleRepository
    {
        public FM_AccoCheckRuleRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FM_AccoCheckRule> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FM_AccoCheckRule>().FirstOrDefaultAsync(o => o.RecordID.ToString() == id);
        }
        public Task<List<FM_AccoCheckRule>> GetListByEnter( string enterpriseID)
        {
            return DbContext.Set<FM_AccoCheckRule>().Where(s => s.EnterpriseID == enterpriseID).ToListAsync();
        }
    }

}
