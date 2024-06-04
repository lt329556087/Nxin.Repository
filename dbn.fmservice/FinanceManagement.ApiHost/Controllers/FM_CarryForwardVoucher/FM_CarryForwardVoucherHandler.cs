using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
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

namespace FinanceManagement.ApiHost.Controllers.FM_CarryForwardVoucher
{
    public class FM_CarryForwardVoucherAddHandler : IRequestHandler<FM_CarryForwardVoucherAddCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_CarryForwardVoucherAddHandler(IIdentityService identityService, IFM_CarryForwardVoucherRepository repository, IFM_CarryForwardVoucherDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherFormulaRepository formula)
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

        public async Task<Result> Handle(FM_CarryForwardVoucherAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<FM_CarryForwardVoucherExtend> extends = new List<FM_CarryForwardVoucherExtend>();
                List<FM_CarryForwardVoucherFormula> formulas = new List<FM_CarryForwardVoucherFormula>();
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FM_CarryForwardVoucher()
                {
                    NumericalOrder = numericalOrder,
                    TransferAccountsType = request.TransferAccountsType,
                    TicketedPointID = request.TicketedPointID,
                    DataSource = request.DataSource,
                    TransferAccountsAbstract = request.TransferAccountsAbstract,
                    TransferAccountsSort = string.IsNullOrEmpty(request.TransferAccountsSort) ? "0" : request.TransferAccountsSort,
                    Remarks = request.Remarks,
                    Number="0",
                    OwnerID = this._identityService.UserId,
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
                        IsPigFram = false,
                        IsProject = false,
                        IsSum = false,
                        DebitFormula = o.DebitFormula,
                        DebitSecFormula = o.DebitSecFormula,
                        CreditFormula = o.CreditFormula,
                        CreditSecFormula = o.CreditSecFormula,
                        ModifiedDate = DateTime.Now
                    }) ;
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

    public class FM_CarryForwardVoucherDeleteHandler : IRequestHandler<FM_CarryForwardVoucherDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_CarryForwardVoucherDeleteHandler(IIdentityService identityService, IBiz_ReviewRepository biz_ReviewRepository, IFM_CarryForwardVoucherRepository repository, IFM_CarryForwardVoucherDetailRepository detailRepository, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extend = extend;
            _formula = formula;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_CarryForwardVoucherDeleteCommand request, CancellationToken cancellationToken)
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

    public class FM_CarryForwardVoucherModifyHandler : IRequestHandler<FM_CarryForwardVoucherModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        NumericalOrderCreator _numericalOrderCreator;
        public FM_CarryForwardVoucherModifyHandler(IIdentityService identityService, IFM_CarryForwardVoucherRepository repository, NumericalOrderCreator numericalOrderCreator, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherDetailRepository detailRepository, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extend = extend;
            _formula = formula;
            _numericalOrderCreator = numericalOrderCreator;
        }

        public async Task<Result> Handle(FM_CarryForwardVoucherModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<FM_CarryForwardVoucherDetail> details = new List<FM_CarryForwardVoucherDetail>();
                List<FM_CarryForwardVoucherExtend> extends = new List<FM_CarryForwardVoucherExtend>();
                List<FM_CarryForwardVoucherFormula> formulas = new List<FM_CarryForwardVoucherFormula>();
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.TransferAccountsType, request.TicketedPointID, request.DataSource, request.TransferAccountsAbstract,
                    request.TransferAccountsSort, request.Remarks, string.IsNullOrEmpty(request.TransactorID) ? "0" : request.TransactorID, request.TransactorDate,""
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
                        IsPigFram = false,
                        IsProject = false,
                        IsSum = false,
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

    public class FM_CarryForwardVoucherCopyHandler : IRequestHandler<FM_CarryForwardVoucherCopyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CarryForwardVoucherRepository _repository;
        IFM_CarryForwardVoucherDetailRepository _detailRepository;
        IFM_CarryForwardVoucherExtendRepository _extend;
        IFM_CarryForwardVoucherFormulaRepository _formula;
        NumericalOrderCreator _numericalOrderCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FMBaseCommon _baseUnit;
        public FM_CarryForwardVoucherCopyHandler(IIdentityService identityService, FMBaseCommon baseUnit, IFM_CarryForwardVoucherRepository repository, IBiz_ReviewRepository biz_ReviewRepository, NumericalOrderCreator numericalOrderCreator, IFM_CarryForwardVoucherExtendRepository extend, IFM_CarryForwardVoucherDetailRepository detailRepository, IFM_CarryForwardVoucherFormulaRepository formula)
        {
            _identityService = identityService;
            _repository = repository;
            _baseUnit = baseUnit;
            _detailRepository = detailRepository;
            _extend = extend;
            _formula = formula;
            _numericalOrderCreator = numericalOrderCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_CarryForwardVoucherCopyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request.TransferAccountsType == "1911081429200000104"||request.TransferAccountsType== "1911081429200099105")
                {
                    foreach (var item in request.EnterpriseIds)
                    {
                        var isHave = await _baseUnit.GetCheckenMealJurisdiction(new DropSelectSearch() { EnterpriseID = item });
                        if (!Convert.ToBoolean(isHave.Data))//没有禽成本套餐的话就移除禽成本结转
                        {
                            result.code = ErrorCode.RequestArgumentError.GetIntValue();
                            return result;
                        }
                    }
                }
                var data = await _repository.GetAsync(request.NumericalOrder);
                var dataExtend = await _extend.GetExtends(request.NumericalOrder);
                var dataFormula = await _formula.GetExtends(request.NumericalOrder);
                foreach (var item in request.EnterpriseIds)
                {
                    List<FM_CarryForwardVoucherExtend> extends = new List<FM_CarryForwardVoucherExtend>();
                    List<FM_CarryForwardVoucherFormula> formulas = new List<FM_CarryForwardVoucherFormula>();
                    var numericalOrder = await _numericalOrderCreator.CreateAsync();
                    var domain = new Domain.FM_CarryForwardVoucher()
                    {
                        NumericalOrder = numericalOrder,
                        TransferAccountsType = data.TransferAccountsType,
                        Number="0",
                        TicketedPointID = data.TicketedPointID,
                        DataSource = data.DataSource,
                        TransferAccountsAbstract = data.TransferAccountsAbstract,
                        TransferAccountsSort = string.IsNullOrEmpty(data.TransferAccountsSort) ? "0" : data.TransferAccountsSort,
                        Remarks = data.Remarks,
                        OwnerID = this._identityService.UserId,
                        EnterpriseID = item,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        TransactorID = "0",
                        TransactorDate = data.TransactorDate,
                    };
                    data.Details?.ForEach(o =>
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
                            IsPigFram = false,
                            IsProject = false,
                            IsSum = false,
                            DebitFormula = o.DebitFormula,
                            DebitSecFormula = o.DebitSecFormula,
                            CreditFormula = o.CreditFormula,
                            CreditSecFormula = o.CreditSecFormula,
                            ModifiedDate = DateTime.Now
                        });
                        var dataByNum = dataExtend?.Where(_ => _.NumericalOrder == request.NumericalOrder && _.NumericalOrderDetail == o.NumericalOrderDetail).ToList();
                        dataByNum?.ForEach(s =>
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
                        var dataByNum1 = dataFormula?.Where(_ => _.NumericalOrder == request.NumericalOrder && _.NumericalOrderDetail == o.NumericalOrderDetail).ToList();
                        dataByNum1?.ForEach(s =>
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
}
