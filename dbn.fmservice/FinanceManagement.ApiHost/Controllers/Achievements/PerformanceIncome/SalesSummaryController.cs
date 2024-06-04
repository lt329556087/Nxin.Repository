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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesSummaryController : ControllerBase
    {
        SalesSummaryODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        FMBaseCommon _fmBaseCommon;

        public SalesSummaryController(SalesSummaryODataProvider prodiver, FMBaseCommon fmBaseCommon,IIdentityService identityService)
        {
            _prodiver = prodiver;
            _fmBaseCommon = fmBaseCommon;
            _identityService = identityService;
        }

        #region 列表查询
        [HttpPost]
        [Route("GetSalesSummaryData")]
        public dynamic GetSalesSummaryData(SalesSummarySearch model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            model.EnterpriseIds = entes;
            return _prodiver.GetSalesSummaryData(model);
        }
        [HttpPost]
        [Route("GetSalesSummaryDataTest")]
        public dynamic GetSalesSummaryDataTest(SalesSummarySearch model)
        {
            if (string.IsNullOrEmpty(model.EnterpriseIds))
            {
                string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
                model.EnterpriseIds = entes;
            }
            return _prodiver.GetSalesSummaryDataTest(model);
        }
        /// <summary>
        /// 绩效专用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSalesInnerPerformanceData")]
        [AllowAnonymous]

        public dynamic GetSalesInnerPerformanceData(SalesSummarySearch model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(model.EnterpriseId, model.UserId);
            model.EnterpriseIds = entes;
            return _prodiver.GetSalesInnerPerformanceData(model);
        }
        #endregion 

    }
}
