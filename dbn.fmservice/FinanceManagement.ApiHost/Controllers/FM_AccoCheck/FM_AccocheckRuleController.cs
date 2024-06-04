using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_AccoCheck;
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

namespace FinanceManagement.ApiHost.Controllers.FM_Accocheck
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_AccoCheckRuleController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMAPIService FMAPIService;
        FMBaseCommon _baseUnit;

        public FM_AccoCheckRuleController(IMediator mediator, FMAPIService FMAPIService, FMBaseCommon baseUnit, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _mediator = mediator;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            this.FMAPIService = FMAPIService;
        }
        //新增
        [HttpPost]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Add([FromBody] FM_AccoCheckRuleAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FM_AccoCheckRuleDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FM_AccoCheckRuleModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
