using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IBiz_RelatedDetailRepository : IRepository<biz_relateddetail, string>
    {
        biz_relateddetail GetDataByRelatedId(string relatedID);
    }
}
