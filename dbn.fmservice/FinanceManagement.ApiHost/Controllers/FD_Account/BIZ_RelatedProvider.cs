using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class BIZ_RelatedODataProvider
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public BIZ_RelatedODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        ///获取资金账户关系  账户用途、结算方式
        public Task<List<BIZ_RelatedODataEntity>> GetRelatedAsync(BIZ_RelatedODataEntity model)
        {
            var strSql = new StringBuilder();
            //StringBuilder strWhere = new StringBuilder();
            strSql.Append(@"select RelatedID,CONVERT(RelatedType USING utf8mb4) RelatedType,CONVERT(ParentType USING utf8mb4) ParentType,CONVERT(ChildType USING utf8mb4) ChildType,CONVERT(ParentValue USING utf8mb4) ParentValue,CONVERT(ChildValue USING utf8mb4) ChildValue,
                            CONVERT(ParentValueDetail USING utf8mb4) ParentValueDetail,CONVERT(ChildValueDetail USING utf8mb4) ChildValueDetail, Remarks  ");
            strSql.Append("  from nxin_qlw_business.BIZ_Related ");
            strSql.AppendFormat(" where RelatedType={0} ",model.RelatedType);
            if (!string.IsNullOrEmpty(model.ParentType))
            {
                strSql.AppendFormat( " and ParentType={0} " ,model.ParentType);
            }
            if (!string.IsNullOrEmpty(model.ChildType))
            {
                strSql.AppendFormat(" and ChildType= {0}",model.ChildType);
            }
            if (!string.IsNullOrEmpty(model.ParentValue))
            {
                strSql.AppendFormat(" and ParentValue= {0}",model.ParentValue);
            }
            if (!string.IsNullOrEmpty(model.ChildValue))
            {
                strSql.AppendFormat(" and ChildValue={0} ",model.ChildValue);
            }           
            if (!string.IsNullOrEmpty(model.ParentValueDetail))
            {
                strSql.AppendFormat(" and ParentValueDetail={0} ", model.ParentValueDetail);
            }
            if (!string.IsNullOrEmpty(model.ChildValueDetail))
            {
                strSql.AppendFormat(" and ChildValueDetail={0} ", model.ChildValueDetail);
            }
            if (!string.IsNullOrEmpty(model.Remarks))
            {
                strSql.AppendFormat(" and Remarks={0} ", model.Remarks);
            }
            return _context.BIZ_RelatedDataSet.FromSqlRaw(strSql.ToString()).ToListAsync();
        }
    }
}
