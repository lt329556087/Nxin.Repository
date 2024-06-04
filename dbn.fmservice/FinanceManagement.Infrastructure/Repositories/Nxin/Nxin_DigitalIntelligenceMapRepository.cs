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
    public class Nxin_DigitalIntelligenceMapRepository : Repository<Nxin_DigitalIntelligenceMap, string, Nxin_Qlw_BusinessContext>, INxin_DigitalIntelligenceMapRepository
    {

        public Nxin_DigitalIntelligenceMapRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<Nxin_DigitalIntelligenceMap> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<Nxin_DigitalIntelligenceMap>().FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }
    }

}
