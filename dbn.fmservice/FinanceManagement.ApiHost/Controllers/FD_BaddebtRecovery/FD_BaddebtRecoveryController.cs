using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BaddebtRecovery;
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
using Architecture.Seedwork.Security;
using Microsoft.AspNet.OData.Query;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Serilog.Core;
using Microsoft.Extensions.Logging;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BaddebtRecoveryController : ControllerBase
    {
        IMediator _mediator;
        FD_BaddebtRecoveryODataProvider _provider;
        private readonly ILogger<FD_BaddebtRecoveryController> _logger;
        private IIdentityService _identityService;

        public FD_BaddebtRecoveryController(IMediator mediator, FD_BaddebtRecoveryODataProvider provider, ILogger<FD_BaddebtRecoveryController> logger, IIdentityService identityService)
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _identityService = identityService;
        }


        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            result.code = ErrorCode.Success.GetIntValue();
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        [PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FD_BaddebtRecoveryAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BaddebtRecoveryDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FD_BaddebtRecoveryModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        #region 按明细删除 
        [HttpDelete("DeleteDetails")]
        public async Task<Result> DeleteDetails([FromBody] FD_BaddebtRecoveryDeleteDetailCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        #endregion
    }
}
