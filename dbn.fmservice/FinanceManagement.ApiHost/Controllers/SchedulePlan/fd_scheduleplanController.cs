using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Payment
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class fd_scheduleplanController : ControllerBase
    {
        IMediator _mediator;
        //fd_scheduleplanTODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;
        fd_scheduleplanODataProvider _prodiver;
        FD_PaymentReceivablesTODataProvider _paymentReceivablesTODataProvider;
        fd_schedulesetODataProvider _SchedulesetODataProvider;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<fd_scheduleplanController> _logger;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        FMBaseCommon _baseUnit;
        public fd_scheduleplanController(FD_PaymentReceivablesTODataProvider paymentReceivablesTODataProvider, FMBaseCommon baseUnit, fd_schedulesetODataProvider schedulesetODataProvider,IMediator mediator, HttpClientUtil httpClientUtil,
            //fd_scheduleplanTODataProvider provider, 
            FundSummaryUtil fundSummaryUtil, IFD_PaymentExtendRepository paymentExtendRepository, NumericalOrderCreator numericalOrderCreator, IIdentityService identityService, fd_scheduleplanODataProvider prodiver, ILogger<fd_scheduleplanController> logger, HostConfiguration hostCongfiguration)
        {
            _paymentReceivablesTODataProvider = paymentReceivablesTODataProvider;
            _baseUnit = baseUnit;
            _mediator = mediator;
            //_provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
            _prodiver = prodiver;
            _SchedulesetODataProvider = schedulesetODataProvider;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _paymentExtendRepository = paymentExtendRepository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        //复核，确定 通用接口
        [HttpPost]
        [Route("Add")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] fd_scheduleplanSaveCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //取消接口 把排程中 变成 历史  列表查询就会直接回复 未排程
        [HttpPost]
        [Route("Cancel")]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Cancel([FromBody] fd_scheduleplanCancelCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("TestGetList")]
        public List<fd_scheduleplanReportEntity> TestGetList()
        {
            string a = "[{\"NumericalOrder\":\"2211021010040000180\",\"count\":0,\"GroupId\":\"2584266\",\"ApplyData\":\"2022-11-02T00:00:00\",\"ApplyEnterpriseId\":\"2584265\",\"ApplyEnterpriseName\":\"业财一体企业\",\"ApplyMenuId\":\"1612052003560000101\",\"ApplyMenuName\":\"支出申请\",\"ApplyNumericalOrder\":\"2211021006310000120\",\"ApplyPayContent\":\"支出申请1102.1\",\"ApplyContactType\":\"201611160104402103\",\"ApplyContactTypeName\":\"员工\",\"ApplyContactEnterpriseId\":\"636462653667031365\",\"ApplyContactEnterpriseName\":\"张志格\",\"ApplyEmergency\":\"否\",\"ApplyDeadLine\":\"2022-11-02T00:00:00\",\"ApplyAmount\":1102.10,\"ApplySurplusAmount\":1102.10,\"PayAmount\":1.01,\"DeadLine\":\"2022-11-02T00:00:00\",\"Level\":2,\"SettlementMethod\":\"非网银\",\"ScheduleStatus\":3,\"OwnerId\":2292057,\"OwnerName\":\"张志格\",\"CreatedDate\":\"2022-11-02T10:10:04\",\"ModifiedDate\":\"2022-11-02T10:10:04\",\"ScheduleCount\":0,\"ScheduleCountList\":null,\"IsSchedule\":true,\"ReceiptAbstractCode\":null,\"PaymentDataDate\":null,\"PaymentAmount\":null,\"cAHrefDetails\":\"https://qlw.nxin.com/FDExpense/ApplyDetail?appid=1612052003560000101&numericalorder=2211021006310000120\",\"PayNumericalOrder\":\"2211021011380000180\"},{\"NumericalOrder\":\"2211021347090000180\",\"count\":0,\"GroupId\":\"2584266\",\"ApplyData\":\"2022-11-02T00:00:00\",\"ApplyEnterpriseId\":\"2584265\",\"ApplyEnterpriseName\":\"业财一体企业\",\"ApplyMenuId\":\"1612052003560000101\",\"ApplyMenuName\":\"支出申请\",\"ApplyNumericalOrder\":\"2211021006310000120\",\"ApplyPayContent\":\"支出申请1102.1\",\"ApplyContactType\":\"201611160104402103\",\"ApplyContactTypeName\":\"员工\",\"ApplyContactEnterpriseId\":\"636462653667031365\",\"ApplyContactEnterpriseName\":\"张志格\",\"ApplyEmergency\":\"否\",\"ApplyDeadLine\":\"2022-11-02T00:00:00\",\"ApplyAmount\":1102.10,\"ApplySurplusAmount\":1096.09,\"PayAmount\":4.00,\"DeadLine\":\"2022-11-04T00:00:00\",\"Level\":4,\"SettlementMethod\":\"网银\",\"ScheduleStatus\":2,\"OwnerId\":2292057,\"OwnerName\":\"张志格\",\"CreatedDate\":\"2022-11-02T13:47:10\",\"ModifiedDate\":\"2022-11-02T13:47:10\",\"ScheduleCount\":0,\"ScheduleCountList\":null,\"IsSchedule\":true,\"ReceiptAbstractCode\":null,\"PaymentDataDate\":null,\"PaymentAmount\":null,\"cAHrefDetails\":\"https://qlw.nxin.com/FDExpense/ApplyDetail?appid=1612052003560000101&numericalorder=2211021006310000120\",\"PayNumericalOrder\":\"0\"},{\"NumericalOrder\":\"2211021345530000880\",\"count\":0,\"GroupId\":\"2584266\",\"ApplyData\":\"2022-11-02T00:00:00\",\"ApplyEnterpriseId\":\"2584265\",\"ApplyEnterpriseName\":\"业财一体企业\",\"ApplyMenuId\":\"1612052003560000101\",\"ApplyMenuName\":\"支出申请\",\"ApplyNumericalOrder\":\"2211021006310000120\",\"ApplyPayContent\":\"支出申请1102.1\",\"ApplyContactType\":\"201611160104402103\",\"ApplyContactTypeName\":\"员工\",\"ApplyContactEnterpriseId\":\"636462653667031365\",\"ApplyContactEnterpriseName\":\"张志格\",\"ApplyEmergency\":\"否\",\"ApplyDeadLine\":\"2022-11-02T00:00:00\",\"ApplyAmount\":1102.10,\"ApplySurplusAmount\":1101.09,\"PayAmount\":5.00,\"DeadLine\":\"2022-11-02T00:00:00\",\"Level\":5,\"SettlementMethod\":\"网银\",\"ScheduleStatus\":2,\"OwnerId\":2292057,\"OwnerName\":\"张志格\",\"CreatedDate\":\"2022-11-02T13:45:54\",\"ModifiedDate\":\"2022-11-02T13:45:54\",\"ScheduleCount\":0,\"ScheduleCountList\":null,\"IsSchedule\":true,\"ReceiptAbstractCode\":null,\"PaymentDataDate\":null,\"PaymentAmount\":null,\"cAHrefDetails\":\"https://qlw.nxin.com/FDExpense/ApplyDetail?appid=1612052003560000101&numericalorder=2211021006310000120\",\"PayNumericalOrder\":\"0\"}]";
            return JsonConvert.DeserializeObject<List<fd_scheduleplanReportEntity>>(a);
        }
        [HttpPost]
        [Route("GetApplyCount")]
        public dynamic GetApplyCount([FromBody] QueryData query)
        {
            var entes =  _baseUnit.GetAuthorEnterpise(_identityService.EnterpriseId,_identityService.UserId);
            //根据申请类型计数
            var RList = new List<dynamic>();
            //根据申请类型汇总金额
            var AList = new List<dynamic>();
            //排程数据
            var PList = new List<dynamic>();
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            var list = _httpClientUtil1.PostJsonAsync<object>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwfm.group.expenselist.get/1.0", query).Result;
            try
            {
                var res = JsonConvert.DeserializeObject<Result>(JsonConvert.SerializeObject(list));
                var resdata = JsonConvert.DeserializeObject<ResultExpense>(JsonConvert.SerializeObject(res.data));
                if (resdata.data != null)
                {
                    var data = JsonConvert.DeserializeObject<List<ExpenseData>>(JsonConvert.SerializeObject(resdata.data));

                    foreach (var item in data.GroupBy(m => m.ExpenseType))
                    {
                        //根据申请类型计数
                        RList.Add(new { item.FirstOrDefault().ExpenseType,item.FirstOrDefault().ExpenseTypeName, Count = item.Count(), Ratio = Math.Round(Convert.ToDecimal(item.Count()) / Convert.ToDecimal(data.Count),4) });
                        //根据申请类型汇总金额
                        AList.Add(new { item.FirstOrDefault().ExpenseType,item.FirstOrDefault().ExpenseTypeName, Count = item.Sum(m => m.Amount), Ratio = Math.Round(Convert.ToDecimal(item.Sum(m => m.Amount) / data.Sum(m => m.Amount)),4) });
                    }
                    //启用集中支付-付款排程
                    var result = _baseUnit.OptionConfigValue("20221026135217", _identityService.GroupId);
                    //开启系统选项
                    if (result != "0" && !string.IsNullOrEmpty(result))
                    {
                        //取已排成数据
                        var planList = _prodiver.GetDatas().Where(m => m.ScheduleStatus == 2 || m.ScheduleStatus == 3).ToList();
                        foreach (var item in planList.GroupBy(m => m.Level))
                        {
                            PList.Add(new { item.FirstOrDefault().Level, Count = item.Count(), PayCount = item.Where(m => m.ScheduleStatus == 3).Count() });
                        }
                    }
                    else
                    {
                        var payList = _paymentReceivablesTODataProvider.GetMergeDatas(entes).ToList();
                        if (!string.IsNullOrEmpty(query.BeginDate))
                        {
                            payList = payList.Where(m => Convert.ToDateTime(m.DataDate) >= Convert.ToDateTime(query.BeginDate)).ToList();
                        }
                        if (!string.IsNullOrEmpty(query.EndDate))
                        {
                            payList = payList.Where(m => Convert.ToDateTime(m.DataDate) <= Convert.ToDateTime(query.EndDate)).ToList();
                        }
                        PList.Add(new { Count = payList.Count, ApplyCount = data.Count });
                    }

                    keyValuePairs.Add("RList", RList);
                    keyValuePairs.Add("AList", AList);
                    keyValuePairs.Add("PList", PList);
                    //网银支付情况统计
                    keyValuePairs.Add("PayReuslt", _paymentReceivablesTODataProvider.GetPayResultCount(query.EnterpriseIDs, query.BeginDate, query.EndDate));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(JsonConvert.SerializeObject(e));
                _logger.LogInformation("付款申请json：" + JsonConvert.SerializeObject(list));
                return keyValuePairs;
            }
            return keyValuePairs;
        }
        /// <summary>
        /// 根据模板获取已填报的 计划期间
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTemplateDataDrops")]
        public List<TemplateDataDrop> GetTemplateDataDrops(string TemplateId)
        {
            return _paymentReceivablesTODataProvider.GetTemplateDataDropGroup(TemplateId);
        }
        /// <summary>
        /// 根据模板获取已填报的 计划数量
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="PlanPeriod"></param>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTemplateSetCount")]
        public Dictionary<string, object> GetTemplateSetCount(string TemplateId,string PlanPeriod,string EnterpriseId)
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            //获取应填写单位数量
            var TemplateData = _paymentReceivablesTODataProvider.GetTemplate(TemplateId);
            //过滤单位筛选
            #region 过滤单位筛选
            List<string> tList = new List<string>();
            foreach (var item in EnterpriseId.Split(','))
            {
                tList.AddRange(TemplateData.FirstOrDefault().EnterpriseList.Split(",").Where(m => m == item).ToList());
            }
            TemplateData.Clear();
            TemplateData.Add(new Infrastructure.QlwCrossDbEntities.TemplateData() { EnterpriseList = string.Join(',', tList) });
            #endregion
            if (TemplateData.Count > 0)
            {
                
                var enters = TemplateData.FirstOrDefault().EnterpriseList.Split(",").ToList();

                var temp = TemplateData.FirstOrDefault().EnterpriseList.Split(",").ToList();
                //获取已填写单位数量
                var data = new List<TemplateDataDrop>();
                foreach (var item in EnterpriseId.Split(",").ToList())
                {
                    data.AddRange(_paymentReceivablesTODataProvider.GetTemplateDataDrop(TemplateId).Where(m=>m.EnterpriseId == item && m.PlanPeriod == PlanPeriod).ToList());
                }
                //获取未填写单位
                foreach (var item in data)
                {
                    if (enters.Where(m=>m == item.EnterpriseId).FirstOrDefault() != null)
                    {
                        enters.Remove(enters.Where(m => m == item.EnterpriseId).FirstOrDefault());
                    }
                }
                var list = _httpClientUtil1.PostJsonAsync<object>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.org.manager/1.0", enters).Result;
                var res = JsonConvert.DeserializeObject<Result>(JsonConvert.SerializeObject(list));
                var resdata = JsonConvert.DeserializeObject<List<OrgRoot>>(JsonConvert.SerializeObject(res.data));
                if (resdata != null)
                {
                    foreach (var item in resdata)
                    {
                        if (item.manager != null)
                        {
                            if (item.manager.Count > 0)
                            {
                                item.manager = item.manager.Where(m => m.roleType == "201612220104402202").ToList();
                                if (item.manager.Count > 0)
                                {
                                    item.ChargeName = string.Join(',', item.manager.Select(m => m.name));
                                }
                            }
                        }
                    }
                }
                //应填写数量
                keyValuePairs.Add("SumCount", temp.Count);
                //已填写数量
                keyValuePairs.Add("ReadyCount", data.Count);
                //未填写数量
                keyValuePairs.Add("NotCount", temp.Count - data.Count);
                //已填报单位
                keyValuePairs.Add("ReadyFill", data);
                //未填报单位
                keyValuePairs.Add("NotFill", resdata);
            }
            return keyValuePairs;
        }
        [HttpPost]
        [Route("GetList")]
        public List<fd_scheduleplanEntity> GetList([FromBody] QueryData query)
        {
            //获取排程中，已排程，历史 列表数据
            query.PageIndex = "1";
            query.PageSize = "99999";
            var PlanNumericalOrder = query.NumericalOrder;
            query.NumericalOrder = "";
            var planList = _prodiver.GetDatas().ToList();
            var menu = _prodiver.GetMeunInfo();
            var setList = _SchedulesetODataProvider.GetList().ToList();
            if (planList.Count > 0)
            {
                planList.FirstOrDefault().count = planList.Count;
            }
            var list = _httpClientUtil1.PostJsonAsync<object>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwfm.group.expenselist.get/1.0", query).Result;
            var res = JsonConvert.DeserializeObject<Result>(JsonConvert.SerializeObject(list));
            var resdata = JsonConvert.DeserializeObject<ResultExpense>(JsonConvert.SerializeObject(res.data));
            var finaly = new List<fd_scheduleplanEntity>();
            if (resdata.data != null)
            {
                var data = JsonConvert.DeserializeObject<List<ExpenseData>>(JsonConvert.SerializeObject(resdata.data));
                if (query.Status == 0)
                {
                    if (resdata.data != null)
                    {
                        foreach (var item in data)
                        {
                            //当前申请 没有排程中的，并且已排程的金额 没超过 申请的金额才会显示出来
                            if (planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 1).Count() == 0){
                                if ((planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 2).Sum(m => m.PayAmount) + planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 3).Sum(m => m.PayAmount)) >= item.Amount)
                                {
                                    continue;
                                }
                                //单位资金调拨,账户资金调拨,支出汇总 差旅汇总 薪资支出 差旅报销单  不走排程
                                //SELECT * FROM `qlw_nxin_com`.`stmenu` WHERE menuid IN(1612101828540000101,1612060928300000101,2108251659100000109,201702230104402501,201702230104402502,201512289611681881,201512291502313749,1612081533020000101,1612122002230000101,1612121954010000101,2112101512170000109)
                                if (item.ExpenseType == "1612101828540000101" || item.ExpenseType == "1612060928300000101" || item.ExpenseType == "201702230104402501" || item.ExpenseType == "201702230104402502" || item.ExpenseType == "201512289611681881" || item.ExpenseType == "201512291502313749" || item.ExpenseType == "1612081533020000101" || item.ExpenseType == "1612122002230000101" || item.ExpenseType == "1612121954010000101" || item.ExpenseType == "2112101512170000109" || item.ExpenseType == "2108251659100000109")
                                {
                                    continue;
                                }
                                if (item.Amount - planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 2).Sum(m => m.PayAmount) - planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 3).Sum(m => m.PayAmount) == 0)
                                {
                                    continue;
                                }
                                MenuInfo menuInfo = null;
                                //单位资金调拨
                                if (item.ExpenseType == "201702230104402501")
                                {
                                    menuInfo = menu.Where(m => m.MenuId == "201512289611681881").FirstOrDefault();
                                }
                                //账户资金调拨
                                else if (item.ExpenseType == "201702230104402502")
                                {
                                    menuInfo = menu.Where(m => m.MenuId == "201512291502313749").FirstOrDefault();
                                }
                                else
                                {
                                    menuInfo = menu.Where(m => m.MenuId == item.ExpenseType).FirstOrDefault();
                                }
                                finaly.Add(new fd_scheduleplanEntity()
                                {
                                    ApplyAmount = item.Amount,
                                    ApplyContactEnterpriseId = item.PayerId,
                                    ApplyContactEnterpriseName = item.PayerName,
                                    ApplyContactType = item.BusinessType,
                                    ApplyEmergency = item.Pressing == 0 ? "否" : "是",
                                    ApplyData = item.DataDate,
                                    ApplyDeadLine = item.HouldPayDate,
                                    ApplyEnterpriseId = item.EnterpriseID,
                                    ApplyEnterpriseName = item.EnterpriseName,
                                    ApplyMenuId = item.ExpenseType,
                                    ApplyMenuName = item.ExpenseTypeName,
                                    ApplyNumericalOrder = item.NumericalOrder,
                                    NumericalOrder = item.NumericalOrder,
                                    ApplyPayContent = item.Content,
                                    ApplySurplusAmount = item.Amount - planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 2).Sum(m => m.PayAmount) - planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 3).Sum(m => m.PayAmount),
                                    ScheduleCount = planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder).Count(),
                                    ScheduleCountList = JsonConvert.SerializeObject(planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder).OrderBy(m => m.CreatedDate).ToList()),
                                    count = resdata.count,
                                    ApplyContactTypeName = item.BusinessTypeName,
                                    SettlementMethod = "网银",
                                    Level = item.Pressing == 0 ? setList.Max(m=>m.Level) : setList.Min(m=>m.Level),
                                    DeadLine = item.HouldPayDate,
                                    PayAmount = item.Amount - planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 2).Sum(m => m.PayAmount) - planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus == 3).Sum(m => m.PayAmount),
                                    OwnerName = item.OwnerName,
                                    IsSchedule = false,
                                    cAHrefDetails = $@"https://qlw.nxin.com" + menuInfo.DetailUrl + $@"?appid={menuInfo.MenuId}&numericalorder={item.NumericalOrder}"
                                }) ;
                            }
                        }
                    }
                    foreach (var item in finaly)
                    {
                        item.count = finaly.Count;
                    }
                    if (!string.IsNullOrEmpty(PlanNumericalOrder))
                    {
                        finaly = finaly.Where(m => m.ApplyNumericalOrder.Contains(PlanNumericalOrder)).ToList();
                    }
                    if (finaly.Count > 0)
                    {
                        finaly.FirstOrDefault().count = finaly.Count;
                        finaly = finaly.Skip((Convert.ToInt32(query.PlanPageIndex) - 1) * Convert.ToInt32(query.PlanPageSize)).ToList();
                        finaly = finaly.Take(Convert.ToInt32(query.PlanPageSize)).ToList();
                    }
                    return finaly.OrderBy(m => m.Level).ThenBy(m => m.CreatedDate).ToList();
                }
                else
                {
                    finaly.AddRange(planList.Where(m => m.ScheduleStatus == query.Status).ToList());
                    if (query.Status == 2)
                    {
                        foreach (var item in data)
                        {
                            //过滤出来 未排程 的申请 进行支付操作
                            if (planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder && m.ScheduleStatus != -1).Count() > 0)
                            {
                                continue;
                            }
                            //支出汇总 差旅汇总 薪资支出 差旅报销单  ，直接显示出来（兼容汇总单 不走排程的申请）
                            if (item.ExpenseType == "1612101828540000101" || item.ExpenseType == "1612060928300000101" || item.ExpenseType == "201702230104402501" || item.ExpenseType == "201702230104402502" || item.ExpenseType == "201512289611681881" || item.ExpenseType == "201512291502313749" || item.ExpenseType == "1612081533020000101" || item.ExpenseType == "1612122002230000101" || item.ExpenseType == "1612121954010000101" || item.ExpenseType == "2112101512170000109" || item.ExpenseType == "2108251659100000109")
                            {
                                MenuInfo menuInfo = null;
                                //单位资金调拨
                                if (item.ExpenseType == "201702230104402501")
                                {
                                    menuInfo = menu.Where(m => m.MenuId == "201512289611681881").FirstOrDefault();
                                }
                                //账户资金调拨
                                else if (item.ExpenseType == "201702230104402502")
                                {
                                    menuInfo = menu.Where(m => m.MenuId == "201512291502313749").FirstOrDefault();
                                }
                                else
                                {
                                    menuInfo = menu.Where(m => m.MenuId == item.ExpenseType).FirstOrDefault();
                                }
                                finaly.Add(new fd_scheduleplanEntity()
                                {
                                    ApplyAmount = item.Amount,
                                    ApplyContactEnterpriseId = item.PayerId,
                                    ApplyContactEnterpriseName = item.PayerName,
                                    ApplyContactType = item.BusinessType,
                                    ApplyEmergency = item.Pressing == 0 ? "否" : "是",
                                    ApplyData = item.DataDate,
                                    ApplyDeadLine = item.HouldPayDate,
                                    ApplyEnterpriseId = item.EnterpriseID,
                                    ApplyEnterpriseName = item.EnterpriseName,
                                    ApplyMenuId = item.ExpenseType,
                                    ApplyMenuName = item.ExpenseTypeName,
                                    ApplyNumericalOrder = item.NumericalOrder,
                                    NumericalOrder = item.NumericalOrder,
                                    ApplyPayContent = item.Content,
                                    ApplySurplusAmount = item.Amount,
                                    ScheduleCount = 0,
                                    count = resdata.count,
                                    ApplyContactTypeName = item.BusinessTypeName,
                                    SettlementMethod = "网银",
                                    Level = setList.Min(m => m.Level),
                                    DeadLine = item.HouldPayDate,
                                    PayAmount = item.Amount,
                                    OwnerName = item.OwnerName,
                                    ReceiptAbstractCode = item.ReceiptAbstractCode,
                                    IsSchedule = false,
                                    OwnerId = Convert.ToInt64(item.OwnerID),
                                    cAHrefDetails = $@"https://qlw.nxin.com" + menuInfo.DetailUrl + $@"?appid={menuInfo.MenuId}&numericalorder={item.NumericalOrder}"
                                });
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(query.BeginDate))
                    {
                        finaly = finaly.Where(m => m.ApplyData >= Convert.ToDateTime(query.BeginDate)).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.EndDate))
                    {
                        finaly = finaly.Where(m => m.ApplyData <= Convert.ToDateTime(query.EndDate)).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.EnterpriseIDs))
                    {
                        var temp = new List<fd_scheduleplanEntity>();
                        foreach (var item in query.EnterpriseIDs.Split(','))
                        {
                            temp.AddRange(finaly.Where(m => m.ApplyEnterpriseId == item).ToList());
                        }
                        finaly = temp;
                    }
                    if (!string.IsNullOrEmpty(query.ExpenseTypes))
                    {
                        var temp = new List<fd_scheduleplanEntity>();
                        foreach (var item in query.ExpenseTypes.Split(','))
                        {
                            temp.AddRange(finaly.Where(m => m.ApplyMenuId == item).ToList());
                        }
                        finaly = temp;
                    }
                    if (!string.IsNullOrEmpty(query.BusinessType))
                    {
                        finaly = finaly.Where(m => m.ApplyContactType == query.BusinessType).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.PayerName))
                    {
                        finaly = finaly.Where(m => m.ApplyContactEnterpriseName.Contains(query.PayerName)).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.NumericalOrder))
                    {
                        finaly = finaly.Where(m => m.ApplyNumericalOrder.Contains(query.NumericalOrder)).ToList();
                    }
                    //true=全量 用于 申请列表，false=排程列表查询
                    if (!query.IsSchedule)
                    {
                        finaly = finaly.Where(m => m.IsSchedule == true).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.Pressing))
                    {
                        finaly = finaly.Where(m => m.ApplyEmergency == (query.Pressing == "1" ? "是" : "否")).ToList();
                    }
                    if (query.MaxAmount > 0 && query.MinAmount > 0)
                    {
                        finaly = finaly.Where(m => m.PayAmount >= query.MinAmount && m.PayAmount <= query.MaxAmount).ToList();
                        
                    }
                    foreach (var item in finaly)
                    {
                        item.count = finaly.Count;
                    }
                    if (finaly.Count > 0)
                    {
                        finaly.FirstOrDefault().count = finaly.Count;
                        finaly = finaly.Skip((Convert.ToInt32(query.PlanPageIndex) - 1) * Convert.ToInt32(query.PlanPageSize)).ToList();
                        finaly = finaly.Take(Convert.ToInt32(query.PlanPageSize)).ToList();
                    }
                    foreach (var item in finaly)
                    {
                        if (!item.IsSchedule)
                        {
                            continue;
                        }
                        item.ScheduleCount = planList.Where(m => m.ApplyNumericalOrder == item.ApplyNumericalOrder).Count();
                        item.ScheduleCountList = JsonConvert.SerializeObject(planList.Where(m => m.ApplyNumericalOrder == item.ApplyNumericalOrder).OrderBy(m => m.CreatedDate).ToList());
                    }
                    return finaly.OrderBy(m => m.Level).ThenBy(m=>m.CreatedDate).ToList();
                }
            }
            //名称赋值
            if (finaly != null)
            {
                if (finaly.Count > 0)
                {
                    var relationList = _prodiver.GetApplyRelation(string.Join(',', finaly.Select(m => string.IsNullOrEmpty(m.ApplyNumericalOrder) ? "0": m.ApplyNumericalOrder)));
                    foreach (var item in finaly)
                    {
                        var temp = relationList.Where(m => m.NumericalOrder == item.NumericalOrder).ToList();
                        item.ApplyContactEnterpriseName = string.Join(',', temp.Select(m => m.Name));
                        item.ApplyContactEnterpriseId = string.Join(',', temp.Select(m => m.Id));
                    }
                }
            }
            return finaly.OrderBy(m => m.Level).ThenBy(m => m.CreatedDate).ToList();
        }
        /// <summary>
        /// 报表专用查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetListByReport")]
        public List<fd_scheduleplanReportEntity> GetListByReport([FromBody] QueryData query)
        {
            try
            {

                //获取排程中，已排程，历史 列表数据
                query.PageIndex = "1";
                query.PageSize = "99999";
                query.Bo_ID = _identityService.UserId;
                //过滤掉 已关闭，已取消记录  只取 排程中+已排程+已支付
                var planList = _prodiver.GetReportDatas().Where(m=>m.ScheduleStatus != 4 && m.ScheduleStatus != -1).ToList();
                var setList = _SchedulesetODataProvider.GetList().ToList();
                var menu = _prodiver.GetMeunInfo();
                var list = _httpClientUtil1.PostJsonAsync<object>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwfm.group.expenselist.get/1.0", query).Result;
                var res = JsonConvert.DeserializeObject<Result>(JsonConvert.SerializeObject(list));
                var resdata = JsonConvert.DeserializeObject<ResultExpense>(JsonConvert.SerializeObject(res.data));
                var finaly = new List<fd_scheduleplanReportEntity>();
                if (resdata.data != null)
                {
                    var data = JsonConvert.DeserializeObject<List<ExpenseData>>(JsonConvert.SerializeObject(resdata.data));
                    foreach (var item in data)
                    {
                        MenuInfo menuInfo = null;
                        //单位资金调拨
                        if (item.ExpenseType == "201702230104402501")
                        {
                            menuInfo = menu.Where(m => m.MenuId == "201512289611681881").FirstOrDefault();
                        }
                        //账户资金调拨
                        else if (item.ExpenseType == "201702230104402502")
                        {
                            menuInfo = menu.Where(m => m.MenuId == "201512291502313749").FirstOrDefault();
                        }
                        else
                        {
                            menuInfo = menu.Where(m => m.MenuId == item.ExpenseType).FirstOrDefault();
                        }
                        var finalyData = new fd_scheduleplanReportEntity()
                        {
                            ApplyAmount = item.Amount,
                            ApplyContactEnterpriseId = item.PayerId,
                            ApplyContactEnterpriseName = item.PayerName,
                            ApplyContactType = item.BusinessType,
                            ApplyEmergency = item.Pressing == 0 ? "否" : "是",
                            ApplyData = item.DataDate,
                            ApplyDeadLine = item.HouldPayDate,
                            ApplyEnterpriseId = item.EnterpriseID,
                            ApplyEnterpriseName = item.EnterpriseName,
                            ApplyMenuId = item.ExpenseType,
                            ApplyMenuName = item.ExpenseTypeName,
                            ApplyNumericalOrder = item.NumericalOrder,
                            ApplyPayContent = item.Content,
                            ApplySurplusAmount = item.Amount,
                            ScheduleCount = 0,
                            count = resdata.count,
                            ApplyContactTypeName = item.BusinessTypeName,
                            SettlementMethod = "网银",
                            Level = item.Pressing == 0 ? setList.Max(m => m.Level) : setList.Min(m => m.Level),
                            DeadLine = item.HouldPayDate,
                            PayAmount = item.Amount,
                            OwnerName = item.OwnerName,
                            ReceiptAbstractCode = item.ReceiptAbstractCode,
                            IsSchedule = false,
                            OwnerId = Convert.ToInt64(item.OwnerID),
                            cAHrefDetails = $@"https://qlw.nxin.com" + menuInfo.DetailUrl + $@"?appid={menuInfo.MenuId}&numericalorder={item.NumericalOrder}"
                        };
                        //支出汇总 差旅汇总 薪资支出 差旅报销单  ，直接显示出来（兼容汇总单 不走排程的申请）
                        if (item.ExpenseType == "201702230104402501" || item.ExpenseType == "201702230104402502" || item.ExpenseType == "201512289611681881" || item.ExpenseType == "201512291502313749" || item.ExpenseType == "1612081533020000101" || item.ExpenseType == "1612122002230000101" || item.ExpenseType == "1612121954010000101" || item.ExpenseType == "2112101512170000109")
                        {
                            finalyData.Level = setList.Min(m => m.Level);
                            finaly.Add(finalyData);
                        }
                        //检测是否排程过，如果排程过 则提取排程信息 直接使用排程信息，未排程 就直接用申请的信息
                        else
                        {
                            var tempList = planList.Where(m => m.ApplyNumericalOrder == item.NumericalOrder).ToList();
                            if (tempList.Count > 0)
                            {
                                finaly.AddRange(tempList);
                                //移除排程过的信息，后续要一次性添加整组数据 防止重复
                                foreach (var x in tempList)
                                {
                                    planList.Remove(x);
                                }
                            }
                            //未排程的数据
                            else
                            {
                                finaly.Add(finalyData);
                            }
                        }
                    }
                    //将剩余的数据追加（剩余的数据都是已付全款的数据）
                    finaly.AddRange(planList);
                    finaly = finaly.Distinct().ToList();
                    //申请日期
                    if (!string.IsNullOrEmpty(query.BeginDate))
                    {
                        finaly = finaly.Where(m => m.ApplyData >= Convert.ToDateTime(query.BeginDate)).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.EndDate))
                    {
                        finaly = finaly.Where(m => m.ApplyData <= Convert.ToDateTime(query.EndDate)).ToList();
                    }
                    //申请单 付款到期日
                    if (query.ApplyDeadBeginDate != null)
                    {
                        finaly = finaly.Where(m => m.ApplyDeadLine >= Convert.ToDateTime(query.ApplyDeadBeginDate)).ToList();
                    }
                    if (query.ApplyDeadEndDate != null)
                    {
                        finaly = finaly.Where(m => m.ApplyDeadLine <= Convert.ToDateTime(query.ApplyDeadEndDate)).ToList();
                    }
                    //付款日期
                    if (query.PaymentBeginDate != null)
                    {
                        finaly = finaly.Where(m => m.PaymentDataDate >= Convert.ToDateTime(query.PaymentBeginDate)).ToList();
                    }
                    if (query.PaymentEndDate != null)
                    {
                        finaly = finaly.Where(m => m.PaymentDataDate <= Convert.ToDateTime(query.PaymentEndDate)).ToList();
                    }
                    //计划付款日期(原形写的是 排程日期）
                    if (query.PlanDeadLineBeginDate != null)
                    {
                        finaly = finaly.Where(m => m.DeadLine >= Convert.ToDateTime(query.PlanDeadLineBeginDate)).ToList();
                    }
                    if (query.PlanDeadLineEndDate != null)
                    {
                        finaly = finaly.Where(m => m.DeadLine <= Convert.ToDateTime(query.PlanDeadLineEndDate)).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.EnterpriseIDs))
                    {
                        var temp = new List<fd_scheduleplanReportEntity>();
                        foreach (var item in query.EnterpriseIDs.Split(','))
                        {
                            temp.AddRange(finaly.Where(m => m.ApplyEnterpriseId == item).ToList());
                        }
                        finaly = temp;
                    }
                    if (!string.IsNullOrEmpty(query.ExpenseTypes))
                    {
                        var temp = new List<fd_scheduleplanReportEntity>();
                        foreach (var item in query.ExpenseTypes.Split(','))
                        {
                            temp.AddRange(finaly.Where(m => m.ApplyMenuId == item).ToList());
                        }
                        finaly = temp;
                    }
                    if (!string.IsNullOrEmpty(query.BusinessType))
                    {
                        var temp = new List<fd_scheduleplanReportEntity>();
                        foreach (var item in query.BusinessType.Split(','))
                        {
                            temp.AddRange(finaly.Where(m => m.ApplyContactType == item).ToList());
                        }
                        finaly = temp;
                    }
                    if (!string.IsNullOrEmpty(query.PayerName))
                    {
                        finaly = finaly.Where(m => m.ApplyContactEnterpriseName.Contains(query.PayerName)).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.NumericalOrder))
                    {
                        finaly = finaly.Where(m => m.ApplyNumericalOrder.Contains(query.NumericalOrder)).ToList();
                    }
                    //true=全量 用于 申请列表，false=排程列表查询
                    if (!query.IsSchedule)
                    {
                        finaly = finaly.Where(m => m.IsSchedule == true).ToList();
                    }
                    if (!string.IsNullOrEmpty(query.Pressing))
                    {
                        finaly = finaly.Where(m => m.ApplyEmergency == (query.Pressing == "1" ? "是" : "否")).ToList();
                    }
                    if (query.MaxAmount > 0 && query.MinAmount > 0)
                    {
                        finaly = finaly.Where(m => m.PayAmount >= query.MinAmount && m.PayAmount <= query.MaxAmount).ToList();

                    }
                    if (!string.IsNullOrEmpty(query.Level))
                    {
                        var temp = new List<fd_scheduleplanReportEntity>();
                        foreach (var item in query.Level.Split(','))
                        {
                            temp.AddRange(finaly.Where(m => m.Level == Convert.ToInt32(item)).ToList());
                        }
                        finaly = temp;
                    }
                    //if (finaly.Count > 0)
                    //{
                    //    finaly.FirstOrDefault().count = finaly.Count;
                    //    finaly = finaly.Skip((Convert.ToInt32(query.PlanPageIndex) - 1) * Convert.ToInt32(query.PlanPageSize)).ToList();
                    //    finaly = finaly.Take(Convert.ToInt32(query.PageSize)).ToList();
                    //}
                    return finaly.OrderBy(m => m.Level).ThenBy(m => m.CreatedDate).ToList();
                }
                //名称赋值
                if (finaly != null)
                {
                    if (finaly.Count > 0)
                    {
                        var relationList = _prodiver.GetApplyRelation(string.Join(',', finaly.Select(m => string.IsNullOrEmpty(m.ApplyNumericalOrder) ? "0" : m.ApplyNumericalOrder)));
                        foreach (var item in finaly)
                        {
                            var temp = relationList.Where(m => m.NumericalOrder == item.NumericalOrder).ToList();
                            item.ApplyContactEnterpriseName = string.Join(',', temp.Select(m => m.Name));
                            item.ApplyContactEnterpriseId = string.Join(',', temp.Select(m => m.Id));
                        }
                    }
                }
                return finaly.OrderBy(m => m.Level).ThenBy(m => m.CreatedDate).ToList();
            }
            catch (Exception e)
            {

                var data = new List<fd_scheduleplanReportEntity>();
                data.Add(new fd_scheduleplanReportEntity() { cAHrefDetails = e.ToString()+"，参数：+"+JsonConvert.SerializeObject(query) });
                return data;
            }
        }
        [NonAction]
        public string postMessage(string strUrl, string strPost)
        {
            try
            {
                CookieContainer objCookieContainer = null;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "Post";
                request.Accept = "*/*";
                request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                request.Headers.Add("Accept-Charset: GBK,utf-8;q=0.7,*;q=0.3");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;

                request.Referer = strUrl;//.Remove(strUrl.LastIndexOf("/"));
                Console.WriteLine(strUrl);
                if (objCookieContainer == null)
                    objCookieContainer = new CookieContainer();

                request.CookieContainer = objCookieContainer;
                if (!string.IsNullOrEmpty(strPost))
                {
                    byte[] byteData = Encoding.UTF8.GetBytes(strPost.ToString().TrimEnd('&'));
                    request.ContentLength = byteData.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteData, 0, byteData.Length);
                        reqStream.Close();
                    }
                }

                string strResponse = "";
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                {
                    objCookieContainer = request.CookieContainer;
                    //QueryRecordForm.LoginCookie = objCookieContainer.GetCookies(new Uri(strUrl));
                    res.Cookies = objCookieContainer.GetCookies(new Uri(strUrl));
                    //foreach (Cookie c in res.Cookies)
                    //{

                    //}

                    using (Stream resStream = res.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read;
                        while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            strResponse += Encoding.UTF8.GetString(buffer, 0, read);
                        }
                    }
                    res.Close();
                }
                return strResponse;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
