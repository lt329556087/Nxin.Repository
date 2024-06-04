using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_NewCarryForwardVoucher
{
    public class FM_NewCarryForwardVoucherAddHandler : IRequestHandler<FM_NewCarryForwardVoucherAddCommand, Result>
    {

        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_NewCarryForwardVoucherAddHandler(IIdentityService identityService, IFM_CarryForwardVoucherRepository repository, IFM_CarryForwardVoucherDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extend = extend;
            _formula = formula;
        }

        public async Task<Result> Handle(FM_NewCarryForwardVoucherAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<FM_CarryForwardVoucherExtend> extends = new List<FM_CarryForwardVoucherExtend>();
                List<FM_CarryForwardVoucherFormula> formulas = new List<FM_CarryForwardVoucherFormula>();
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                long number = _numberCreator.Create<Domain.FM_CarryForwardVoucher>(o => o.CreatedDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == _identityService.EnterpriseId);
                var domain = new Domain.FM_CarryForwardVoucher()
                {
                    NumericalOrder = numericalOrder,
                    TransferAccountsType = request.TransferAccountsType,
                    TicketedPointID = request.TicketedPointID,
                    Number = number.ToString(),
                    DataSource = request.DataSource,
                    TransferAccountsAbstract = string.IsNullOrEmpty(request.TransferAccountsAbstract) ? "0" : request.TransferAccountsAbstract,
                    TransferAccountsSort = string.IsNullOrEmpty(request.TransferAccountsSort) ? "0" : request.TransferAccountsSort,
                    Remarks = request.Remarks,
                    OwnerID = this._identityService.UserId,
                    SettleNumber=request.SettleNumber,
                    EnterpriseID = this._identityService.EnterpriseId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TransactorID = "0",
                    TransactorDate = request.TransactorDate,
                };
                request.Lines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FM_CarryForwardVoucherDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        AccoSubjectID = o.AccoSubjectID,
                        IsPerson = o.IsPerson,
                        IsCustomer = o.IsCustomer,
                        IsMarket = o.IsMarket,
                        IsProduct = o.IsProduct,
                        IsPigFram = o.IsPigFram,
                        IsProject = o.IsProject,
                        IsSum = o.IsSum,
                        DebitFormula = o.DebitFormula,
                        DebitSecFormula = o.DebitSecFormula,
                        CreditFormula = o.CreditFormula,
                        CreditSecFormula = o.CreditSecFormula,
                        ModifiedDate = DateTime.Now
                    });
                    o.Extends?.ForEach(s =>
                    {
                        extends.Add(new FM_CarryForwardVoucherExtend()
                        {
                            NumericalOrder = numericalOrder,
                            NumericalOrderDetail = numericalOrderDetail.Result,
                            Sort = s.Sort,
                            Symbol = "=",
                            Object = s.Object,
                            ModifiedDate = DateTime.Now
                        });
                    });
                    o.Formulas?.ForEach(s =>
                    {
                        formulas.Add(new FM_CarryForwardVoucherFormula()
                        {
                            NumericalOrder = numericalOrder,
                            NumericalOrderDetail = numericalOrderDetail.Result,
                            RowNum = s.RowNum,
                            Bracket = s.Bracket,
                            FormulaID = s.FormulaID,
                            Operator = s.Operator,
                            ModifiedDate = DateTime.Now
                        });
                    });
                });
                await _repository.AddAsync(domain);
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                if (extends.Count > 0)
                {
                    await _extend.AddRangeAsync(extends);
                }
                if (formulas.Count > 0)
                {
                    await _formula.AddRangeAsync(formulas);
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
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

    public class FM_NewCarryForwardVoucherDeleteHandler : IRequestHandler<FM_NewCarryForwardVoucherDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_NewCarryForwardVoucherDeleteHandler(IIdentityService identityService, IBiz_ReviewRepository biz_ReviewRepository, IFM_CarryForwardVoucherRepository repository, IFM_CarryForwardVoucherDetailRepository detailRepository, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extend = extend;
            _formula = formula;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_NewCarryForwardVoucherDeleteCommand request, CancellationToken cancellationToken)
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
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _extend.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _formula.RemoveRangeAsync(o => o.NumericalOrder == num);
                }
                await _repository.UnitOfWork.SaveChangesAsync();
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

    public class FM_NewCarryForwardVoucherModifyHandler : IRequestHandler<FM_NewCarryForwardVoucherModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        NumericalOrderCreator _numericalOrderCreator;
        public FM_NewCarryForwardVoucherModifyHandler(IIdentityService identityService, IFM_CarryForwardVoucherRepository repository, NumericalOrderCreator numericalOrderCreator, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherDetailRepository detailRepository, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extend = extend;
            _formula = formula;
            _numericalOrderCreator = numericalOrderCreator;
        }
        public async Task<Result> Handle(FM_NewCarryForwardVoucherModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<FM_CarryForwardVoucherDetail> details = new List<FM_CarryForwardVoucherDetail>();
                List<FM_CarryForwardVoucherExtend> extends = new List<FM_CarryForwardVoucherExtend>();
                List<FM_CarryForwardVoucherFormula> formulas = new List<FM_CarryForwardVoucherFormula>();
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.TransferAccountsType, request.TicketedPointID, request.DataSource, request.TransferAccountsAbstract,
                    request.TransferAccountsSort, request.Remarks, string.IsNullOrEmpty(request.TransactorID) ? "0" : request.TransactorID, request.TransactorDate,request.SettleNumber
                    );
                await _detailRepository.RemoveRangeAsync(s => s.NumericalOrder == domain.NumericalOrder);
                foreach (var item in request.Lines)
                {
                    var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                    details.Add(new FM_CarryForwardVoucherDetail()
                    {
                        NumericalOrder = domain.NumericalOrder,
                        NumericalOrderDetail = numericalOrderDetail,
                        ReceiptAbstractID = item.ReceiptAbstractID,
                        AccoSubjectCode = item.AccoSubjectCode,
                        AccoSubjectID = item.AccoSubjectID,
                        IsPerson = item.IsPerson,
                        IsCustomer = item.IsCustomer,
                        IsMarket = item.IsMarket,
                        IsProduct = item.IsProduct,
                        IsPigFram = item.IsPigFram,
                        IsProject = item.IsProject,
                        IsSum = item.IsSum,
                        DebitFormula = item.DebitFormula,
                        DebitSecFormula = item.DebitSecFormula,
                        CreditFormula = item.CreditFormula,
                        CreditSecFormula = item.CreditSecFormula,
                        ModifiedDate = DateTime.Now
                    });
                    item.Extends?.ForEach(s =>
                    {
                        extends.Add(new FM_CarryForwardVoucherExtend()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            Sort = s.Sort,
                            Symbol = "=",
                            Object = s.Object,
                            ModifiedDate = DateTime.Now
                        });
                    });
                    item.Formulas?.ForEach(s =>
                    {
                        formulas.Add(new FM_CarryForwardVoucherFormula()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            RowNum = s.RowNum,
                            Bracket = s.Bracket,
                            FormulaID = s.FormulaID,
                            Operator = s.Operator,
                            ModifiedDate = DateTime.Now
                        });
                    });
                }
                await _extend.RemoveRangeAsync(s => s.NumericalOrder == domain.NumericalOrder);
                if (extends.Count > 0)
                {
                    await _extend.AddRangeAsync(extends);
                }
                await _formula.RemoveRangeAsync(s => s.NumericalOrder == domain.NumericalOrder);
                if (formulas.Count > 0)
                {
                    await _formula.AddRangeAsync(formulas);
                }
                foreach (var detail in details)
                {
                    _detailRepository.Add(detail);
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                }
                await _repository.UnitOfWork.SaveChangesAsync();
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

    public class FM_NewCarryForwardVoucherCopyHandler : IRequestHandler<FM_NewCarryForwardVoucherCopyCommand, Result>
    {
        private readonly string inventorypid = "2016030703636788604";
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        NumericalOrderCreator _numericalOrderCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        FMBaseCommon _baseUnit;
        FM_NewCarryForwardVoucherODataProvider _prodiver;
        TreeModelODataProvider _treeModel;
        BIZ_DataDictODataProvider _dictProvider;
        public FM_NewCarryForwardVoucherCopyHandler(IIdentityService identityService, FM_NewCarryForwardVoucherODataProvider prodiver, BIZ_DataDictODataProvider dictProvider, TreeModelODataProvider treeModel, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, FMBaseCommon baseUnit, IFM_CarryForwardVoucherRepository repository, IBiz_ReviewRepository biz_ReviewRepository, NumericalOrderCreator numericalOrderCreator, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherDetailRepository detailRepository, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _baseUnit = baseUnit;
            _detailRepository = detailRepository;
            _extend = extend;
            _formula = formula;
            _numericalOrderCreator = numericalOrderCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _numberCreator = numberCreator;
            _prodiver = prodiver;
            _treeModel = treeModel;
            _dictProvider = dictProvider;
        }
        public async Task<Result> Handle(FM_NewCarryForwardVoucherCopyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var data = _prodiver.GetSingleData(Convert.ToInt64(request.NumericalOrder));
                foreach (var enter in request.EnterpriseIds)
                {
                    List<Subject> subjectList = GetSubjectList(enter, request.LastDate);//获取本单位会计科目
                    List<Biz_Settlesummary> receiptAbstractList = GetReceiptAbstractList(enter, request.LastDate);//获取本单位摘要
                    data = ConvertToEnterprise(Convert.ToInt64(data.TransferAccountsType), enter, data);
                    List<FM_CarryForwardVoucherExtend> extends = new List<FM_CarryForwardVoucherExtend>();
                    List<FM_CarryForwardVoucherFormula> formulas = new List<FM_CarryForwardVoucherFormula>();
                    //非本单位复制
                    var ticketedPointId = "0";
                    if (data?.EnterpriseID != enter)
                    {
                        //单据字
                        var ticketedPointList = _baseUnit.GetSymbol(enter).Result.Data;
                        if (ticketedPointList?.Count > 0)
                        {
                            ticketedPointId = ticketedPointList.FirstOrDefault().TicketedPointID;
                        }
                    }
                    else
                    {
                        ticketedPointId = data?.TicketedPointID;
                    }
                    var numericalOrder = await _numericalOrderCreator.CreateAsync();
                    long number = _numberCreator.Create<Domain.FM_CarryForwardVoucher>(o => o.CreatedDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == enter);
                    var domain = new Domain.FM_CarryForwardVoucher()
                    {
                        NumericalOrder = numericalOrder,
                        TransferAccountsType = data.TransferAccountsType,
                        Number = number.ToString(),
                        TicketedPointID = ticketedPointId,//data.TicketedPointID,
                        DataSource = data.DataSource,
                        TransferAccountsAbstract = string.IsNullOrEmpty(request.TransferAccountsAbstract) ? "0" : request.TransferAccountsAbstract,
                        TransferAccountsSort = string.IsNullOrEmpty(data.TransferAccountsSort) ? "0" : data.TransferAccountsSort,
                        Remarks = data.Remarks + "_复制",
                        OwnerID = this._identityService.UserId,
                        SettleNumber=data.SettleNumber,
                        EnterpriseID = enter,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        TransactorID = "0",
                        TransactorDate = Convert.ToDateTime(data.TransactorDate),
                    };
                    data.Lines?.ForEach(o =>
                    {
                        var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                        var subject = subjectList.Where(s => s.cAccoSubjectFullName == o.AccoSubjectName).FirstOrDefault();
                        var receiptAbstract = receiptAbstractList.Where(s => s.name == o.ReceiptAbstractName).FirstOrDefault();
                        domain.AddDetail(new FM_CarryForwardVoucherDetail()
                        {
                            NumericalOrder = numericalOrder,
                            NumericalOrderDetail = numericalOrderDetail.Result,
                            ReceiptAbstractID = receiptAbstract?.id ?? "0",
                            AccoSubjectCode = subject?.cAccoSubjectCode ?? "0",
                            AccoSubjectID = subject?.AccoSubjectID ?? "0",
                            IsPerson = o.IsPerson,
                            IsCustomer = o.IsCustomer,
                            IsMarket = o.IsMarket,
                            IsPigFram = o.IsPigFram,
                            IsProject = o.IsProject,
                            IsSum = o.IsSum,
                            IsProduct = o.IsProduct,
                            DebitFormula = o.DebitFormula,
                            DebitSecFormula = o.DebitSecFormula,
                            CreditFormula = o.CreditFormula,
                            CreditSecFormula = o.CreditSecFormula,
                            ModifiedDate = DateTime.Now
                        });
                        o.Extends?.ForEach(s =>
                        {
                            extends.Add(new FM_CarryForwardVoucherExtend()
                            {
                                NumericalOrder = numericalOrder,
                                NumericalOrderDetail = numericalOrderDetail.Result,
                                Sort = s.Sort,
                                Symbol = "=",
                                Object = s.Object,
                                ModifiedDate = DateTime.Now
                            });
                        });
                        o.Formulas?.ForEach(s =>
                        {
                            formulas.Add(new FM_CarryForwardVoucherFormula()
                            {
                                NumericalOrder = numericalOrder,
                                NumericalOrderDetail = numericalOrderDetail.Result,
                                RowNum = s.RowNum,
                                Bracket = s.Bracket,
                                FormulaID = s.FormulaID,
                                Operator = s.Operator,
                                ModifiedDate = DateTime.Now
                            });
                        });
                    });
                    await _repository.AddAsync(domain);
                    Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                    await _biz_ReviewRepository.AddAsync(review);
                    if (extends.Count > 0)
                    {
                        await _extend.AddRangeAsync(extends);
                    }
                    if (formulas.Count > 0)
                    {
                        await _formula.AddRangeAsync(formulas);
                    }
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }

        private List<Subject> GetSubjectList(string EnterpriseId, string LastDate)
        {
            var result = _baseUnit.GetSubjectList(0, Convert.ToInt64(EnterpriseId), LastDate).Result;
            return result;
        }
        private List<Biz_Settlesummary> GetReceiptAbstractList(string EnterpriseId,string LastDate)
        {
            var result = _baseUnit.GetReceiptAbstractAllList(EnterpriseId, LastDate).Result;
            return result;
        }
        private FM_NewCarryForwardVoucherODataEntity ConvertToEnterprise(long TransferAccountsType, string EnterpriseId, FM_NewCarryForwardVoucherODataEntity main)
        {
            List<TreeModelODataEntity> produtClassList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> supplierList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> suppliesList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> chickenList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> breedList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> batchList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> changquList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> jisheList = new List<TreeModelODataEntity>();
            List<BIZ_DataDictODataEntity> dictList = new List<BIZ_DataDictODataEntity>();
            List<TreeModelODataEntity> marketList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> pigfarmList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> chickfarmList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> personList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> porductList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> pigtypeList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> assetsClassificationList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> outInTypeList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> invoiceTypeList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> carriageAbstractList = new List<TreeModelODataEntity>();
            List<TreeModelODataEntity> inventoryList = new List<TreeModelODataEntity>();
            List<BIZ_DataDictODataEntity> costList = new List<BIZ_DataDictODataEntity>();
            List<BIZ_DataDictODataEntity> dictlist = new List<BIZ_DataDictODataEntity>();
            bool IsSetNewFormula = false;
            switch (TransferAccountsType)
            {
                case (long)TransferAccountsTypeEnum.销售结转:
                case (long)TransferAccountsTypeEnum.采购结转:
                case (long)TransferAccountsTypeEnum.福利计提:
                case (long)TransferAccountsTypeEnum.养户成本结转:
                case (long)TransferAccountsTypeEnum.禽成本蛋鸡结转:
                case (long)TransferAccountsTypeEnum.禽成本结转:
                case (long)TransferAccountsTypeEnum.损益结转:
                case (long)TransferAccountsTypeEnum.折旧计提:
                case (long)TransferAccountsTypeEnum.物品结转:
                case (long)TransferAccountsTypeEnum.预收款核销:
                case (long)TransferAccountsTypeEnum.税费抵扣:
                case (long)TransferAccountsTypeEnum.运费结转:
                case (long)TransferAccountsTypeEnum.生产成本结转:
                    dictlist.AddRange(_prodiver.GetDataDictAsync(TransferAccountsType));
                    break;
                case (long)TransferAccountsTypeEnum.薪资计提:
                    dictlist.AddRange(_prodiver.GetSalarySetItemList(EnterpriseId,out IsSetNewFormula));
                    break;
                case (long)TransferAccountsTypeEnum.费用分摊结转:
                case (long)TransferAccountsTypeEnum.猪成本结转:
                    dictlist.AddRange(_prodiver.GetDataDictAsync(TransferAccountsType));
                    dictlist.AddRange(_prodiver.GetShareCostItemList(EnterpriseId, out IsSetNewFormula));
                    break;
                default:
                    break;
            }
            dictlist.Add(new BIZ_DataDictODataEntity() { DataDictID = "2208081518000000151", DataDictName = "贷方合计" });
            dictlist.Add(new BIZ_DataDictODataEntity() { DataDictID = "2208081518000000152", DataDictName = "借方合计" });

            #region 分录条件基础信息获取
            List<FM_NewCarryForwardVoucherExtendODataEntity> ExtendList = new List<FM_NewCarryForwardVoucherExtendODataEntity>();
            foreach (var item in main.Lines)
            {
                ExtendList.AddRange(item.Extends);
            }
            if (ExtendList.Count() > 0)
            {
                var groupKey = ExtendList.GroupBy(s => s.Sort).Select(s => s.Key);
                foreach (var key in groupKey)
                {
                    switch (key)
                    {
                        case (int)SortTypeEnum.商品分类:
                            produtClassList = _treeModel.GetProductGroupClassAsync(_identityService.GroupId);
                            break;
                        case (int)SortTypeEnum.供应商:
                            supplierList = _treeModel.GetSupplierAsync(EnterpriseId);
                            break;
                        case (int)SortTypeEnum.物品分类:
                            suppliesList = _treeModel.GetSuppliesAsync(EnterpriseId);
                            break;
                        case (int)SortTypeEnum.鸡场:
                            chickenList = _baseUnit.GetChickenFarm(new DropSelectSearch() { EnterpriseID = EnterpriseId });
                            break;
                        case (int)SortTypeEnum.品种:
                            breedList = _baseUnit.GetBreeding(new DropSelectSearch() { EnterpriseID = EnterpriseId });
                            break;
                        case (int)SortTypeEnum.批次:
                            batchList = _baseUnit.GetBatching(new DropSelectSearch() { EnterpriseID = EnterpriseId });
                            break;
                        case (int)SortTypeEnum.厂区:
                            changquList = _baseUnit.GetJurisdictionList(new DropSelectSearch() { EnterpriseID = EnterpriseId }, "2");
                            break;
                        case (int)SortTypeEnum.鸡舍:
                            jisheList = _baseUnit.GetJurisdictionList(new DropSelectSearch() { EnterpriseID = EnterpriseId }, "3");
                            break;
                        case (int)SortTypeEnum.销售摘要:
                            dictList.AddRange(_dictProvider.GetDataDictAsync("201610140104402501").Result);//销售摘要
                            break;
                        case (int)SortTypeEnum.采购摘要:
                            dictList.AddRange(_dictProvider.GetDataDictAsync("201610140104402301").Result);//采购摘要
                            break;
                        case (int)SortTypeEnum.费用性质:
                            costList.AddRange(_dictProvider.GetDataDictAsyncExtend("202205111355001101").Result);//费用性质
                            break;
                        case (int)SortTypeEnum.猪场:
                            pigfarmList.AddRange(_baseUnit.GetPigFarm(new DropSelectSearch() { EnterpriseID = EnterpriseId }));//猪场
                            break;
                        case (int)SortTypeEnum.养殖场:
                            chickfarmList.AddRange(_baseUnit.getChickenFarmList( EnterpriseId));//养殖场
                            break;
                        case (int)SortTypeEnum.部门:
                            marketList = _baseUnit.GetMarket(new DropSelectSearch() { EnterpriseID = EnterpriseId });
                            break;
                        case (int)SortTypeEnum.人员:
                            personList = _baseUnit.GetPerson(new DropSelectSearch() { EnterpriseID = EnterpriseId });
                            break;
                        case (int)SortTypeEnum.商品代号:
                            porductList = _baseUnit.getProductData(new DropSelectSearch() { EnterpriseID = EnterpriseId });
                            break;
                        case (int)SortTypeEnum.猪只类型:
                            pigtypeList = _baseUnit.GetPigTypes(EnterpriseId);
                            break;
                        case (int)SortTypeEnum.资产类别:
                            assetsClassificationList = _baseUnit.getFA_AssetsClassificationData(_identityService.GroupId);
                            break;
                        case (int)SortTypeEnum.出入库方式:
                            outInTypeList = _baseUnit.GetInOutAbstract();
                            break;
                        case (int)SortTypeEnum.发票类型:
                            invoiceTypeList = _baseUnit.GetInvoiceType();
                            break;
                        case (int)SortTypeEnum.运费摘要:
                            carriageAbstractList = _baseUnit.GetCarriageAbstract(EnterpriseId);
                            break;
                        case (int)SortTypeEnum.存货分类:
                            inventoryList = _dictProvider.GetDataDictConvertDrop(inventorypid);
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion
            foreach (var line in main?.Lines)
            {
                #region 公式信息匹配赋值
               string dformula = !string.IsNullOrEmpty(line.DebitSecFormula)? line.DebitSecFormula:"" ;
               string cformula = !string.IsNullOrEmpty(line.CreditSecFormula)? line.CreditSecFormula:"";
                foreach (var formula in line?.Formulas)
                {
                    if (TransferAccountsType.ToString()== "1911081429200099102" ||
                        dformula.Contains("2201101409320000151") ||
                        cformula.Contains("2201101409320000151") ||
                        dformula.Contains("2201101409320000152") ||
                        cformula.Contains("2201101409320000152"))
                    {
                        var dict = dictlist.Where(s => s.DataDictName == formula.FormulaName).FirstOrDefault();
                        if (dict != null)
                        {
                            if (!string.IsNullOrEmpty(dformula))
                            {
                                dformula = dformula?.Replace("[" + formula.FormulaID + "]", "[" + dict.DataDictID + "]");
                            }
                            if (!string.IsNullOrEmpty(cformula))
                            {
                                cformula = cformula?.Replace("[" + formula.FormulaID + "]", "[" + dict.DataDictID + "]");
                            }
                            formula.FormulaID = dict.DataDictID;
                        }
                        line.DebitSecFormula = dformula;
                        line.CreditSecFormula = cformula;
                    }
                }
                #endregion
                #region 分录信息匹配赋值
                foreach (var extend in line?.Extends)
                {
                    switch (extend.Sort)
                    {
                        case (int)SortTypeEnum.商品分类:
                            extend.Object = produtClassList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.供应商:
                            extend.Object = supplierList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.物品分类:
                            extend.Object = suppliesList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.鸡场:
                            extend.Object = chickenList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.品种:
                            extend.Object = breedList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.批次:
                            extend.Object = batchList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.厂区:
                            extend.Object = changquList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.鸡舍:
                            extend.Object = jisheList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.销售摘要:
                            extend.Object = dictList?.Where(_ => _.DataDictName == extend.ObjectName)?.FirstOrDefault()?.DataDictID ?? "0";
                            break;
                        case (int)SortTypeEnum.采购摘要:
                            extend.Object = dictList?.Where(_ => _.DataDictName == extend.ObjectName)?.FirstOrDefault()?.DataDictID ?? "0";
                            break;
                        case (int)SortTypeEnum.费用性质:
                            extend.Object = costList?.Where(_ => _.DataDictName == extend.ObjectName)?.FirstOrDefault()?.DataDictID ?? "0";
                            break;
                        case (int)SortTypeEnum.猪场:
                            extend.Object = pigfarmList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.养殖场:
                            extend.Object = chickfarmList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.部门:
                            if (!string.IsNullOrEmpty(extend.ObjectName))
                            {
                                extend.Object = marketList?.Where(_ => _.cName.Contains(extend.ObjectName?.Substring(extend.ObjectName.Length - 3)) && _.ExtendId.ToLower() == "true")?.FirstOrDefault()?.Id ?? "0";
                            }
                            break;
                        case (int)SortTypeEnum.人员:
                            extend.Object = personList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.商品代号:
                            extend.Object = porductList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.猪只类型:
                            extend.Object = pigtypeList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.资产类别:
                            extend.Object = assetsClassificationList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.出入库方式:
                            extend.Object = outInTypeList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.发票类型:
                            extend.Object = invoiceTypeList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.运费摘要:
                            extend.Object = carriageAbstractList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        case (int)SortTypeEnum.存货分类:
                            extend.Object = inventoryList?.Where(_ => _.cName == extend.ObjectName)?.FirstOrDefault()?.Id ?? "0";
                            break;
                        default:
                            break;
                    }
                }
                #endregion
            }
            return main;
        }

    }

    public class FD_SettleReceiptSaveHandler : IRequestHandler<FinanceManagement.Common.NewMakeVoucherCommon.FD_SettleReceipt, Result>
    {
        IIdentityService _identityService;
        IFD_SettleReceiptRepository _repository;
        IFM_CarryForwardVoucherRecordRepository _recordrepository;
        IFD_SettleReceiptDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FMBaseCommon _baseUnit;
        FD_SettleReceiptNewODataProvider _provider;
        VoucherAmortizationUtil _comUtil;
        private TreeModelODataProvider _treeModel;
        private readonly ILogger<PerformanceIncomeEntity> _logger;

        public FD_SettleReceiptSaveHandler(IIdentityService identityService, VoucherAmortizationUtil comUtil, FMBaseCommon baseUnit, FD_SettleReceiptNewODataProvider provider, TreeModelODataProvider treeModel, IFD_SettleReceiptRepository repository, IFM_CarryForwardVoucherRecordRepository recordrepository, IFD_SettleReceiptDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, ILogger<PerformanceIncomeEntity> logger)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _baseUnit = baseUnit;
            _comUtil = comUtil;
            _biz_ReviewRepository = biz_ReviewRepository;
            _treeModel = treeModel;
            _provider = provider;
            _recordrepository = recordrepository;
            _logger = logger;

        }
        public async Task<Result> Handle(FinanceManagement.Common.NewMakeVoucherCommon.FD_SettleReceipt request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                FM_CarryForwardVoucherRecord record = new FM_CarryForwardVoucherRecord()
                {
                    NumericalOrderCarry = request.CarryData.NumericalOrder,
                    TransferAccountsType = request.CarryData.TransferAccountsType,
                    CarryName = request.CarryData.Remarks,
                    DataSource = request.CarryData.DataSource,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = _identityService.EnterpriseId,
                    TransBeginDate = Convert.ToDateTime(request.TransBeginDate),
                    TransEndDate = Convert.ToDateTime(request.TransEndDate),
                    TransSummary = request.SummaryType,
                    TransSummaryName = request.SummaryTypeName,
                    TransWhereList = request.ProductLst,
                    CreatedDate = DateTime.Now,
                    Remarks = request.RptSearchText,
                    TicketedPointID = request.TicketedPointID,
                    TransferAccountsAbstract = request.CarryData.TransferAccountsAbstract,
                    ModifiedDate = DateTime.Now,
                };
                result = Verification(request);
                if (result.code > 0)
                {
                    record.ImplementResult = result.msg;
                    record.ResultState = false;
                    record.NumericalOrderSettl = "0";
                    await _recordrepository.AddAsync(record);
                    await _recordrepository.UnitOfWork.SaveChangesAsync();
                    return result;
                }

                List<Common.MakeVoucherCommon.EnterprisePeriod> periods = new List<Common.MakeVoucherCommon.EnterprisePeriod>();
                //获取会计期间
                periods = _comUtil.getEnterprisePeriod(new Common.MakeVoucherCommon.EnterprisePeriodSearch() { EnterpriseID = request.EnterpriseID, Year = Convert.ToDateTime(request.DataDate).Year, Month = Convert.ToDateTime(request.DataDate).Month }).Result;
                //会计凭证支持手工录入凭证号
                long number = Convert.ToInt64(_provider.GetMaxNumberByDate(request.SettleReceipType, request.EnterpriseID, periods?.FirstOrDefault()?.StartDate.ToString("yyyy-MM-dd"), periods?.FirstOrDefault()?.EndDate.ToString("yyyy-MM-dd"), request.TicketedPointID).MaxNumber);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                _logger.LogInformation("月末处理凭证结果" + JsonConvert.SerializeObject(request));
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
                    Remarks = request.CarryData.TransferAccountsTypeName + "模板生成",
                    EnterpriseID = _identityService.EnterpriseId,
                    OwnerID = _identityService.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    details = new List<FD_SettleReceiptDetail>()
                };
                int i = 1;
                //request.Lines=request.Lines.OrderByDescending(s => s.Debit).ToList();
                request.Lines?.ForEach(o =>
                {
                    domain.details.Add(new FD_SettleReceiptDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        EnterpriseID = _identityService.EnterpriseId,
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        AccoSubjectID = o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID.Replace("[null]", "0"),
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID.Replace("[null]", "0"),
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID.Replace("[null]", "0"),
                        ProjectID = string.IsNullOrEmpty(o.ProjectID) ? "0" : o.ProjectID.Replace("[null]", "0"),
                        ProductID = string.IsNullOrEmpty(o.ProductID) ? "0" : o.ProductID.Replace("[null]", "0"),
                        ClassificationID = string.IsNullOrEmpty(o.ClassificationID) ? "0" : o.ClassificationID.Replace("[null]", "0"),
                        PaymentTypeID = o.PaymentTypeID,
                        AccountID = o.AccountID,
                        LorR = o.LorR,
                        Credit = Math.Round(o.Credit,2) ,
                        Debit = Math.Round(o.Debit, 2),
                        Content = request.CarryData.Remarks,
                        RowNum = i++,
                        OrganizationSortID = string.IsNullOrEmpty(o.OrganizationSortID) ? "0" : o.OrganizationSortID,
                        IsCharges = false
                    });
                });
                var amountAllDebitAmount = request.Lines?.Sum(k => Convert.ToDecimal(k.Debit));
                var amountAllCebitAmount = request.Lines?.Sum(k => Convert.ToDecimal(k.Credit));
                if (amountAllDebitAmount != amountAllCebitAmount)
                {
                    record.ImplementResult = "借贷方金额不平";
                    result.code = ErrorCode.Update.GetIntValue();
                    record.ResultState = false;
                }
                else
                {
                    record.ImplementResult = "执行成功";
                    result.code = ErrorCode.Success.GetIntValue();
                    record.ResultState = true;
                }
                record.NumericalOrderSettl = numericalOrder;
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(domain.details);
                Biz_Review review = new Biz_Review(numericalOrder, "1611091727140000101", _identityService.UserId).SetMaking();
                await _recordrepository.AddAsync(record);
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _recordrepository.UnitOfWork.SaveChangesAsync();
                result.msg = record.ImplementResult;
                result.data = new { NumericalOrder = numericalOrder, Number = number, request = request, domain = domain };
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.data = ex.ToString()+"参数"+JsonConvert.SerializeObject(request.Lines) ;
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }

        /// <summary>
        /// 保存凭证基础验证
        /// </summary>
        /// <returns></returns>
        private Result Verification(FinanceManagement.Common.NewMakeVoucherCommon.FD_SettleReceipt model)
        {
            Result mdata = new Result();
            if (model.IsSettleCheckOut)
            {
                mdata.code = ErrorCode.Create.GetIntValue(); ;
                mdata.msg = "该期间会计凭证已结账！";
                return mdata;
            }
            if (model.Lines?.Count <= 0)
            {
                mdata.code = ErrorCode.Create.GetIntValue();
                mdata.msg = "该期间没有数据发生！";
                return mdata;
            }
            if (!string.IsNullOrEmpty(model.ErrorMsg))
            {
                mdata.code = ErrorCode.Create.GetIntValue();
                mdata.msg = model.ErrorMsg;
                return mdata;
            }
            mdata.code = 0;
            return mdata;
        }
        
    }
}
