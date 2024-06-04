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

namespace FinanceManagement.ApiHost.Controllers.FA_MarketSubject
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FA_MarketSubjectAddHandler : IRequestHandler<FA_MarketSubjectAddCommand, Result>
    {
        IIdentityService _identityService;
        IFA_MarketSubjectRepository _repository;
        IFA_MarketSubjectDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;

        public FA_MarketSubjectAddHandler(IIdentityService identityService, IFA_MarketSubjectRepository repository, IFA_MarketSubjectDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
        }

        public async Task<Result> Handle(FA_MarketSubjectAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {

                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_MarketSubject()
                {
                    DataDate=request.DataDate,
                    Remarks=request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = _identityService.EnterpriseId
                };
                request.Lines?.ForEach(o =>
                {
                    domain.AddDetail(new FA_MarketSubjectDetail()
                    {
                        NumericalOrder = numericalOrder,
                        MarketID =o.MarketID,
                        AccoSubjectID = o.AccoSubjectID,
                        ModifiedDate = DateTime.Now
                    });
                });

                await _repository.AddAsync(domain);
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

    /// <summary>
    /// 删除
    /// </summary>
    public class FA_MarketSubjectDeleteHandler : IRequestHandler<FA_MarketSubjectDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFA_MarketSubjectRepository _repository;
        IFA_MarketSubjectDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_MarketSubjectDeleteHandler(IIdentityService identityService, IFA_MarketSubjectRepository repository, IFA_MarketSubjectDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FA_MarketSubjectDeleteCommand request, CancellationToken cancellationToken)
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
                result.msg = "保存异常";
            }

            return result;
        }
    }

    public class FA_MarketSubjectModifyHandler : IRequestHandler<FA_MarketSubjectModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFA_MarketSubjectRepository _repository;
        IFA_MarketSubjectDetailRepository _detailRepository;
        public FA_MarketSubjectModifyHandler(IIdentityService identityService, IFA_MarketSubjectRepository repository, IFA_MarketSubjectDetailRepository detailRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
        }

        public async Task<Result> Handle(FA_MarketSubjectModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.DataDate, request.Remarks, request.EnterpriseID);

                foreach (var item in request.Lines)
                {
                    if (item.RowStatus == "A" || item.IsCreate)
                    {
                        domain.AddDetail(new FA_MarketSubjectDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            MarketID = item.MarketID,
                            AccoSubjectID = item.AccoSubjectID,
                            ModifiedDate = DateTime.Now
                        });
                    }
                    else if (item.RowStatus == "M" || item.IsUpdate)
                    {
                        var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                        if (obj == null) continue;
                        obj.Update(item.MarketID, item.AccoSubjectID);
                    }
                    else if (item.RowStatus == "D" || item.IsDelete)
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
