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
    public class MS_FormulaRepository : Repository<MS_Formula, string, Nxin_Qlw_BusinessContext>, IMS_FormulaRepository
    {
        public MS_FormulaRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<MS_Formula> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<MS_Formula>().Include(o => o.Details).Include(o => o.Extends).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
       
    }

    public class MS_FormulaDetailRepository : Repository<MS_FormulaDetail, string, Nxin_Qlw_BusinessContext>, IMS_FormulaDetailRepository
    {
        public MS_FormulaDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public  Task<List< MS_FormulaDetail>> GetListAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<MS_FormulaDetail>().Where(o => o.NumericalOrder == id).ToListAsync();
        }

    }
    public class MS_FormulaExtendRepository : Repository<MS_FormulaExtend, string, Nxin_Qlw_BusinessContext>, IMS_FormulaExtendRepository
    {
        public MS_FormulaExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<MS_FormulaExtend>> GetExtends(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<MS_FormulaExtend>().Where(s => s.NumericalOrder == id).ToListAsync();
        }
    }
   

}
