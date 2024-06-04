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

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtSetting
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_BaddebtSettingAddHandler : IRequestHandler<FD_BaddebtSettingAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtSettingRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;

        public FD_BaddebtSettingAddHandler(IIdentityService identityService, IFD_BaddebtSettingRepository repository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
        }

        public async Task<Result> Handle(FD_BaddebtSettingAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BaddebtSetting()
                {
                    DataDate=request.DataDate,
                    ProvisionMethod = request.ProvisionMethod,
                    BadAccoSubjectOne = request.BadAccoSubjectOne,
                    BadAccoSubjectTwo = request.BadAccoSubjectTwo,
                    OtherAccoSubjectOne = request.OtherAccoSubjectOne,
                    OtherAccoSubjectTwo = request.OtherAccoSubjectTwo,
                    DebtReceAccoSubjectOne = request.DebtReceAccoSubjectOne,
                    DebtReceAccoSubjectTwo = request.DebtReceAccoSubjectTwo,
                    ProvisionReceiptAbstractID = request.ProvisionReceiptAbstractID,
                    OccurReceiptAbstractID = request.OccurReceiptAbstractID,
                    RecoverReceiptAbstractID = request.RecoverReceiptAbstractID,
                    ReversalReceiptAbstractID=request.ReversalReceiptAbstractID,
                    BadReversalReceiptAbstractID=request.BadReversalReceiptAbstractID,
                    ReceAccoSubjectOne=request.ReceAccoSubjectOne,
                    ReceAccoSubjectTwo=request.ReceAccoSubjectTwo,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID
                };

                await _repository.AddAsync(domain);
                await _repository.UnitOfWork.SaveChangesAsync();

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
    public class FD_BaddebtSettingDeleteHandler : IRequestHandler<FD_BaddebtSettingDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtSettingRepository _repository;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_BaddebtSettingDeleteHandler(IIdentityService identityService, IFD_BaddebtSettingRepository repository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_BaddebtSettingDeleteCommand request, CancellationToken cancellationToken)
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

    public class FD_BaddebtSettingModifyHandler : IRequestHandler<FD_BaddebtSettingModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtSettingRepository _repository;
        public FD_BaddebtSettingModifyHandler(IIdentityService identityService, IFD_BaddebtSettingRepository repository)
        {
            _identityService = identityService;
            _repository = repository;
        }

        public async Task<Result> Handle(FD_BaddebtSettingModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain?.Update(request.DataDate, request.ProvisionMethod, request.BadAccoSubjectOne, request.BadAccoSubjectTwo, request.OtherAccoSubjectOne, request.OtherAccoSubjectTwo, request.DebtReceAccoSubjectOne, request.DebtReceAccoSubjectTwo, request.ReceAccoSubjectOne, request.ReceAccoSubjectTwo, request.ProvisionReceiptAbstractID, request.OccurReceiptAbstractID, request.RecoverReceiptAbstractID, request.ReversalReceiptAbstractID, request.BadReversalReceiptAbstractID, request.EnterpriseID);//,request.GroupNumericalOrder);
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
}
