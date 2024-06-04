using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FA_MarketSubject;
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
    public class FA_MarketSubjectController : ControllerBase
    {
        IMediator _mediator;
        FA_MarketSubjectODataProvider _provider;

        public FA_MarketSubjectController(IMediator mediator, FA_MarketSubjectODataProvider provider)
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
            result.data = data;
            return result;
        }
        [HttpGet("GetDataByDate/{date}")]
        public FA_MarketSubjectODataEntity GetDataByDate(string date)
        {
            if (string.IsNullOrEmpty(date)) date = DateTime.Now.ToString("yyyy-MM-dd");
            var list = _provider.GetDataByDate(date).ToList();
            if (list != null && list.Count > 0)
            {
                var firstData = list[0];
                var numOrder = firstData.NumericalOrder;
                var data = _provider.GetDetaiData(long.Parse(numOrder));
                firstData.Lines = data;
                return firstData;
            }
            return null;
        }
        ///// <summary>
        ///// 根据部门ID获取该部门下的所有部门
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //[HttpGet("GetDetailList/{key}")]
        //public async Task<Result> GetDetailList(long key)
        //{
        //    var result = new Result();
        //    return result;
        //}

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FA_MarketSubjectAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FA_MarketSubjectDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FA_MarketSubjectModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        
    }
}
