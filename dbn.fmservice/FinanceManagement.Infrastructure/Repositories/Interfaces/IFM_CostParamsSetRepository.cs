using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFM_CostParamsSetRepository : IRepository<FmCostparamsset, string>
    {
    }

    public interface IFM_CostParamsSetDetailRepository : IRepository<FmCostparamssetdetail, int>
    {
    }

    public interface IFM_CostParamsSetExtendRepository : IRepository<FmCostparamssetextend, int>
    {
    }
}
