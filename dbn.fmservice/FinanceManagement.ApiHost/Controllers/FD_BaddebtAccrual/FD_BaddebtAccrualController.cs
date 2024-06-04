using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_BaddebtAccrual;
using FinanceManagement.Common;
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
using Microsoft.AspNet.OData.Query;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Serilog.Core;
using Microsoft.Extensions.Logging;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_BaddebtAccrualController : ControllerBase
    {
        IMediator _mediator;
        FD_BaddebtAccrualODataProvider _provider;
        private readonly ILogger<FD_BaddebtAccrualController> _logger;
        private IIdentityService _identityService;
        AgingDataUtil _agingDataUtil;
        IFD_BaddebtAccrualExtRepository _extRepository;
        FD_BaddebtSettingODataProvider _settingProvider;
        RptAgingReclassificationUtil _agingReclassificationUtil;
        FD_IndividualIdentificationODataProvider _identificationODataProvider;
        FMBaseCommon _baseUtil;
        FD_BaddebtGroupSettingODataProvider _baddebtGroupSettingODataProvider;



        public FD_BaddebtAccrualController(IMediator mediator, FD_BaddebtAccrualODataProvider provider, ILogger<FD_BaddebtAccrualController> logger, IIdentityService identityService,
            AgingDataUtil agingDataUtil,
            IFD_BadDebtProvisionDetailRepository iProvisionDetailRepository,
            FD_BaddebtSettingODataProvider settingProvider,
            RptAgingReclassificationUtil agingReclassificationUtil,
            IFD_BaddebtAccrualExtRepository extRepository,
            FD_BaddebtGroupSettingODataProvider baddebtGroupSettingODataProvider,
            FD_IndividualIdentificationODataProvider identificationODataProvider,
            FMBaseCommon baseUtil
            )
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _identityService = identityService;
            _agingDataUtil = agingDataUtil;
            _settingProvider = settingProvider;
            _agingReclassificationUtil = agingReclassificationUtil;
            _extRepository = extRepository;
            _baseUtil = baseUtil;
            _baddebtGroupSettingODataProvider = baddebtGroupSettingODataProvider;
            _identificationODataProvider = identificationODataProvider;
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
            data?.Lines?.ForEach(o =>
            {
                o.AgingList = _provider.GetExtData(o.NumericalOrderDetail)?.Result;
            });
            result.code = ErrorCode.Success.GetIntValue();
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        [PermissionAuthorize(Permission.Create)]
        public async Task<Result> Add([FromBody] FD_BaddebtAccrualAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_BaddebtAccrualDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Update)]
        public async Task<Result> Modify([FromBody] FD_BaddebtAccrualModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        #region 重分类数据
        /// <summary>
        /// 获取重分类数据
        /// </summary>
        /// <param name="model">BusiType 业务类型（0：应收账款 1：其他应收款）</param>
        /// 获取认定类型科目对应设置；过滤非空科目，循环查询账龄重分类；过滤余额为0的；获取个别认定信息（距离查询日期最近的）；重分类数据中剔除个别认定信息
        /// <returns></returns>
        [HttpPost("GetReclassificationData")]
        public Result GetReclassificationData(AgingReclass model)
        {
            var result = new Result();
            var resultList = new List<FD_BaddebtAccrualDetailODataEntity>();
            try
            {
                if (string.IsNullOrEmpty(model.DataDate))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "DataDate不能为空";
                    return result;
                }

                if (string.IsNullOrEmpty(model.EnterpriseID)) { model.EnterpriseID = _identityService.EnterpriseId; }
                //获取认定类型科目对应设置 过滤非空科目
                var typeSubjectList =  _baddebtGroupSettingODataProvider.GetIdentificationTypeSubjectList(new FD_BaddebtGroupSettingODataEntity() { DataDate = model.DataDate, EnterpriseID = _identityService.GroupId, IsFiltersubject = true },1,model.BusiType,model.EnterpriseID)?.Result;
                if (typeSubjectList == null || typeSubjectList.Count == 0)
                {
                    result.msg = "未获取到认定类型科目对应信息";
                    result.code = ErrorCode.NoContent.GetIntValue();
                    return result;
                }
                //获取集团区间设置
                var groupSetting = _baddebtGroupSettingODataProvider.GetGroupSettingDetailByDate( model.DataDate, model.EnterpriseID,model.BusiType)?.Result;
                //_logger.LogInformation("FD_BaddebtAccrual/GetReclassificationData:"+JsonConvert.SerializeObject(groupSetting)+"\n 参数"+ model.DataDate+";"+ model.EnterpriseID + ";" + model.BusiType);
                if (groupSetting == null || groupSetting.Count == 0)
                {
                    result.msg = "未获取到集团区间设置";
                    result.code = ErrorCode.NoContent.GetIntValue();
                    return result;
                }
                //查询账龄重分类
                var list = GetReclassificationRequest(model, typeSubjectList, "1");
                if (list?.Count > 0)
                {
                    resultList.AddRange(list);
                }
                if (model.BusiType == 1)
                {
                    var list2 = GetReclassificationRequest(model, typeSubjectList, "2");
                    if (list2?.Count > 0)
                    {
                        resultList.AddRange(list2);
                    }
                }
              
                if (resultList == null || resultList.Count == 0)
                {
                    result.code = ErrorCode.Success.GetIntValue();
                    result.data = resultList;
                    return result;
                }
                //获取个别认定
                var IdentificationList = _identificationODataProvider.GetDetaiByConAsync(model)?.Result;
                if (IdentificationList?.Count > 0)
                {
                    foreach (var ident in IdentificationList)
                    {
                        resultList.RemoveAll(p => p.BusiType == ident.BusiType && p.CurrentUnit == ident.CurrentUnit);
                    }
                }                
                if (resultList?.Count > 0)
                {
                    //账龄重分类区间和获取到的集团区间设置是否一致
                    var filterAge = resultList.Where(p=>p.AgingList?.Count!= groupSetting.Count);
                    if (filterAge?.Count()>0)
                    {
                        result.msg = "账龄重分类区间和获取到的集团区间设置不一致;"+groupSetting.FirstOrDefault().NumericalOrder;
                        result.code = ErrorCode.NoContent.GetIntValue();
                        return result;
                    }
                   // _logger.LogInformation("FD_BaddebtAccrual/GetReclassificationData:resultList=" + JsonConvert.SerializeObject(resultList));
                    //根据集团区间设置比例，计算坏账准备金额
                    foreach (var item in resultList)
                    {
                        var newageList = new List<FD_BaddebtAccrualExtODataEntity>();
                        for(var i=0;i<item.AgingList.Count;i++)
                        {
                            var newAge = new FD_BaddebtAccrualExtODataEntity();
                            var age = item.AgingList[i];
                            newAge.BusiType = 1;//坏账准备
                            newAge.Name = age.Name;
                            newAge.Amount = age.Amount * groupSetting[i].ProvisionRatio;
                            newageList.Add(newAge);
                        }
                        item.AccrualAmount = newageList.Sum(p => p.Amount);
                        item.AgingList.AddRange(newageList);
                    }
                }
               
                result.data = resultList;
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(" FD_BaddebtAccrual/GetReclassificationData:参数：" + JsonConvert.SerializeObject(model) + "\n 异常" + ex.ToString());
                result.msg = "获取重分类数据异常";
                result.code = ErrorCode.ServerBusy.GetIntValue();
            }
            return result;
        }

        private List<FD_BaddebtAccrualDetailODataEntity> GetReclassificationRequest(AgingReclass model, List<FD_IdentificationTypeSubjectODataEntity> subjectList, string CustomerType = "1")
        {
            try
            {
                var request = new RptAgingReclassificationRequest();
                var subject = subjectList[0];
                if (subject.Rank == null) return new List<FD_BaddebtAccrualDetailODataEntity>();
                request.CustomerType = CustomerType;
                request.Enddate = model.DataDate;
                //request.AccountingSubjectsID = subject.AccoSubjectCode;
                // request.Boid = _identityService.UserId;
                request.SubjectLevel =int.Parse(subject.Rank.ToString());
                request.FilterSubject = true;
                request.EnteID = model.EnterpriseID;               
                request.GroupID = _identityService.GroupId;
                request.EnterpriseList = $"{ request.EnteID}";
                request.MenuParttern = "0";//单位
                request.IntervalType = model.IntervalType;
                request.OwnEntes = new List<string>() { request.EnteID };
                request.CanWatchEntes = new List<string>() { request.EnteID };
                var list =GetReclassificationList(request, model,subjectList);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(" FD_BaddebtAccrual/GetReclassificationRequest:参数：" + JsonConvert.SerializeObject(model) + "\n 异常" + ex.ToString());
                return new List<FD_BaddebtAccrualDetailODataEntity>();
            }
        }
        private List<FD_BaddebtAccrualDetailODataEntity> GetReclassificationList(RptAgingReclassificationRequest request, AgingReclass model, List<FD_IdentificationTypeSubjectODataEntity> subjectList)
        {
            try
            {
                var response = _agingReclassificationUtil.GetData(request)?.Result;
                var retdata = response?.Where(o => o.AdjustAmount > 0)?.ToList();
                var list = new List<FD_BaddebtAccrualDetailODataEntity>();
                if (retdata==null||retdata.Count == 0) { return list; }
                foreach (var item in subjectList)
                {
                    var data = retdata?.Where(p => p.AccoSubjectCode == item.AccoSubjectCode);
                    var itemList = data?.Select(o => new FD_BaddebtAccrualDetailODataEntity()
                    {
                        BusinessType = request.CustomerType == "1" ? "201611160104402101" : "201611160104402103",//客户:员工
                        BusinessTypeName = request.CustomerType == "1" ? "客户" : "员工",
                        Amount = o.AdjustAmount,//重分类后金额
                        AgingList = o.AgingintervalDatas?.OrderBy(p => p.Serial)?.Select(a => new FD_BaddebtAccrualExtODataEntity { Amount = a.Amount, Name = a.Name, BusiType = 0 })?.ToList(),
                        CurrentUnit = o.SummaryType1,
                        CurrentUnitName = o.SummaryType1Name,
                        BusiType = model.BusiType,
                        TypeID = item.TypeID,
                        TypeName = item.TypeName
                    }).ToList();
                    list.AddRange(itemList);
                }
                return list;
            }
            catch (Exception ex)
            {

                _logger.LogError(" FD_BaddebtAccrual/GetReclassificationList:参数：" + JsonConvert.SerializeObject(model) + "\n 异常" + ex.ToString());
                return new List<FD_BaddebtAccrualDetailODataEntity>();
            }
        }
        #endregion
       
    }
}
