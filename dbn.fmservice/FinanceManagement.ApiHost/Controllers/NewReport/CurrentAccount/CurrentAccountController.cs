using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_CashSweep;
using FinanceManagement.Common;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Codeless.Report;
using Newtonsoft.Json;
using FinanceManagement.ApiHost.Controllers.CurrentAccount;
using Architecture.Seedwork.Security;
using Architecture.Common.HttpClientUtil;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentAccountController : ControllerBase
    {
        IMediator _mediator;
        private readonly FinanceTradeUtil _financeTradeUtil;
        private IIdentityService _identityService;
        private HostConfiguration _hostCongfiguration;
        private HttpClientUtil _httpClientUtil1;
        public CurrentAccountController(IMediator mediator, FinanceTradeUtil financeTradeUtil, IIdentityService identityService, HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil1)
        {
            _mediator = mediator;
            _financeTradeUtil = financeTradeUtil;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil1;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("data")]
        //[AppCode(CommonField.MenuId)]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<SettleReceiptBalance.GatewayResultModel> GetData([FromBody] CurrentAccountQueryCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
