using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface Ifm_expensereportRepository : IRepository<fm_expensereport, string>
    {
    }

    public interface Ifm_expensereportdetailRepository : IRepository<fm_expensereportdetail, int>
    {
    }

    public interface Ifm_expensereportdetaillogRepository : IRepository<fm_expensereportdetaillog, int>
    {
    }

    public interface Ifm_expensereportextendRepository : IRepository<fm_expensereportextend, int>
    {
    }

    public interface Ifm_expensereportextendlistRepository : IRepository<fm_expensereportextendlist, int>
    {
    }
}
