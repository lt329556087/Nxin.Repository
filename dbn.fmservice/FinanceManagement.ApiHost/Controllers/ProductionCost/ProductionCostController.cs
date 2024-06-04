﻿using Architecture.Seedwork.Security;
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
    public class ProductionCostController : ControllerBase
    {
        ProductionCostODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        FMBaseCommon _fmBaseCommon;

        public ProductionCostController(ProductionCostODataProvider prodiver, FMBaseCommon fmBaseCommon,IIdentityService identityService)
        {
            _prodiver = prodiver;
            _fmBaseCommon = fmBaseCommon;
            _identityService = identityService;
        }

        #region 列表查询
        [HttpPost]
        [Route("GetProductionCostData")]
        public dynamic GetProductionCostData(dynamic model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            if (model.EnterSortIds == null)
            {

            }
            if (string.IsNullOrEmpty(model.EnterSortIds.ToString()))
            {
                model.EnterSortIds = entes;
            }
            return _prodiver.GetProductionCostData(model);
        }
        #endregion 
    }
}
