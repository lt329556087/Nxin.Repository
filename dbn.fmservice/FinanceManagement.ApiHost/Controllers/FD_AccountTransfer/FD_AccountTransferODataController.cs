using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_AccountTransfer
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AccountTransferODataController : ODataController
    {
        FD_AccountTransferODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        private AuditUtil _auditUtil;

        public FD_AccountTransferODataController(FD_AccountTransferODataProvider prodiver, AuditUtil auditUtil)
        {
            _prodiver = prodiver;
            _auditUtil = auditUtil;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IEnumerable<FD_AccountTransferODataEntity> Get(ODataQueryOptions<FD_AccountTransferODataEntity> odataqueryoptions, Uri uri)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>() {
            {6,"无审批" },
            {5,"未审批" },
            {4,"审批中" },
            {3,"拒绝" },
            {2,"驳回" },
            {1,"通过" },
            };

            var list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value)).ToList();
            var listNum=new List<string>();
            //查询资金归集生成的调拨单 审批流程是资金归集的审批
            if (list?.Count > 0)
            {
                var gjList = list.Where(p => !string.IsNullOrEmpty(p.NumericalOrderForCashSweep) && p.NumericalOrderForCashSweep != "0");
                var noGjList= list.Where(p => string.IsNullOrEmpty(p.NumericalOrderForCashSweep) || p.NumericalOrderForCashSweep == "0");
                listNum=gjList?.Select(p=>p.NumericalOrderForCashSweep).ToList();
                if(noGjList?.Count() > 0)
                {
                    listNum.AddRange(noGjList.Select(p=>p.NumericalOrder));
                }
            }
           
            //listNum = list.Select(o => o.NumericalOrder).ToList();
            if (listNum.Count > 0)
            {
                var listAudit = _auditUtil.GetAuditList(listNum).Result.data;
                list.ForEach(o =>
                {
                    if(o.NumericalOrder== "2211030930100000150")
                    {

                    }
                    var audit = listAudit.Find(n => n.NumericalOrder == o.NumericalOrder||n.NumericalOrder==o.NumericalOrderForCashSweep);
                    //var audit = listAudit.Find(n => n.NumericalOrder == o.NumericalOrder);
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
            return list.AsQueryable();
        }
        #endregion 
    }
}
