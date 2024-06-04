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
    public class fm_expensereportRepository : Repository<fm_expensereport, string, Nxin_Qlw_BusinessContext>, Ifm_expensereportRepository
    {
        public fm_expensereportRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class fm_expensereportdetailRepository : Repository<fm_expensereportdetail, int, Nxin_Qlw_BusinessContext>, Ifm_expensereportdetailRepository
    {
        public fm_expensereportdetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class fm_expensereportdetaillogRepository : Repository<fm_expensereportdetaillog, int, Nxin_Qlw_BusinessContext>, Ifm_expensereportdetaillogRepository
    {
        public fm_expensereportdetaillogRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class fm_expensereportextendRepository : Repository<fm_expensereportextend, int, Nxin_Qlw_BusinessContext>, Ifm_expensereportextendRepository
    {
        public fm_expensereportextendRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

    public class fm_expensereportextendlistRepository : Repository<fm_expensereportextendlist, int, Nxin_Qlw_BusinessContext>, Ifm_expensereportextendlistRepository
    {
        public fm_expensereportextendlistRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
