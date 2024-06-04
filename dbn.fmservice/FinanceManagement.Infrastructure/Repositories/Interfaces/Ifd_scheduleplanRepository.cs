using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifd_scheduleplanRepository : IRepository<fd_scheduleplan, string>
    {
        List<fd_scheduleplan> GetList(string GroupId);

    }
}
