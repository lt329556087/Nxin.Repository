using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_CarryForwardVoucher;
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

namespace FinanceManagement.ApiHost.Controllers.FM_CarryForwardVoucher
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CarryForwardVoucherController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        FM_CarryForwardVoucherODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;

        public FM_CarryForwardVoucherController(IMediator mediator, FM_CarryForwardVoucherODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
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
        public async Task<Result> GetDetail(long key)
        {
            var result = new Result();
            var data = await _provider.GetSingleDataAsync(key);
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FM_CarryForwardVoucherAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //删除
        [HttpDelete]
        //[PermissionAuthorize(Permission.Delete)]
        public async Task<Result> Delete([FromBody] FM_CarryForwardVoucherDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        public async Task<Result> Modify([FromBody] FM_CarryForwardVoucherModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //复制
        [HttpPost]
        [Route("Copy")]
        public async Task<Result> Copy([FromBody] FM_CarryForwardVoucherCopyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("MakeVoucher")]
        public List<ResultModel> MakeVoucher([FromBody] FM_CarryForwardVoucherInterfaceSearchCommand request)
        {
            List<ResultModel> list = new List<ResultModel>();
            if (!string.IsNullOrEmpty(request.NumericalOrderList))
            {
                request.Boid = _identityService.UserId;
                request.EnterpriseID = _identityService.EnterpriseId;
                request.GroupID = _identityService.GroupId;
                var numericalOrders = request.NumericalOrderList.Split(',');
                foreach (var item in numericalOrders)
                {
                    request.NumericalOrder = item;
                    ResultModel resut = MakeVoucherPorc(request);
                    list.Add(resut);
                }
                return list;
            }
            else
            {
                list.Add(new ResultModel()
                {
                    CodeNo = 1,
                    Msg = "请选择需要结转的单据！",
                    ResultState = false
                });
                return list;
            }
        }
        [HttpPost]
        [Route("MakeVoucherPorc")]
        public ResultModel MakeVoucherPorc(FM_CarryForwardVoucherInterfaceSearchCommand request)
        {
            try
            {
                MakeVoucherBase _make = null;
                var data = _provider.GetSingleData(Convert.ToInt64(request.NumericalOrder));
                if (data == null) return new ResultModel() { CodeNo = 1, ResultState = false, Msg = request.NumericalOrder + "单据不存在" };
                List<EnterprisePeriod> periods = new List<EnterprisePeriod>();
                switch (data?.TransferAccountsType)
                {
                    case "1911081429200000101"://销售结转
                        _make = new SaleSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                        //获取会计期间
                         periods = _comUtil.getEnterprisePeriod(new EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.DataDate).Year, Month = Convert.ToDateTime(request.DataDate).Month }).Result;
                        bool resultVer = _make.VerificationCheckout(request, 1610311318270000101, data?.TransferAccountsType);
                        if (!resultVer) return new ResultModel() { CodeNo = 1, Msg = "销售未结账,请先结转后再生成凭证！", ResultState = false };
                        break;
                    case "1911081429200000102"://采购结转
                        _make = new PurchaseSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                        //获取会计期间
                        periods = _comUtil.getEnterprisePeriod(new EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.DataDate).Year, Month = Convert.ToDateTime(request.DataDate).Month }).Result;
                        bool resultVer1 = _make.VerificationCheckout(request, 1611051506540000101, data?.TransferAccountsType);
                        if (!resultVer1) return new ResultModel() { CodeNo = 1, Msg = "采购未结账,请先结转后再生成凭证！", ResultState = false };
                        break;
                    case "1911081429200000103"://物品暂估结转
                        _make = new SuppliesEstimationSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                        //获取会计期间
                        periods = _comUtil.getEnterprisePeriod(new EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.Enddate).Year, Month = Convert.ToDateTime(request.Enddate).Month }).Result;
                        break;
                    case "1911081429200000104"://禽成本结转
                        _make = new CheckenCostSummaryCommon(_httpClientUtil1, _hostCongfiguration);
                        //获取会计期间
                        periods = _comUtil.getEnterprisePeriod(new EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.DataDate).Year, Month = Convert.ToDateTime(request.DataDate).Month }).Result;
                        break;
                    default:
                        break;
                }
                if (_make==null)
                {
                    return new ResultModel() { Code = 1, Msg = "请选择结转类别", ResultState = false };
                }
                FinanceManagement.Common.MakeVoucherCommon.FD_SettleReceipt result = _make?.getVoucherList(data, request);
                result.Lines = result.Lines.Where(p => p.Debit != 0 || p.Credit != 0).ToList();
                result.DataDate = (DateTime)periods.FirstOrDefault()?.EndDate;
                result.CurrentEnterDate = request.CurrentEnterDate;
                result.EnterpriseID = _identityService.EnterpriseId;
                result.OwnerID = _identityService.UserId;
                result.CarryData = data;
                result.IsSettleCheckOut = _make.VerificationCheckout(request, 1611091727140000101, data?.TransferAccountsType);//会计凭证结账
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>($"{_hostCongfiguration.QlwServiceHost}/api/CreateVoucher/InsertSettleReceiptExtend", result).Result;
                return res;
            }
            catch (Exception ex)
            {
                return new ResultModel() { Code = 1, Msg = "系统内部报错，请联系管理员！" };
                throw;
            }
        }
        [HttpPost]
        [Route("Get")]
        public ResultModel<SaleSummaryResultData> Get(FM_CarryForwardVoucherInterfaceSearchCommand request)
        {
            SaleSummaryCommon _make = new SaleSummaryCommon(_httpClientUtil1,_hostCongfiguration);
            var data = _provider.GetSingleData(Convert.ToInt64(request.NumericalOrder));
            FinanceManagement.Common.MakeVoucherCommon.FD_SettleReceipt domain = new FinanceManagement.Common.MakeVoucherCommon.FD_SettleReceipt()
            {
                SettleReceipType = "201610220104402203",
                TicketedPointID = data.TicketedPointID,
                TransBeginDate = Convert.ToDateTime(request.DataDate).ToString(),
                TransEndDate = Convert.ToDateTime(request.DataDate).AddMonths(1).AddDays(-1).ToString(),
            };
            _make = new SaleSummaryCommon(_httpClientUtil1, _hostCongfiguration);
            var summ = _make.GetSummaryList(request);
            _make.reqUrl = _hostCongfiguration.ReportService + "/api/SaleSummary/GetSummaryAsync";
            var resu = new ResultModel<SaleSummaryResultData>();// _make.GetSaleSummaryDataList(data, request,data.Lines[0], summ, domain,false);
            return resu;
        }

    }
}
