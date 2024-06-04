using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IMS_FormulaRepository : IRepository<MS_Formula, string>
    {
        
    }

    public interface IMS_FormulaDetailRepository : IRepository<MS_FormulaDetail, string>
    {
        Task<List<MS_FormulaDetail>> GetListAsync(string id, CancellationToken cancellationToken = default);
    }
    public interface IMS_FormulaExtendRepository : IRepository<MS_FormulaExtend, string>
    {
        Task<List<MS_FormulaExtend>> GetExtends(string id, CancellationToken cancellationToken = default);
    }
   
}
