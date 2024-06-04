using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_CashSweepSetting;
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

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CashSweepSettingController : ControllerBase
    {
        IMediator _mediator;
        FM_CashSweepSettingODataProvider _provider;

        public FM_CashSweepSettingController(IMediator mediator, FM_CashSweepSettingODataProvider provider)
        {
            _mediator = mediator;
            _provider = provider;
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
            if (data?.Lines?.Count > 0)
            {
                var extList = await _provider.GetExtendDatasAsync(key);
                if (extList?.Count > 0)
                {
                    foreach (var item in data.Lines)
                    {
                        item.Extends =extList.Where(p =>p.NumericalOrderDetail==item.NumericalOrderDetail)?.ToList();
                    }
                }                
            }
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }

        /// <summary>
        /// 按单位查询
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        [HttpGet("GetData/{EnterpriseId}")]
        public async Task<Result> GetData(string EnterpriseId)
        {
            var result = new Result();
            var data = _provider.GetDataByEnterID(EnterpriseId);
            if (data?.Count() > 0)
            {
                var model = data.FirstOrDefault();
                var num = long.Parse(model.NumericalOrder);
                var detailList = await _provider.GetDetaiDatasAsync(num);
                if (detailList?.Count > 0)
                {
                    model.Lines = detailList;
                    var extList = await _provider.GetExtendDatasAsync(num);
                    if (extList?.Count > 0)
                    {
                        foreach (var item in detailList)
                        {
                            item.Extends = extList.Where(p => p.NumericalOrderDetail == item.NumericalOrderDetail)?.ToList();
                        }
                    }
                }
                result.code = ErrorCode.Success.GetIntValue();
                result.data = model;
                return result;
            }
            result.code = ErrorCode.Success.GetIntValue();
            result.msg = "无数据";
            result.data = null;
            return result;
        }

        //增加
        [HttpPost]
        [PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FM_CashSweepSettingAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FM_CashSweepSettingDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FM_CashSweepSettingModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
           
    }
}
