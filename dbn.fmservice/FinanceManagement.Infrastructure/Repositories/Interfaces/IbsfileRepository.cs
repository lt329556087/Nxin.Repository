using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IbsfileRepository : IRepository<bsfile, string>
    {
        List<bsfile> GetBsfiles(string nums);
    }
}
