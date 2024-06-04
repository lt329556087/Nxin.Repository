using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceipt;
using FinanceManagement.Common;
using FinanceManagement.Common.MakeVoucherCommon;
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
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface
{
    [Route("api/[controller]")]
    [ApiController]
    public class FD_SettleReceiptInterfaceController : ControllerBase
    {
        IMediator _mediator;
        FD_SettleReceiptInterfaceODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;

        public FD_SettleReceiptInterfaceController(IMediator mediator, FD_SettleReceiptInterfaceODataProvider provider,  IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            _provider = provider;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }
        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_SettleReceiptInterfaceAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //删除
        [HttpDelete]
        public async Task<Result> Delete([FromBody] FD_SettleReceiptInterfaceDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_SettleReceiptInterfaceModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
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
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }

        /// <summary>
        /// 查询 按单位日期
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetDataList")]
        public Result GetDataList(SettleSearch model)
        {
            var result = new Result();
            var data = _provider.GetDataList(model)?.ToList();
            if (data?.Count > 0)
            {
                foreach(var item in data)
                {
                    var details = _provider.GetDetaiDatasAsync(long.Parse( item.NumericalOrder))?.Result;
                    item.Lines = details;
                }
            }
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
    }
}
