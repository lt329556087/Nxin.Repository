using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtOccur
{
    public class FD_BadDebtOccurAddHandler : IRequestHandler<FD_BadDebtOccurAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtOccurRepository _repository;
        IFD_BadDebtOccurDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_BadDebtOccurAddHandler(IIdentityService identityService, IFD_BadDebtOccurRepository repository, IFD_BadDebtOccurDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_BadDebtOccurAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FD_BadDebtOccur>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BadDebtOccur()
                {
                    TicketedPointID = request.TicketedPointID,
                    NumericalOrder = numericalOrder,
                    Number = number.ToString(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CustomerID = request.CustomerID,
                    PersonID = request.PersonID,
                    BusinessType = request.BusinessType,
                    CreateDate = DateTime.Now,
                    AccoSubjectID1 = request.AccoSubjectID1,
                    AccoSubjectID2 = request.AccoSubjectID2,
                    EnterpriseID = _identityService.EnterpriseId,
                    CAccoSubjectID = request.CAccoSubjectID,
                    NumericalOrderSetting = string.IsNullOrEmpty(request.NumericalOrderSetting) ? "0" : request.NumericalOrderSetting
                };

                domain.Lines.Clear();
                request.Lines1.AddRange(request.Lines2);
                request.Lines1?.ForEach(o =>
                {
                    domain.Lines.Add(new FD_BadDebtOccurDetail()
                    {
                        NumericalOrder = numericalOrder,
                        AccoSubjectID = o.AccoSubjectID ?? "0",
                        MarketID = o.MarketID ?? "0",
                        PersonID = o.PersonID ?? "0",
                        Amount = o.Amount,
                        CurrentOccurAmount = o.CurrentOccurAmount,
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

    public class FD_BadDebtOccurDeleteHandler : IRequestHandler<FD_BadDebtOccurDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtOccurRepository _repository;
        IFD_BadDebtOccurDetailRepository _detailRepository;

        public FD_BadDebtOccurDeleteHandler(IIdentityService identityService, IFD_BadDebtOccurRepository repository, IFD_BadDebtOccurDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_BadDebtOccurDeleteCommand request, CancellationToken cancellationToken)
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
    public class FD_BadDebtOccurModifyHandler : IRequestHandler<FD_BadDebtOccurModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtOccurRepository _repository;
        IFD_BadDebtOccurDetailRepository _detailRepository;
        Nxin_Qlw_BusinessContext _context;
        public FD_BadDebtOccurModifyHandler(IIdentityService identityService, IFD_BadDebtOccurRepository repository, IFD_BadDebtOccurDetailRepository detailRepository, Nxin_Qlw_BusinessContext context)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _context = context;
        }

        public async Task<Result> Handle(FD_BadDebtOccurModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var entity = _context.Set<Domain.FD_BadDebtOccurDetail>().FirstOrDefault();
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.CustomerID = request.CustomerID;
                domain.TicketedPointID = request.TicketedPointID;
                domain.CAccoSubjectID = request.CAccoSubjectID;
                request.Lines.Clear();
                request.Lines.AddRange(request.Lines1);
                request.Lines.AddRange(request.Lines2);
                domain.Lines.ForEach(o =>
                {
                    var item = request.Lines.Find(i => i.RecordID == o.RecordID);
                    if (item != null)
                    {
                        o.Amount = item.Amount;
                        o.CurrentOccurAmount = item.CurrentOccurAmount;
                        o.PersonID = item.PersonID;
                        o.MarketID = item.MarketID;
                    }
                });

                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.data = new { NumericalOrder = domain.NumericalOrder };
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