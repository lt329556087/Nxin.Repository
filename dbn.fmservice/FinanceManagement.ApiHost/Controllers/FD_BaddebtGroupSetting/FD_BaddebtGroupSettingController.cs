using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BaddebtGroupSetting;
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
using Architecture.Common.NumericalOrderCreator;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BaddebtGroupSettingController : ControllerBase
    {
        IMediator _mediator;
        FD_BaddebtGroupSettingODataProvider _provider;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_BaddebtGroupSettingController(IMediator mediator, FD_BaddebtGroupSettingODataProvider provider, NumericalOrderCreator numericalOrderCreator)
        {
            _mediator = mediator;
            _provider = provider;
            _numericalOrderCreator = numericalOrderCreator;
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
            if (data != null && !string.IsNullOrEmpty(data.NumericalOrder))
            {
                var extends = _provider.GetExtendDatasAsync(key);
                data.Extends = extends.Result;
                var typeAndSubjects = _provider.GetIdentificationTypeAndSubjectAsync(key);
                data.TypeAndSubjects = typeAndSubjects.Result;
            }
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }

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
        //            if (dataA == null) dataA = new FD_BaddebtGroupSettingODataEntity();
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
        
        //增加
        [HttpPost]
        [PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FD_BaddebtGroupSettingAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BaddebtGroupSettingDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FD_BaddebtGroupSettingModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        /// <summary>
        /// 获取空对象
        /// </summary>
        /// <param name="enterpriseId">集团ID</param>
        /// <returns></returns>
        [HttpGet("GetAgingDetaiData/{key}")]
        public Result GetAgingDetaiData(long enterpriseId)
        {
            var result = new Result();
            var agingList = _provider.GetAgingDetaiDatasAsync(enterpriseId);
            result.data = agingList;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        [HttpGet("GetExtendData/{key}")]
        public Task<List<FD_BaddebtGroupSettingExtendODataEntity>>  GetExtendDatasAsync(long key)
        {
            return _provider.GetExtendDatasAsync(key);
        }
        [HttpGet("GetExtendsByDate")]
        public Task<List<FD_BaddebtGroupSettingExtendODataEntity>> GetExtendsByDate(string startDate, string endDate)
        {
            return _provider.GetExtendsByDate(startDate,endDate);
        }
        /// <summary>
        /// 根据单位、日期获取集团坏账区间设置
        /// </summary>
        /// <param name="date"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        [HttpGet("GetGroupSetting")]
        public async Task<Result> GetGroupSetting(string date, string enterpriseId)
        {
            var result = new Result();
            //if (string.IsNullOrEmpty(date))
            //{
            //    result.msg = "date不能为空！";
            //    return result;
            //}
            if (string.IsNullOrEmpty(enterpriseId))
            {
                result.msg = "enterpriseID不能为空！";
                return result;
            }
            var setting =await _provider.GetGroupSetting(date, enterpriseId);
            if (setting == null || setting.Count == 0)
            {
                result.data = setting;
                return result;
            }
            foreach (var item in setting)
            {
                if (item == null) continue;
                var numericalOrder = long.Parse(item?.NumericalOrder);
                var filter = await _provider.GetDetaiDatasAsync(numericalOrder);
                item.Lines = filter;
                var typeAndSubjects = _provider.GetIdentificationTypeAndSubjectAsync(numericalOrder);
                item.TypeAndSubjects = typeAndSubjects.Result;
            }          
            result.data = setting;
            result.code = ErrorCode.Success.GetIntValue();
            return result;

        }

        /// <summary>
        /// 根据单位、日期获取集团坏账区间设置 单个单子（用于账龄计提）
        /// </summary>
        /// <param name="date"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        [HttpGet("GetGroupSettingByDate")]
        public async Task<Result> GetGroupSettingByDate(string date, string enterpriseId)
        {
            var result = new Result();
            if (string.IsNullOrEmpty(enterpriseId))
            {
                result.msg = "enterpriseID不能为空！";
                return result;
            }
            var setting = await _provider.GetGroupSetting(date, enterpriseId);
            if (setting == null || setting.Count == 0)
            {
                result.data = setting;
                return result;
            }
            var data = setting.FirstOrDefault();
            var numericalOrder = long.Parse(data.NumericalOrder);
            var detail = await _provider.GetDetaiDatasAsync(numericalOrder);
            if (detail == null || detail.Count == 0)
            {
                result.data = data;
                return result;
            }
            data.Lines = detail;
            result.data = data;
            result.code = ErrorCode.Success.GetIntValue();
            return result;

        }
        /// <summary>
        /// 获取认定类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetIdentificationTypeList")]
        public async Task<Result> GetIdentificationTypeList(IdentificationTypeSearchModel model)
        {
            var result = new Result();
            var agingList =await _provider.GetIdentificationTypeList(model);
            result.data = agingList;
            result.code = ErrorCode.Success.GetIntValue();
            return result;
        }
        /// <summary>
        /// 获取集团认定类型-集团未设置过，预设值类型；设置过 集团设置项+预设值类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetIdenTypeList")]
        public async Task<Result> GetIdenTypeList(IdentificationTypeSearchModel model)
        {
            var result = new Result();
            var dataList = new List<FD_IdentificationTypeODataEntity>();
            //应收账款预置枚举值：集团内部单位/联营单位/除联营企业外的其他关联方/单项金额不重大但单项计提的单位/单项金额重大并单独计提的单位/按账龄计提的单位/自定义
            //其他应收款预置枚举值：集团内部单位/联营单位/除联营企业外的其他关联方/个别认定不计提的单位/个别认定计提的单位/按账龄计提的单位/自定义
            if (model.BusiType == 0)
            {
                for (var i = 0; i < 6; i++)
                {
                    var newItem = new FD_IdentificationTypeODataEntity();
                    var numericalOrder = _numericalOrderCreator.Create();
                    newItem.TypeID = numericalOrder;
                    newItem.TypeName = this.arryYSName[i];
                    //newItem.EnterpriseID = model.EnterpriseID;
                    //newItem.BusiType = model.BusiType;
                    ////计提方式（0：个别认定 1：账龄计提）
                    //if (newItem.TypeName== "按账龄计提的单位")
                    //{
                    //    newItem.AccrualType = 1;
                    //}
                    //else
                    //{
                    //    newItem.AccrualType = 0;
                    //}
                    dataList.Add(newItem);
                }                
            }
            else if (model.BusiType == 1)
            {
                for (var i = 0; i < 6; i++)
                {
                    var newItem = new FD_IdentificationTypeODataEntity();
                    var numericalOrder = _numericalOrderCreator.Create();
                    newItem.TypeID = numericalOrder;
                    newItem.TypeName = this.arryOtherName[i];
                    dataList.Add(newItem);
                }
            }
            
            var groupTypeList = await _provider.GetIdenTypeList(model);
            //集团已设置
            if (groupTypeList?.Count>0)
            {
                foreach(var item in groupTypeList)
                {
                    var filterList = dataList.Where(p => p.TypeName == item.TypeName);
                    if (filterList == null || filterList.Count() == 0)
                    {
                        dataList.Add(item);
                    }
                }
            }
            //dataList.Add(new FD_IdentificationTypeODataEntity()
            //{
            //    TypeID = "-1",
            //    TypeName = "自定义"
            //});
            result.code = ErrorCode.Success.GetIntValue();
            result.data = dataList;
            return result;
        }

        /// <summary>
        /// 获取预置内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetPreSetList")]
        public async Task<Result> GetPreSetList(IdentificationTypeSearchModel model)
        {
            var result = new Result();
            var dataList = new List<FD_IdentificationTypeAndSubjectODataEntity>();
            var subjectList =await _provider.GetSubjectList(model);
            if (subjectList == null || subjectList.Count == 0)
            {
                result.msg = "未获取到科目";
                return result;
            }
            if (model.BusiType == 0)
            {
                for (var i = 0; i < 7; i++)
                {
                    string AccoSubjectID=null;
                    string cAccoSubjectFullName = "";
                    if (!string.IsNullOrEmpty(arryYSSubjectName[i]))
                    {
                        var subjectFilterList = subjectList.Where(p => p.cAccoSubjectFullName == arryYSSubjectName[i]);
                        if (subjectFilterList == null || subjectFilterList.Count() == 0) continue;
                        var suject = subjectFilterList.FirstOrDefault();
                        AccoSubjectID = suject.AccoSubjectID;
                        cAccoSubjectFullName= suject.cAccoSubjectFullName;
                    }                   
                   
                    var newItem = new FD_IdentificationTypeAndSubjectODataEntity();
                    var numericalOrder = _numericalOrderCreator.Create();
                    newItem.TypeID = numericalOrder;
                    newItem.TypeName = this.arryYSTName[i];
                    newItem.BusiType = model.BusiType;
                    //计提方式（0：个别认定 1：账龄计提）
                    if (newItem.TypeName == "按账龄计提的单位")
                    {
                        newItem.AccrualType = 1;
                    }
                    else
                    {
                        newItem.AccrualType = 0;
                    }
                    newItem.AccoSubjectID = AccoSubjectID;
                    newItem.AccoSubjectFullName = cAccoSubjectFullName;
                    newItem.DataSourceType = 0;// 数据来源（0：账龄重分类）
                    newItem.DataSourceTypeName = "账龄重分类表";
                    dataList.Add(newItem);
                }
            }
            else if (model.BusiType == 1)
            {
                for (var i = 0; i < 7; i++)
                {
                    string AccoSubjectID = null;
                    string cAccoSubjectFullName = "";
                    if (!string.IsNullOrEmpty(arryOtherSubjectName[i]))
                    {
                        var subjectFilterList = subjectList.Where(p => p.cAccoSubjectFullName == arryOtherSubjectName[i]);
                        if (subjectFilterList == null || subjectFilterList.Count() == 0) continue;
                        var suject = subjectFilterList.FirstOrDefault();
                        AccoSubjectID = suject.AccoSubjectID;
                        cAccoSubjectFullName = suject.cAccoSubjectFullName;
                    }
                   
                    var newItem = new FD_IdentificationTypeAndSubjectODataEntity();
                    var numericalOrder = _numericalOrderCreator.Create();
                    newItem.TypeID = numericalOrder;
                    newItem.TypeName = this.arryOtherTName[i];
                    newItem.BusiType = model.BusiType;
                    //计提方式（0：个别认定 1：账龄计提）
                    if (newItem.TypeName == "按账龄计提的单位")
                    {
                        newItem.AccrualType = 1;
                    }
                    else
                    {
                        newItem.AccrualType = 0;
                    }
                    newItem.AccoSubjectID = AccoSubjectID;
                    newItem.AccoSubjectFullName = cAccoSubjectFullName;
                    newItem.DataSourceType = 0;// 数据来源（0：账龄重分类）
                    newItem.DataSourceTypeName = "账龄重分类表";
                    dataList.Add(newItem);
                }
            }
            result.code = ErrorCode.Success.GetIntValue();
            result.data = dataList;
            return result;
        }
        //计提类型
        private List<string> arryYSName = new List<string>() { "集团内部单位", "联营单位", "除联营企业外的其他关联方", "单项金额不重大但单项计提的单位", "单项金额重大并单独计提的单位", "按账龄计提的单位" };
        private List<string> arryOtherName = new List<string>() { "集团内部单位", "联营单位", "除联营企业外的其他关联方", "个别认定不计提的单位", "个别认定计提的单位", "按账龄计提的单位" };
        //计提类型科目
        private List<string> arryYSTName = new List<string>() { "集团内部单位", "集团内部单位", "联营单位", "除联营企业外的其他关联方", "单项金额不重大但单项计提的单位", "单项金额重大并单独计提的单位", "按账龄计提的单位" };
        private List<string> arryOtherTName = new List<string>() { "集团内部单位", "集团内部单位", "联营单位", "除联营企业外的其他关联方", "个别认定不计提的单位", "个别认定计提的单位", "按账龄计提的单位" };
        //类型科目
        private List<string> arryYSSubjectName = new List<string>() { "应收账款/应收公司内部款", "应收账款/应收集团内部款", "应收账款/应收联营企业款", "应收账款/应收其他关联方款", null, null, "应收账款" };
        private List<string> arryOtherSubjectName = new List<string>() { "其他应收款/其他应收公司内部款", "其他应收款/其他应收集团内部款", "其他应收款/其他应收联营企业款", "其他应收款/其他应收关联方款", "其他应收款/其他应收员工款",null, "其他应收款" };
    }
}
