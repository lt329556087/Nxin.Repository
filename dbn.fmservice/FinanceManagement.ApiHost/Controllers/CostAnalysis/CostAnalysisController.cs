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

namespace FinanceManagement.ApiHost.Controllers.PerformanceIncome
{
    /// <summary>
    /// 费用分析
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CostAnalysisController : ControllerBase
    {
        CostAnalysisODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        FMBaseCommon _fmBaseCommon;

        public CostAnalysisController(CostAnalysisODataProvider prodiver, FMBaseCommon fmBaseCommon,IIdentityService identityService)
        {
            _prodiver = prodiver;
            _fmBaseCommon = fmBaseCommon;
            _identityService = identityService;
        }

        #region 列表查询
        [HttpPost]
        [Route("GetCostAnalysisData")]
        public dynamic GetCostAnalysisData(dynamic model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            if (string.IsNullOrEmpty(model.EnterpriseIds))
            {
                model.EnterpriseIds = entes;
            }
            return _prodiver.GetCostAnalysisData(model);
        }
        #endregion 
        /// <summary>
        /// 获取最大部门级次（前端使用 控制汇总方式中的内容有几级）
        /// 单位部门  非部门组织
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMaxRankByMarket")]
        public dynamic GetMaxRankByMarket(dynamic model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            if (string.IsNullOrEmpty(model.EnterpriseIds))
            {
                model.EnterpriseIds = entes;
            }
            return _prodiver.GetMaxMarketRank(model);
        }
        /// <summary>
        /// 获取最大科目级次（前端使用 控制汇总方式中的内容有几级）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMaxRankByAccSubject")]
        public dynamic GetMaxRankByAccSubject(dynamic model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            if (string.IsNullOrEmpty(model.EnterpriseIds))
            {
                model.EnterpriseIds = entes;
            }
            return _prodiver.GetMaxAccSubjectRank(model);
        }
    }
}
