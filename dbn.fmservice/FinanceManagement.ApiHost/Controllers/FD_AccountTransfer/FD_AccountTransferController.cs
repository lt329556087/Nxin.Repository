using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_AccountTransfer;
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
using FinanceManagement.Common;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_AccountTransferController : ControllerBase
    {
        IMediator _mediator;
        FD_AccountTransferODataProvider _provider;
        AccountUtil _accountUtil;


        public FD_AccountTransferController(IMediator mediator, FD_AccountTransferODataProvider provider, AccountUtil accountUtil)
        {
            _mediator = mediator;
            _provider = provider;
            _accountUtil = accountUtil;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            var obj = data.Lines.Find(o => o.IsIn);

            data.Lines.ForEach(line =>
            {
                var account = _accountUtil.GetAccountInfo(line.AccountID).Result.FirstOrDefault();
                if (account != null)
                    line.AccountNumber = account.AccountNumber;
            });

            obj.AmountUpper = TheRMBCapital.Transform(obj.Amount.ToString());
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_AccountTransferAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_AccountTransferDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_AccountTransferModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
