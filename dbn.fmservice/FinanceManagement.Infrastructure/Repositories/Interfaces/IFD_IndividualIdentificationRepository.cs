using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_IndividualIdentificationRepository : IRepository<FD_IndividualIdentification, string>
    {
        //public List<FD_IndividualIdentification> GetIdentitys(string enterpriseId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    }



    public interface IFD_IndividualIdentificationDetailRepository : IRepository<FD_IndividualIdentificationDetail, string>
    {
    }


    public interface IFD_IndividualIdentificationExtRepository : IRepository<FD_IndividualIdentificationExt, string>
    {
        Task<List<FD_IndividualIdentificationExt>> GetExtAsync(string id,string detailID, int BusiType);
    }
    
}
