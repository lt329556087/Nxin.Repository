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

namespace FinanceManagement.ApiHost.Controllers.FD_AccountInventory
{
    public class FD_AccountInventoryAddHandler : IRequestHandler<FD_AccountInventoryAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountInventoryRepository _repository;
        IFD_AccountInventoryDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_AccountInventoryAddHandler(IIdentityService identityService, IFD_AccountInventoryRepository repository, IFD_AccountInventoryDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_AccountInventoryAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FD_AccountInventory>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_AccountInventory()
                {
                    TicketedPointID = request.TicketedPointID,
                    Number = number.ToString(),
                    ResponsiblePerson = request.ResponsiblePerson,
                    Guid = Guid.NewGuid(),
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
                    domain.AddDetail(new FD_AccountInventoryDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        AccountID = o.AccountID,
                        AccoSubjectID = o.AccoSubjectID,
                        AccoSubjectCode = o.AccoSubjectCode,
                        FlowAmount = o.FlowAmount,
                        DepositAmount = o.DepositAmount,
                        FrozeAmount = o.FrozeAmount,
                        FuturesBond = o.FuturesBond,
                        OtherBond = o.OtherBond,
                        BankFrozen = o.BankFrozen,
                        OtherAmount = o.OtherAmount,
                        BookAmount = o.BookAmount,
                        Remarks = o.Remarks
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

    public class FD_AccountInventoryDeleteHandler : IRequestHandler<FD_AccountInventoryDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountInventoryRepository _repository;
        IFD_AccountInventoryDetailRepository _detailRepository;

        public FD_AccountInventoryDeleteHandler(IIdentityService identityService, IFD_AccountInventoryRepository repository, IFD_AccountInventoryDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_AccountInventoryDeleteCommand request, CancellationToken cancellationToken)
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

    public class FD_AccountInventoryModifyHandler : IRequestHandler<FD_AccountInventoryModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_AccountInventoryRepository _repository;
        IFD_AccountInventoryDetailRepository _detailRepository;
        public FD_AccountInventoryModifyHandler(IIdentityService identityService, IFD_AccountInventoryRepository repository, IFD_AccountInventoryDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_AccountInventoryModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(Convert.ToDateTime(request.DataDate), request.TicketedPointID, request.ResponsiblePerson, request.Remarks);
                foreach (var item in request.Lines)
                {
                    var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                    if (item.IsCreate)
                    {
                        domain.AddDetail(new FD_AccountInventoryDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            Guid = new Guid(),
                            AccountID = item.AccountID ?? "0",
                            AccoSubjectID = item.AccoSubjectID,
                            AccoSubjectCode = item.AccoSubjectCode,
                            FlowAmount = item.FlowAmount,
                            DepositAmount = item.DepositAmount,
                            FrozeAmount = item.FrozeAmount,
                            FuturesBond = item.FuturesBond,
                            OtherBond = item.OtherBond,
                            BankFrozen = item.BankFrozen,
                            OtherAmount = item.OtherAmount,
                            BookAmount = item.BookAmount,
                            Remarks = item.Remarks
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        obj.Update(item.AccountID, item.AccoSubjectID, item.AccoSubjectCode, item.FlowAmount, item.DepositAmount, item.FrozeAmount, item.FuturesBond, item.OtherBond, item.BankFrozen, item.OtherAmount, item.BookAmount);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
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
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }


}
