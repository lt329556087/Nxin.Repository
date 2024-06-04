using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FinanceManagement.Common;
using Newtonsoft.Json;
using FinanceManagement.ApiHost.Applications.Queries;
namespace FinanceManagement.ApiHost.Controllers.FD_Expense
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_ExpenseAddHandler : IRequestHandler<FD_ExpenseAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ExpenseRepository _repository;
        IFD_ExpenseDetailRepository _detailRepository;
        IFD_ExpenseExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Qlw_Nxin_ComContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        ApplySubjectUtil _applySubjectUtil;
        IbsfileRepository _ibsfileRepository;
        FD_ExpenseODataProvider _provider;
        private readonly string PurexpenseAppid = "1612091318520000101";//采购付款申请
        private string FormRelatedType = "201610210104402122";//表单关联关系
        public FD_ExpenseAddHandler(IIdentityService identityService, IFD_ExpenseRepository repository, IFD_ExpenseDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Qlw_Nxin_ComContext> numberCreator
            ,IBiz_ReviewRepository biz_ReviewRepository
            , IFD_ExpenseExtRepository extRepository
            ,IBiz_Related biz_RelatedRepository
            , IBiz_RelatedDetailRepository biz_RelatedDetailRepository
            , ApplySubjectUtil applySubjectUtil
            , IbsfileRepository ibsfileRepository
            , FD_ExpenseODataProvider provider)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extRepository = extRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _applySubjectUtil = applySubjectUtil;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
        }

        public async Task<Result> Handle(FD_ExpenseAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null) { result.msg = "参数不能为空";return result;}
                if (request.Lines == null) { request.Lines = new List<FD_ExpenseDetailCommand>(); }
                if (request.Extends == null) { request.Extends = new List<FD_ExpenseExtCommand>(); }
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId??"0";
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId ?? "0";
                }
                var number = _numberCreator.Create<Domain.FD_Expense>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == request.EnterpriseID);

                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_Expense()
                {
                    NumericalOrder = numericalOrder,
                    DataDate = request.DataDate,
                    Remarks = request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID,
                    Guid = Guid.NewGuid(),
                    ExpenseType =PurexpenseAppid,// String.IsNullOrEmpty(request.ExpenseType) ? "0" : request.ExpenseType,
                    ExpenseAbstract = String.IsNullOrEmpty(request.ExpenseAbstract) ? "0" : request.ExpenseAbstract,
                    ExpenseSort = String.IsNullOrEmpty(request.ExpenseSort) ? "0" : request.ExpenseSort,
                    PersonID = String.IsNullOrEmpty(request.PersonID) ? "0" : request.PersonID,
                    HouldPayDate = request.HouldPayDate,
                    PayDate = request.PayDate,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    DraweeID = String.IsNullOrEmpty(request.DraweeID) ? "0" : request.DraweeID,
                    CurrentVerificationAmount = request.CurrentVerificationAmount,
                    Pressing = request.Pressing,
                    Number = number.ToString(),
                    TicketedPointID = String.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID
                };
                var detailList = new List<FD_ExpenseDetail>();
                var extList = new List<FD_ExpenseExt>();
              
                decimal sumAmount = 0;
                foreach (var o in request.Lines)
                {
                    var numericalOrderDetail = await _numericalOrderCreator.CreateAsync();
                    if (string.IsNullOrEmpty(o.AccountName)) o.AccountName = "";
                    if (string.IsNullOrEmpty(o.BankDeposit)) o.BankDeposit = "";
                    if (string.IsNullOrEmpty(o.BankAccount)) o.BankAccount = "";
                    sumAmount+=o.Amount;
                    o.AccountInformation = o.AccountName + "^" + o.BankDeposit + "^" + o.BankAccount;
                    detailList.Add(new FD_ExpenseDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail,
                        Guid = Guid.NewGuid(),
                        ReceiptAbstractID = String.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        BusinessType = String.IsNullOrEmpty(o.BusinessType) ? "0" : o.BusinessType,
                        SettleBusinessType = String.IsNullOrEmpty(o.SettleBusinessType) ? "0" : o.SettleBusinessType,
                        CustomerID = String.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = String.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = String.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = String.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        SettlePayerID = String.IsNullOrEmpty(o.SettlePayerID) ? "0" : o.SettlePayerID,
                        Amount = o.Amount,
                        Content = o.Content,
                        AccountInformation = o.AccountInformation,
                        ReceiptAbstractDetail = String.IsNullOrEmpty(o.ReceiptAbstractDetail) ? "0" : o.ReceiptAbstractDetail
                    });
                }
               
                Biz_Review review = new Biz_Review(numericalOrder, PurexpenseAppid, request.OwnerID).SetMaking();
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(detailList);
                //await _extRepository.AddRangeAsync(extList);
                await _biz_ReviewRepository.AddAsync(review);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                //await _extRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();

                #region 附件
                List<FD_PaymentReceivables.UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion
                #region 审批主题
                var receObject = "";
                var content = "";
                if (request.Lines?.Count > 0)
                {
                    var row = request.Lines[0];
                    receObject = string.IsNullOrEmpty(row.CustomerName) ? "" : row.CustomerName;
                    content = string.IsNullOrEmpty(row.Content) ? "" : row.Content;
                }
                var houldPayDate = (domain.HouldPayDate == null ? "" : domain.HouldPayDate?.ToString("yyyy-MM-dd"));
                var subject = new SubjectInfo()
                {
                    NumericalOrder = numericalOrder,                    
                    Subject =string.Format("付款到期日: {0}，收款单位:{1},付款金额(元):{2}，付款内容:{3}", houldPayDate, receObject, sumAmount,content)
                };
                _applySubjectUtil.AddSubject(subject);
                #endregion

                #region 关联关系
                var relatedPurList = new List<BIZ_Related>();
                //采购单和申请单
                if(request.RelatedPurList!=null&& request.RelatedPurList.Any())
                {
                    foreach(var item in request.RelatedPurList)
                    {
                        var relatedPur = new BIZ_Related()
                        {
                            RelatedType = FormRelatedType,
                            ParentType = PurexpenseAppid,//采购付款申请
                            ChildType = PurexpenseAppid,//采购付款申请
                            ParentValue = numericalOrder,//采购付款申请流水号
                            ChildValue = item.RelatedNumericalOrder,
                            //ParentValueDetail = detail.NumericalOrder,
                            Remarks = "从采购付款申请引入到采购单"
                        };
                        await _biz_RelatedRepository.AddAsync(relatedPur);
                        var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                        var rData = _biz_RelatedRepository.GetList(relatedPur).FirstOrDefault();
                        //获取已付金额
                        var relateList = _provider.GetRelatedDatasAsync(PurexpenseAppid, PurexpenseAppid, "", item.RelatedNumericalOrder);
                        decimal paid = 0;
                        if (relateList?.Count > 0)
                        {
                            paid = relateList.Sum(p => p.Payment);
                        }
                        _biz_RelatedDetailRepository.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(request.OwnerID),
                            Paid = paid,//已付金额
                            Payment = item.Payment,//本次支付金额
                            Payable = item.Amount,//应付金额
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "从关联到关联详情添加"
                        });
                    }
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion

                //电子发票

                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存异常";
            }

            return result;
        }
    }
    /// <summary>
    /// 删除
    /// </summary>
    public class FD_ExpenseDeleteHandler : IRequestHandler<FD_ExpenseDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ExpenseRepository _repository;
        IFD_ExpenseDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        ApplySubjectUtil _applySubjectUtil;
        IbsfileRepository _ibsfileRepository;
        private readonly string PreexpenseAppid = "1803081320210000101";//预付款申请
        private readonly string ContractAppid = "1612030929440000101";//采购合同
        private readonly string PurexpenseAppid = "1612091318520000101";//采购付款申请
        public FD_ExpenseDeleteHandler(IIdentityService identityService, IFD_ExpenseRepository repository, IFD_ExpenseDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository
        ,IBiz_Related biz_RelatedRepository,
        IBiz_RelatedDetailRepository biz_RelatedDetailRepository,
        ApplySubjectUtil applySubjectUtil,
        IbsfileRepository ibsfileRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _applySubjectUtil = applySubjectUtil;
            _ibsfileRepository = ibsfileRepository;
        }

        public async Task<Result> Handle(FD_ExpenseDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList();
                }

                foreach (var num in list)
                {
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.ParentValue == request.NumericalOrder&&((o.ParentType == PurexpenseAppid &&o.ChildType == PurexpenseAppid)|| (o.ParentType == PreexpenseAppid && o.ChildType == ContractAppid)));
                    await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(request.NumericalOrder));
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }

            return result;
        }
    }

    public class FD_ExpenseModifyHandler : IRequestHandler<FD_ExpenseModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ExpenseRepository _repository;
        IFD_ExpenseDetailRepository _detailRepository;
        IFD_ExpenseExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        //NumberCreator<Qlw_Nxin_ComContext> _numberCreator;
        //IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        ApplySubjectUtil _applySubjectUtil;
        IbsfileRepository _ibsfileRepository;
        FD_ExpenseODataProvider _provider;
        private readonly string PurexpenseAppid = "1612091318520000101";//采购付款申请
        private string FormRelatedType = "201610210104402122";//表单关联关系
        public FD_ExpenseModifyHandler(IIdentityService identityService, IFD_ExpenseRepository repository, IFD_ExpenseDetailRepository detailRepository
           , IBiz_Related biz_RelatedRepository
            , IBiz_RelatedDetailRepository biz_RelatedDetailRepository
            , ApplySubjectUtil applySubjectUtil
            , IbsfileRepository ibsfileRepository
            , FD_ExpenseODataProvider provider, NumericalOrderCreator numericalOrderCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            //_biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository=biz_RelatedRepository;
            _biz_RelatedDetailRepository=biz_RelatedDetailRepository;
            _applySubjectUtil = applySubjectUtil;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
            _numericalOrderCreator=numericalOrderCreator;
        }

        public async Task<Result> Handle(FD_ExpenseModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null) { result.msg = "参数不能为空"; return result; }
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if(domain == null) { result.msg = "未查询到数据";return result; }

                #region 先删后增

                #region 表单
                if (request.Lines == null) { request.Lines = new List<FD_ExpenseDetailCommand>(); }
                if (request.Extends == null) { request.Extends = new List<FD_ExpenseExtCommand>(); }
                var detailList = new List<FD_ExpenseDetail>();
                decimal sumAmount = 0;
                var numericalOrder = request.NumericalOrder;
                foreach (var o in request.Lines)
                {
                    var numericalOrderDetail = _numericalOrderCreator.Create();
                    if (string.IsNullOrEmpty(o.AccountName)) o.AccountName = "";
                    if (string.IsNullOrEmpty(o.BankDeposit)) o.BankDeposit = "";
                    if (string.IsNullOrEmpty(o.BankAccount)) o.BankAccount = "";
                    sumAmount += o.Amount;
                    o.AccountInformation = o.AccountName + "^" + o.BankDeposit + "^" + o.BankAccount;
                    detailList.Add(new FD_ExpenseDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail,
                        Guid = Guid.NewGuid(),
                        ReceiptAbstractID = String.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        BusinessType = String.IsNullOrEmpty(o.BusinessType) ? "0" : o.BusinessType,
                        SettleBusinessType = String.IsNullOrEmpty(o.SettleBusinessType) ? "0" : o.SettleBusinessType,
                        CustomerID = String.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = String.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = String.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = String.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        SettlePayerID = String.IsNullOrEmpty(o.SettlePayerID) ? "0" : o.SettlePayerID,
                        Amount = o.Amount,
                        Content = o.Content,
                        AccountInformation = o.AccountInformation,
                        ReceiptAbstractDetail = String.IsNullOrEmpty(o.ReceiptAbstractDetail) ? "0" : o.ReceiptAbstractDetail
                    });
                }
                domain.UpdateDetail(request.DataDate, request.Remarks, request.ExpenseAbstract, request.ExpenseSort, request.PersonID, request.HouldPayDate, request.PayDate, request.StartDate, request.EndDate, request.DraweeID, request.CurrentVerificationAmount, request.Pressing, request.TicketedPointID);
                #endregion
                await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.ParentValue == request.NumericalOrder && ((o.ParentType == PurexpenseAppid && o.ChildType == PurexpenseAppid)));
                await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(request.NumericalOrder));
                await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                #endregion
                #region 增加                
                await _detailRepository.AddRangeAsync(detailList);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();

                #region 附件
                List<FD_PaymentReceivables.UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion
                #region 审批主题
                //var receObject = "";
                //var content = "";
                //if (request.Lines?.Count > 0)
                //{
                //    var row = request.Lines[0];
                //    receObject = string.IsNullOrEmpty(row.CustomerName) ? "" : row.CustomerName;
                //    content = string.IsNullOrEmpty(row.Content) ? "" : row.Content;
                //}
                //var houldPayDate = (domain.HouldPayDate == null ? "" : domain.HouldPayDate?.ToString("yyyy-MM-dd"));
                //var subject = new SubjectInfo()
                //{
                //    NumericalOrder = numericalOrder,
                //    Subject = string.Format("付款到期日: {0}，收款单位:{1},付款金额(元):{2}，付款内容:{3}", houldPayDate, receObject, sumAmount, content)
                //};
                //_applySubjectUtil.AddSubject(subject);
                #endregion

                #region 关联关系
                var relatedPurList = new List<BIZ_Related>();
                //采购单和申请单
                if (request.RelatedPurList != null && request.RelatedPurList.Any())
                {
                    foreach (var item in request.RelatedPurList)
                    {
                        var relatedPur = new BIZ_Related()
                        {
                            RelatedType = FormRelatedType,
                            ParentType = PurexpenseAppid,//采购付款申请
                            ChildType = PurexpenseAppid,//采购付款申请
                            ParentValue = numericalOrder,//采购付款申请流水号
                            ChildValue = item.RelatedNumericalOrder,
                            Remarks = "从采购付款申请引入到采购单"
                        };
                        await _biz_RelatedRepository.AddAsync(relatedPur);
                        var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                        var rData = _biz_RelatedRepository.GetList(relatedPur).FirstOrDefault();
                        //获取已付金额
                        var relateList = _provider.GetRelatedDatasAsync(PurexpenseAppid, PurexpenseAppid, "", item.RelatedNumericalOrder);
                        decimal paid = 0;
                        if (relateList?.Count > 0)
                        {
                            paid = relateList.Sum(p => p.Payment);
                        }
                        _biz_RelatedDetailRepository.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(request.OwnerID),
                            Paid = paid,//已付金额
                            Payment = item.Payment,//本次支付金额
                            Payable = item.Amount,//应付金额
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "从关联到关联详情添加"
                        });
                    }
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion

                //电子发票
                
                #endregion

                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }


    #region 预付款申请

    /// <summary>
    /// 预付款申请增加
    /// </summary>
    public class FD_ExpensePreAddHandler : IRequestHandler<FD_ExpensePreAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ExpenseRepository _repository;
        IFD_ExpenseDetailRepository _detailRepository;
        IFD_ExpenseExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Qlw_Nxin_ComContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        ApplySubjectUtil _applySubjectUtil;
        IbsfileRepository _ibsfileRepository;
        FD_ExpenseODataProvider _provider;
        private readonly string PreexpenseAppid = "1803081320210000101";//预付款申请
        private readonly string ContractAppid = "1612030929440000101";//采购合同
        private string FormRelatedType = "201610210104402122";//表单关联关系

        public FD_ExpensePreAddHandler(IIdentityService identityService, IFD_ExpenseRepository repository, IFD_ExpenseDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Qlw_Nxin_ComContext> numberCreator
            , IBiz_ReviewRepository biz_ReviewRepository
            , IFD_ExpenseExtRepository extRepository
            , IBiz_Related biz_RelatedRepository
            , IBiz_RelatedDetailRepository biz_RelatedDetailRepository
            , ApplySubjectUtil applySubjectUtil
            , IbsfileRepository ibsfileRepository
            , FD_ExpenseODataProvider provider)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extRepository = extRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _applySubjectUtil = applySubjectUtil;
            _ibsfileRepository = ibsfileRepository;
            _provider=provider;
        }

        public async Task<Result> Handle(FD_ExpensePreAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null) { result.msg = "参数不能为空"; return result; }
                if (request.Lines == null) { request.Lines = new List<FD_ExpenseDetailCommand>(); }
                if (request.Extends == null) { request.Extends = new List<FD_ExpenseExtCommand>(); }
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId ?? "0";
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId ?? "0";
                }
                #region 表单
                var number = _numberCreator.Create<Domain.FD_Expense>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == request.EnterpriseID);

                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_Expense()
                {
                    NumericalOrder = numericalOrder,
                    DataDate = request.DataDate,
                    Remarks = request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID,
                    Guid = Guid.NewGuid(),
                    ExpenseType = PreexpenseAppid,// String.IsNullOrEmpty(request.ExpenseType) ? "0" : request.ExpenseType,
                    ExpenseAbstract = String.IsNullOrEmpty(request.ExpenseAbstract) ? "0" : request.ExpenseAbstract,
                    ExpenseSort = String.IsNullOrEmpty(request.ExpenseSort) ? "0" : request.ExpenseSort,
                    PersonID = String.IsNullOrEmpty(request.PersonID) ? "0" : request.PersonID,
                    HouldPayDate = request.HouldPayDate,
                    PayDate = request.PayDate,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    DraweeID = String.IsNullOrEmpty(request.DraweeID) ? "0" : request.DraweeID,
                    CurrentVerificationAmount = request.CurrentVerificationAmount,
                    Pressing = request.Pressing,
                    Number = number.ToString(),
                    TicketedPointID = String.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID
                };
                var detailList = new List<FD_ExpenseDetail>();
                var extList = new List<FD_ExpenseExt>();
                decimal sumAmount = 0;
                foreach (var o in request.Lines)
                {
                    var numericalOrderDetail = await _numericalOrderCreator.CreateAsync();
                    if (string.IsNullOrEmpty(o.AccountName)) o.AccountName = "";
                    if (string.IsNullOrEmpty(o.BankDeposit)) o.BankDeposit = "";
                    if (string.IsNullOrEmpty(o.BankAccount)) o.BankAccount = "";
                    sumAmount += o.Amount;
                    o.AccountInformation = "账户名称:" + o.AccountName+ ",开户行:"+ o.BankDeposit + ",银行账号:" + o.BankAccount;//格式同支出申请
                    detailList.Add(new FD_ExpenseDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail,
                        Guid = Guid.NewGuid(),
                        ReceiptAbstractID = String.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        BusinessType = String.IsNullOrEmpty(o.BusinessType) ? "0" : o.BusinessType,
                        SettleBusinessType = String.IsNullOrEmpty(o.SettleBusinessType) ? "0" : o.SettleBusinessType,
                        CustomerID = String.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = String.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = String.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = String.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        SettlePayerID = String.IsNullOrEmpty(o.SettlePayerID) ? "0" : o.SettlePayerID,
                        Amount = o.Amount,
                        Content = o.Content,
                        AccountInformation = o.AccountInformation,
                        ReceiptAbstractDetail = String.IsNullOrEmpty(o.ReceiptAbstractDetail) ? "0" : o.ReceiptAbstractDetail
                    });
                }

                Biz_Review review = new Biz_Review(numericalOrder, PreexpenseAppid, request.OwnerID).SetMaking();
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(detailList);
                await _biz_ReviewRepository.AddAsync(review);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                #region 附件
                List<FD_PaymentReceivables.UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion

                #region 审批主题
                var receObject = "";
                var content = "";
                if (request.Lines?.Count > 0)
                {
                    var row = request.Lines[0];
                    receObject =string.IsNullOrEmpty(row.CustomerName)?"": row.CustomerName;
                    content = string.IsNullOrEmpty(row.Content) ? "" : row.Content;
                }
                var houldPayDate = (domain.HouldPayDate == null ? "" : domain.HouldPayDate?.ToString("yyyy-MM-dd"));
                var subject = new SubjectInfo()
                {
                    NumericalOrder = numericalOrder,
                    Subject = string.Format("付款到期日: {0}，收款单位:{1},付款金额(元):{2}，付款内容:{3}", houldPayDate, receObject, sumAmount, content)
                };
                _applySubjectUtil.AddSubject(subject);
                #endregion

                #region 关联关系
                var relatedPurList = new List<BIZ_Related>();
                //采购单和申请单
                if (request.RelatedPurList != null && request.RelatedPurList.Any())
                {
                    foreach (var item in request.RelatedPurList)
                    {
                        var relatedPur = new BIZ_Related()
                        {
                            RelatedType = FormRelatedType,
                            ParentType = PreexpenseAppid,//采购付款申请
                            ChildType = ContractAppid,//采购合同
                            ParentValue = numericalOrder,//采购付款申请流水号
                            ChildValue = item.RelatedNumericalOrder,
                            Remarks = "预付款申请单"
                        };
                        await _biz_RelatedRepository.AddAsync(relatedPur);
                        var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                        var rData = _biz_RelatedRepository.GetList(relatedPur).FirstOrDefault();
                        //获取已付金额
                        var relateList = _provider.GetRelatedDatasAsync(PreexpenseAppid, ContractAppid, "", item.RelatedNumericalOrder);
                        decimal paid = 0;
                        if (relateList?.Count > 0)
                        {
                            paid = relateList.Sum(p => p.Payment);
                        }
                        _biz_RelatedDetailRepository.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(request.OwnerID),
                            Paid = paid,//已付金额
                            Payment = item.Payment,//本次支付金额
                            Payable = item.Amount,//应付金额
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "预付款申请引采购合同"
                        });
                    }
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion                

                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存异常";
            }

            return result;
        }
    }

    /// <summary>
    /// 预付款申请修改
    /// </summary>
    public class FD_ExpensePreModifyHandler : IRequestHandler<FD_ExpensePreModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ExpenseRepository _repository;
        IFD_ExpenseDetailRepository _detailRepository;
        IFD_ExpenseExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Qlw_Nxin_ComContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        ApplySubjectUtil _applySubjectUtil;
        IbsfileRepository _ibsfileRepository;
        FD_ExpenseODataProvider _provider;
        private readonly string PreexpenseAppid = "1803081320210000101";//预付款申请
        private readonly string ContractAppid = "1612030929440000101";//采购合同
        private string FormRelatedType = "201610210104402122";//表单关联关系
        public FD_ExpensePreModifyHandler(IIdentityService identityService, IFD_ExpenseRepository repository, IFD_ExpenseDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository
            ,NumericalOrderCreator numericalOrderCreator
            ,NumberCreator<Qlw_Nxin_ComContext> numberCreator
            , IBiz_Related biz_RelatedRepository
            , IBiz_RelatedDetailRepository biz_RelatedDetailRepository
            , ApplySubjectUtil applySubjectUtil
            , IbsfileRepository ibsfileRepository
            , FD_ExpenseODataProvider provider)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _applySubjectUtil = applySubjectUtil;
            _ibsfileRepository = ibsfileRepository;
            _numericalOrderCreator=numericalOrderCreator;
            _numberCreator=numberCreator;
            _provider=provider;
        }

        public async Task<Result> Handle(FD_ExpensePreModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null) { result.msg = "参数不能为空"; return result; }
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (domain == null) { result.msg = "未查询到数据"; return result; }

                #region 先删后增
                #region 表单               
                if (request.Lines == null) { request.Lines = new List<FD_ExpenseDetailCommand>(); }
                if (request.Extends == null) { request.Extends = new List<FD_ExpenseExtCommand>(); }
                var detailList = new List<FD_ExpenseDetail>();
                var extList = new List<FD_ExpenseExt>();
                decimal sumAmount = 0;
                var numericalOrder = request.NumericalOrder;
                foreach (var o in request.Lines)
                {
                    var numericalOrderDetail = await _numericalOrderCreator.CreateAsync();
                    if (string.IsNullOrEmpty(o.AccountName)) o.AccountName = "";
                    if (string.IsNullOrEmpty(o.BankDeposit)) o.BankDeposit = "";
                    if (string.IsNullOrEmpty(o.BankAccount)) o.BankAccount = "";
                    sumAmount += o.Amount;
                    o.AccountInformation = "账户名称:" + o.AccountName + ",开户行:" + o.BankDeposit + ",银行账号:" + o.BankAccount;//格式同支出申请
                    detailList.Add(new FD_ExpenseDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail,
                        Guid = Guid.NewGuid(),
                        ReceiptAbstractID = String.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        BusinessType = String.IsNullOrEmpty(o.BusinessType) ? "0" : o.BusinessType,
                        SettleBusinessType = String.IsNullOrEmpty(o.SettleBusinessType) ? "0" : o.SettleBusinessType,
                        CustomerID = String.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = String.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = String.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = String.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        SettlePayerID = String.IsNullOrEmpty(o.SettlePayerID) ? "0" : o.SettlePayerID,
                        Amount = o.Amount,
                        Content = o.Content,
                        AccountInformation = o.AccountInformation,
                        ReceiptAbstractDetail = String.IsNullOrEmpty(o.ReceiptAbstractDetail) ? "0" : o.ReceiptAbstractDetail
                    });
                }

                domain.UpdateDetail(request.DataDate, request.Remarks, request.ExpenseAbstract, request.ExpenseSort, request.PersonID, request.HouldPayDate, request.PayDate, request.StartDate, request.EndDate, request.DraweeID, request.CurrentVerificationAmount, request.Pressing, request.TicketedPointID);

                #endregion

                await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.ParentValue == request.NumericalOrder && ((o.ParentType == PreexpenseAppid && o.ChildType == ContractAppid)));
                await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(request.NumericalOrder));
                await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                #endregion
                #region 增加
                

                #region 表单保存 
                
                await _detailRepository.AddRangeAsync(detailList);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                #region 附件
                List<FD_PaymentReceivables.UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion

                #region 审批主题
                //var receObject = "";
                //var content = "";
                //if (request.Lines?.Count > 0)
                //{
                //    var row = request.Lines[0];
                //    receObject = string.IsNullOrEmpty(row.CustomerName) ? "" : row.CustomerName;
                //    content = string.IsNullOrEmpty(row.Content) ? "" : row.Content;
                //}
                //var houldPayDate = (domain.HouldPayDate == null ? "" : domain.HouldPayDate?.ToString("yyyy-MM-dd"));
                //var subject = new SubjectInfo()
                //{
                //    NumericalOrder = numericalOrder,
                //    Subject = string.Format("付款到期日: {0}，收款单位:{1},付款金额(元):{2}，付款内容:{3}", houldPayDate, receObject, sumAmount, content)
                //};
                //_applySubjectUtil.AddSubject(subject);
                #endregion

                #region 关联关系
                var relatedPurList = new List<BIZ_Related>();
                //采购单和申请单
                if (request.RelatedPurList != null && request.RelatedPurList.Any())
                {
                    foreach (var item in request.RelatedPurList)
                    {
                        var relatedPur = new BIZ_Related()
                        {
                            RelatedType = FormRelatedType,
                            ParentType = PreexpenseAppid,//采购付款申请
                            ChildType = ContractAppid,//采购合同
                            ParentValue = numericalOrder,//采购付款申请流水号
                            ChildValue = item.RelatedNumericalOrder,
                            Remarks = "预付款申请单"
                        };
                        await _biz_RelatedRepository.AddAsync(relatedPur);
                        var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                        var rData = _biz_RelatedRepository.GetList(relatedPur).FirstOrDefault();
                        //获取已付金额
                        var relateList = _provider.GetRelatedDatasAsync(PreexpenseAppid, ContractAppid, "", item.RelatedNumericalOrder);
                        decimal paid = 0;
                        if(relateList?.Count > 0)
                        {
                            paid = relateList.Sum(p => p.Payment);
                        }
                        _biz_RelatedDetailRepository.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(request.OwnerID),
                            Paid = paid,//已付金额
                            Payment = item.Payment,//本次支付金额
                            Payable = item.Amount,//应付金额
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "预付款申请引采购合同"
                        });
                    }
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion                

                #endregion

                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }
    #endregion
}
