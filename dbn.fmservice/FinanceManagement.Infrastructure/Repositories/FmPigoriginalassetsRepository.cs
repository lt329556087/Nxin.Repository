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
    public class FmPigoriginalassetsRepository : Repository<FmPigoriginalassets, string, Nxin_Qlw_BusinessContext>, IFmPigoriginalassetsRepository
    {
        public FmPigoriginalassetsRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FmPigoriginalassetsdetailRepository : Repository<FmPigoriginalassetsdetail, string, Nxin_Qlw_BusinessContext>, IFmPigoriginalassetsdetailRepository
    {
        public FmPigoriginalassetsdetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class FmPigoriginalassetsdetaillistRepository : Repository<FmPigoriginalassetsdetaillist, int, Nxin_Qlw_BusinessContext>, IFmPigoriginalassetsdetaillistRepository
    {
        public FmPigoriginalassetsdetaillistRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
