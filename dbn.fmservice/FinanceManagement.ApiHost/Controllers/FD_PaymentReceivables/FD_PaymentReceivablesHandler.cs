using Architecture.Common.HttpClientUtil;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Util;
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
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables
{
    public class FD_PaymentReceivablesAddHandler : IRequestHandler<FD_PaymentReceivablesAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_PaymentReceivablesRepository _repository;
        IFD_PaymentReceivablesDetailRepository _detailRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherdetailRepository;
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
        FMBaseCommon _baseUnit;
        FD_SettleReceiptNewODataProvider _provider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        FD_PaymentReceivablesTODataProvider _paymentReceivablesTODataProvider;
        private QlwCrossDbContext _context;

        public FD_PaymentReceivablesAddHandler(QlwCrossDbContext context, FD_PaymentReceivablesTODataProvider paymentReceivablesTODataProvider, HostConfiguration hostCongfiguration,HttpClientUtil httpClientUtil, IHttpContextAccessor httpContextAccessor, FD_SettleReceiptNewODataProvider provider, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository,Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherdetailRepository, IBiz_RelatedDetailRepository biz_RelatedDetailRepository,FMBaseCommon baseUnit, Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_PaymentReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
            _baseUnit = baseUnit;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherdetailRepository = paymentreceivablesvoucherdetailRepository;
            _provider = provider;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
            _paymentReceivablesTODataProvider = paymentReceivablesTODataProvider;
            _context = context;
        }

        public async Task<Result> Handle(FD_PaymentReceivablesAddCommand request, CancellationToken cancellationToken)
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
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                #region 结账控制,序时控制
                var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(request.EnterpriseID), OwnerID = Convert.ToInt64(request.OwnerID), DataDate = Convert.ToDateTime(request.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{request.EnterpriseID}/{request.OwnerID}/FM_AccoCheck/IsLockForm";
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                if ((bool)res?.ResultState)
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
                //默认 付款凭证
                long settleNumber = Convert.ToInt64(_provider.GetMaxNumberByDate("201610220104402202", request.EnterpriseID, request.BeginDate, request.EndDate, request.TicketedPointID).MaxNumber);
                var numericalOrder = _numericalOrderCreator.CreateAsync().Result;
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
                        BankDeposit = o.BankDeposit.Trim(),
                        CollectionId = string.IsNullOrEmpty(o.CollectionId) ? "0" : o.CollectionId,
                        NumericalOrder = numericalOrder,
                        PersonId = string.IsNullOrEmpty(o.PersonId) ? "0" : o.PersonId,
                        Guid = o.Guid
                    });
                });
                //2022-12-07 增加调拨逻辑 如果是账户资金调拨 生成的会计凭证 借方资金账户 为 账户资金调拨 中的 AccountID，汇总单不需要
                FD_AccountTransferDetail tempTransfer = new();
                if (!string.IsNullOrEmpty(request.ApplyNumericalOrder) && request.ApplyAppId != "201611180104402201" && request.ApplyAppId != "201611180104402203" && request.ApplyAppId != "201612070104402204" && string.IsNullOrEmpty(request.ApplyScAppId))
                {
                    tempTransfer = _AccountTransferDetailRepository.GetSingleDataIsIn(request.ApplyNumericalOrder);
                    if (tempTransfer == null)
                    {
                        tempTransfer = new FD_AccountTransferDetail();
                    }
                }
                var tempAcc = accoList.Where(m => m.AccoSubjectID == request.DebitAccoSubjectID).FirstOrDefault();
                if (tempAcc == null)
                {
                    //如果为空 则默认资金科目
                    tempAcc = new BIZ_AccoSubject() { bTorF = true };
                }
                #region 收付款扩展表（凭证）
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
                    SettleReceipType = request.SettleReceipType,
                    details = new List<fd_paymentreceivablesvoucherdetail>()
                };
                ////根据系统选项自取 科目
                //if (optionStatus)
                //{
                //    _h
                //}
                //AuthenticationHeaderValue authentication;
                //bool verification = AuthenticationHeaderValue.TryParse(_httpContextAccessor.HttpContext.Request.GetAuthToken(), out authentication);
                voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    AccountID = tempAcc.bTorF ? (tempTransfer.AccountID != null ? tempTransfer.AccountID : request.details[0].AccountID) : "0",
                    EnterpriseID = request.EnterpriseID,
                    ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                    AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                    AccoSubjectCode = request.details[0].AccoSubjectCode,
                    CustomerID = string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID,
                    PersonID = string.IsNullOrEmpty(request.details[0].SettlePersonID) ? "0" : request.details[0].SettlePersonID,
                    MarketID = string.IsNullOrEmpty(request.details[0].SettleMarketID) ? "0" : request.details[0].SettleMarketID,
                    ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                    ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                    Content = request.details[0].Content,
                    PaymentTypeID = tempAcc.bTorF ? (request.details[0].PaymentTypeID) : "0",
                    Credit = 0,
                    Debit = Convert.ToDecimal(request.details.Sum(m => m.Amount)),
                    LorR = false,
                    RowNum = 1,
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

                //表体传过来的付款单信息都是贷方信息 汇总生成一行借方明细
                request.details?.ForEach(o =>
                {
                    //增加 借方会计科目非资金类科目时，不需要显示结算方式
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
                        AccountID = tempAcc.bTorF ? o.AccountID : "0",
                        EnterpriseID = o.EnterpriseID ?? "0",
                        ReceiptAbstractID = o.ReceiptAbstractID ?? "0",
                        AccoSubjectID = o.AccoSubjectID ?? "0",
                        AccoSubjectCode = o.AccoSubjectCode ?? "0",
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        Content = o.Content,
                        PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                        Credit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                        Debit = o.Debit,
                        LorR = true,
                        RowNum = voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                    });
                });
                #endregion


                if (!string.IsNullOrEmpty(request.ApplyNumericalOrder) && (string.IsNullOrEmpty(request.PlanNumericalOrder) || request.PlanNumericalOrder == "0"))
                {
                    var relatedList = _context.DynamicSqlQuery($"SELECT * from nxin_qlw_business.biz_related where ChildValue = {request.ApplyNumericalOrder} AND ParentType = 1612011058280000101");
                    if (relatedList.Count > 0)
                    {
                        result.code = ErrorCode.ServerBusy.GetIntValue();
                        result.msg = "当前申请已被引用，保存失败";
                        return result;
                    }
                }
                //退款业务
                if (request.RelatedList != null && request.ApplyAppId == "201611180104402201")
                {
                    foreach (var item in request.RelatedList)
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//付款或付款汇总appid
                            ParentValue = numericalOrder,//付款单/付款汇总 流水号
                            ChildType = item.SettleReceipType,//收款单类型
                            ChildValue = item.ApplyNumericalOrder,//收款流水号
                            ChildValueDetail = item.RecordID,
                            Remarks = "付款单关联收款单（收款退回业务）"
                            //ParentValueDetail = numericalOrder
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                        var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                        var rData = _biz_RelatedRepository.GetList(related).FirstOrDefault();
                        _biz_RelatedDetailRepository.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(_identityService.UserId),
                            Paid = item.Paid,//实收金额
                            Payment = item.Payment,//累计退款金额
                            Payable = item.Payable,//本次退款金额
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "付款单关联收款单（收款退回业务明细）"
                        });
                    }
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
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
                        });
                    }
                }
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _paymentExtendRepository.AddRangeAsync(domain.extend);
                if (optionStatus)
                {
                    Domain.FD_SettleReceipt settleDomain = CreateSettleReceipt(request, optionStatus, settleNumber, numericalOrder, accoList, tempTransfer, tempAcc);
                    await _settleReceiptRepository.AddAsync(settleDomain);
                    await _settleReceiptDetailRepository.AddRangeAsync(settleDomain.details);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
                    Biz_Review review2 = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                    await _biz_ReviewRepository.AddAsync(review2);
                }
                await _paymentreceivablesvoucherRepository.AddAsync(voucherDomain);
                await _paymentreceivablesvoucherdetailRepository.AddRangeAsync(voucherDomain.details);
                //增加排程流程  获取当前申请 是否已排程
                var plan = _ScheduleplanRepository.GetList(_identityService.GroupId).Where(m => m.ApplyNumericalOrder == request.ApplyNumericalOrder && (m.ScheduleStatus == 2 || m.ScheduleStatus == 3) && m.NumericalOrder == request.PlanNumericalOrder).ToList();

                if (plan != null && plan.Count > 0)
                {
                    foreach (var item in plan)
                    {
                        item.ScheduleStatus = 3;
                        item.PayNumericalOrder = domain.NumericalOrder;
                        _ScheduleplanRepository.Update(item);
                    }
                    await _ScheduleplanRepository.UnitOfWork.SaveChangesAsync();
                    var sumPlan = _ScheduleplanRepository.GetList(_identityService.GroupId).Where(m => m.ApplyNumericalOrder == request.ApplyNumericalOrder && (m.ScheduleStatus == 2 || m.ScheduleStatus == 3)).ToList();
                    if (sumPlan.Sum(m => m.PayAmount) == sumPlan.FirstOrDefault().ApplyAmount)
                    {
                        await SetExpens(request, numericalOrder);
                    }
                    //增加关系 当排程数据完全付完后，在修改申请数据
                    else
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//付款或付款汇总appid
                            ParentValue = numericalOrder,//付款单/付款汇总 流水号
                            ChildType = request.ApplyAppId,//申请单appid
                            ChildValue = request.ApplyNumericalOrder,//申请流水号
                            Remarks = "申请关联收付款单"
                            //ParentValueDetail = numericalOrder
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                    }
                }
                else
                {
                    await SetExpens(request, numericalOrder);
                }
                if (up != null)
                {
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                //付款单
                Biz_Review review = new Biz_Review(numericalOrder, request.AppId, _identityService.UserId).SetMaking();
                //凭证
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                if (!string.IsNullOrEmpty(request.ApplyAppId))
                {
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                }
                result.data = new { NumericalOrder = numericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                #region 商城关系处理
                if (request.ApplyScAppId == "201612070104402204" && string.IsNullOrEmpty(request.ApplyAppId))
                {
                    var query = _detailRepository.GetNums(numericalOrder);
                    for (int x = 0; x < request.details.Count; x++)
                    {
                        var details = request.details[x];
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//付款或付款汇总appid
                            ParentValue = numericalOrder,//付款单/付款汇总 流水号
                            ParentValueDetail = query.FirstOrDefault(m=>m.Guid == details.Guid).RecordID.ToString(),
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

        public Domain.FD_SettleReceipt CreateSettleReceipt(FD_PaymentReceivablesAddCommand request, bool optionStatus, long settleNumber, string numericalOrder, List<BIZ_AccoSubject> accoList, FD_AccountTransferDetail tempTransfer, BIZ_AccoSubject tempAcc)
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
                SettleReceipType =  "201610220104402202",
                details = new List<FD_SettleReceiptDetail>()
            };

            settleDomain.details.Add(new FD_SettleReceiptDetail()
            {
                NumericalOrder = numericalOrder,
                Guid = Guid.NewGuid(),
                AccountID = tempAcc.bTorF ? (tempTransfer.AccountID != null ? tempTransfer.AccountID : request.details[0].AccountID) : "0",
                EnterpriseID = request.EnterpriseID,
                ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                AccoSubjectCode = request.details[0].AccoSubjectCode,
                CustomerID = string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID,
                PersonID = string.IsNullOrEmpty(request.details[0].SettlePersonID) ? "0" : request.details[0].SettlePersonID,
                MarketID = string.IsNullOrEmpty(request.details[0].SettleMarketID) ? "0" : request.details[0].SettleMarketID,
                ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                Content = request.details[0].Content,
                PaymentTypeID = tempAcc.bTorF ? (request.details[0].PaymentTypeID) : "0",
                Credit = 0,
                Debit = Convert.ToDecimal(request.details.Sum(m => m.Amount)),
                LorR = false,
                RowNum = 1,
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
            //表体传过来的付款单信息都是贷方信息 汇总生成一行借方明细
            request.details?.ForEach(o =>
            {
                //增加 借方会计科目非资金类科目时，不需要显示结算方式
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
                    AccountID = tempAcc.bTorF ? o.AccountID : "0",
                    EnterpriseID = o.EnterpriseID,
                    ReceiptAbstractID = o.ReceiptAbstractID,
                    AccoSubjectID = o.AccoSubjectID,
                    AccoSubjectCode = o.AccoSubjectCode,
                    CustomerID = "0",
                    PersonID = "0",
                    MarketID = "0",
                    ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                    ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                    Content = o.Content,
                    PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                    Credit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                    Debit = o.Debit,
                    LorR = true,
                    RowNum = settleDomain.details.Max(m => m.RowNum) + 1,
                    OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                });
            });
            return settleDomain;
        }

        private async Task SetExpens(FD_PaymentReceivablesAddCommand request, string numericalOrder)
        {
            if (!string.IsNullOrEmpty(request.ApplyAppId) && request.ApplyAppId != "201611180104402201" && request.ApplyAppId != "201611180104402203" && request.ApplyAppId != "201612070104402204" && string.IsNullOrEmpty(request.ApplyScAppId))
            {
                var related = new BIZ_Related()
                {
                    RelatedType = "201610210104402122",
                    ParentType = request.AppId,//付款或付款汇总appid
                    ParentValue = numericalOrder,//付款单/付款汇总 流水号
                    ChildType = request.ApplyAppId,//申请单appid
                    ChildValue = request.ApplyNumericalOrder,//申请流水号
                    Remarks = "申请关联收付款单"
                    //ParentValueDetail = numericalOrder
                };
                await _biz_RelatedRepository.AddAsync(related);
                //修改申请单的付款日期和付款人
                var exdomain = _expenseRepository.Get(request.ApplyNumericalOrder);
                if (exdomain != null)
                {
                    exdomain.UpdateApply(DateTime.Now, _identityService.UserId);
                    _expenseRepository.Update(exdomain);
                    await _expenseRepository.UnitOfWork.SaveChangesAsync();
                }
                else
                {
                    var item = request.details.FirstOrDefault();
                    if (item != null)
                    {
                        var transfer = _AccountTransferDetailRepository.GetSingleData(request.ApplyNumericalOrder);
                        if (transfer != null)
                        {
                            transfer.AccountID = item.AccountID;
                            transfer.Amount = (decimal)item.Amount;
                            transfer.DataDateTime = DateTime.Now;
                            transfer.PaymentTypeID = item.PaymentTypeID;
                            _AccountTransferDetailRepository.Update(transfer);
                            await _AccountTransferDetailRepository.UnitOfWork.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }

    public class FD_PaymentReceivablesDeleteHandler : IRequestHandler<FD_PaymentReceivablesDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_PaymentReceivablesRepository _repository;
        IFD_PaymentReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherdetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IbsfileRepository _ibsfileRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil1;
        public FD_PaymentReceivablesDeleteHandler(HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil1, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository, Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherdetailRepository, IBiz_RelatedDetailRepository biz_RelatedDetailRepository, Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_PaymentReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
            _paymentExtendRepository = paymentExtendRepository;
            _ibsfileRepository = ibsfileRepository;
            _ScheduleplanRepository = scheduleplanRepository;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherdetailRepository = paymentreceivablesvoucherdetailRepository;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil1;
        }

        public async Task<Result> Handle(FD_PaymentReceivablesDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList();
                }

                //RelatedType = "201610210104402122",
                //ParentType = "2303061011170000150",//凭证处理
                //ParentValue = voucher.NumericalOrder,//凭证处理 流水号
                //ChildType = "1611091727140000101",//会计凭证appid
                //ChildValue = settleReceipt.NumericalOrder,//会计凭证 流水号
                foreach (var num in list)
                {
                    var oldDomain = _repository.Get(request.NumericalOrder);
                    #region 结账控制,序时控制
                    var param = new VerificationCheckoutModel { EnterpriseID = Convert.ToInt64(oldDomain.EnterpriseID), OwnerID = Convert.ToInt64(oldDomain.OwnerID), DataDate = Convert.ToDateTime(oldDomain.DataDate).ToString("yyyy-MM-dd"), AppID = 2108021645470000109 };
                    string requrl = $"{this._hostCongfiguration.QlwServiceHost}/api/{oldDomain.EnterpriseID}/{_identityService.UserId}/FM_AccoCheck/IsLockForm";
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
                    //判断是否是凭证处理生成
                    var Validation = _biz_RelatedRepository.GetRelated(new BIZ_Related() { RelatedType = "201610210104402122", ParentType = "2303061011170000150", ChildType = "1611091727140000101", ParentValue = num }).Result;
                    if (Validation.Count > 0)
                    {
                        var settle = _settleReceiptRepository.Get(Validation.FirstOrDefault().ChildValue);
                        return new Result() { code = ErrorCode.ServerBusy.GetIntValue(),data = new {settle,request }, msg = $"已生成会计凭证{settle.Number}号，请先删除会计凭证" };
                    }

                    await _paymentreceivablesvoucherRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();

                    await _paymentreceivablesvoucherdetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherdetailRepository.UnitOfWork.SaveChangesAsync();

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
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.ParentValue == request.NumericalOrder);
                    await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(request.NumericalOrder));
                    //增加排程流程  获取当前申请 是否存在 已排程 并且付款了
                    var plan = _ScheduleplanRepository.GetList(_identityService.GroupId).Where(m => m.ScheduleStatus == 3 && m.PayNumericalOrder == request.NumericalOrder && m.ApplyNumericalOrder == request.ApplyNumericalOrder).ToList();
                    if (plan != null && plan.Count > 0)
                    {
                        foreach (var item in plan)
                        {
                            //变回 已排程 未付款状态
                            item.ScheduleStatus = 2;
                            item.PayNumericalOrder = "0";
                            _ScheduleplanRepository.Update(item);
                        }
                        await _ScheduleplanRepository.UnitOfWork.SaveChangesAsync();
                    }
                    if (!string.IsNullOrEmpty(request.ApplyAppId) && request.ApplyAppId != "201612070104402204")
                    {
                        //修改申请单的付款日期和付款人
                        var exdomain = _expenseRepository.Get(request.ApplyNumericalOrder);
                        if (exdomain != null)
                        {
                            exdomain.UpdateApply(null, "0");
                            _expenseRepository.Update(exdomain);
                            await _expenseRepository.UnitOfWork.SaveChangesAsync();
                        }
                    }
                    else if(request.ApplyAppId != "201612070104402204")
                    {
                        var item = request.details.FirstOrDefault();
                        if (item != null)
                        {
                            var transfer = _AccountTransferDetailRepository.GetSingleData(request.ApplyNumericalOrder);
                            if (transfer != null)
                            {
                                transfer.AccountID = "0";
                                transfer.Amount = 0.00M;
                                transfer.DataDateTime = DateTime.MinValue;
                                transfer.PaymentTypeID = "0";
                                _AccountTransferDetailRepository.Update(transfer);
                                await _AccountTransferDetailRepository.UnitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                }
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
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
    public class FD_PaymentReceivablesCancelHandler : IRequestHandler<FD_PaymentReceivablesCancelLogicCommand, Result>
    {
        IIdentityService _identityService;
        IFD_PaymentReceivablesRepository _repository;
        IFD_PaymentReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IbsfileRepository _ibsfileRepository;

        public FD_PaymentReceivablesCancelHandler(IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_PaymentReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
        }

        public async Task<Result> Handle(FD_PaymentReceivablesCancelLogicCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                //NumericalOrder = recordid
                var eData = _paymentExtendRepository.GetAsyncByRecordId(request.NumericalOrder).Result;
                if (eData == null)
                {
                    return new Result() { code = ErrorCode.NoContent.GetIntValue(),msg = "无数据" };
                }
                eData.IsRecheck = request.IsRecheck;
                eData.RecheckId = request.RecheckId;
                try
                {
                    _paymentExtendRepository.SaveChange(eData);
                    return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "修改复核状态成功" };
                }
                catch (Exception e)
                {
                    LogHelper.LogWarning("发起支付出现异常：" + e.ToString());
                }
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
    /// <summary>
    /// 复核 驳回逻辑
    /// </summary>
    public class FD_PaymentReceivablesReviewLogicHandler : IRequestHandler<FD_PaymentReceivablesReviewLogicCommand, Result>
    {
        IIdentityService _identityService;
        IFD_PaymentReceivablesRepository _repository;
        IFD_PaymentReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_Related_FM _biz_Related_FMRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IbsfileRepository _ibsfileRepository;
        ILogger<FD_PaymentReceivablesReviewLogicHandler> _logger;

        public FD_PaymentReceivablesReviewLogicHandler(IBiz_Related_FM biz_Related_FMRepository, ILogger<FD_PaymentReceivablesReviewLogicHandler> logger, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_PaymentReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
            _biz_Related_FMRepository = biz_Related_FMRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _ibsfileRepository = ibsfileRepository;
            _logger = logger;
        }
        /// <summary>
        /// `RelatedType` = 2205231634370000109   /  2205231634370000999 = 发起新的阶段流程
        /// `ParentType` = 总级次
        /// `ChildType` = 级次
        /// `ParentValue` = 付款流水
        /// `ChildValue` = 付款明细标识 extend RecordId
        /// `ParentValueDetail` = 复核人
        /// `ChildValueDetail` = 复核状态 复核/驳回   1/2
        /// `Remarks` = 记录操作时间 2023-04-11 15:02:34
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(FD_PaymentReceivablesReviewLogicCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var eData = _paymentExtendRepository.GetAsyncByRecordId(request.RecordId).Result;
                var relate = _biz_Related_FMRepository.GetBIZ_Related(new BIZ_Related_FM()
                {
                    RelatedType = "2205231634370000109",
                    ParentType = request.Level.ToString(),
                    ChildType = request.RawLevel.ToString(),
                    ChildValue = request.RecordId,
                });
                var relateByUser = relate.Where(m => m.ParentValueDetail == _identityService.UserId).ToList();
                //驳回逻辑暂时没有用到 2023-04-14 09:33:53
                //驳回逻辑
                if (request.Status == 2)
                {
                    eData.IsRecheck = false;
                    eData.RecheckId = "0";
                    
                    foreach (var item in relateByUser)
                    {
                        item.ParentValueDetail = _identityService.UserId;
                        item.ChildValueDetail = "2";
                        item.Remarks = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        _biz_Related_FMRepository.Update(item);
                    }
                    if (relate.Count > 0)
                    {
                        _paymentExtendRepository.SaveChange(eData);
                        await _biz_Related_FMRepository.UnitOfWork.SaveChangesAsync();
                        return new Result() {code = ErrorCode.Success.GetIntValue(),msg = "驳回成功" };
                    }
                    else
                    {
                        return new Result() { code = ErrorCode.NoContent.GetIntValue(),msg = "当前登录人无复核权限" };
                    }
                }
                //复核 逻辑  
                else if(request.Status == 1)
                {
                    foreach (var item in relateByUser)
                    {
                        item.ParentValueDetail = _identityService.UserId;
                        item.ChildValueDetail = "1";
                        item.Remarks =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        _biz_Related_FMRepository.Update(item);
                    }
                    if (relateByUser.Count > 0)
                    {
                        await _biz_Related_FMRepository.UnitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Result() { code = ErrorCode.NoContent.GetIntValue(), msg = "当前登录人无复核权限" };
                    }
                    //是否最后一级
                    if (request.Level == request.RawLevel)
                    {
                        var finalyData = _biz_Related_FMRepository.GetBIZ_Related(new BIZ_Related_FM()
                        {
                            RelatedType = "2205231634370000109",
                            ParentType = request.Level.ToString(),
                            ChildType = request.RawLevel.ToString(),
                            ChildValue = request.RecordId,
                        });
                        //最后一级 + 最后一人复核完成 发起支付
                        if (finalyData.Count == finalyData.Where(m=>m.ChildValueDetail == "1").Count())
                        {
                            eData.RecheckId = _identityService.UserId;
                            _paymentExtendRepository.SaveChange(eData);
                            return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "发起支付成功" };
                        }
                        else
                        {
                            return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "复核成功" };
                        }
                    }
                    else
                    {
                        return new Result() { code = ErrorCode.Success.GetIntValue(), msg = "复核成功" };
                    }
                }
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
    public class FD_PaymentReceivablesLogicHandler : IRequestHandler<FD_PaymentReceivablesLogicCommand, Result>
    {
        IIdentityService _identityService;
        IFD_PaymentReceivablesRepository _repository;
        IFD_PaymentReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IBiz_Related_FM _biz_Related_FMRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IbsfileRepository _ibsfileRepository;
        ILogger<FD_PaymentReceivablesLogicHandler> _logger;

        public FD_PaymentReceivablesLogicHandler(IBiz_Related_FM biz_Related_FMRepository, ILogger<FD_PaymentReceivablesLogicHandler> logger, IbsfileRepository ibsfileRepository, IIdentityService identityService, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_PaymentReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
            _biz_Related_FMRepository = biz_Related_FMRepository;
            _paymentExtendRepository = paymentExtendRepository;
            _ibsfileRepository = ibsfileRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(FD_PaymentReceivablesLogicCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                _logger.LogInformation("发起支付后UPDATE入参FD_PaymentReceivablesLogicCommand参数：" + JsonConvert.SerializeObject(request));
                //NumericalOrder = recordid
                var eData = _paymentExtendRepository.GetAsyncByRecordId(request.NumericalOrder).Result;
                _logger.LogInformation("eData数据："+JsonConvert.SerializeObject(eData));
                //s.RelatedType == model.RelatedType && s.ParentType == model.ParentType && s.ChildType == model.ChildType && s.ChildValue == model.ChildValue
                var relate = _biz_Related_FMRepository.GetPayReviewList(new BIZ_Related_FM()
                {
                    RelatedType = "2205231634370000109",
                    ParentValue = eData.NumericalOrder,
                    ChildValue = request.NumericalOrder,
                });
                //第一次提交逻辑
                //isPay = 已经发起支付 不对流程节点做任何处理
                if (!request.IsPay)
                {
                    if (request.IsRecheck)
                    {
                        eData.IsRecheck = true;
                        if (request.AuditList == null)
                        {
                            request.AuditList = new List<AuditInfomation>();
                        }
                        var audit = request.AuditList.FirstOrDefault();

                        //第一次提交 插入阶段流程数据 = 提交成功 进入复核状态
                        if (relate.Count == 0)
                        {
                            foreach (var item in request.AuditList)
                            {
                                _biz_Related_FMRepository.Add(new BIZ_Related_FM()
                                {
                                    RelatedType = "2205231634370000109",
                                    ParentType = request.AuditList.Max(m => m.RawLevel).ToString(),
                                    ChildType = item.RawLevel.ToString(),
                                    ParentValue = eData.NumericalOrder,
                                    ChildValue = request.NumericalOrder,
                                    ParentValueDetail = item.PersonID.ToString(),
                                    ChildValueDetail = "0",
                                });
                            }
                            _biz_Related_FMRepository.Add(new BIZ_Related_FM()
                            {
                                RelatedType = "2205231634370000109",
                                ParentType = "2205231634370000109",
                                ChildType = "0",
                                ParentValue = eData.NumericalOrder,
                                ChildValue = request.NumericalOrder,
                                ParentValueDetail = _identityService.UserId,
                                ChildValueDetail = "0",
                                Remarks = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            await _biz_Related_FMRepository.UnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            foreach (var item in relate)
                            {
                                item.RelatedType = "2205231634370000999";
                                _biz_Related_FMRepository.Update(item);
                            }
                            foreach (var item in request.AuditList)
                            {
                                _biz_Related_FMRepository.Add(new BIZ_Related_FM()
                                {
                                    RelatedType = "2205231634370000109",
                                    ParentType = request.AuditList.Max(m => m.RawLevel).ToString(),
                                    ChildType = item.RawLevel.ToString(),
                                    ParentValue = eData.NumericalOrder,
                                    ChildValue = request.NumericalOrder,
                                    ParentValueDetail = item.PersonID.ToString(),
                                    ChildValueDetail = "0",
                                });
                            }
                            _biz_Related_FMRepository.Add(new BIZ_Related_FM()
                            {
                                RelatedType = "2205231634370000109",
                                ParentType = "2205231634370000109",
                                ChildType = "0",
                                ParentValue = eData.NumericalOrder,
                                ChildValue = request.NumericalOrder,
                                ParentValueDetail = _identityService.UserId,
                                ChildValueDetail = "0",
                                Remarks = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            await _biz_Related_FMRepository.UnitOfWork.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        //第一次提交 插入阶段流程数据 = 提交成功 进入复核状态
                        if (relate.Count == 0)
                        {
                            _biz_Related_FMRepository.Add(new BIZ_Related_FM()
                            {
                                RelatedType = "2205231634370000109",
                                ParentType = "2205231634370000109",
                                ChildType = "0",
                                ParentValue = eData.NumericalOrder,
                                ChildValue = request.NumericalOrder,
                                ParentValueDetail = _identityService.UserId,
                                ChildValueDetail = "0",
                                Remarks = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            var Rdata = _biz_Related_FMRepository.UnitOfWork.SaveChangesAsync().Result;
                        }
                        else
                        {
                            foreach (var item in relate)
                            {
                                item.RelatedType = "2205231634370000999";
                                _biz_Related_FMRepository.Update(item);
                            }
                            _biz_Related_FMRepository.Add(new BIZ_Related_FM()
                            {
                                RelatedType = "2205231634370000109",
                                ParentType = "2205231634370000109",
                                ChildType = "0",
                                ParentValue = eData.NumericalOrder,
                                ChildValue = request.NumericalOrder,
                                ParentValueDetail = _identityService.UserId,
                                ChildValueDetail = "0",
                                Remarks = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            var Rdata = _biz_Related_FMRepository.UnitOfWork.SaveChangesAsync().Result;
                        }
                    }
                }
                eData.UpdatePayInfo(request.PayUse, request.TransferType);
                eData.IsRecheck = request.IsRecheck;
                //点提交 不赋值复核人
                eData.RecheckId = !request.IsRecheck ? _identityService.UserId : "0";
                eData.TradeNo = request.TradeNo;
                if (!string.IsNullOrEmpty(eData.RecheckId))
                {
                    eData.IsRecheck = true;
                }
                //失败 写入原因
                if (request.failNum == "1")
                {
                    eData.UpdateFailed(request.failInfo.Length > 200 ? "交易失败，原因请查日志" : request.failInfo, 0);
                }
                //前端点确定 更改状态
                //// 发起交易成功  （不代表交易成功）
                //else
                //{
                //    eData.UpdateFailed("", 3);
                //}
                try
                {
                    eData.ModifiedDate = DateTime.Now;
                    _paymentExtendRepository.SaveChange(eData);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("发起支付出现异常：" + JsonConvert.SerializeObject(e));
                }
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
    public class FD_PaymentReceivablesModifyHandler : IRequestHandler<FD_PaymentReceivablesModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_PaymentReceivablesRepository _repository;
        IFD_PaymentReceivablesDetailRepository _detailRepository;
        IFD_SettleReceiptRepository _settleReceiptRepository;
        IFD_SettleReceiptDetailRepository _settleReceiptDetailRepository;
        Ifd_paymentreceivablesvoucherRepository _paymentreceivablesvoucherRepository;
        Ifd_paymentreceivablesvoucherDetailRepository _paymentreceivablesvoucherdetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        IFD_PaymentExtendRepository _paymentExtendRepository;
        IFD_ExpenseRepository _expenseRepository;
        IFD_AccountTransferDetailRepository _AccountTransferDetailRepository;
        IbsfileRepository _ibsfileRepository;
        Ifd_scheduleplanRepository _ScheduleplanRepository;
        FMBaseCommon _baseUnit;
        IBiz_RelatedDetailRepository _biz_RelatedDetailRepository;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil1;
        FD_SettleReceiptNewODataProvider _provider;

        public FD_PaymentReceivablesModifyHandler(FD_SettleReceiptNewODataProvider provider, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration, Ifd_paymentreceivablesvoucherRepository paymentreceivablesvoucherRepository , Ifd_paymentreceivablesvoucherDetailRepository paymentreceivablesvoucherdetailRepository, IBiz_RelatedDetailRepository biz_RelatedDetailRepository, FMBaseCommon baseUnit, Ifd_scheduleplanRepository scheduleplanRepository,IFD_AccountTransferDetailRepository accountTransferDetailRepository,IbsfileRepository ibsfileRepository,IIdentityService identityService, IFD_ExpenseRepository expenseRepository, IFD_PaymentExtendRepository paymentExtendRepository, IFD_SettleReceiptRepository settleReceiptRepository, IFD_SettleReceiptDetailRepository settleReceiptDetailRepository, IFD_PaymentReceivablesRepository repository, IBiz_Related biz_RelatedRepository, IFD_PaymentReceivablesDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
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
            _baseUnit = baseUnit;
            _biz_RelatedDetailRepository = biz_RelatedDetailRepository;
            _paymentreceivablesvoucherRepository = paymentreceivablesvoucherRepository;
            _paymentreceivablesvoucherdetailRepository = paymentreceivablesvoucherdetailRepository;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil;
            _provider = provider;
        }

        public async Task<Result> Handle(FD_PaymentReceivablesModifyCommand request, CancellationToken cancellationToken)
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
                var res = _httpClientUtil1.PostJsonAsync<ResultModel>(requrl, param).Result;
                if ((bool)res?.ResultState)
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
                //判断是否是凭证处理生成
                var Validation = _biz_RelatedRepository.GetRelated(new BIZ_Related() { RelatedType = "201610210104402122", ParentType = "2303061011170000150", ChildType = "1611091727140000101", ParentValue = request.NumericalOrder }).Result;
                if (Validation.Count > 0)
                {
                    var settle = _settleReceiptRepository.Get(Validation.FirstOrDefault().ChildValue);
                    return new Result() { code = ErrorCode.ServerBusy.GetIntValue(), data = new { settle, request }, msg = $"已生成会计凭证{settle.Number}号，请先删除会计凭证" };
                }
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



                var accoList = await _baseUnit.GetSubjectListExtend(0, Convert.ToInt64(string.IsNullOrEmpty(request.EnterpriseID) ? _identityService.EnterpriseId : request.EnterpriseID), request.DataDate.ToString("yyyy-MM-dd"));
                string number = oldDomain.Number;
                string settleNumber = oldSettleDomain.Number;
                var numericalOrder = request.NumericalOrder;
                #region 先删后增
                foreach (var num in list)
                {
                    await _settleReceiptDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();

                    await _settleReceiptRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();

                    await _paymentreceivablesvoucherdetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _paymentreceivablesvoucherRepository.RemoveRangeAsync(o => o.NumericalOrder == num);

                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _paymentExtendRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }
                if (!string.IsNullOrEmpty(request.ApplyNumericalOrder))
                {
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402122" && o.ParentType == request.AppId && o.ChildType == request.ApplyAppId && o.ParentValue == request.NumericalOrder && o.ChildValue == request.ApplyNumericalOrder);
                    await _biz_RelatedDetailRepository.RemoveRangeAsync(o => o.RelatedDetailType == Convert.ToInt64(request.NumericalOrder));
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }
                #endregion
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
                        BankDeposit = o.BankDeposit.Trim(),
                        CollectionId = string.IsNullOrEmpty(o.CollectionId) ? "0" : o.CollectionId,
                        NumericalOrder = numericalOrder,
                        PersonId = string.IsNullOrEmpty(o.PersonId) ? "0" : o.PersonId,
                        Guid = o.Guid
                    });
                });
                //2022-12-07 增加调拨逻辑 如果是账户资金调拨 生成的会计凭证 借方资金账户 为 账户资金调拨 中的 AccountID，汇总单不需要
                FD_AccountTransferDetail tempTransfer = new();
                if (!string.IsNullOrEmpty(request.ApplyNumericalOrder) && request.ApplyAppId != "201611180104402201" && request.ApplyAppId != "201611180104402203" && request.ApplyAppId != "201612070104402204" && string.IsNullOrEmpty(request.ApplyScAppId))
                {
                    tempTransfer = _AccountTransferDetailRepository.GetSingleDataIsIn(request.ApplyNumericalOrder);
                    if (tempTransfer == null)
                    {
                        tempTransfer = new FD_AccountTransferDetail();
                    }
                }
                var tempAcc = accoList.Where(m => m.AccoSubjectID == request.DebitAccoSubjectID).FirstOrDefault();
                if (tempAcc == null)
                {
                    //如果为空 则默认资金科目
                    tempAcc = new BIZ_AccoSubject() { bTorF = true };
                }
                #region 收付款扩展表（凭证）
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
                    SettleReceipType = request.SettleReceipType,
                    details = new List<fd_paymentreceivablesvoucherdetail>()
                };

                voucherDomain.details.Add(new fd_paymentreceivablesvoucherdetail()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    AccountID = tempAcc.bTorF ? (tempTransfer.AccountID != null ? tempTransfer.AccountID : request.details[0].AccountID) : "0",
                    EnterpriseID = request.EnterpriseID,
                    ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                    AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                    AccoSubjectCode = request.details[0].AccoSubjectCode,
                    CustomerID = string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID,
                    PersonID = string.IsNullOrEmpty(request.details[0].SettlePersonID) ? "0" : request.details[0].SettlePersonID,
                    MarketID = string.IsNullOrEmpty(request.details[0].SettleMarketID) ? "0" : request.details[0].SettleMarketID,
                    ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                    ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                    Content = request.details[0].Content,
                    PaymentTypeID = tempAcc.bTorF ? (request.details[0].PaymentTypeID) : "0",
                    Credit = 0,
                    Debit = Convert.ToDecimal(request.details.Sum(m => m.Amount)),
                    LorR = false,
                    RowNum = 1,
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

                //表体传过来的付款单信息都是贷方信息 汇总生成一行借方明细
                request.details?.ForEach(o =>
                {
                    //增加 借方会计科目非资金类科目时，不需要显示结算方式
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
                        AccountID = tempAcc.bTorF ? o.AccountID : "0",
                        EnterpriseID = o.EnterpriseID,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = "0",
                        PersonID = "0",
                        MarketID = "0",
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                        Content = o.Content,
                        PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                        Credit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                        Debit = o.Debit,
                        LorR = true,
                        RowNum = voucherDomain.details.Max(m => m.RowNum) + 1,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                    });
                });
                #endregion
                //退款业务
                if (request.RelatedList != null && request.ApplyAppId == "201611180104402201")
                {
                    foreach (var item in request.RelatedList)
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//付款或付款汇总appid
                            ParentValue = numericalOrder,//付款单/付款汇总 流水号
                            ChildType = item.SettleReceipType,//收款单类型
                            ChildValue = item.ApplyNumericalOrder,//收款流水号
                            ChildValueDetail = item.RecordID,
                            Remarks = "付款单关联收款单（收款退回业务）"
                            //ParentValueDetail = numericalOrder
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                        var a = _biz_RelatedRepository.UnitOfWork.SaveChangesAsync().Result;
                        var rData = _biz_RelatedRepository.GetList(related).FirstOrDefault();
                        _biz_RelatedDetailRepository.Add(new biz_relateddetail()
                        {
                            ModifiedDate = DateTime.Now,
                            OwnerID = Convert.ToInt64(_identityService.UserId),
                            Paid = item.Paid,//实收金额
                            Payment = item.Payment,//累计退款金额
                            Payable = item.Payable,//本次退款金额
                            RelatedID = rData.RelatedID,
                            RelatedDetailType = Convert.ToInt64(numericalOrder),
                            Remarks = "付款单关联收款单（收款退回业务明细）"
                        });
                    }
                    await _biz_RelatedDetailRepository.UnitOfWork.SaveChangesAsync();
                }

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
                        });
                    }
                }
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                await _paymentExtendRepository.AddRangeAsync(domain.extend);
                await _paymentreceivablesvoucherRepository.AddAsync(voucherDomain);
                await _paymentreceivablesvoucherdetailRepository.AddRangeAsync(voucherDomain.details);
                //增加排程流程  获取当前申请 是否已排程
                var plan = _ScheduleplanRepository.GetList(_identityService.GroupId).Where(m => m.ApplyNumericalOrder == request.ApplyNumericalOrder && (m.ScheduleStatus == 2 || m.ScheduleStatus == 3) && m.PayNumericalOrder == request.NumericalOrder).ToList();
                if (plan != null && plan.Count > 0)
                {
                    foreach (var item in plan)
                    {
                        item.ScheduleStatus = 3;
                        _ScheduleplanRepository.Update(item);
                    }
                    await _ScheduleplanRepository.UnitOfWork.SaveChangesAsync();
                    var sumPlan = _ScheduleplanRepository.GetList(_identityService.GroupId).Where(m => m.ApplyNumericalOrder == request.ApplyNumericalOrder && (m.ScheduleStatus == 2 || m.ScheduleStatus == 3)).ToList();
                    if (sumPlan.Sum(m => m.PayAmount) == sumPlan.FirstOrDefault().ApplyAmount)
                    {
                        await SetExpens(request, numericalOrder);
                    }
                    //增加关系 当排程数据完全付完后，在修改申请数据
                    else
                    {
                        var related = new BIZ_Related()
                        {
                            RelatedType = "201610210104402122",
                            ParentType = request.AppId,//付款或付款汇总appid
                            ParentValue = numericalOrder,//付款单/付款汇总 流水号
                            ChildType = request.ApplyAppId,//申请单appid
                            ChildValue = request.ApplyNumericalOrder,//申请流水号
                            Remarks = "申请关联收付款单"
                            //ParentValueDetail = numericalOrder
                        };
                        await _biz_RelatedRepository.AddAsync(related);
                    }
                }
                else
                {
                    await SetExpens(request, numericalOrder);
                }
                if (up != null)
                {
                    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                }
                if (optionStatus)
                {
                    Domain.FD_SettleReceipt settleDomain = CreateSettleReceipt(request, optionStatus, Convert.ToInt64(settleNumber), numericalOrder, accoList, tempTransfer, tempAcc);
                    await _settleReceiptRepository.AddAsync(settleDomain);
                    await _settleReceiptDetailRepository.AddRangeAsync(settleDomain.details);
                    await _settleReceiptRepository.UnitOfWork.SaveChangesAsync();
                    await _settleReceiptDetailRepository.UnitOfWork.SaveChangesAsync();
                    Biz_Review review2 = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                    await _biz_ReviewRepository.AddAsync(review2);
                }
                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                //付款单
                Biz_Review review = new Biz_Review(numericalOrder, request.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherdetailRepository.UnitOfWork.SaveChangesAsync();
                await _paymentreceivablesvoucherRepository.UnitOfWork.SaveChangesAsync();
                if (!string.IsNullOrEmpty(request.ApplyAppId))
                {
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                }
                
                await _paymentExtendRepository.UnitOfWork.SaveChangesAsync();
                await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();

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
        public Domain.FD_SettleReceipt CreateSettleReceipt(FD_PaymentReceivablesModifyCommand request, bool optionStatus, long settleNumber, string numericalOrder, List<BIZ_AccoSubject> accoList, FD_AccountTransferDetail tempTransfer, BIZ_AccoSubject tempAcc)
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
                SettleReceipType =  "201610220104402202",
                details = new List<FD_SettleReceiptDetail>()
            };

            settleDomain.details.Add(new FD_SettleReceiptDetail()
            {
                NumericalOrder = numericalOrder,
                Guid = Guid.NewGuid(),
                AccountID = tempAcc.bTorF ? (tempTransfer.AccountID != null ? tempTransfer.AccountID : request.details[0].AccountID) : "0",
                EnterpriseID = request.EnterpriseID,
                ReceiptAbstractID = request.details[0].ReceiptAbstractID,
                AccoSubjectID = string.IsNullOrEmpty(request.DebitAccoSubjectID) ? "0" : request.DebitAccoSubjectID,
                AccoSubjectCode = request.details[0].AccoSubjectCode,
                CustomerID = string.IsNullOrEmpty(request.details[0].CustomerID) ? "0" : request.details[0].CustomerID,
                PersonID = string.IsNullOrEmpty(request.details[0].SettlePersonID) ? "0" : request.details[0].SettlePersonID,
                MarketID = string.IsNullOrEmpty(request.details[0].SettleMarketID) ? "0" : request.details[0].SettleMarketID,
                ProjectID = string.IsNullOrEmpty(request.details[0].ProjectID) ? "0" : request.details[0].ProjectID,
                ProductID = string.IsNullOrEmpty(request.details[0].ProductID) ? "0" : request.details[0].ProductID,
                Content = request.details[0].Content,
                PaymentTypeID = tempAcc.bTorF ? (request.details[0].PaymentTypeID) : "0",
                Credit = 0,
                Debit = Convert.ToDecimal(request.details.Sum(m => m.Amount)),
                LorR = false,
                RowNum = 1,
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
            //表体传过来的付款单信息都是贷方信息 汇总生成一行借方明细
            request.details?.ForEach(o =>
            {
                //增加 借方会计科目非资金类科目时，不需要显示结算方式
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
                    AccountID = tempAcc.bTorF ? o.AccountID : "0",
                    EnterpriseID = o.EnterpriseID,
                    ReceiptAbstractID = o.ReceiptAbstractID,
                    AccoSubjectID = o.AccoSubjectID,
                    AccoSubjectCode = o.AccoSubjectCode,
                    CustomerID = "0",
                    PersonID = "0",
                    MarketID = "0",
                    ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID,
                    ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID,
                    Content = o.Content,
                    PaymentTypeID = tempAcc.bTorF ? o.PaymentTypeID : "0",
                    Credit = o.Amount == null ? 0.00M : Convert.ToDecimal(o.Amount),
                    Debit = o.Debit,
                    LorR = true,
                    RowNum = settleDomain.details.Max(m => m.RowNum) + 1,
                    OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                });
            });
            return settleDomain;
        }
        private async Task SetExpens(FD_PaymentReceivablesModifyCommand request, string numericalOrder)
        {
            if (!string.IsNullOrEmpty(request.ApplyAppId) && request.ApplyAppId != "201611180104402201" && request.ApplyAppId != "201611180104402203" && request.ApplyAppId != "201612070104402204" && string.IsNullOrEmpty(request.ApplyScAppId))
            {
                var related = new BIZ_Related()
                {
                    RelatedType = "201610210104402122",
                    ParentType = request.AppId,//付款或付款汇总appid
                    ParentValue = numericalOrder,//付款单/付款汇总 流水号
                    ChildType = request.ApplyAppId,//申请单appid
                    ChildValue = request.ApplyNumericalOrder,//申请流水号
                    Remarks = "申请关联收付款单"
                    //ParentValueDetail = numericalOrder
                };
                await _biz_RelatedRepository.AddAsync(related);
                //修改申请单的付款日期和付款人
                var exdomain = _expenseRepository.Get(request.ApplyNumericalOrder);
                if (exdomain != null)
                {
                    exdomain.UpdateApply(DateTime.Now, _identityService.UserId);
                    _expenseRepository.Update(exdomain);
                    await _expenseRepository.UnitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                var item = request.details.FirstOrDefault();
                if (item != null)
                {
                    var transfer = _AccountTransferDetailRepository.GetSingleData(request.ApplyNumericalOrder);
                    if (transfer != null)
                    {
                        transfer.AccountID = item.AccountID;
                        transfer.Amount = (decimal)item.Amount;
                        transfer.DataDateTime = DateTime.Now;
                        transfer.PaymentTypeID = item.PaymentTypeID;
                        _AccountTransferDetailRepository.Update(transfer);
                        await _AccountTransferDetailRepository.UnitOfWork.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
