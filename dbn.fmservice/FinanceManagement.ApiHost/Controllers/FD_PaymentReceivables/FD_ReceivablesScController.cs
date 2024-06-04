using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Util;
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
using Microsoft.OData.Edm;
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
    /*商城大有谷推单 GK商城购物发货后生成*/
    [Route("api/[controller]")]
    [ApiController]
    public class FD_ReceivablesScController : ControllerBase
    {
        IMediator _mediator;
        //FD_PaymentReceivablesTODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;
        FD_PaymentReceivablesTODataProvider _prodiver;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FD_ReceivablesScController> _logger;
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
        public FD_ReceivablesScController(Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,IFD_settlereceiptextendRepository settlereceiptextendRepository, IFD_SettleReceiptRepository settleReceiptRepository,IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository paymentReceivablesRepository,IFD_PaymentReceivablesDetailRepository paymentReceivablesDetailRepository, FD_SettleReceiptNewODataProvider provider,NumericalOrderCreator numericalOrderCreator, FD_AccountODataProvider AccountODataProvider, IFD_AccountRepository fD_AccountRepository, Ifd_bankreceivableRepository ifd_Bankreceivable, Ifd_receivablessetRepository ReceivablessetRepository, IMediator mediator, HttpClientUtil httpClientUtil,QlwCrossDbContext context,
            //FD_PaymentReceivablesTODataProvider provider, 
            EnterprisePeriodUtil enterprisePeriodUtil,
            FundSummaryUtil fundSummaryUtil, IFD_PaymentExtendRepository paymentExtendRepository, IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, ILogger<FD_ReceivablesScController> logger, HostConfiguration hostCongfiguration)
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
        #region 默认科目账户等 作废
        /// <summary>
        /// 商城专用 凭证新增接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SettleReceipt.Add")]
        public Result SettleReceiptAdd(FD_SettleReceiptNewAddCommand param)
        {
            var res = new Result() { code = 1 };
            try
            {
                _logger.LogInformation("商城SettleReceiptAdd:param=" + JsonConvert.SerializeObject(param));
                if (param == null) { res.msg = "参数不能为空";  }
                if (string.IsNullOrEmpty(param.TemplateName))
                {
                    if (string.IsNullOrEmpty(param.SettleReceipType)) { res.msg += "SettleReceipType不能为空!"; }
                    if (string.IsNullOrEmpty(param.TicketedPointID)) { res.msg += "TicketedPointID不能为空!"; }
                }
                if (string.IsNullOrEmpty(param.EnterpriseID)) { res.msg += "EnterpriseID不能为空!";  }
                if (string.IsNullOrEmpty(param.DataDate)) { res.msg += "DataDate不能为空!";  }
                if (string.IsNullOrEmpty(param.OwnerID)) { res.msg += "OwnerID不能为空!";  }
                if (param.Lines == null || param.Lines.Count == 0) { res.msg += "Lines不能无数据!"; }
                var period = _enterprisePeriodUtil.GetEnterperisePeriod(param.EnterpriseID, param.DataDate);
                param.BeginDate = period.StartDate.ToString("yyyy-MM-dd");
                param.EndDate = period.EndDate.ToString("yyyy-MM-dd");
                for (var i=0;i<param?.Lines?.Count;i++)
                {
                    var detail=param.Lines[i];
                    var index = i + 1;
                    if (string.IsNullOrEmpty(param.TemplateName))
                    {
                        if (string.IsNullOrEmpty(detail.ReceiptAbstractID)) { res.msg += $"details的第{index}个ReceiptAbstractID不能为空!"; }
                        if (string.IsNullOrEmpty(detail.AccoSubjectID)) { res.msg += $"details的第{index}个AccoSubjectID不能为空"; }
                    }
                    if (string.IsNullOrEmpty(detail.PersonID))
                    {
                        detail.PersonID = "0";
                    }
                    //商城定制（因为他们拒绝调用接口查询 personid）
                    var hr = _context.HrInfoDataSet.FromSqlRaw($@"SELECT CONCAT(PersonID) PersonID,CONCAT(BO_ID) BO_ID ,Name FROM nxin_qlw_business.hr_person where bo_id = {detail.PersonID}").FirstOrDefault();
                    if (hr != null)
                    {
                        detail.PersonID = hr.PersonID;
                    }
                }
                if (!string.IsNullOrEmpty(res.msg)) { _logger.LogInformation($"商城SettleReceiptAdd:param={JsonConvert.SerializeObject(param)},验证未通过，返回：{JsonConvert.SerializeObject(res)}"); return res; }

                res = _mediator.Send(param, HttpContext.RequestAborted).Result;
                _logger.LogInformation($"商城SettleReceiptAdd:param={JsonConvert.SerializeObject(param)},返回：{JsonConvert.SerializeObject(res)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"商城AddSettleReceipt:ex={ex.ToString()};param={JsonConvert.SerializeObject(param)}");
            }

            return res;
        }



        /// <summary>
        /// 商城生成收款单
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("Receivables.Add")]
        public Result ReceivablesAdd(FD_ReceivablesAddCommand param)
        {
            var res = new Result() { code = 1 };
            try
            {
                _logger.LogInformation("商城ReceivablesAdd:param=" + JsonConvert.SerializeObject(param));
                if (param == null) { res.msg = "参数不能为空"; }
                if (string.IsNullOrEmpty(param.EnterpriseID)) { res.msg += "EnterpriseID不能为空!"; }
                if (string.IsNullOrEmpty(param.SettleReceipType)) { res.msg += "SettleReceipType不能为空!"; }
                if (param.DataDate == DateTime.MinValue) { res.msg += "DataDate不能为空!"; }
                if (string.IsNullOrEmpty(param.TicketedPointID)) { res.msg += "TicketedPointID不能为空!"; }
                if (string.IsNullOrEmpty(param.OwnerID)) { res.msg += "OwnerID不能为空!"; }
                if (param.details == null || param.details.Count == 0) { res.msg += "details不能无数据!"; }
                var period = _enterprisePeriodUtil.GetEnterperisePeriod(param.EnterpriseID, param.DataDate.ToString("yyyy-MM-dd"));
                param.BeginDate = period.StartDate.ToString("yyyy-MM-dd");
                param.EndDate = period.EndDate.ToString("yyyy-MM-dd");
                for (var i = 0; i < param?.details?.Count; i++)
                {
                    var detail = param.details[i];
                    var index = i + 1;
                    if (string.IsNullOrEmpty(detail.BusinessType)) { res.msg += $"details的第{index}个BusinessType不能为空!"; }
                    if (string.IsNullOrEmpty(detail.ReceiptAbstractID)) { res.msg += $"details的第{index}个ReceiptAbstractID不能为空!"; }
                    if (string.IsNullOrEmpty(detail.PaymentTypeID)) { res.msg += $"details的第{index}个PaymentTypeID不能为空"; }
                    if (string.IsNullOrEmpty(detail.AccountID)) { res.msg += $"details的第{index}个AccountID不能为空"; }
                    if (!string.IsNullOrEmpty(detail.PersonID))
                    {
                        //商场定制（因为他们拒绝调用接口查询 personid）
                        var hr = _context.HrInfoDataSet.FromSqlRaw($@"SELECT CONCAT(PersonID) PersonID,CONCAT(BO_ID) BO_ID ,Name FROM nxin_qlw_business.hr_person where bo_id = {detail.PersonID}").FirstOrDefault();
                        if (hr != null)
                        {
                            detail.PersonID = hr.PersonID;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(res.msg)) { _logger.LogInformation($"商城ReceivablesAdd:param={JsonConvert.SerializeObject(param)},验证未通过，返回：{JsonConvert.SerializeObject(res)}"); return res; }

                res = _mediator.Send(param, HttpContext.RequestAborted).Result;
                _logger.LogInformation($"商城ReceivablesAdd:param={JsonConvert.SerializeObject(param)},返回：{JsonConvert.SerializeObject(res)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"商城ReceivablesAdd:ex={ex.ToString()};param={JsonConvert.SerializeObject(param)}");
            }
            return res;
        }

        #endregion

        #region 默认科目账户等 作废
        ///// <summary>
        ///// 商城专用 凭证新增接口
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("AddSettleReceipt")]
        //public Result AddSettleReceipt([FromBody] FD_ScReceivable param)
        //{
        //    var res = new Result() { code = 1 };
        //    try
        //    {
        //        _logger.LogInformation("商城AddSettleReceipt:param=" + JsonConvert.SerializeObject(param));
        //        if (param == null) { res.msg = "参数不能为空"; return res; }
        //        if (string.IsNullOrEmpty(param.enterpriseId)) { res.msg = "enterpriseId不能为空"; return res; }

        //        //推送往来冲抵单，客户=DW，金额=礼品卡部分
        //        //（1）凭证类别 = 转账凭证
        //        //（2）单据字，对应单位默认第一个
        //        //（3）摘要：客户转账
        //        //（4）会计凭证：会计科目分录
        //        //借：应收账款 / 应收外部款 / 应收外部货款--DW
        //        //   贷：应收账款 / 应收外部款 / 应收外部货款-- GK(大有谷零售客户)   （红冲）
        //        //（5）推送数据提供：业务日期、客户ID、金额
        //        var enterpriseId = param.enterpriseId;
        //        var enData = _provider.GetBiz_EnterpriseInfos(enterpriseId);
        //        if (enData == null)
        //        {
        //            return new Result() { code = 1, msg = $"当前单位:{enterpriseId}在企联网中不存在" };
        //        }
        //        //日期
        //        var dataDate = DateTime.Now.ToString();
        //        var tickets = _provider.GetTicketPointByEnteId(enterpriseId);
        //        //单据字，对应单位默认第一个
        //        var ticketedPointID = tickets.Where(m => m.EnterpriseId == enterpriseId).FirstOrDefault()?.TicketedPointId;
        //        if (string.IsNullOrEmpty(ticketedPointID) || ticketedPointID == "0") { res.msg = "无单据字"; return res; }               

        //        //科目
        //        var accos = _provider.GetAccoSubjectInfos(dataDate, new EnterpriseInfo() { EnterpriseId = enterpriseId }, enData.Pid);
        //        //摘要
        //        var settles = _provider.GetSettleSummaryByDate(enData.Pid, enterpriseId, dataDate); ;

        //        //借：应收账款/应收外部款/应收外部货款--DW
        //        var dAccoSubjectCode = "11220301";
        //        var dAccoSubjectID = accos.Where(m => m.AccoSubjectCode == dAccoSubjectCode).FirstOrDefault()?.AccoSubjectId;
        //        //贷：应收账款/应收外部款/应收外部货款-- GK(大有谷零售客户)   （红冲）
        //        var cAccoSubjectCode = "11220301";
        //        var cAccoSubjectID = accos.Where(m => m.AccoSubjectCode == cAccoSubjectCode).FirstOrDefault()?.AccoSubjectId;
        //        //摘要：客户转账 030104
        //        var receiptAbstractID = settles.Where(m => m.Remarks == "030104" && m.EnterpriseId == enterpriseId).FirstOrDefault()?.SettleSummaryId;
        //        //资金账户
        //        var account = _AccountODataProvider.GetDataByEnterpriseId(Convert.ToInt64(enterpriseId)).FirstOrDefault();
        //        //var accountResult = encryptAccount.AccountNumberDecrypt(account.AccountNumber);
        //        //var accountNumber = accountResult?.Item1 == true ? accountResult.Item2 : "0";
        //        //if (string.IsNullOrEmpty(accountNumber))
        //        //{
        //        //    _logger.LogError($"商城AddReceivables:银行账号空:{JsonConvert.SerializeObject(account)},accountResult={JsonConvert.SerializeObject(accountResult)}");
        //        //}
        //        //结算方式
        //        var paymentTypeID = "201610140104402004";//网银 pid=201610140104402001

        //        //金额
        //        var amount = param.amount;
        //        //内容
        //        var content = "";
        //        //客户
        //        var customerId = param.customerId;
        //        //客户银行账户
        //        //会计期间
        //        var period = _enterprisePeriodUtil.GetEnterperisePeriod(enterpriseId, dataDate);
        //        //制单人
        //        var ownerID = "0";
        //        var numericalOrder = _numericalOrderCreator.CreateAsync().Result;
        //        //凭证类别:转账凭证
        //        var settleReceipType = "201610220104402203";
        //        var request = new FD_SettleReceiptNewAddCommand()
        //        {
        //            SettleReceipType = settleReceipType,
        //            DataDate = dataDate,
        //            TicketedPointID = ticketedPointID,
        //            Remarks = "商城生成的会计凭证",
        //            EnterpriseID = enterpriseId,
        //            OwnerID =ownerID,
        //            BeginDate = period?.StartDate.ToString("yyyy-MM-dd"),
        //            EndDate = period?.EndDate.ToString("yyyy-MM-dd")
        //        };
        //        //借方
        //        var dDetail = new FD_SettleReceiptDetailInterfaceCommand()
        //        {
        //            EnterpriseID =enterpriseId,
        //            ReceiptAbstractID = receiptAbstractID,
        //            AccoSubjectID = dAccoSubjectID,
        //            AccoSubjectCode = dAccoSubjectCode,
        //            CustomerID = customerId,
        //            PersonID =  "0",
        //            MarketID = "0",
        //            ProjectID = "0",
        //            ProductID = "0",
        //            ProductGroupID = "0",
        //            ClassificationID = "0",
        //            PaymentTypeID = "0",
        //            AccountID = "0",
        //            LorR = false,
        //            Credit = 0,
        //            Debit = amount,
        //            Content = content,
        //            RowNum = 1,
        //            OrganizationSortID = "0",
        //            IsCharges = false
        //        };
        //        //贷方
        //        var cDetail = new FD_SettleReceiptDetailInterfaceCommand()
        //        {
        //            EnterpriseID = enterpriseId,
        //            ReceiptAbstractID = receiptAbstractID,
        //            AccoSubjectID = cAccoSubjectID,
        //            AccoSubjectCode = cAccoSubjectCode,
        //            CustomerID = "0",
        //            PersonID = "0",
        //            MarketID = "0",
        //            ProjectID = "0",
        //            ProductID = "0",
        //            ProductGroupID = "0",
        //            ClassificationID = "0",
        //            PaymentTypeID = paymentTypeID,
        //            AccountID = account.AccountID,
        //            LorR = true,
        //            Credit = amount,
        //            Debit = 0,
        //            Content = content,
        //            RowNum = 2,
        //            OrganizationSortID = "0",
        //            IsCharges = false
        //        };
        //        var detailList = new List<FD_SettleReceiptDetailInterfaceCommand>();
        //        detailList.Add(dDetail);
        //        detailList.Add(cDetail);
        //        request.Lines = detailList;
        //        res = _mediator.Send(request, HttpContext.RequestAborted).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"商城AddSettleReceipt:ex={ex.ToString()};param={JsonConvert.SerializeObject(param)}");
        //    }

        //    return res;
        //}



        ///// <summary>
        ///// 商城生成收款单
        ///// </summary>
        ///// <param name="param"></param>
        //[HttpPost]
        //[Route("AddReceivables")]
        //public Result AddReceivables(FD_ScReceivable param)
        //{
        //    var res = new Result() { code = 1 };
        //    try
        //    {
        //        _logger.LogInformation($"商城AddReceivables:param={JsonConvert.SerializeObject(param)}");
        //        if (param==null) { res.msg = "参数不能为空"; return res; }
        //        if (string.IsNullOrEmpty(param.enterpriseId)) { res.msg = "enterpriseId不能为空"; return res; }
        //        //推送收款单，超出礼品卡金额部分，客户 = GK，金额是超出部分
        //        //（1）单据 = 收款单，收款会计凭证（根据系统选项判断是否推送会计凭证）
        //        //（2）单据字，对应单位默认第一个
        //        //（3）摘要：外销收款
        //        //（4）往来类型：客户
        //        //（5）客户ID：推送数据提供
        //        //（6）会计科目：
        //        //借：银行存款 / 对公普通账户 / 第三方账户 / 易宝
        //        //贷：应收账款 / 应收外部款 / 应收外部货款—大有谷零售客户（GK）
        //        //（7）推送数据提供：业务日期、客户ID、金额
        //        //单位
        //        var enterpriseId=param.enterpriseId;
        //        var enData = _provider.GetBiz_EnterpriseInfos(enterpriseId);
        //        if (enData == null) { res.msg = $"当前单位:{enterpriseId}在企联网中不存在"; return res; }
        //        //日期
        //        var dataDate=DateTime.Now;
        //        var tickets = _provider.GetTicketPointByEnteId(enterpriseId);
        //        //单据字，对应单位默认第一个
        //        var ticketedPointID = tickets.Where(m => m.EnterpriseId == enterpriseId).FirstOrDefault()?.TicketedPointId;
        //        if (string.IsNullOrEmpty(ticketedPointID)||ticketedPointID=="0") { res.msg = "无单据字";return res; }
        //        //科目
        //        var accos = _provider.GetAccoSubjectInfos(param.dataDate, new EnterpriseInfo() { EnterpriseId = enterpriseId }, enData.Pid);
        //        //摘要
        //        var settles = _provider.GetSettleSummaryByDate(enData.Pid,enterpriseId,dataDate.ToString());

        //        //借：银行存款/对公普通账户/第三方账户/易宝 1002010504
        //        var dAccoSubjectCode = "1002010504";
        //        var dAccoSubjectID = accos.Where(m => m.AccoSubjectCode == dAccoSubjectCode).FirstOrDefault()?.AccoSubjectId;
        //        //贷：应收账款/应收外部款/应收外部货款—大有谷零售客户（GK）
        //        var cAccoSubjectCode = "11220301";
        //        var cAccoSubjectID = accos.Where(m => m.AccoSubjectCode == cAccoSubjectCode).FirstOrDefault()?.AccoSubjectId;
        //        //摘要：外销收款 010101
        //        var receiptAbstractID = settles.Where(m => m.Remarks == "010101" && m.EnterpriseId == enterpriseId).FirstOrDefault()?.SettleSummaryId;

        //        EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
        //        //资金账户
        //        var account = _AccountODataProvider.GetDataByEnterpriseId(Convert.ToInt64(enterpriseId)).FirstOrDefault();
        //        //var accountResult = encryptAccount.AccountNumberDecrypt(account.AccountNumber);
        //        //var accountNumber = accountResult?.Item1 == true ? accountResult.Item2 : "0";
        //        //if (string.IsNullOrEmpty(accountNumber))
        //        //{
        //        //    _logger.LogError($"商城AddReceivables:银行账号空:{JsonConvert.SerializeObject(account)},accountResult={JsonConvert.SerializeObject(accountResult)}");
        //        //}
        //        //结算方式
        //        var paymentTypeID = "201610140104402004";//网银 pid=201610140104402001
        //        //往来类型
        //        var businessType = "201611180104402201";//客户
        //        //金额
        //        var amount = param.amount;
        //        //内容
        //        var content = "";
        //        //客户
        //        var customerId= param.customerId;
        //        //客户银行账户
        //        //会计期间
        //        var period = _enterprisePeriodUtil.GetEnterperisePeriod(enterpriseId, dataDate.ToString("yyyy-MM-dd"));
        //        //制单人
        //        var ownerID = "0";
        //        var numericalOrder = _numericalOrderCreator.CreateAsync().Result;
        //        var request = new FD_ReceivablesAddCommand()
        //        {
        //            NumericalOrder = numericalOrder,
        //            AppId = "1611231950150000101",
        //            BusinessType = businessType,
        //            EnterpriseID = enterpriseId,
        //            DataDate = dataDate,
        //            BeginDate = period?.StartDate.ToString("yyyy-MM-dd"),
        //            EndDate = period?.EndDate.ToString("yyyy-MM-dd"),
        //            TicketedPointID = ticketedPointID,
        //            DebitAccoSubjectID = account.AccoSubjectID,
        //            details = new List<FD_PaymentReceivablesDetailCommand>(),
        //            extend = new List<FD_PaymentReceivables.FD_PaymentExtend>(),
        //            Remarks = "商城生成收款单",
        //            OwnerID = ownerID,
        //            SettleReceipType = "201611180104402201",
        //            GroupId = enData.Pid,
        //        };
        //        request.details.Add(new FD_PaymentReceivablesDetailCommand()
        //        {
        //            NumericalOrder = numericalOrder,
        //            AccoSubjectID = cAccoSubjectID,
        //            PaymentTypeID = paymentTypeID,
        //            AccountID = account.AccountID,
        //            Amount = amount,
        //            Charges = 0,
        //            CostAccoSubjectID = "0",
        //            OrganizationSortID = "0",
        //            BusinessType = businessType,
        //            Content = content,
        //            ReceiptAbstractID = receiptAbstractID,
        //            EnterpriseID = enterpriseId,
        //            CustomerID = customerId,
        //            MarketID = "0",
        //            PersonID = "0",
        //            ProjectID = "0",
        //            ProductID = "0",
        //        }) ;
        //        request.extend.Add(new FD_PaymentReceivables.FD_PaymentExtend()
        //        {
        //            NumericalOrder = numericalOrder,
        //            CollectionId = customerId,
        //            BankAccount = "",
        //            Amount = amount
        //        });
        //        //foreach (var ritem in request.extend)
        //        //{
        //        //    var resultRece = encryptAccount.AccountNumberEncrypt(ritem.BankAccount);
        //        //    ritem.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : ritem.BankAccount;
        //        //}
        //        _logger.LogInformation("商城AddReceivables生成收款调用前：" + JsonConvert.SerializeObject(request));
        //        res = _mediator.Send(request, HttpContext.RequestAborted).Result;                
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"商城AddReceivables:ex={ex.ToString()};param={JsonConvert.SerializeObject(param)}");
        //    }
        //    return res;
        //}

        #endregion
    }
}
