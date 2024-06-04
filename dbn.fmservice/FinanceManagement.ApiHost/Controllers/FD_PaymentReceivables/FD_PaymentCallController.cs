using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceiptNew;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Route("api/[controller]")]
    [ApiController]
    public class FD_PaymentCallController : ControllerBase
    {
        IMediator _mediator;
        //FD_PaymentReceivablesTODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;
        FD_PaymentReceivablesTODataProvider _prodiver;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FD_PaymentReceivablesController> _logger;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_PaymentReceivablesRepository _paymentReceivablesRepository;
        IFD_PaymentReceivablesDetailRepository _paymentReceivablesDetailRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        IFD_settlereceiptextendRepository _settlereceiptextendRepository;
        private QlwCrossDbContext _context;
        NumericalOrderCreator _numericalOrderCreator;
        Ifd_receivablessetRepository _ReceivablessetRepository;
        Ifd_bankreceivableRepository _ifd_Bankreceivable;
        IFD_AccountRepository _fD_AccountRepository;
        FD_AccountODataProvider _AccountODataProvider;
        FD_SettleReceiptNewODataProvider _provider;
        EnterprisePeriodUtil _enterprisePeriodUtil;
        public FD_PaymentCallController(Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,IFD_settlereceiptextendRepository settlereceiptextendRepository, IFD_SettleReceiptRepository settleReceiptRepository,IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository paymentReceivablesRepository,IFD_PaymentReceivablesDetailRepository paymentReceivablesDetailRepository, FD_SettleReceiptNewODataProvider provider,NumericalOrderCreator numericalOrderCreator, FD_AccountODataProvider AccountODataProvider, IFD_AccountRepository fD_AccountRepository, Ifd_bankreceivableRepository ifd_Bankreceivable, Ifd_receivablessetRepository ReceivablessetRepository, IMediator mediator, HttpClientUtil httpClientUtil,QlwCrossDbContext context,
            //FD_PaymentReceivablesTODataProvider provider, 
            EnterprisePeriodUtil enterprisePeriodUtil,
            FundSummaryUtil fundSummaryUtil, IFD_PaymentExtendRepository paymentExtendRepository, IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, ILogger<FD_PaymentReceivablesController> logger, HostConfiguration hostCongfiguration)
        {
            _mediator = mediator;
            _provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
            _prodiver = prodiver;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _paymentExtendRepository = paymentExtendRepository;
            _context = context;
            _numericalOrderCreator = numericalOrderCreator;
            _ReceivablessetRepository = ReceivablessetRepository;
            _fD_AccountRepository = fD_AccountRepository;
            _AccountODataProvider = AccountODataProvider;
            _ifd_Bankreceivable = ifd_Bankreceivable;
            _enterprisePeriodUtil = enterprisePeriodUtil;
            _paymentReceivablesRepository = paymentReceivablesRepository;
            _paymentReceivablesDetailRepository = paymentReceivablesDetailRepository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _settlereceiptextendRepository = settlereceiptextendRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
        }
        [HttpGet]
        [Route("GetPaymentList")]
        public Result GetPaymentList(long key)
        {
            var result = new Result();
            result.code = 0;
            var paymentLists = _prodiver.GetPaymentListByOpen(key.ToString());
            //解密银行卡号
            foreach (var item in paymentLists)
            {
                var resultRece = AccountNumberDecrypt(item.ffAccountID);
                item.ffAccountID = resultRece?.Item1 == true ? resultRece.Item2 : item.ffAccountID;

                var resultRece2 = AccountNumberDecrypt(item.sfBankNumber);
                item.sfBankNumber = resultRece2?.Item1 == true ? resultRece2.Item2 : item.sfBankNumber;
            }
            result.data = paymentLists;
            return result;
        }
        /// <summary>
        /// 对外接口，根据销售单流水号获取已收金额 ，收款方式，收款账号
        /// 工作任务【65429】
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSalseUnionReceivables")]
        public Result GetSalseUnionReceivables(string nums)
        {
            var result = new Result();
            result.code = 0;
            var paymentLists = _prodiver.GetSalseUnionReceivables(nums);
            //解密银行卡号
            foreach (var item in paymentLists)
            {
                var resultRece = AccountNumberDecrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            result.data = paymentLists;
            return result;
        }
        /// <summary>
        /// 金融专用 凭证新增接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddSettleReceipt")]
        public Result Add([FromBody] FD_SettleReceiptNewOutSideAddCommand request)
        {
            _logger.LogInformation("(金融)预付款使用记录信息同步:"+JsonConvert.SerializeObject(request));
            request.Remarks = "(金融)预付款使用记录信息同步,订单号："+request.Remarks;
            var res = new Result() { code = 1 };
            if (string.IsNullOrEmpty(request.EnterpriseID))
            {
                res.msg += "EnterpriseID 不允许为空,";
            }
            if (string.IsNullOrEmpty(request.SettleReceipType))
            {
                res.msg += "SettleReceipType 不允许为空,";
            }
            if (string.IsNullOrEmpty(request.DataDate))
            {
                res.msg += "DataDate 不允许为空,";
            }
            if (string.IsNullOrEmpty(request.TicketedPointID))
            {
                res.msg += "TicketedPointID 不允许为空,";
            }
            if (string.IsNullOrEmpty(request.OwnerID))
            {
                res.msg += "OwnerId 不允许为空,";
            }
            if (request.Lines.Count == 0)
            {
                res.msg += "Lines 不允许为空,";
            }
            if (string.IsNullOrEmpty(request.JRNumericalOrder))
            {
                res.msg += "JRNumericalOrder 不允许为空, ";
            }
            else
            {
                if (request.Lines.Sum(m => m.Credit) != request.Lines.Sum(m => m.Debit))
                {
                    res.msg += "Lines 明细行，借贷方金额不一致,";
                }
            }
            if (!string.IsNullOrEmpty(res.msg))
            {
                return res;
            }
            //一、金融给企联网推购物冲抵
            //1、凭证类别 = 转账凭证
            //2、单据字，对应单位默认第一个
            //3、摘要：预收充值冲抵（030102）
            //4、会计科目：
            //当客户消费后：
            //借：应收账款 - 预收充值 112205
            //贷：应收账款 - 应收外部款 112204
            //5、辅助项：客户ID

            var enData = _provider.GetBiz_EnterpriseInfos(request.EnterpriseID);
            if (enData == null)
            {
                return new Result() { code = 1,msg = $"当前单位:{request.EnterpriseID}在企联网中不存在" };
            }
            var period = _enterprisePeriodUtil.GetEnterperisePeriod(request.EnterpriseID, request.DataDate);
            request.BeginDate = period.StartDate.ToString("yyyy-MM-dd");
            request.EndDate = period.EndDate.ToString("yyyy-MM-dd");
            //科目
            var accos = _provider.GetAccoSubjectInfos(request.DataDate, new EnterpriseInfo() { EnterpriseId = request.EnterpriseID },enData.Pid);
            //摘要
            var settles = _provider.GetSettleSummaryInfos(enData.Pid);
            //单据字
            var tickets = _provider.GetTicketPointInfos(enData.Pid);
            //2、单据字，对应单位默认第一个
            request.TicketedPointID = tickets.Where(m => m.EnterpriseId == request.EnterpriseID).FirstOrDefault()?.TicketedPointId;
            //借：应收账款 - 预收充值 112205
            request.Lines.Where(m => !m.LorR).FirstOrDefault().AccoSubjectID = accos.Where(m => m.AccoSubjectCode == "112205").FirstOrDefault()?.AccoSubjectId;
            //贷：应收账款 - 应收外部款 112204
            request.Lines.Where(m => m.LorR).FirstOrDefault().AccoSubjectID = accos.Where(m => m.AccoSubjectCode == "112204").FirstOrDefault()?.AccoSubjectId;
            //3、摘要：预收充值冲抵（030102）
            foreach (var item in request.Lines)
            {
                item.EnterpriseID = "0";
                item.ReceiptAbstractID = settles.Where(m => m.Remarks == "030102" && m.EnterpriseId == request.EnterpriseID).FirstOrDefault()?.SettleSummaryId;
            }

            var rest = _mediator.Send(request, HttpContext.RequestAborted).Result;
            return rest;
        }
        [HttpPost]
        [Route("ReCharge")]
        public async Task<int> ReCharge(AdvanceCharge advance)
        {
            if (advance.busiNo == null)
            {
                return 1;
            }
            _logger.LogInformation("集中收款充值处理：" + JsonConvert.SerializeObject(advance));
            for (int xx = 0; xx < 5; xx++)
            {
                if (advance == null)
                {
                    return 0;
                }
                var domain = _paymentExtendRepository.GetAsync(advance.busiOrderNo).Result;
                //充值 更新流水
                if (advance.transType == "RECHARGE")
                {
                    domain.TradeNo = _numericalOrderCreator.CreateAsync().Result;
                    advance.busiOrderNo = domain.TradeNo;
                    _paymentExtendRepository.SaveChange(domain);
                }
                advance.times = DateTime.Now.ToString("yyyyMMddHHmm");
                advance.transTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("acctType", advance.acctType);
                dic.Add("amt", advance.amt);
                dic.Add("busiNo", advance.busiNo);
                dic.Add("busiOrderNo", advance.busiOrderNo);
                dic.Add("custId", advance.custId);
                dic.Add("entId", advance.entId);
                dic.Add("operatorId", advance.operatorId);
                dic.Add("platCode", advance.platCode);
                dic.Add("times", advance.times);
                dic.Add("transTime", advance.transTime);
                dic.Add("transType", advance.transType);
                dic.Add("sourceBusiOrderNo", advance.sourceBusiOrderNo);
                //获取sign
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

                advance.signmsg = md5Str;

                try
                {
                    var res = _httpClientUtil1.PostJsonAsync<dynamic, Result<ReCharge>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.cmp.cashbook.recharge/1.0?open_req_src=nxin_shuju", advance).Result;
                    if (domain == null)
                    {
                        domain = new Domain.FD_PaymentExtend();
                    }
                    domain.PayResult = res.data.resMsg;
                    domain.ModifiedDate = DateTime.Now;
                    if (res.data.resCode == "0" && xx == 0)
                    {
                        domain.Status = 2;
                        if (advance.transType != "RECHARGEBACK")
                        {
                            _paymentExtendRepository.SaveChange(domain);
                        }
                        await Task.Delay(60000);
                    }
                    if (res.data.resCode == "1" && xx == 0)
                    {
                        domain.Status = 1;
                        if (advance.transType != "RECHARGEBACK")
                        {
                            _paymentExtendRepository.SaveChange(domain);
                        }
                        Console.WriteLine($@"集中收款充值【{advance.transType}】调用成功：" + $"【第{xx}次结果：】" + JsonConvert.SerializeObject(res));
                        _logger.LogInformation($@"集中收款充值【{advance.transType}】调用成功：" + $"【第{xx}次结果：】" + JsonConvert.SerializeObject(res));
                        break;
                    }
                    //重试机制
                    if (xx > 0)
                    {
                        if (res.data.resCode == "1")
                        {
                            if (domain != null)
                            {
                                domain.PayResult = "重试:" + res.data.resMsg;
                                domain.Status = 1;
                                if (advance.transType != "RECHARGEBACK")
                                {
                                    _paymentExtendRepository.SaveChange(domain);
                                }
                            }
                            Console.WriteLine($@"集中收款充值【{advance.transType}】重试机制：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(res));
                            _logger.LogInformation($@"集中收款充值【{advance.transType}】重试机制：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(res));
                            return 1;
                        }
                        else
                        {
                            if (xx + 1 == 5)
                            {
                                if (domain != null)
                                {
                                    domain.PayResult = "重试:" + res.data.resMsg;
                                    domain.Status = 2;
                                    if (advance.transType != "RECHARGEBACK")
                                    {
                                        _paymentExtendRepository.SaveChange(domain);
                                    }
                                }
                                Console.WriteLine($@"集中收款充值【{advance.transType}】重试机制：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(res));
                                _logger.LogInformation($@"集中收款充值【{advance.transType}】重试机制：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(res));
                                return 1;
                            }
                            else
                            {
                                Console.WriteLine($@"集中收款充值【{advance.transType}】重试机制：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(res));
                                _logger.LogInformation($@"集中收款充值【{advance.transType}】重试机制：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(res));
                                await Task.Delay(60000);
                                continue;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($@"集中收款充值【{advance.transType}】重试机制异常：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(e));
                    _logger.LogInformation($@"集中收款充值【{advance.transType}】重试机制异常：" + $"【第{xx}次重试结果：】" + JsonConvert.SerializeObject(e));
                    await Task.Delay(60000);
                    continue;
                }
            }
            return 1;
        }
        [HttpPost]
        [Route("ChangeStatuss")]
        public int ChangeStatuss(PayReturnResults model)
        {
            _logger.LogInformation("集中支付前端调用改变状态：" + JsonConvert.SerializeObject(model));
            var data = _paymentExtendRepository.GetAsyncByRecordId(model.tradeNo).Result;
            if (data == null)
            {
                return 0;
            }
            data.Status = Convert.ToInt32(model.status);
            data.ModifiedDate = DateTime.Now;
            _paymentExtendRepository.Update(data);
            var Rdata = _paymentExtendRepository.UnitOfWork.SaveChangesAsync().Result;
            return 1;
        }

        [HttpPost]
        [Route("GetPaymentCallBack")]
        public int GetPaymentCallBack([FromForm]PayReturnResults model)
        {
            _logger.LogInformation("集中支付转账接口回调："+JsonConvert.SerializeObject(model));
            //金融 0：失败，1：成功 ，2：处理中
            //我方逻辑 0:未发起，1：成功，2：失败，3：交易中
            //金融回调只返回 0，1,2
            //处理中 直接返回1（发起支付后我方自动变成处理中）
            if (model.status == "2")
            {
                model.status = "3";
                var cdata = _paymentExtendRepository.GetAsync(model.tradeNo).Result;
                cdata.Status = Convert.ToInt32(model.status);
                _paymentExtendRepository.SaveChange(cdata);
                return 1;
            }
            if (model.status == "0")
            {
                model.status = "2";
            }
            
            var data = _paymentExtendRepository.GetAsync(model.tradeNo).Result;
            if (data == null)
            {
                return 0;
            }
            data.Status = Convert.ToInt32(model.status);
            data.PayResult = "结果："+ model.failureMsg + ",交易流水：" + model.tradeNo;
            data.ModifiedDate = DateTime.Now;
            _paymentExtendRepository.SaveChange(data);
            //推送OA
            var ApplyData = _context.ApplyDataDbSet.FromSqlInterpolated($@"
            -- 申请 用这个sql
            SELECT Concat(fe.NumericalOrder) NumericalOrder,CONCAT(st.`MenuID`) MenuID,hr.`Name`,CONCAT(hr.Bo_Id) PersonID,CONCAT(fe.`OwnerID`) OwnerID,hr2.`Name` AS OwnerName,CONVERT(DATE_FORMAT(fe.`DataDate`,'%Y-%m-%d') USING utf8mb4) DataDate,Concat(fe.enterpriseid) EnterpriseId,be.EnterpriseName,st.`cText` FROM `nxin_qlw_business`.`biz_related` br
            INNER JOIN `qlw_nxin_com`.`fd_expense` fe ON fe.`NumericalOrder` = br.`ChildValue`
            LEFT JOIN `qlw_nxin_com`.`stmenu` st ON st.`MenuID` = fe.`ExpenseType`
            LEFT JOIN `nxin_qlw_business`.`hr_person` hr ON hr.`PersonID` = fe.`PersonID`
            LEFT JOIN `nxin_qlw_business`.`hr_person` hr2 ON hr2.`BO_ID` = fe.`OwnerID`
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` be ON be.enterpriseid = fe.enterpriseid
            WHERE br.parentvalue = {data.NumericalOrder}").SingleOrDefault();

            //非申请 不通知直接返回
            if (ApplyData == null)
            {
                _logger.LogInformation("集中支付转账接口回调：非申请 不通知直接返回");
                return 1;
            }
            string OAurl = _hostCongfiguration._rdUrl + "/api/BSNewsCenter/SendNeweCenter";
            string ZNTurl = _hostCongfiguration._rdUrl + "/api/BSNewsCenter/SendNotice";
            var oaList = new List<OASend>();
            var zntList = new List<NoticeInfo>();
            //付款成功 通知 成功的通知申请人
            //付款失败 通知 失败的通知付款人
            if (model.status != "1")
            {
                var ulist = new List<long>();
                //付款单数据
                var TData = _prodiver.GetDataAsync(Convert.ToInt64(data.NumericalOrder)).Result;
                var TExtend = _prodiver.GetExtend(data.NumericalOrder).Where(m=>m.TradeNo == data.TradeNo).FirstOrDefault();
                if (TData == null)
                {
                    _logger.LogInformation("集中支付转账接口回调：通知失败 当前付款单数据为空");
                    return 1;
                }
                oaList.Add(new()
                {
                    Type = (TData.SettleReceipType == "201611180104402202" ? 1612011058280000101 : 2205251638120000109),
                    BusinessID = Convert.ToInt64(data.NumericalOrder),
                    EnterpriseID = Convert.ToInt64(ApplyData.EnterpriseId),
                    NewsCenterAbstract = 201707220104402208L,
                    OwnerID = Convert.ToInt64(ApplyData.OwnerID),
                    Subject = $@"{TExtend.RecheckName}您好，您于{TData.DataDate}申请的{TData.EnterpriseName}【{ (TData.SettleReceipType == "201611180104402202" ? "付款单" : "付款汇总单据")}；单据号：{TData.Number}】付款失败，失败原因是{model.failureMsg}。",
                    UserIds = TExtend.RecheckId
                });
                ulist.Add(Convert.ToInt64(TExtend.RecheckId));
                zntList.Add(new()
                {
                    MsgType = MessageType.html,
                    UserId = ulist,
                    Title = "支付消息",
                    AccontId = Convert.ToInt64(_hostCongfiguration.NoticeAccountID),//测试218 正式 217
                    Content = $@"{TExtend.RecheckName}您好，您于{ApplyData.DataDate}申请的{ApplyData.EnterpriseName}【{ (TData.SettleReceipType == "201611180104402202" ? "付款单" : "付款汇总单据")}；单据号：{TData.Number}】付款失败，失败原因是{model.failureMsg}。",
                    MsgSendDescribe = "支付消息",
                    //Url = _hostCongfiguration.qlwMobileUrl + "FMAudit/TC_Order?id= " + model.NumericalOrder
                });
            }
            else
            {
                OASend tsend = new()
                {
                    Type = Convert.ToInt64(ApplyData.MenuID),
                    BusinessID = Convert.ToInt64(data.NumericalOrder),
                    EnterpriseID = Convert.ToInt64(ApplyData.EnterpriseId),
                    NewsCenterAbstract = 201707220104402208L,
                    OwnerID = Convert.ToInt64(ApplyData.OwnerID),
                    Subject = $@"{ApplyData.OwnerName}您好，您于{ApplyData.DataDate}申请的{ApplyData.EnterpriseName}【{ApplyData.cText}；申请编号：{ApplyData.NumericalOrder}】付款成功。",
                    UserIds = ApplyData.OwnerID
                };
                oaList.Add(tsend);
                var ulist = new List<long>();
                ulist.Add(Convert.ToInt64(ApplyData.OwnerID));
                NoticeInfo tnotice = new()
                {
                    MsgType = MessageType.html,
                    UserId = ulist,
                    Title = "支付消息",
                    AccontId = Convert.ToInt64(_hostCongfiguration.NoticeAccountID),//测试218 正式 217
                    Content = $@"{ApplyData.OwnerName}您好，您于{ApplyData.DataDate}申请的{ApplyData.EnterpriseName}【{ApplyData.cText}；申请编号：{ApplyData.NumericalOrder}】付款成功。",
                    MsgSendDescribe = "支付消息",
                    //Url = _hostCongfiguration.qlwMobileUrl + "FMAudit/TC_Order?id= " + model.NumericalOrder

                };
                zntList.Add(tnotice);

                if (ApplyData.Name != ApplyData.OwnerName)
                {
                    tsend = new()
                    {
                        Type = Convert.ToInt64(ApplyData.MenuID),
                        BusinessID = Convert.ToInt64(data.NumericalOrder),
                        EnterpriseID = Convert.ToInt64(ApplyData.EnterpriseId),
                        NewsCenterAbstract = 201707220104402208L,
                        OwnerID = Convert.ToInt64(ApplyData.PersonID),
                        Subject = $@"{ApplyData.Name}您好，您于{ApplyData.DataDate}申请的{ApplyData.EnterpriseName}【{ApplyData.cText}；申请编号：{ApplyData.NumericalOrder}】付款成功。",
                        UserIds = ApplyData.OwnerID
                    };
                    oaList.Add(tsend);
                    ulist = new List<long>();
                    ulist.Add(Convert.ToInt64(ApplyData.PersonID));
                    tnotice = new()
                    {
                        MsgType = MessageType.html,
                        UserId = ulist,
                        Title = "支付消息",
                        AccontId = Convert.ToInt64(_hostCongfiguration.NoticeAccountID),//测试218 正式 217
                        Content = $@"{ApplyData.Name}您好，您于{ApplyData.DataDate}申请的{ApplyData.EnterpriseName}【{ApplyData.cText}；申请编号：{ApplyData.NumericalOrder}】付款成功。",
                        MsgSendDescribe = "MsgSendDescribe",
                        //Url = _hostCongfiguration.qlwMobileUrl + "FMAudit/TC_Order?id= " + model.NumericalOrder

                    };
                    zntList.Add(tnotice);
                }
            }

            foreach (var item in oaList)
            {
                var oaresult = _httpClientUtil1.PostJsonAsync<dynamic, dynamic>(OAurl, item).Result;
            }
            foreach (var item in zntList)
            {
                var zntresult = _httpClientUtil1.PostJsonAsync<dynamic, dynamic>(ZNTurl, item).Result;
            }
            return 1;
        }
        /// <summary>
        /// 自动化任务 金融推送消息 符合条件自动 生成收款单
        /// </summary>
        [HttpPost]
        [Route("AutoWrite")]

        public Result AutoWrite(QueueMsg msg)
        {
            string message = "";
            if (msg != null)
            {
                message = msg.message;
            }
            _logger.LogInformation("银行流水消息队列："+JsonConvert.SerializeObject(message));
            if (string.IsNullOrEmpty(message))
            {
                return new Result() { code = ErrorCode.NoContent.GetIntValue(), msg = "无数据" };
                //receiveContent = "{\"sysnType\":\"bank_receipts\",\"data\":[{\"transIndex\":\"d54306166ad811edbb85fa8df5b38500\",\"bankSerial\":\"2211221703090000150\",\"amount\":100,\"receiveDay\":\"2022/11/23 16:16:52\",\"entId\":\"634086739144001721\",\"entName\":\"北京助农\",\"acctIndex\":\"2d7db62d6ad911edbb85fa8df5b38500\",\"acctNo\":\"测试账户\",\"otherSideName\":\"测试对方名称\",\"otherSideAcctIndex\":\"4bd4793c6ad911edbb85fa8df5b38500\",\"otherSideAcct\":\"测试对方账户\",\"fee\":10,\"msgCode\":\"\",\"msg\":\"\",\"custList\":[{\"custId\":\"123444123\",\"custName\":\"测试客户名称\",\"marketId\":\"65746187465\",\"marketName\":\"测试部门名称\",\"boId\":\"25788\",\"userName\":\"李明测试\"}]}]}";
            }
            var list = JsonConvert.DeserializeObject<BankReceivableData>(message);
            //回单队列
            if (list.sysnType == "receipt")
            {
                _logger.LogInformation("银行回单消息队列："+JsonConvert.SerializeObject(message));
                try
                {
                    int i = 0;
                    foreach (var item in list.data)
                    {
                        var redata = _ifd_Bankreceivable.GetDataByIndex(item.transIndex);
                        if (redata != null)
                        {
                            redata.Remarks = item.receiptUrl;
                            _ifd_Bankreceivable.Update(redata);
                            i++;
                        }
                    }
                    if (i > 0)
                    {
                        var resIndex = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                        if (resIndex > 0)
                        {
                            return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "保存成功" };
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("回单异常："+JsonConvert.SerializeObject(e));
                    return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), msg = "保存失败", data = e };
                }
                return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "保存成功" };
            }
            //银行流水
            foreach (var item in list.data)
            {
                if (item.custList == null)
                {
                    item.custList = new List<CustListItem>();
                }
                fd_bankreceivable data = new fd_bankreceivable()
                {
                    NumericalOrder = _numericalOrderCreator.CreateAsync().Result,
                    CreateTime = DateTime.Now,
                    acctIndex = item.acctIndex,
                    entId = item.entId,
                    entName = item.entName,
                    acctNo = item.acctNo,
                    amount = item.amount,
                    bankSerial = item.bankSerial,
                    custList = JsonConvert.SerializeObject(item.custList == null ? "" : item.custList),
                    dataSource = item.dataSource,
                    fee = item.fee,
                    IsGenerate = item.custList.Count == 1 ? 3 : 2,
                    SourceNum = item.custList.Count == 1 ? _numericalOrderCreator.CreateAsync().Result : "0",
                    msg = item.msg,
                    msgCode = item.msgCode,
                    otherSideAcct = item.otherSideAcct,
                    otherSideAcctIndex = item.otherSideAcctIndex,
                    otherSideName = item.otherSideName,
                    receiveDay = item.receiveDay,
                    transIndex = item.transIndex
                };
                if (_ifd_Bankreceivable.GetDataByIndex(item.transIndex) == null)
                {
                    _ifd_Bankreceivable.Add(data);
                    var r = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                }
                else
                {
                    continue;
                }
                if (data.SourceNum != "0")
                {
                    try
                    {
                        var arryMarket = new List<string>();
                        arryMarket.Add(item.custList.FirstOrDefault().marketId);
                        var requestresult = _httpClientUtil1.PostJsonAsync<object>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.bsorg.QueryOrgInfoByMarketID/2.0", arryMarket).Result;
                        OrgResult orgInfo = JsonConvert.DeserializeObject<OrgResult>(JsonConvert.SerializeObject(requestresult));
                        EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                        //收款设置
                        var set = _ReceivablessetRepository.GetDataByEnterpriseId(data.entId);
                        if (set == null)
                        {
                            _logger.LogInformation("收款设置获取为空："+JsonConvert.SerializeObject(data));
                            data.IsGenerate = 2;
                            data.Remarks = "收款设置获取为空";
                            data.SourceNum = "0";
                            _ifd_Bankreceivable.Update(data);
                            var rr = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                            continue;
                        }
                        //资金账户
                        var accounts = _AccountODataProvider.GetDataByEnterpriseId(Convert.ToInt64(data.entId));
                        foreach (var aitem in accounts)
                        {
                            aitem.AccountNumber = encryptAccount.AccountNumberDecrypt(aitem.AccountNumber).Item2;
                        }
                        var account = accounts.Where(m => m.AccountNumber == data.acctNo).FirstOrDefault();
                        if (account == null)
                        {
                            _logger.LogInformation("资金账户获取为空：" + JsonConvert.SerializeObject(data));
                            data.IsGenerate = 2;
                            data.Remarks = "资金账户获取为空";
                            data.SourceNum = "0";
                            _ifd_Bankreceivable.Update(data);
                            var rr = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                            continue;
                        }
                        //资金账户详情
                        var accountDetail = _AccountODataProvider.GetDataByAutoWrite(Convert.ToInt64(account.AccountID));
                        if (accountDetail == null)
                        {
                            _logger.LogInformation("资金账户详情获取为空：" + JsonConvert.SerializeObject(data));
                            data.IsGenerate = 2;
                            data.Remarks = "资金账户详情获取为空";
                            data.SourceNum = "0";
                            _ifd_Bankreceivable.Update(data);
                            var rr = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                            continue;
                        }
                        var enterInfo = _provider.GetBiz_EnterpriseInfos(data.entId);
                        var period = _enterprisePeriodUtil.GetEnterperisePeriod(data.entId, item.receiveDay.ToString("yyyy-MM-dd"));
                        FD_ReceivablesAddCommand request = new FD_ReceivablesAddCommand()
                        {
                            NumericalOrder = data.SourceNum,
                            AppId = "1611231950150000101",
                            BusinessType = "201611180104402201",
                            EnterpriseID = data.entId,
                            DataDate = item.receiveDay,
                            BeginDate = period.StartDate.ToString("yyyy-MM-dd"),
                            EndDate = period.EndDate.ToString("yyyy-MM-dd"),
                            TicketedPointID = set.TicketedPointID,
                            DebitAccoSubjectID = set.CreditAccoSubjectID,
                            details = new List<FD_PaymentReceivablesDetailCommand>(),
                            extend = new List<FD_PaymentReceivables.FD_PaymentExtend>(),
                            Remarks = "银行流水自动生成",
                            OwnerID = set.OwnerID,
                            SettleReceipType = "201611180104402201",
                            GroupId = enterInfo.Pid,
                            
                            
                        };
                        request.details.Add(new FD_PaymentReceivablesDetailCommand()
                        {
                            NumericalOrder = data.SourceNum,
                            AccoSubjectID = account.AccoSubjectID,
                            PaymentTypeID = string.IsNullOrEmpty(accountDetail.PaymentTypeID) ? "0" : accountDetail.PaymentTypeID,
                            AccountID = account.AccountID,
                            Amount = data.amount,
                            Charges = data.fee,
                            CostAccoSubjectID = data.fee == 0 ? "" : set.CostAccoSubjectID,
                            OrganizationSortID = orgInfo != null ? (orgInfo.msg == "获取数据成功" ? orgInfo.data.FirstOrDefault().SortId : "0") : "0",
                            BusinessType = "201611160104402101",//默认客户
                            Content = set.Content+$@"({item.custList.FirstOrDefault().custName})",
                            ReceiptAbstractID = set.ReceiptAbstractID,
                            EnterpriseID = data.entId,
                            MarketID = item.custList.FirstOrDefault().marketId,
                            CustomerID = item.custList.FirstOrDefault().custId,
                            PersonID = "0",
                            ProjectID = "0",
                            ProductID = "0",
                        });
                        request.extend.Add(new FD_PaymentReceivables.FD_PaymentExtend()
                        {
                            NumericalOrder = data.SourceNum,
                            CollectionId = item.custList.FirstOrDefault().custId,
                            BankAccount = item.otherSideAcct,
                            Amount = item.amount + item.fee,
                        });
                        foreach (var ritem in request.extend)
                        {
                            var resultRece = encryptAccount.AccountNumberEncrypt(ritem.BankAccount);
                            ritem.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : ritem.BankAccount;
                        }
                        _logger.LogInformation("银行流水自动生成收款调用前："+JsonConvert.SerializeObject(request));
                        var sendResult = _mediator.Send(request, HttpContext.RequestAborted).Result;
                        if (sendResult.msg?.IndexOf("保存异常") > 0)
                        {
                            AutoFail(data);
                        }
                        continue;
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("银行流水自动生成收款异常："+JsonConvert.SerializeObject(e));
                        AutoFail(data);
                        continue;
                    }
                }
            }
            return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "保存成功" };
        }
        /// <summary>
        /// 失败方法
        /// </summary>
        /// <param name="data"></param>
        private void AutoFail(fd_bankreceivable data)
        {
            data.IsGenerate = 2;
            data.Remarks = "自动化生成失败";
            _ifd_Bankreceivable.Update(data);
            var rr = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
            _logger.LogInformation("自动化生成失败：" + JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 银行生成收款单
        /// </summary>
        /// <param name="list"></param>
        [HttpPost]
        [Route("AoutReceivables")]
        public Result AoutReceivables(List<fd_bankreceivable> list)
        {
            if (list != null)
            {
                var sourNum = _numericalOrderCreator.CreateAsync().Result;
                FD_ReceivablesAddCommand request = new FD_ReceivablesAddCommand();
                foreach (var item in list)
                {
                    var custom = JsonConvert.DeserializeObject<List<CustListItem>>(item.custList);
                    item.SourceNum = sourNum;
                    item.IsGenerate = 1;
                    _ifd_Bankreceivable.Update(item);
                    {
                        EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
                        //收款设置
                        var set = _ReceivablessetRepository.GetDataByEnterpriseId(item.entId);
                        //资金账户
                        var account = _fD_AccountRepository.GetAccountByAccountNumber(encryptAccount.AccountNumberEncrypt(item.acctNo).Item2);
                        //资金账户详情
                        var accountDetail = _AccountODataProvider.GetDataByAutoWrite(Convert.ToInt64(account.AccountID));
                        request = new FD_ReceivablesAddCommand()
                        {
                            NumericalOrder = item.SourceNum,
                            AppId = "1611231950150000101",
                            BusinessType = "201611180104402201",
                            EnterpriseID = item.entId,
                            DataDate = DateTime.Now,
                            TicketedPointID = set.TicketedPointID,
                            DebitAccoSubjectID = account.AccoSubjectID,
                            details = new List<FD_PaymentReceivablesDetailCommand>(),
                            extend = new List<FD_PaymentReceivables.FD_PaymentExtend>(),
                            Remarks = "银行流水手动生成收款单",
                        };
                        request.details.Add(new FD_PaymentReceivablesDetailCommand()
                        {
                            NumericalOrder = item.SourceNum,
                            AccoSubjectID = set.CreditAccoSubjectID,
                            PaymentTypeID = accountDetail.PaymentTypeID,
                            AccountID = account.AccountID,
                            Amount = item.amount,
                            Charges = item.fee,
                            CostAccoSubjectID = set.CostAccoSubjectID,
                            OrganizationSortID = "0",
                            BusinessType = "201611160104402101",//默认客户
                            Content = set.Content,
                            ReceiptAbstractID = set.ReceiptAbstractID,
                            EnterpriseID = item.entId,
                            MarketID = "0",
                            PersonID = "0",
                            ProjectID = "0",
                            ProductID = "0",
                        });
                        request.extend.Add(new FD_PaymentReceivables.FD_PaymentExtend()
                        {
                            NumericalOrder = item.SourceNum,
                            CollectionId = custom.FirstOrDefault().custId,
                            BankAccount = item.otherSideAcct,
                            Amount = (decimal)(item.amount + item.fee)
                        });
                        foreach (var ritem in request.extend)
                        {
                            var resultRece = encryptAccount.AccountNumberEncrypt(ritem.BankAccount);
                            ritem.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : ritem.BankAccount;
                        }
                    }
                    var rd = _mediator.Send(request, HttpContext.RequestAborted).Result;
                    if (rd.code == ErrorCode.Success.GetIntValue())
                    {
                        var r = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                    }
                    else
                    {
                        return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), msg = "生成失败" };
                    }
                }
                return new Result() { code = ErrorCode.Success.GetIntValue(),msg = "生成成功" };
            }
            else
            {
                return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), msg = "生成失败" };
            }
        }
        /// <summary>
        /// 银行生成收款汇总单
        /// </summary>
        /// <param name="list"></param>
        [HttpPost]
        [Route("AoutReceivablesSummary")]
        public Result AoutReceivablesSummary(List<fd_bankreceivable> list)
        {
            if (list != null)
            {
                if (list.Count == 0)
                {
                    return new Result() { code = ErrorCode.NoContent.GetIntValue(),msg = "无数据" };
                }
            }
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            var group = list.GroupBy(m => m.entId).ToList();
            foreach (var item in group)
            {
                //收款设置
                var set = _ReceivablessetRepository.GetDataByEnterpriseId(item.FirstOrDefault().entId);
                var sourceNum = _numericalOrderCreator.CreateAsync().Result;
                FD_ReceivablesSummaryAddCommand request = new FD_ReceivablesSummaryAddCommand()
                {
                    NumericalOrder = sourceNum,
                    AppId = "1611231950150000101",
                    BusinessType = "201611160104402101",
                    EnterpriseID = item.FirstOrDefault().entId,
                    DataDate = DateTime.Now,
                    TicketedPointID = set.TicketedPointID,
                    details = new List<SummaryData>(),
                    extend = new List<SummaryData>(),
                    Remarks = "银行流水手动生成收款汇总单",
                    SettleReceipType = "201611180104402203",
                };
                foreach (var items in item)
                {
                    items.SourceNum = sourceNum;
                    items.IsGenerate = 1;
                    //资金账户
                    var account = _fD_AccountRepository.GetAccountByAccountNumber(encryptAccount.AccountNumberEncrypt(items.acctNo).Item2);
                    //资金账户详情
                    var accountDetail = _AccountODataProvider.GetDataByAutoWrite(Convert.ToInt64(account.AccountID));
                    
                    request.details.Add(new SummaryData()
                    {
                        NumericalOrder = items.SourceNum,
                        AccountID = account.AccountID,
                        EnterpriseID = items.entId,
                        Amount = items.amount,
                        BusinessType = request.BusinessType,
                        Charges = items.fee,
                        Content = set.Content,
                        CustomerID = "0",
                        MarketID = "0",
                        PaymentTypeID = accountDetail.PaymentTypeID,
                        PersonID = "0",
                        ReceiptAbstractID = set.ReceiptAbstractID,
                        OrganizationSortID = "0",
                        AccoSubjectID = account.AccoSubjectID,
                        ProductID = "0",
                        AccoSubjectCode = "0",
                        ProjectID = "0",
                        //CollectionId = items.oth

                    });
                    request.extend.Add(new SummaryData()
                    {
                        //NumericalOrder = items.SourceNum,
                        //Amount = items.amount,
                        //CollectionId = o.CollectionId,
                        //PersonId = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        //AccoSubjectID = o.AccoSubjectID,
                        //AccoSubjectCode = o.AccoSubjectCode,
                        //ProductID = o.ProductID,
                        //ProjectID = o.ProjectID,
                        //OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        //AccountName = o.AccountName,
                        //BankAccount = o.BankAccount,
                        //BankDeposit = o.BankDeposit,
                        //Charges = o.Charges
                    });
                    foreach (var ritem in request.extend)
                    {
                        var resultRece = encryptAccount.AccountNumberEncrypt(ritem.BankAccount);
                        ritem.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : ritem.BankAccount;
                    }
                }
            }


            if (list != null)
            {
                var sourNum = _numericalOrderCreator.CreateAsync().Result;
                FD_ReceivablesSummaryAddCommand request = new FD_ReceivablesSummaryAddCommand();
                foreach (var item in list)
                {
                    item.SourceNum = sourNum;
                    _ifd_Bankreceivable.Update(item);
                    {
                        
                        request = new FD_ReceivablesSummaryAddCommand()
                        {
                            NumericalOrder = item.SourceNum,
                            AppId = "1611231950150000101",
                            BusinessType = "201611160104402101",
                            EnterpriseID = item.entId,
                            DataDate = DateTime.Now,
                            //TicketedPointID = set.TicketedPointID,
                            //DebitAccoSubjectID = account.AccoSubjectID,
                            details = new List<SummaryData>(),
                            extend = new List<SummaryData>(),
                            Remarks = "银行流水手动生成收款汇总单",
                            SettleReceipType = "201611180104402203",
                        };
                        request.details.Add(new SummaryData() 
                        {
                            
                        });
                        request.extend.Add(new SummaryData() 
                        {
                        
                        });
                        foreach (var ritem in request.extend)
                        {
                            var resultRece = encryptAccount.AccountNumberEncrypt(ritem.BankAccount);
                            ritem.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : ritem.BankAccount;
                        }
                    }
                }
                var rd = _mediator.Send(request, HttpContext.RequestAborted).Result;
                if (rd.code == ErrorCode.Success.GetIntValue())
                {
                    var r = _ifd_Bankreceivable.UnitOfWork.SaveChangesAsync().Result;
                }
                else
                {
                    return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), msg = "生成失败" };
                }
                return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "生成成功" };
            }
            else
            {
                return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), msg = "生成失败" };
            }
        }
        [NonAction]
        public Tuple<bool, string> AccountNumberDecrypt(string AccountNumber)
        {
            var spubKeyPath = _hostCongfiguration.OAToBanking;
            var sCertificatePass = _hostCongfiguration.OAToBankingPass;
            Tuple<bool, string> result = new Tuple<bool, string>(false, "");
            if (string.IsNullOrEmpty(AccountNumber)) return result = new Tuple<bool, string>(false, "账号空!");
            if (string.IsNullOrEmpty(spubKeyPath) || string.IsNullOrEmpty(sCertificatePass))
            {
                LogHelper.LogWarning("FMCommonServices/AccountNumberDecrypt:参数：OAToBanking=" + spubKeyPath + ";OAToBankingPass=" + sCertificatePass);
                return result = new Tuple<bool, string>(false, "密钥空!");
            }
            if (AccountNumber.Length <= 20)
            {
                return result = new Tuple<bool, string>(false, "请核实银行账号!");
            }
            var accountNumberStr = "";
            try
            {
                accountNumberStr = EncryptAccount.getFieldDecrypt(AccountNumber, spubKeyPath, sCertificatePass);
                if (string.IsNullOrEmpty(accountNumberStr)) { accountNumberStr = AccountNumber; }
                return result = new Tuple<bool, string>(true, accountNumberStr);
            }
            catch (Exception ex)
            {
                LogHelper.LogWarning(string.Format("FMCommonServices/AccountNumberDecrypt 解密账号:[{0}],[{1}],[{2}],[{3}],[{4}]", AccountNumber, accountNumberStr, spubKeyPath, sCertificatePass, ex));
                accountNumberStr = string.Empty;
            }
            return result = new Tuple<bool, string>(false, accountNumberStr);
        }
        //旧数据兼容
        [HttpGet]
        public string CcompatibleOldData(string EnterpriseId,string BeginDate,string EndDate)
        {
            //201611180104402201  收款
            //201611180104402202  付款
            //201611180104402203  汇总收款
            //201611180104402204  汇总付款

            //获取收付款单数据
            var list = _paymentReceivablesRepository.GetList(EnterpriseId, BeginDate, EndDate);
            if (list == null)
            {
                return "没有数据";
            }

            //获取收付款详情数据
            var listDetails = _paymentReceivablesDetailRepository.GetNums(string.Join(',', list.Select(m => m.NumericalOrder)));
            //获取付款凭证扩展信息
            var settleExtends = _settlereceiptextendRepository.GetNums(string.Join(',', list.Select(m => m.NumericalOrder)));
            //获取会计凭证数据
            var settleList = _settleReceiptRepository.GetByNums(string.Join(',', list.Select(m => m.NumericalOrder)));
            //获取会计凭证详情数据
            var settleDetailList = _settleReceiptDetailRepository.GetByNums(string.Join(',', list.Select(m => m.NumericalOrder)));
            //获取付款单数据
            var payList = list.Where(x => x.SettleReceipType == "201611180104402202").ToList();
            //获取付款汇总单数据
            var paySummaryList = list.Where(x => x.SettleReceipType == "201611180104402204").ToList();
            //获取收款单数据
            var receList = list.Where(x => x.SettleReceipType == "201611180104402201").ToList();
            //获取收款汇总单数据
            var receSummaryList = list.Where(x => x.SettleReceipType == "201611180104402203").ToList();
            #region 判空处理
            if (payList == null)
            {
                payList = new List<Domain.FD_PaymentReceivables>();
            }
            if (paySummaryList == null)
            {
                paySummaryList = new List<Domain.FD_PaymentReceivables>();
            }
            if (receList == null)
            {
                receList = new List<Domain.FD_PaymentReceivables>();
            }
            if (receSummaryList == null)
            {
                receSummaryList = new List<Domain.FD_PaymentReceivables>();
            }
            #endregion
            #region 付款数据处理
            foreach ( var pay in payList )
            {
                var temp = settleList.Where(m=>m.NumericalOrder == pay.NumericalOrder).FirstOrDefault();
                var tempDetail = settleDetailList.Where(m => m.NumericalOrder == pay.NumericalOrder).ToList();
                var payDetail = listDetails.Where(m => m.NumericalOrder == pay.NumericalOrder).FirstOrDefault();
                var settleExtend = settleExtends.Where(m => m.NumericalOrder == pay.NumericalOrder).FirstOrDefault();
                var collectionId = "";
                Validation(ref temp, ref tempDetail, ref payDetail, ref settleExtend);
                //客户/供应商
                if (payDetail.BusinessType == "201611160104402101" || payDetail.BusinessType == "201611160104402104")
                {
                    collectionId = payDetail.CustomerID;
                }
                //部门
                else if (payDetail.BusinessType == "201611160104402102")
                {
                    collectionId = payDetail.MarketID;
                }
                //员工
                else if(payDetail.BusinessType == "201611160104402103")
                {
                    collectionId = payDetail.PersonID;
                }
                //供应商
                else if (payDetail.BusinessType == "201611160104402105")
                {
                    collectionId = payDetail.EnterpriseID;
                }

                if (temp != null)
                {
                    _paymentExtendRepository.Add(new Domain.FD_PaymentExtend() 
                    {
                        RecheckId = "0",
                        IsRecheck = false,
                        Status = 0,
                        NumericalOrder = pay.NumericalOrder,
                        CollectionId = collectionId,
                        PersonId = collectionId,//老版本没有收款人
                        AccountName = settleExtend.AccountName,
                        BankDeposit = settleExtend.DepositBank,
                        BankAccount = settleExtend.AccountNumber,
                        Amount = (decimal)payDetail.Amount,
                        Charges = payDetail.Charges,
                        Guid = payDetail.Guid,
                    });
                    _paymentreceivablesvoucherRepository.Add(new fd_paymentreceivablesvoucher()
                    {
                        AccountNo = temp.AccountNo,
                        AttachmentNum = temp.AttachmentNum,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        DataDate = temp.DataDate,
                        EnterpriseID = temp.EnterpriseID,
                        Guid = temp.Guid,
                        Number = temp.Number,
                        NumericalOrder = temp.NumericalOrder,
                        OwnerID = temp.OwnerID,
                        Remarks = temp.Remarks,
                        SettleReceipType = temp.SettleReceipType,
                        TicketedPointID = temp.TicketedPointID,
                    });
                    foreach (var detail in tempDetail)
                    {
                        _paymentreceivablesvoucherDetailRepository.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            AccoSubjectCode = detail.AccoSubjectCode,
                            AccoSubjectID = detail.AccoSubjectID,
                            EnterpriseID = detail.EnterpriseID,
                            AccountID = detail.AccountID,
                            AgingDate = detail.AgingDate,
                            ClassificationID = detail.ClassificationID,
                            Content = detail.Content,
                            Credit = detail.Credit,
                            CustomerID =  detail.CustomerID,
                            Debit = detail.Debit,
                            Guid = detail.Guid,
                            IsCharges = detail.IsCharges,
                            LorR = detail.LorR,
                            MarketID = detail.MarketID,
                            NumericalOrder = detail.NumericalOrder,
                            OrganizationSortID = detail.OrganizationSortID,
                            PaymentTypeID = detail.PaymentTypeID,
                            PersonID = detail.PersonID,
                            ProductGroupID = detail.ProductGroupID,
                            ProductID = detail.ProductID,
                            ProjectID = detail.ProjectID,
                            ReceiptAbstractID = detail.ReceiptAbstractID,
                            RowNum = detail.RowNum,
                        });
                    }
                }
            }
            #endregion
            #region 收款数据处理
            foreach (var item in receList)
            {

                var temp = settleList.Where(m => m.NumericalOrder == item.NumericalOrder).FirstOrDefault();
                var tempDetail = settleDetailList.Where(m => m.NumericalOrder == item.NumericalOrder).ToList();
                var payDetail = listDetails.Where(m => m.NumericalOrder == item.NumericalOrder).FirstOrDefault();
                var settleExtend = settleExtends.Where(m => m.NumericalOrder == item.NumericalOrder).FirstOrDefault();
                var collectionId = "";
                Validation(ref temp, ref tempDetail, ref payDetail, ref settleExtend);
                //客户/供应商
                if (payDetail.BusinessType == "201611160104402101" || payDetail.BusinessType == "201611160104402104")
                {
                    collectionId = payDetail.CustomerID;
                }
                //部门
                else if (payDetail.BusinessType == "201611160104402102")
                {
                    collectionId = payDetail.MarketID;
                }
                //员工
                else if (payDetail.BusinessType == "201611160104402103")
                {
                    collectionId = payDetail.PersonID;
                }
                //供应商
                else if (payDetail.BusinessType == "201611160104402105")
                {
                    collectionId = payDetail.EnterpriseID;
                }

                if (temp != null)
                {
                    _paymentExtendRepository.Add(new Domain.FD_PaymentExtend()
                    {
                        RecheckId = "0",
                        IsRecheck = false,
                        Status = 0,
                        NumericalOrder = item.NumericalOrder,
                        CollectionId = collectionId,
                        PersonId = collectionId,//老版本没有收款人
                        AccountName = settleExtend.AccountName,
                        BankDeposit = settleExtend.DepositBank,
                        BankAccount = settleExtend.AccountNumber,
                        Amount = (decimal)payDetail.Amount,
                        Charges = payDetail.Charges,
                        Guid = payDetail.Guid,
                    });
                    _paymentreceivablesvoucherRepository.Add(new fd_paymentreceivablesvoucher()
                    {
                        AccountNo = temp.AccountNo,
                        AttachmentNum = temp.AttachmentNum,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        DataDate = temp.DataDate,
                        EnterpriseID = temp.EnterpriseID,
                        Guid = temp.Guid,
                        Number = temp.Number,
                        NumericalOrder = temp.NumericalOrder,
                        OwnerID = temp.OwnerID,
                        Remarks = temp.Remarks,
                        SettleReceipType = temp.SettleReceipType,
                        TicketedPointID = temp.TicketedPointID,
                    });
                    foreach (var detail in tempDetail)
                    {
                        _paymentreceivablesvoucherDetailRepository.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            AccoSubjectCode = detail.AccoSubjectCode,
                            AccoSubjectID = detail.AccoSubjectID,
                            EnterpriseID = detail.EnterpriseID,
                            AccountID = detail.AccountID,
                            AgingDate = detail.AgingDate,
                            ClassificationID = detail.ClassificationID,
                            Content = detail.Content,
                            Credit = detail.Credit,
                            CustomerID = detail.CustomerID,
                            Debit = detail.Debit,
                            Guid = detail.Guid,
                            IsCharges = detail.IsCharges,
                            LorR = detail.LorR,
                            MarketID = detail.MarketID,
                            NumericalOrder = detail.NumericalOrder,
                            OrganizationSortID = detail.OrganizationSortID,
                            PaymentTypeID = detail.PaymentTypeID,
                            PersonID = detail.PersonID,
                            ProductGroupID = detail.ProductGroupID,
                            ProductID = detail.ProductID,
                            ProjectID = detail.ProjectID,
                            ReceiptAbstractID = detail.ReceiptAbstractID,
                            RowNum = detail.RowNum,
                        });
                    }
                }
            }
            #endregion
            #region 付款汇总数据处理
            foreach (var item in paySummaryList)
            {
                var temp = settleList.Where(m => m.NumericalOrder == item.NumericalOrder).FirstOrDefault();
                var tempDetail = settleDetailList.Where(m => m.NumericalOrder == item.NumericalOrder).ToList();
                var payDetail = listDetails.Where(m => m.NumericalOrder == item.NumericalOrder).ToList();

                foreach (var pd in payDetail)
                {
                    string collectionId = "";
                    //客户/供应商
                    if (pd.BusinessType == "201611160104402101" || pd.BusinessType == "201611160104402104")
                    {
                        collectionId = pd.CustomerID;
                    }
                    //部门
                    else if (pd.BusinessType == "201611160104402102")
                    {
                        collectionId = pd.MarketID;
                    }
                    //员工
                    else if (pd.BusinessType == "201611160104402103")
                    {
                        collectionId = pd.PersonID;
                    }
                    //供应商
                    else if (pd.BusinessType == "201611160104402105")
                    {
                        collectionId = pd.EnterpriseID;
                    }
                    _paymentExtendRepository.Add(new Domain.FD_PaymentExtend()
                    {
                        RecheckId = "0",
                        IsRecheck = false,
                        Status = 0,
                        NumericalOrder = pd.NumericalOrder,
                        CollectionId = collectionId,
                        PersonId = collectionId,//老版本没有收款人
                        //汇总没有银行信息
                        //AccountName = settleExtend.AccountName,
                        //BankDeposit = settleExtend.DepositBank,
                        //BankAccount = settleExtend.AccountNumber,
                        Amount = (decimal)pd.Amount,
                        Charges = pd.Charges,
                        Guid = pd.Guid,
                    });
                }

                if (temp != null)
                {
                    _paymentreceivablesvoucherRepository.Add(new fd_paymentreceivablesvoucher()
                    {
                        AccountNo = temp.AccountNo,
                        AttachmentNum = temp.AttachmentNum,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        DataDate = temp.DataDate,
                        EnterpriseID = temp.EnterpriseID,
                        Guid = temp.Guid,
                        Number = temp.Number,
                        NumericalOrder = temp.NumericalOrder,
                        OwnerID = temp.OwnerID,
                        Remarks = temp.Remarks,
                        SettleReceipType = temp.SettleReceipType,
                        TicketedPointID = temp.TicketedPointID,
                    });
                    foreach (var detail in tempDetail)
                    {
                        _paymentreceivablesvoucherDetailRepository.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            AccoSubjectCode = detail.AccoSubjectCode,
                            AccoSubjectID = detail.AccoSubjectID,
                            EnterpriseID = detail.EnterpriseID,
                            AccountID = detail.AccountID,
                            AgingDate = detail.AgingDate,
                            ClassificationID = detail.ClassificationID,
                            Content = detail.Content,
                            Credit = detail.Credit,
                            CustomerID = detail.CustomerID,
                            Debit = detail.Debit,
                            Guid = detail.Guid,
                            IsCharges = detail.IsCharges,
                            LorR = detail.LorR,
                            MarketID = detail.MarketID,
                            NumericalOrder = detail.NumericalOrder,
                            OrganizationSortID = detail.OrganizationSortID,
                            PaymentTypeID = detail.PaymentTypeID,
                            PersonID = detail.PersonID,
                            ProductGroupID = detail.ProductGroupID,
                            ProductID = detail.ProductID,
                            ProjectID = detail.ProjectID,
                            ReceiptAbstractID = detail.ReceiptAbstractID,
                            RowNum = detail.RowNum,
                        });
                    }
                }
            }
            #endregion
            return "";
        }
        [HttpGet]
        [Route("GetMaxNumberByDate")]
        /// <summary>
        /// 老版 付款单调用此接口 生成会计凭证单据号 保持统一 老版付款单单据号生成规则 作废
        /// </summary>
        /// <param name="SettleReceipType"></param>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="TicketPoint"></param>
        /// <returns></returns>
        public string GetMaxNumberByDate(string SettleReceipType, string EnterpriseID, string BeginDate, string EndDate, string TicketPoint)
        {
            return _provider.GetMaxNumberByDate(SettleReceipType, EnterpriseID, BeginDate, EndDate, TicketPoint).MaxNumber;
        }
        private static void Validation(ref Domain.FD_SettleReceipt temp, ref List<FD_SettleReceiptDetail> tempDetail, ref FD_PaymentReceivablesDetail payDetail, ref fd_settlereceiptextend settleExtend)
        {
            //增加防空判断
            if (temp == null)
            {
                temp = new Domain.FD_SettleReceipt();
            }
            if (tempDetail == null)
            {
                tempDetail = new List<FD_SettleReceiptDetail>();
            }
            if (payDetail == null)
            {
                payDetail = new FD_PaymentReceivablesDetail();
            }
            if (settleExtend == null)
            {
                settleExtend = new fd_settlereceiptextend();
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
