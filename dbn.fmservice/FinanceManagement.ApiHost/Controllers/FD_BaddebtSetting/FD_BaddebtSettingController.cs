using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BaddebtSetting;
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
    public class FD_BaddebtSettingController : ControllerBase
    {
        IMediator _mediator;
        FD_BaddebtSettingODataProvider _provider;

        public FD_BaddebtSettingController(IMediator mediator, FD_BaddebtSettingODataProvider provider)
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
            var data = await _provider.GetDataAsync(key); 
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        #region
        ///// <summary>
        ///// 获取
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //[HttpGet("GetDetailList/{key}")]
        //public async Task<Result> GetDetailList(long key)
        //{
        //    var result = new Result();
        //    var agingList = _provider.GetAgingDetaiDatasAsync(key);
        //    if(agingList==null|| agingList.Count < 1)
        //    {
        //        result.msg = "请先设置[账龄区间设置]";
        //        return result;
        //    }
        //    var list = await _provider.GetDataByEnter(key);  
        //    foreach(var dataAitem in list)
        //    {
        //        var dataA = dataAitem;
        //        //新增
        //        if (dataA == null || string.IsNullOrEmpty(dataA.NumericalOrder) || dataA.NumericalOrder == "0")
        //        {
        //            if (dataA == null) dataA = new FD_BaddebtSettingODataEntity();
        //            dataA.DataStatus = "A";
        //            var dataB = await _provider.GetDetaiDatasAsync(key);
        //            dataB.ForEach(p => p.RowStatus = "D");
        //            foreach (var aging in agingList)
        //            {
        //                var newItem = aging;
        //                newItem.RowStatus = "A";
        //                dataB.Add(newItem);
        //            }
        //            dataA.Lines = dataB;
        //        }
        //        else
        //        {
        //            dataA.DataStatus = "M";
        //            var dataB = await _provider.GetDetaiListAsync(dataA.NumericalOrder);
        //            foreach (var item in dataB)
        //            {
        //                var filterItem = agingList.Where(p => p.AgingIntervalID == item.AgingIntervalID);
        //                if (filterItem == null || filterItem.Count() < 1)//删除
        //                {
        //                    item.RowStatus = "D";
        //                }
        //            }
        //            foreach (var aging in agingList)
        //            {
        //                var item = dataB.Where(p => p.AgingIntervalID == aging.AgingIntervalID);
        //                if (item == null || item.Count() < 1)//新增
        //                {
        //                    var newItem = aging;
        //                    newItem.RowStatus = "A";
        //                    dataB.Add(newItem);
        //                }
        //            }
        //            dataA.Lines = dataB;
        //        }

        //        result.data = dataA;

        //    }
            
        //    return result;
        //}
        #endregion
        //增加
        [HttpPost]
        [PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FD_BaddebtSettingAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BaddebtSettingDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FD_BaddebtSettingModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
       
        [HttpGet("GetDataByEnterId")]
        public FD_BaddebtSettingODataEntity GetDataByEnterId(long enterpriseId, string date)
        {
            return _provider.GetDataByEnterId(enterpriseId,date);
        }
    }

}
