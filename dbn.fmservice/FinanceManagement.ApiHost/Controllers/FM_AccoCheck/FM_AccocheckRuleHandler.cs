using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FM_AccoCheck;
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

namespace FinanceManagement.ApiHost.Controllers.FM_Accocheck
{
    public class FM_AccoCheckRuleAddHandler : IRequestHandler<FM_AccoCheckRuleAddCommand, Result>
    {
        IIdentityService _identityService;
        IFM_AccoCheckRuleRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FM_AccoCheckRuleAddHandler(IIdentityService identityService, IFM_AccoCheckRuleRepository repository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_AccoCheckRuleAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                FM_AccoCheckRule domain = new FM_AccoCheckRule(_identityService.EnterpriseId, request.AccoCheckType, request.MasterDataSource, request.MasterFormula, request.MasterSecFormula, request.FollowDataSource, request.FollowFormula, request.FollowSecFormula,
                request.CheckValue, _identityService.UserId, request.IsUse);
                await _repository.AddAsync(domain);
                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { RecordID = domain.RecordID };
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
    public class FM_AccoCheckRuleDeleteHandler : IRequestHandler<FM_AccoCheckRuleDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFM_AccoCheckRuleRepository _repository;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_AccoCheckRuleDeleteHandler(IIdentityService identityService, IBiz_ReviewRepository biz_ReviewRepository, IFM_AccoCheckRuleRepository repository)
        {
            _identityService = identityService;
            _repository = repository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }
        public async Task<Result> Handle(FM_AccoCheckRuleDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request.RecordID==0)
                {
                    result.code = ErrorCode.Delete.GetIntValue();
                    result.msg = "参数有误";
                    return result;
                }
                await _repository.RemoveRangeAsync(o => o.RecordID == request.RecordID);
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

    public class FM_AccoCheckRuleModifyHandler : IRequestHandler<FM_AccoCheckRuleModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_AccoCheckRuleRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        public FM_AccoCheckRuleModifyHandler(IIdentityService identityService, IFM_AccoCheckRuleRepository repository, NumericalOrderCreator numericalOrderCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        public async Task<Result> Handle(FM_AccoCheckRuleModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.RecordID.ToString());
                //true走单条数据保存，false走开关保存
                if (request.IsSave)
                {
                    domain.Update(request.EnterpriseID, request.AccoCheckType, request.MasterDataSource, request.MasterFormula, request.MasterSecFormula, request.FollowDataSource, request.FollowFormula, request.FollowSecFormula,
                                  request.CheckValue, _identityService.UserId, request.IsUse);
                }
                else
                {
                    domain.IsUse = request.IsUse;
                    domain.OwnerID = _identityService.UserId;
                    domain.ModifiedDate = DateTime.Now;
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { RecordID = request.RecordID };
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
