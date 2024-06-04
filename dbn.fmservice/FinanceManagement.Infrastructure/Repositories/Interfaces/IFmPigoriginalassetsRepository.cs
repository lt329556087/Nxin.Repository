using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFmPigoriginalassetsRepository : IRepository<FmPigoriginalassets, string>
    {
    }

    public interface IFmPigoriginalassetsdetailRepository : IRepository<FmPigoriginalassetsdetail, string>
    {
    }

    public interface IFmPigoriginalassetsdetaillistRepository : IRepository<FmPigoriginalassetsdetaillist, int>
    {
    }
}
