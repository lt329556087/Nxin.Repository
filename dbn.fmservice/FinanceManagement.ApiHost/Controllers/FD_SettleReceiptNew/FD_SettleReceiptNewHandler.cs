using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Architecture.Common.Application.Commands;
using FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables;
using Newtonsoft.Json;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface;
using FinanceManagement.Common;
using Architecture.Common.Util;
using Microsoft.Extensions.Logging;
using Architecture.Common.HttpClientUtil;
using FinanceManagement.Common.MakeVoucherCommon;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceiptNew
{
    public class FD_SettleReceiptNewFundAddHandler : IRequestHandler<FD_SettleReceiptNewFundAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        private TreeModelODataProvider _treeModel;
        Common.FMBaseCommon _baseCommon;
        IbsfileRepository _ibsfileRepository;
        FD_SettleReceiptNewODataProvider _provider;
        IBiz_Related _biz_RelatedRepository;
        EnterprisePeriodUtil _enterprisePeriodUtil;

        public FD_SettleReceiptNewFundAddHandler(EnterprisePeriodUtil enterprisePeriodUtil, IBiz_Related related, IbsfileRepository ibsfileRepository, FD_SettleReceiptNewODataProvider provider, IIdentityService identityService, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, Common.FMBaseCommon baseCommon)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
            _baseCommon = baseCommon;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
            _biz_RelatedRepository = related;
            _enterprisePeriodUtil = enterprisePeriodUtil;
        }
        public async Task<Result> Handle(FD_SettleReceiptNewFundAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                result = ValidParam(request);
                if (!string.IsNullOrEmpty(result.msg))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                long number = 0;
                
                number = Convert.ToInt64(_provider.GetMaxNumberByDate(request.SettleReceipType, request.EnterpriseID, request.BeginDate, request.EndDate, request.TicketedPointID).MaxNumber);

                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                List<string> nums = new List<string>();

                var domain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    SettleReceipType = request.SettleReceipType,
                    DataDate = Convert.ToDateTime(request.DataDate),
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    Number = number.ToString(),
                    AccountNo = request.AccountNo,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId ?? request.OwnerID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    details = new List<FD_SettleReceiptDetail>()
                };
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? request.EnterpriseID : o.EnterpriseID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) || o.ReceiptAbstractID == "无数据" ? "0" : o.ReceiptAbstractID,
                        AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) || o.AccoSubjectID == "无数据" ? "0" : o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) || o.CustomerID == "无数据" ? "0" : o.CustomerID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) || o.PersonID == "无数据" ? "0" : o.PersonID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) || o.MarketID == "无数据" ? "0" : o.MarketID,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) || o.ProjectID == "无数据" ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) || o.ProductID == "无数据" ? "0" : o.ProductID,
                        ProductGroupID = string.IsNullOrEmpty(o.ProductGroupID) || o.ProductGroupID == "无数据" ? "0" : o.ProductGroupID,
                        ClassificationID = string.IsNullOrEmpty(o.ClassificationID) || o.ClassificationID == "无数据" ? "0" : o.ClassificationID,
                        PaymentTypeID = string.IsNullOrEmpty(o.PaymentTypeID) || o.PaymentTypeID == "无数据" ? "0" : o.PaymentTypeID,
                        AccountID = string.IsNullOrEmpty(o.AccountID) || o.AccountID == "无数据" ? "0" : o.AccountID,
                        LorR = o.LorR,
                        Credit = o.Credit,
                        Debit = o.Debit,
                        Content = o.Content,
                        RowNum = o.RowNum,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) || o.OrganizationSortID == "无数据" ? "0" : o.OrganizationSortID,
                        IsCharges = false,
                        Auxiliary1 = string.IsNullOrEmpty(o.Auxiliary1) ? "0" : o.Auxiliary1,
                        Auxiliary2 = string.IsNullOrEmpty(o.Auxiliary2) ? "0" : o.Auxiliary2,
                        Auxiliary3 = string.IsNullOrEmpty(o.Auxiliary3) ? "0" : o.Auxiliary3,
                        Auxiliary4 = string.IsNullOrEmpty(o.Auxiliary4) ? "0" : o.Auxiliary4,
                        Auxiliary5 = string.IsNullOrEmpty(o.Auxiliary5) ? "0" : o.Auxiliary5,
                        Auxiliary6 = string.IsNullOrEmpty(o.Auxiliary6) ? "0" : o.Auxiliary6,
                        Auxiliary7 = string.IsNullOrEmpty(o.Auxiliary7) ? "0" : o.Auxiliary7,
                        Auxiliary8 = string.IsNullOrEmpty(o.Auxiliary8) ? "0" : o.Auxiliary8,
                        Auxiliary9 = string.IsNullOrEmpty(o.Auxiliary9) ? "0" : o.Auxiliary9,
                        Auxiliary10 = string.IsNullOrEmpty(o.Auxiliary10) ? "0" : o.Auxiliary10,
                    });
                });

                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                var data = _repository.UnitOfWork.SaveChangesAsync().Result;
                var d2 = _detailRepository.UnitOfWork.SaveChangesAsync().Result;
                Biz_Review review = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId ?? request.OwnerID).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                result.msg = "保存成功";
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID ?? _identityService.EnterpriseId,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID ?? _identityService.UserId,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "保存失败,请联系管理员！";
                result.data = ex;
            }
            return result;
        }
        private Result ValidParam(FD_SettleReceiptNewFundAddCommand model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.msg = "参数有误";
                return result;
            }
            if (string.IsNullOrEmpty(model.EnterpriseID))
            {
                result.msg = "EnterpriseID空";
                return result;
            }
            if (string.IsNullOrEmpty(model.SettleReceipType))
            {
                result.msg = "SettleReceipType空";
                return result;
            }
            var year = model.DataDate.Substring(0, 4);
            List<dynamic> List = _provider.GetFundList( year+ "-01-01",year + "-12-31");
            if (List.Count > 0)
            {
                result.msg = "当前年度已存在资金账户期初数据，请勿重复添加！";
                return result;
            }
            return result;
        }
    }
    public class FD_SettleReceiptNewOutSideAddHandler : IRequestHandler<FD_SettleReceiptNewOutSideAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        private TreeModelODataProvider _treeModel;
        Common.FMBaseCommon _baseCommon;
        IbsfileRepository _ibsfileRepository;
        FD_SettleReceiptNewODataProvider _provider;
        IBiz_Related _biz_RelatedRepository;
        EnterprisePeriodUtil _enterprisePeriodUtil;

        public FD_SettleReceiptNewOutSideAddHandler(EnterprisePeriodUtil enterprisePeriodUtil,IBiz_Related related, IbsfileRepository ibsfileRepository, FD_SettleReceiptNewODataProvider provider, IIdentityService identityService, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, Common.FMBaseCommon baseCommon)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
            _baseCommon = baseCommon;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
            _biz_RelatedRepository = related;
            _enterprisePeriodUtil = enterprisePeriodUtil;
        }
        public async Task<Result> Handle(FD_SettleReceiptNewOutSideAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                result = ValidParam(request);
                if (!string.IsNullOrEmpty(result.msg))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                long number = 0;
                if (string.IsNullOrEmpty(request.BeginDate))
                {
                    var period = _enterprisePeriodUtil.GetEnterperisePeriod(request.EnterpriseID, request.DataDate);
                    request.BeginDate = period.StartDate.ToString("yyyy-MM-dd");
                    request.EndDate = period.EndDate.ToString("yyyy-MM-dd");
                }
                //会计凭证支持手工录入凭证号
                number = Convert.ToInt64(_provider.GetMaxNumberByDate(request.SettleReceipType, request.EnterpriseID, request.BeginDate, request.EndDate,request.TicketedPointID).MaxNumber);
                //如果手动输入号码
                if (!string.IsNullOrEmpty(request.Number))
                {
                    //验证是否存在过
                    var temp = _provider.GetIsExitNumber(request.SettleReceipType, request.EnterpriseID, request.Number, request.BeginDate, request.EndDate,"0",request.TicketedPointID).MaxNumber;
                    //0 = 不存在可以使用
                    if (temp == "0")
                    {
                        number = Convert.ToInt64(request.Number);
                    }
                    else
                    {
                        return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), data = request, msg = "手工录入凭证号码重复！" };
                    }
                }
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                List<string> nums = new List<string>();
                //金融推送数据逻辑，新增时候保存关系，只有在删除的时候才会清除关系，修改不删除关系
                if (!string.IsNullOrEmpty(request.JRNumericalOrder))
                {
                    var related = new BIZ_Related()
                    {
                        RelatedType = "201610210104402122",
                        ChildType = "2303061011170000888",//金融专用APPID
                        ChildValue = request.JRNumericalOrder,//金融专用 流水号
                        ParentType = "1611091727140000101",//会计凭证appid
                        ParentValue = numericalOrder,//会计凭证 流水号
                        Remarks = "(金融)预付款使用记录信息同步（集团）"
                        //ParentValueDetail = numericalOrder
                    };
                    await _biz_RelatedRepository.AddAsync(related);
                }
                var domain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    SettleReceipType = request.SettleReceipType,
                    DataDate = Convert.ToDateTime(request.DataDate),
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    Number = number.ToString(),
                    AccountNo = request.AccountNo,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId ?? request.OwnerID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    details = new List<FD_SettleReceiptDetail>()
                };
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? request.EnterpriseID : o.EnterpriseID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) || o.ReceiptAbstractID == "无数据" ? "0" : o.ReceiptAbstractID,
                        AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) || o.AccoSubjectID == "无数据" ? "0" : o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) || o.CustomerID == "无数据" ? "0" : o.CustomerID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) || o.PersonID == "无数据" ? "0" : o.PersonID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) || o.MarketID == "无数据" ? "0" : o.MarketID,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) || o.ProjectID == "无数据" ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) || o.ProductID == "无数据" ? "0" : o.ProductID,
                        ProductGroupID = string.IsNullOrEmpty(o.ProductGroupID) || o.ProductGroupID == "无数据" ? "0" : o.ProductGroupID,
                        ClassificationID = string.IsNullOrEmpty(o.ClassificationID) || o.ClassificationID == "无数据" ? "0" : o.ClassificationID,
                        PaymentTypeID = string.IsNullOrEmpty(o.PaymentTypeID) || o.PaymentTypeID == "无数据" ? "0" : o.PaymentTypeID,
                        AccountID = string.IsNullOrEmpty(o.AccountID) || o.AccountID == "无数据" ? "0" : o.AccountID,
                        LorR = o.LorR,
                        Credit = o.Credit,
                        Debit = o.Debit,
                        Content = o.Content,
                        RowNum = o.RowNum,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) || o.OrganizationSortID == "无数据" ? "0" : o.OrganizationSortID,
                        IsCharges = false,
                        Auxiliary1 = string.IsNullOrEmpty(o.Auxiliary1) ? "0" : o.Auxiliary1, 
                        Auxiliary2 = string.IsNullOrEmpty(o.Auxiliary2)? "0" : o.Auxiliary2, 
                        Auxiliary3 = string.IsNullOrEmpty(o.Auxiliary3) ? "0" : o.Auxiliary3, 
                        Auxiliary4 = string.IsNullOrEmpty(o.Auxiliary4) ? "0" : o.Auxiliary4, 
                        Auxiliary5 = string.IsNullOrEmpty(o.Auxiliary5) ? "0" : o.Auxiliary5, 
                        Auxiliary6 = string.IsNullOrEmpty(o.Auxiliary6) ? "0" : o.Auxiliary6, 
                        Auxiliary7 = string.IsNullOrEmpty(o.Auxiliary7) ? "0" : o.Auxiliary7, 
                        Auxiliary8 = string.IsNullOrEmpty(o.Auxiliary8) ? "0" : o.Auxiliary8, 
                        Auxiliary9 = string.IsNullOrEmpty(o.Auxiliary9) ? "0" : o.Auxiliary9, 
                        Auxiliary10 = string.IsNullOrEmpty(o.Auxiliary10) ? "0" : o.Auxiliary10,
                    });
                });

                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                var data = _repository.UnitOfWork.SaveChangesAsync().Result;
                var d2 = _detailRepository.UnitOfWork.SaveChangesAsync().Result;
                Biz_Review review = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId ?? request.OwnerID).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                result.msg = "保存成功";
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID ?? _identityService.EnterpriseId,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID ?? _identityService.UserId,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "保存失败,请联系管理员！";
                result.data = ex;
            }
            return result;
        }
        private Result ValidParam(FD_SettleReceiptNewOutSideAddCommand model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.msg = "参数有误";
                return result;
            }
            if (string.IsNullOrEmpty(model.EnterpriseID))
            {
                result.msg = "EnterpriseID空";
                return result;
            }
            if (string.IsNullOrEmpty(model.SettleReceipType))
            {
                result.msg = "SettleReceipType空";
                return result;
            }
            var jrList = _biz_RelatedRepository.GetBIZ_Related(new BIZ_Related() { RelatedType = "201610210104402122", ParentType = "1611091727140000101", ChildType = "2303061011170000888", ChildValue = model.JRNumericalOrder });
            if (jrList != null)
            {
                if (jrList.Count > 0)
                {
                    result.msg = "当前数据已推送过，请勿重复推送！";
                    return result;
                }
            }
            return result;
        }
    }
    public class FD_SettleReceiptNewAddHandler : IRequestHandler<FD_SettleReceiptNewAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        private TreeModelODataProvider _treeModel;
        Common.FMBaseCommon _baseCommon;
        IbsfileRepository _ibsfileRepository;
        FD_SettleReceiptNewODataProvider _provider;
        IBiz_Related _biz_RelatedRepository;
        EnterprisePeriodUtil _enterprisePeriodUtil;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil1;
        ILogger<FD_SettleReceiptNewAddHandler> _logger;
        Ifd_receivablessetRepository _ReceivablessetRepository;
        public FD_SettleReceiptNewAddHandler(Ifd_receivablessetRepository ReceivablessetRepository,ILogger<FD_SettleReceiptNewAddHandler> logger, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration,EnterprisePeriodUtil enterprisePeriodUtil, IBiz_Related related,IbsfileRepository ibsfileRepository, FD_SettleReceiptNewODataProvider provider, IIdentityService identityService, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository,Common.FMBaseCommon baseCommon)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
            _baseCommon = baseCommon;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
            _biz_RelatedRepository = related;
            _enterprisePeriodUtil = enterprisePeriodUtil;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil;
            _logger = logger;
            _ReceivablessetRepository = ReceivablessetRepository;
        }
        public async Task<Result> Handle(FD_SettleReceiptNewAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                result= ValidParam(request);
                if (!string.IsNullOrEmpty(result.msg))
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                long number = 0;
                if (string.IsNullOrEmpty(request.BeginDate))
                {
                    var period = _enterprisePeriodUtil.GetEnterperisePeriod(request.EnterpriseID, request.DataDate);
                    request.BeginDate = period.StartDate.ToString("yyyy-MM-dd");
                    request.EndDate = period.EndDate.ToString("yyyy-MM-dd");
                }
                #region [会计凭证/收款] 设置取数   
                if (!string.IsNullOrEmpty(request.TemplateName))
                {
                    //通过模板名称取数 入参参数存在 TemplateName 执行 设置取数规则
                    var templates = _ReceivablessetRepository.GetListByEnterpriseId(request.EnterpriseID).FirstOrDefault(m => m.TemplateName == request.TemplateName && m.AppID == "1611091727140000101");
                    if (templates != null)
                    {
                        request.SettleReceipType = templates.SettleReceipType;
                        request.TicketedPointID = templates.TicketedPointID;
                        foreach (var item in request.Lines)
                        {
                            //false = 借,true = 贷
                            if (!item.LorR)
                            {
                                item.AccoSubjectID = templates.DebitAccoSubjectID;
                            }
                            else
                            {
                                item.AccoSubjectID = templates.CreditAccoSubjectID;
                            }
                            item.ReceiptAbstractID = templates.ReceiptAbstractID;
                            item.Content = templates.Content;
                        }

                    }
                    else
                    {
                        return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), data = request, msg = "[[会计凭证/收款]设置] 模板名称不存在,请核查后重试！" };
                    }
                }
                #endregion
                #region 结账控制,序时控制
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.OwnerID), DataDate = Convert.ToDateTime(request.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.OwnerID}/FM_AccoCheck/IsLockForm";
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                if ((bool)res?.ResultState && request.SettleReceipType != "201610220104402205")
                {
                    return new Result()
                    {
                        msg = $"当前会计期间已结账，请先取消结账",
                        code = ErrorCode.RequestArgumentError.GetIntValue(),
                    };
                }
                if (_provider.SequentialTime(request.DataDate,request.EnterpriseID,request.SettleReceipType) && request.SettleReceipType != "201610220104402205")
                {
                    return new Result()
                    {
                        msg = $"请遵循序时原则",
                        code = ErrorCode.RequestArgumentError.GetIntValue(),
                    };
                }
                #endregion
                //会计凭证支持手工录入凭证号
                number = Convert.ToInt64(_provider.GetMaxNumberByDate(request.SettleReceipType, request.EnterpriseID, request.BeginDate, request.EndDate, request.TicketedPointID).MaxNumber);
                //如果手动输入号码
                if (!string.IsNullOrEmpty(request.Number))
                {
                    //验证是否存在过
                    var temp = _provider.GetIsExitNumber(request.SettleReceipType, request.EnterpriseID, request.Number, request.BeginDate, request.EndDate, "0", request.TicketedPointID).MaxNumber;
                    //0 = 不存在可以使用
                    if (temp == "0")
                    {
                        number = Convert.ToInt64(request.Number);
                    }
                    else
                    {
                        return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), data = request, msg = "手工录入凭证号码重复！" };
                    }
                }
                //var numberResult = _baseCommon.GetNumber(new NumberSearchModel() {IsComplex="2303061011170000150",DataDate=request.DataDate,ChildSettleReceipType=request.SettleReceipType,TicketedPointID=request.TicketedPointID,EnterpriseID=request.EnterpriseID }); 

                //if (!numberResult.ResultState&& !string.IsNullOrEmpty(numberResult.Msg))
                //{
                //    return result;
                //}
                //var number = numberResult.Data;
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                List<string> nums = new List<string>();
                //凭证处理逻辑，新增时候保存关系，只有在删除的时候才会清除关系，修改不删除关系
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    nums = request.NumericalOrder.Split(',').ToList();
                    foreach (var item in nums)
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = "2303061011170000150",//凭证处理
                            ParentValue = item,//凭证处理 流水号
                            ChildType = "1611091727140000101",//会计凭证appid
                            ChildValue = numericalOrder,//会计凭证 流水号
                            Remarks = "凭证处理生成会计凭证（集团-汇总）"
                            //ParentValueDetail = numericalOrder
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                    }
                }
                var domain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    SettleReceipType = request.SettleReceipType,
                    DataDate = Convert.ToDateTime(request.DataDate),
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    Number = number.ToString(),
                    AccountNo = request.AccountNo,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID =request.EnterpriseID,
                    OwnerID = _identityService.UserId?? request.OwnerID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    details = new List<FD_SettleReceiptDetail>()
                };
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID =string.IsNullOrEmpty(o.EnterpriseID)?request.EnterpriseID:o.EnterpriseID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) || o.ReceiptAbstractID == "无数据" ? "0" : o.ReceiptAbstractID,
                        AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) || o.AccoSubjectID == "无数据" ? "0" : o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) || o.CustomerID == "无数据" ? "0" : o.CustomerID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) || o.PersonID == "无数据" ? "0" : o.PersonID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) || o.MarketID == "无数据" ? "0" : o.MarketID,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) || o.ProjectID == "无数据" ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) || o.ProductID == "无数据" ? "0" : o.ProductID,
                        ProductGroupID = string.IsNullOrEmpty(o.ProductGroupID) || o.ProductGroupID == "无数据" ? "0" : o.ProductGroupID,
                        ClassificationID = string.IsNullOrEmpty(o.ClassificationID) || o.ClassificationID == "无数据" ? "0" : o.ClassificationID,
                        PaymentTypeID = string.IsNullOrEmpty(o.PaymentTypeID) || o.PaymentTypeID == "无数据" ? "0" : o.PaymentTypeID,
                        AccountID = string.IsNullOrEmpty(o.AccountID) || o.AccountID == "无数据" ? "0" : o.AccountID,
                        LorR = o.LorR,
                        Credit = o.Credit,
                        Debit = o.Debit,
                        Content = o.Content,
                        RowNum = o.RowNum,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) || o.OrganizationSortID == "无数据" ? "0" : o.OrganizationSortID,
                        IsCharges = false,
                        Auxiliary1 = string.IsNullOrEmpty(o.Auxiliary1) ? "0" : o.Auxiliary1,
                        Auxiliary2 = string.IsNullOrEmpty(o.Auxiliary2) ? "0" : o.Auxiliary2,
                        Auxiliary3 = string.IsNullOrEmpty(o.Auxiliary3) ? "0" : o.Auxiliary3,
                        Auxiliary4 = string.IsNullOrEmpty(o.Auxiliary4) ? "0" : o.Auxiliary4,
                        Auxiliary5 = string.IsNullOrEmpty(o.Auxiliary5) ? "0" : o.Auxiliary5,
                        Auxiliary6 = string.IsNullOrEmpty(o.Auxiliary6) ? "0" : o.Auxiliary6,
                        Auxiliary7 = string.IsNullOrEmpty(o.Auxiliary7) ? "0" : o.Auxiliary7,
                        Auxiliary8 = string.IsNullOrEmpty(o.Auxiliary8) ? "0" : o.Auxiliary8,
                        Auxiliary9 = string.IsNullOrEmpty(o.Auxiliary9) ? "0" : o.Auxiliary9,
                        Auxiliary10 = string.IsNullOrEmpty(o.Auxiliary10) ? "0" : o.Auxiliary10,
                    });
                });
                
                //#region 重试机制
                //for (int i = 0; i < 5; i++)
                //{
                //    //验证是否存在过
                //    var temp = _provider.GetIsExitNumber(request.SettleReceipType, request.EnterpriseID, domain.Number, request.BeginDate, request.EndDate).MaxNumber;
                //    //0 = 不存在可以使用
                //    if (temp == "0")
                //    {
                //        number = Convert.ToInt64(request.Number);
                //        break;
                //    }
                //    else
                //    {
                //        domain.Number = number.ToString();
                //        await Task.Delay(1000);
                //    }
                //}
                //#endregion

                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                var data = _repository.UnitOfWork.SaveChangesAsync().Result;
                var d2 = _detailRepository.UnitOfWork.SaveChangesAsync().Result;
                Biz_Review review = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId ?? request.OwnerID).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                result.msg = "保存成功";
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = request.EnterpriseID ?? _identityService.EnterpriseId,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID =  request.OwnerID ?? _identityService.UserId,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                _logger.LogInformation($@"操作记录：会计凭证新增操作,\n 
                时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                传入参数：{JsonConvert.SerializeObject(request)},\n
                保存后参数：{JsonConvert.SerializeObject(domain)},\n
                ");
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "保存失败,请联系管理员！";
                result.data = ex;
            }
            return result;
        }
        private Result ValidParam(FD_SettleReceiptNewAddCommand model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.msg = "参数有误";
                return result;
            }
            if (string.IsNullOrEmpty(model.EnterpriseID))
            {
                result.msg = "EnterpriseID空";
                return result;
            }
            if (string.IsNullOrEmpty(model.SettleReceipType))
            {
                result.msg = "SettleReceipType空";
                return result;
            }
            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FD_SettleReceiptNewDeleteHandler : IRequestHandler<FD_SettleReceiptNewDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IbsfileRepository _ibsfileRepository;
        IBiz_Related _biz_RelatedRepository;
        Ifm_transferrealtimeRepository _transferrealtimeRepository;
        Ifm_transferrealtimedetailRepository _transferrealtimedetailRepository;
        Ifm_voucheramortizationrelatedRepository _voucheramortizationrelatedRepository;
        IFD_VoucherAmortizationPeriodDetailRepository _periodDetailRepository;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil1;
        ILogger<FD_SettleReceiptNewDeleteHandler> _logger;
        public FD_SettleReceiptNewDeleteHandler(ILogger<FD_SettleReceiptNewDeleteHandler> logger,HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil1, Ifm_voucheramortizationrelatedRepository voucheramortizationrelatedRepository,Ifm_transferrealtimedetailRepository transferrealtimedetailRepository,Ifm_transferrealtimeRepository transferrealtimeRepository, IBiz_Related related,IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository, IFD_VoucherAmortizationPeriodDetailRepository periodDetailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _ibsfileRepository = ibsfileRepository;
            _biz_RelatedRepository = related;
            _transferrealtimedetailRepository = transferrealtimedetailRepository;
            _transferrealtimeRepository = transferrealtimeRepository;
            _voucheramortizationrelatedRepository = voucheramortizationrelatedRepository;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil1;
            _logger = logger;
            _periodDetailRepository = periodDetailRepository;
        }

        public async Task<Result> Handle(FD_SettleReceiptNewDeleteCommand request, CancellationToken cancellationToken)
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
                    var domain = await _repository.GetDataAsync(num);
                    #region 结账控制
                    var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(domain.EnterpriseID), OwnerID = Convert.ToInt64(domain.OwnerID), DataDate = Convert.ToDateTime(domain.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                    string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{domain.EnterpriseID}/{_identityService.UserId}/FM_AccoCheck/IsLockForm";
                    var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                    if ((bool)res?.ResultState)
                    {
                        return new Result()
                        {
                            msg = $"当前会计期间已结账，请先取消结账",
                            code = ErrorCode.RequestArgumentError.GetIntValue(),
                        };
                    }
                    #endregion
                    //检测是否存在已摊销数据(关系)
                    var Amortization = _voucheramortizationrelatedRepository.GetBySettle(Convert.ToInt64(num)).FirstOrDefault();
                    if (Amortization != null) 
                    {
                        //获取明细摊销数据
                        var AmortizationData = _periodDetailRepository.GetPeriods(Amortization.NumericalOrderVoucher.ToString()).Result.Where(m=>m.NumericalOrderDetail == Amortization.NumericalOrderInto.ToString()).FirstOrDefault();
                        AmortizationData.IsAmort = false;
                        _periodDetailRepository.Update(AmortizationData);
                    }
                    _logger.LogInformation($@"操作记录：会计凭证删除操作,\n 
                    时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                    操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                    传入参数：{JsonConvert.SerializeObject(request)},\n
                    当前删除参数：{JsonConvert.SerializeObject(num)},\n
                    当前被删除的数据：{JsonConvert.SerializeObject(domain)},\n
                    ");
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.ChildValue == num);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.ParentValue == num);
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num && o.ReviweType == "1611091727140000101");  
                    await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                    #region 删除 凭证其他关系  摊销 + 结转生成
                    await _transferrealtimedetailRepository.RemoveRangeAsync(o => o.NumericalOrderReceipt.ToString() == num);
                    await _transferrealtimedetailRepository.UnitOfWork.SaveChangesAsync();
                    await _transferrealtimeRepository.RemoveRangeAsync(o => o.NumericalOrder.ToString() == num);
                    await _transferrealtimeRepository.UnitOfWork.SaveChangesAsync();
                    await _voucheramortizationrelatedRepository.RemoveRangeAsync(o => o.NumericalOrderSettl.ToString() == num);
                    await _voucheramortizationrelatedRepository.UnitOfWork.SaveChangesAsync();
                    await _periodDetailRepository.UnitOfWork.SaveChangesAsync();
                    #endregion

                }
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
                result.data = ex;
            }

            return result;
        }
    }
    /// <summary>
    /// 凭证整理
    /// </summary>
    public class FD_SettleReceiptRemakeHandler : IRequestHandler<FD_SettleReceiptRemakeCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FD_SettleReceiptNewODataProvider _provider;
        ILogger<FD_SettleReceiptRemakeHandler> _logger;

        public FD_SettleReceiptRemakeHandler(ILogger<FD_SettleReceiptRemakeHandler> logger,FD_SettleReceiptNewODataProvider provider, IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _provider = provider;
            _logger = logger;
        }
        public async Task<Result> Handle(FD_SettleReceiptRemakeCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            var uuid = Guid.NewGuid();
            try
            {
                if (request.StartNumber == 0 || request.StartNumber == null)
                {
                    request.StartNumber = 1;
                }
                //选项一：
                //选项ID：20191118103011（集团和单位选项）
                //开启，会计凭证编号按单据字进行编号； 关闭，会计凭证编号不按单据字进行编号； 默认关闭
                var optionId1 = "20191118103011";
                var option1 = _provider.GetOptionInfos(optionId1, request.EnterpriseID);
                //选项二：
                //选项ID：20191118103100（集团和单位选项）
                //默认不开启，开启后会计凭证编码只以凭证类别进行编码，例如：收 - 1、付 - 1、转 - 1等开始编码
                var optionId2 = "20191118103100";
                var option2 = _provider.GetOptionInfos(optionId2, request.EnterpriseID);
                // true = 按凭证号重新顺次排序  
                if (request.IsOrder)
                {
                    var list = _repository.GetDataList(request.beginDate,request.endDate,request.EnterpriseID).OrderBy(m=>m.Number);
                    _logger.LogInformation($@"操作记录：会计凭证整理操作(按凭证号重新顺次排序),\n 
                    UUID={uuid}\n
                    时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                    操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                    传入参数：{JsonConvert.SerializeObject(request)},\n
                    系统选项1：20191118103011:{JsonConvert.SerializeObject(option1)}
                    系统选项2：20191118103100:{JsonConvert.SerializeObject(option2)}
                    准备整理前数据：{JsonConvert.SerializeObject(list)},\n
                    ");
                    //场景二：选项二关闭，选项一关闭
                    //编号规则：什么都不区分，编号就是1开始
                    if (!option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
                    {
                        foreach (var items in list)
                        {
                            items.Number = request.StartNumber.ToString();
                            request.StartNumber++;
                            _repository.Update(items);
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                    //场景三：选项二开启，选项一关闭
                    //编号规则：按凭证类别分类编号
                    else if (option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
                    {
                        foreach (var items in list.GroupBy(m => m.SettleReceipType))
                        {
                            var Number = request.StartNumber;
                            foreach (var item in items)
                            {
                                item.Number = Number.ToString();
                                Number++;
                                _repository.Update(item);
                            }
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                    //场景四：选项二开启，选项一开启
                    //编号规则：按单据字、凭证类别分类编号 列表排序如 北京 收1 北京收2 北京 付 1 北京付2
                    else if (option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)
                    {
                        foreach (var items in list.GroupBy(m => new { m.SettleReceipType,m.TicketedPointID }))
                        {
                            var Number = request.StartNumber;
                            foreach (var item in items)
                            {
                                item.Number = Number.ToString();
                                Number++;
                                _repository.Update(item);
                            }
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                    //场景一：选项二关闭，选项一开启
                    //编号规则：按单据字分类排序，列表排序如 北京 1 北京 2 上海1 上海2
                    else /*if (!option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)*/
                    {
                        foreach (var items in list.GroupBy(m => m.TicketedPointID))
                        {
                            var Number = request.StartNumber;
                            foreach (var item in items)
                            {
                                item.Number = Number.ToString();
                                Number++;
                                _repository.Update(item);
                            }
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                }
                //false = 按凭证日期重新顺次排序
                else
                {
                    var list = _repository.GetDataList(request.beginDate, request.endDate, request.EnterpriseID).OrderBy(m => m.DataDate);
                    _logger.LogInformation($@"操作记录：会计凭证整理操作(按凭证日期重新顺次排序),\n 
                    UUID={uuid}\n
                    时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                    操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                    传入参数：{JsonConvert.SerializeObject(request)},\n
                    系统选项1：20191118103011:{JsonConvert.SerializeObject(option1)}
                    系统选项2：20191118103100:{JsonConvert.SerializeObject(option2)}
                    准备整理前数据：{JsonConvert.SerializeObject(list)},\n
                    ");
                    //场景二：选项二关闭，选项一关闭
                    //编号规则：什么都不区分，编号就是1开始
                    if (!option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
                    {
                        foreach (var items in list)
                        {
                            items.Number = request.StartNumber.ToString();
                            request.StartNumber++;
                            _repository.Update(items);
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                    //场景三：选项二开启，选项一关闭
                    //编号规则：按凭证类别分类编号
                    else if (option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
                    {
                        foreach (var items in list.GroupBy(m => m.SettleReceipType))
                        {
                            var Number = request.StartNumber;
                            foreach (var item in items)
                            {
                                item.Number = Number.ToString();
                                Number++;
                                _repository.Update(item);
                            }
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                    //场景四：选项二开启，选项一开启
                    //编号规则：按单据字、凭证类别分类编号 列表排序如 北京 收1 北京收2 北京 付 1 北京付2
                    else if (option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)
                    {
                        foreach (var items in list.GroupBy(m => new { m.SettleReceipType, m.TicketedPointID }))
                        {
                            var Number = request.StartNumber;
                            foreach (var item in items)
                            {
                                item.Number = Number.ToString();
                                Number++;
                                _repository.Update(item);
                            }
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                    //场景一：选项二关闭，选项一开启
                    //编号规则：按单据字分类排序，列表排序如 北京 1 北京 2 上海1 上海2
                    else /*if (!option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)*/
                    {
                        foreach (var items in list.GroupBy(m => m.TicketedPointID))
                        {
                            var Number = request.StartNumber;
                            foreach (var item in items)
                            {
                                item.Number = Number.ToString();
                                Number++;
                                _repository.Update(item);
                            }
                        }
                        await _repository.UnitOfWork.SaveChangesAsync();
                    }
                }
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = $"【{request.beginDate}<==>{request.endDate}】会计期间凭证整理成功";
                _logger.LogInformation($@"整理完成：UUID={uuid}\n");
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
                result.data = ex;
            }

            return result;
        }
    }
    /// <summary>
    /// 批量审核
    /// </summary>
    public class FD_SettleReceiptNewBatchReviewHandler : IRequestHandler<FD_SettleReceiptBatchReviewCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FD_SettleReceiptNewODataProvider _provider;
        ILogger<FD_SettleReceiptNewBatchReviewHandler> _logger;

        public FD_SettleReceiptNewBatchReviewHandler(ILogger<FD_SettleReceiptNewBatchReviewHandler> logger,FD_SettleReceiptNewODataProvider provider ,IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _provider = provider;
            _logger = logger;
        }

        public async Task<Result> Handle(FD_SettleReceiptBatchReviewCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrWhiteSpace(request.NumericalOrder))
                {
                    result.code = ErrorCode.NoContent.GetIntValue();
                    result.msg = "要审核的流水号为空";
                    return result;
                }
                var list = request.NumericalOrder.Split(',');
                foreach (var item in list)
                {
                    Biz_Review review = new Biz_Review(item, "1611091727140000101", _identityService.UserId).SetAudit(2);
                    await _biz_ReviewRepository.AddAsync(review);
                    #region 【80668】【收付款单】与【会计凭证】审核互通后端
                    try
                    {
                        var domain = await _repository.GetDataAsync(item);
                        var optionId = "201612270104402002";
                        var option = _provider.GetOptionInfos(optionId, domain.EnterpriseID);
                        if (option.FirstOrDefault().OptionSwitch)
                        {
                            //获取审核数据
                            var data = _biz_ReviewRepository.GetByNumericalOrder(item).ToList();
                            //收款单数据集
                            var Receivables = data.Where(m => m.ReviweType == "1611231950150000101").ToList();
                            //付款单数据集
                            var Pays = data.Where(m => m.ReviweType == "1612011058280000101").ToList();

                            //付款单汇总数据集
                            var PaysSummary = data.Where(m => m.ReviweType == "1612060935280000101").ToList();
                            //收款单汇总数据集
                            var ReceivablesSummary = data.Where(m => m.ReviweType == "1612101120530000101").ToList();


                            //监测 当前数据是否是收款单 如 是 并且 已制单未审核+系统选项开启 则会计凭证是审核 赋值收款单审核

                            //201611180104402204付款汇总单 
                            //201611180104402202付款单
                            //201611180104402203收款汇总单
                            //201611180104402201收款单
                            if (Receivables.Count() > 0 && Receivables.Where(m => m.CheckMark == ReviewCode.审核.GetIntValue()).Count() == 0)
                            {
                                //收款单
                                await _biz_ReviewRepository.AddAsync(new Biz_Review(item, "1611231950150000101", _identityService.UserId).SetAudit(2));
                            }
                            else if (Pays.Count() > 0 && Pays.Where(m => m.CheckMark == ReviewCode.审核.GetIntValue()).Count() == 0)
                            {
                                //付款单
                                await _biz_ReviewRepository.AddAsync(new Biz_Review(item, "1612011058280000101", _identityService.UserId).SetAudit(2));
                            }
                            else if (PaysSummary.Count() > 0 && PaysSummary.Where(m => m.CheckMark == ReviewCode.审核.GetIntValue()).Count() == 0)
                            {
                                //付款单汇总
                                await _biz_ReviewRepository.AddAsync(new Biz_Review(item, "1612060935280000101", _identityService.UserId).SetAudit(2));
                            }
                            else if (ReceivablesSummary.Count() > 0 && ReceivablesSummary.Where(m => m.CheckMark == ReviewCode.审核.GetIntValue()).Count() == 0)
                            {
                                //收款汇总
                                await _biz_ReviewRepository.AddAsync(new Biz_Review(item, "1612101120530000101", _identityService.UserId).SetAudit(2));
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(JsonConvert.SerializeObject(e));
                    }
                    #endregion
                }
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = $"本次审核成功的凭证共{list.Length}张";
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
                result.data = ex;
            }

            return result;
        }
    }
    /// <summary>
    /// 批量取消审核
    /// 【80993】【会计凭证】接口增加取消审核同步收付款单
    /// </summary>
    public class FD_SettleReceiptNewBatchCancelReviewHandler : IRequestHandler<FD_SettleReceiptBatchCancelReviewCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FD_SettleReceiptNewODataProvider _provider;
        ILogger<FD_SettleReceiptNewBatchCancelReviewHandler> _logger;

        public FD_SettleReceiptNewBatchCancelReviewHandler(ILogger<FD_SettleReceiptNewBatchCancelReviewHandler> logger, FD_SettleReceiptNewODataProvider provider, IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _provider = provider;
            _logger = logger;
        }

        public async Task<Result> Handle(FD_SettleReceiptBatchCancelReviewCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrWhiteSpace(request.NumericalOrder))
                {
                    result.code = ErrorCode.NoContent.GetIntValue();
                    result.msg = "要取消审核的流水号为空";
                    return result;
                }
                var list = request.NumericalOrder.Split(',');
                foreach (var item in list)
                {
                    await _biz_ReviewRepository.RemoveRangeAsync(m=> m.CheckMark == ReviewCode.审核.GetIntValue() && m.NumericalOrder == item && m.ReviweType == "1611091727140000101");
                    #region 【80668】【收付款单】与【会计凭证】审核互通后端
                    try
                    {
                        var domain = await _repository.GetDataAsync(request.NumericalOrder);
                        var optionId = "201612270104402002";
                        var option = _provider.GetOptionInfos(optionId, domain.EnterpriseID);
                        if (option.FirstOrDefault().OptionSwitch)
                        {
                            //收款单
                            await _biz_ReviewRepository.RemoveRangeAsync(m => m.CheckMark == ReviewCode.审核.GetIntValue() && m.NumericalOrder == item && m.ReviweType == "1611231950150000101");
                            //付款单
                            await _biz_ReviewRepository.RemoveRangeAsync(m => m.CheckMark == ReviewCode.审核.GetIntValue() && m.NumericalOrder == item && m.ReviweType == "1612011058280000101");
                            //收款汇总单
                            await _biz_ReviewRepository.RemoveRangeAsync(m => m.CheckMark == ReviewCode.审核.GetIntValue() && m.NumericalOrder == item && m.ReviweType == "1612060935280000101");
                            //付款汇总单
                            await _biz_ReviewRepository.RemoveRangeAsync(m => m.CheckMark == ReviewCode.审核.GetIntValue() && m.NumericalOrder == item && m.ReviweType == "1612101120530000101");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(JsonConvert.SerializeObject(e));
                    }
                    #endregion
                }
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = $"取消审核成功";
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
                result.data = ex;
                _logger.LogError("批量取消审核异常："+JsonConvert.SerializeObject(ex));
            }
            return result;
        }
    }
    public class FD_SettleReceiptNewModifyHandler : IRequestHandler<FD_SettleReceiptNewModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        FD_SettleReceiptNewODataProvider _provider;
        IbsfileRepository _ibsfileRepository;
        EnterprisePeriodUtil _enterprisePeriodUtil;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil1;
        ILogger<FD_SettleReceiptNewModifyHandler> _logger;
        public FD_SettleReceiptNewModifyHandler(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, ILogger<FD_SettleReceiptNewModifyHandler> logger, EnterprisePeriodUtil enterprisePeriodUtil, FD_SettleReceiptNewODataProvider provider, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
            _enterprisePeriodUtil = enterprisePeriodUtil;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil;
            _logger = logger;
        }

        public async Task<Result> Handle(FD_SettleReceiptNewModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var uuid = Guid.NewGuid();
                var domain = await _repository.GetDataAsync(request.NumericalOrder);
                if (domain == null) { result.code = ErrorCode.RequestArgumentError.GetIntValue(); result.msg = "NumericalOrder查询空"; return result; }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                #region 结账控制
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(domain.EnterpriseID), OwnerID = Convert.ToInt64(_identityService.UserId), DataDate = Convert.ToDateTime(domain.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{_identityService.UserId}/FM_AccoCheck/IsLockForm";
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                if ((bool)res?.ResultState && request.SettleReceipType != "201610220104402205")
                {
                    return new Result()
                    {
                        msg = $"当前会计期间已结账，请先取消结账",
                        code = ErrorCode.RequestArgumentError.GetIntValue(),
                    };
                }
                if (domain.SettleReceipType != request.SettleReceipType || domain.DataDate.ToString("yyyy-MM-dd") != request.DataDate || domain.EnterpriseID != request.EnterpriseID)
                {
                    if (_provider.SequentialTime(request.DataDate, request.EnterpriseID, request.SettleReceipType) && request.SettleReceipType != "201610220104402205")
                    {
                        return new Result()
                        {
                            msg = $"请遵循序时原则",
                            code = ErrorCode.RequestArgumentError.GetIntValue(),
                        };
                    }
                }
                #endregion
                if (string.IsNullOrEmpty(request.BeginDate))
                {
                    var period = _enterprisePeriodUtil.GetEnterperisePeriod(request.EnterpriseID, request.DataDate);
                    request.BeginDate = period.StartDate.ToString("yyyy-MM-dd");
                    request.EndDate = period.EndDate.ToString("yyyy-MM-dd");
                }
                _logger.LogInformation($@"操作记录：会计凭证修改操作,\n 
                UUID={uuid}\n
                时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                传入参数：{JsonConvert.SerializeObject(request)},\n
                修改前参数：{JsonConvert.SerializeObject(domain)},\n
                ");
                //验证是否存在过
                var temp = _provider.GetIsExitNumber(request.SettleReceipType, request.EnterpriseID, request.Number,request.BeginDate,request.EndDate,request.NumericalOrder,request.TicketedPointID).MaxNumber;
                //0 = 不存在可以使用 或者 跟当前单据号一致
                if (temp == "0" && request.SettleReceipType != "201610220104402205")
                {
                    domain.Number = request.Number;
                }
                else
                {
                    if (request.SettleReceipType != "201610220104402205")
                    {
                        return new Result() { code = ErrorCode.RequestArgumentError.GetIntValue(), data = request, msg = "手工录入凭证号码重复！" };
                    }
                }
                domain.DataDate = Convert.ToDateTime(request.DataDate);
                domain.TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID;
                domain.AccountNo = request.AccountNo;
                domain.AttachmentNum = request.AttachmentNum;
                domain.Remarks = request.Remarks;
                domain.SettleReceipType = request.SettleReceipType;
                _repository.Update(domain);
                domain.details = new List<FD_SettleReceiptDetail>();
                #region 先删后增
                await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                #endregion

                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = request.NumericalOrder,
                        Guid = string.IsNullOrEmpty(o.Guid) ? Guid.NewGuid() : Guid.Parse(o.Guid),
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? request.EnterpriseID : o.EnterpriseID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) ? "0" : o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        ProductGroupID = string.IsNullOrEmpty(o.ProductGroupID) ? "0" : o.ProductGroupID,
                        ClassificationID = string.IsNullOrEmpty(o.ClassificationID) ? "0" : o.ClassificationID,
                        PaymentTypeID = string.IsNullOrEmpty(o.PaymentTypeID) ? "0" : o.PaymentTypeID,
                        AccountID = string.IsNullOrEmpty(o.AccountID) ? "0" : o.AccountID,
                        LorR = o.LorR,
                        Credit = o.Credit,
                        Debit = o.Debit,
                        Content = o.Content,
                        RowNum = o.RowNum,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        IsCharges = false,
                        Auxiliary1 = string.IsNullOrEmpty(o.Auxiliary1) ? "0" : o.Auxiliary1,
                        Auxiliary2 = string.IsNullOrEmpty(o.Auxiliary2) ? "0" : o.Auxiliary2,
                        Auxiliary3 = string.IsNullOrEmpty(o.Auxiliary3) ? "0" : o.Auxiliary3,
                        Auxiliary4 = string.IsNullOrEmpty(o.Auxiliary4) ? "0" : o.Auxiliary4,
                        Auxiliary5 = string.IsNullOrEmpty(o.Auxiliary5) ? "0" : o.Auxiliary5,
                        Auxiliary6 = string.IsNullOrEmpty(o.Auxiliary6) ? "0" : o.Auxiliary6,
                        Auxiliary7 = string.IsNullOrEmpty(o.Auxiliary7) ? "0" : o.Auxiliary7,
                        Auxiliary8 = string.IsNullOrEmpty(o.Auxiliary8) ? "0" : o.Auxiliary8,
                        Auxiliary9 = string.IsNullOrEmpty(o.Auxiliary9) ? "0" : o.Auxiliary9,
                        Auxiliary10 = string.IsNullOrEmpty(o.Auxiliary10) ? "0" : o.Auxiliary10,
                    });
                });
                await _detailRepository.AddRangeAsync(domain.details);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "修改成功";
                _logger.LogInformation($@"操作记录：会计凭证修改后操作,\n 
                UUID={uuid}\n
                时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                传入参数：{JsonConvert.SerializeObject(request)},\n
                修改后参数：{JsonConvert.SerializeObject(domain)},\n
                ");
                if (up != null)
                {
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = _identityService.EnterpriseId,
                            NumericalOrder = request.NumericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = _identityService.UserId,
                            Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
                _logger.LogError("操作异常："+JsonConvert.SerializeObject(errorCodeEx));
                _logger.LogError("操作异常参数："+JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "修改失败";
                result.data = ex;
                result.errors = new List<ErrorRow>() { new ErrorRow() { index = 1, columns = new List<ErrorColumn>() { new ErrorColumn() { value = ex } } } };
                _logger.LogError("2操作异常参数："+JsonConvert.SerializeObject(request));
            }
            return result;
        }
    }
    public class VoucherHandler : IRequestHandler<VoucherCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        private TreeModelODataProvider _treeModel;
        Common.FMBaseCommon _baseCommon;
        IbsfileRepository _ibsfileRepository;
        FD_SettleReceiptNewODataProvider _provider;
        Ifd_paymentreceivablesvoucherRepository _PaymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _PaymentreceivablesvoucherDetailRepository;
        IBiz_Related _biz_RelatedRepository;
        ILogger<VoucherHandler> _logger;

        public VoucherHandler(ILogger<VoucherHandler> logger, IBiz_Related related,Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,IbsfileRepository ibsfileRepository, FD_SettleReceiptNewODataProvider provider, IIdentityService identityService, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, Common.FMBaseCommon baseCommon)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
            _baseCommon = baseCommon;
            _ibsfileRepository = ibsfileRepository;
            _provider = provider;
            _PaymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _PaymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _biz_RelatedRepository = related;
            _logger = logger;
        }
        public async Task<Result> Handle(VoucherCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            var uuid = Guid.NewGuid();
            try
            {
                if (string.IsNullOrEmpty(request.NumericalOrder))
                {
                    return new Result()
                    {
                        code = ErrorCode.NoContent.GetIntValue(),
                        msg = "请选择要生成的凭证，勾选项为空不可生成凭证"
                    };
                }
                List<string> nums = request.NumericalOrder.Split(',').ToList();
                foreach (var item in nums)
                {
                    //防止重复生成
                    var Validation = _biz_RelatedRepository.GetRelated(new BIZ_Related() { RelatedType = "201610210104402122", ParentType = "2303061011170000150", ChildType = "1611091727140000101", ParentValue = item }).Result;
                    if (Validation.Count > 0)
                    {
                        return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), msg = "当前数据已生成过凭证" };
                    }
                }
                //当单独生成凭证的时候走这个逻辑
                if (nums.Count == 1)
                {
                    //防止重复生成
                    var Validation = _biz_RelatedRepository.GetRelated(new BIZ_Related() { RelatedType = "201610210104402122",ParentType = "2303061011170000150", ChildType = "1611091727140000101", ParentValue = nums.FirstOrDefault() }).Result;
                    if (Validation.Count > 0)
                    {
                        return new Result() { code = ErrorCode.ServerBusy.GetIntValue(),msg = "当前数据已生成过凭证" };
                    }
                    //获取扩展表数据
                    var voucher = _PaymentreceivablesvoucherRepository.Get(nums.FirstOrDefault());
                    var voucherDetail = _PaymentreceivablesvoucherDetailRepository.GetDetailByIdAsync(nums.FirstOrDefault());
                    var settleReceipt = new Domain.FD_SettleReceipt()
                    {
                        NumericalOrder = voucher.NumericalOrder,
                        Guid = voucher.Guid,
                        SettleReceipType = request.SettleReceipType,
                        DataDate = Convert.ToDateTime(request.DataDate),
                        TicketedPointID = voucher.TicketedPointID,
                        Number = _provider.GetMaxNumberByDate(request.SettleReceipType, request.EnterpriseID, request.BeginDate, request.EndDate, request.TicketedPointID).MaxNumber,
                        AccountNo = voucher.AccountNo,
                        AttachmentNum = voucher.AttachmentNum,
                        Remarks = voucher.Remarks,
                        EnterpriseID = request.EnterpriseID,
                        OwnerID = _identityService.UserId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        details = new List<FD_SettleReceiptDetail>()
                    };
                    voucherDetail.ForEach(o =>
                    {
                        settleReceipt.details.Add(new FD_SettleReceiptDetail()
                        {
                            NumericalOrder = o.NumericalOrder,
                            Guid = o.Guid,
                            EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? request.EnterpriseID : o.EnterpriseID,
                            ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) || o.ReceiptAbstractID == "无数据" ? "0" : o.ReceiptAbstractID,
                            AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) || o.AccoSubjectID == "无数据" ? "0" : o.AccoSubjectID,
                            AccoSubjectCode = o.AccoSubjectCode,
                            CustomerID = string.IsNullOrEmpty(o.CustomerID) || o.CustomerID == "无数据" ? "0" : o.CustomerID,
                            PersonID = string.IsNullOrEmpty(o.PersonID) || o.PersonID == "无数据" ? "0" : o.PersonID,
                            MarketID = string.IsNullOrEmpty(o.MarketID) || o.MarketID == "无数据" ? "0" : o.MarketID,
                            ProjectID = string.IsNullOrEmpty(o.ProjectID) || o.ProjectID == "无数据" ? "0" : o.ProjectID,
                            ProductID = string.IsNullOrEmpty(o.ProductID) || o.ProductID == "无数据" ? "0" : o.ProductID,
                            ProductGroupID = string.IsNullOrEmpty(o.ProductGroupID) || o.ProductGroupID == "无数据" ? "0" : o.ProductGroupID,
                            ClassificationID = string.IsNullOrEmpty(o.ClassificationID) || o.ClassificationID == "无数据" ? "0" : o.ClassificationID,
                            PaymentTypeID = string.IsNullOrEmpty(o.PaymentTypeID) || o.PaymentTypeID == "无数据" ? "0" : o.PaymentTypeID,
                            AccountID = string.IsNullOrEmpty(o.AccountID) || o.AccountID == "无数据" ? "0" : o.AccountID,
                            LorR = o.LorR,
                            Credit = o.Credit,
                            Debit = o.Debit,
                            Content = o.Content,
                            RowNum = o.RowNum,
                            OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) || o.OrganizationSortID == "无数据" ? "0" : o.OrganizationSortID,
                            IsCharges = o.IsCharges,
                        });
                    });
                    var related = new BIZ_Related()
                    {
                        RelatedType = "201610210104402122",
                        ParentType = "2303061011170000150",//凭证处理
                        ParentValue = voucher.NumericalOrder,//凭证处理 流水号
                        ChildType = "1611091727140000101",//会计凭证appid
                        ChildValue = settleReceipt.NumericalOrder,//会计凭证 流水号
                        Remarks = "凭证处理生成会计凭证（集团）"
                        //ParentValueDetail = numericalOrder
                    };
                    var msg = "";
                    foreach (var item in voucherDetail)
                    {
                        //LorR=false = 借方
                        if (string.IsNullOrEmpty(item.AccoSubjectID) && !item.LorR)
                        {
                            msg += $"{item.RowNum},借方科目为空。\n";
                        }
                        else if(string.IsNullOrEmpty(item.AccoSubjectID) && item.LorR)
                        {
                            msg += $"{item.RowNum},贷方科目为空。\n";
                        }
                    }
                    await _biz_RelatedRepository.AddAsync(related);
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                    await _repository.AddAsync(settleReceipt);
                    await _detailRepository.AddRangeAsync(settleReceipt.details);
                    _logger.LogInformation($@"操作记录：凭证整理生成会计凭证操作,\n 
                    UUID={uuid}\n
                    当单独生成凭证的时候走这个逻辑，\n
                    时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                    操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                    传入参数：{JsonConvert.SerializeObject(request)},\n
                    保存后参数：{JsonConvert.SerializeObject(settleReceipt)},\n
                    ");
                    var data = _repository.UnitOfWork.SaveChangesAsync().Result;
                    var d2 = _detailRepository.UnitOfWork.SaveChangesAsync().Result;
                    Biz_Review review = new Biz_Review(voucher.NumericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                    await _biz_ReviewRepository.AddAsync(review);
                    await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                    result.msg = $@"执行成功！凭证号{settleReceipt.Number}{msg}";
                    result.data = new { NumericalOrder = voucher.NumericalOrder, settleReceipt.Number };
                    result.code = ErrorCode.Success.GetIntValue();
                }
                else
                {
                    var voucher = _PaymentreceivablesvoucherRepository.Get(nums[0]);
                    var settleReceipt = new Domain.FD_SettleReceipt()
                    {
                        NumericalOrder = voucher.NumericalOrder,
                        Guid = voucher.Guid,
                        SettleReceipType = request.SettleReceipType,
                        DataDate = Convert.ToDateTime(request.DataDate),
                        TicketedPointID = voucher.TicketedPointID,
                        Number = _provider.GetMaxNumberByDate(request.SettleReceipType, request.EnterpriseID, request.BeginDate, request.EndDate, request.TicketedPointID).MaxNumber,
                        AccountNo = voucher.AccountNo,
                        AttachmentNum = voucher.AttachmentNum,
                        Remarks = voucher.Remarks,
                        EnterpriseID = request.EnterpriseID,
                        OwnerID = _identityService.UserId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        details = new List<FD_SettleReceiptDetail>()
                    };
                    foreach (var item in nums)
                    {
                        //获取扩展表数据
                        var voucherDetail = _PaymentreceivablesvoucherDetailRepository.GetDetailByIdAsync(item);
                        
                        voucherDetail.ForEach(o =>
                        {
                            settleReceipt.details.Add(new FD_SettleReceiptDetail()
                            {
                                NumericalOrder = o.NumericalOrder,
                                Guid = o.Guid,
                                EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? request.EnterpriseID : o.EnterpriseID,
                                ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) || o.ReceiptAbstractID == "无数据" ? "0" : o.ReceiptAbstractID,
                                AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) || o.AccoSubjectID == "无数据" ? "0" : o.AccoSubjectID,
                                AccoSubjectCode = o.AccoSubjectCode,
                                CustomerID = string.IsNullOrEmpty(o.CustomerID) || o.CustomerID == "无数据" ? "0" : o.CustomerID,
                                PersonID = string.IsNullOrEmpty(o.PersonID) || o.PersonID == "无数据" ? "0" : o.PersonID,
                                MarketID = string.IsNullOrEmpty(o.MarketID) || o.MarketID == "无数据" ? "0" : o.MarketID,
                                ProjectID = string.IsNullOrEmpty(o.ProjectID) || o.ProjectID == "无数据" ? "0" : o.ProjectID,
                                ProductID = string.IsNullOrEmpty(o.ProductID) || o.ProductID == "无数据" ? "0" : o.ProductID,
                                ProductGroupID = string.IsNullOrEmpty(o.ProductGroupID) || o.ProductGroupID == "无数据" ? "0" : o.ProductGroupID,
                                ClassificationID = string.IsNullOrEmpty(o.ClassificationID) || o.ClassificationID == "无数据" ? "0" : o.ClassificationID,
                                PaymentTypeID = string.IsNullOrEmpty(o.PaymentTypeID) || o.PaymentTypeID == "无数据" ? "0" : o.PaymentTypeID,
                                AccountID = string.IsNullOrEmpty(o.AccountID) || o.AccountID == "无数据" ? "0" : o.AccountID,
                                LorR = o.LorR,
                                Credit = o.Credit,
                                Debit = o.Debit,
                                Content = o.Content,
                                RowNum = o.RowNum,
                                OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) || o.OrganizationSortID == "无数据" ? "0" : o.OrganizationSortID,
                                IsCharges = o.IsCharges,
                            });
                        });
                    }
                    settleReceipt.details = settleReceipt.details.OrderBy(m => m.LorR).ToList();
                    var a = JsonConvert.SerializeObject(settleReceipt);
                    //如果开启科目合并
                    if (request.IsSummaryAccosubject)
                    {
                        //记录第一次出现的科目行下表
                        int tempIndex = 0;
                        FD_SettleReceiptDetail tempData = null;
                        decimal tempCreditAmount = 0;
                        decimal tempDebitAmount = 0;
                        for (int i = 0; i < settleReceipt.details.Count; i++)
                        {
                            var temp = settleReceipt.details[i];
                            if (tempData == null)
                            {
                                tempData = temp;
                                tempIndex = i;
                                continue;
                            }
                            if (temp.AccoSubjectID == tempData.AccoSubjectID && temp.PersonID == tempData.PersonID && temp.MarketID == tempData.MarketID && temp.CustomerID == tempData.CustomerID && temp.ProjectID == tempData.ProjectID && temp.ProductID == tempData.ProductID && temp.AccountID == tempData.AccountID && temp.PaymentTypeID == tempData.PaymentTypeID && temp.EnterpriseID == tempData.EnterpriseID)
                            {
                                tempDebitAmount += temp.Debit;
                                tempCreditAmount += temp.Credit;
                                settleReceipt.details.Remove(temp);
                                i--;
                                if (i+1 == settleReceipt.details.Count)
                                {
                                    settleReceipt.details[tempIndex].Credit += tempCreditAmount;
                                    settleReceipt.details[tempIndex].Debit += tempDebitAmount;
                                }
                            }
                            else
                            {
                                settleReceipt.details[tempIndex].Credit += tempCreditAmount;
                                settleReceipt.details[tempIndex].Debit += tempDebitAmount;
                                tempData = temp;
                                tempCreditAmount = 0;
                                tempDebitAmount = 0;
                                tempIndex = i;
                            }
                        }
                    }
                    _logger.LogInformation($@"操作记录：凭证整理生成会计凭证操作,\n 
                    UUID={uuid}\n
                    当多张生成凭证的时候走这个逻辑，\n
                    时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},\n 
                    操作人信息：{JsonConvert.SerializeObject(_identityService)},\n
                    传入参数：{JsonConvert.SerializeObject(request)},\n
                    保存后参数：{JsonConvert.SerializeObject(settleReceipt)},\n
                    ");
                    return new Result() { code = ErrorCode.Success.GetIntValue(),msg = "获取数据成功",data = new { settleReceipt } };
                }

                
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = "保存失败,请联系管理员！";
                result.data = ex;
            }
            return result;
        }
    }
}
