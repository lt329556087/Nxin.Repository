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

namespace FinanceManagement.ApiHost.Controllers.FD_CapitalBudget
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_CapitalBudgetAddHandler : IRequestHandler<FD_CapitalBudgetAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_CapitalBudgetRepository _repository;
        IFD_CapitalBudgetDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Qlw_Nxin_ComContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_CapitalBudgetAddHandler(IIdentityService identityService, IFD_CapitalBudgetRepository repository, IFD_CapitalBudgetDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Qlw_Nxin_ComContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_CapitalBudgetAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FD_CapitalBudget>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_CapitalBudget()
                {
                    // TicketedPointID = "0",//request.TicketedPointID,
                    Guid = Guid.NewGuid(),
                    Number = number.ToString(),
                    CapitalBudgetType = "201612130104402103",
                    CapitalBudgetAbstract = request.CapitalBudgetAbstract,
                    StartDate = Convert.ToDateTime(request.StartDate),
                    EndDate = Convert.ToDateTime(request.EndDate),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    Remarks = request.Remarks,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = _identityService.EnterpriseId,
                    MarketID = request.MarketID,
                    Amount = request.Amount
                };

                request.Lines?.ForEach(o =>
                {
                    domain.AddDetail(new FD_CapitalBudgetDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        ReceiptAbstractID = o.ReceiptAbstractID,
                        PaymentObjectID = o.PaymentObjectID,
                        PayAmount = o.PayAmount,
                        ReceiptAmount = o.ReceiptAmount,
                        Remarks = o.Remarks
                    });
                });

                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.AddAsync(domain);
                await _repository.UnitOfWork.SaveChangesAsync();
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
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FD_CapitalBudgetDeleteHandler : IRequestHandler<FD_CapitalBudgetDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_CapitalBudgetRepository _repository;
        IFD_CapitalBudgetDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FD_CapitalBudgetDeleteHandler(IIdentityService identityService, IFD_CapitalBudgetRepository repository, IFD_CapitalBudgetDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_CapitalBudgetDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList<string>();
                }

                foreach (var num in list)
                {
                    //var domain = await _repository.GetAsync(request.NumericalOrder);
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
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

    public class FD_CapitalBudgetModifyHandler : IRequestHandler<FD_CapitalBudgetModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_CapitalBudgetRepository _repository;
        IFD_CapitalBudgetDetailRepository _detailRepository;
        public FD_CapitalBudgetModifyHandler(IIdentityService identityService, IFD_CapitalBudgetRepository repository, IFD_CapitalBudgetDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_CapitalBudgetModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);

                if (string.IsNullOrEmpty(request.DataDate))
                {
                    result.msg = "单据日期不能为空！";
                    result.code = ErrorCode.Update.GetIntValue();
                    return result;
                }

                domain.Update(Convert.ToDateTime(request.DataDate),
                    request.TicketedPointID,
                    request.Number,
                    request.CapitalBudgetAbstract,
                  Convert.ToDateTime(request.StartDate),
                  Convert.ToDateTime(request.EndDate),
                    request.Remarks,
                 DateTime.Now, request.MarketID);

                foreach (var item in request.Lines)
                {
                    var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                    if (item.IsCreate)
                    {
                        domain.AddDetail(
                            new FD_CapitalBudgetDetail()
                            {
                                NumericalOrder = domain.NumericalOrder,
                                Guid = Guid.NewGuid(),
                                ReceiptAbstractID = item.ReceiptAbstractID,
                                PaymentObjectID = item.PaymentObjectID,
                                PayAmount = item.PayAmount,
                                ReceiptAmount = item.ReceiptAmount,
                                Remarks = item.Remarks
                            });
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        obj.Update(item.ReceiptAbstractID, item.PaymentObjectID, item.PayAmount, item.ReceiptAmount, item.Remarks);
                        continue;
                    }
                }
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }






    }
}
