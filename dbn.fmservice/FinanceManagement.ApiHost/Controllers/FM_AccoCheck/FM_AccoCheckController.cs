using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.Common.NewMakeVoucherCommon;
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
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_AccoCheck
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_AccoCheckController : ControllerBase
    {
        VoucherAmortizationUtil _comUtil;
        IMediator _mediator;
        FM_AccoCheckODataProvider _provider;
        IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FMAPIService FMAPIService;
        FMBaseCommon _baseUnit;
        AccocheckFormulaProperty _accocheckFormulaProperty;
        public FM_AccoCheckController(IMediator mediator, FMAPIService FMAPIService, AccocheckFormulaProperty accocheckFormulaProperty, FMBaseCommon baseUnit, FM_AccoCheckODataProvider provider, VoucherAmortizationUtil comUtil, IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _mediator = mediator;
            _provider = provider;
            _comUtil = comUtil;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            this.FMAPIService = FMAPIService;
            this._accocheckFormulaProperty = accocheckFormulaProperty;
        }
        /// <summary>
        /// 获取本年结账数据
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("GetDetail/{year}")]
        public Result GetDetail(int year)
        {
            List<TreeModelODataEntity> persons = _baseUnit.GetPerson(new Domain.DropSelectSearch() { EnterpriseID = _identityService.EnterpriseId });
            var result = new Result();
            var data = _provider.GetList(year, persons);
            result.data = data;
            return result;
        }

        /// <summary>
        /// 获取会计区间
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("GetPeriod/{year}")]
        public Result GetPeriod(int year)
        {
            var result = new Result();
            var data = _provider.GetEnterprisePeriod(year);
            result.data = data;
            return result;
        }
        /// <summary>
        /// 获取月末结账规则
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRule")]
        public Result GetRule()
        {
            var result = new Result();
            var data = _provider.GetAaccocheckRule().Result;
            result.data = data;
            return result;
        }

        /// <summary>
        /// 获取月末结账公式
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFormulaProperty")]
        public Result GetFormulaProperty()
        {
            var result = new Result();
            var data = _accocheckFormulaProperty.GetFormulaProperty();
            result.data = data;
            return result;
        }

        //增加
        [HttpPost]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FM_AccoCheckAddCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //增加
        [HttpPut]
        //[PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FM_AccoCheckModifyCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("DataCheck")]
        public List<ResultModel> DataCheck(FMSAccoCheckResultModel request)
        {
            try
            {
                List<ResultModel> results = new List<ResultModel>();
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = "无身份权限", ResultState = false, } };
                }
                SetParam(request);
                request._token = authentication;
                var accocheckType = (AccoCheckTypeEnum)Convert.ToInt64(request.AccoCheckType);
                var _make = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType);
                results = _make.DataCheck(request);
                return results;
            }
            catch (Exception ex)
            {
                return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = ex.ToString(), ResultState = false, } };
            }
        }
        [HttpPost]
        [Route("Checkout")]
        public async Task<CheckResult> Checkout(FM_AccoCheckAddCommand model)
        {
            try
            {
                CheckResult checkResult = new CheckResult();
                List<ResultModel> results = new List<ResultModel>();
                FMSAccoCheckResultModel request = new FMSAccoCheckResultModel();
                request.Date = Convert.ToDateTime(model.DataDate);
                request.EnterpriseID = Convert.ToInt64(_identityService.EnterpriseId);
                model.EnterpriseID = _identityService.EnterpriseId;
                request.GroupID = Convert.ToInt64(_identityService.GroupId);
                request.OwnerID = Convert.ToInt64(_identityService.UserId);
                model.OwnerID = _identityService.UserId;
                int day = DateTime.DaysInMonth(request.Date.Year, request.Date.Month);
                request.DataDate = request.Date.ToString("yyyy-MM") + "-" + day;
                model.DataDate = Convert.ToDateTime(request.DataDate);
                //获取会计期间
                var period = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = _identityService.EnterpriseId, Year = request.Date.Year, Month = request.Date.Month }).Result?.FirstOrDefault();
                request.BeginDate = period?.StartDate.ToString("yyyy-MM-dd");
                request.EndDate = period?.EndDate.ToString("yyyy-MM-dd");
                request.PigFarmIds = model.PigFarmIds;
                model.StartDate = (DateTime)period?.StartDate;
                model.EndDate = (DateTime)period?.EndDate;
                var checkoutDict = CheckOutKey();
                //已结账的集合
                List<Dictionary<string, List<int>>> alreadyList = new List<Dictionary<string, List<int>>>();
                foreach (var item in model.Lines)
                {
                    List<int> checkMenuList = new List<int>();
                    var accocheckType = (AccoCheckTypeEnum)Convert.ToInt64(item.AccoCheckType);
                    var _make = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType);
                    var menuids = checkoutDict.Where(s => s.Key == item.AccoCheckType).Select(s => s.Value).FirstOrDefault();
                    //没有结账步骤，只核对数据的话在这里打已结账标识。--比如成本结账
                    if (menuids.Count == 0)
                    {
                        item.CheckMark = true;
                    }
                    foreach (var menuid in menuids)
                    {
                        var resultList = _make.Checkout(request, menuid);
                        var noCheckout = resultList.Where(s => s.ResultState == false).ToList();
                        if (noCheckout.Count > 0)
                        {
                            //回滚当前类别已结账数据
                            foreach (var curmenuid in checkMenuList)
                            {
                                _make.CancelCheckout(request, curmenuid);
                            }
                            //回滚已经结账的数据
                            foreach (var type in alreadyList)
                            {
                                var accocheckType1 = (AccoCheckTypeEnum)Convert.ToInt64(type.Keys.FirstOrDefault());
                                var _make1 = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType1);
                                foreach (var armenuid in type[type.Keys.FirstOrDefault()])
                                {
                                    _make1.CancelCheckout(request, armenuid);
                                }
                            }
                            checkResult.ResultList = noCheckout;
                            return checkResult;
                        }
                        else
                        {
                            checkMenuList.Insert(0, menuid);
                            item.CheckMark = true;
                            resultList.ForEach(s =>
                            {
                                item.Extends.Add(new FM_AccoCheckExtendCommand()
                                {
                                    AccoCheckType = item.AccoCheckType,
                                    MenuID = menuid.ToString(),
                                    CheckMark = true
                                });
                            });
                            results.AddRange(resultList);
                        }
                    }
                    Dictionary<string, List<int>> alreadydict = new Dictionary<string, List<int>>();
                    alreadydict.Add(item.AccoCheckType, checkMenuList);
                    alreadyList.Insert(0, alreadydict);
                }
                checkResult.ResultList = results;
                if (results.Where(s => s.ResultState == false).Count() == 0)
                {
                    checkResult.Result = await _mediator.Send(model, HttpContext.RequestAborted);
                    return checkResult;
                }
                return checkResult;
            }
            catch (Exception ex)
            {
                return new CheckResult() { ResultList = new List<ResultModel>() { new ResultModel() { Code = 1, ResultState = false, Msg = "结账异常", Data = ex.ToString() } } };
            }
        }
        [HttpPost]
        [Route("CancelCheck")]
        public async Task<List<ResultModel>> CancelCheck(FM_AccoCheckDeleteCommand model)
        {
            try
            {
                model.IsSinge = false;
                List<ResultModel> results = new List<ResultModel>();
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = "无身份权限", ResultState = false, } };
                }
                FMSAccoCheckResultModel request = new FMSAccoCheckResultModel();
                request.Date = Convert.ToDateTime(model.DataDate);
                request.EnterpriseID = Convert.ToInt64(_identityService.EnterpriseId);
                request.GroupID = Convert.ToInt64(_identityService.GroupId);
                request.OwnerID = Convert.ToInt64(_identityService.UserId);
                int day = DateTime.DaysInMonth(request.Date.Year, request.Date.Month);
                request.DataDate = request.Date.ToString("yyyy-MM") + "-" + day;
                //获取会计期间
                var period = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = _identityService.EnterpriseId, Year = request.Date.Year, Month = request.Date.Month }).Result?.FirstOrDefault();
                request.BeginDate = period?.StartDate.ToString("yyyy-MM-dd");
                request.EndDate = period?.EndDate.ToString("yyyy-MM-dd");
                List<Dictionary<string, List<int>>> alreadyList = new List<Dictionary<string, List<int>>>();
                Dictionary<string, List<int>> canlceKeys = CanlceKey();
                foreach (var item in canlceKeys)
                {
                    List<int> menuList = new List<int>();
                    var accocheckType = (AccoCheckTypeEnum)Convert.ToInt64(item.Key);
                    var _make = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType);
                    foreach (var menuid in item.Value)
                    {
                        var result = _make.CancelCheckout(request, menuid);
                        if (result.Where(s => s.ResultState == false).Count() > 0)
                        {
                            //回滚当前类别已反结账数据
                            foreach (var curmenuid in menuList)
                            {
                                _make.Checkout(request, curmenuid);
                            }
                            //回滚已经反结账的数据
                            foreach (var type in alreadyList)
                            {
                                var accocheckType1 = (AccoCheckTypeEnum)Convert.ToInt64(type.Keys.FirstOrDefault());
                                var _make1 = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType1);
                                foreach (var armenuid in type[type.Keys.FirstOrDefault()])
                                {
                                    _make1.Checkout(request, armenuid);
                                }
                            }
                            return result;
                        }
                        //记录已经反结账的节点
                        menuList.Insert(0, menuid);
                        results.AddRange(result);
                    }
                    Dictionary<string, List<int>> alreadydict = new Dictionary<string, List<int>>();
                    //记录已经反结账的类别
                    alreadydict.Add(item.Key, menuList);
                    alreadyList.Insert(0, alreadydict);
                }
                if (results.Where(s => s.ResultState == false).Count() == 0)
                {
                    await _mediator.Send(model, HttpContext.RequestAborted);
                }
                return results;
            }
            catch (Exception ex)
            {
                return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = ex.ToString(), ResultState = false, } };
            }
        }

        [HttpPost]
        [Route("SingeCancelCheck")]
        public async Task<List<ResultModel>> SingeCancelCheck(FM_AccoCheckDeleteCommand model)
        {
            try
            {
                model.IsSinge = true;
                List<ResultModel> results = new List<ResultModel>();
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = "无身份权限", ResultState = false, } };
                }
                FMSAccoCheckResultModel request = new FMSAccoCheckResultModel();
                request.Date = Convert.ToDateTime(model.DataDate);
                request.EnterpriseID = Convert.ToInt64(_identityService.EnterpriseId);
                request.GroupID = Convert.ToInt64(_identityService.GroupId);
                request.OwnerID = Convert.ToInt64(_identityService.UserId);
                int day = DateTime.DaysInMonth(request.Date.Year, request.Date.Month);
                request.DataDate = request.Date.ToString("yyyy-MM") + "-" + day;
                //获取会计期间
                var period = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = _identityService.EnterpriseId, Year = request.Date.Year, Month = request.Date.Month }).Result?.FirstOrDefault();
                request.BeginDate = period?.StartDate.ToString("yyyy-MM-dd");
                request.EndDate = period?.EndDate.ToString("yyyy-MM-dd");
                List<Dictionary<string, List<int>>> alreadyList = new List<Dictionary<string, List<int>>>();
                Dictionary<string, List<int>> canlceKeys = CanlceKey();
                foreach (var line in model.Lines)
                {
                    var accocheckType = (AccoCheckTypeEnum)Convert.ToInt64(line.AccoCheckType);
                    var _make = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType);
                    var menuids = canlceKeys.Where(s => s.Key == line.AccoCheckType).Select(s => s.Value).FirstOrDefault();
                    foreach (var menuid in menuids)
                    {
                        List<int> menuList = new List<int>();
                        var result = _make.CancelCheckout(request, menuid);
                        if (result.Where(s => s.ResultState == false).Count() > 0)
                        {
                            //回滚当前类别已反结账数据
                            foreach (var curmenuid in menuList)
                            {
                                _make.Checkout(request, curmenuid);
                            }
                            //回滚已经反结账的数据
                            foreach (var type in alreadyList)
                            {
                                var accocheckType1 = (AccoCheckTypeEnum)Convert.ToInt64(type.Keys.FirstOrDefault());
                                var _make1 = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType1);
                                foreach (var armenuid in type[type.Keys.FirstOrDefault()])
                                {
                                    _make1.Checkout(request, armenuid);
                                }
                            }
                            return result;
                        }
                        //记录已经反结账的节点
                        menuList.Insert(0, menuid);
                        results.AddRange(result);
                        Dictionary<string, List<int>> alreadydict = new Dictionary<string, List<int>>();
                        //记录已经反结账的类别
                        alreadydict.Add(line.AccoCheckType, menuList);
                        alreadyList.Insert(0, alreadydict);
                    }
                }
                if (results.Where(s => s.ResultState == false).Count() == 0)
                {
                    await _mediator.Send(model, HttpContext.RequestAborted);
                }
                return results;
            }
            catch (Exception ex)
            {
                return new List<ResultModel>() { new ResultModel() { Code = 1, Msg = ex.ToString(), ResultState = false, } };
            }
        }

        private void SetParam(FMSAccoCheckResultModel request)
        {
            request.Date = Convert.ToDateTime(request.DataDate);
            request.EnterpriseID = Convert.ToInt64(_identityService.EnterpriseId);
            request.GroupID = Convert.ToInt64(_identityService.GroupId);
            request.OwnerID = Convert.ToInt64(_identityService.UserId);
            int day = DateTime.DaysInMonth(request.Date.Year, request.Date.Month);
            request.DataDate = request.Date.ToString("yyyy-MM") + "-" + day;
            //获取会计期间
            var period = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = _identityService.EnterpriseId, Year = request.Date.Year, Month = request.Date.Month }).Result?.FirstOrDefault();
            request.BeginDate = period?.StartDate.ToString("yyyy-MM-dd");
            request.EndDate = period?.EndDate.ToString("yyyy-MM-dd");
        }
        private Dictionary<string, List<int>> CheckOutKey()
        {
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            dict.Add(AccoCheckTypeEnum.销售结账.GetStringValue(), new List<int>() { 98, 99 });
            dict.Add(AccoCheckTypeEnum.采购结账.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.存货结账.GetStringValue(), new List<int>() { 98, 99 });
            dict.Add(AccoCheckTypeEnum.固定资产.GetStringValue(), new List<int>() { });
            dict.Add(AccoCheckTypeEnum.物品结账.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.放养管理.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.猪场结账.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.成本管理.GetStringValue(), new List<int>() { });
            dict.Add(AccoCheckTypeEnum.会计结账.GetStringValue(), new List<int>() { 96, 97, 98, 99 });


            return dict;
        }
        private Dictionary<string, List<int>> CanlceKey()
        {
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            dict.Add(AccoCheckTypeEnum.会计结账.GetStringValue(), new List<int>() { 99, 98, 97, 96 });
            dict.Add(AccoCheckTypeEnum.成本管理.GetStringValue(), new List<int>() { });
            dict.Add(AccoCheckTypeEnum.猪场结账.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.放养管理.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.物品结账.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.固定资产.GetStringValue(), new List<int>() { });
            dict.Add(AccoCheckTypeEnum.存货结账.GetStringValue(), new List<int>() { 99, 98 });
            dict.Add(AccoCheckTypeEnum.采购结账.GetStringValue(), new List<int>() { 99 });
            dict.Add(AccoCheckTypeEnum.销售结账.GetStringValue(), new List<int>() { 99, 98 });
            return dict;
        }

        #region 一致性检测详情
        [HttpPost]
        [Route("DataDetailCheck")]
        public ResultModel DataDetailCheck(FMSAccoCheckResultModel request)
        {
            var resultModel = new ResultModel() { Code = 1, ResultState = false };
            try
            {
                AuthenticationHeaderValue authentication = null;
                bool verification = AuthenticationHeaderValue.TryParse(HttpContext.Request.GetAuthToken(), out authentication);
                if (!verification)
                {
                    resultModel.Msg = "无身份权限";
                    return resultModel;
                }
                SetParam(request);
                var accocheckType = (AccoCheckTypeEnum)Convert.ToInt64(request.AccoCheckType);
                var _make = new MonthEndCheckoutFactory(FMAPIService).CreateMonthEndCheckout(accocheckType);
                resultModel = _make.DataDetailCheck(request);
            }
            catch (Exception ex)
            {
                resultModel.Msg = "查询异常" + ex.Message;
            }
            return resultModel;

        }
        #endregion
    }
}
