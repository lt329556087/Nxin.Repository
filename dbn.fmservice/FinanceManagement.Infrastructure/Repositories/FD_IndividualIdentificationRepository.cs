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
    public class FD_IndividualIdentificationRepository : Repository<FD_IndividualIdentification, string, Nxin_Qlw_BusinessContext>, IFD_IndividualIdentificationRepository
    {

        public FD_IndividualIdentificationRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public override Task<FD_IndividualIdentification> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<FD_IndividualIdentification>().Include(o => o.Lines).FirstOrDefaultAsync(o => o.NumericalOrder == id);
        }

        //public List<FD_IndividualIdentification> GetIdentitys(string enterpriseId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        //{
        //    return DbContext.Set<FD_IndividualIdentification>().Where(o => o.EnterpriseID == enterpriseId && (o.DataDate >= startDate && o.DataDate <= endDate)).Include(o => o.Lines).ToList();
        //}
    }

    public class FD_IndividualIdentificationDetailRepository : Repository<FD_IndividualIdentificationDetail, string, Nxin_Qlw_BusinessContext>, IFD_IndividualIdentificationDetailRepository
    {
        public FD_IndividualIdentificationDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
       
    }

    public class FD_IndividualIdentificationExtRepository : Repository<FD_IndividualIdentificationExt, string, Nxin_Qlw_BusinessContext>, IFD_IndividualIdentificationExtRepository
    {
        public FD_IndividualIdentificationExtRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }
        public Task<List<FD_IndividualIdentificationExt>> GetExtAsync(string id, string detailID, int BusiType)
        {
            return DbContext.Set<FD_IndividualIdentificationExt>().Where(o =>o.NumericalOrder==id&& o.NumericalOrderDetail == detailID && o.BusiType== BusiType)?.ToListAsync();
        }
    }

}
