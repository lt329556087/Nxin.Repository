using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace FinanceManagement.Infrastructure.Repositories
{
    public class FM_CostParamsSetRepository : Repository<FmCostparamsset, string, Nxin_Qlw_BusinessContext>, IFM_CostParamsSetRepository
    {
        public FM_CostParamsSetRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FM_CostParamsSetDetailRepository : Repository<FmCostparamssetdetail, int, Nxin_Qlw_BusinessContext>, IFM_CostParamsSetDetailRepository
    {
        public FM_CostParamsSetDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FM_CostParamsSetExtendRepository : Repository<FmCostparamssetextend, int, Nxin_Qlw_BusinessContext>, IFM_CostParamsSetExtendRepository
    {
        public FM_CostParamsSetExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
