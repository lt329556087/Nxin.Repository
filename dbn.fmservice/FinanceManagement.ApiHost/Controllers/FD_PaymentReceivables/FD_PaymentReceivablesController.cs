using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
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
    public class FD_PaymentReceivablesController : ControllerBase
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
        NumericalOrderCreator _numericalOrderCreator;
        IBiz_Related _biz_Related;
        IBiz_RelatedDetailRepository _RelatedDetailRepository;
        FMBaseCommon _fmBaseCommon;
        public FD_PaymentReceivablesController(IBiz_RelatedDetailRepository RelatedDetailRepository, IBiz_Related biz_Related, IMediator mediator, HttpClientUtil httpClientUtil,
            FMBaseCommon fmBaseCommon,
            //FD_PaymentReceivablesTODataProvider provider, 
            FundSummaryUtil fundSummaryUtil, IFD_PaymentExtendRepository paymentExtendRepository, NumericalOrderCreator numericalOrderCreator, IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, ILogger<FD_PaymentReceivablesController> logger, HostConfiguration hostCongfiguration)
        {
            _fmBaseCommon = fmBaseCommon;
            _mediator = mediator;
            //_provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
            _prodiver = prodiver;
            _biz_Related = biz_Related;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _paymentExtendRepository = paymentExtendRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _RelatedDetailRepository = RelatedDetailRepository;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        //[HttpGet("{key}")]
        //public async Task<Result> GetDetail(long key)
        //{
        //    var result = new Result();
        //    var data = await _provider.GetSingleDataAsync(key);
        //    result.data = data;
        //    return result;
        //}

        //增加
        [HttpPost]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_PaymentReceivablesAddCommand request)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in request.extend)
            {
                var resultRece = encryptAccount.AccountNumberEncrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //增加
        [HttpPost]
        [Route("AddSummary")]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> AddSummary([FromBody] FD_PaymentReceivablesSummaryAddCommand request)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in request.details)
            {
                var resultRece = encryptAccount.AccountNumberEncrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //删除
        [HttpDelete]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Delete([FromBody] FD_PaymentReceivablesDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_PaymentReceivablesModifyCommand request)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in request.extend)
            {
                var resultRece = encryptAccount.AccountNumberEncrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //修改
        [HttpPut]
        [Route("ModifySummary")]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_PaymentReceivablesSummaryModifyCommand request)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in request.details)
            {
                var resultRece = encryptAccount.AccountNumberEncrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        [HttpPost]
        [Route("GetRefundList")]
        [EnableQuery(MaxNodeCount = 10000)]
        public IQueryable<RefundList> GetRefundList(ODataQueryOptions<RefundList> odataqueryoptions, Uri uri)
        {
            var entes = _fmBaseCommon.GetAuthorEnterpise(_identityService.EnterpriseId, _identityService.UserId);
            return _prodiver.GetRefundsList(entes).ToList().AsQueryable();
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [PermissionAuthorize(Permission.Retrieve)]
        public Result GetDetail(long key)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return new Result() { code = 401, data = null, msg = "无浏览权限" };
            }
            var result = new Result();
            var domain = new FD_PaymentDoMain();
            var data = _prodiver.GetDataMobileAsync(key).Result;
            if (data == null)
            {
                return new Result() { code = 404, data = null, msg = "无数据" };
            }
            domain.UploadInfo = data.UploadInfo;
            domain.Remarks = data.Remarks;
            domain.Amount = data.Amount;
            domain.ProjectID = data.ProjectID;
            domain.ProductID = data.ProductID;
            domain.CollectionId = data.CollectionId;
            domain.DebitAccoSubjectID = data.DebitAccoSubjectID;
            domain.ReceiptAbstractId = data.ReceiptAbstractId;
            domain.AttachmentNum = data.AttachmentNum;
            domain.CollectionName = data.CollectionName;
            domain.Content = data.Content;
            domain.CreatedDate = data.CreatedDate;
            domain.DataDate = data.DataDate;
            domain.EnterpriseID = data.EnterpriseID;
            domain.EnterpriseName = data.EnterpriseName;
            domain.Guid = data.Guid;
            domain.ModifiedDate = data.ModifiedDate;
            domain.Number = data.Number;
            domain.NumericalOrder = data.NumericalOrder;
            domain.OwnerID = data.OwnerID;
            domain.OwnerName = data.OwnerName;
            domain.ReceiptAbstractName = data.ReceiptAbstractName;
            domain.SettleReceipType = data.SettleReceipType;
            domain.TicketedPointID = data.TicketedPointID;
            domain.TicketedPointName = data.TicketedPointName;
            domain.ProductName = data.ProductName;
            domain.ProjectName = data.ProjectName;
            domain.VoucherNumber = data.VoucherNumber;
            domain.extend = _prodiver.GetExtend(key.ToString());
            #region 自定义辅助项
            domain.Auxiliary1 = data.Auxiliary1;
            domain.Auxiliary2 = data.Auxiliary2;
            domain.Auxiliary3 = data.Auxiliary3;
            domain.Auxiliary4 = data.Auxiliary4;
            domain.Auxiliary5 = data.Auxiliary5;
            domain.Auxiliary6 = data.Auxiliary6;
            domain.Auxiliary7 = data.Auxiliary7;
            domain.Auxiliary8 = data.Auxiliary8;
            domain.Auxiliary9 = data.Auxiliary9;
            domain.Auxiliary10 = data.Auxiliary10;
            #endregion
            domain.Auxiliary = data.Auxiliary;
            var list = _biz_Related.GetListByNum(key.ToString());
            domain.RelatedList = new List<Infrastructure.QlwCrossDbEntities.RelatedList>();
            foreach (var item in list)
            {
                var temp = _RelatedDetailRepository.GetDataByRelatedId(item.RelatedID.ToString());
                domain.RelatedList.Add(new Infrastructure.QlwCrossDbEntities.RelatedList() 
                {
                    ApplyNumericalOrder = item.ChildValue,
                    Paid = temp.Paid,
                    Payable = temp.Payable,
                    Payment = temp.Payment,
                    RecordId = item.ChildValueDetail.ToString(),
                    RelatedID = temp.RelatedID.ToString(),
                    SettleReceipType = item.ChildType,
                });
            }
            //没有制单权限的需要脱敏
            //银行卡号解密
            foreach (var item in domain.extend)
            {
                var resultRece = AccountNumberDecrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }

            domain.details = _prodiver.GetDetaiDatasAsync(key).Result;
            domain.ApplyNumericalOrder = data.ApplyNumericalOrder;
            domain.ApplyAppId = data.ApplyAppId;
            domain.BankUrl = data.BankUrl;
            domain.IsRecheck = data.IsRecheck;
            domain.PayCount = data.PayCount;
            domain.Status = data.Status;
            result.data = domain;
            result.code = 0;
            return result;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSummaryDetail")]
        [PermissionAuthorize(Permission.Retrieve)]
        public Result GetSummaryDetail(long key)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return new Result() { code = 401, data = null, msg = "无浏览权限" };
            }
            var result = new Result() { code = 0 };
            var data = _prodiver.GetSummaryDataAsync(key).Result;
            if (data == null)
            {
                return new Result() { code = 404, data = null, msg = "无数据" };
            }
            
            var details = _prodiver.GetSummaryDetaiDatasAsync(key).Result;
            foreach (var item in details)
            {
                var resultRece = AccountNumberDecrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            result.data = new
            {
                data.BusinessType,
                data.BusinessTypeName,
                data.BankUrl,
                data.IsRecheck,
                data.PayCount,
                data.Status,
                data.UploadInfo,
                Remarks = data.Remarks,
                Amount = data.Amount,
                ProjectID = data.ProjectID,
                ProductID = data.ProductID,
                CollectionId = data.CollectionId,
                DebitAccoSubjectID = data.DebitAccoSubjectID,
                ReceiptAbstractId = data.ReceiptAbstractId,
                AttachmentNum = data.AttachmentNum,
                CollectionName = data.CollectionName,
                Content = data.Content,
                CreatedDate = data.CreatedDate,
                DataDate = data.DataDate,
                EnterpriseID = data.EnterpriseID,
                EnterpriseName = data.EnterpriseName,
                Guid = data.Guid,
                ModifiedDate = data.ModifiedDate,
                Number = data.Number,
                NumericalOrder = data.NumericalOrder,
                OwnerID = data.OwnerID,
                OwnerName = data.OwnerName,
                ReceiptAbstractName = data.ReceiptAbstractName,
                SettleReceipType = data.SettleReceipType,
                TicketedPointID = data.TicketedPointID,
                TicketedPointName = data.TicketedPointName,
                data.ApplyAppId,
                data.ApplyNumericalOrder,
                data.VoucherNumber,
                extend = _prodiver.GetPaymentExtendSummaryDatas(key),
                details = details
            };
            return result;
        }
        [HttpGet]
        [Route("GetPaymentList")]
        public Result GetPaymentList(long key)
        {
            if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Retrieve))
            {
                return new Result() { code = 401, data = null, msg = "无浏览权限" };
            }
            var result = new Result();
            result.code = 0;
            var paymentLists = _prodiver.GetPaymentList(key.ToString());
            //解密银行卡号
            foreach (var item in paymentLists)
            {
                var resultRece = AccountNumberDecrypt(item.AccountID);
                item.AccountID = resultRece?.Item1 == true ? resultRece.Item2 : item.AccountID;

                var resultRece2 = AccountNumberDecrypt(item.BankAccount);
                item.BankAccount = resultRece2?.Item1 == true ? resultRece2.Item2 : item.BankAccount;
            }
            result.data = paymentLists;
            return result;
        }
        /// <summary>
        /// 获取审批条件
        /// </summary>
        /// <param name="ApprovalTypeID"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApprovalSetDetail")]
        public Result GetApprovalSetDetail(long ApprovalTypeID, long EnterpriseID)
        {
            var result = new Result();
            result.code = 0;
            result.data = _prodiver.GetApprovalSetDetail(ApprovalTypeID,EnterpriseID).Result;
            return result;
        }
        /// <summary>
        /// 获取复核阶段流程数据
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReviewFlowPath")]
        public Result GetReviewFlowPath(string num)
        {
            var result = new Result();
            var list = _prodiver.GetReviewFlowPath(num).Result;
            result.code = 0;
            result.data = list;
            return result;
        }
        /// <summary>
        /// 更新复核状态）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("CancelRecheck")]
        public Result CancelRecheck([FromBody] FD_PaymentReceivablesCancelLogicCommand request)
        {
            if (request.RawLevel != 0)
            {
                return _mediator.Send(new FD_PaymentReceivablesReviewLogicCommand() { Level = request.Level, RawLevel = request.RawLevel, RecordId = request.NumericalOrder, Status = request.Status }, HttpContext.RequestAborted).Result;
            }
            return _mediator.Send(request, HttpContext.RequestAborted).Result; 
        }
        /// <summary>
        /// 批量支付（跳转金融输入密码）
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("YqtBatchPay")]
        public List<Result> YqtBatchPay(List<PayType> models)
        {
            _logger.LogInformation("集中支付开始支付："+JsonConvert.SerializeObject(models));
            if (!_identityService.IsPermisson(Permission.Report))
            {
                var data = new List<Result>();
                data.Add(new Result() { code = 401, data = null, msg = "无浏览权限" });
                return data;
            }
            var result = new List<Result>();
            List<AccountList> list = new List<AccountList>();

            string OAurl = _hostCongfiguration._rdUrl + "/api/BSNewsCenter/SendNeweCenter";
            string ZNTurl = _hostCongfiguration._rdUrl + "/api/BSNewsCenter/SendNotice";
            //string ZNTurl = "http://qlw.data.test.com/api/BSNewsCenter/SendSingleNotice";
            var oaList = new List<OASend>();
            var zntList = new List<NoticeInfo>();
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                var ulist = new List<long>();
                if (string.IsNullOrEmpty(model.TransferType))
                {
                    model.TransferType = "0";
                }
                //true = 走复合 这里只保存用途和转账方式
                //支持 多级复核
                if (model.IsRecheck)
                {
                    if (model.RawLevel == 0)
                    {
                        _mediator.Send(new FD_PaymentReceivablesLogicCommand() { Level = model.Level,RawLevel = model.RawLevel,AuditList = model.AuditList,IsRecheck = model.IsRecheck, PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = "", failNum = "1", NumericalOrder = model.extendRecordID }, HttpContext.RequestAborted);
                        result.Add(new Result() { code = 0, msg = "提交成功" });
                    }
                    else
                    {
                        var reviewData = _mediator.Send(new FD_PaymentReceivablesReviewLogicCommand() { Level = model.Level,RawLevel = model.RawLevel,RecordId = model.extendRecordID,Status = model.Status }, HttpContext.RequestAborted).Result;
                        if (reviewData.code == 0 && reviewData.msg == "发起支付成功")
                        {
                            i--;
                            model.IsRecheck = false;
                            continue;
                        }
                        else
                        {
                            result.Add(reviewData);
                        }
                    }
                    try
                    {
                        var TData = _prodiver.GetDataMobileAsync(Convert.ToInt64(model.NumericalOrder)).Result;
                        var TDetail = _prodiver.GetDetaiDatasAsync(Convert.ToInt64(model.NumericalOrder)).Result;
                        var TExtend = _prodiver.GetExtend(model.NumericalOrder);
                        //付方账户
                        var payAccountName = string.Join(",", TDetail.Where(m => m.RecordID.ToString() == model.RecordId).Select(m => m.AccountName));
                        //收方账户
                        var receivablesName = string.Join(",", TExtend.Where(m => m.RecordID.ToString() == model.extendRecordID).Select(m => m.CollectionName));
                        //支付金额 (单条RecordID 明细 金额)
                        var payAmount = TDetail.Where(m => m.RecordID.ToString() == model.RecordId).Sum(m => m.Amount);
                        //人员id
                        var personid = TDetail.Where(m => m.RecordID.ToString() == model.RecordId).FirstOrDefault().PersonID;
                        //资金账户ID
                        var accountid = TDetail.Where(m => m.RecordID.ToString() == model.RecordId).FirstOrDefault().AccountID;
                        if (string.IsNullOrEmpty(model.PersonIds))
                        {
                            model.PersonIds = "0";
                        }
                        oaList.Add(new()
                        {
                            Type = (TData.SettleReceipType == "201611180104402202" ? 1612011058280000101 : 2205251638120000109),
                            BusinessID = Convert.ToInt64(model.NumericalOrder),
                            EnterpriseID = Convert.ToInt64(TData.EnterpriseID),
                            NewsCenterAbstract = 201707220104402207L,
                            OwnerID = Convert.ToInt64(TData.OwnerID),
                            Subject = $@"您有一笔 {TData.EnterpriseName}【付款单】待复核，申请日期：{TData.DataDate}，付方账户：【{payAccountName}】，收方账户：【{receivablesName}】，支付金额（元）：¥ {payAmount}。",
                            UserIds = model.PersonIds
                        });
                        foreach (var item in model.PersonIds.Split(',').ToList())
                        {
                            ulist.Add(Convert.ToInt64(item));
                        }
                        zntList.Add(new()
                        {
                            MsgType = MessageType.notice,
                            UserId = ulist,
                            Title = "消息提醒",
                            AccontId = Convert.ToInt64(_hostCongfiguration.NoticeAccountID),//测试218 正式 217
                            Content = $@"您有一笔 {TData.EnterpriseName}【付款单】待复核，申请日期：{TData.DataDate}，付方账户：【{payAccountName}】，收方账户：【{receivablesName}】，支付金额（元）：¥ {payAmount}。",
                            MsgSendDescribe = "消息提醒",
                            Url = TData.SettleReceipType == "201611180104402202" ? _hostCongfiguration.fsfMobileUrl + $"/mobile/mobile.html#/order-info?RecordId={model.RecordId}&NumericalOrder={model.NumericalOrder}&MsgType=notice&PersonId={personid}&AccountID={accountid}" : _hostCongfiguration.fsfMobileUrl + $"/mobile/mobile.html#/summary-info?RecordId={model.RecordId}&NumericalOrder={model.NumericalOrder}&MsgType=notice&PersonId={personid}&AccountID={accountid}",
                            bizId = model.extendRecordID
                        });
                    }
                    catch (Exception e)
                    {
                        var TData = _prodiver.GetDataMobileAsync(Convert.ToInt64(model.NumericalOrder)).Result;
                        var TDetail = _prodiver.GetDetaiDatasAsync(Convert.ToInt64(model.NumericalOrder)).Result;
                        var TExtend = _prodiver.GetExtend(model.NumericalOrder);
                        //付方账户
                        var payAccountName = string.Join(",", TDetail.Where(m => m.RecordID.ToString() == model.RecordId).Select(m => m.AccountName));
                        //收方账户
                        var receivablesName = string.Join(",", TExtend.Where(m => m.RecordID.ToString() == model.extendRecordID).Select(m => m.CollectionName));
                        //支付金额 (单条RecordID 明细 金额)
                        var payAmount = TDetail.Where(m => m.RecordID.ToString() == model.RecordId).Sum(m => m.Amount);
                        //人员id
                        var personid = TDetail.Where(m => m.RecordID.ToString() == model.RecordId).FirstOrDefault().PersonID;
                        //资金账户ID
                        var accountid = TDetail.Where(m => m.RecordID.ToString() == model.RecordId).FirstOrDefault().AccountID;
                        _logger.LogInformation("消息通知发送失败可能存在空值："+JsonConvert.SerializeObject(e));
                        _logger.LogInformation("model:" + JsonConvert.SerializeObject(model));
                        _logger.LogInformation("TData:"+ JsonConvert.SerializeObject(TData));
                        _logger.LogInformation("TDetail:" + JsonConvert.SerializeObject(TDetail));
                        _logger.LogInformation("TExtend:" + JsonConvert.SerializeObject(TExtend));
                        _logger.LogInformation("payAccountName:" + JsonConvert.SerializeObject(payAccountName));
                        _logger.LogInformation("receivablesName:" + JsonConvert.SerializeObject(receivablesName));
                        _logger.LogInformation("payAmount:" + JsonConvert.SerializeObject(payAmount));
                        _logger.LogInformation("personid:" + JsonConvert.SerializeObject(personid));
                        _logger.LogInformation("accountid:" + JsonConvert.SerializeObject(accountid));
                        _logger.LogInformation("model.PersonIds:" + JsonConvert.SerializeObject(model.PersonIds));
                        _logger.LogInformation("ulist:" + JsonConvert.SerializeObject(ulist));
                        _logger.LogInformation("_hostCongfiguration.NoticeAccountID:" + JsonConvert.SerializeObject(_hostCongfiguration.NoticeAccountID));
                    }
                    continue;
                }
                //var Hdata = _prodiver.GetDataAsync(Convert.ToInt64(model.NumericalOrder)).Result;
                var data = _prodiver.GetDetaiDatasAsync(Convert.ToInt64(model.NumericalOrder)).Result.Where(m => m.RecordID == Convert.ToInt32(model.RecordId)).ToList();
                var detail = _prodiver.GetPaymentList(model.NumericalOrder).Where(m => m.RecordId == Convert.ToInt32(model.RecordId)).ToList();
                var extend = _prodiver.GetExtend(model.NumericalOrder).Where(m => m.RecordID == Convert.ToInt32(model.extendRecordID)).ToList();
                //发起支付前 确保 未支付
                var eData = _paymentExtendRepository.GetAsyncByRecordId(extend.FirstOrDefault().RecordID.ToString()).Result;
                if (eData.Status == 1)
                {
                    result.Add(new Result() { code = 500, msg = "该笔交易已支付：" + eData.RecordID });
                    continue;
                }
                if (eData.Status == 3)
                {
                    result.Add(new Result() { code = 500, msg = "该笔交易处理中：" + eData.RecordID });
                    continue;
                }
                if (extend.Count == 0)
                {
                    extend.Add(new FD_PaymentExtendEntity()
                    {
                        AccountName = "",
                        AccountNature = true,
                        Amount = 1,
                        BankAccount = "ee",
                        BankCode = "s",
                        BankDeposit = "e",
                        BankName = "ss",
                        BankNumber = "sss",
                        CollectionId = "123123",
                        NumericalOrder = "1234",
                        PersonID = "123123",
                        RecordID = 0
                    });
                }
                foreach (var item in detail)
                {
                    //AccountID = AccountNumber

                    var resultRece = AccountNumberDecrypt(item.AccountID);
                    item.AccountID = resultRece?.Item1 == true ? resultRece.Item2 : item.AccountID;

                    var resultRece2 = AccountNumberDecrypt(extend[0].BankNumber);
                    extend[0].BankNumber = resultRece2?.Item1 == true ? resultRece2.Item2 : extend[0].BankNumber;
                    model.TradeNo = _numericalOrderCreator.CreateAsync().Result;
                    list.Add(new AccountList()
                    {
                        accName = item.AccountName,
                        accNo = item.AccountID,
                        amount = item.Amount.ToString(),
                        bankType = item.PayBank,
                        clientId = item.BankNumber,
                        drawee = data[0].EnterpriseID,
                        iscomm = data[0].BusinessType == "201611160104402103" ? "1" : (extend[0].AccountNature == null ? false : Convert.ToBoolean(extend[0].AccountNature)) ? "2" : "1",//? 收款人类型 对公对私
                        localFlag = model.TransferType, // 汇路
                        note = model.PayUse,
                        orderNo = model.TradeNo,//金融所用的流水号
                        payee = string.IsNullOrEmpty(extend[0].BO_ID) ? "1262417" : extend[0].BO_ID,
                        toAccName = extend[0].AccountName,
                        toAccNo = extend[0].BankNumber,
                        toBankName = extend[0].BankName,
                        toBankCode = extend[0].BankCode, // 收款人银行编码
                        billName = extend[0].TicketedPointName,
                        billNo = extend[0].Number,
                        markingAmount = extend[0].Amount.ToString(),
                        uniqueNo = model.RecordId
                    });
                };
            }

            //复核消息通知
            foreach (var item in oaList)
            {
                var oaresult = _httpClientUtil1.PostJsonAsync<dynamic, dynamic>(OAurl, item).Result;
            }
            foreach (var item in zntList)
            {
                var zntresult = _httpClientUtil1.PostJsonAsync<dynamic, dynamic>(ZNTurl, item).Result;
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("batch_no", _numericalOrderCreator.CreateAsync().Result);//批次号不可重复（重新发起支付）
            dic.Add("opt_user_id", _identityService.UserId);
            //金融加密规则 会移除所有空字符
            dic.Add("data", JsonConvert.SerializeObject(list).Replace(" ", ""));
            dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
            dic.Add("busi_no", "NX-I-005"); /*业务来源*/
            dic.Add("call_back_url", string.Format($"{_hostCongfiguration._wgUrl}/dbn/fm/api/FD_PaymentCall/GetPaymentCallBack")); /*回调地址*/
            dic.Add("busi_order_no", models[0].NumericalOrder);
            dic.Add("busi_type", "17");
            var sgin = "";
            //73fb3ddb05fc466b92f1a096903a93c0
            var key = _hostCongfiguration.OrderKey ?? "5dcc96e21db642dab159f97dbe61ff39";
            dic.Add("key", key); /*发送平台的key*/
            Dictionary<string, object> sortMap = new Dictionary<string, object>();

            sortMap = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

            foreach (var t in sortMap)
            {
                sgin += t.Key + t.Value;
            }
            _logger.LogInformation("加密前数据sgin：" + sgin);
            var md5Str = RptBankAccountInquiryCommon.MD5Encrypt(sgin);
            _logger.LogInformation("加密后数据sgin：" + sgin);
            dic.Add("signmsg", md5Str);
            dic.Add("open_req_src", "qlw-web");
            dic = dic.Where(t => t.Key != "key").ToDictionary(t => t.Key, t => t.Value);
            dic = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

            string ps = "";
            foreach (var keyValuePair in dic)
            {
                ps += keyValuePair.Key + "=" + keyValuePair.Value + "&";
            }

            ps = ps.TrimEnd('&');

            if (list.Count == 0)
            {
                _logger.LogInformation("集中支付发起支付前轮训无数据：" + ps);
                return result;
            }
            _logger.LogInformation("集中支付发起支付前List：" + JsonConvert.SerializeObject(list));
            _logger.LogInformation("集中支付发起支付前：" + ps);
            //如果触发支付 则优先弹出 支付界面 复核结果 自行查看
            result = new List<Result>();
            var res = postMessage($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.batch.transfer/1.0", ps);
            result.Add(JsonConvert.DeserializeObject<Result>(res));
            _logger.LogInformation("集中支付发起支付后结果：" + JsonConvert.SerializeObject(res));
            PayConection payConection = new PayConection() {failInfo = ""+res,failNum = "1" };
            try
            {
                try
                {
                    payConection = JsonConvert.DeserializeObject<PayConection>(JsonConvert.DeserializeObject<Result>(res).data.ToString());
                }
                catch (Exception exx)
                {
                    _logger.LogInformation("堆栈："+JsonConvert.SerializeObject(exx)+"\n 序列化异常："+JsonConvert.SerializeObject(res));
                }
                foreach (var model in models)
                {
                    try
                    {
                        _logger.LogInformation("集中支付UPDATE调用前FD_PaymentReceivablesLogicCommand--------------------------------------------：" + JsonConvert.SerializeObject(new { TradeNo = model.TradeNo, RecheckId = model.RecheckId, PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = JsonConvert.SerializeObject(payConection.failInfo), failNum = payConection.failNum, NumericalOrder = model.extendRecordID }));
                        _mediator.Send(new FD_PaymentReceivablesLogicCommand() {IsPay = true, TradeNo = model.TradeNo, RecheckId = model.RecheckId, PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = JsonConvert.SerializeObject(payConection.failInfo), failNum = payConection.failNum, NumericalOrder = model.extendRecordID }, HttpContext.RequestAborted);
                    }
                    catch
                    {
                        //失败 写入原因
                        _logger.LogInformation("集中支付UPDATE 失败 写入原因:"+JsonConvert.SerializeObject(new { RecheckId = model.RecheckId, PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = JsonConvert.SerializeObject("验签未通过"), failNum = "0", NumericalOrder = model.extendRecordID }));
                        _mediator.Send(new FD_PaymentReceivablesLogicCommand() { IsPay = true, RecheckId = model.RecheckId, PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = JsonConvert.SerializeObject("验签未通过"), failNum = "0", NumericalOrder = model.extendRecordID }, HttpContext.RequestAborted);
                    }
                }
            }
            catch (Exception e)
            {
                return result;
            }
            return result;
        }
        [HttpPost]
        [Route("YqtPay")]
        public List<Result> YqtPay(List<PayType> models)
        {
            if (!_identityService.IsPermisson(Permission.Report))
            {
                var data = new List<Result>();
                data.Add(new Result() { code = 401, data = null, msg = "无浏览权限" });
                return data;
            }
            var result = new List<Result>();
            foreach (var model in models)
            {
                //true = 走复合 这里只保存用途和转账方式
                if (model.IsRecheck)
                {
                    _mediator.Send(new FD_PaymentReceivablesLogicCommand() { IsRecheck = model.IsRecheck,PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = "", failNum = "1", NumericalOrder = model.extendRecordID }, HttpContext.RequestAborted);
                    result.Add(new Result() { code = 0,msg = "提交成功" });
                    continue;
                }
                //var Hdata = _prodiver.GetDataAsync(Convert.ToInt64(model.NumericalOrder)).Result;
                var data = _prodiver.GetDetaiDatasAsync(Convert.ToInt64(model.NumericalOrder)).Result.Where(m => m.RecordID == Convert.ToInt32(model.RecordId)).ToList();
                var detail = _prodiver.GetPaymentList(model.NumericalOrder).Where(m => m.RecordId == Convert.ToInt32(model.RecordId)).ToList();
                var extend = _prodiver.GetExtend(model.NumericalOrder).Where(m=>m.RecordID == Convert.ToInt32(model.extendRecordID)).ToList();

                if (extend.Count == 0)
                {
                    extend.Add(new FD_PaymentExtendEntity()
                    {
                        AccountName = "",
                        AccountNature = true,
                        Amount = 1,
                        BankAccount = "ee",
                        BankCode = "s",
                        BankDeposit = "e",
                        BankName = "ss",
                        BankNumber = "sss",
                        CollectionId = "123123",
                        NumericalOrder = "1234",
                        PersonID = "123123",
                        RecordID = 0
                    });
                }
                List<AccountList> list = new List<AccountList>();
                foreach (var item in detail)
                {
                    //AccountID = AccountNumber

                    var resultRece = AccountNumberDecrypt(item.AccountID);
                    item.AccountID = resultRece?.Item1 == true ? resultRece.Item2 : item.AccountID;

                    var resultRece2 = AccountNumberDecrypt(extend[0].BankNumber);
                    extend[0].BankNumber = resultRece2?.Item1 == true ? resultRece2.Item2 : extend[0].BankNumber;

                    list.Add(new AccountList()
                    {
                        accName = item.AccountName,
                        accNo = item.AccountID,
                        amount = item.Amount.ToString(),
                        bankType = item.PayBank,
                        clientId = item.BankNumber,
                        drawee = _identityService.UserId,
                        iscomm = data[0].BusinessType == "201611160104402103" ? "1" : (extend[0].AccountNature == null ? false : Convert.ToBoolean(extend[0].AccountNature)) ? "2" : "1",//? 收款人类型 对公对私
                        localFlag = model.TransferType, // 汇路
                        note = model.PayUse,
                        orderNo = _numericalOrderCreator.CreateAsync().Result,//金融所用的流水号
                        payee = extend[0].BO_ID,
                        toAccName = extend[0].AccountName,
                        toAccNo = extend[0].BankNumber,
                        toBankName = extend[0].BankName,
                        toBankCode = extend[0].BankCode // 收款人银行编码
                    });
                };
                #region 注释
                //open_req_src String  GET 是   nxin_shuju 此参数为网关标识接口调用系统，例如大数据平台调用此接口 open_req_src = nxin_shuju ，参数值与proj上项目名保持一致
                //signmsg String POST表单  是 d41d8cd98f00b204e9800998ecf8427e    加密串规则待定
                //times   String POST表单  是   202011120945    请求时间 yyyyMMddHHmm
                //busi_no String  POST表单 是   NX - I - 005    业务来源
                //   call_back_url   String POST表单  是 http://jrd.t.nxin.com/demo/cashier/callBack.shtml	回调地址
                //data String  POST表单 是[{ "accName": "付款账户名称", "accNo": "付款账户", "amount": "转账金额,单位:元", "bankType": "银行类型", "clientId": "企业客户号", "drawee": "付款人ID", "iscomm": "收款人类型1对私2对公（20200107）", "localFlag": "汇路", "note": "用途", "orderNo": "业务支付订单号", "payee": "收款人ID", "toAccName": "测2200003220", "toAccNo": "收款账户", "toBankCode": "收款人银行编码", "toBankName": "收款账户银行名称" }]	订单list
                //busi_order_no   String POST表单  是 业务订单号
                //busi_type String  POST表单 是   25  批量转账：25
                //响应参数
                //名称  类型 示例  描述
                //failInfo    String  "TETS160430s3093220": "参数验证未通过" 订单号：原因
                //failNum String  1 字符串 失败条数
                //successNum String  2 字符串 成功条数
                #endregion
                foreach (var item in list)
                {

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    var tempList = new List<AccountList>();
                    tempList.Add(item);
                    dic.Add("data", JsonConvert.SerializeObject(tempList));
                    dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
                    dic.Add("busi_no", "NX-I-005"); /*业务来源*/
                    dic.Add("call_back_url", string.Format($"{_hostCongfiguration._wgUrl}/dbn/fm/api/FD_PaymentCall/GetPaymentCallBack")); /*回调地址*/
                    dic.Add("busi_order_no", model.extendRecordID);
                    //dic.Add("busi_type", "25");
                    var sgin = "";
                    //73fb3ddb05fc466b92f1a096903a93c0
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

                    ps = ps.TrimEnd('&');

                    //发起支付前 确保 未支付
                    var eData = _paymentExtendRepository.GetAsync(extend.FirstOrDefault().RecordID.ToString()).Result;
                    if (eData.Status == 1)
                    {
                        result.Add(new Result() { code = 500, msg = "该笔交易已支付：" + eData.RecordID });
                        continue;
                    }
                    if (eData.Status == 3)
                    {
                        result.Add(new Result() { code = 500, msg = "该笔交易处理中：" + eData.RecordID });
                        continue;
                    }
                    _logger.LogInformation("集中支付发起支付前："+ps);
                    var res = postMessage($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.batch.transfer/1.0", ps);
                    result.Add(JsonConvert.DeserializeObject<Result>(res));
                    try
                    {
                        PayConection payConection = JsonConvert.DeserializeObject<PayConection>(JsonConvert.DeserializeObject<Result>(res).data.ToString());
                        _mediator.Send(new FD_PaymentReceivablesLogicCommand() { RecheckId = model.RecheckId, PayUse = model.PayUse,TransferType = Convert.ToInt32(model.TransferType),failInfo = JsonConvert.SerializeObject(payConection.failInfo),failNum = payConection.failNum,NumericalOrder = extend.FirstOrDefault().RecordID.ToString() }, HttpContext.RequestAborted);
                    }
                    catch
                    {
                        _mediator.Send(new FD_PaymentReceivablesLogicCommand() { RecheckId = model.RecheckId, PayUse = model.PayUse, TransferType = Convert.ToInt32(model.TransferType), failInfo = JsonConvert.SerializeObject("验签未通过"), failNum = "0", NumericalOrder = extend.FirstOrDefault().RecordID.ToString() }, HttpContext.RequestAborted);
                    }

                    //失败 写入原因
                    
                }
            }
            return result;
        }
        [HttpPost]
        [Route("CashReCharge")]
        public dynamic CashReCharge()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            AdvanceCharge advance = new AdvanceCharge();
            advance.acctType = "YFK";
            advance.amt = "100.00";
            advance.busiNo = "NX-I-005";
            advance.busiOrderNo = "2201041949200032001";
            advance.custId = "88";
            advance.entId = "8";
            advance.operatorId = "57543";
            advance.platCode = "PLAT_NX";
            advance.times = DateTime.Now.ToString("yyyyMMddHHmm");
            advance.transTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            advance.transType = "RECHARGE";
            advance.sourceBusiOrderNo = "";
            //advance.times = "202303161850";
            //advance.transTime = "20230316162536";
            dic.Add("acctType",advance.acctType);
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

            var res = _httpClientUtil1.PostJsonAsync<dynamic,dynamic>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.cmp.cashbook.recharge/1.0?open_req_src=nxin_shuju",advance).Result;
            string json = JsonConvert.SerializeObject(advance);
            return res;
        }
        [NonAction]
        public Tuple<bool, string> AccountNumberDecrypt(string AccountNumber)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            var number = encryptAccount.AccountNumberDecrypt(AccountNumber);
            try
            {
                //没有制单权限 数据脱敏
                if (!_identityService.IsPermisson(Permission.Making) && !_identityService.IsPermisson(Permission.Report))
                {
                    var str = number.Item1 ? number.Item2.Substring(4, number.Item2.Length - 4) : "";
                    if (string.IsNullOrEmpty(str))
                    {
                        return number;
                    }
                    str = str.Substring(0, str.Length - 4);
                    str = number.Item2.Replace(str, "****");
                    return new Tuple<bool, string>(true, str);
                }
                else
                {
                    return number;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("银行卡号过短，不符合脱敏要求（非法银行卡号）" + JsonConvert.SerializeObject(number)+":堆栈："+JsonConvert.SerializeObject(ex));
                return number;
            }
        }
        
        [HttpGet]
        [Route("GetPerson")]
        public List<ResultModel> GetPerson(string EnterpriseId,string Type = "1",string isonjob = null)
        {
            //http://open.i.nxin.com/inner/api/nxin.qlw.person.get/1.0?group_id=957025251000000&open_req_src=dbn.fmservice&type=1&enter_id=634086739144000031
            List<ResultModel> resultModel = new List<ResultModel>();
            foreach (var item in EnterpriseId.Split(','))
            {
                resultModel.Add(_httpClientUtil1.GetJsonAsync<ResultModel>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlw.person.get/1.0?group_id={_identityService.GroupId}&open_req_src=dbn.fmservice&type={Type}&enter_id={item}&isonjob={isonjob}").Result);
            }
            return resultModel;
        }
        [HttpGet]
        [Route("GetUseNote")]
        public ResultModel GetUseNote(string client_id,string bank_type)
        {
            //http://open.i.nxin.com/inner/api/nxin.qlw.person.get/1.0?group_id=957025251000000&open_req_src=dbn.fmservice&type=1&enter_id=634086739144000031
            var resultModel = _httpClientUtil1.GetJsonAsync<ResultModel>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.yqt.manage.note.list/1.0?open_req_src=qlw-web&client_id={client_id}&bank_type={bank_type}").Result;
            return resultModel;
        }
        [HttpPost]
        [Route("GetYqtAmount")]
        public dynamic GetYqtAmount(string acc_no,string bank_type, string client_id,string drawee)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("acc_no", acc_no);
            dic.Add("client_id", client_id);
            dic.Add("bank_type", bank_type);
            dic.Add("times", DateTime.Now.ToString("yyyyMMddHHmm"));
            dic.Add("busi_no", "NX-I-005"); /*业务来源*/
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

            dic.Add("signmsg", md5Str);
            dic.Add("open_req_src", "qlw-web");

            dic = dic.Where(t => t.Key != "key").ToDictionary(t => t.Key, t => t.Value);
            dic = dic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

            string ps = "";
            foreach (var keyValuePair in dic)
            {
                ps += keyValuePair.Key + "=" + keyValuePair.Value + "&";
            }

            ps = ps.TrimEnd('&');

            //http://open.i.nxin.com/inner/api/nxin.jrbase.yqt.bal.get/1.0?open_req_src=qlw-web&acc_no=911000010001750378&bank_type=PSBC&client_id=110020200011&signmsg=73fb3ddb05fc466b92f1a096903a93c0&times=202206091656&busi_no=NX-I-005
            var res = postMessage($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.jrbase.yqt.bal.get/1.0", ps);
            return JsonConvert.DeserializeObject<dynamic>(res);
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
