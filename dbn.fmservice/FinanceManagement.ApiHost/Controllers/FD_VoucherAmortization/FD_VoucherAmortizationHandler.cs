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

namespace FinanceManagement.ApiHost.Controllers.FD_VoucherAmortization
{
    public class FD_VoucherAmortizationAddHandler : IRequestHandler<FD_VoucherAmortizationAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_VoucherAmortizationRepository _repository;
        IFD_VoucherAmortizationDetailRepository _detailRepository;
        IFD_VoucherAmortizationPeriodDetailRepository _periodDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_VoucherAmortizationAddHandler(IIdentityService identityService, IFD_VoucherAmortizationRepository repository, IFD_VoucherAmortizationDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IFD_VoucherAmortizationPeriodDetailRepository periodDetailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _periodDetailRepository = periodDetailRepository;
        }

        public async Task<Result> Handle(FD_VoucherAmortizationAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FD_VoucherAmortization>(o => o.CreatedDate, o => o.Number, DateTime.Now, o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_VoucherAmortization()
                {
                    NumericalOrder = numericalOrder,
                    Number = number.ToString(),
                    AmortizationName = request.AmortizationName,
                    TicketedPointID = request.TicketedPointID,
                    AbstractID = request.AbstractID,
                    Remarks = request.Remarks,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = this._identityService.EnterpriseId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    IsUse = true,
                    OperatorID = "0"
                };
                request.Lines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FD_VoucherAmortizationDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AccoSubjectCode = o.AccoSubjectCode,
                        AccoSubjectID = o.AccoSubjectID,
                        PersonID = string.IsNullOrEmpty(o.PersonID) ? "0" : o.PersonID,
                        CustomerID = string.IsNullOrEmpty(o.CustomerID) ? "0" : o.CustomerID,
                        MarketID = string.IsNullOrEmpty(o.MarketID) ? "0" : o.MarketID,
                        SupplierID = string.IsNullOrEmpty(o.SupplierID) ? "0" : o.SupplierID,
                        ValueNumber = o.ValueNumber,
                        IsDebit = o.IsDebit,
                        ModifiedDate = DateTime.Now,
                    });
                });
                request.PeriodLines?.Select(s => s.IsLast = false);
                request.PeriodLines.Last().IsLast=true;
                request.PeriodLines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    domain.AddPeriodDetail(new FD_VoucherAmortizationPeriodDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        RowNum = o.RowNum,
                        AccountDate = o.AccountDate,
                        AmortizationAmount = o.AmortizationAmount,
                        IsAmort = o.IsAmort,
                        IsLast = o.IsLast,
                        ModifiedDate = DateTime.Now,
                    });
                });
                await _repository.AddAsync(domain);
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _periodDetailRepository.UnitOfWork.SaveChangesAsync();
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
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
    }

    public class FD_VoucherAmortizationDeleteHandler : IRequestHandler<FD_VoucherAmortizationDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_VoucherAmortizationRepository _repository;
        IFD_VoucherAmortizationDetailRepository _detailRepository;
        IFD_VoucherAmortizationPeriodDetailRepository _periodDetailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_VoucherAmortizationDeleteHandler(IIdentityService identityService, IBiz_ReviewRepository biz_ReviewRepository, IFD_VoucherAmortizationRepository repository, IFD_VoucherAmortizationDetailRepository detailRepository, IFD_VoucherAmortizationPeriodDetailRepository periodDetailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _periodDetailRepository = periodDetailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_VoucherAmortizationDeleteCommand request, CancellationToken cancellationToken)
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
                    await _periodDetailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "删除成功!";
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存失败,请联系管理员！";
            }

            return result;
        }
    }

    public class FD_VoucherAmortizationModifyHandler : IRequestHandler<FD_VoucherAmortizationModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_VoucherAmortizationRepository _repository;
        IFD_VoucherAmortizationDetailRepository _detailRepository;
        IFD_VoucherAmortizationPeriodDetailRepository _periodDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_VoucherAmortizationModifyHandler(IIdentityService identityService, IFD_VoucherAmortizationRepository repository, NumericalOrderCreator numericalOrderCreator, IFD_VoucherAmortizationPeriodDetailRepository periodDetailRepository, IFD_VoucherAmortizationDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _periodDetailRepository = periodDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        public async Task<Result> Handle(FD_VoucherAmortizationModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<FD_VoucherAmortizationPeriodDetail> extends = new List<FD_VoucherAmortizationPeriodDetail>();
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.AmortizationName, request.TicketedPointID, request.AbstractID, request.Remarks, DateTime.Now,request.IsUse,request.OperatorID);
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        domain.AddDetail(new FD_VoucherAmortizationDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            AccoSubjectCode = item.AccoSubjectCode,
                            AccoSubjectID = item.AccoSubjectID,
                            PersonID = string.IsNullOrEmpty(item.PersonID) ? "0" : item.PersonID,
                            CustomerID = string.IsNullOrEmpty(item.CustomerID) ? "0" : item.CustomerID,
                            MarketID = string.IsNullOrEmpty(item.MarketID) ? "0" : item.MarketID,
                            SupplierID = string.IsNullOrEmpty(item.SupplierID) ? "0" : item.SupplierID,
                            ValueNumber = item.ValueNumber,
                            IsDebit = item.IsDebit,
                            ModifiedDate = DateTime.Now,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                        obj.Update(item.AccoSubjectCode, item.AccoSubjectID, item.PersonID, item.CustomerID, item.MarketID, item.SupplierID, item.ValueNumber, item.IsDebit,DateTime.Now);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                }
                request.PeriodLines?.Select(s => s.IsLast = false);
                request.PeriodLines.Last().IsLast = true;
                foreach (var item in request.PeriodLines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        domain.AddPeriodDetail(new FD_VoucherAmortizationPeriodDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            RowNum = item.RowNum,
                            AccountDate = item.AccountDate,
                            AmortizationAmount = item.AmortizationAmount,
                            IsAmort = item.IsAmort,
                            IsLast = item.IsLast,
                            ModifiedDate = DateTime.Now
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.PeriodDetails.Find(o => o.RecordID == item.RecordID);
                        obj.Update(item.RowNum, item.AccountDate, item.AmortizationAmount, item.IsAmort, item.IsLast,DateTime.Now);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _periodDetailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                }
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _periodDetailRepository.UnitOfWork.SaveChangesAsync();
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
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
    }

    public class FD_VoucherAmortizationListModifyHandler : IRequestHandler<FD_VoucherAmortizationListModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_VoucherAmortizationRepository _repository;
        IFD_VoucherAmortizationDetailRepository _detailRepository;
        IFD_VoucherAmortizationPeriodDetailRepository _periodDetailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_VoucherAmortizationListModifyHandler(IIdentityService identityService, IFD_VoucherAmortizationRepository repository, NumericalOrderCreator numericalOrderCreator, IFD_VoucherAmortizationPeriodDetailRepository periodDetailRepository, IFD_VoucherAmortizationDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _periodDetailRepository = periodDetailRepository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        public async Task<Result> Handle(FD_VoucherAmortizationListModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (!string.IsNullOrEmpty( request.NumericalOrder))
                {
                    var list = request.NumericalOrder.Split(',');
                    foreach (var item in list)
                    {
                        var domain = await _repository.GetAsync(item);
                        domain.Update(domain.AmortizationName, domain.TicketedPointID, domain.AbstractID, domain.Remarks, DateTime.Now, request.IsUse, _identityService.UserId);
                    }
                    await _repository.UnitOfWork.SaveChangesAsync();
                    result.data = new { NumericalOrder = request.NumericalOrder };
                    result.code = ErrorCode.Success.GetIntValue();
                    return result;
                }
                else
                {
                    return new Result() { code = -1, msg = "请选择需要修改的业务单据" };
                }
                
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
    }

}
