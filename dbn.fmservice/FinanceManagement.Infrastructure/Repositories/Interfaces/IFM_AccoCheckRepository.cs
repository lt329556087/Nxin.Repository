using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFM_AccoCheckRepository : IRepository<FM_AccoCheck, string>
    {
        Task<List<FM_AccoCheck>> GetListByYear(int year, string enterpriseID);
    }

    public interface IFM_AccoCheckDetailRepository : IRepository<FM_AccoCheckDetail, string>
    {
        Task<List<FM_AccoCheckDetail>> GetListAsync(string id, CancellationToken cancellationToken = default);
    }
    public interface IFM_AccoCheckExtendRepository : IRepository<FM_AccoCheckExtend, string>
    {
        Task<List<FM_AccoCheckExtend>> GetExtends(string id, CancellationToken cancellationToken = default);
    }
    public interface IFM_AccoCheckRuleRepository : IRepository<FM_AccoCheckRule, string>
    {
        Task<List<FM_AccoCheckRule>> GetListByEnter(string enterpriseID);
    }
}
