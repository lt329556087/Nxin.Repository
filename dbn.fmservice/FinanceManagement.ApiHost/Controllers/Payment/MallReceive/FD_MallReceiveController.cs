using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.MallReceive;
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
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;
using FinanceManagement.Common;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_MallReceiveController : ControllerBase
    {
        IMediator _mediator;
        public FD_MallReceiveController(IMediator mediator)
        {
            _mediator = mediator;
        }


        #region 获取商城接口
        [HttpPost]
        [Route("GetList")]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<GatewayResultModel> GetList([FromBody] MallReceiveRequest request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //[HttpPost]
        //[Route("DeletRedisKey")]
        //public async Task<long> DeletRedisKey([FromBody] MallReceiveRequestBase request)
        //{
        //    string rediskey = $"SCSK:{JsonConvert.SerializeObject(request)}-GroupId:{_identityService.GroupId}-Userid={_identityService.UserId}";
        //    var csredis = new CSRedis.CSRedisClient(_hostCongfiguration.RedisServer);
        //    //初始化 RedisHelper
        //    RedisHelper.Initialization(csredis);
        //    return RedisHelper.Del(rediskey);
        //}
        #endregion
    }
}
