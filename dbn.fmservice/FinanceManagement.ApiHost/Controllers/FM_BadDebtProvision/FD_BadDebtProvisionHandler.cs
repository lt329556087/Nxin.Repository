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

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtProvision
{
    public class FD_BadDebtProvisionAddHandler : IRequestHandler<FD_BadDebtProvisionAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _repository;
        IFD_BadDebtProvisionDetailRepository _detailRepository;
        IFD_BadDebtProvisionExtRepository _extRepository;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_Related;

        public FD_BadDebtProvisionAddHandler(IIdentityService identityService, IFD_BadDebtProvisionRepository repository, IFD_BadDebtProvisionDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
            IBiz_ReviewRepository biz_ReviewRepository,
                    IFD_BadDebtProvisionExtRepository extRepository,
                       IBiz_Related biz_Related)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extRepository = extRepository;
            _biz_Related = biz_Related;
        }

        public async Task<Result> Handle(FD_BadDebtProvisionAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                long number = _numberCreator.Create<Domain.FD_BadDebtProvision>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BadDebtProvision()
                {
                    NumericalOrder = numericalOrder,
                    TicketedPointID = request.TicketedPointID,
                    Number = number.ToString(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    EnterpriseID = _identityService.EnterpriseId,
                    MarketID = request.MarketID,
                    PersonID = request.PersonID,
                    Remarks = request.Remarks,
                    CreateDate = DateTime.Now,
                    HaveProvisionAmount1 = request.HaveProvisionAmount1,
                    HaveProvisionAmount2 = request.HaveProvisionAmount2,
                    AccoSubjectID1 = request.AccoSubjectID1,
                    AccoSubjectID2 = request.AccoSubjectID2,
                    NumericalOrderSetting = request.NumericalOrderSetting
                };

                request.Lines.Clear();
                request.Lines1.AddRange(request.Lines2);
                request.Lines1?.ForEach(o =>
                 {
                     var numericalOrderDetail = _numericalOrderCreator.CreateAsync().Result;
                     var detail = new FD_BadDebtProvisionDetail()
                     {
                         NumericalOrder = numericalOrder,
                         NumericalOrderDetail = numericalOrderDetail,
                         AccoSubjectID = o.AccoSubjectID,
                         CustomerID = o.CustomerID,
                         CurrentDebtPrepareAmount = o.CurrentDebtPrepareAmount,
                         LastDebtPrepareAmount = o.LastDebtPrepareAmount,
                         TransferAmount = o.TransferAmount,
                         ProvisionAmount = o.ProvisionAmount,
                         NoReceiveAmount = o.NoReceiveAmount,
                         ReclassAmount = o.ReclassAmount,
                         EndAmount = o.EndAmount,
                         ProvisionType = o.ProvisionType,
                         BusinessType = o.BusinessType,
                         NumericalOrderSpecific = o.NumericalOrderSpecific
                     };

                     o.AgingList.ForEach(a =>
                     {
                         var domainExt = new Domain.FD_BadDebtProvisionExt()
                         {
                             NumericalOrderDetail = numericalOrderDetail,
                             Name = a.Name,
                             Ratio = a.Ratio,
                             Amount = a.Amount
                         };
                         detail.AgingList.Add(domainExt);
                     });

                     domain.Lines.Add(detail);
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

    public class FD_BadDebtProvisionDeleteHandler : IRequestHandler<FD_BadDebtProvisionDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _repository;
        IFD_BadDebtProvisionDetailRepository _detailRepository;

        public FD_BadDebtProvisionDeleteHandler(IIdentityService identityService, IFD_BadDebtProvisionRepository repository, IFD_BadDebtProvisionDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_BadDebtProvisionDeleteCommand request, CancellationToken cancellationToken)
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

    public class FD_BadDebtProvisionModifyHandler : IRequestHandler<FD_BadDebtProvisionModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BadDebtProvisionRepository _repository;
        IFD_BadDebtProvisionDetailRepository _detailRepository;
        public FD_BadDebtProvisionModifyHandler(IIdentityService identityService, IFD_BadDebtProvisionRepository repository, IFD_BadDebtProvisionDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FD_BadDebtProvisionModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);

                domain.MarketID = request.MarketID;
                domain.PersonID = request.PersonID;
                domain.TicketedPointID = request.TicketedPointID;
                domain.Remarks = request.Remarks;

                request.Lines.Clear();
                request.Lines1.AddRange(request.Lines2);

                request.Lines.ForEach(o =>
                {
                    var item = domain.Lines.Find(i => i.NumericalOrderDetail == o.NumericalOrderDetail);
                    if (item != null)
                    {
                        item.TransferAmount = o.TransferAmount;
                        item.ProvisionAmount = o.ProvisionAmount;
                    }
                });

                await _detailRepository.UnitOfWork.SaveChangesAsync();
                result.data = new { NumericalOrder = domain.NumericalOrder };
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
