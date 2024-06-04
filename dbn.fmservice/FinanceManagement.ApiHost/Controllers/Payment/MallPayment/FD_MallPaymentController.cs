using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.MallPayment;
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

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_MallPaymentController : ControllerBase
    {
        IMediator _mediator;
        public FD_MallPaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }
       

        #region 获取商城接口
        [HttpPost]
        [Route("GetList")]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<GatewayResultModel> GetList([FromBody] MallPaymentRequest request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        #endregion
    }
}
