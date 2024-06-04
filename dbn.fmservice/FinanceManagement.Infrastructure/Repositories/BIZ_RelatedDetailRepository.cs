using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories
{
    public class BIZ_RelatedDetailRepository : Repository<biz_relateddetail, string, Nxin_Qlw_BusinessContext>, IBiz_RelatedDetailRepository
    {
        public BIZ_RelatedDetailRepository(Nxin_Qlw_BusinessContext context) : base(context)
        {
        }

        public biz_relateddetail GetDataByRelatedId(string relatedID)
        {
            return Set.Where(m => m.RelatedID == Convert.ToInt64(relatedID)).FirstOrDefault();
        }
    }
}
