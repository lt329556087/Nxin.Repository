using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FA_AssetsApply;
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

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsApply
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FA_AssetsApplyController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        FA_AssetsApplyODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;

        public FA_AssetsApplyController(IMediator mediator, FA_AssetsApplyODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public Result GetDetail(long key)
        {
            try
            {
                var result = new Result();
                var data = _provider.GetDataAsync(key).Result;
                result.data = data;
                return result;
            }
            catch (Exception ex)
            {
                return new Result() { code = -1, msg = "查询失败,请联系管理员！",data=ex };
            }
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FA_AssetsApplyAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FA_AssetsApplyDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FA_AssetsApplyModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //复制
        [HttpPost]
        [Route("Copy")]
        public async Task<Result> Copy([FromBody] FA_AssetsApplyCopyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
    }
}
