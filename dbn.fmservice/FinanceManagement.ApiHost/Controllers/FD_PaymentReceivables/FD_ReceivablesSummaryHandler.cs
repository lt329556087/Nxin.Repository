using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.Common;
using FinanceManagement.Common.MakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.Util;
using MediatR;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables
{
    public class FD_ReceivablesSummaryAddHandler : IRequestHandler<FD_ReceivablesSummaryAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ReceivablesRepository _repository;
        IFD_ReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_ReceivablesExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        Ifd_bankreceivableRepository _BankreceivableRepository;
        FMBaseCommon _baseUnit;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        FD_SettleReceiptNewODataProvider _provider;
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;
        public FD_ReceivablesSummaryAddHandler(HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil, FD_SettleReceiptNewODataProvider provider, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository, Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository, FMBaseCommon baseUnit,Ifd_bankreceivableRepository BankreceivableRepository,IBiz_RelatedDetailRepository biz_RelatedDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_ReceivablesExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _expenseRepository = expenseRepository;
            _ibsfileRepository = ibsfileRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _BankreceivableRepository = BankreceivableRepository;
            _baseUnit = baseUnit;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _provider = provider;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result> Handle(FD_ReceivablesSummaryAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //启用，则收付款单（不含收付款汇总单）自动生成会计凭证；
                //未启用，则收付款单需要到凭证处理形成一条待生成凭证的记录，且收付款单借贷方科目不展示
                //var option = _baseUnit.OptionConfigValue("201612270104402002", _identityService.GroupId);
                //开启状态  汇总不需要此逻辑
                bool optionStatus = true;
                //开启系统选项
                //if (option != "0" && !string.IsNullOrEmpty(option))
                //{
                //    optionStatus = true;
                //}
                #region 结账控制,序时控制
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.OwnerID), DataDate = Convert.ToDateTime(request.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.OwnerID}/FM_AccoCheck/IsLockForm";
                var ress = _httpClientUtil.PostJsonAsync<ResultModel>(requrl, param).Result;
                if ((bool)ress?.ResultState)
                {
                    return new Result()
                    {
                        msg = $"当前会计期间已结账，请先取消结账",
                        code = ErrorCode.RequestArgumentError.GetIntValue(),
                    };
                }
                if (_provider.SequentialTime(request.DataDate.ToString("yyyy-MM-dd"), request.EnterpriseID, request.SettleReceipType))
                {
                    return new Result()
                    {
                        msg = $"请遵循序时原则",
                        code = ErrorCode.RequestArgumentError.GetIntValue(),
                    };
                }
                #endregion
                long number = _numberCreator.Create<Domain.FD_PaymentReceivables>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == request.EnterpriseID);
                //默认收款凭证
                long settleNumber = Convert.ToInt64(_provider.GetMaxNumberByDate("201610220104402201", request.EnterpriseID, request.BeginDate, request.EndDate, request.TicketedPointID).MaxNumber);
                var numericalOrder = _numericalOrderCreator.CreateAsync().Result;
                var accoList = await _baseUnit.GetSubjectListExtend(0, Convert.ToInt64(string.IsNullOrEmpty(request.EnterpriseID) ? _identityService.EnterpriseId : request.EnterpriseID), request.DataDate.ToString("yyyy-MM-dd"));
                UploadInfo up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<UploadInfo>(request.UploadInfo);
                }
                var domain = new Domain.FD_PaymentReceivables()
                {
                    TicketedPointID = request.TicketedPointID,
                    Number = number.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    Remarks = request.Remarks,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = request.EnterpriseID,
                    IsPush = request.IsPush,
                    SettleReceipType = request.SettleReceipType,
                    IsGroupPay = true
                };

                //提取收付方信息
                foreach (var item in request.extend)
                {
                    foreach (var sp in item.CollectionId.Split(','))
                    {
                        foreach (var tx in request.details.Where(m => m.CollectionId == sp).ToList())
                        {
                            tx.AccountID = string.IsNullOrEmpty(item.AccountID) ? "0" : item.AccountID;
                            //tx.PaymentTypeID = item.PaymentTypeID;
                            tx.ExtendAccoSubjectID = item.AccoSubjectID;
                            //tx.AccoSubjectCode = item.AccoSubjectCode;
                            item.EnterpriseID = tx.EnterpriseID;
                        }
                    }
                }
                foreach (var item in request.details)
                {
                    item.Guid = Guid.NewGuid();
                    var o = item;
                    domain.details.Add(new FD_PaymentReceivablesDetail() 
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = request.extend.Where(m=>m.CollectionId.Contains(item.CollectionId)).FirstOrDefault().AccountID,
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                        Amount = o.Amount,
                        BusinessType = request.BusinessType,
                        Charges = o.Charges,
                        Content = string.IsNullOrEmpty(o.Content) ? "0" : o.Content,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        AttachCount = o.AttachCount,
                        PaymentTypeID = request.extend.Where(m => m.CollectionId.Contains(item.CollectionId)).FirstOrDefault().PaymentTypeID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        AccoSubjectID =string.IsNullOrEmpty(o.AccoSubjectID) ? "0" : o.AccoSubjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        AccoSubjectCode = string.IsNullOrEmpty(o.AccoSubjectCode) ? "0" : o.AccoSubjectCode,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,

                    });
                    domain.extend.Add(new Domain.FD_PaymentExtend()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        Amount = Convert.ToDecimal(o.Amount),
                        CollectionId = o.CollectionId,
                        PersonId = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        AccoSubjectID = o.ExtendAccoSubjectID,
                        ExtendAccoSubjectID = o.ExtendAccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        ProductID = o.ProductID,
                        ProjectID = o.ProjectID,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        AccountName = o.AccountName,
                        BankAccount = o.BankAccount,
                        BankDeposit = o.BankDeposit,
                        Charges = o.Charges,
                        AccountID = o.AccountID
                    });
                }

                #region 扩展存储
                var voucherDomain = new Domain.fd_paymentreceivablesvoucher()
                {
                    NumericalOrder = numericalOrder,
                    Number = settleNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = request.TicketedPointID,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SettleReceipType = "201610220104402201",
                    details = new List<fd_paymentreceivablesvoucherdetail>()
                };

                int i = 0;
                //收方信息 借方
                foreach (var itemGroup in domain.extend.ToList())
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == itemGroup.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        tempAcc = new BIZ_AccoSubject();
                    }
                    voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = itemGroup.Guid,
                        AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                        EnterpriseID = domain.details[i].EnterpriseID,
                        ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                        AccoSubjectID = itemGroup.ExtendAccoSubjectID,
                        AccoSubjectCode = itemGroup.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                        ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                        Credit = 0,
                        Debit = itemGroup.Amount,
                        LorR = false,
                        RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                        Content = domain.details[i].Content,
                        PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                        //Remarks = o.Remarks,
                    });
                    if (itemGroup.Charges > 0)
                    {
                        voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            NumericalOrder = numericalOrder,
                            Guid = itemGroup.Guid,
                            AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                            EnterpriseID = domain.details[i].EnterpriseID,
                            ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                            AccoSubjectID = request.extend.Where(m => m.CollectionId.Contains(request.details[i].CollectionId)).FirstOrDefault().CostAccoSubjectID,
                            AccoSubjectCode = itemGroup.AccoSubjectCode,
                            CustomerID = "0",
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                            ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                            Credit = 0,
                            Debit = itemGroup.Charges == null ? 0 : Convert.ToDecimal(itemGroup.Charges),
                            LorR = false,
                            RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                            OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                            Content = domain.details[i].Content,
                            PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                            //Remarks = o.Remarks,
                        });
                    }
                    i++;
                }
                i = 0;
                //付款信息 贷方
                foreach (var item in request.details)
                {
                    var str = JsonConvert.SerializeObject(item);
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == item.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        //如果为空 则默认资金科目
                        tempAcc = new BIZ_AccoSubject() { bTorF = true };
                    }
                    voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = item.Guid,
                        AccountID = "0",
                        EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? "0" : item.EnterpriseID,
                        ReceiptAbstractID = domain.details[0].ReceiptAbstractID,
                        AccoSubjectID = item.AccoSubjectID,
                        AccoSubjectCode = item.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(request.details[i].SettleCustomerID) ? "0" : request.details[i].SettleCustomerID,
                        PersonID = string.IsNullOrEmpty(request.details[i].SettlePersonID) ? "0" : request.details[i].SettlePersonID,
                        MarketID = string.IsNullOrEmpty(request.details[i].SettleMarketID) ? "0" : request.details[i].SettleMarketID,
                        ProjectID = string.IsNullOrEmpty(item.ProjectID) ? "0" : item.ProjectID,
                        ProductID = string.IsNullOrEmpty(item.ProductID) ? "0" : item.ProductID,
                        Content = item.Content,
                        PaymentTypeID = tempAcc.bTorF ? item.PaymentTypeID : "0",
                        Credit = Convert.ToDecimal(item.Amount + (item.Charges == null ? 0.00M : item.Charges)),
                        Debit = 0,
                        LorR = true,
                        RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(item.OrganizationSortID) ? "0" : item.OrganizationSortID,
                        //Remarks = o.Remarks,
                    });
                    i++;
                }
                #endregion

                var settleDomain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Number = settleNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = request.TicketedPointID,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SettleReceipType = "201610220104402201",
                    details = new List<FD_SettleReceiptDetail>()
                };

                i = 0;
                //收方信息 借方
                foreach (var itemGroup in domain.extend.ToList())
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == itemGroup.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        tempAcc = new BIZ_AccoSubject();
                    }
                    settleDomain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = itemGroup.Guid,
                        AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                        EnterpriseID = domain.details[i].EnterpriseID,
                        ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                        AccoSubjectID = itemGroup.ExtendAccoSubjectID,
                        AccoSubjectCode = itemGroup.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                        ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                        Credit = 0,
                        Debit = itemGroup.Amount,
                        LorR = false,
                        RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                        Content = domain.details[i].Content,
                        PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                        //Remarks = o.Remarks,
                    });
                    if (itemGroup.Charges > 0)
                    {
                        settleDomain.details.Add(new FD_SettleReceiptDetail()
                        {
                            NumericalOrder = numericalOrder,
                            Guid = itemGroup.Guid,
                            AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                            EnterpriseID = domain.details[i].EnterpriseID,
                            ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                            AccoSubjectID = request.extend.Where(m => m.CollectionId.Contains(request.details[i].CollectionId)).FirstOrDefault().CostAccoSubjectID,
                            AccoSubjectCode = itemGroup.AccoSubjectCode,
                            CustomerID = "0",
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                            ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                            Credit = 0,
                            Debit = itemGroup.Charges == null ? 0 : Convert.ToDecimal(itemGroup.Charges),
                            LorR = false,
                            RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                            OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                            Content = domain.details[i].Content,
                            PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                            //Remarks = o.Remarks,
                        });
                    }
                    i++;
                }
                i = 0;
                //付款信息 贷方
                foreach (var item in request.details)
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == item.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        //如果为空 则默认资金科目
                        tempAcc = new BIZ_AccoSubject() { bTorF = true };
                    }
                    settleDomain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = item.Guid,
                        AccountID =  "0" ,
                        EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? "0" : item.EnterpriseID,
                        ReceiptAbstractID = domain.details[0].ReceiptAbstractID,
                        AccoSubjectID = item.AccoSubjectID,
                        AccoSubjectCode = item.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(request.details[i].SettleCustomerID) ? "0" : request.details[i].SettleCustomerID,
                        PersonID = string.IsNullOrEmpty(request.details[i].SettlePersonID) ? "0" : request.details[i].SettlePersonID,
                        MarketID = string.IsNullOrEmpty(request.details[i].SettleMarketID) ? "0" : request.details[i].SettleMarketID,
                        ProjectID = string.IsNullOrEmpty(item.ProjectID) ? "0" : item.ProjectID,    
                        ProductID = string.IsNullOrEmpty(item.ProductID) ? "0" : item.ProductID,
                        Content = item.Content,
                        PaymentTypeID = tempAcc.bTorF ? item.PaymentTypeID : "0",
                        Credit = Convert.ToDecimal(item.Amount + (item.Charges == null ? 0.00M : item.Charges) ),
                        Debit = 0,
                        LorR = true,
                        RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(item.OrganizationSortID) ? "0" : item.OrganizationSortID,
                        //Remarks = o.Remarks,
                    });
                    i++;
                }
                if (up != null)
                {
                    await _ibsfileRepository.AddAsync(new bsfile()
                    {
                        Guid = Guid.NewGuid(),
                        EnterId = _identityService.EnterpriseId,
                        NumericalOrder = numericalOrder,
                        Type = 2,
                        FileName = up.FileName,
                        FilePath = up.PathUrl,
                        OwnerID = _identityService.UserId,
                        //Remarks = request.UploadInfo
                    });
                }
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _paymentExtendRepository.AddRangeAsync(domain.extend);
                await _paymentreceivablesvoucherRepository.AddAsync(voucherDomain);
                await _paymentreceivablesvoucherDetailRepository.AddRangeAsync(voucherDomain.details);
                await _settleReceiptRepository.AddAsync(settleDomain);
                await _settleReceiptDetailRepository.AddRangeAsync(settleDomain.details);
                
                if (up != null)
                {
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                //付款单
                Biz_Review review = new Biz_Review(numericalOrder, request.AppId, _identityService.UserId).SetMaking();
                //凭证
                Biz_Review review2 = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_ReviewRepository.AddAsync(review2);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();
                await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
                if (!string.IsNullOrEmpty(request.ApplyAppId))
                {
                    var DataList = _detailRepository.GetDetails(numericalOrder);
                    List<biz_relateddetail> relateddetail = new();

                    foreach (var item in request.details)
                    {
                        foreach (var ritem in item.RelatedList)
                        {
                            var related = new BIZ_Related()
                            {
                                RelatedType = "201610210104402122",
                                ParentType = request.AppId,//收款或收款汇总appid
                                ParentValue = numericalOrder,//收款单/收款汇总 流水号
                                ChildType = request.ApplyAppId,//申请单appid
                                ChildValue = ritem.ApplyNumericalOrder,//申请流水号
                                ParentValueDetail = DataList.Where(m => m.Guid == item.Guid).FirstOrDefault().RecordID.ToString(),
                                Remarks = "销售单引用到集中收款单"
                                //ParentValueDetail = numericalOrder
                            };
                            await _biz_RelatedRepository.AddAsync(related);
                            await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                            var rData = _biz_RelatedRepository.GetList(related).FirstOrDefault();
                            relateddetail.Add(new biz_relateddetail()
                            {
                                ModifiedDate = DateTime.Now,
                                OwnerID = Convert.ToInt64(_identityService.UserId),
                                Paid = ritem.Paid,
                                Payment = ritem.Payment,
                                Payable = ritem.Payable,
                                RelatedID = rData.RelatedID,
                                RelatedDetailID = DataList.Where(m => m.Guid == item.Guid).FirstOrDefault().RecordID,
                                RelatedDetailType = Convert.ToInt64(numericalOrder),
                                Remarks = "销售单-集中收款单关联详情"
                            });
                        }
                    }
                    await _biz_RelatedDetailRepository.AddRangeAsync(relateddetail);
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                if (!string.IsNullOrEmpty(request.BankNumericalOrder))
                {
                    foreach (var item in request.BankNumericalOrder.Split(','))
                    {
                        var bankData = _BankreceivableRepository.Get(item);
                        if (bankData != null)
                        {
                            bankData.SourceNum = numericalOrder;
                            bankData.IsGenerate = 1;
                            _BankreceivableRepository.Update(bankData);
                        }
                    }
                    await _BankreceivableRepository.UnitOfWork.SaveChangesAsync();
                }
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                #region 商城关系处理
                if (request.ApplyScAppId == "201612070104402204" && string.IsNullOrEmpty(request.ApplyAppId))
                {
                    var query = _detailRepository.GetDetails(numericalOrder);
                    for (int x = 0; x < request.details.Count; x++)
                    {
                        var details = request.details[x];
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//付款或付款汇总appid
                            ParentValue = numericalOrder,//付款单/付款汇总 流水号
                            ParentValueDetail = query.FirstOrDefault(m => m.Guid == details.Guid).RecordID.ToString(),
                            ChildType = request.ApplyScAppId,//商城APPID
                            ChildValue = details.ApplyScNumericalOrder,//商城支付单号
                            ChildValueDetail = details.ApplyScOrderNo,//商场单号
                            Remarks = "商城订单生成付款单"
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                    }
                    var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                }
                #endregion
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                var error = new ErrorRow();
                error.columns = new List<ErrorColumn>();
                error.columns.Add(new ErrorColumn());
                error.columns[0].value = ex;
                error.columns[0].name = "参数" + JsonConvert.SerializeObject(request);
                result.errors.Add(error);
                result.msg = _repository.ExceptionMessageHandle("保存异常", JsonConvert.SerializeObject(ex));
            }

            return result;
        }
    }

    public class FD_ReceivablesSummaryDeleteHandler : IRequestHandler<FD_ReceivablesDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ReceivablesRepository _repository;
        IFD_ReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_ReceivablesExtendRepository _paymentExtendRepository;
        IbsfileRepository _ibsfileRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        Ifd_bankreceivableRepository _BankreceivableRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        FD_PaymentReceivablesTODataProvider _prodiver;
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;
        IFD_PaymentExtendRepository _extendRepository;
        public FD_ReceivablesSummaryDeleteHandler(IFD_PaymentExtendRepository extendRepository, HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil, FD_PaymentReceivablesTODataProvider prodiver,Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,Ifd_bankreceivableRepository BankreceivableRepository,IBiz_RelatedDetailRepository biz_RelatedDetailRepository,IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ReceivablesExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _ibsfileRepository = ibsfileRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _BankreceivableRepository = BankreceivableRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _prodiver = prodiver;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _paymentExtendRepository = paymentExtendRepository;
            _extendRepository = extendRepository;
        }

        public async Task<Result> Handle(FD_ReceivablesDeleteCommand request, CancellationToken cancellationToken)
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
                    var data = _prodiver.GetSummaryDataAsync(Convert.ToInt64(num)).Result;
                    #region 结账控制,序时控制
                    var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(data.EnterpriseID), OwnerID = Convert.ToInt64(data.OwnerID), DataDate = Convert.ToDateTime(data.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                    string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{data.EnterpriseID}/{_identityService.UserId}/FM_AccoCheck/IsLockForm";
                    var ress = _httpClientUtil.PostJsonAsync<ResultModel>(requrl, param).Result;
                    if ((bool)ress?.ResultState)
                    {
                        return new Result()
                        {
                            msg = $"当前会计期间已结账，请先取消结账",
                            code = ErrorCode.RequestArgumentError.GetIntValue(),
                        };
                    }
                    #endregion
                    var details = _prodiver.GetReceivablesSummaryDetaiDatasAsync(Convert.ToInt64(num)).Result;
                    var extend = _prodiver.GetPaymentExtendSummaryDatas(Convert.ToInt64(num));
                    var extends = _extendRepository.GetDetails(num);

                    await _paymentreceivablesvoucherRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();

                    await _paymentreceivablesvoucherDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();


                    await _settleReceiptDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();

                    await _settleReceiptRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();


                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _paymentExtendRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.ParentValue == num);
                    await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(num));
                    var temp = _BankreceivableRepository.GetDataBySourceNum(num);
                    if (temp != null)
                    {
                        temp.IsGenerate = 2;
                        temp.SourceNum = "";
                        _BankreceivableRepository.Update(temp);
                    }
                }
                await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                await _BankreceivableRepository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "删除成功";
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                var error = new ErrorRow();
                error.columns = new List<ErrorColumn>();
                error.columns.Add(new ErrorColumn());
                error.columns[0].value = ex;
                error.columns[0].name = "参数" + JsonConvert.SerializeObject(request);
                result.errors.Add(error);
                result.msg = _repository.ExceptionMessageHandle("保存异常", JsonConvert.SerializeObject(ex));
            }

            return result;
        }
    }

    public class FD_ReceivablesSummaryModifyHandler : IRequestHandler<FD_ReceivablesSummaryModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_ReceivablesRepository _repository;
        IFD_ReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_ReceivablesExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        Ifd_bankreceivableRepository _BankreceivableRepository;
        FMBaseCommon _baseUnit;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;
        FD_PaymentReceivablesTODataProvider _prodiver;
        IFD_PaymentExtendRepository _extendRepository;
        FD_SettleReceiptNewODataProvider _provider;
        public FD_ReceivablesSummaryModifyHandler(FD_SettleReceiptNewODataProvider provider,IFD_PaymentExtendRepository extendRepository,FD_PaymentReceivablesTODataProvider prodiver,HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,FMBaseCommon baseUnit, Ifd_bankreceivableRepository BankreceivableRepository,IBiz_RelatedDetailRepository biz_RelatedDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_ReceivablesExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _expenseRepository = expenseRepository;
            _ibsfileRepository = ibsfileRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _BankreceivableRepository = BankreceivableRepository;
            _baseUnit = baseUnit;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _prodiver = prodiver;
            _extendRepository = extendRepository;
            _provider = provider;
        }

        public async Task<Result> Handle(FD_ReceivablesSummaryModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //启用，则收付款单（不含收付款汇总单）自动生成会计凭证；
                //未启用，则收付款单需要到凭证处理形成一条待生成凭证的记录，且收付款单借贷方科目不展示
                //var option = _baseUnit.OptionConfigValue("201612270104402002", _identityService.GroupId);
                //开启状态
                bool optionStatus = true;
                //开启系统选项
                //if (option != "0" && !string.IsNullOrEmpty(option))
                //{
                //    optionStatus = true;
                //}
                var data = _prodiver.GetSummaryDataAsync(Convert.ToInt64(request.NumericalOrder)).Result;
                var oldDomain = _repository.Get(request.NumericalOrder);
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                #region 结账控制,序时控制
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.OwnerID), DataDate = Convert.ToDateTime(request.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.OwnerID}/FM_AccoCheck/IsLockForm";
                var ress = _httpClientUtil.PostJsonAsync<ResultModel>(requrl, param).Result;
                if ((bool)ress?.ResultState)
                {
                    return new Result()
                    {
                        msg = $"当前会计期间已结账，请先取消结账",
                        code = ErrorCode.RequestArgumentError.GetIntValue(),
                    };
                }
                if (oldDomain.SettleReceipType != request.SettleReceipType || oldDomain.DataDate != request.DataDate || oldDomain.EnterpriseID != request.EnterpriseID)
                {
                    if (_provider.SequentialTime(request.DataDate.ToString("yyyy-MM-dd"), request.EnterpriseID, request.SettleReceipType))
                    {
                        return new Result()
                        {
                            msg = $"请遵循序时原则",
                            code = ErrorCode.RequestArgumentError.GetIntValue(),
                        };
                    }
                }
                #endregion
                var oldSettleDomain = _settleReceiptRepository.Get(request.NumericalOrder);
                if (data == null || oldSettleDomain == null)
                {
                    return new Result() { msg = "当期单据已被修改，请重新查列表" };
                }
                var accoList = await _baseUnit.GetSubjectListExtend(0, Convert.ToInt64(string.IsNullOrEmpty(request.EnterpriseID) ? _identityService.EnterpriseId : request.EnterpriseID), request.DataDate.ToString("yyyy-MM-dd"));
                UploadInfo up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<UploadInfo>(request.UploadInfo);
                }
                var lists = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    lists = request.NumericalOrder.Split(',').ToList();
                }

                

                string number = data.Number;
                string settleNumber = oldSettleDomain.Number;
                var numericalOrder = request.NumericalOrder;
                if (string.IsNullOrEmpty(request.ApplyAppId))
                {
                    request.ApplyAppId = "0";
                }
                if (string.IsNullOrEmpty(request.ApplyNumericalOrder))
                {
                    request.ApplyNumericalOrder = "0";
                }
                #region 金融充值 先退回
                var details = _detailRepository.GetDetails(request.NumericalOrder);
                var extend = _extendRepository.GetDetails(request.NumericalOrder);
                #endregion
                //先增 后删，防止修改保存 老数据丢失
                foreach (var num in lists)
                {

                    await _paymentreceivablesvoucherRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();

                    await _paymentreceivablesvoucherDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();

                    await _settleReceiptDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();

                    await _settleReceiptRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();

                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _paymentExtendRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402122" && o.ParentType == request.AppId && o.ChildType == request.ApplyAppId && o.ParentValue == num);
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                    await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(num));
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                var domain = new Domain.FD_PaymentReceivables()
                {
                    TicketedPointID = request.TicketedPointID,
                    Number = number.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    Remarks = request.Remarks,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = request.EnterpriseID,
                    IsPush = request.IsPush,
                    SettleReceipType = request.SettleReceipType,
                    IsGroupPay = true
                };

                //提取收付方信息
                foreach (var item in request.extend)
                {
                    foreach (var sp in item.CollectionId.Split(','))
                    {
                        foreach (var tx in request.details.Where(m => m.CollectionId == sp).ToList())
                        {
                            tx.AccountID = string.IsNullOrEmpty(item.AccountID) ? "0" : item.AccountID;
                            //tx.PaymentTypeID = item.PaymentTypeID;
                            tx.ExtendAccoSubjectID = item.AccoSubjectID;
                            //tx.AccoSubjectCode = item.AccoSubjectCode;
                            item.EnterpriseID = tx.EnterpriseID;
                        }
                    }
                }
                foreach (var item in request.details)
                {
                    item.Guid = Guid.NewGuid();
                    var o = item;
                    domain.details.Add(new FD_PaymentReceivablesDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = request.extend.Where(m => m.CollectionId.Contains(item.CollectionId)).FirstOrDefault().AccountID,
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                        Amount = o.Amount,
                        BusinessType = request.BusinessType,
                        Charges = o.Charges,
                        Content = string.IsNullOrEmpty(o.Content) ? "0" : o.Content,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        AttachCount = o.AttachCount,
                        PaymentTypeID = request.extend.Where(m => m.CollectionId.Contains(item.CollectionId)).FirstOrDefault().PaymentTypeID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        ReceiptAbstractID = string.IsNullOrEmpty(o.ReceiptAbstractID) ? "0" : o.ReceiptAbstractID,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        AccoSubjectID = string.IsNullOrEmpty(o.AccoSubjectID) ? "0" : o.AccoSubjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        AccoSubjectCode = string.IsNullOrEmpty(o.AccoSubjectCode) ? "0" : o.AccoSubjectCode,
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,

                    });
                    domain.extend.Add(new Domain.FD_PaymentExtend()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        Amount = Convert.ToDecimal(o.Amount),
                        CollectionId = o.CollectionId,
                        PersonId = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        AccoSubjectID = o.ExtendAccoSubjectID,
                        ExtendAccoSubjectID = o.ExtendAccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        ProductID = o.ProductID,
                        ProjectID = o.ProjectID,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        AccountName = o.AccountName,
                        BankAccount = o.BankAccount,
                        BankDeposit = o.BankDeposit,
                        Charges = o.Charges,
                        AccountID = o.AccountID
                    });
                }
                #region 扩展存储
                var voucherDomain = new Domain.fd_paymentreceivablesvoucher()
                {
                    NumericalOrder = numericalOrder,
                    Number = settleNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = request.TicketedPointID,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SettleReceipType = "201610220104402201",
                    details = new List<fd_paymentreceivablesvoucherdetail>()
                };

                int i = 0;
                //收方信息 借方
                foreach (var itemGroup in domain.extend.ToList())
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == itemGroup.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        tempAcc = new BIZ_AccoSubject();
                    }
                    voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = itemGroup.Guid,
                        AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                        EnterpriseID = domain.details[i].EnterpriseID,
                        ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                        AccoSubjectID = itemGroup.ExtendAccoSubjectID,
                        AccoSubjectCode = itemGroup.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                        ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                        Credit = 0,
                        Debit = itemGroup.Amount,
                        LorR = false,
                        RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                        Content = domain.details[i].Content,
                        PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                        //Remarks = o.Remarks,
                    });
                    if (itemGroup.Charges > 0)
                    {
                        voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            NumericalOrder = numericalOrder,
                            Guid = itemGroup.Guid,
                            AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                            EnterpriseID = domain.details[i].EnterpriseID,
                            ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                            AccoSubjectID = request.extend.Where(m => m.CollectionId.Contains(request.details[i].CollectionId)).FirstOrDefault().CostAccoSubjectID,
                            AccoSubjectCode = itemGroup.AccoSubjectCode,
                            CustomerID = "0",
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                            ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                            Credit = 0,
                            Debit = itemGroup.Charges == null ? 0 : Convert.ToDecimal(itemGroup.Charges),
                            LorR = false,
                            RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                            OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                            Content = domain.details[i].Content,
                            PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                            //Remarks = o.Remarks,
                        });
                    }
                    i++;
                }
                i = 0;
                //付款信息 贷方
                foreach (var item in request.details)
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == item.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        //如果为空 则默认资金科目
                        tempAcc = new BIZ_AccoSubject() { bTorF = true };
                    }
                    voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = item.Guid,
                        AccountID = "0",
                        EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? "0" : item.EnterpriseID,
                        ReceiptAbstractID = domain.details[0].ReceiptAbstractID,
                        AccoSubjectID = item.AccoSubjectID,
                        AccoSubjectCode = item.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(request.details[i].SettleCustomerID) ? "0" : request.details[i].SettleCustomerID,
                        PersonID = string.IsNullOrEmpty(request.details[i].SettlePersonID) ? "0" : request.details[i].SettlePersonID,
                        MarketID = string.IsNullOrEmpty(request.details[i].SettleMarketID) ? "0" : request.details[i].SettleMarketID,
                        ProjectID = string.IsNullOrEmpty(item.ProjectID) ? "0" : item.ProjectID,
                        ProductID = string.IsNullOrEmpty(item.ProductID) ? "0" : item.ProductID,
                        Content = item.Content,
                        PaymentTypeID = tempAcc.bTorF ? item.PaymentTypeID : "0",
                        Credit = Convert.ToDecimal(item.Amount + (item.Charges == null ? 0.00M : item.Charges)),
                        Debit = 0,
                        LorR = true,
                        RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(item.OrganizationSortID) ? "0" : item.OrganizationSortID,
                        //Remarks = o.Remarks,
                    });
                    i++;
                }
                #endregion
                var settleDomain = new Domain.FD_SettleReceipt()
                {
                    NumericalOrder = numericalOrder,
                    Number = settleNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = request.TicketedPointID,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SettleReceipType = "201610220104402201",
                    details = new List<FD_SettleReceiptDetail>()
                };

                i = 0;
                //收方信息 借方
                foreach (var itemGroup in domain.extend.ToList())
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == itemGroup.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        tempAcc = new BIZ_AccoSubject();
                    }
                    settleDomain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = itemGroup.Guid,
                        AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                        EnterpriseID = domain.details[i].EnterpriseID,
                        ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                        AccoSubjectID = itemGroup.ExtendAccoSubjectID,
                        AccoSubjectCode = itemGroup.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                        ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                        Credit = 0,
                        Debit = itemGroup.Amount,
                        LorR = false,
                        RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                        Content = domain.details[i].Content,
                        PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                        //Remarks = o.Remarks,
                    });
                    if (itemGroup.Charges > 0)
                    {
                        settleDomain.details.Add(new FD_SettleReceiptDetail()
                        {
                            NumericalOrder = numericalOrder,
                            Guid = itemGroup.Guid,
                            AccountID = string.IsNullOrEmpty(itemGroup.AccountID) ? "0" : itemGroup.AccountID,
                            EnterpriseID = domain.details[i].EnterpriseID,
                            ReceiptAbstractID = domain.details[i].ReceiptAbstractID,
                            AccoSubjectID = request.extend.Where(m => m.CollectionId.Contains(request.details[i].CollectionId)).FirstOrDefault().CostAccoSubjectID,
                            AccoSubjectCode = itemGroup.AccoSubjectCode,
                            CustomerID = "0",
                            PersonID = "0",
                            MarketID = "0",
                            ProjectID = string.IsNullOrEmpty(itemGroup.ProjectID) ? "0" : itemGroup.ProjectID,
                            ProductID = string.IsNullOrEmpty(itemGroup.ProductID) ? "0" : itemGroup.ProductID,
                            Credit = 0,
                            Debit = itemGroup.Charges == null ? 0 : Convert.ToDecimal(itemGroup.Charges),
                            LorR = false,
                            RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                            OrganizationSortID = string.IsNullOrEmpty(domain.details[i].OrganizationSortID) ? "0" : domain.details[i].OrganizationSortID,
                            Content = domain.details[i].Content,
                            PaymentTypeID = tempAcc.bTorF ? domain.details[i].PaymentTypeID : "0",
                            //Remarks = o.Remarks,
                        });
                    }
                    i++;
                }
                i = 0;
                //付款信息 贷方
                foreach (var item in request.details)
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == item.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        //如果为空 则默认资金科目
                        tempAcc = new BIZ_AccoSubject() { bTorF = true };
                    }
                    settleDomain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = item.Guid,
                        AccountID = "0",
                        EnterpriseID = string.IsNullOrEmpty(item.EnterpriseID) ? "0" : item.EnterpriseID,
                        ReceiptAbstractID = domain.details[0].ReceiptAbstractID,
                        AccoSubjectID = item.AccoSubjectID,
                        AccoSubjectCode = item.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(request.details[i].SettleCustomerID) ? "0" : request.details[i].SettleCustomerID,
                        PersonID = string.IsNullOrEmpty(request.details[i].SettlePersonID) ? "0" : request.details[i].SettlePersonID,
                        MarketID = string.IsNullOrEmpty(request.details[i].SettleMarketID) ? "0" : request.details[i].SettleMarketID,
                        ProjectID = string.IsNullOrEmpty(item.ProjectID) ? "0" : item.ProjectID,
                        ProductID = string.IsNullOrEmpty(item.ProductID) ? "0" : item.ProductID,
                        Content = item.Content,
                        PaymentTypeID = tempAcc.bTorF ? item.PaymentTypeID : "0",
                        Credit = Convert.ToDecimal(item.Amount + (item.Charges == null ? 0.00M : item.Charges)),
                        Debit = 0,
                        LorR = true,
                        RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(item.OrganizationSortID) ? "0" : item.OrganizationSortID,
                        //Remarks = o.Remarks,
                    });
                    i++;
                }

                if (up != null)
                {
                    await _ibsfileRepository.AddAsync(new bsfile()
                    {
                        Guid = Guid.NewGuid(),
                        EnterId = _identityService.EnterpriseId,
                        NumericalOrder = numericalOrder,
                        Type = 2,
                        FileName = up.FileName,
                        FilePath = up.PathUrl,
                        OwnerID = _identityService.UserId,
                        //Remarks = request.UploadInfo
                    });
                }
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _paymentExtendRepository.AddRangeAsync(domain.extend);
                await _paymentreceivablesvoucherRepository.AddAsync(voucherDomain);
                await _paymentreceivablesvoucherDetailRepository.AddRangeAsync(voucherDomain.details);
                await _settleReceiptRepository.AddAsync(settleDomain);
                await _settleReceiptDetailRepository.AddRangeAsync(settleDomain.details);
                
                if (up != null)
                {
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                //付款单
                Biz_Review review = new Biz_Review(numericalOrder, request.AppId, _identityService.UserId).SetMaking();
                //凭证
                Biz_Review review2 = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_ReviewRepository.AddAsync(review2);
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
               

                if (!string.IsNullOrEmpty(request.ApplyAppId) && request.ApplyAppId != "0")
                {
                    var DataList = _detailRepository.GetDetails(numericalOrder);
                    List<biz_relateddetail> relateddetail = new();

                    foreach (var item in request.details)
                    {
                        foreach (var ritem in item.RelatedList)
                        {
                            var related = new BIZ_Related()
                            {
                                RelatedType = "201610210104402122",
                                ParentType = request.AppId,//收款或收款汇总appid
                                ParentValue = numericalOrder,//收款单/收款汇总 流水号
                                ChildType = request.ApplyAppId,//申请单appid
                                ChildValue = ritem.ApplyNumericalOrder,//申请流水号
                                ParentValueDetail = DataList.Where(m => m.Guid == item.Guid).FirstOrDefault().RecordID.ToString(),
                                Remarks = "销售单引用到集中收款单"
                                //ParentValueDetail = numericalOrder
                            };
                            await _biz_RelatedRepository.AddAsync(related);
                            await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                            var rData = _biz_RelatedRepository.GetList(related).FirstOrDefault();
                            relateddetail.Add(new biz_relateddetail()
                            {
                                ModifiedDate = DateTime.Now,
                                OwnerID = Convert.ToInt64(_identityService.UserId),
                                Paid = ritem.Paid,
                                Payment = ritem.Payment,
                                Payable = ritem.Payable,
                                RelatedID = rData.RelatedID,
                                RelatedDetailID = DataList.Where(m => m.Guid == item.Guid).FirstOrDefault().RecordID,
                                RelatedDetailType = Convert.ToInt64(numericalOrder),
                                Remarks = "销售单-集中收款单关联详情"
                            });
                        }
                    }
                    await _biz_RelatedDetailRepository.AddRangeAsync(relateddetail);
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                //银行流水判断
                var bankData = _BankreceivableRepository.GetDataBySourceNum(request.NumericalOrder);
                if (bankData != null)
                {
                    bankData.SourceNum = numericalOrder;
                    _BankreceivableRepository.Update(bankData);
                    await _BankreceivableRepository.UnitOfWork.SaveChangesAsync();
                }
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
            }

            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                var error = new ErrorRow();
                error.columns = new List<ErrorColumn>();
                error.columns.Add(new ErrorColumn());
                error.columns[0].value = ex;
                error.columns[0].name = "参数"+JsonConvert.SerializeObject(request);
                result.errors.Add(error);
                result.msg = _repository.ExceptionMessageHandle("保存异常", JsonConvert.SerializeObject(ex));
            }
            return result;
        }
    }


}
