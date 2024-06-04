using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Biz_EnterpriseperiodAPIController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMAPIService FMAPIService;
        FMBaseCommon _baseUnit;

        public Biz_EnterpriseperiodAPIController(IMediator mediator, FMAPIService FMAPIService, FMBaseCommon baseUnit, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _mediator = mediator;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            this.FMAPIService = FMAPIService;
        }
        [HttpPost]
        [Route("CreatePeriodByYear")]
        public Biz_EnterprisePeriodInfo CreatePeriodByYear(Biz_EnterprisePeriodInfo model)
        {
            AuthenticationHeaderValue authentication = null;
            bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
            if (!verification)
            {
                return new Biz_EnterprisePeriodInfo() { };
            }
            model.EnterpriseID = _identityService.EnterpriseId;
            model.BO_ID = _identityService.UserId;
            return _baseUnit.CreatePeriodByYear(model);
        }
        /// <summary>
        /// 修改建账日期时，判断猪场设置的建账日期
        /// </summary>
        /// <param name="JsonData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidatePigFarmCreatedDate")]
        public FMResultModel ValidatePigFarmCreatedDate(Biz_EnterprisePeriodInfo JsonData)
        {
            return _baseUnit.ValidatePigFarmCreatedDate(JsonData);
        }
        [HttpPost]
        [Route("Post")]
        public int Post(Biz_EnterprisePeriodInfo JsonData)
        {
            return _baseUnit.Post(JsonData);
        }
    }
}
