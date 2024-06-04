using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFmCostprojectRepository : IRepository<FmCostproject, string>
    {
    }

    public interface IFmCostprojectDetailRepository : IRepository<FmCostprojectdetail, int>
    {
    }

    public interface IFmCostprojectExtendRepository : IRepository<FmCostprojectExtend, int>
    { 
    }
}
