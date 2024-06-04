using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FA_UseStateTransfer;
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
    public class FA_UseStateTransferController : ControllerBase
    {
        IMediator _mediator;
        FA_UseStateTransferODataProvider _provider;

        public FA_UseStateTransferController(IMediator mediator, FA_UseStateTransferODataProvider provider)
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
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            if(data!= null)
            {
                if (!string.IsNullOrEmpty(data.CheckedByName))
                {
                    data.AuditStatus = "1";
                    data.AuditStatusName = "已审核";
                }
                else
                {
                    data.AuditStatus = "2";
                    data.AuditStatusName = "未审核";
                }
            }
            
            result.data = data;
            return result;
        }
        //[HttpGet("GetDataByDate/{date}")]
        //public FA_UseStateTransferODataEntity GetDataByDate(string date)
        //{
        //    if (string.IsNullOrEmpty(date)) date = DateTime.Now.ToString("yyyy-MM-dd");
        //    var list = _provider.GetDataByDate(date).ToList();
        //    if (list != null && list.Count > 0)
        //    {
        //        var firstData = list[0];
        //        var numOrder = firstData.NumericalOrder;
        //        var data = _provider.GetDetaiData(long.Parse(numOrder));
        //        firstData.Lines = data;
        //        return firstData;
        //    }
        //    return null;
        //}
       
        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FA_UseStateTransferAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FA_UseStateTransferDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FA_UseStateTransferModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        
    }
}
