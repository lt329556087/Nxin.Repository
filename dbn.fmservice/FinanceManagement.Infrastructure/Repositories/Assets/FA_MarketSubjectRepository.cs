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
    public class FA_MarketSubjectRepository : Repository<FA_MarketSubject, string, Nxin_Qlw_BusinessContext>, IFA_MarketSubjectRepository
    {

        public FA_MarketSubjectRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FA_MarketSubject> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FA_MarketSubject>().Include(o => o.Details).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

    public class FA_MarketSubjectDetailRepository : Repository<FA_MarketSubjectDetail, string, Nxin_Qlw_BusinessContext>, IFA_MarketSubjectDetailRepository
    {
        public FA_MarketSubjectDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }
}
