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
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.BaddebtAccrualDraft;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaddebtAccrualDraftController : ControllerBase
    {
        IMediator _mediator;
        //private readonly ILogger<BaddebtAccrualDraftController> _logger;
        //private IIdentityService _identityService;      
        //HostConfiguration _hostCongfiguration;
        public BaddebtAccrualDraftController(IMediator mediator)/*,  ILogger<BaddebtAccrualDraftController> logger, IIdentityService identityService, HostConfiguration hostCongfiguration)*/
        {
            _mediator = mediator;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("data")]
        //[AppCode(CommonField.MenuId)]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<RptResult> GetData([FromBody] BaddebtAccrualDraftQueryCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
