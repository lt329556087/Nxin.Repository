using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_SpecificIdentificationRepository : IRepository<FD_SpecificIdentification, string>
    {
        public List<FD_SpecificIdentification> GetIdentitys(string enterpriseId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    }



    public interface IFD_SpecificIdentificationDetailRepository : IRepository<FD_SpecificIdentificationDetail, string>
    {
    }


    public interface IFD_SpecificIdentificationExtRepository : IRepository<FD_SpecificIdentificationExt, string>
    {
    }
    
}
