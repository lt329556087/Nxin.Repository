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
    public class FD_ReceivablesController : ControllerBase
    {
        IMediator _mediator;
        //FD_PaymentReceivablesTODataProvider _provider;
        FundSummaryUtil _fundSummaryUtil;
        IIdentityService _identityService;
        FD_PaymentReceivablesTODataProvider _prodiver;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FD_ReceivablesController> _logger;
        IFD_ReceivablesExtendRepository _paymentExtendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        Ifd_receivablessetRepository _ReceivablessetRepository;
        Ifd_bankreceivableRepository _ifd_Bankreceivable;
        IFD_AccountRepository _fD_AccountRepository;
        FD_AccountODataProvider _AccountODataProvider;
        public FD_ReceivablesController(FD_AccountODataProvider AccountODataProvider,IFD_AccountRepository fD_AccountRepository,Ifd_bankreceivableRepository ifd_Bankreceivable,Ifd_receivablessetRepository ReceivablessetRepository,IMediator mediator, HttpClientUtil httpClientUtil,
            //FD_PaymentReceivablesTODataProvider provider, 
            FundSummaryUtil fundSummaryUtil, IFD_ReceivablesExtendRepository paymentExtendRepository, NumericalOrderCreator numericalOrderCreator, IIdentityService identityService, FD_PaymentReceivablesTODataProvider prodiver, ILogger<FD_ReceivablesController> logger, HostConfiguration hostCongfiguration)
        {
            _ifd_Bankreceivable = ifd_Bankreceivable;
            _mediator = mediator;
            //_provider = provider;
            _fundSummaryUtil = fundSummaryUtil;
            _identityService = identityService;
            _prodiver = prodiver;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _paymentExtendRepository = paymentExtendRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _ReceivablessetRepository = ReceivablessetRepository;
            _fD_AccountRepository = fD_AccountRepository;
            _AccountODataProvider = AccountODataProvider;
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
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("GetListByKingDee")]
        public List<FD_PaymentReceivablesEntity> GetListByKingDee(string entes,string beginDate,string endDate)
        {
            var list = _prodiver.GetReceivablesMergeDatasByKingDee(entes,beginDate,endDate).ToList();
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.CollectionName))
                {
                    continue;
                }
                var temp = item.CollectionName.Split(',');
                temp = temp.Distinct().ToArray();
                item.CollectionName = string.Join(',', temp);
            }
            var payList = _prodiver.GetMergeDatasByKingDee(entes, beginDate, endDate).ToList();
            list.AddRange(payList);
            foreach (var item in list)
            {
                //收款单
                if (item.SettleReceipType == "201611180104402201")
                {
                    item.Detail = GetDetail(Convert.ToInt64(item.NumericalOrder)).data;
                }
                ////收款汇总单
                //else if (item.SettleReceipType == "201611180104402203")
                //{
                //    item.Details = GetSummaryDetail(Convert.ToInt64(item.NumericalOrder)).data;
                //}
                ////付款汇总
                //else if (item.SettleReceipType == "201611180104402204")
                //{
                //    item.Details = GetPayDetail(Convert.ToInt64(item.NumericalOrder)).data;
                //}
                //付款单
                else if (item.SettleReceipType == "201611180104402202")
                {
                    item.Detail = GetPayDetail(Convert.ToInt64(item.NumericalOrder)).data;
                }
            }
            return list;
        }
        /// <summary>
        /// 付款管理（结算方式）
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetFundPayReceivablesPayTypeData")]
        public List<FundReceivablesData> GetFundPayReceivablesPayTypeData(FundsParameter model)
        {
            var data = _prodiver.GetFundPayReceivablesPayTypeData(model.EnterpriseID, model.BeginDate, model.EndDate).OrderByDescending(m => m.Amount).ToList();
            //此摘要收款金额 / 所有摘要总收款金额
            foreach (var item in data)
            {
                item.Ratio = Math.Round(item.Amount / data.Sum(m => m.Amount),4);
            }
            return data;
        }
        /// <summary>
        /// 资金主页-余额管理-个数
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBankInfoCount")]
        public List<BankInfoCount> GetBankInfoCount(FundsParameter model)
        {
            return _prodiver.GetBankInfoCount(model.EnterpriseID).ToList();
        }
        /// <summary>
        /// 资金主页 资金归集 向上 向下（资金下拨）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCashSweepInfo")]
        public List<CashSweepInfo> GetCashSweepInfo(FundsParameter model)
        {
            return _prodiver.GetCashSweepInfo(model.EnterpriseID, model.BeginDate, model.EndDate).ToList();
        }
        /// <summary>
        /// 付款管理
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetFundPayReceivablesData")]
        public List<FundReceivablesData> GetFundPayReceivablesData(FundsParameter model)
        {
            var data = _prodiver.GetFundPayReceivablesData(model.EnterpriseID, model.BeginDate, model.EndDate).OrderByDescending(m => m.Amount).ToList();
            //此摘要收款金额 / 所有摘要总收款金额
            foreach (var item in data)
            {
                item.Ratio = item.Amount / data.Sum(m => m.Amount);
            }
            return data;
        }
        /// <summary>
        /// 收款管理
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetFundReceivablesData")]
        public List<FundReceivablesData> GetFundReceivablesData(FundsParameter model)
        {
            var data = _prodiver.GetFundReceivablesData(model.EnterpriseID, model.BeginDate, model.EndDate).OrderByDescending(m=>m.Amount).ToList();
            //此摘要收款金额 / 所有摘要总收款金额
            foreach (var item in data)
            {
                item.Ratio = item.Amount / data.Sum(m => m.Amount);
            }
            return data;
        }
        /// <summary>
        /// 收款关联（客户）
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetFundReceivablesCusData")]
        public List<FundReceivablesCusData> GetFundReceivablesCusData(FundsParameter model)
        {
            return _prodiver.GetFundReceivablesCusData(model.EnterpriseID, model.BeginDate, model.EndDate).OrderByDescending(m => m.Amount).ToList();
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [NonAction]
        public Result GetPayDetail(long key)
        {
            var result = new Result();
            var domain = new FD_PaymentDoMain();
            var data = _prodiver.GetDataAsync(key).Result;
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
            domain.extend = _prodiver.GetExtend(key.ToString());
            //银行卡号解密
            foreach (var item in domain.extend)
            {
                var resultRece = AccountNumberDecrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            domain.details = _prodiver.GetDetaiDatasAsync(key).Result;
            domain.ApplyNumericalOrder = data.ApplyNumericalOrder;
            domain.ApplyAppId = data.ApplyAppId;
            result.data = domain;
            result.code = 0;
            return result;
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [NonAction]
        public Result GetPaySummaryDetail(long key)
        {
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
                UploadInfo = data.UploadInfo,
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
                extend = _prodiver.GetPaymentExtendSummaryDatas(key),
                details = details
            };
            return result;
        }
        //增加
        [HttpPost]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Add([FromBody] FD_ReceivablesAddCommand request)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in request.extend)
            {
                var resultRece = encryptAccount.AccountNumberEncrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //上传附件
        [HttpPost]
        [PermissionAuthorize(Permission.Making)]
        [Route("UpdateUpInfo")]
        public async Task<Result> UpdateUpInfo([FromBody] FD_ReceivablesUpInfoCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }
        //增加
        [HttpPost]
        [Route("AddSummary")]
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> AddSummary([FromBody] FD_ReceivablesSummaryAddCommand request)
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
        public async Task<Result> Delete([FromBody] FD_ReceivablesDeleteCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        //修改
        [HttpPut]   
        [PermissionAuthorize(Permission.Making)]
        public async Task<Result> Modify([FromBody] FD_ReceivablesModifyCommand request)
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
        public async Task<Result> Modify([FromBody] FD_ReceivablesSummaryModifyCommand request)
        {
            EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);
            foreach (var item in request.details)
            {
                var resultRece = encryptAccount.AccountNumberEncrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            return await _mediator.Send(request, HttpContext.RequestAborted);
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
                var domain = new FD_PaymentDoMain();
                var data = _prodiver.GetReceivablesDataAsync(key).Result;
                if (data == null)
                {
                    return new Result() { code = 404, data = null, msg = "无数据" };
                }
                var related = _prodiver.GetApplyNumericalorders(key).Result;
                if (related != null)
                {
                    domain.ApplyNumericalOrder = related.EnterpriseID;
                }
                domain.UploadInfo = data.UploadInfo;
                domain.ApplyAppId = data.ApplyAppId;
                domain.Remarks = data.Remarks;
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
                domain.extend = _prodiver.GetExtend(key.ToString());
                domain.Amount = domain.extend.FirstOrDefault().Amount;
                domain.BankUrl = data.BankUrl;
                domain.VoucherNumber = data.VoucherNumber;
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
                //银行卡号解密
                foreach (var item in domain.extend)
                {
                    var resultRece = AccountNumberDecrypt(item.BankAccount);
                    item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
                }
                domain.details = _prodiver.GetReceivablesDetaiDatasAsync(key).Result;
                domain.RelatedList = _prodiver.GetSalesRelatedList(key);

                var tempCost = _prodiver.GetReceivablesDetaiByCost(key).Result;
                foreach (var item in domain.details)
                {
                    if (item.Charges > 0)
                    {
                        var temp = tempCost.Where(m => m.Guid == item.Guid && m.Charges == item.Charges).FirstOrDefault();
                        if (temp != null)
                        {
                            item.CostAccoSubjectID = temp.AccoSubjectID;
                        }
                    }
                }
                result.data = domain;
                result.code = 0;
                return result;
            }
            catch (Exception e)
            {

                _logger.LogError(JsonConvert.SerializeObject(e));
                return new Result() {code = ErrorCode.RequestArgumentError.GetIntValue(),msg = "查询详情操作异常" };
            }
        }
        /// <summary>
        /// 详情页查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSummaryDetail")]
        public Result GetSummaryDetail(long key)
        {
            var result = new Result() { code = 0 };
            var data = _prodiver.GetSummaryDataAsync(key).Result;
            if (data == null)
            {
                return new Result() { code = 404, data = null, msg = "无数据" };
            }
            
            var details = _prodiver.GetReceivablesSummaryDetaiDatasAsync(key).Result;
            var realtedList = _prodiver.GetSalesRelatedList(key);
            foreach (var item in details)
            {
                item.RelatedList.AddRange(realtedList.Where(m => m.RecordId == item.RecordID.ToString()));
            }
            //获取费用科目
            var tempCost = _prodiver.GetReceivablesSummaryDetaiByCost(key).Result;
            var extend = _prodiver.GetPaymentExtendSummaryDatas(key);
            foreach (var item in extend)
            {
                var cost = tempCost.Where(m => m.AccountID == item.AccountID && m.Charges == item.Charges && m.Amount == item.Amount).FirstOrDefault();
                if (cost != null)
                {
                    item.CostAccoSubjectID = cost.AccoSubjectID;
                }
            }
            foreach (var item in details)
            {
                var resultRece = AccountNumberDecrypt(item.BankAccount);
                item.BankAccount = resultRece?.Item1 == true ? resultRece.Item2 : item.BankAccount;
            }
            var related = _prodiver.GetApplyNumericalorders(key).Result;
            data.ApplyNumericalOrder = related == null ? "" : related.EnterpriseID;
            result.data = new
            {
                data.BankUrl,
                data.BusinessType,
                data.BusinessTypeName,
                UploadInfo = data.UploadInfo,
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
                extend = extend,
                details = details,
                data.VoucherNumber,
            };
            return result;
        }
        /// <summary>
        /// 收款设置保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveSetAsync")]
        public async Task<Result> SaveSetAsync(fd_receivablesset model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.CostAccoSubjectID))
                {
                    model.CostAccoSubjectID = "0";
                }
                if (model.IsDelete)
                {
                    _ReceivablessetRepository.Remove(model);
                    await _ReceivablessetRepository.UnitOfWork.SaveChangesAsync();
                    return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "删除成功" };
                }
                else if (string.IsNullOrEmpty(model.NumericalOrder))
                {
                    if (string.IsNullOrEmpty(model.OwnerID))
                    {
                        model.OwnerID = _identityService.UserId;
                    }
                    var temp = GetReceivablesSet();
                    if (temp != null)
                    {
                        return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(),msg = "当前单位已存在数据,刷新查看最新数据" };
                    }
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.EnterpriseID = _identityService.EnterpriseId;
                    model.NumericalOrder = _numericalOrderCreator.CreateAsync().Result;
                    _ReceivablessetRepository.Add(model);
                    await _ReceivablessetRepository.UnitOfWork.SaveChangesAsync();
                    return new Result() { code = ErrorCode.Success.GetIntValue(),data = model.NumericalOrder, msg = "新增成功" };
                }
                else
                {
                    if (string.IsNullOrEmpty(model.OwnerID))
                    {
                        model.OwnerID = _identityService.UserId;
                    }
                    model.ModifiedDate = DateTime.Now;
                    _ReceivablessetRepository.Update(model);
                    await _ReceivablessetRepository.UnitOfWork.SaveChangesAsync();
                    return new Result() { code = ErrorCode.Success.GetIntValue(),data = model.NumericalOrder, msg = "修改成功" };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), msg = "保存失败" };
            }
        }
        /// <summary>
        /// 批量收款设置保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveSetAsyncs")]
        public async Task<Result> SaveSetAsyncs(List<fd_receivablesset> list)
        {
            try
            {
                Result result = new Result();
                if (list.Count > 0)
                {
                    foreach (var model in list)
                    {
                        if (string.IsNullOrEmpty(model.CostAccoSubjectID))
                        {
                            model.CostAccoSubjectID = "0";
                        }
                        if (string.IsNullOrEmpty(model.DebitAccoSubjectID))
                        {
                            model.DebitAccoSubjectID = "0";
                        }
                        if (string.IsNullOrEmpty(model.AccountID))
                        {
                            model.AccountID = "0";
                        }
                        if (string.IsNullOrEmpty(model.PaymentType))
                        {
                            model.PaymentType = "0";
                        }
                        if (string.IsNullOrEmpty(model.SettleReceipType))
                        {
                            model.SettleReceipType = "0";
                        }
                        if (model.IsDelete)
                        {
                            _ReceivablessetRepository.Remove(model);
                            await _ReceivablessetRepository.UnitOfWork.SaveChangesAsync();
                            return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "删除成功" };
                        }
                        else if (string.IsNullOrEmpty(model.NumericalOrder))
                        {
                            if (string.IsNullOrEmpty(model.OwnerID))
                            {
                                model.OwnerID = _identityService.UserId;
                            }
                            var temp = GetReceivablesSetList().FirstOrDefault(m=>m.TemplateName == model.TemplateName);
                            if (temp != null)
                            {
                                result.msg += $"模板名称：【{model.TemplateName}】当前单位已存在数据,刷新查看最新数据\n";
                            }
                            model.CreatedDate = DateTime.Now;
                            model.ModifiedDate = DateTime.Now;
                            model.EnterpriseID = _identityService.EnterpriseId;
                            model.NumericalOrder = _numericalOrderCreator.CreateAsync().Result;
                            _ReceivablessetRepository.Add(model);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(model.OwnerID))
                            {
                                model.OwnerID = _identityService.UserId;
                            }
                            model.ModifiedDate = DateTime.Now;
                            _ReceivablessetRepository.Update(model);
                        }
                    }
                    if (string.IsNullOrEmpty(result.msg))
                    {
                        result.code = ErrorCode.Success.GetIntValue();
                        result.msg = "保存成功";
                        await _ReceivablessetRepository.UnitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), msg = "保存失败" };
            }
        }
        /// <summary>
        /// 收款设置查询
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReceivablesSet")]
        public fd_receivablesset GetReceivablesSet(string EnterpriseId="")
        {
            if (string.IsNullOrEmpty(EnterpriseId))
            {
                EnterpriseId = _identityService.EnterpriseId;
            }
            return _ReceivablessetRepository.GetDataByEnterpriseId(EnterpriseId);
        }
        /// <summary>
        /// 收款设置查询
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReceivablesSetList")]
        public List<fd_receivablesset> GetReceivablesSetList(string EnterpriseId = "")
        {
            if (string.IsNullOrEmpty(EnterpriseId))
            {
                EnterpriseId = _identityService.EnterpriseId;
            }
            return _ReceivablessetRepository.GetListByEnterpriseId(EnterpriseId);
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
    }
}
