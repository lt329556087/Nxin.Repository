using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_SpecificIdentification;
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

namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_SpecificIdentificationController : ControllerBase
    {
        IMediator _mediator;
        FD_SpecificIdentificationODataProvider _provider;
        private readonly ILogger<FD_SpecificIdentificationController> _logger;
        private IIdentityService _identityService;
        AgingDataUtil _agingDataUtil;
        IFD_BadDebtProvisionDetailRepository _iProvisionDetailRepository;
        IFD_SpecificIdentificationExtRepository _extRepository;
        FD_BaddebtSettingODataProvider _settingProvider;
        RptAgingReclassificationUtil _agingReclassificationUtil;
        IFD_BadDebtProvisionRepository _iProvisionRepository;
        FMBaseCommon _baseUtil;


        public FD_SpecificIdentificationController(IMediator mediator, FD_SpecificIdentificationODataProvider provider, ILogger<FD_SpecificIdentificationController> logger, IIdentityService identityService,
            AgingDataUtil agingDataUtil,
            IFD_BadDebtProvisionDetailRepository iProvisionDetailRepository,
            FD_BaddebtSettingODataProvider settingProvider,
            RptAgingReclassificationUtil agingReclassificationUtil,
            IFD_SpecificIdentificationExtRepository extRepository,
            IFD_BadDebtProvisionRepository iProvisionRepository,
            FMBaseCommon baseUtil
            )
        {
            _mediator = mediator;
            _provider = provider;
            _logger = logger;
            _identityService = identityService;
            _agingDataUtil = agingDataUtil;
            _iProvisionDetailRepository = iProvisionDetailRepository;
            _settingProvider = settingProvider;
            _agingReclassificationUtil = agingReclassificationUtil;
            _extRepository = extRepository;
            _iProvisionRepository = iProvisionRepository;
            _baseUtil = baseUtil;
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
            data?.Lines?.ForEach(o =>
            {
                o.IsProvision = _iProvisionDetailRepository.IsSpecificDataExist(o.NumericalOrderDetail.ToString(), _identityService.EnterpriseId);
                o.AgingList = _provider.GetExtData(o.NumericalOrderDetail).Result;
            });

            data.Lines1 = data.Lines.Where(o => data.AccoSubjectID1 == o.AccoSubjectID).ToList();
            data.Lines2 = data.Lines.Where(o => data.AccoSubjectID2 == o.AccoSubjectID).ToList();

            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        [AllowAnonymous]
        public async Task<Result> Add([FromBody] FD_SpecificIdentificationAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        public async Task<Result> Delete([FromBody] FD_SpecificIdentificationDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_SpecificIdentificationModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        /// <summary>
        /// 账龄分析表
        /// </summary>
        /// <param name="dataDate"></param>
        /// <param name="accoSubjectId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpPost("GetAgingData")]
        public Result GetAgingData([FromBody] DealingOccurRequest param)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(param.CustomerID) || param.CustomerID == "0")
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "账龄客户不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(param.Enddate))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "账龄日期不能为空";
                    return result;
                }
                param.Boid = _identityService.UserId;
                param.EnteID = _identityService.EnterpriseId;
                param.GroupID = _identityService.GroupId;
                param.EnterpriseList = $"|{_identityService.EnterpriseId}";
                param.CustomerType = "1";
                param.MenuParttern = "0";//单位
                if (string.IsNullOrEmpty(param.IntervalType) || param.IntervalType == "0")
                {
                    param.IntervalType = "1803300944570000101";//默认按年
                }
                param.OwnEntes = new List<string>() { _identityService.EnterpriseId };
                var data = _agingDataUtil.GetAgingData(param).Result;//账龄报表默认只有一条
                if (data != null)
                {
                    var amount = data.Sum(p => p.Amount);//多个科目合计
                    result.data = amount;
                    result.code = ErrorCode.Success.GetIntValue();
                }
                else
                {
                    result.code = ErrorCode.NoContent.GetIntValue();
                    result.msg = "未获取到账龄分析表数据";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("FD_SpecificIdentification/GetAgingData:异常：{0}，参数{1},{2}", ex.ToString(), param.CustomerID, param.Enddate));
                result.code = ErrorCode.Aggregate.GetIntValue();
                result.msg = "获取账龄数据异常";
            }

            return result;
        }

        [HttpPost("GetAgingReclassification")]
        public Result GetAgingReclassification([FromBody] RptAgingReclassificationRequest param)
        {
            Result result = new Result();
            // var param = new RptAgingReclassificationRequest();
            try
            {
                //if (string.IsNullOrEmpty(request.CustomerID) || request.CustomerID == "0")
                //{
                //    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                //    result.msg = "账龄客户不能为空";
                //    return result;
                //}
                if (string.IsNullOrEmpty(param.Enddate))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "账龄日期不能为空";
                    return result;
                }
                if (string.IsNullOrEmpty(param.CustomerType))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "往来类型不能为空";
                    return result;
                }
                //param.Enddate = request.DataDate;
                param.Boid = _identityService.UserId;
                param.EnteID = _identityService.EnterpriseId;
                param.GroupID = _identityService.GroupId;
                param.EnterpriseList = $"{_identityService.EnterpriseId}";

                param.MenuParttern = "0";//单位
                if (string.IsNullOrEmpty(param.IntervalType) || param.IntervalType == "0")
                {
                    param.IntervalType = "1803300944570000101";//默认按年
                }
                param.OwnEntes = new List<string>() { _identityService.EnterpriseId };
                param.CanWatchEntes = new List<string>() { _identityService.EnterpriseId };
                var data = _agingReclassificationUtil.GetData(param).Result;//账龄报表默认只有一条
                if (data != null)
                {
                    result.data = data;
                    result.code = ErrorCode.Success.GetIntValue();
                }
                else
                {
                    result.code = ErrorCode.NoContent.GetIntValue();
                    result.msg = "未获取到账龄分析表数据";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("FD_SpecificIdentification/GetAgingReclassification:异常：{0}，参数{1},{2}", ex.ToString(), param.CustomerID, param.Enddate));
                result.code = ErrorCode.Aggregate.GetIntValue();
                result.msg = "获取账龄重分类异常";
            }

            return result;
        }

        //增加
        [HttpPost, Route("GetEmptyModel")]
        public Result GetEmptyModel([FromBody] FD_SpecificIdentificationAddCommand request)
        {
            var result = new Result();
            var specificIdModel = new FD_SpecificIdentificationAddCommand();
            specificIdModel.DataDate = Convert.ToDateTime(request.DataDate).ToString("yyyy-MM-dd");
            var enterpriseId = Convert.ToInt64(_identityService.EnterpriseId);
            //var settingModel = _settingProvider.GetDataByEnterId(enterpriseId, request.DataDate);

            var subjectCodeList = _baseUtil.GetEnterSubjectList(0, Convert.ToInt64(_identityService.EnterpriseId), specificIdModel.DataDate).Result;
            //应收账款
            specificIdModel.AccoSubjectID1 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1122")?.AccoSubjectID;
            specificIdModel.AccoSubjectName1 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1122")?.cAccoSubjectFullName;
            //其他应收款
            specificIdModel.AccoSubjectID2 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1221")?.AccoSubjectID;
            specificIdModel.AccoSubjectName2 = subjectCodeList.FirstOrDefault(o => o.cAccoSubjectCode == "1221")?.cAccoSubjectFullName;

            specificIdModel.BusinessType = request.BusinessType ?? "201611160104402101";
            specificIdModel.Lines1 = new List<FD_SpecificIdentificationDetailCommand>() { new FD_SpecificIdentificationDetailCommand() };
            specificIdModel.Lines2 = new List<FD_SpecificIdentificationDetailCommand>() { new FD_SpecificIdentificationDetailCommand() };
            result.data = specificIdModel;
            return result;
        }

        /// <summary>
        /// 判断增加/删除的日期是否有效
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("ValidDate")]
        public Result ValidDate([FromBody] FD_SpecificIdentificationAddCommand request)
        {
            var result = new Result();
            var provision = _iProvisionRepository.GetLastest(_identityService.EnterpriseId).Result;
            if (provision != null)
            {
                if (DateTime.Compare(Convert.ToDateTime(request.DataDate), provision.DataDate) < 0)
                {
                    result.data = false;
                    return result;
                }
            }
            result.data = true;
            return result;
        }

        #region 最新个别认定
        [HttpGet("GetDataByEnterId")]
        public FD_SpecificIdentificationODataEntity GetDataByEnterId(long enterpriseId, string date, string customerId)
        {
            return _provider.GetDataByEnterId(enterpriseId, date, customerId);
        }
        #endregion
    }
}
