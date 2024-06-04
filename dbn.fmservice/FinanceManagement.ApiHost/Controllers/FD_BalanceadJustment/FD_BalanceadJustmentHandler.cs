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

namespace FinanceManagement.ApiHost.Controllers.FD_BalanceadJustment
{
    public class FD_BalanceadJustmentAddHandler : IRequestHandler<FD_BalanceadJustmentAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BalanceadJustmentRepository _repository;
        IFD_BalanceadJustmentDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_BalanceadJustmentAddHandler(IIdentityService identityService, IFD_BalanceadJustmentRepository repository, IFD_BalanceadJustmentDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_BalanceadJustmentAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FD_BalanceadJustment>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BalanceadJustment()
                {
                    AccountID = request.AccountID,
                    Number = number.ToString(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    Remarks = request.Remarks,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = _identityService.EnterpriseId
                };
                request.Lines?.ForEach(o =>
                {
                    domain.AddDetail(new FD_BalanceadJustmentDetail()
                    {
                        NumericalOrder = numericalOrder,
                        EnterProjectID = o.EnterProjectID,
                        EnterProjectAmount = o.EnterProjectAmount,
                        BankProjectID = o.BankProjectID,
                        BankProjectAmount = o.BankProjectAmount
                    });
                });
                await _repository.AddAsync(domain);
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
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

    public class FD_BalanceadJustmentDeleteHandler : IRequestHandler<FD_BalanceadJustmentDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BalanceadJustmentRepository _repository;
        IFD_BalanceadJustmentDetailRepository _detailRepository;

        public FD_BalanceadJustmentDeleteHandler(IIdentityService identityService, IFD_BalanceadJustmentRepository repository, IFD_BalanceadJustmentDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_BalanceadJustmentDeleteCommand request, CancellationToken cancellationToken)
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

    public class FD_BalanceadJustmentModifyHandler : IRequestHandler<FD_BalanceadJustmentModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BalanceadJustmentRepository _repository;
        IFD_BalanceadJustmentDetailRepository _detailRepository;
        public FD_BalanceadJustmentModifyHandler(IIdentityService identityService, IFD_BalanceadJustmentRepository repository, IFD_BalanceadJustmentDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_BalanceadJustmentModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(Convert.ToDateTime(request.DataDate), request.AccountID, request.Remarks);
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        domain.AddDetail(new FD_BalanceadJustmentDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            EnterProjectID = item.EnterProjectID,
                            EnterProjectAmount = item.EnterProjectAmount,
                            BankProjectID = item.BankProjectID,
                            BankProjectAmount = item.BankProjectAmount
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                        obj.Update(item.EnterProjectID, item.EnterProjectAmount, item.BankProjectID, item.BankProjectAmount);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                }
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
