using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Common.Application.Query;

namespace FinanceManagement.ApiHost.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AuditODataController : ODataController
    {
        FD_CapitalBudgetODataProvider _provider;
        AuditUtil _auditUtil;

        public FD_AuditODataController(FD_CapitalBudgetODataProvider provider, AuditUtil auditUtil)
        {
            _provider = provider;
            _auditUtil = auditUtil;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataqueryoptions"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        [EnableQuery]
        public async Task<IEnumerable<AuditODataEntity>> Get(ODataQueryOptions<AuditODataEntity> odataqueryoptions, Uri uri)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>() {
            {6,"无审批" },
            {5,"未审批" },
            {4,"审批中" },
            {3,"拒绝" },
            {2,"驳回" },
            {1,"通过" },
            };
            var data =await _provider.GetAudit(odataqueryoptions, uri);
            var list = data.Select(o => o.NumericalOrder).ToList();
            if (list.Count > 0)
            {
                var listAudit = _auditUtil.GetAuditList(list).Result.data;
                data.ForEach(o =>
                {
                    var audit = listAudit.Find(i => i.NumericalOrder == o.NumericalOrder);
                    if (audit != null)
                    {
                        audit.AuditStatus = audit.AuditStatus == 0 ? 6 : audit.AuditStatus;
                        o.ApprovalStateName = dic[audit.AuditStatus];
                        o.ApprovalState = audit.AuditStatus;
                    }
                    else
                    {
                        o.ApprovalState = 6;
                        o.ApprovalStateName = "无审批";
                    }
                });
            }

            return data;
        }


    }
}
