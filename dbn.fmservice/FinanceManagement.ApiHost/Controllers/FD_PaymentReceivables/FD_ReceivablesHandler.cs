using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables;
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

namespace FinanceManagement.ApiHost.Controllers.FD_Receivables
{
    public class FD_ReceivablesAddHandler : IRequestHandler<FD_ReceivablesAddCommand, Result>
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
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        Ifd_bankreceivableRepository _BankreceivableRepository;
        FMBaseCommon _baseUnit;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        FD_SettleReceiptNewODataProvider _provider;
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;
        public FD_ReceivablesAddHandler(HostConfiguration hostCongfiguration,HttpClientUtil httpClientUtil, FD_SettleReceiptNewODataProvider provider, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository, Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository, FMBaseCommon baseUnit, Ifd_bankreceivableRepository BankreceivableRepository,IBiz_RelatedDetailRepository biz_RelatedDetailRepository, Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
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
            _ScheduleplanRepository = scheduleplanRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _BankreceivableRepository = BankreceivableRepository;
            _baseUnit = baseUnit;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _provider = provider;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result> Handle(FD_ReceivablesAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //启用，则收付款单（不含收付款汇总单）自动生成会计凭证；
                //未启用，则收付款单需要到凭证处理形成一条待生成凭证的记录，且收付款单借贷方科目不展示
                var option = _baseUnit.OptionConfigValue("201612270104402002", (string.IsNullOrEmpty(_identityService.GroupId) || _identityService.GroupId == "0") ? request.GroupId : _identityService.GroupId);
                //开启状态
                bool optionStatus = false;
                //开启系统选项
                if (option != "0" && !string.IsNullOrEmpty(option))
                {
                    optionStatus = true;
                }
                //为了适配 银行收款自动生成收款
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId.ToString();
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
                var numericalOrder = string.IsNullOrEmpty(request.NumericalOrder) ? _numericalOrderCreator.CreateAsync().Result : request.NumericalOrder;
                var accoList = await _baseUnit.GetSubjectListExtend(0, Convert.ToInt64(string.IsNullOrEmpty(request.EnterpriseID) ? _identityService.EnterpriseId : request.EnterpriseID), request.DataDate.ToString("yyyy-MM-dd"));
                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                var domain = new Domain.FD_PaymentReceivables()
                {
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    Number = number.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    Remarks = request.Remarks,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID,
                    IsPush = request.IsPush,
                    SettleReceipType = request.SettleReceipType,
                    IsGroupPay = true
                };

                int i = 0;
                request.details?.ForEach(o =>
                {
                    o.Guid = Guid.NewGuid();
                    request.extend[i].Guid = o.Guid;
                    domain.details.Add(new FD_PaymentReceivablesDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = string.IsNullOrEmpty(o.AccountID) ? "0" : o.AccountID,
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                        Amount = o.Amount,
                        BusinessType = o.BusinessType,
                        Charges = o.Charges,
                        Content = o.Content,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        AttachCount = o.AttachCount,
                        PaymentTypeID = o.PaymentTypeID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        ReceiptAbstractID = o.ReceiptAbstractID,

                    });
                });
                request.extend?.ForEach(o =>
                {
                    domain.extend.Add(new Domain.FD_PaymentExtend()
                    {
                        AccountName = o.AccountName,
                        Amount = o.Amount,
                        BankAccount = o.BankAccount,
                        BankDeposit = o.BankDeposit,
                        CollectionId = string.IsNullOrEmpty(o.CollectionId) ? "0" : o.CollectionId,
                        NumericalOrder = numericalOrder,
                        PersonId = string.IsNullOrEmpty(o.PersonId) ? "0" : o.PersonId,
                        Guid = o.Guid,
                    });
                });

                var voucherDomain = new Domain.fd_paymentreceivablesvoucher()
                {
                    NumericalOrder = numericalOrder,
                    Number = settleNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = request.OwnerID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SettleReceipType = "201610220104402201",
                    details = new List<fd_paymentreceivablesvoucherdetail>()
                };
                var tempAcc = accoList.Where(m => m.AccoSubjectID == request.DebitAccoSubjectID).FirstOrDefault();
                if (tempAcc == null)
                {
                    //如果为空 则默认资金科目
                    tempAcc = new BIZ_AccoSubject() { bTorF = true };
                }
                //表体传过来的收款单信息都是贷方信息 汇总生成一行借方明细
                request.details?.ForEach(o =>
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == o.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        //如果为空 则默认资金科目
                        tempAcc = new BIZ_AccoSubject() { bTorF = true };
                    }
                    voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = tempAcc.bTorF ? string.IsNullOrEmpty(o.AccountID) ? "0" : o.AccountID : "0",
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                        PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                        MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        Content = o.Content,
                        PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                        Credit = o.Debit,
                        Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                        LorR = false,
                        RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                    });
                    //手续费另生成一条金额
                    if (o.Charges > 0)
                    {
                        voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            NumericalOrder = numericalOrder,
                            Guid = o.Guid,
                            AccountID = o.AccountID,
                            EnterpriseID = o.EnterpriseID,
                            ReceiptAbstractID = o.ReceiptAbstractID,
                            AccoSubjectID = o.CostAccoSubjectID,
                            AccoSubjectCode = o.AccoSubjectCode,
                            CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                            PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                            MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                            ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                            ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                            Content = o.Content,
                            PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                            Credit = o.Debit,
                            Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Charges),
                            LorR = false,
                            RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                            OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        });
                    }
                });
                voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    AccountID = tempAcc.bTorF ? request.details[0].AccountID : "0",
                    EnterpriseID = request.EnterpriseID,
                    ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                    AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                    AccoSubjectCode = request.details[0].AccoSubjectCode,
                    CustomerID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID : "0",
                    PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].PersonID) ? "0" : request.details[0].SettlePersonID : "0",
                    MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].MarketID) ? "0" : request.details[0].SettleMarketID : "0",
                    ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                    ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                    Content = request.details[0].Content,
                    PaymentTypeID = tempAcc.bTorF ? request.details[0].PaymentTypeID : "0",
                    Credit = Convert.ToDecimal(request.details.Sum(m => m.Amount)) + Convert.ToDecimal(request.details.Sum(m => m.Charges)),
                    Debit = 0,
                    LorR = true,
                    RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                    OrganizationSortID = string.IsNullOrEmpty(request.details[0].OrganizationSortID) ? "0" : request.details[0].OrganizationSortID,
                    Auxiliary1 = string.IsNullOrEmpty(request.Auxiliary1) ? "0" : request.Auxiliary1,
                    Auxiliary2 = string.IsNullOrEmpty(request.Auxiliary2) ? "0" : request.Auxiliary2,
                    Auxiliary3 = string.IsNullOrEmpty(request.Auxiliary3) ? "0" : request.Auxiliary3,
                    Auxiliary4 = string.IsNullOrEmpty(request.Auxiliary4) ? "0" : request.Auxiliary4,
                    Auxiliary5 = string.IsNullOrEmpty(request.Auxiliary5) ? "0" : request.Auxiliary5,
                    Auxiliary6 = string.IsNullOrEmpty(request.Auxiliary6) ? "0" : request.Auxiliary6,
                    Auxiliary7 = string.IsNullOrEmpty(request.Auxiliary7) ? "0" : request.Auxiliary7,
                    Auxiliary8 = string.IsNullOrEmpty(request.Auxiliary8) ? "0" : request.Auxiliary8,
                    Auxiliary9 = string.IsNullOrEmpty(request.Auxiliary9) ? "0" : request.Auxiliary9,
                    Auxiliary10 = string.IsNullOrEmpty(request.Auxiliary10) ? "0" : request.Auxiliary10,
                    //OrganizationSortID = o.OrganizationSortID,
                    //Remarks = o.Remarks,
                });
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = _identityService.EnterpriseId,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = request.OwnerID,
                            //Remarks = request.UploadInfo
                        });
                    }
                }
                //余额结算  金融充值接口，保存付款单 判断是否 收款方式=余额结算，是 就走金融充值接口
                //https://confluence.nxin.com/pages/viewpage.action?pageId=65055307
                var amountData = domain.details.Where(m => m.PaymentTypeID == "1912201259040000102");
                if (amountData.Count() > 0)
                {
                    domain.extend.FirstOrDefault().TradeNo = _numericalOrderCreator.CreateAsync().Result;
                }
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _paymentExtendRepository.AddRangeAsync(domain.extend);
                await _paymentreceivablesvoucherRepository.AddAsync(voucherDomain);
                await _paymentreceivablesvoucherDetailRepository.AddRangeAsync(voucherDomain.details);

                if (up != null)
                {
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }

                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                //收款单
                Biz_Review review = new Biz_Review(numericalOrder, request.AppId, request.OwnerID).SetMaking();
                if (optionStatus)
                {
                    Domain.FD_SettleReceipt settleDomain = CreateSettleReceipt(request, optionStatus, settleNumber, numericalOrder, accoList, tempAcc);
                    //凭证
                    Biz_Review review2 = new Biz_Review(numericalOrder, "1611091727140000101", request.OwnerID).SetMaking();
                    await _settleReceiptRepository.AddAsync(settleDomain);
                    await _settleReceiptDetailRepository.AddRangeAsync(settleDomain.details);
                    await _biz_ReviewRepository.AddAsync(review2);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(request.ApplyAppId))
                {
                    List<biz_relateddetail> relateddetail = new();

                    foreach (var ritem in request.RelatedList)
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//收款或收款汇总appid
                            ParentValue = numericalOrder,//收款单/收款汇总 流水号
                            ChildType = request.ApplyAppId,//申请单appid
                            ChildValue = ritem.ApplyNumericalOrder,//申请流水号
                            Remarks = "销售单引用到集中收款单"
                            //ParentValueDetail = numericalOrder
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                        await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                        var rData = _biz_RelatedRepository.GetList(related).FirstOrDefault();
                        relateddetail.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(request.OwnerID),
                            Paid = ritem.Paid,
                            Payment = ritem.Payment,
                            Payable = ritem.Payable,
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "销售单-集中收款单关联详情"
                        });
                    }
                    await _biz_RelatedDetailRepository.AddRangeAsync(relateddetail);
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                if (!string.IsNullOrEmpty(request.BankNumericalOrder))
                {
                    var bankData = _BankreceivableRepository.Get(request.BankNumericalOrder);
                    if (bankData != null)
                    {
                        bankData.SourceNum = numericalOrder;
                        bankData.IsGenerate = 1;
                        _BankreceivableRepository.Update(bankData);
                        await _BankreceivableRepository.UnitOfWork.SaveChangesAsync();
                    }
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

        private static Domain.FD_SettleReceipt CreateSettleReceipt(FD_ReceivablesAddCommand request, bool optionStatus, long settleNumber, string numericalOrder, List<BIZ_AccoSubject> accoList, BIZ_AccoSubject tempAcc)
        {
            var settleDomain = new Domain.FD_SettleReceipt()
            {
                NumericalOrder = numericalOrder,
                Number = settleNumber.ToString(),
                Guid = Guid.NewGuid(),
                DataDate = request.DataDate,
                TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                AttachmentNum = request.AttachmentNum,
                Remarks = request.Remarks,
                EnterpriseID = request.EnterpriseID,
                OwnerID = request.OwnerID,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                SettleReceipType = "201610220104402201",
                details = new List<FD_SettleReceiptDetail>()
            };
            
            //表体传过来的收款单信息都是贷方信息 汇总生成一行借方明细
            request.details?.ForEach(o =>
            {
                var tempAcc = accoList.Where(m => m.AccoSubjectID == o.AccoSubjectID).FirstOrDefault();
                if (tempAcc == null)
                {
                    //如果为空 则默认资金科目
                    tempAcc = new BIZ_AccoSubject() { bTorF = true };
                }
                settleDomain.details.Add(new FD_SettleReceiptDetail()
                {
                    NumericalOrder = numericalOrder,
                    Guid = o.Guid,
                    AccountID = tempAcc.bTorF ? string.IsNullOrEmpty(o.AccountID) ? "0" : o.AccountID : "0",
                    EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                    ReceiptAbstractID = o.ReceiptAbstractID,
                    AccoSubjectID = o.AccoSubjectID,
                    AccoSubjectCode = o.AccoSubjectCode,
                    CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                    PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                    MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                    ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                    ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                    Content = o.Content,
                    PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                    Credit = o.Debit,
                    Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                    LorR = false,
                    RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                    OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                });
                //手续费另生成一条金额
                if (o.Charges > 0)
                {
                    settleDomain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = o.AccountID,
                        EnterpriseID = o.EnterpriseID,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.CostAccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                        PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                        MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        Content = o.Content,
                        PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                        Credit = o.Debit,
                        Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Charges),
                        LorR = false,
                        RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                    });
                }
            });
            settleDomain.details.Add(new FD_SettleReceiptDetail()
            {
                NumericalOrder = numericalOrder,
                Guid = Guid.NewGuid(),
                AccountID = tempAcc.bTorF ? request.details[0].AccountID : "0",
                EnterpriseID = request.EnterpriseID,
                ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                AccoSubjectCode = request.details[0].AccoSubjectCode,
                CustomerID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID : "0",
                PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].PersonID) ? "0" : request.details[0].SettlePersonID : "0",
                MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].MarketID) ? "0" : request.details[0].SettleMarketID : "0",
                ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                Content = request.details[0].Content,
                PaymentTypeID = tempAcc.bTorF ? request.details[0].PaymentTypeID : "0",
                Credit = Convert.ToDecimal(request.details.Sum(m => m.Amount)) + Convert.ToDecimal(request.details.Sum(m => m.Charges)),
                Debit = 0,
                LorR = true,
                RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                OrganizationSortID = string.IsNullOrEmpty(request.details[0].OrganizationSortID) ? "0" : request.details[0].OrganizationSortID,
                Auxiliary1 = string.IsNullOrEmpty(request.Auxiliary1) ? "0" : request.Auxiliary1,
                Auxiliary2 = string.IsNullOrEmpty(request.Auxiliary2) ? "0" : request.Auxiliary2,
                Auxiliary3 = string.IsNullOrEmpty(request.Auxiliary3) ? "0" : request.Auxiliary3,
                Auxiliary4 = string.IsNullOrEmpty(request.Auxiliary4) ? "0" : request.Auxiliary4,
                Auxiliary5 = string.IsNullOrEmpty(request.Auxiliary5) ? "0" : request.Auxiliary5,
                Auxiliary6 = string.IsNullOrEmpty(request.Auxiliary6) ? "0" : request.Auxiliary6,
                Auxiliary7 = string.IsNullOrEmpty(request.Auxiliary7) ? "0" : request.Auxiliary7,
                Auxiliary8 = string.IsNullOrEmpty(request.Auxiliary8) ? "0" : request.Auxiliary8,
                Auxiliary9 = string.IsNullOrEmpty(request.Auxiliary9) ? "0" : request.Auxiliary9,
                Auxiliary10 = string.IsNullOrEmpty(request.Auxiliary10) ? "0" : request.Auxiliary10,
                //OrganizationSortID = o.OrganizationSortID,
                //Remarks = o.Remarks,
            });
            return settleDomain;
        }
    }

    public class FD_ReceivablesDeleteHandler : IRequestHandler<FD_ReceivablesDeleteCommand, Result>
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
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        Ifd_bankreceivableRepository _BankreceivableRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        HttpClientUtil _httpClientUtil;
        FD_PaymentReceivablesTODataProvider _prodiver;
        HostConfiguration _hostCongfiguration;
        public FD_ReceivablesDeleteHandler(FD_PaymentReceivablesTODataProvider prodiver,HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,Ifd_bankreceivableRepository BankreceivableRepository,IBiz_RelatedDetailRepository biz_RelatedDetailRepository,Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _settleReceiptRepository = settleReceiptRepository;
            _settleReceiptDetailRepository = settleReceiptDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _expenseRepository = expenseRepository;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _ibsfileRepository = ibsfileRepository;
            _ScheduleplanRepository = scheduleplanRepository;
            _BankreceivableRepository = BankreceivableRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _prodiver = prodiver;
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
                    var details = _detailRepository.GetDetails(num);
                    var extend = _paymentExtendRepository.GetDetails(num);
                    //判断是否是凭证处理生成
                    var Validation = _biz_RelatedRepository.GetRelated(new BIZ_Related() { RelatedType = "201610210104402122", ParentType = "2303061011170000150", ChildType = "1611091727140000101", ParentValue = num }).Result;
                    if (Validation.Count > 0)
                    {
                        var settle = _settleReceiptRepository.Get(Validation.FirstOrDefault().ChildValue);
                        return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), data = new { settle, request }, msg = $"已生成会计凭证{settle.Number}号，请先删除会计凭证" };
                    }

                    await _paymentreceivablesvoucherDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();

                    await _paymentreceivablesvoucherRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();

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
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402122" && o.ParentValue == num);
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
    public class FD_ReceivablesModifyHandler : IRequestHandler<FD_ReceivablesModifyCommand, Result>
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
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;
        IbsfileRepository _ibsfileRepository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        Ifd_bankreceivableRepository _BankreceivableRepository;
        FMBaseCommon _baseUnit;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherDetailRepository;
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;
        FD_SettleReceiptNewODataProvider _provider;

        public FD_ReceivablesModifyHandler(FD_SettleReceiptNewODataProvider provider, HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherDetailRepository,FMBaseCommon baseUnit, Ifd_bankreceivableRepository BankreceivableRepository,IBiz_RelatedDetailRepository biz_RelatedDetailRepository,Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
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
            _ScheduleplanRepository = scheduleplanRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _BankreceivableRepository = BankreceivableRepository;
            _baseUnit = baseUnit;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherDetailRepository = paymentreceivablesvoucherDetailRepository;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _provider = provider;
        }

        public async Task<Result> Handle(FD_ReceivablesModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //启用，则收付款单（不含收付款汇总单）自动生成会计凭证；
                //未启用，则收付款单需要到凭证处理形成一条待生成凭证的记录，且收付款单借贷方科目不展示
                var option = _baseUnit.OptionConfigValue("201612270104402002", _identityService.GroupId);
                //开启状态
                bool optionStatus = false;
                //开启系统选项
                if (option != "0" && !string.IsNullOrEmpty(option))
                {
                    optionStatus = true;
                }
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
                var oldSettleDomain = _paymentreceivablesvoucherRepository.Get(request.NumericalOrder);
                var accoList = await _baseUnit.GetSubjectListExtend(0, Convert.ToInt64(string.IsNullOrEmpty(request.EnterpriseID) ? _identityService.EnterpriseId : request.EnterpriseID), request.DataDate.ToString("yyyy-MM-dd"));
                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList();
                }

                //判断是否是凭证处理生成
                var Validation = _biz_RelatedRepository.GetRelated(new BIZ_Related() { RelatedType = "201610210104402122", ParentType = "2303061011170000150", ChildType = "1611091727140000101", ParentValue = request.NumericalOrder }).Result;
                if (Validation.Count > 0)
                {
                    var settle = _settleReceiptRepository.Get(Validation.FirstOrDefault().ChildValue);
                    return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), data = new { settle, request }, msg = $"已生成会计凭证{settle.Number}号，请先删除会计凭证" };
                }


                string number = oldDomain.Number;
                string settleNumber = oldSettleDomain.Number;
                var numericalOrder = request.NumericalOrder;

                //先删后增
                foreach (var num in list)
                {

                    await _paymentreceivablesvoucherDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();

                    await _paymentreceivablesvoucherRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();


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
                    var temp = _BankreceivableRepository.GetDataBySourceNum(num);
                    if (temp != null)
                    {
                        temp.SourceNum = numericalOrder;
                        _BankreceivableRepository.Update(temp);
                    }
                }
                var domain = new Domain.FD_PaymentReceivables()
                {
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
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
                int i = 0;
                request.details?.ForEach(o =>
                {
                    o.Guid = Guid.NewGuid();
                    request.extend[i].Guid = o.Guid;
                    domain.details.Add(new FD_PaymentReceivablesDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = o.AccountID,
                        EnterpriseID = o.EnterpriseID,
                        Amount = o.Amount,
                        BusinessType = o.BusinessType,
                        Charges = o.Charges,
                        Content = o.Content,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        AttachCount = o.AttachCount,
                        PaymentTypeID = o.PaymentTypeID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        ProductID = o.ReceiptAbstractID,
                        ProjectID = o.ProjectID,
                        OrganizationSortID = o.OrganizationSortID,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                    });
                });
                request.extend?.ForEach(o =>
                {
                    domain.extend.Add(new Domain.FD_PaymentExtend()
                    {
                        AccountName = o.AccountName,
                        Amount = o.Amount,
                        BankAccount = o.BankAccount,
                        BankDeposit = o.BankDeposit,
                        CollectionId = string.IsNullOrEmpty(o.CollectionId) ? "0" : o.CollectionId,
                        NumericalOrder = numericalOrder,
                        PersonId = string.IsNullOrEmpty(o.PersonId) ? "0" : o.PersonId,
                        Guid = o.Guid
                    });
                });
                var voucherDomain = new Domain.fd_paymentreceivablesvoucher()
                {
                    NumericalOrder = numericalOrder,
                    Number = settleNumber.ToString(),
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                    AttachmentNum = request.AttachmentNum,
                    Remarks = request.Remarks,
                    EnterpriseID = request.EnterpriseID,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SettleReceipType = "201610220104402201",
                    details = new List<fd_paymentreceivablesvoucherdetail>()
                };
                var tempAcc = accoList.Where(m => m.AccoSubjectID == request.DebitAccoSubjectID).FirstOrDefault();
                if (tempAcc == null)
                {
                    //如果为空 则默认资金科目
                    tempAcc = new BIZ_AccoSubject() { bTorF = true };
                }
                
                //表体传过来的收款单信息都是贷方信息 汇总生成一行借方明细
                request.details?.ForEach(o =>
                {
                    var tempAcc = accoList.Where(m => m.AccoSubjectID == o.AccoSubjectID).FirstOrDefault();
                    if (tempAcc == null)
                    {
                        //如果为空 则默认资金科目
                        tempAcc = new BIZ_AccoSubject() { bTorF = true };
                    }
                    voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = tempAcc.bTorF ? string.IsNullOrEmpty(o.AccountID) ? "0" : o.AccountID : "0",
                        EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                        PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                        MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        Content = o.Content,
                        PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                        Debit = o.Debit,
                        Credit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                        LorR = false,
                        RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                    });
                    //手续费另生成一条金额
                    if (o.Charges > 0)
                    {
                        voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                        {
                            NumericalOrder = numericalOrder,
                            Guid = o.Guid,
                            AccountID = o.AccountID,
                            EnterpriseID = o.EnterpriseID,
                            ReceiptAbstractID = o.ReceiptAbstractID,
                            AccoSubjectID = o.CostAccoSubjectID,
                            AccoSubjectCode = o.AccoSubjectCode,
                            CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                            PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                            MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                            ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                            ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                            Content = o.Content,
                            PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                            Credit = o.Debit,
                            Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Charges),
                            LorR = false,
                            RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                            OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        });
                    }
                });
                voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    AccountID = tempAcc.bTorF ? request.details[0].AccountID : "0",
                    EnterpriseID = request.EnterpriseID,
                    ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                    AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                    AccoSubjectCode = request.details[0].AccoSubjectCode,
                    CustomerID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID : "0",
                    PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].PersonID) ? "0" : request.details[0].SettlePersonID : "0",
                    MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].MarketID) ? "0" : request.details[0].SettleMarketID : "0",
                    ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                    ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                    Content = request.details[0].Content,
                    PaymentTypeID = tempAcc.bTorF ? request.details[0].PaymentTypeID : "0",
                    Credit = Convert.ToDecimal(request.details.Sum(m => m.Amount)) + Convert.ToDecimal(request.details.Sum(m => m.Charges)),
                    Debit = 0,
                    LorR = true,
                    RowNum = voucherDomain.details.Count == 0 ? 1 : voucherDomain.details.Max(m => m.RowNum) + 1,
                    OrganizationSortID = string.IsNullOrEmpty(request.details[0].OrganizationSortID) ? "0" : request.details[0].OrganizationSortID,
                    Auxiliary1 = string.IsNullOrEmpty(request.Auxiliary1) ? "0" : request.Auxiliary1,
                    Auxiliary2 = string.IsNullOrEmpty(request.Auxiliary2) ? "0" : request.Auxiliary2,
                    Auxiliary3 = string.IsNullOrEmpty(request.Auxiliary3) ? "0" : request.Auxiliary3,
                    Auxiliary4 = string.IsNullOrEmpty(request.Auxiliary4) ? "0" : request.Auxiliary4,
                    Auxiliary5 = string.IsNullOrEmpty(request.Auxiliary5) ? "0" : request.Auxiliary5,
                    Auxiliary6 = string.IsNullOrEmpty(request.Auxiliary6) ? "0" : request.Auxiliary6,
                    Auxiliary7 = string.IsNullOrEmpty(request.Auxiliary7) ? "0" : request.Auxiliary7,
                    Auxiliary8 = string.IsNullOrEmpty(request.Auxiliary8) ? "0" : request.Auxiliary8,
                    Auxiliary9 = string.IsNullOrEmpty(request.Auxiliary9) ? "0" : request.Auxiliary9,
                    Auxiliary10 = string.IsNullOrEmpty(request.Auxiliary10) ? "0" : request.Auxiliary10,
                    //OrganizationSortID = o.OrganizationSortID,
                    //Remarks = o.Remarks,
                });
                if (up != null)
                {
                    foreach (var item in up)
                    {
                        await _ibsfileRepository.AddAsync(new bsfile()
                        {
                            Guid = Guid.NewGuid(),
                            EnterId = _identityService.EnterpriseId,
                            NumericalOrder = numericalOrder,
                            Type = 2,
                            FileName = item.FileName,
                            FilePath = item.PathUrl,
                            OwnerID = _identityService.UserId,
                            //Remarks = request.UploadInfo
                        });
                    }
                }
                //余额结算  金融充值接口，保存付款单 判断是否 收款方式=余额结算，是 就走金融充值接口
                //https://confluence.nxin.com/pages/viewpage.action?pageId=65055307
                var amountData = domain.details.Where(m => m.PaymentTypeID == "1912201259040000102");
                if (amountData.Count() > 0)
                {
                    domain.extend.FirstOrDefault().TradeNo = _numericalOrderCreator.CreateAsync().Result;
                }
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _paymentExtendRepository.AddRangeAsync(domain.extend);
                await _paymentreceivablesvoucherRepository.AddAsync(voucherDomain);
                await _paymentreceivablesvoucherDetailRepository.AddRangeAsync(voucherDomain.details);
                

                if (up != null)
                {
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                //收款单
                Biz_Review review = new Biz_Review(numericalOrder, request.AppId, _identityService.UserId).SetMaking();

                if (optionStatus)
                {
                    Domain.FD_SettleReceipt settleDomain = CreateSettleReceipt(request, optionStatus, accoList, settleNumber, numericalOrder, tempAcc);
                    //凭证
                    Biz_Review review2 = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                    await _settleReceiptRepository.AddAsync(settleDomain);
                    await _settleReceiptDetailRepository.AddRangeAsync(settleDomain.details);
                    await _biz_ReviewRepository.AddAsync(review2);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();

                await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherDetailRepository.UnitOfWork.SaveChangesAsync();

                await _BankreceivableRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                //await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                if (!string.IsNullOrEmpty(request.ApplyAppId) && request.ApplyAppId != "0")
                {
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402122" && o.ParentType == request.AppId && o.ChildType == request.ApplyAppId && o.ParentValue == request.NumericalOrder);
                    await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(request.NumericalOrder));
                    List<biz_relateddetail> relateddetail = new();
                    foreach (var ritem in request.RelatedList)
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//收款或收款汇总appid
                            ParentValue = numericalOrder,//收款单/收款汇总 流水号
                            ChildType = request.ApplyAppId,//申请单appid
                            ChildValue = ritem.ApplyNumericalOrder,//申请流水号
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
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "销售单-集中收款单关联详情"
                        });
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
                error.columns[0].name = "参数" + JsonConvert.SerializeObject(request);
                result.errors.Add(error);
                result.msg = _repository.ExceptionMessageHandle("保存异常", JsonConvert.SerializeObject(ex));
            }
            return result;
        }

        private Domain.FD_SettleReceipt CreateSettleReceipt(FD_ReceivablesModifyCommand request, bool optionStatus, List<BIZ_AccoSubject> accoList, string settleNumber, string numericalOrder, BIZ_AccoSubject tempAcc)
        {
            var settleDomain = new Domain.FD_SettleReceipt()
            {
                NumericalOrder = numericalOrder,
                Number = settleNumber.ToString(),
                Guid = Guid.NewGuid(),
                DataDate = request.DataDate,
                TicketedPointID = string.IsNullOrEmpty(request.TicketedPointID) ? "0" : request.TicketedPointID,
                AttachmentNum = request.AttachmentNum,
                Remarks = request.Remarks,
                EnterpriseID = request.EnterpriseID,
                OwnerID = _identityService.UserId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                SettleReceipType = "201610220104402201",
                details = new List<FD_SettleReceiptDetail>()
            };
            
            //表体传过来的收款单信息都是贷方信息 汇总生成一行借方明细
            request.details?.ForEach(o =>
            {
                var tempAcc = accoList.Where(m => m.AccoSubjectID == o.AccoSubjectID).FirstOrDefault();
                if (tempAcc == null)
                {
                    //如果为空 则默认资金科目
                    tempAcc = new BIZ_AccoSubject() { bTorF = true };
                }
                settleDomain.details.Add(new FD_SettleReceiptDetail()
                {
                    NumericalOrder = numericalOrder,
                    Guid = o.Guid,
                    AccountID = tempAcc.bTorF ? string.IsNullOrEmpty(o.AccountID) ? "0" : o.AccountID : "0",
                    EnterpriseID = string.IsNullOrEmpty(o.EnterpriseID) ? "0" : o.EnterpriseID,
                    ReceiptAbstractID = o.ReceiptAbstractID,
                    AccoSubjectID = o.AccoSubjectID,
                    AccoSubjectCode = o.AccoSubjectCode,
                    CustomerID = !tempAcc.bTorF ? o.CustomerID : "0",
                    PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                    MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                    ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                    ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                    Content = o.Content,
                    PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                    Credit = o.Debit,
                    Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                    LorR = false,
                    RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                    OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                });
                //手续费另生成一条金额
                if (o.Charges > 0)
                {
                    settleDomain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = o.Guid,
                        AccountID = o.AccountID,
                        EnterpriseID = o.EnterpriseID,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.CostAccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = !tempAcc.bTorF ?  o.AccountID : "0",
                        PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID : "0",
                        MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(o.MarketID) ? "0" :o.MarketID : "0",
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        Content = o.Content,
                        PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                        Credit = o.Debit,
                        Debit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Charges),
                        LorR = false,
                        RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                    });
                }
            });

            settleDomain.details.Add(new FD_SettleReceiptDetail()
            {
                NumericalOrder = numericalOrder,
                Guid = Guid.NewGuid(),
                AccountID = tempAcc.bTorF ? request.details[0].AccountID : "0",
                EnterpriseID = request.EnterpriseID,
                ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                AccoSubjectCode = request.details[0].AccoSubjectCode,
                CustomerID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID : "0",
                PersonID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].PersonID) ? "0" : request.details[0].SettlePersonID : "0",
                MarketID = !tempAcc.bTorF ? string.IsNullOrEmpty(request.details[0].MarketID) ? "0" : request.details[0].SettleMarketID : "0",
                ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                Content = request.details[0].Content,
                PaymentTypeID = tempAcc.bTorF ? request.details[0].PaymentTypeID : "0",
                Credit = Convert.ToDecimal(request.details.Sum(m => m.Amount)) + Convert.ToDecimal(request.details.Sum(m => m.Charges)),
                Debit = 0,
                LorR = true,
                RowNum = settleDomain.details.Count == 0 ? 1 : settleDomain.details.Max(m => m.RowNum) + 1,
                OrganizationSortID = string.IsNullOrEmpty(request.details[0].OrganizationSortID) ? "0" : request.details[0].OrganizationSortID,
                Auxiliary1 = string.IsNullOrEmpty(request.Auxiliary1) ? "0" : request.Auxiliary1,
                Auxiliary2 = string.IsNullOrEmpty(request.Auxiliary2) ? "0" : request.Auxiliary2,
                Auxiliary3 = string.IsNullOrEmpty(request.Auxiliary3) ? "0" : request.Auxiliary3,
                Auxiliary4 = string.IsNullOrEmpty(request.Auxiliary4) ? "0" : request.Auxiliary4,
                Auxiliary5 = string.IsNullOrEmpty(request.Auxiliary5) ? "0" : request.Auxiliary5,
                Auxiliary6 = string.IsNullOrEmpty(request.Auxiliary6) ? "0" : request.Auxiliary6,
                Auxiliary7 = string.IsNullOrEmpty(request.Auxiliary7) ? "0" : request.Auxiliary7,
                Auxiliary8 = string.IsNullOrEmpty(request.Auxiliary8) ? "0" : request.Auxiliary8,
                Auxiliary9 = string.IsNullOrEmpty(request.Auxiliary9) ? "0" : request.Auxiliary9,
                Auxiliary10 = string.IsNullOrEmpty(request.Auxiliary10) ? "0" : request.Auxiliary10,
                //OrganizationSortID = o.OrganizationSortID,
                //Remarks = o.Remarks,
            });
            return settleDomain;
        }
    }

    public class FD_ReceivablesUpInfoHandler : IRequestHandler<FD_ReceivablesUpInfoCommand, Result>
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
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;
        IbsfileRepository _ibsfileRepository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        public FD_ReceivablesUpInfoHandler(Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_scheduleplanRepository scheduleplanRepository, IFD_AccountTransferDetailRepository accountTransferDetailRepository, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_ReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_ReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _AccountTransferDetailRepository = accountTransferDetailRepository;
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
            _ScheduleplanRepository = scheduleplanRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
        }

        public async Task<Result> Handle(FD_ReceivablesUpInfoCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //request.SettleReceipType = "201611180104402202";
                var oldDomain = _repository.Get(request.NumericalOrder);
                var oldSettleDomain = _paymentreceivablesvoucherRepository.Get(request.NumericalOrder);
                List<UploadInfo> up = null;
                if (!string.IsNullOrEmpty(request.UploadInfo))
                {
                    up = JsonConvert.DeserializeObject<List<UploadInfo>>(request.UploadInfo);
                }
                if (up != null)
                {
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
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
                            //Remarks = request.UploadInfo
                        });
                    }
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
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
                var error = new ErrorRow();
                error.columns = new List<ErrorColumn>();
                error.columns.Add(new ErrorColumn());
                error.columns[0].value = ex;
                error.columns[0].name = "参数" + JsonConvert.SerializeObject(request);
                result.errors.Add(error);
                result.msg = _repository.ExceptionMessageHandle("上传附件异常", JsonConvert.SerializeObject(ex));
            }
            return result;
        }
    }
}
