using System.IO;
using System.Net;
using System.Text;
using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_CashSweep;
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
using Architecture.Common.HttpClientUtil;
using FinanceManagement.Infrastructure;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.OpenApi.Any;

namespace FinanceManagement.ApiHost.Controllers.Api
{
    /****************资金归集金融接口************************/   
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CashSweepFinanceController : ControllerBase
    {
        #region 变量
        FM_CashSweepODataProvider _provider;
        private readonly ILogger<FM_CashSweepController> _logger;
        private IIdentityService _identityService;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil1;
        NumericalOrderCreator _numericalOrderCreator;
        IFM_CashSweepRepository _cashSweeprepository;
        IFM_CashSweepDetailRepository _cashSweepdetailRepository;
        IFD_PayextendRepository _payextendRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private FinanceTradeUtil _financeTradeUtil;
        private FM_CashSweepSettingODataProvider _cashSweepSettingProvider;
        IFM_CashSweepLogRepository _cashSweepLogRepository;
        FMBaseCommon _fmBaseCommon;
        private FD_AccountODataProvider _accountProvider;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherdetailRepository;
        FD_SettleReceiptNewODataProvider _settleReceiptprovider;
        EnterprisePeriodUtil _enterprisePeriodUtil;
        private string AccountTransferType = "201702230104402502";//账户资金调拨
        private string EnteAccountTransferType = "201702230104402501";//单位资金调拨
        private string CashSweepAppID = "2210111522080000109";//资金归集菜单
        private string AccountTransferAppID = "2108021014530000109";//资金调拨菜单
        private string PayType = "201611180104402202";//付款单类型
        private string ReceType = "201611180104402201";//收款单类型
        private string PaySettleReceipType = "201610220104402202";//付款凭证类型
        private string ReceSettleReceipType = "201610220104402201";//收款凭证类型
        private string GroupBusinessType = "201611160104402105";//往来类型-集团单位
        private string OtherBusinessType = "201611160104402106";//往来类型-其他
        private string FormRelatedType = "201610210104402122";//表单关联关系
        private string PayNewAppID = "1612011058280000101";//集中支付付款单 
        private string PayAppID = "1612011058280000101";//单位级付款单菜单
        private string ReceAppID = "1611231950150000101";//单位级收款单菜单
        private string SettleAppID = "1611091727140000101";//凭证菜单
        private string AutoSweepType = "1811191754180000202";//自动归集类型
        #endregion

        #region 构造方法
        public FM_CashSweepFinanceController(FM_CashSweepODataProvider provider, ILogger<FM_CashSweepController> logger, IIdentityService identityService,
            HostConfiguration hostCongfiguration,
            HttpClientUtil httpClientUtil1,
            NumericalOrderCreator numericalOrderCreator,
            IServiceScopeFactory serviceScopeFactory,
            IFM_CashSweepRepository cashSweeprepository,
            IFM_CashSweepDetailRepository cashSweepdetailRepository,
            IFD_PayextendRepository payextendRepository,
            FinanceTradeUtil financeTradeUtil,
            FM_CashSweepSettingODataProvider cashSweepSettingProvider,
            FD_AccountODataProvider accountProvider,
            FMBaseCommon fmBaseCommon,
            ApplySubjectUtil applySubjectUtil,
            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
            IFM_CashSweepLogRepository cashSweepLogRepository,
            Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,
            Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherdetailRepository,
            FD_SettleReceiptNewODataProvider settleReceiptprovider,
            EnterprisePeriodUtil enterprisePeriodUtil
            )
        {
            _provider = provider;
            _logger = logger;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil1;
            _numericalOrderCreator = numericalOrderCreator;
            _serviceScopeFactory = serviceScopeFactory;
            _cashSweeprepository = cashSweeprepository;
            _cashSweepdetailRepository = cashSweepdetailRepository;
            _payextendRepository = payextendRepository;
            _financeTradeUtil = financeTradeUtil;
            _cashSweepSettingProvider = cashSweepSettingProvider;
            _accountProvider = accountProvider;
            _fmBaseCommon = fmBaseCommon;
            _cashSweepLogRepository = cashSweepLogRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherdetailRepository = paymentreceivablesvoucherdetailRepository;
            _settleReceiptprovider = settleReceiptprovider;
            _enterprisePeriodUtil = enterprisePeriodUtil;
        }
        #endregion

        #region 资金归集
        /// <summary>
        /// 执行归集
        /// 手动归集做申请时有金额；自动归集展示的是公式，执行归集时再获取余额和归集金额
        /// 手动归集一次申请完成后除查看，不能做其他操作；自动归集可以多次归集
        /// 归集成功后，若未生成单据，则用户手动补单据
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Result 0:失败 1：成功</returns>
        [Authorize]
        [HttpPost]
        [Route("ExecuteSweep")]
        [PermissionAuthorize(Permission.Report)]
        public YqtPayResonseModel ExecuteSweep([FromBody] ExecuteSweepRequestModel model)
        {
            var retModel = new YqtPayResonseModel() { Code = 0 };
            var accountList = new List<PageResponse>();
            retModel.Data = accountList;
            try
            {
                var key = model.NumericalOrder;
                //查询归集单详情
                var detailList = _provider.GetDetaiDatas(key);
                if (detailList == null || detailList.Count == 0)
                {
                    retModel.Msg = "未获取到单据详情信息" + key;
                    return retModel;
                }
                //根据归集状态做不同处理 0：失败;1：成功;2：处理中;3:初始化
                //成功
                var successList = detailList.Where(p => p.Status == 1);
                if (successList?.Count() > 0)
                {
                    foreach (var item in successList)
                    {
                        accountList.Add(new PageResponse() { Code = 1, Msg = $"[{item.AccountName}]成功" });
                        //查询是否已生成单据，若未生成则生成单据
                    }
                }
                //处理中
                var handlingList = detailList.Where(p => p.Status == 2);
                if (handlingList?.Count() > 0)
                {
                    foreach (var item in handlingList)
                    {
                        accountList.Add(new PageResponse() { Code = 2, Msg = $"[{item.AccountName}]处理中" });
                    }
                }
                //失败或未归集
                var reqList = detailList.Where(p => p.Status != 1 && p.Status != 2);
                if (reqList?.Count() > 0)
                {
                    //获取表头信息
                    var head = _provider.GetData(key);
                    if (head == null || string.IsNullOrEmpty(head.NumericalOrder))
                    {
                        retModel.Msg = "未获取到单据信息" + key;
                        return retModel;
                    }
                    if (!head.IsNew)
                    {
                        retModel.Msg = "请到原菜单归集";
                        return retModel;
                    }

                    //var bankList = reqList.Where(p => p.BankID != head.BankID);
                    //if (bankList?.Count() > 0)
                    //{
                    //    retModel.Msg = "不支持跨行归集";
                    //    return retModel;
                    //}
                    head.Lines = reqList.ToList();
                    //执行归集请求
                    var reqResult = ExecuteSweepOperate(head);
                    if (reqResult != null)
                    {
                        retModel.Msg += reqResult.Msg;
                        retModel.SecurityPCUrl = reqResult.SecurityPCUrl;
                        retModel.RiskList = reqResult.RiskList;
                        retModel.BatchNo = reqResult.BatchNo;
                        if (reqResult.Data?.Count > 0)
                        {
                            retModel.Data.AddRange(reqResult.Data);
                        }
                    }
                }
                if (retModel.Data?.Count > 0||retModel.RiskList?.Count>0)
                {
                    retModel.Code = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/ExecuteSweep:异常{ex.ToString()}\n param={JsonConvert.SerializeObject(model)}");
                retModel.Msg = $"执行归集异常";
                retModel.Code = 0;
            }
            return retModel;
        }
        //手动归集 表头银行编码转换+交易
        private YqtPayResonseModel ExecuteSweepOperate(FM_CashSweepODataEntity model)
        {
            var retModel = new YqtPayResonseModel() { Code = 0 };
            var failAccountList = new List<PageResponse>();
            retModel.Data = failAccountList;
            try
            {                
                //转换银行编码
                var bankcodeResult = ConvertBankType(model.BankID);
                if (bankcodeResult == null)
                {
                    retModel.Msg = "未获取到表头银行编码";
                    return retModel;
                }
                else if (!bankcodeResult.Item1)
                {
                    retModel.Code = 0;
                    retModel.Msg = $"表头{bankcodeResult.Item2}";
                    return retModel;
                }                

                model.BankCode = bankcodeResult.Item2;
                //账号解密
                retModel = GetAccountNumberDecrypt(model, retModel, failAccountList);
                if (!string.IsNullOrEmpty(retModel.Msg))
                {
                    return retModel;
                }
                return YqtTrade(model,retModel,failAccountList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/ExecuteSweepOperate:异常{ex.ToString()}\n param={JsonConvert.SerializeObject(model)}");
                return GetResponseModel(retModel, "归集异常", 0, failAccountList);
            }
        }
       
        /// <summary>
        /// 交易请求
        /// </summary>
        /// <param name="cashSweepmodel"></param>
        /// <param name="retModel"></param>
        /// <param name="failAccountList"></param>
        /// <returns></returns>
        private YqtPayResonseModel YqtTrade(FM_CashSweepODataEntity cashSweepmodel, YqtPayResonseModel retModel, List<PageResponse> failAccountList)
        {           
            try
            {
                List<FinanceAccount> list = new List<FinanceAccount>();
                var realDetailList = cashSweepmodel.Lines;
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);

                //查询用归集还是转账接口 true-归集 false-转账
                //var sweepOrTranResult = GetCashSweepOrTransform(cashSweepmodel.BankNumber, cashSweepmodel.BankCode);
                //if (!string.IsNullOrEmpty(sweepOrTranResult.msg))
                //{
                //    return GetResponseModel(retModel, sweepOrTranResult.msg,0, failAccountList);
                //}
                //var isSweep = sweepOrTranResult.data;
                //var isSweep = false;//2022-10-19 确认规则：不需要判断用哪个接口，统一都用转账接口
                Tuple<string, string> tuple = null;
                var batchNo = _numericalOrderCreator.Create();
                var res = "";
                tuple = YqtTransformPay(cashSweepmodel, realDetailList, list, batchNo).Result;
                //if (isSweep)//归集
                //{
                //    tuple = YqtCollection(cashSweepmodel, realDetailList, list, batchNo).Result;                   
                //}
                //else//转账
                //{
                //    tuple = YqtTransformPay(cashSweepmodel, realDetailList, list, batchNo).Result;
                //}
                if (tuple == null)
                {
                    return GetResponseModel(retModel, "接口返回空", 0, failAccountList);
                }
                else if (!string.IsNullOrEmpty(tuple.Item1))
                {
                    return GetResponseModel(retModel, tuple.Item1, 0, failAccountList);
                }
                res = tuple.Item2;
                if (string.IsNullOrEmpty(res))
                {
                    return GetResponseModel(retModel, "归集失败，接口返回空或归集方向不支持", 0, failAccountList);
                }
                var responModel = JsonConvert.DeserializeObject<RestfulResult>(res);
                if (responModel.code != 0 || responModel.data == null)
                {
                    return GetResponseModel(retModel, string.IsNullOrEmpty(responModel.msg) ? "归集失败，返回数据空" : responModel.msg, 0, failAccountList);
                }
                var realData = JsonConvert.DeserializeObject<YqtPayResonseData>(responModel.data.ToString());
                if (realData == null)
                {
                    return GetResponseModel(retModel, "归集失败，返回数据空"+ responModel.msg, 0, failAccountList);
                }               
                
                retModel.SecurityPCUrl = realData.securityPCUrl;
                retModel.BatchNo = batchNo;
                Dictionary<string, string> failInfo = null;
                if (realData.failInfo != null)
                {
                    failInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(realData.failInfo.ToString());
                }
                Dictionary<string, string> riskInfo = null;
                if (realData.riskInfo != null)
                {
                    riskInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(realData.riskInfo.ToString());
                }
                //修改fm_cashsweep 执行人和日期
                var cashSweepDomain = _cashSweeprepository.Get(cashSweepmodel.NumericalOrder);
                var detailDomainList = new List<FM_CashSweepDetail>();
                var excuteTime = DateTime.Now;
                if (cashSweepDomain == null) { retModel.Msg = $"修改未获取到归集信息{cashSweepmodel.NumericalOrder},请联系管理员";
                    _logger.LogError("FM_CashSweepFinance/YqtTrade归集：修改未获取到归集信息-" + JsonConvert.SerializeObject(cashSweepmodel));
                }
                else
                {
                    long? userID =null;
                    if (cashSweepDomain.SweepType == AutoSweepType)//自动归集的归集人为启用人,在启用时赋值
                    {
                        userID = cashSweepDomain.ExcuterID;
                    }
                    else
                    {
                        userID = string.IsNullOrEmpty(_identityService.UserId) ? 0 : long.Parse(_identityService.UserId);
                    }                    
                    cashSweepDomain.UpdateDate(excuteTime, userID);
                }
                var addPayextendList = new List<FD_Payextend>();
                var detailList = new List<FM_CashSweepDetail>();
                var riskList = new List<RiskModel>();
                retModel.RiskList = riskList;
                var isAutoSweepType = (cashSweepmodel.SweepType== AutoSweepType);//自动归集 没有预警和安全认证，返回的结果无需4状态
                if (realData.failNum > 0||(realData.successNum==0&&realData.failNum==0)||realData.resCode==0)
                {
                    var allFail = false;
                    if (realData.failNum == list.Count)
                    {
                        allFail = true;
                    }
                    for (var i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        var model = realDetailList[i];
                        var failMsg = "";
                        var status = 0;//失败
                        var statusName = "失败";
                        var resultPay = encryptAccount.AccountNumberEncrypt(item.accNo);
                        var payCard = resultPay?.Item1 == true ? resultPay.Item2 : item.accNo;
                        var resultRece = encryptAccount.AccountNumberEncrypt(item.toAccNo);
                        var receCard = resultRece?.Item1 == true ? resultRece.Item2 : item.toAccNo;
                        var filterList = failInfo?.Where(p => p.Key == model.OrderNo);
                        failMsg = filterList?.FirstOrDefault().Value;
                       
                        if (allFail|| realData.resCode == 0)
                        {
                            failMsg += realData.resMsg;
                        }
                        else if (filterList==null|| filterList.Count()==0)
                        {
                            if (isAutoSweepType)
                            {
                                statusName = failMsg = "处理中";
                                status = 2;
                            }
                            else
                            {
                                statusName = failMsg = "待认证";
                                status = 4;
                            }                            
                        }
                        var amount = decimal.Parse(item.amount);
                        var riskFilterList = riskInfo?.Where(p => p.Key == model.OrderNo);
                        if (riskFilterList?.Count() > 0)
                        {
                            var riskMsg = riskFilterList?.FirstOrDefault().Value;
                            failMsg += string.IsNullOrEmpty(riskMsg) ? "" : riskMsg;
                            riskList.Add(new RiskModel() {DateDate= cashSweepmodel.DataDate,EnterpriseName= cashSweepmodel.EnterpriseName, AccountNumber=model.AccountNumber,SweepTypeName= cashSweepmodel.SweepTypeName,SweepDirectionName= cashSweepmodel.SweepDirectionName,Amount= amount, Msg = riskMsg });
                        }
                        else
                        {
                            failAccountList.Add(new PageResponse() { Code = status, Msg = $"[{model.AccountName}]-{statusName}-{failMsg};" });
                        }
                        addPayextendList.Add(new FD_Payextend() { NumericalOrder = model.NumericalOrder, NumericalOrderDetail = model.RecordID.ToString(), OrderNo = item.orderNo, PayeeID = item.payee, PayerID = item.drawee, ReceCardNo = receCard, PayCardNo = payCard, PayCode = "YQT", PayTypeName = "银企通", Remarks = failMsg, PayStatus = status, AuditLevel = 0, Purpose = item.note, BankRoute = item.localFlag, Amount = amount, PayTime = excuteTime, BatchNo= batchNo });
                        var detailDomain = _cashSweepdetailRepository.GetDetailByRecordID(model.RecordID);
                        detailDomain?.UpdateStatus(status, failMsg);
                        detailDomainList.Add(detailDomain);                       
                    }
                }
                else 
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        var model = realDetailList[i];
                        var failMsg = "待认证";
                        var status = 4;//处理中
                        if (isAutoSweepType)
                        {
                            failMsg = "处理中";
                            status = 2;
                        }
                        var resultPay = encryptAccount.AccountNumberEncrypt(item.accNo);
                        var payCard = resultPay?.Item1 == true ? resultPay.Item2 : item.accNo;
                        var resultRece = encryptAccount.AccountNumberEncrypt(item.toAccNo);
                        var receCard = resultRece?.Item1 == true ? resultRece.Item2 : item.toAccNo;
                        var amount = decimal.Parse(item.amount);
                        var riskFilterList = riskInfo?.Where(p => p.Key == model.OrderNo);
                        if (riskFilterList?.Count() > 0)
                        {
                            var riskMsg = riskFilterList?.FirstOrDefault().Value;
                            failMsg += string.IsNullOrEmpty(riskMsg) ? "" : riskMsg;
                            riskList.Add(new RiskModel() { DateDate = cashSweepmodel.DataDate, EnterpriseName = cashSweepmodel.EnterpriseName, AccountNumber = model.AccountNumber, SweepTypeName = cashSweepmodel.SweepTypeName, SweepDirectionName = cashSweepmodel.SweepDirectionName, Amount = amount, Msg = riskMsg });
                        }
                        else
                        {
                            failAccountList.Add(new PageResponse() { Code = status, Msg = $"[{model.AccountName}]-{failMsg};" });
                        }
                        addPayextendList.Add(new FD_Payextend() { NumericalOrder = model.NumericalOrder, NumericalOrderDetail = model.RecordID.ToString(), OrderNo = item.orderNo, PayeeID = item.payee, PayerID = item.drawee, ReceCardNo = receCard, PayCardNo = payCard, PayCode = "YQT", PayTypeName = "银企通", Remarks = failMsg, PayStatus = status, AuditLevel = 0, Purpose = item.note, BankRoute = item.localFlag, Amount = amount, PayTime = excuteTime, BatchNo = batchNo });
                        var detailDomain = _cashSweepdetailRepository.GetDetailByRecordID(model.RecordID);
                        detailDomain.UpdateStatus(status, failMsg);
                        detailDomainList.Add(detailDomain);                        
                    }
                }
                
                //因context被释放，需要重新注入
                Task.Factory.StartNew(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var newAccountService = scope.ServiceProvider.GetRequiredService<IFM_CashSweepRepository>();
                        var newAccountService2 = scope.ServiceProvider.GetRequiredService<IFM_CashSweepDetailRepository>();
                        var newAccountService3 = scope.ServiceProvider.GetRequiredService<IFD_PayextendRepository>();

                        newAccountService.Update(cashSweepDomain);
                        foreach (var item in detailDomainList)
                        {
                            newAccountService2.Update(item);
                            //自动归集，自动归集每天执行一次 同一张申请会归集多次，流水号和RecordID相同，不能删除
                            if (cashSweepDomain.SweepType != "1811191754180000202")
                            {
                                //删除fd_payextend
                                await newAccountService3.RemoveRangeAsync(o => o.NumericalOrderDetail == item.RecordID.ToString() && o.PayStatus != 1 && o.PayStatus != 2);
                            }                            
                        }

                        newAccountService3.AddRange(addPayextendList);

                        await newAccountService.UnitOfWork.SaveChangesAsync();
                        await newAccountService2.UnitOfWork.SaveChangesAsync();
                        await newAccountService3.UnitOfWork.SaveChangesAsync();
                    }
                });
                _logger.LogInformation("FM_CashSweepFinance/YqtTrade归集结果;执行数量" + list?.Count);
                retModel.Code = 1;
                return retModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/YqtTrade归集:异常{ex.ToString()}\n cashSweepmodel={JsonConvert.SerializeObject(cashSweepmodel)};");
                retModel.Msg = "";
                return GetResponseModel(retModel, "归集异常", 0, failAccountList);
            }            
        }
       
        /// <summary>
        /// 账号解密
        /// </summary>
        /// <param name="cashSweepmodel"></param>
        /// <param name="retModel"></param>
        /// <param name="failAccountList"></param>
        /// <returns></returns>
        private YqtPayResonseModel GetAccountNumberDecrypt(FM_CashSweepODataEntity cashSweepmodel, YqtPayResonseModel retModel, List<PageResponse> failAccountList)
        {
            try
            {
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);

                //解密银行卡号
                var resultNumMain = encryptAccount.AccountNumberDecrypt(cashSweepmodel.AccountNumber);
                cashSweepmodel.AccountNumber = resultNumMain?.Item1 == true ? resultNumMain.Item2 : cashSweepmodel.AccountNumber;
                //if (string.IsNullOrEmpty(cashSweepmodel.AccountNumber))
                //{
                //    return GetResponseModel(retModel, "表头账号空" , 0, failAccountList);
                //}
                var realDetailList = new List<FM_CashSweepDetailODataEntity>();
                foreach (var item in cashSweepmodel.Lines)
                {
                    //解密银行卡号
                    var resultNum = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                    item.AccountNumber = resultNum?.Item1 == true ? resultNum.Item2 : item.AccountNumber;
                    //转换银行编码
                    var convertResult = ConvertBankType(item.BankID);
                    if (convertResult == null)
                    {
                        failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]未获取到银行编码" });
                        continue;
                    }
                    else if (!convertResult.Item1)
                    {
                        failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-{convertResult.Item1}" });
                        continue;
                    }
                    item.BankCode = convertResult.Item2;
                    realDetailList.Add(item);
                }

                if (realDetailList.Count == 0)
                {
                    return GetResponseModel(retModel, "无有效归集账户",0, failAccountList);
                }
                cashSweepmodel.Lines = realDetailList;
                return retModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/GetAccountNumberDecrypt:异常{ex.ToString()}\n cashSweepmodel={JsonConvert.SerializeObject(cashSweepmodel)};");
                return GetResponseModel(retModel, "账号解密异常", 0, failAccountList);
            }
        }
        
        #region 转账  
        /// <summary>
        /// 交易请求 支付状态(0：失败;1：成功;2：处理中;3:提交 )
        /// </summary>
        /// <param name="cashSweepmodel"></param>
        /// <param name="realDetailList"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<Tuple<string, string>> YqtTransformPay(FM_CashSweepODataEntity cashSweepmodel, List<FM_CashSweepDetailODataEntity> realDetailList, List<FinanceAccount> list, string batchNo)
        {
            var msg = "";
            try
            {
                var direction = cashSweepmodel.SweepDirectionID;
                if (direction == "1811191754180000101")////向上归集
                {
                    foreach (var item in realDetailList)
                    {
                        var orderNo = _numericalOrderCreator.CreateAsync().Result;
                        item.OrderNo = orderNo;
                        list.Add(new FinanceAccount()
                        {
                            accName = item.AccountFullName,
                            accNo = item.AccountNumber,
                            amount = item.AutoSweepBalance.ToString(),//归集金额，对应之前的自动归集金额
                            bankType = item.BankCode,
                            clientId = item.BankNumber,
                            drawee = item.EnterpriseID,//付款人
                            iscomm = "2",// 1:对私 2：对公
                            //localFlag = "0", // 汇路
                            note = "转账归集", //string.IsNullOrEmpty(cashSweepmodel.Remarks) ? "归集转账" : cashSweepmodel.Remarks,
                            orderNo = orderNo,//金融所用的流水号
                            payee = cashSweepmodel.EnterpriseID,
                            toAccName = cashSweepmodel.AccountFullName,
                            toAccNo = cashSweepmodel.AccountNumber,
                            toBankName = cashSweepmodel.DepositBank,
                            toBankCode = cashSweepmodel.BankCode, // 收款人银行编码
                            flag = 0,//0：上存；1：下拨
                            //billName = "默认",//
                            billNo = cashSweepmodel.Number,
                            uniqueNo = (item.NumericalOrder + "" + item.RecordID),
                            //markingAmount = sumAmount.ToString(),
                            makingDate = cashSweepmodel.DataDate,
                            makingOrgId = cashSweepmodel.EnterpriseID
                        });
                    }
                }
                else if (direction == "1811191754180000102")//向下归集
                {
                    foreach (var item in realDetailList)
                    {
                        var orderNo = _numericalOrderCreator.CreateAsync().Result;
                        item.OrderNo = orderNo;
                        list.Add(new FinanceAccount()
                        {
                            accName = cashSweepmodel.AccountFullName,
                            accNo = cashSweepmodel.AccountNumber,
                            amount = item.AutoSweepBalance.ToString(),//归集金额，对应之前的自动归集金额
                            bankType = cashSweepmodel.BankCode,
                            clientId = cashSweepmodel.BankNumber,
                            drawee = cashSweepmodel.EnterpriseID,//付款人
                            iscomm = "2",// 1:对私 2：对公
                            //localFlag = "0", // 汇路
                            note = "转账归集", //string.IsNullOrEmpty(cashSweepmodel.Remarks) ? "归集转账" : cashSweepmodel.Remarks,
                            orderNo = orderNo,//金融所用的流水号
                            payee = item.EnterpriseID,
                            toAccName = item.AccountFullName,
                            toAccNo = item.AccountNumber,
                            toBankName = item.DepositBank,
                            toBankCode = item.BankCode, // 收款人银行编码
                            flag = 1,//0：上存；1：下拨
                            //billName = "默认",//
                            billNo = cashSweepmodel.Number,
                            uniqueNo = (item.NumericalOrder + "" + item.RecordID),
                            //markingAmount = sumAmount.ToString(),
                            makingDate = cashSweepmodel.DataDate,
                            makingOrgId = cashSweepmodel.EnterpriseID
                            //detailUrl= "/CommonRedirect/RedirectToView?toView="
                        });
                    }
                }
                else
                {
                    return new Tuple<string, string>("归集方向错误", "");
                }
                var clientIdEmptyList = list.Where(p => string.IsNullOrEmpty(p.clientId));
                if (clientIdEmptyList?.Count() > 0)
                {
                    var accnameList = clientIdEmptyList.Select(p => p.accName).Distinct().ToArray();
                    var accnames = string.Join(",", accnameList);
                    return new Tuple<string, string>(accnames + "网银客户号空", "");
                }
                #region 请求数据

                #region Dictionary值
                var userid = string.IsNullOrEmpty(_identityService.UserId) ? cashSweepmodel.OwnerID : _identityService.UserId;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("batch_no", batchNo);//批次号不可重复（重新发起支付）
                dic.Add("opt_user_id", userid);
                dic.Add("data", JsonConvert.SerializeObject(list));
                dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
                dic.Add("busi_no", "NX-I-005"); /*业务来源*/
                dic.Add("call_back_url", string.Format($"{_hostCongfiguration._wgUrl}/dbn/fm/api/FM_CashSweepFinance/GetCashSweepCallBack")); /*回调地址*/
                dic.Add("busi_order_no", cashSweepmodel.NumericalOrder);
                //1811191754180000201 手动归集  1811191754180000202 自动归集
                if (cashSweepmodel.SweepType == "1811191754180000201")
                {
                    dic.Add("collect_type", "MANUAL");//自动归集-AUTO 手动归集-MANUAL
                }
                else if (cashSweepmodel.SweepType == "1811191754180000202")
                {
                    dic.Add("collect_type", "AUTO");
                }

                dic.Add("busi_type", "26"); //17:转账; 26:转账归集]
                dic.Add("apply_id",cashSweepmodel.NumericalOrder);//自动归集方案申请id	busi_type=26&collect_type=AUTO 必填
                var sgin = "";
                var key = _hostCongfiguration.OrderKey ?? "5dcc96e21db642dab159f97dbe61ff39";
                dic.Add("key", key); /*发送平台的key*/
                Dictionary<string, object> sortMap = new Dictionary<string, object>();

                sortMap = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                foreach (var t in sortMap)
                {
                    sgin += t.Key + t.Value;
                }
                var md5Str = RptBankAccountInquiryCommon.MD5Encrypt(sgin);


                dic.Add("signmsg", md5Str);
                dic.Add("open_req_src", "qlw-web");
                dic = dic.Where(t => t.Key != "key").ToDictionary(t => t.Key, t => t.Value);
                dic = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                string ps = "";
                foreach (var keyValuePair in dic)
                {
                    ps += keyValuePair.Key + "=" + keyValuePair.Value + "&";
                }
                #endregion
                ps = ps.TrimEnd('&');
                #endregion

                var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.batch.transfer/1.0";
                _logger.LogInformation($@"YqtTransformPay：url={url}\n param={ps}");
                var responseStr = postMessage(url, ps);
                await AddLog(null, cashSweepmodel.NumericalOrder, batchNo, url +"?"+ ps, responseStr,1);
                return new Tuple<string, string>("", responseStr);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/YqtTransformPay:异常{ex.ToString()}\n cashSweepmodel={JsonConvert.SerializeObject(cashSweepmodel)};");
                return new Tuple<string, string>("金融转账归集接口异常", "");
            }
        }

        #endregion

        #region 无效
        #region 获取使用接口（转账还是归集）
        /// <summary>
        /// 查询归集调用的接口方式
        /// </summary>
        /// <param name="clientId"></param>
        ///  <param name="bankType"></param>
        /// <returns> true-归集 false-转账</returns>
        private Result<bool> GetCashSweepOrTransform(string clientId, string bankType)
        {
            var result = new Result<bool>();
            try
            {
                var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.yqt.collect.api.get/1.0";
                var requestParm = "?clientId=" + clientId + "&bankType=" + bankType + "&open_req_src=qlw-web";
                url = url + requestParm;
                result.code = 0;
                return _httpClientUtil1.GetJsonAsync<Result<bool>>(url).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/GetCashSweepOrTransform:异常{ex.ToString()}\n clientId={clientId};bankType={bankType}");
                result.msg = $"获取归集使用接口异常";
            }
            return result;
        }
        #endregion

        #region 归集
        /// <summary>
        /// 归集调用的接口
        /// </summary>
        /// <returns></returns>
        private async Task<Tuple<string, string>> YqtCollection(FM_CashSweepODataEntity cashSweepmodel, List<FM_CashSweepDetailODataEntity> realDetailList, List<FinanceAccount> list, string batchNo)
        {
            var msg = "";
            try
            {
                var direction = cashSweepmodel.SweepDirectionID;
                //向上
                if (direction == "1811191754180000101")
                {
                    foreach (var item in realDetailList)
                    {
                        var orderNo = _numericalOrderCreator.CreateAsync().Result;
                        item.OrderNo = orderNo;
                        list.Add(new FinanceAccount()
                        {
                            accNo = item.AccountNumber,
                            amount = item.AutoSweepBalance.ToString(),//归集金额，对应之前的自动归集金额
                            bankType = item.BankCode,
                            clientId = item.BankNumber,
                            orderNo = orderNo,//金融所用的流水号                           
                            toAccNo = cashSweepmodel.AccountNumber,
                            //billName = "默认",
                            billNo = cashSweepmodel.Number,
                            uniqueNo = item.NumericalOrder + "" + item.RecordID,
                            flag = 0,//归集标识[0：上存；1：下拨]

                            accName = item.AccountFullName,
                            drawee = item.EnterpriseID,//付款人
                            payee = cashSweepmodel.EnterpriseID,
                            toAccName = cashSweepmodel.AccountFullName,
                        });
                    }
                }
                else if (direction == "1811191754180000102")//向下归集
                {
                    foreach (var item in realDetailList)
                    {
                        var orderNo = _numericalOrderCreator.CreateAsync().Result;
                        item.OrderNo = orderNo;
                        list.Add(new FinanceAccount()
                        {
                            accNo = cashSweepmodel.AccountNumber,
                            amount = item.AutoSweepBalance.ToString(),//归集金额，对应之前的自动归集金额
                            bankType = cashSweepmodel.BankCode,
                            clientId = cashSweepmodel.BankNumber,
                            orderNo = orderNo,//金融所用的流水号                         
                            toAccNo = item.AccountNumber,
                            //billName = "默认",
                            billNo = cashSweepmodel.Number,
                            uniqueNo = item.NumericalOrder + "" + item.RecordID,
                            flag = 1,//归集标识[0：上存；1：下拨]

                            accName = cashSweepmodel.AccountFullName,
                            drawee = cashSweepmodel.EnterpriseID,//付款人
                            payee = item.EnterpriseID,
                            toAccName = item.AccountFullName,
                            //toBankName = item.DepositBank,
                            //toBankCode = item.BankCode, // 收款人银行编码
                            //markingAmount = sumAmount.ToString(),
                            //makingDate = cashSweepmodel.DataDate,
                            //makingOrgId = cashSweepmodel.EnterpriseID,                            
                            //detailUrl= "/CommonRedirect/RedirectToView?toView="
                        });
                    }
                }
                else
                {
                    return new Tuple<string, string>("归集方向错误", "");
                }
                var clientIdEmptyList = list.Where(p => string.IsNullOrEmpty(p.clientId));
                if (clientIdEmptyList?.Count() > 0)
                {
                    var accnameList = clientIdEmptyList.Select(p => p.accName).Distinct().ToArray();
                    var accnames = string.Join(",", accnameList);
                    return new Tuple<string, string>(accnames + "网银客户号空", "");
                }
                #region 请求数据

                #region Dictionary值
                var userid = string.IsNullOrEmpty(_identityService.UserId) ? cashSweepmodel.OwnerID : _identityService.UserId;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("batch_no", batchNo);//批次号不可重复（重新发起支付）
                dic.Add("busi_type", "24");
                dic.Add("opt_user_id", userid);
                dic.Add("data", JsonConvert.SerializeObject(list));
                dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
                dic.Add("busi_no", "NX-I-005"); /*业务来源*/
                dic.Add("call_back_url", string.Format($"{_hostCongfiguration._wgUrl}/dbn/fm/api/FM_CashSweepFinance/GetCashSweepCallBack")); /*回调地址*/
                dic.Add("busi_order_no", cashSweepmodel.NumericalOrder);
                //1811191754180000201 手动归集  1811191754180000202 自动归集
                if (cashSweepmodel.SweepType == "1811191754180000201")
                {
                    dic.Add("collect_type", "MANUAL");//自动归集-AUTO 手动归集-MANUAL
                }
                else if (cashSweepmodel.SweepType == "1811191754180000202")
                {
                    dic.Add("collect_type", "AUTO");
                }

                var sgin = "";

                var key = _hostCongfiguration.OrderKey ?? "5dcc96e21db642dab159f97dbe61ff39";
                dic.Add("key", key); /*发送平台的key*/ // 2022.9.27测试可去掉
                Dictionary<string, object> sortMap = new Dictionary<string, object>();

                sortMap = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                foreach (var t in sortMap)
                {
                    sgin += t.Key + t.Value;
                }
                var md5Str = RptBankAccountInquiryCommon.MD5Encrypt(sgin);


                dic.Add("signmsg", md5Str);
                dic.Add("open_req_src", "qlw-web");
                dic = dic.Where(t => t.Key != "key").ToDictionary(t => t.Key, t => t.Value);
                dic = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                string ps = "";
                foreach (var keyValuePair in dic)
                {
                    ps += keyValuePair.Key + "=" + keyValuePair.Value + "&";
                }
                #endregion
                ps = ps.TrimEnd('&');
                #endregion

                var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.auto.collection/1.0";
                _logger.LogInformation($@"YqtCollection：url={url}\n param={ps}");
                var responseStr = postMessage(url, ps);
                await AddLog(null, cashSweepmodel.NumericalOrder, batchNo, url + ps, responseStr, 1);
                return new Tuple<string, string>("", responseStr);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/YqtCollection:异常{ex.ToString()}\n cashSweepmodel={JsonConvert.SerializeObject(cashSweepmodel)};");
                return new Tuple<string, string>("金融归集接口异常", "");
            }
        }
        #endregion
        #endregion
        #endregion

        #region 修改状态
        /// <summary>
        /// 修改payextend和detail归集状态（安全认证结果后调用）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateStatus")]
        public Result UpdateStatus(VerifyStatus model)
        {
            if (model?.Status == 1)
            {
                model.Status = 2;//处理中
            }
            return UpdateExcuteStatus(model)?.Result;
        }
        [HttpPost]
        [Route("UpdateExcuteStatus")]
        public async Task<Result> UpdateExcuteStatus(VerifyStatus model)
        {
            var result = new Result();
            try
            {
                if (model == null)
                {
                    result.msg = "参数空";
                    return result;
                }
                if (string.IsNullOrEmpty(model.NumericalOrder))
                {
                    result.msg = "NumericalOrder空";
                    return result;
                }
                if (string.IsNullOrEmpty(model.BatchNo))
                {
                    result.msg = "BatchNo空";
                    return result;
                }
                var payextendList = _payextendRepository.GetById(model.NumericalOrder)?.Where(p => p.BatchNo == model.BatchNo);
                if (payextendList == null || payextendList.Count() == 0)
                {
                    result.msg = "未查询到归集记录";
                    return result;
                }
                var detailList = _cashSweepdetailRepository.GetDetailByID(model.NumericalOrder);
                if (detailList == null || detailList.Count() == 0)
                {
                    result.msg = "未查询到归集详情";
                    return result;
                }
                if (model.Status == 2)
                {
                    model.Msg = "处理中";
                }
                else if (model.Status == 0)
                {
                    if (string.IsNullOrEmpty(model.Msg))
                    {
                        model.Msg = "交易失败";
                    }
                }

                foreach (var item in payextendList)
                {
                    if (item.PayStatus != 1)
                    {
                        item.Remarks = model.Msg;
                        item.PayStatus = model.Status;
                        _payextendRepository.Update(item);
                    }
                    var filterDetails = detailList.Where(p => p.RecordID.ToString() == item.NumericalOrderDetail);
                    if (filterDetails?.Count() > 0)
                    {
                        var detail = filterDetails.FirstOrDefault();
                        if (detail.Status != 1)
                        {
                            detail.ExcuteMsg = model.Msg;
                            detail.Status = model.Status;
                            _cashSweepdetailRepository.Update(detail);
                        }
                    }
                }
                await _payextendRepository.UnitOfWork.SaveChangesAsync();
                await _cashSweepdetailRepository.UnitOfWork.SaveChangesAsync();

                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/UpdateStatus:异常{ex.ToString()}\n param={JsonConvert.SerializeObject(model)}");
                result.msg = $"更新状态异常";
            }
            return result;
        }
        #endregion
                
        #region 交易回调
        /// <summary>
        ///  交易结果回调 
        ///  0：失败，1：成功 ，2：处理中
        ///  修改fd_payextend、FM_CashSweepDetail，若成功则生成单据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCashSweepCallBack")]
        public async Task<int> GetCashSweepCallBack([FromForm] PayReturnResult pay)
        {
            return await CashSweepCallBack(pay);
        }


        private async Task<int> CashSweepCallBack(PayReturnResult pay)
        {
            var result = 0;
            try
            {
                var responseStr = JsonConvert.SerializeObject(pay);
                _logger.LogInformation($"GetCashSweepCallBack资金归集消息返回数据：" + responseStr);
                if (pay == null || string.IsNullOrEmpty(pay.OrderNo))
                {
                    return result;
                }
                await AddLog(null, pay.OrderNo, "0", "", responseStr, 2);
                var rediskey = $"CashSweep|OrderNo:{pay.OrderNo}|TradeNo:{pay.TradeNo}|SerialNo:{pay.SerialNo}|Status:{pay.Status}|Amount:{pay.Amount}";
                var csredis = new CSRedis.CSRedisClient(_hostCongfiguration.RedisServer);
                //初始化 RedisHelper
                RedisHelper.Initialization(csredis);
                //Redis锁防止并发
                if (!RedisHelper.Exists(rediskey))
                {
                    RedisHelper.Set(rediskey, JsonConvert.SerializeObject(pay));
                }
                else
                {
                    return 1;
                }

                if (pay != null)
                {
                    result = 1;
                    //获取支付记录
                    var payextendList = _payextendRepository.GetByOrderNo(pay.TradeNo)?.Where(p => p.NumericalOrder == pay.OrderNo);
                    if (payextendList?.Count() > 0)
                    {
                        var payextend = payextendList.FirstOrDefault();
                        var isSuccess = false;
                        isSuccess = payextend.PayStatus == 1 ? true : false;
                        payextend.PayStatus = int.Parse(pay.Status);
                        payextend.PayTypeName = pay.PayTypeName;
                        payextend.PayCode = pay.PayCode;
                        payextend.PayNO = pay.SerialNo;
                        payextend.Remarks = pay.FailureMsg;
                        _payextendRepository.Update(payextend);
                        await _payextendRepository.UnitOfWork.SaveChangesAsync();
                        //获取归集详情
                        var detailDomain = _cashSweepdetailRepository.GetDetailByRecordID(int.Parse(payextend.NumericalOrderDetail));
                        if (detailDomain == null || string.IsNullOrEmpty(detailDomain.NumericalOrder))
                        {
                            //_logger.LogInformation($"GetCashSweepCallBack资金归集消息返回数据：未查询到FM_CashSweepDetail,recordID={payextend.NumericalOrderDetail}");
                            var msg = $"未生成单据：未查询到详情,标识：{payextend.NumericalOrderDetail}";
                            UpdateDb(payextend, detailDomain, msg);
                            
                        }
                        else
                        {
                            detailDomain.Status = int.Parse(pay.Status);
                            //detailDomain.Remark += pay.FailureMsg;
                            detailDomain.ExcuteMsg = pay.FailureMsg;
                            _cashSweepdetailRepository.Update(detailDomain);
                            await _cashSweepdetailRepository.UnitOfWork.SaveChangesAsync();
                        }
                        //支付成功 生成单据
                        if (pay.Status == "1")
                        {
                            AddData(payextend, detailDomain, pay);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"GetCashSweepCallBack资金归集消息返回数据：未查询到Payextend");
                        //如果是自动归集 可能存在fd_payextend表未写入就收到消息
                        RedisHelper.Del(rediskey);
                        result = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCashSweepCallBack资金归集消息返回异常: " + ex.ToString() + "结果：" + JsonConvert.SerializeObject(pay));
            }
            return result;
        }
        /// <summary>
        ///  交易结果回调 
        ///  0：失败，1：成功 ，2：处理中
        ///  修改fd_payextend、FM_CashSweepDetail，若成功则生成单据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCashSweepCallBackBody")]
        public async Task<int> GetCashSweepCallBackBody([FromBody] PayReturnResult pay)
        {
            return await CashSweepCallBack(pay);
        }
        /// <summary>
        /// 归集成功生成单据：资金调拨单、付款单、收款单、根据系统选项是否生成凭证
        /// 关联数据：关联关系、制单人
        /// 表头与表体单位一致，账户资金调拨，只生成付款单，付款凭证借贷方都有账户，不生成收款单
        /// </summary>
        /// <param name="payextend"></param>
        /// <param name="detail"></param>
        /// <param name="pay"></param>
        private async Task<int> AddData(FD_Payextend payextend,FM_CashSweepDetail detail, PayReturnResult pay)
        {
            #region 保存表
            //调拨单：fd_accounttransfer / fd_accounttransferdetail
            //             biz_related 调拨单和归集单关系
            //             SubjectInfo 申请主题
            //付款单：fd_paymentreceivables / fd_paymentreceivablesdetail （集团标识）
            //             biz_related 调拨单和付款单关系
            //             biz_reviwe 制单人（单位、集团）
            //             fd_settlereceiptextend 收款账户
            //付款凭证：fd_settlereceipt / fd_settlereceiptdetail 
            //             biz_reviwe 制单人
            //收款单：fd_paymentreceivables / fd_paymentreceivablesdetail
            //             biz_related 调拨单和收款单关系
            //             biz_reviwe 制单人
            //收款凭证：fd_settlereceipt / fd_settlereceiptdetail 
            //             biz_reviwe 制单人
            #endregion
            var retresult = 0;
            try
            {               
                var msg = "";
                #region 获取验证数据
               
                //获取资金调拨表头                  
                var main = _cashSweeprepository.Get(detail.NumericalOrder);               
                if (main == null || string.IsNullOrEmpty(main.NumericalOrder))
                {
                    msg = "未生成单据：未获取到资金归集表头数据";
                    UpdateDb(payextend, detail, msg);
                    return retresult;
                }
                if (string.IsNullOrEmpty(main?.AccountID))
                {
                    msg = "未生成单据：表头账号空";
                    UpdateDb(payextend, detail, msg);
                    return retresult;
                }
                if (string.IsNullOrEmpty(detail.AccountID))
                {
                    msg = "未生成单据：账号空";
                    UpdateDb(payextend, detail, msg);
                    return retresult;
                }                
               
                //获取资金账户设置
                var mainAccount = _accountProvider.GetData(long.Parse(main.AccountID));
                if (mainAccount == null)
                {
                    msg = "未生成单据：未获取到表头账户设置信息";
                    UpdateDb(payextend, detail, msg);
                    return retresult;
                }
                var detailAccount = _accountProvider.GetData(long.Parse(detail.AccountID));
                if (detailAccount == null)
                {
                    msg = "未生成单据：未获取到账户设置信息";
                    UpdateDb(payextend, detail, msg);
                    return retresult;
                }

                #region 资金归集设置
                var direction = 0;
                var payEnteID = "0";
                var receEnteID = "0";               
                var outAccount = new FD_AccountAODataEntity();
                var inAccount = new FD_AccountAODataEntity();
                //向上归集 表头：收方 表体：付方
                if (main.SweepDirectionID == "1811191754180000101") 
                {
                    direction = 0;
                    payEnteID = detail.EnterpriseID;
                    receEnteID = main.EnterpriseID;
                    outAccount = detailAccount;//调出
                    inAccount = mainAccount;//调入
                }
                else if (main.SweepDirectionID == "1811191754180000102")//向下归集
                {
                    direction = 1;
                    payEnteID = main.EnterpriseID;
                    receEnteID = detail.EnterpriseID;
                    outAccount = mainAccount;//调出
                    inAccount = detailAccount;//调入
                }
                else
                {
                    msg = "未生成单据：归集方向有误：" + main.SweepDirectionID;
                    UpdateDb(payextend, detail, msg);
                    return retresult;
                }
                var paySettingDetail = new FM_CashSweepSettingDetailODataEntity();
                var receSettingDetail = new FM_CashSweepSettingDetailODataEntity();
                var paysetting = new FM_CashSweepSettingExtODataEntity();
                var recesetting = new FM_CashSweepSettingExtODataEntity();
                var paySettingResult = GetSettingResult(payextend, detail, direction, payEnteID,0);//付款
                if (paySettingResult == null || !string.IsNullOrEmpty(paySettingResult.Item1))
                {
                    return retresult;
                }
                paySettingDetail = paySettingResult.Item2;
                paysetting = paySettingResult.Item3;
                if (main.EnterpriseID != detail.EnterpriseID)
                {
                    var receSettingResult = GetSettingResult(payextend, detail, direction, receEnteID,1);
                    if (receSettingResult == null || !string.IsNullOrEmpty(receSettingResult.Item1))
                    {
                        return retresult;
                    }
                    receSettingDetail = receSettingResult.Item2;
                    recesetting = receSettingResult.Item3;
                }
                else
                {
                    receSettingDetail = paySettingDetail;                   
                    recesetting = paySettingDetail?.Extends?.Where(p=>p.BusiType==1)?.FirstOrDefault();
                }

                #endregion
                if (!string.IsNullOrEmpty(msg))
                {
                    return retresult;
                }
                #endregion
                #region 保存变量声明
                var transfer = new Domain.FD_AccountTransfer();
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                List<Biz_Review> reviewList = new List<Biz_Review>();
                Domain.FD_PaymentReceivables payDomian = null;
                Domain.FD_SettleReceipt paySettle = null;
                //List<fd_paymentreceivablesvoucher> voucherList = new List<fd_paymentreceivablesvoucher>();
                fd_paymentreceivablesvoucher voucher = new fd_paymentreceivablesvoucher();
                fd_paymentreceivablesvoucher recevoucher = new fd_paymentreceivablesvoucher();
                Domain.FD_PaymentReceivables receDomian = null;
                Domain.FD_SettleReceipt receSettle = null;
                SubjectInfo subject = null;
                fd_settlereceiptextend settlereceiptextend = null;
                FD_PaymentExtend paymentExtend = null;
                FD_PaymentExtend recepaymentExtend = null;
                string transferNumericalOrder = "0";
                var accountTransferType = "";
                var accountTransferAbstract = "0";
                var inPaymentTypeID = "0";
                var inAccountID = "0";
                var businessType = "0";
                var pCustomerID = "0";
                #endregion

                #region 构建保存数据
                //向上归集   单位一致：表体单位账户资金调拨，表体单位付款单 、凭证（借贷方都有资金账户）                
                #region 资金调拨
                if (main.EnterpriseID != detail.EnterpriseID)
                {
                    accountTransferType = EnteAccountTransferType;//单位资金调拨
                    accountTransferAbstract = receSettingDetail?.AccountTransferAbstract;
                    businessType = GroupBusinessType;
                    pCustomerID = receEnteID;
                }
                else
                {
                    accountTransferType = AccountTransferType;  //账户资金调拨
                    inPaymentTypeID = inAccount.PaymentTypeID;//账户资金调拨的借贷方都要挂资金账户、结算方式
                    inAccountID = inAccount.AccountID;
                    businessType = OtherBusinessType;
                }
                transferNumericalOrder = await _numericalOrderCreator.CreateAsync();
                transfer.EnterpriseID = receEnteID;//调入单位
                transfer.AccountTransferAbstract =string.IsNullOrEmpty(accountTransferAbstract)?"0": accountTransferAbstract;
                transfer.AccountTransferType = accountTransferType;
                transfer.Guid = Guid.NewGuid();
                transfer.DataDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                transfer.CreatedDate = DateTime.Now;
                transfer.ModifiedDate = DateTime.Now;
                transfer.NumericalOrder = transferNumericalOrder;
                transfer.Remarks = receSettingDetail?.Remarks;
                transfer.OwnerID =string.IsNullOrEmpty(receSettingDetail?.OwnerID)?"0":receSettingDetail?.OwnerID;
                //调入
                var inDetail = new FD_AccountTransferDetail()
                {
                    NumericalOrder = transferNumericalOrder,
                    Guid = Guid.NewGuid(),
                    EnterpriseID = receEnteID,
                    PaymentTypeID = inAccount.PaymentTypeID ?? "0",
                    AccountID = inAccount.AccountID ?? "0",
                    Amount = payextend.Amount,
                    IsIn = true,
                    DataDateTime = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Remarks = ""
                };
                transfer.AddDetail(inDetail);
                //调出
                var outDetail = new FD_AccountTransferDetail()
                {
                    NumericalOrder = transferNumericalOrder,
                    Guid = Guid.NewGuid(),
                    EnterpriseID = payEnteID,
                    PaymentTypeID = outAccount.PaymentTypeID ?? "0",
                    AccountID = outAccount.AccountID ?? "0",
                    Amount = payextend.Amount,
                    IsIn = false,
                    DataDateTime = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Remarks = ""
                };
                transfer.AddDetail(outDetail);

                //调拨单和归集单关系
                relateds.Add(new BIZ_Related()
                {
                    RelatedType = FormRelatedType,
                    ParentType = CashSweepAppID,//资金归集
                    ChildType = AccountTransferAppID,//资金调拨单
                    ParentValue = detail.RecordID.ToString(),
                    ChildValue = transferNumericalOrder,
                    ParentValueDetail = detail.NumericalOrder,
                    Remarks = "资金调拨关联资金归集"
                });
                //添加申请主题
                //subject = new SubjectInfo()
                //{
                //    NumericalOrder = transferNumericalOrder,
                //    Subject = "资金调拨",
                //    SubjectExtendInfo = new SubjectExtendInfo()
                //    {
                //        Amount = payextend.Amount,
                //        DateInfo = new NameValue { Name = "申请日期", Value = transfer.DataDate.ToString() }
                //    }
                //};
                #endregion

                #region 付款单
                //获取系统选项，是否复杂版
                var isComplex = _fmBaseCommon.GetBizOptionConfig("201612270104402002", outDetail.EnterpriseID, 4, "1");
                //单据号调RD接口
                var payNumber = CreateNumber(transfer.DataDate, outDetail.EnterpriseID).Result;
                var payNumericalOrder = await _numericalOrderCreator.CreateAsync();
                payDomian = new Domain.FD_PaymentReceivables()
                {
                    TicketedPointID = string.IsNullOrEmpty(paysetting.TicketedPointID) ? "0" : paysetting.TicketedPointID,
                    Number = payNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = transfer.DataDate,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = payNumericalOrder,
                    Remarks = "资金集中归集",
                    OwnerID = paysetting.OwnerID,
                    EnterpriseID = outDetail.EnterpriseID,
                    SettleReceipType = PayType,
                    IsGroupPay = true
                };
                var guid = Guid.NewGuid();
                var payDetail = new FD_PaymentReceivablesDetail()
                {
                    NumericalOrder = payNumericalOrder,
                    Guid = guid,
                    AccountID = outAccount.AccountID,
                    EnterpriseID = payDomian.EnterpriseID,
                    Amount = outDetail.Amount,
                    BusinessType = businessType,
                    Content = transfer.Remarks,
                    CustomerID = pCustomerID,
                    PersonID = "0",
                    MarketID = "0",
                    PaymentTypeID = outAccount.PaymentTypeID,
                    ReceiptAbstractID = paysetting.ReceiptAbstractID
                };
                payDomian.details.Add(payDetail);
                //调拨单和付款单关系
                relateds.Add(new BIZ_Related()
                {
                    RelatedType = FormRelatedType,
                    ParentType = PayNewAppID,//付款单
                    ChildType = AccountTransferAppID,//资金调拨单
                    ParentValue = payNumericalOrder,
                    ChildValue = transferNumericalOrder,
                    Remarks = "申请关联收付款单"
                });

                //付款单制单人
                reviewList.Add(new Biz_Review(payNumericalOrder, PayNewAppID, payDomian.OwnerID).SetMaking());//集团
                reviewList.Add(new Biz_Review(payNumericalOrder, PayAppID, payDomian.OwnerID).SetMaking());//单位
                //账户信息
                settlereceiptextend = new fd_settlereceiptextend
                {
                    NumericalOrder = payNumericalOrder,
                    NumericalOrderDetail = "0",
                    BankID = inAccount.BankID,
                    AccountName = inAccount.AccountFullName,
                    AccountNumber = inAccount.AccountNumber,
                    DepositBank = inAccount.DepositBank,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                paymentExtend = new FD_PaymentExtend
                {
                    NumericalOrder = payDetail.NumericalOrder,
                    Guid = payDetail.Guid,
                    CollectionId = payDetail.CustomerID,
                    PersonId = payDetail.CustomerID,
                    AccountName = inAccount.AccountName,
                    BankDeposit = inAccount.DepositBank,
                    BankAccount = inAccount.AccountNumber,
                    Amount = outDetail.Amount,
                    PayResult = pay.FailureMsg,
                    Status = 1,//0:未发起，1：成功，2：失败，3：交易中
                    PayUse = "转账归集",
                    IsRecheck=false
                };
                #region 集中支付需要

                #endregion

                #endregion

                #region 付款凭证 
                if (isComplex == "1")
                {                    
                    var paysettleNumber =await CreateSettleNumber(payDomian.DataDate.ToString(), payDomian.EnterpriseID, PaySettleReceipType,payDomian.TicketedPointID);
                    paySettle = new Domain.FD_SettleReceipt()
                    {
                        NumericalOrder = payDomian.NumericalOrder,
                        Number = paysettleNumber,//_settleReceiptprovider.GetMaxNumberByDate(PaySettleReceipType, payDomian.EnterpriseID, period.StartDate.ToString("yyyy-MM-dd"), period.EndDate.ToString("yyyy-MM-dd")).MaxNumber,
                        Guid = payDomian.Guid,
                        DataDate = payDomian.DataDate,
                        TicketedPointID = payDomian.TicketedPointID,
                        Remarks = payDomian.Remarks,
                        EnterpriseID = payDomian.EnterpriseID,
                        OwnerID = payDomian.OwnerID,
                        CreatedDate = payDomian.CreatedDate,
                        ModifiedDate = payDomian.ModifiedDate,
                        SettleReceipType = PaySettleReceipType,
                        details = new List<FD_SettleReceiptDetail>()
                    };
                    //借方 多方
                    paySettle.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = payDetail.NumericalOrder,
                        Guid = payDetail.Guid,
                        EnterpriseID = payDetail.EnterpriseID,
                        ReceiptAbstractID = payDetail.ReceiptAbstractID,
                        AccoSubjectID = paysetting.AccoSubjectID,
                        AccoSubjectCode = paysetting.AccoSubjectCode,
                        CustomerID = payDetail.CustomerID,
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = "0",
                        ProductID = "0",
                        Content = payDetail.Content,
                        PaymentTypeID = inPaymentTypeID,
                        AccountID = inAccountID,
                        Credit = 0,
                        Debit = payDetail.Amount == null ? 0 : decimal.Parse(payDetail.Amount.ToString()),
                        LorR = false,
                        RowNum = 1,
                        OrganizationSortID = string.IsNullOrEmpty(paysetting.OrganizationSortID) ? "0" : paysetting.OrganizationSortID,
                    });
                    //贷方
                    paySettle.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = payDetail.NumericalOrder,
                        Guid = payDetail.Guid,
                        EnterpriseID = payDetail.EnterpriseID,
                        ReceiptAbstractID = payDetail.ReceiptAbstractID,
                        AccoSubjectID = outAccount.AccoSubjectID,//调出账户资金账户设置科目
                        AccoSubjectCode = outAccount.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = "0",
                        ProductID = "0",
                        Content = payDetail.Content,
                        PaymentTypeID = payDetail.PaymentTypeID,
                        AccountID = payDetail.AccountID,
                        Credit = payDetail.Amount == null ? 0 : decimal.Parse(payDetail.Amount.ToString()),
                        Debit = 0,
                        LorR = true,
                        RowNum = 2,
                        OrganizationSortID = string.IsNullOrEmpty(paysetting.OrganizationSortID) ? "0" : paysetting.OrganizationSortID,
                    });
                    //凭证制单人
                    reviewList.Add(new Biz_Review(payNumericalOrder, SettleAppID, paySettle.OwnerID).SetMaking());
                    #region 会计凭证扩展数据
                    voucher = new Domain.fd_paymentreceivablesvoucher()
                    {
                        NumericalOrder = payDomian.NumericalOrder,
                        Number = payDomian.Number,
                        Guid = payDomian.Guid,
                        DataDate = payDomian.DataDate,
                        TicketedPointID = payDomian.TicketedPointID,
                        Remarks = payDomian.Remarks,
                        EnterpriseID = payDomian.EnterpriseID,
                        OwnerID = payDomian.OwnerID,
                        CreatedDate = payDomian.CreatedDate,
                        ModifiedDate = payDomian.ModifiedDate,
                        SettleReceipType = PaySettleReceipType,
                        details = new List<fd_paymentreceivablesvoucherdetail>()
                    };                   
                    //借方 多方
                    voucher.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = payDetail.NumericalOrder,
                        Guid = payDetail.Guid,
                        EnterpriseID = payDetail.EnterpriseID,
                        ReceiptAbstractID = payDetail.ReceiptAbstractID,
                        AccoSubjectID = paysetting.AccoSubjectID,
                        AccoSubjectCode = paysetting.AccoSubjectCode,
                        CustomerID = payDetail.CustomerID,
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = "0",
                        ProductID = "0",
                        Content = payDetail.Content,
                        PaymentTypeID = inPaymentTypeID,
                        AccountID = inAccountID,
                        Credit = 0,
                        Debit = payDetail.Amount == null ? 0 : decimal.Parse(payDetail.Amount.ToString()),
                        LorR = false,
                        RowNum = 1,
                        OrganizationSortID = string.IsNullOrEmpty(paysetting.OrganizationSortID) ? "0" : paysetting.OrganizationSortID,
                    });
                    //贷方
                    voucher.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = payDetail.NumericalOrder,
                        Guid = payDetail.Guid,
                        EnterpriseID = payDetail.EnterpriseID,
                        ReceiptAbstractID = payDetail.ReceiptAbstractID,
                        AccoSubjectID = outAccount.AccoSubjectID,//调出账户资金账户设置科目
                        AccoSubjectCode = outAccount.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = "0",
                        ProductID = "0",
                        Content = payDetail.Content,
                        PaymentTypeID = payDetail.PaymentTypeID,
                        AccountID = payDetail.AccountID,
                        Credit = payDetail.Amount == null ? 0 : decimal.Parse(payDetail.Amount.ToString()),
                        Debit = 0,
                        LorR = true,
                        RowNum = 2,
                        OrganizationSortID = string.IsNullOrEmpty(paysetting.OrganizationSortID) ? "0" : paysetting.OrganizationSortID,
                    });
                    #endregion
                }

                #endregion

                #region 收款单/收款凭证
                //资金调拨单 表头和表体单位一样则生成账户资金调拨，否则生成单位资金调拨
                if (main.EnterpriseID != detail.EnterpriseID)
                {
                    #region 收款单
                    var receNumber = CreateNumber(transfer.DataDate, inDetail.EnterpriseID).Result; //long receNumber = _numberCreator.Create<Domain.FD_PaymentReceivables>(o => o.DataDate, o => o.Number, transfer.DataDate, o => o.EnterpriseID == transfer.EnterpriseID);
                    var receNumericalOrder = await _numericalOrderCreator.CreateAsync();
                    receDomian = new Domain.FD_PaymentReceivables()
                    {
                        TicketedPointID = string.IsNullOrEmpty(recesetting.TicketedPointID) ? "0" : recesetting.TicketedPointID,
                        Number = receNumber.ToString(),
                        Guid = Guid.NewGuid(),
                        DataDate = transfer.DataDate,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        NumericalOrder = receNumericalOrder,
                        Remarks = "资金集中归集",
                        OwnerID = recesetting.OwnerID,
                        EnterpriseID = inDetail.EnterpriseID,
                        SettleReceipType = ReceType,
                        IsGroupPay = true
                    };
                    var receDetail = new FD_PaymentReceivablesDetail()
                    {
                        NumericalOrder = receNumericalOrder,
                        Guid = Guid.NewGuid(),
                        AccountID = inAccount.AccountID,
                        EnterpriseID = receDomian.EnterpriseID,
                        Amount = payextend.Amount,
                        BusinessType = GroupBusinessType,
                        Content = "资金集中归集",
                        CustomerID = detail.EnterpriseID,
                        PersonID = "0",
                        MarketID = "0",
                        PaymentTypeID = inAccount.PaymentTypeID,
                        ReceiptAbstractID = recesetting.ReceiptAbstractID
                    };
                    receDomian.details.Add(receDetail);
                    //调拨单和收款单关系
                    relateds.Add(new BIZ_Related()
                    {
                        RelatedType = FormRelatedType,
                        ParentType = AccountTransferAppID,//资金调拨单
                        ChildType = ReceAppID,//收款单
                        ParentValue = transferNumericalOrder,
                        ChildValue = receNumericalOrder,
                        Remarks = "资金调拨关联收款单"
                    });

                    //收款单制单人
                    reviewList.Add(new Biz_Review(receNumericalOrder, ReceAppID, receDomian.OwnerID).SetMaking());

                    recepaymentExtend = new FD_PaymentExtend
                    {
                        NumericalOrder = receDetail.NumericalOrder,
                        Guid = receDetail.Guid,
                        CollectionId = receDetail.CustomerID,
                        PersonId = receDetail.CustomerID,
                        AccountName = outAccount.AccountName,
                        BankDeposit = outAccount.DepositBank,
                        BankAccount = outAccount.AccountNumber,
                        Amount = inDetail.Amount,
                        PayResult = pay.FailureMsg,
                        Status = 1,//0:未发起，1：成功，2：失败，3：交易中
                        PayUse = "转账归集",
                        IsRecheck = false
                    };
                    #endregion

                    #region 收款凭证
                    if (isComplex == "1")
                    {
                        var recesettleNumber = await CreateSettleNumber(receDomian.DataDate.ToString(), receDomian.EnterpriseID, ReceSettleReceipType,receDomian.TicketedPointID);
                        receSettle = new Domain.FD_SettleReceipt()
                        {
                            NumericalOrder = receDomian.NumericalOrder,
                            Number = recesettleNumber,
                            Guid = receDomian.Guid,
                            DataDate = receDomian.DataDate,
                            TicketedPointID = receDomian.TicketedPointID,
                            Remarks = receDomian.Remarks,
                            EnterpriseID = receDomian.EnterpriseID,
                            OwnerID = receDomian.OwnerID,
                            CreatedDate = receDomian.CreatedDate,
                            ModifiedDate = receDomian.ModifiedDate,
                            SettleReceipType = ReceSettleReceipType,
                            details = new List<FD_SettleReceiptDetail>()
                        };
                        //借方
                        receSettle.details.Add(new FD_SettleReceiptDetail()
                        {
                            NumericalOrder = receDetail.NumericalOrder,
                            Guid = receDetail.Guid,
                            EnterpriseID = receDetail.EnterpriseID,
                            ReceiptAbstractID = receDetail.ReceiptAbstractID,
                            AccoSubjectID = inAccount.AccoSubjectID,
                            AccoSubjectCode = inAccount.AccoSubjectCode,
                            CustomerID = "0",
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = "0",
                            ProductID = "0",
                            Content = receDetail.Content,
                            PaymentTypeID = receDetail.PaymentTypeID,
                            AccountID = receDetail.AccountID,
                            Credit = 0,
                            Debit = receDetail.Amount == null ? 0 : decimal.Parse(receDetail.Amount.ToString()),
                            LorR = false,
                            RowNum = 1,
                            OrganizationSortID = string.IsNullOrEmpty(recesetting.OrganizationSortID) ? "0" : recesetting.OrganizationSortID,
                        });
                        //贷方 多方
                        receSettle.details.Add(new FD_SettleReceiptDetail()
                        {
                            NumericalOrder = receDetail.NumericalOrder,
                            Guid = receDetail.Guid,
                            EnterpriseID = receDetail.EnterpriseID,
                            ReceiptAbstractID = receDetail.ReceiptAbstractID,
                            AccoSubjectID = recesetting.AccoSubjectID,
                            AccoSubjectCode = recesetting.AccoSubjectCode,
                            CustomerID = receDetail.CustomerID,
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = "0",
                            ProductID = "0",
                            Content = receDetail.Content,
                            PaymentTypeID = "0",
                            AccountID = "0",
                            Credit = receDetail.Amount == null ? 0 : decimal.Parse(receDetail.Amount.ToString()),
                            Debit = 0,
                            LorR = true,
                            RowNum = 2,
                            OrganizationSortID = string.IsNullOrEmpty(recesetting.OrganizationSortID) ? "0" : recesetting.OrganizationSortID,
                        });
                        //凭证制单人
                        reviewList.Add(new Biz_Review(receNumericalOrder, SettleAppID, receSettle.OwnerID).SetMaking());
                        #region 会计凭证扩展数据
                        recevoucher = new fd_paymentreceivablesvoucher()
                        {
                            NumericalOrder = receDomian.NumericalOrder,
                            Number = receDomian.Number,
                            Guid = receDomian.Guid,
                            DataDate = receDomian.DataDate,
                            TicketedPointID = receDomian.TicketedPointID,
                            Remarks = receDomian.Remarks,
                            EnterpriseID = receDomian.EnterpriseID,
                            OwnerID = receDomian.OwnerID,
                            CreatedDate = receDomian.CreatedDate,
                            ModifiedDate = receDomian.ModifiedDate,
                            SettleReceipType = ReceSettleReceipType,
                            details = new List<fd_paymentreceivablesvoucherdetail>()
                        };
                        //借方
                        recevoucher.details.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            NumericalOrder = receDetail.NumericalOrder,
                            Guid = receDetail.Guid,
                            EnterpriseID = receDetail.EnterpriseID,
                            ReceiptAbstractID = receDetail.ReceiptAbstractID,
                            AccoSubjectID = inAccount.AccoSubjectID,
                            AccoSubjectCode = inAccount.AccoSubjectCode,
                            CustomerID = "0",
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = "0",
                            ProductID = "0",
                            Content = receDetail.Content,
                            PaymentTypeID = receDetail.PaymentTypeID,
                            AccountID = receDetail.AccountID,
                            Credit = 0,
                            Debit = receDetail.Amount == null ? 0 : decimal.Parse(receDetail.Amount.ToString()),
                            LorR = false,
                            RowNum = 1,
                            OrganizationSortID = string.IsNullOrEmpty(recesetting.OrganizationSortID) ? "0" : recesetting.OrganizationSortID,
                        });
                        //贷方 多方
                        recevoucher.details.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            NumericalOrder = receDetail.NumericalOrder,
                            Guid = receDetail.Guid,
                            EnterpriseID = receDetail.EnterpriseID,
                            ReceiptAbstractID = receDetail.ReceiptAbstractID,
                            AccoSubjectID = recesetting.AccoSubjectID,
                            AccoSubjectCode = recesetting.AccoSubjectCode,
                            CustomerID = receDetail.CustomerID,
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = "0",
                            ProductID = "0",
                            Content = receDetail.Content,
                            PaymentTypeID = "0",
                            AccountID = "0",
                            Credit = receDetail.Amount == null ? 0 : decimal.Parse(receDetail.Amount.ToString()),
                            Debit = 0,
                            LorR = true,
                            RowNum = 2,
                            OrganizationSortID = string.IsNullOrEmpty(recesetting.OrganizationSortID) ? "0" : recesetting.OrganizationSortID,
                        });
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #endregion

                #region 保存数据
                await Task.Factory.StartNew(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var accountTransferRepository = scope.ServiceProvider.GetRequiredService<IFD_AccountTransferRepository>();
                        var reviewRepository = scope.ServiceProvider.GetRequiredService<IBiz_ReviewRepository>();
                        var relatedRepository = scope.ServiceProvider.GetRequiredService<IBiz_Related>();
                        var paymentReceivablesRepository = scope.ServiceProvider.GetRequiredService<IFD_PaymentReceivablesRepository>();
                        var paymentReceivablesDetailRepository = scope.ServiceProvider.GetRequiredService<IFD_PaymentReceivablesDetailRepository>();
                        var settleReceiptRepository = scope.ServiceProvider.GetRequiredService<IFD_SettleReceiptRepository>();
                        var settleReceiptDetailRepository = scope.ServiceProvider.GetRequiredService<IFD_SettleReceiptDetailRepository>();
                        var settlereceiptextendRepository = scope.ServiceProvider.GetRequiredService<IFD_settlereceiptextendRepository>();
                        var paymentExtendRepository = scope.ServiceProvider.GetRequiredService<IFD_PaymentExtendRepository>();
                        var voucherRepository = scope.ServiceProvider.GetRequiredService<Ifd_paymentreceivablesvoucherRepository>();
                        var voucherDetailRepository = scope.ServiceProvider.GetRequiredService<Ifd_paymentreceivablesvoucherDetailRepository>();
                        var type = "";
                        try
                        {

                            await accountTransferRepository.AddAsync(transfer);
                            //_applySubjectUtil.AddSubject(subject); 使用资金归集审批，不需要增加审批
                            await relatedRepository.AddRangeAsync(relateds);
                            await reviewRepository.AddRangeAsync(reviewList);
                            await paymentReceivablesRepository.AddAsync(payDomian);
                            await paymentReceivablesDetailRepository.AddRangeAsync(payDomian.details);
                            await settlereceiptextendRepository.AddAsync(settlereceiptextend);
                            await paymentExtendRepository.AddAsync(paymentExtend);
                            if (paySettle != null && !string.IsNullOrEmpty(paySettle.NumericalOrder))
                            {
                                await settleReceiptRepository.AddAsync(paySettle);
                                await settleReceiptDetailRepository.AddRangeAsync(paySettle.details);
                                await voucherRepository.AddAsync(voucher);
                                await voucherDetailRepository.AddRangeAsync(voucher.details);
                            }
                            if (receDomian != null)
                            {
                                await paymentReceivablesRepository.AddAsync(receDomian);
                                await paymentReceivablesDetailRepository.AddRangeAsync(receDomian.details);
                                await paymentExtendRepository.AddAsync(recepaymentExtend);
                                if (receSettle != null && !string.IsNullOrEmpty(receSettle.NumericalOrder))
                                {
                                    await settleReceiptRepository.AddAsync(receSettle);
                                    await settleReceiptDetailRepository.AddRangeAsync(receSettle.details);
                                    await voucherRepository.AddAsync(recevoucher);
                                    await voucherDetailRepository.AddRangeAsync(recevoucher.details);
                                }
                            }
                            await accountTransferRepository.UnitOfWork.SaveChangesAsync();
                            type = "调拨单";
                            await relatedRepository.UnitOfWork.SaveChangesAsync();
                            type = "关系";
                            await reviewRepository.UnitOfWork.SaveChangesAsync();
                            type = "制单人";
                            await paymentReceivablesRepository.UnitOfWork.SaveChangesAsync();
                            await paymentReceivablesDetailRepository.UnitOfWork.SaveChangesAsync();
                            type = "收付款单";
                            await settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                            await settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
                            type = "收付款凭证";
                            await settlereceiptextendRepository.UnitOfWork.SaveChangesAsync();
                            type = "付款扩展";
                            await paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                            type = "集中付款扩展";
                            await voucherRepository.UnitOfWork.SaveChangesAsync();
                            await voucherDetailRepository.UnitOfWork.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"AddData资金归集生成单据保存异常:{ ex.ToString()}param：payextend={JsonConvert.SerializeObject(payextend)}");
                            msg = $"生成单据异常:{type}" + ex.Message;
                            UpdateDb(payextend, detail, msg);
                        }
                    }
                });
                retresult = 1;
                #endregion               
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddData资金归集生成单据异常:{ ex.ToString()}param：payextend={JsonConvert.SerializeObject(payextend)}");
                var errormsg = "生成单据异常"+ex.Message;
                UpdateDb(payextend, detail, errormsg);
            }
            
            return retresult;
        }
        private Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity> GetSettingResult(FD_Payextend payextend, FM_CashSweepDetail detail,int direction,string EnterpriseID,int busiType)
        {
            var msg = "";
            //Tuple<string, FM_CashSweepSettingExt, FM_CashSweepSettingExt> tuple = null;
            var settingExt = new FM_CashSweepSettingExtODataEntity();
            var settingDetail = new FM_CashSweepSettingDetailODataEntity();
            try
            {
                //获取资金归集设置
                var settingResult = GetSetting(EnterpriseID);
                if (settingResult == null || settingResult.data == null)
                {
                    msg = "未生成单据：未获取到资金归集设置";
                    UpdateDb(payextend, detail, msg);
                    return new Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity>(msg, settingDetail, settingExt);
                }
                else if (!string.IsNullOrEmpty(settingResult.msg))
                {
                    msg = settingResult.msg;
                    UpdateDb(payextend, detail, msg);
                    return new Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity>(msg, settingDetail, settingExt);
                }
                var dirctionMsg = direction == 0 ? "上" : "下";
                var setting = settingResult.data;
                //归集方向(0：向上归集；1：向下归集)
                var upSettingResult = setting.Lines.Where(p => p.SweepDirection == direction);
                if (upSettingResult == null || upSettingResult.Count() == 0)
                {
                    msg =string.Format("未生成单据：资金归集设置中无向{0}归集设置", dirctionMsg);
                    UpdateDb(payextend, detail, msg);
                    return new Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity>(msg, settingDetail, settingExt);
                }
                else
                {
                    settingDetail = upSettingResult.FirstOrDefault();
                    //业务类型(0:付款 1：收款)
                    var busiTypeMsg = busiType==0?"付": "收";
                    var paySettingResult = settingDetail.Extends.Where(p => p.BusiType == busiType);
                    if (paySettingResult == null || paySettingResult.Count() == 0)
                    {
                        msg = string.Format("未生成单据：资金归集设置中向{0}归集无{1}款单设置", dirctionMsg, busiTypeMsg);
                        UpdateDb(payextend, detail, msg);
                        return new Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity>(msg, settingDetail, settingExt);
                    }
                    else
                    {
                        settingExt = paySettingResult.FirstOrDefault();
                        return new Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity>(msg, settingDetail, settingExt);
                    }                    
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"GetSettingResult资金归集生成单据获取归集设置异常:{ ex.ToString()}param：payextend={JsonConvert.SerializeObject(payextend)}");
                msg = "未生成单据：获取资金归集设置异常";
                UpdateDb(payextend, detail, msg);
                return new Tuple<string, FM_CashSweepSettingDetailODataEntity, FM_CashSweepSettingExtODataEntity>(msg, settingDetail, settingExt);
            }
        }
        //[HttpPost]
        //[Route("TestCreateNumber")]
        private async Task<long> CreateNumber(DateTime DataDate, string EnterpriseID)
        {
           long payNumber = 0;
           await Task.Factory.StartNew(() =>
           {
               using (var scope = _serviceScopeFactory.CreateScope())
               {
                   var numberCreator = scope.ServiceProvider.GetRequiredService<NumberCreator<Nxin_Qlw_BusinessContext>>();
                   payNumber = numberCreator.Create<Domain.FD_PaymentReceivables>(DataDate, o => o.Number, o => o.Number.StartsWith(DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == EnterpriseID); //numberCreator.Create<Domain.FD_PaymentReceivables>(o => o.DataDate, o => o.Number, DataDate, o => o.EnterpriseID == EnterpriseID);
               }
           });
            return payNumber;
        }
        private async Task<string> CreateSettleNumber(string DataDate, string EnterpriseID,string SettleReceipType,string TicketedPointID)
        {
            string Number = "0";
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        //获取会计期间
                        var period = _enterprisePeriodUtil.GetEnterperisePeriod(EnterpriseID, DataDate);
                        var tsettleReceiptprovider = scope.ServiceProvider.GetRequiredService<FD_SettleReceiptNewODataProvider>();
                        Number = tsettleReceiptprovider.GetMaxNumberByDate(SettleReceipType, EnterpriseID, period.StartDate.ToString("yyyy-MM-dd"), period.EndDate.ToString("yyyy-MM-dd"), TicketedPointID).MaxNumber;
                }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateSettleNumber资金归集生成单据异常 生成凭证单据号:{ ex.ToString()}param：SettleReceipType={SettleReceipType}，EnterpriseID={EnterpriseID}，DataDate={DataDate}");
            }            
            return Number;
        }
        /// <summary>
        /// 生成单据结果 更新
        /// </summary>
        /// <param name="payextend"></param>
        /// <param name="detail"></param>
        /// <param name="msg"></param>
        private async Task<int> UpdateDb(FD_Payextend payextend, FM_CashSweepDetail detail, string msg)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        payextend.Remarks = msg;
                        var payextendRepository = scope.ServiceProvider.GetRequiredService<IFD_PayextendRepository>();
                        payextendRepository.Update(payextend);
                        await payextendRepository.UnitOfWork.SaveChangesAsync();
                        if (detail != null)
                        {
                            detail.ExcuteMsg = msg;
                            //detail.Remark += msg;
                            var cashSweepDetailRepository = scope.ServiceProvider.GetRequiredService<IFM_CashSweepDetailRepository>();
                            cashSweepDetailRepository.Update(detail);
                            await cashSweepDetailRepository.UnitOfWork.SaveChangesAsync();
                        }                        
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateDb资金归集生成单据更新异常:{ ex.ToString()}param：payextend={JsonConvert.SerializeObject(payextend)}");
            }
            return 1;
        }
        /// <summary>
        /// 获取单位的资金归集设置
        /// </summary>
        /// <param name="EnterpriseID"></param>
        private Result<FM_CashSweepSettingODataEntity> GetSetting(string EnterpriseID)
        {
            var result = new Result<FM_CashSweepSettingODataEntity>();
            try
            {
                var data = _cashSweepSettingProvider.GetDataByEnterID(EnterpriseID);
                if (data?.Count() > 0)
                {
                    var model = data.FirstOrDefault();
                    var num = long.Parse(model.NumericalOrder);
                    var detailList = _cashSweepSettingProvider.GetDetaiDatas(num);
                    if (detailList?.Count > 0)
                    {
                        model.Lines = detailList;
                        var extList = _cashSweepSettingProvider.GetExtendDatas(num);
                        if (extList?.Count > 0)
                        {
                            foreach (var item in detailList)
                            {
                                item.Extends = extList.Where(p => p.NumericalOrderDetail == item.NumericalOrderDetail)?.ToList();
                            }
                            result.code = ErrorCode.Success.GetIntValue();
                            result.data = model;
                            return result;
                        }
                        else
                        {
                            result.msg = ";未生成单据：资金归集设置有误,未获取到收付款设置";
                        }
                    }
                    else
                    {
                        result.msg = ";未生成单据：资金归集设置有误,未获取到详情设置";
                    }

                }
                else
                {
                    result.msg = ";未生成单据：资金归集设置未设置！";
                }
            }
            catch(Exception ex)
            {
                result.msg = "未生成单据：资金归集设置获取异常！";
                _logger.LogError($"GetSetting资金归集生成单据异常:{ ex.ToString()}param：EnterpriseID={EnterpriseID}");
            }
            return result;
        }

        #endregion

        #region 自动归集

        #region 启用/停用归集
        /// <summary>
        /// 修改归集启用状态
        /// 启动定时任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateIsUse")]
        public async Task<Result> UpdateIsUse([FromBody] FM_CashSweepStateModifyCommand request)
        {
            var result = new Result();
            try
            {
                //获取归集
                var domain = _cashSweeprepository.Get(request.NumericalOrder);
                if (domain == null)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "启用失败；未查询到单据信息！";
                    return result;
                }
                if(domain.SweepType!= "1811191754180000202")
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "非自动归集类型！";
                    return result;
                }
                if (domain.AutoTime==null)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "自动归集时间空！";
                    return result;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var action = "";   
                long? excuterID = null;
                //设置任务的时间，时间间隔 自动归集接口                
                //启用
                if (request.IsUse)
                {
                    var jobid = 0;
                    //新增任务
                    if (domain.JobID == null)
                    {
                        action = "add";
                        var jobUrl = $"{_hostCongfiguration._wgUrl}/dbn/fm/api/FM_CashSweepFinance/ExcuteAutoSweep"; //{_hostCongfiguration.DbnFmserviceUrl}/api/FM_CashSweepFinance
                        var timeConf = $"{domain.AutoTime?.Second} {domain.AutoTime?.Minute} {domain.AutoTime?.Hour} * * ?";
                        var jobParam="{\"NumericalOrder\":\"" + domain.NumericalOrder + "\"}";
                        var glueSource = "curl -d '{\"NumericalOrder\":\"" + domain.NumericalOrder + "\"}' -H 'Content-Type: application/json' " + jobUrl;
                        var a=  "curl -d '"+ jobParam+"}' -H 'Content-Type: application/json' " + jobUrl;
                        var jobGroup = _hostCongfiguration.FinanceJobGroup??"1";
                        dic.Add("jobGroup", jobGroup);//执行器ID 1：财务执行器
                        dic.Add("jobDesc", domain.EnterpriseID+ "资金自动归集"+domain.NumericalOrder);//任务描述
                        dic.Add("author", "luyacai");//负责人
                        dic.Add("scheduleType", "CRON");//调度类型 CRON:表达式
                        dic.Add("scheduleConf", timeConf);//调度表达式 执行时间 规则请搜索RON表达式写法
                        dic.Add("glueType", "GLUE_SHELL");//运行模式
                        dic.Add("executorParam", jobParam);//拓展参数 参数格式： xxx = xxxx & xxx = xx 
                        dic.Add("executorRouteStrategy", "FIRST");//路由策略 FIRST:第一个执行节点执行（分布式多节点路由规则）
                        dic.Add("misfireStrategy", "DO_NOTHING");//调度过期策略 （到设置时间未执行时策略）
                        dic.Add("executorBlockStrategy", "SERIAL_EXECUTION");//阻塞处理策略  SERIAL_EXECUTION：单机串行
                        dic.Add("executorTimeout", 0);//任务超时时间 单位秒，0不限制
                        //dic.Add("executorFailRetryCount", 3);//任务重试次数 
                        dic.Add("glueRemark", "SERIAL_EXECUTION");//脚本标注
                        dic.Add("glueSource", glueSource);
                        var responseJob = GetJobResult(action, dic);
                        result= GetReturnModel(responseJob, "新增");
                        if (!string.IsNullOrEmpty(result.msg)) { return result; }                       
                        int.TryParse(responseJob.content, out jobid);
                        //成功保存任务ID，否则提示
                        domain.JobID = jobid;
                    }
                    else
                    {
                        var timeConf = domain.AutoTime?.Second + " " + domain.AutoTime?.Minute + " " + domain.AutoTime?.Hour + " * * ?";
                        //编辑任务时间+启动任务 
                        action = "update";
                        dic.Add("id", domain.JobID);
                        dic.Add("scheduleConf", timeConf);//调度表达式 执行时间 规则请搜索RON表达式写法
                        var responseJob = GetJobResult(action, dic);
                        result = GetReturnModel(responseJob, "编辑");
                        if (!string.IsNullOrEmpty(result.msg)) { return result; }
                    }
                    Dictionary<string, object> dicStart = new Dictionary<string, object>();
                    dicStart.Add("id", domain.JobID);
                    //启动任务
                    action = "start";
                    var response = GetJobResult(action, dicStart);
                    result = GetReturnModel(response, "启动");
                    //新增任务后启动任务失败，更新任务ID,为停用状态
                    if (jobid>0) 
                    {
                        if (!string.IsNullOrEmpty(result.msg))
                        {
                            request.IsUse = false;
                        }                        
                    }
                    else if (!string.IsNullOrEmpty(result.msg))
                    {
                        return result;
                    }
                    excuterID = (string.IsNullOrEmpty(_identityService.UserId) ? 0 : long.Parse(_identityService.UserId));
                }
                else 
                {
                    if (domain.JobID != null)
                    {
                        //暂停任务
                        action = "stop";
                        dic.Add("id", domain.JobID);
                        var responseJob = GetJobResult(action, dic);
                        result = GetReturnModel(responseJob, "暂停");
                        if (!string.IsNullOrEmpty(result.msg)) { return result; }
                    }
                    else
                    {
                        result.msg = "任务值空";
                    }
                    excuterID = null;
                }
                //修改启用状态
                await Task.Factory.StartNew(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var cashSweepRepository = scope.ServiceProvider.GetRequiredService<IFM_CashSweepRepository>();
                        domain.IsUse = request.IsUse;
                        domain.ExcuterID =excuterID;
                        cashSweepRepository.Update(domain);
                        await cashSweepRepository.UnitOfWork.SaveChangesAsync();
                       
                    }
                });
                result.data = true;
                result.code = ErrorCode.Success.GetIntValue();

            }
            catch(Exception ex)
            {
                _logger.LogError($"UpdateIsUse自动归集异常：{ex.ToString()},param={JsonConvert.SerializeObject(request)}");
                result.code = ErrorCode.Aggregate.GetIntValue();
                result.msg = "启用异常；" + ex.Message;
                result.data = false;
            }
            
            return result;
        }
        private JobResonseModel GetJobResult(string action, Dictionary<string, object> dic)
        {
            var result = new JobResonseModel();
            try
            {
                var url = $"{_hostCongfiguration.FmXxlJobUrl}/{action}";
                var param = "";
                foreach (var t in dic)
                {
                    param += t.Key + "=" + t.Value + "&";
                }
                param = param.TrimEnd('&');
                var resonseStr = postMessage(url, param);
                _logger.LogInformation($"GetJobResult自动归集-定时任务操作：url={url},param={param},resonseStr={resonseStr}");
                return JsonConvert.DeserializeObject<JobResonseModel>(resonseStr);                
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetJobResult自动归集-定时任务操作异常：{ex.ToString()},param={JsonConvert.SerializeObject(dic)},actiong={action}");
                result.code = ErrorCode.Aggregate.GetIntValue();
                result.msg = "定时任务操作异常；" + ex.Message;
            }
            return result;
        }
        private Result GetReturnModel(JobResonseModel responseJob,string msg)
        {
            var result = new Result();
            try
            {
                if (responseJob == null)
                {
                    result.msg = $"失败，{msg}任务返回空！";
                    return result;
                }
                if (responseJob.code != 200)//成功
                {
                    result.msg = (string.IsNullOrEmpty(responseJob.msg) ? $"失败，{msg}任务失败！" : (msg+responseJob.msg));
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReturnModel自动归集修改使用状态异常：{ex.ToString()},param={JsonConvert.SerializeObject(responseJob)}，msg={msg}");
                result.code = ErrorCode.Aggregate.GetIntValue();
                result.msg = "定时任务接口异常";
                result.data = false;
            }
            return result;
        }
        #endregion

        #region 执行自动归集
        /// <summary>
        /// 自动归集  归集金额根据归集方案计算
        /// 归集方案为自定义时 归集金额=实时余额-备用金+调整金额（实时余额和备用金都取执行日期的金额）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExcuteAutoSweep")]
        public async Task<YqtPayResonseModel> ExcuteAutoSweep(AutoSweep param)
        {
            _logger.LogInformation($"FM_CashSweepFinance/AutoSweep: param={JsonConvert.SerializeObject(param)}");
            var retModel = new YqtPayResonseModel() { Code = 0 };
            var result = new YqtPayResonseModel(); 
            try
            {
                if (param == null )
                {
                    return await GetResponseAddLog(retModel, "参数空","0" , "0");
                }
                var NumericalOrder = param.NumericalOrder;
                if (string.IsNullOrEmpty(NumericalOrder))
                {
                    return await GetResponseAddLog(retModel, "NumericalOrder空", NumericalOrder, "0");
                }
                
                var num = long.Parse(param.NumericalOrder);
                //获取表头信息
                var model = _provider.GetData(num);
                if (model == null || string.IsNullOrEmpty(model.NumericalOrder))
                {
                    return await GetResponseAddLog(retModel, "未获取到单据信息" , NumericalOrder,"0");
                }
                if (model.SweepType != "1811191754180000202")
                {
                    return await GetResponseAddLog(retModel, "非自动归集类型" + model.SweepType, NumericalOrder, "0");
                }
                if (!model.IsNew)
                {
                    return await GetResponseAddLog(retModel, "请到原菜单归集" , NumericalOrder, "0");
                }
                if (model.IsUse==null||model.IsUse==false)
                {
                    return await GetResponseAddLog(retModel, "未启用", NumericalOrder, "0");
                }

                //转换银行编码
                var bankcodeResult = ConvertBankType(model.BankID);
                if (bankcodeResult == null)
                {
                    return await GetResponseAddLog(retModel, "未获取到表头银行编码" + model.BankID,NumericalOrder, "0");
                }
                else if (!bankcodeResult.Item1)
                {
                    return await GetResponseAddLog(retModel, $"{bankcodeResult.Item2},{model.BankID},{model.NumericalOrder}", NumericalOrder, "0");
                }
                model.BankCode = bankcodeResult.Item2;
                var detailList = _provider.GetDetaiDatas(num);
                if (detailList == null || detailList.Count == 0)
                {
                    return await GetResponseAddLog(retModel, "未获取到单据详情信息" , NumericalOrder, "0");
                }
                var reqList = detailList.Where(p => p.Status != 1 && p.Status != 2);
                if (reqList == null || reqList.Count() == 0)
                {
                    return await GetResponseAddLog(retModel, "无需要归集的详情信息" , NumericalOrder, "0");
                }
                //var bankList = reqList.Where(p => p.BankID != model.BankID);
                //if (bankList?.Count() > 0)
                //{
                //    return await GetResponseAddLog(retModel, "不支持跨行归集" + model.SweepType,NumericalOrder, "0");
                //}
                model.Lines = reqList.ToList();
                //执行归集请求
                retModel =await AutoSweep(model);               
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/AutoSweep:异常{ex.ToString()}\n param={JsonConvert.SerializeObject(param)}");
                retModel.Msg = $"归集异常";
                retModel.Code = 0;
            }
            return retModel;
        }
        //自动归集金额
        private async Task<YqtPayResonseModel> AutoSweep(FM_CashSweepODataEntity model)
        {
            var retModel = new YqtPayResonseModel() { Code = 0 };
            var failAccountList = new List<PageResponse>();
            retModel.Data = failAccountList;
            try
            {
                //账号解密
                retModel = GetAccountNumberDecrypt(model, retModel, failAccountList);
                if (!string.IsNullOrEmpty(retModel.Msg))
                {
                    return retModel;
                }
                switch (model.SchemeType)
                {
                    case "FixedAmount"://固定金额归集     方案设置的固定金额
                    case "FixedAmountDial"://固定金额下拨 方案设置的固定金额                         
                        if (model.SchemeAmount==null)
                        {
                            return await GetResponseAddLog(retModel, "方案设置金额空",model.NumericalOrder,"0");
                        }
                        var amount = decimal.Parse(model.SchemeAmount.ToString());
                        model.Lines?.ForEach(p => p.AutoSweepBalance = amount);
                        break;
                    case "ExcessDepositing"://保底超额归集 实时余额 - 方案设置金额或实时余额 *（1 - 方案设置百分比）
                        if ((model.SchemeAmount == null || model.SchemeAmount == 0)&&(model.Rate==null||model.Rate==0))
                        {
                            return await GetResponseAddLog(retModel, "保底超额归集方案金额和比例都为空或零 " , model.NumericalOrder,"0");
                        }
                        if ((model.SchemeAmount != null && model.SchemeAmount != 0) && (model.Rate != null && model.Rate != 0))
                        {
                            return await GetResponseAddLog(retModel, "保底超额归集方案金额和比例不能同时设置 " , model.NumericalOrder,"0");
                        }
                        var detailList = new List<FM_CashSweepDetailODataEntity>();
                        //获取实时余额
                        foreach (var item in model.Lines)
                        {
                           var balResult = _financeTradeUtil.GetYqtBal(item.AccountNumber, item.BankCode, item.BankNumber);
                            if (balResult==null|| !string.IsNullOrEmpty(balResult.Item1))
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-{balResult?.Item1}-{item.RecordID}" });
                                continue;
                            }
                            if (balResult.Item2 == 0)
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-实时余额为0-{item.RecordID}" });
                                continue;
                            }
                            var balAmount = balResult.Item2;
                            if ((model.SchemeAmount != null && model.SchemeAmount != 0))
                            {
                                var schemeAmount = decimal.Parse(model.SchemeAmount.ToString());
                                item.AutoSweepBalance = balAmount - schemeAmount;
                                if (item.AutoSweepBalance <= 0)
                                {
                                    failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-归集金额小于等于零。余额{balAmount},方案设置金额{schemeAmount}-{item.RecordID}" });
                                    continue;
                                }
                            }
                            else if ((model.Rate != null && model.Rate != 0))
                            {
                                var rate = decimal.Parse(model.Rate.ToString());
                                item.AutoSweepBalance = Math.Round(balAmount * (1-rate), 2, MidpointRounding.AwayFromZero);
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-归集金额小于等于零。余额{balAmount},方案设置比例{rate}-{item.RecordID}" });
                                continue;
                            }
                            detailList.Add(item);                           
                        }
                        model.Lines = detailList;                        
                        break;
                    case "FundPlan"://资金计划归集 lastday/ theday / nextday  方案按当日资金日计划流入金额或方案按上一日资金日计划流入金额
                    case "FundPlanDial"://资金计划下拨 lastday/ theday / nextday 方案按当日资金日计划流出金额或方案按上一日资金日计划流出金额
                        return await GetResponseAddLog(retModel, "资金计划归集方案暂不支持 " + model.SchemeType,model.NumericalOrder,"0");
                        break;
                    case "FundFull"://资金全额归集 实时余额
                        var fdetailList = new List<FM_CashSweepDetailODataEntity>();
                        //获取实时余额
                        foreach (var item in model.Lines)
                        {
                            var balResult = _financeTradeUtil.GetYqtBal(item.AccountNumber, item.BankCode, item.BankNumber);
                            if (balResult == null || !string.IsNullOrEmpty(balResult.Item1))
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-{balResult?.Item1}-{item.RecordID}" });
                                continue;
                            }
                            if (balResult.Item2 == 0)
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-实时余额为0-{item.RecordID}" });
                                continue;
                            }
                            item.AutoSweepBalance = balResult.Item2;
                            fdetailList.Add(item);                            
                        }
                        model.Lines = fdetailList;
                        break;
                    case "-1":
                        //自定义 实时余额-备用金+调整金额
                        var cdetailList = new List<FM_CashSweepDetailODataEntity>();                     
                        
                        foreach (var item in model.Lines)
                        {
                            //获取实时余额
                            var balResult = _financeTradeUtil.GetYqtBal(item.AccountNumber, item.BankCode, item.BankNumber);
                            if (balResult == null || !string.IsNullOrEmpty(balResult.Item1))
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-{balResult?.Item1}-{item.RecordID}" });
                                continue;
                            }
                            if (balResult.Item2 == 0)
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-实时余额为0-{item.RecordID}" });
                                continue;
                            }
                            //获取备用金
                            decimal reolvingFundAmount = 0;
                            var reolvingFundList = _provider.GetRevolvingFundData(new RevolvingFundDetailRequest { AccountID = item.AccountID, EnterpriseID = item.EnterpriseID, dDate = DateTime.Now.ToString("yyyy-MM-dd") })?.Result;
                            if (reolvingFundList != null && reolvingFundList.Count > 0)
                            {
                                //failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-理论额度（备用金）未获取到数据-{item.RecordID}" });
                                //continue;
                                reolvingFundAmount = reolvingFundList.FirstOrDefault().nMinimum;
                            }
                            
                            var balAmount = balResult.Item2;
                            var transformBalance = item.TransformBalance;
                            

                            item.AutoSweepBalance = balAmount- reolvingFundAmount + transformBalance;
                            if (item.AutoSweepBalance <= 0)
                            {
                                failAccountList.Add(new PageResponse() { Code = 0, Msg = $"[{item.AccountName}]-归集金额小于等于零。余额{balAmount},备用金{reolvingFundAmount},理论额度{transformBalance}-{item.RecordID}" });
                                continue;
                            }
                            cdetailList.Add(item);
                        }
                        model.Lines = cdetailList;
                        break;
                    default:
                        return await GetResponseAddLog(retModel, "归集方案不支持 " + model.SchemeType,model.NumericalOrder,"0");
                        break;
                }
                if (!string.IsNullOrEmpty(retModel?.Msg))
                {
                    return retModel;
                }
                if (model.Lines?.Count == 0)
                {
                    return await GetResponseAddLog(retModel, "无归集详情数据 ", model.NumericalOrder, "0");
                }
                retModel =YqtTrade(model, retModel, failAccountList);
                await AddLog(retModel, model.NumericalOrder, "0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/ExcuteAutoSweep:异常{ex.ToString()}\n param={JsonConvert.SerializeObject(model)}");
                return await GetResponseAddLog(retModel, "归集异常", model.NumericalOrder,"0");
            }
            return retModel;
        }
        #endregion
        #endregion

        #region 公用
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
                    res.Cookies = objCookieContainer.GetCookies(new Uri(strUrl));

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
                //_logger.LogError($@"异常：{e.ToString()},param={strPost};url={strUrl}");
                throw e;
            }

        }

        #region 银企直连 银行类型

        private Tuple<bool, string> ConvertBankType(string bankID)
        {
            var retResult = new Tuple<bool, string>(false, "账户暂不支持");
            try
            {
                if (bankID == ((long)FMSettleReceipType.CEBBankDicID).ToString())
                {
                    retResult = new Tuple<bool, string>(true, "CEB");
                }
                else if (bankID == ((long)FMSettleReceipType.ABCBankDicID).ToString())
                {
                    retResult = new Tuple<bool, string>(true, "ABC");
                }
                else if (bankID == ((long)FMSettleReceipType.MinShengBankDicID).ToString())
                {
                    retResult = new Tuple<bool, string>(true, "CMBC");
                }
                else if (bankID == ((long)FMSettleReceipType.ICBCBankDicID).ToString())
                {
                    retResult = new Tuple<bool, string>(true, "ICBC");
                }
                else if (bankID == ((long)FMSettleReceipType.PSBCBankDicID).ToString())
                {
                    retResult = new Tuple<bool, string>(true, "PSBC");
                }
                else if (bankID == ((long)FMSettleReceipType.ADBCBankDicID).ToString())//农业发展
                {
                    retResult = new Tuple<bool, string>(true, "ADBC");
                }
                else if (bankID == ((long)FMSettleReceipType.BCMBankDicID).ToString())
                {
                    retResult = new Tuple<bool, string>(true, "BCM");
                }
                //else if (bankID == ((long)FMSettleReceipType.NBBCBankDicID).ToString())
                //{
                //    retResult = new Tuple<bool, string>(true, BankType.Ningbo);
                //}
                else
                {
                    retResult = new Tuple<bool, string>(false, "账户暂不支持!");
                }
            }
            catch (Exception ex)
            {
                retResult = new Tuple<bool, string>(false, "转换银行类型异常");
            }

            return retResult;
        }
        #endregion

        #region 返回结果
        private YqtPayResonseModel GetResponseModel(YqtPayResonseModel retModel, string msg, int code = 0, List<PageResponse> data = null)
        {
            retModel.Code = code;
            retModel.Msg = msg;
            retModel.Data = data;
            _logger.LogInformation($"FM_CashSweepFinance/GetResponseModel:result={JsonConvert.SerializeObject(retModel)}");
            return retModel;
        }
        private async Task<YqtPayResonseModel> GetResponseAddLog(YqtPayResonseModel retModel, string msg, string numericalOrder, string batchNo, int code = 0, List<PageResponse> data = null, string request = "", string response = "")
        {
            retModel.Code = code;
            retModel.Msg = msg;
            retModel.Data = data;
            var returnStr = JsonConvert.SerializeObject(retModel);
            _logger.LogInformation($"FM_CashSweepFinance/GetResponseModel:result={returnStr}");

            await AddLog(retModel, numericalOrder, batchNo, request, response);
            return retModel;
        }
        private async Task<YqtPayResonseModel> AddLog(YqtPayResonseModel retModel, string numericalOrder, string batchNo, string request = "", string response = "",int type=0)//type=1：交易 2：消息通知 0：返回值
        {
            try
            {
                var returnStr = retModel == null ? "" : JsonConvert.SerializeObject(retModel);
                var log = new FM_CashSweepLog()
                {
                    NumericalOrder = numericalOrder,
                    BatchNo = batchNo,
                    ReturnResult = returnStr.Length > 2048 ? returnStr.Substring(0, 2048) : returnStr,
                    ResponseResult = response.Length > 2048 ? response.Substring(0, 2048) : response,
                    RequestResult = request.Length > 2048 ? request.Substring(0, 2048) : request,
                    BusType = type,
                    Remarks = retModel?.Msg,
                    ModifiedDate = DateTime.Now
                };
                _cashSweepLogRepository.Add(log);
                await _cashSweepLogRepository.UnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"FM_CashSweepFinance/AddLog:异常{ex.ToString()}\n cashSweepmodel={JsonConvert.SerializeObject(retModel)};");
            }
            return retModel;
        }
        #endregion

        #endregion

        //[HttpPost]
        //[Route("TestLog")]
        //public int TestLog([FromBody] FM_CashSweepODataEntity param)
        //{
        //    _logger.LogInformation($"FM_CashSweepFinance/LogInformation:1 \n  param={JsonConvert.SerializeObject(param)}");
        //    _logger.LogWarning($"FM_CashSweepFinance/LogWarning:2");
        //    _logger.LogError($"FM_CashSweepFinance/LogError:3");
        //    Serilog.Log.Information($"FM_CashSweepFinance/Serilog.Log.Information:4");
        //    Serilog.Log.Warning($"FM_CashSweepFinance/Serilog.Log.Warning");
        //    Serilog.Log.Error($"FM_CashSweepFinance/Serilog.Log.Error");
        //    return 1;
        //}
    }

    #region Model
    public class ExecuteSweepRequestModel
    {
        public long NumericalOrder { get; set; }
    }
    /// <summary>
    /// 金融交易参数
    /// </summary>
    public class FinanceAccount
    {
        /// <summary>
        /// 付款账户名称
        /// </summary>
        public string accName { get; set; }
        /// <summary>
        /// 付款账户
        /// </summary>
        public string accNo { get; set; }
        /// <summary>
        /// 转账金额,单位:元
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 银行类型
        /// </summary>
        public string bankType { get; set; }
        /// <summary>
        /// 企业客户号
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 付款人ID
        /// </summary>
        public string drawee { get; set; }
        /// <summary>
        /// 收款人类型1对私2对公（20200107）
        /// </summary>
        public string iscomm { get; set; }
        /// <summary>
        /// 汇路
        /// </summary>
        public string localFlag { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 业务支付订单号
        /// </summary>
        public string orderNo { get; set; }
        /// <summary>
        /// 收款人ID
        /// </summary>
        public string payee { get; set; }
        /// <summary>
        /// 测2200003220
        /// </summary>
        public string toAccName { get; set; }
        /// <summary>
        /// 收款账户
        /// </summary>
        public string toAccNo { get; set; }
        /// <summary>
        /// 收款人银行编码
        /// </summary>
        public string toBankCode { get; set; }
        /// <summary>
        /// 收款账户银行名称
        /// </summary>
        public string toBankName { get; set; }
        //制单单位id
        public string makingOrgId { get; set; }
        //摘要名称
        public string summary { get; set; }
        //制单日期 付款单/汇总单的DataDate
        public string makingDate { get; set; }
        //详情页
        public string detailUrl { get; set; }
        /// <summary>
        /// 付款单金额
        /// </summary>
        public string markingAmount { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string billName { get; set; }
        public string uniqueNo { get; set; }
        /// <summary>
        /// 归集标识[0：上存；1：下拨]
        /// </summary>
        public int? flag { get; set; }
    }

    public class RestfulResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
    public class YqtPayResonseData
    {
        /// <summary>
        /// 失败信息
        /// </summary>
        public object failInfo { get; set; }
        /// <summary>
        /// 失败条数
        /// </summary>
        public int failNum { get; set; }
        /// <summary>
        /// 成功条数
        /// </summary>
        public int successNum { get; set; }
        /// <summary>
        /// 成功信息
        /// </summary>
        public object successInfo { get; set; }
        public string securityPCUrl { get; set; }//PC端安全验证URL
        public object riskInfo { get; set; }//风控规则提示
        public int resCode { get; set; }//1:成功
        public string resMsg { get; set; }
}
    public class YqtPayResonseModel
    {
        public YqtPayResonseModel()
        {
            Data = new List<PageResponse>();
        }
        public string SecurityPCUrl { get; set; }//PC端安全验证URL
        public List<RiskModel> RiskList { get; set; }//风控规则
        public string BatchNo { get; set; }//批次号
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<PageResponse> Data { get; set; }
    }
    public class PageResponse
    {
        public string AccountName { get; set; }
        public int Code { get; set; }//(0：失败;1：成功;2：处理中;3:提交 )
        public string Msg { get; set; }
    }
    public class RiskModel
    {
        public string DateDate { get; set; }
        public string EnterpriseName { get; set; }
        public string AccountNumber { get; set; }
        public string SweepDirectionName { get; set; }
        public string SweepTypeName { get; set; }
        
        public decimal Amount { get; set; }
        public string Msg { get; set; }
        
    }
    public class PayReturnResult
    {
        /// <summary>
        /// 业务订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 业务支付订单号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 支付系统支付订单号
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 支付工具标识
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 支付工具编码
        /// </summary>
        public string PayCode { get; set; }
        /// <summary>
        /// 支付工具描述
        /// </summary>
        public string PayTypeName { get; set; }
        /// <summary>
        /// 支付结果
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 支付失败描述信息
        /// </summary>
        public string FailureMsg { get; set; }
    }

    public class JobResonseModel
    {
        public int code { get; set; }//200 成功
        public string msg { get; set; }
        public string content { get; set; }//  新建的任务ID
    }
    public class VerifyStatus
    {        
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 0:失败 1：成功 2:处理中
        /// </summary>
        public int Status { get; set; }
        public string Msg { get; set; }
    }
    public class AutoSweep
    {
        public string NumericalOrder { get; set; }
    }
    #endregion
}
