using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_VoucherAmortization;
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

namespace FinanceManagement.ApiHost.Controllers.FD_VoucherAmortization
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FD_VoucherAmortizationController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        FD_VoucherAmortizationODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMBaseCommon _fmBaseCommon;

        public FD_VoucherAmortizationController(FMBaseCommon fmBaseCommon, IMediator mediator, FD_VoucherAmortizationODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _fmBaseCommon = fmBaseCommon;
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
                var data = _provider.GetSingleData(key);
                data.LinesExtend = data.Lines.Where(s => s.IsDebit == false).ToList();
                data.Lines = data.Lines.Where(s => s.IsDebit == true).ToList();
                result.data = data;
                return result;
            }
            catch (Exception ex )
            {
                return new Result() { code = -1, msg = "查询失败,请联系管理员！" };
            }
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_VoucherAmortizationAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FD_VoucherAmortizationDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FD_VoucherAmortizationModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPost]
        [Route("ModifyList")]
        public async Task<Result> ModifyList([FromBody] FD_VoucherAmortizationListModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Route("MakeVoucher")]
        public List<ResultModel> MakeVoucher([FromBody] MakeVoucherModel request)
        {
            try
            {
                List<ResultModel> list = new List<ResultModel>();
                if (!string.IsNullOrEmpty(request.NumericalOrderList))
                {
                    request.Boid = _identityService.UserId;
                    request.Enddate = request.DataDate;
                    request.EnterpriseID = _identityService.EnterpriseId;
                    request.GroupID = _identityService.GroupId;
                    var numericalOrders = request.NumericalOrderList.Split(',');
                    //会计期间
                    List<EnterprisePeriod> periods = _comUtil.getEnterprisePeriod(new EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.DataDate).Year, Month = Convert.ToDateTime(request.DataDate).Month }).Result;
                    request.PeriodEndDate = (DateTime) periods.FirstOrDefault()?.EndDate;
                    request.IsSettleCheckOut = new SaleSummaryCommon(_httpClientUtil1, _hostCongfiguration).VerificationCheckout(request, 1611091727140000101, "1611091727140000101");//会计凭证结账
                    if (request.IsMultiple)//多个方案生成一张凭证
                    {
                        List<FD_VoucherAmortizationODataEntity> voucherAmortizationList = new List<FD_VoucherAmortizationODataEntity>();
                        List<FM_VoucherAmortizationRecord> records = new List<FM_VoucherAmortizationRecord>();
                        foreach (var item in numericalOrders)
                        {
                            var voucherAmortization = _provider.GetSingleData(Convert.ToInt64(item));
                            if (voucherAmortization == null) { list.Add(new ResultModel() { CodeNo = 1, ResultState = false, Msg = request.NumericalOrder + "单据不存在" });continue; } ;
                            var currentData = voucherAmortization?.PeriodLines.Where(s => Convert.ToDateTime(s.AccountDate).Year == Convert.ToDateTime(request.DataDate).Year && Convert.ToDateTime(s.AccountDate).Month == Convert.ToDateTime(request.DataDate).Month).FirstOrDefault();
                            int currentIndex = voucherAmortization.PeriodLines.FindIndex(s => Convert.ToDateTime(s.AccountDate).Year == Convert.ToDateTime(request.DataDate).Year && Convert.ToDateTime(s.AccountDate).Month == Convert.ToDateTime(request.DataDate).Month);
                            FM_VoucherAmortizationRecord record = _comUtil.verificationAmortization(voucherAmortization, request, currentData, currentIndex);
                            if (!string.IsNullOrEmpty(record.ImplementResult))
                            {
                                list.Add(new ResultModel() { CodeNo = 1, ResultState = false, Msg = record.ImplementResult });
                            }
                            records.Add(record);//每个验证结果集中保存
                            voucherAmortizationList.Add(voucherAmortization);//每个业务结果集中保存
                        }
                        if (list.Count == 0)
                        {
                            List<FM_VoucherAmortizationRelatedODataEntity> relateds = new List<FM_VoucherAmortizationRelatedODataEntity>();
                            foreach (var item in voucherAmortizationList)
                            {
                                relateds.AddRange(_provider.GetRelatedDatas(Convert.ToInt64(item.NumericalOrder)));//获取摊销的中间关系
                            }
                            var res = _comUtil.getSingleMakeSettleResult(voucherAmortizationList, request, relateds, records).Result;//获取摊销结果
                            list.Add(res);
                        }
                        else
                        {
                            return list;
                        }
                    }
                    else
                    {
                        foreach (var item in numericalOrders)
                        {
                            var voucherAmortization = _provider.GetSingleData(Convert.ToInt64(item));
                            if (voucherAmortization == null) { list.Add(new ResultModel() { CodeNo = 1, ResultState = false, Msg = request.NumericalOrder + "单据不存在" });continue; };
                            var currentData = voucherAmortization?.PeriodLines.Where(s => Convert.ToDateTime(s.AccountDate).Year == Convert.ToDateTime(request.DataDate).Year && Convert.ToDateTime(s.AccountDate).Month == Convert.ToDateTime(request.DataDate).Month).FirstOrDefault();
                            int currentIndex = voucherAmortization.PeriodLines.FindIndex(s => Convert.ToDateTime(s.AccountDate).Year == Convert.ToDateTime(request.DataDate).Year && Convert.ToDateTime(s.AccountDate).Month == Convert.ToDateTime(request.DataDate).Month);
                            List<FM_VoucherAmortizationRelatedODataEntity> relateds = new List<FM_VoucherAmortizationRelatedODataEntity>();
                            if (currentIndex > 0)
                            {
                                relateds = _provider.GetRelatedDatas(Convert.ToInt64(voucherAmortization.NumericalOrder));//获取摊销的中间关系
                            }
                            var res = _comUtil.getMultipleMakeSettleResult(voucherAmortization, request, currentData, currentIndex, relateds).Result;//获取摊销结果
                            list.Add(res);
                        }
                    }
                    return list;
                }
                else
                {
                    list.Add(new ResultModel()
                    {
                        CodeNo = 1,
                        Msg = "请选择需要摊销的单据！",
                        ResultState = false
                    });
                    return list;
                }
            }
            catch (Exception ex)
            {
                return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = "系统错误!" } };
                throw;
            }

        }

        [HttpPost]
        [Route("GetViycgerAnirtuzationList")]
        public dynamic GetViycgerAnirtuzationList(VoucherSearch model)
        {
            string entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            if (string.IsNullOrEmpty(model.EnterpriseIds))
            {
                model.EnterpriseIds = entes;
            }
            return _provider.GetViycgerAnirtuzationList(model);
        }
    }
}