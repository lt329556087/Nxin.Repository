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
    public class FmCostprojectRepository : Repository<FmCostproject, string, Nxin_Qlw_BusinessContext>, IFmCostprojectRepository
    {
        public FmCostprojectRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FmCostprojectDetailRepository : Repository<FmCostprojectdetail, int, Nxin_Qlw_BusinessContext>, IFmCostprojectDetailRepository
    {
        public FmCostprojectDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FmCostprojectExtendRepository : Repository<FmCostprojectExtend, int, Nxin_Qlw_BusinessContext>, IFmCostprojectExtendRepository
    {
        public FmCostprojectExtendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
