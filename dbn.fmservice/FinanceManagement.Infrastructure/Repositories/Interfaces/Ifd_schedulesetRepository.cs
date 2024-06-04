using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_schedulesetRepository : IRepository<fd_scheduleset, string>
    {
        List<fd_scheduleset> GetList(string GroupId);
    }
}
