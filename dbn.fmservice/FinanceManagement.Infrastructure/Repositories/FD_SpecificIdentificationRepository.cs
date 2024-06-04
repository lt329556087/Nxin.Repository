using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class FD_SpecificIdentificationRepository : Repository<FD_SpecificIdentification, string, Nxin_Qlw_BusinessContext>, IFD_SpecificIdentificationRepository
    {

        public FD_SpecificIdentificationRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FD_SpecificIdentification> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SpecificIdentification>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

        public List<FD_SpecificIdentification> GetIdentitys(string enterpriseId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SpecificIdentification>().Where(o => o.EnterpriseID == enterpriseId && (o.DataDate >= startDate && o.DataDate <= endDate)).Include(o => o.Lines).ToList();
        }
    }

    public class FD_SpecificIdentificationDetailRepository : Repository<FD_SpecificIdentificationDetail, string, Nxin_Qlw_BusinessContext>, IFD_SpecificIdentificationDetailRepository
    {
        public FD_SpecificIdentificationDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public override Task<FD_SpecificIdentificationDetail> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_SpecificIdentificationDetail>().Include(o => o.AgingList).FirstOrDefaultAsync(o => o.NumericalOrderDetail == id);
        }
    }

    public class FD_SpecificIdentificationExtRepository : Repository<FD_SpecificIdentificationExt, string, Nxin_Qlw_BusinessContext>, IFD_SpecificIdentificationExtRepository
    {
        public FD_SpecificIdentificationExtRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
    }

}
