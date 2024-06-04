using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_CostParamsSet
{
    public class FM_CostParamsSetHandler : IRequestHandler<FM_CostParamsSetCommand, Result>
    {
        ILogger<FM_CostParamsSetHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFM_CostParamsSetRepository _fmcostsetRepository;
        IFM_CostParamsSetDetailRepository _fmcostsetDetailRepository;
        IIdentityService _identityService;
        IFM_CostParamsSetExtendRepository _fmcostsetExtendRepository;
        FM_CostParamsSetODataProvider _queryProvider;
        public FM_CostParamsSetHandler(ILogger<FM_CostParamsSetHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFM_CostParamsSetRepository fmcostsetRepository,
                                                IFM_CostParamsSetDetailRepository fmcostsetDetailRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                IFM_CostParamsSetExtendRepository fmcostsetExtendRepository,
                                                FM_CostParamsSetODataProvider queryProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _identityService = identityService;
            _fmcostsetExtendRepository = fmcostsetExtendRepository;
            _queryProvider = queryProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_CostParamsSetCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                ErrorRecordBuilder errorRecordBuilder = ErrorRecordBuilder.Create();
                //验证基础入参
                if (request == null)
                {
                    throw new ErrorCodeExecption(ErrorCode.NoContent);
                }
                if (request.EnterpriseId == "0" || string.IsNullOrEmpty(request.EnterpriseId))
                {
                    throw new ErrorCodeExecption(ErrorCode.NoContent, "单位ID不能为空或0！");
                }
                if (errorRecordBuilder.Any())
                {
                    return errorRecordBuilder.MapToResult(result);
                }

                #region Business Code
                #region Old Code
                //PM_UnifiedPlan main = _pmunifiedPlanRepository.Get(request.NumericalOrder);
                //if (main!=null)
                //{
                //    main.PM_UnifiedPlanDetails = _pmunifiedPlanDetailRepository.GetByPMUnifiedPlanDetails(request.NumericalOrder).ToList();
                //}
                #endregion
                FM_CostParamsSetEntity main = _queryProvider.GetSingleData();//request.EnterpriseId
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.data = main;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                result.code = ErrorCode.ServerBusy.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
        #endregion
    }

    public class FM_CostParamsSetSaveHandler : IRequestHandler<FM_CostParamsSetAddCommand, Result>
    {
        ILogger<FM_CostParamsSetSaveHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFM_CostParamsSetRepository _fmcostsetRepository;
        IFM_CostParamsSetDetailRepository _fmcostsetDetailRepository;
        IFM_CostParamsSetExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        public FM_CostParamsSetSaveHandler(ILogger<FM_CostParamsSetSaveHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFM_CostParamsSetRepository fmcostsetRepository,
                                                IFM_CostParamsSetDetailRepository fmcostsetDetailRepository,
                                                IFM_CostParamsSetExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _fmcostsetExtendRepository = fmcostsetExtendRepository;
            _identityService = identityService;
        }

        #region Handle
        public async Task<Result> Handle(FM_CostParamsSetAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                ErrorRecordBuilder errorRecordBuilder = ErrorRecordBuilder.Create();

                #region Business Code
                //业务验证
                if (request == null)
                {
                    throw new ErrorCodeExecption(ErrorCode.NoContent);
                }

                var numericalOrder = Convert.ToInt64(await _numericalOrderCreator.CreateAsync());
                request.NumericalOrder = numericalOrder.ToString();
                //[yyyyMMdd+4位数字]，共计12位
                //var number = _numberCreator.Create<PM_UnifiedPlan>(o => o.DataDate, o => o.Number, request.DataDate, o => o.EnterpriseID == _identityService.EnterpriseId && o.GroupID == _identityService.GroupId);
                //request.Number = number.ToString();

                #region 赋值
                FmCostparamsset fmCostparamsset = new()
                {
                    NumericalOrder = numericalOrder.ToString(),
                    EnterpriseId = _identityService.EnterpriseId,
                    GenerationMode = request.GenerationMode,
                    TotalDepreciationMonths = request.TotalDepreciationMonths,
                    ResidualValueCalMethod = request.ResidualValueCalMethod,
                    ResidualValueRate= request.ResidualValueRate.HasValue ? request.ResidualValueRate.Value : 0,
                    ResidualValue=request.ResidualValue.HasValue?request.ResidualValue.Value:0,
                    OwnerId = _identityService.UserId,
                    Remarks = request.Remarks
                };
                //fmCostparamsset=request;
                #endregion

                var details = request.Details.Select(_ => new FmCostparamssetdetail()
                {
                    //RecordID="0",
                    NumericalOrder = numericalOrder.ToString(),//_.NumericalOrder,
                    PigFarmId = string.IsNullOrEmpty(_.PigFarmId) ? "0" : _.PigFarmId,
                    BeginPeriod = _.BeginPeriod,
                    BeginDate = _.BeginDate,
                    EnablePeriod = _.EnablePeriod,
                    EnableDate = _.EnableDate,
                    CreatedDate = _.CreatedDate
                }).ToList();

                var extends = request.Extends.Select(_ => new FmCostparamssetextend()
                {
                    //RecordID="0",
                    NumericalOrder = numericalOrder.ToString(),//_.NumericalOrder,
                    ExtendTypeId = string.IsNullOrEmpty(_.ExtendTypeId) ? "0" : _.ExtendTypeId,
                    SourceTypeId = _.SourceTypeId,
                    CreatedTypeId = _.CreatedTypeId,
                    TotalDepreciationMonths = _.TotalDepreciationMonths,
                    CreatedDate = _.CreatedDate
                }).ToList();

                var extends1 = request.DepreciationExtends.Select(_ => new FmCostparamssetextend()
                {
                    //RecordID="0",
                    NumericalOrder = numericalOrder.ToString(),//_.NumericalOrder,
                    ExtendTypeId = string.IsNullOrEmpty(_.ExtendTypeId) ? "0" : _.ExtendTypeId,
                    SourceTypeId = _.SourceTypeId,
                    CreatedTypeId = _.CreatedTypeId,
                    TotalDepreciationMonths = _.TotalDepreciationMonths,
                    CreatedDate = _.CreatedDate
                }).ToList();
                extends.AddRange(extends1);

                _fmcostsetRepository.Add(fmCostparamsset);
                _fmcostsetDetailRepository.AddRange(details);
                _fmcostsetExtendRepository.AddRange(extends);
                await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();
                await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();
                await _fmcostsetRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "保存成功！";
                result.data = new { NumericalOrder = numericalOrder.ToString() };
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                result.code = ErrorCode.ServerBusy.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
        #endregion
    }

    public class FM_CostParamsSetDelHandler : IRequestHandler<FM_CostParamsSetDeleteCommand, Result>
    {
        ILogger<FM_CostParamsSetDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFM_CostParamsSetRepository _fmcostsetRepository;
        IFM_CostParamsSetDetailRepository _fmcostsetDetailRepository;
        IFM_CostParamsSetExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        public FM_CostParamsSetDelHandler(ILogger<FM_CostParamsSetDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFM_CostParamsSetRepository fmcostsetRepository,
                                                IFM_CostParamsSetDetailRepository fmcostsetDetailRepository,
                                                IFM_CostParamsSetExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _fmcostsetExtendRepository = fmcostsetExtendRepository;
            _identityService = identityService;
        }

        #region Handle
        public async Task<Result> Handle(FM_CostParamsSetDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result() { code=0 };
            try
            {
                ErrorRecordBuilder errorRecordBuilder = ErrorRecordBuilder.Create();

                #region Business Code
                //业务验证
                //验证基础入参
                if (request == null)
                {
                    throw new ErrorCodeExecption(ErrorCode.NoContent);
                }
                if (errorRecordBuilder.Any())
                {
                    return errorRecordBuilder.MapToResult(result);
                }

                #region 引用验证
                //var vdata = _queryProvider.ValidationNoticData(request.NumericalOrder);
                //if (vdata != null)
                //{
                //    if (vdata.Count > 0)
                //    {
                //        result.code = ErrorCode.ServerBusy.GetIntValue();
                //        vdata.ForEach(_ => {
                //            result.msg += $"当前单据数据已被{_.PlanTypeName}单:【{_.Number}】引用，不能删除！";
                //        });
                //        return result;
                //    }
                //}
                #endregion

                _fmcostsetRepository.Delete(request.NumericalOrder);
                await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.NumericalOrder == request.NumericalOrder);

                await _fmcostsetExtendRepository.RemoveRangeAsync(_ => _.NumericalOrder == request.NumericalOrder);

                await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();
                await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();
                await _fmcostsetRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                result.code = 1;
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                result.code = ErrorCode.ServerBusy.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
        #endregion
    }

    public class FM_CostParamsSetUpdateHandler : IRequestHandler<FM_CostParamsSetModifyCommand, Result>
    {
        ILogger<FM_CostParamsSetUpdateHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFM_CostParamsSetRepository _fmcostsetRepository;
        IFM_CostParamsSetDetailRepository _fmcostsetDetailRepository;
        IFM_CostParamsSetExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        public FM_CostParamsSetUpdateHandler(ILogger<FM_CostParamsSetUpdateHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFM_CostParamsSetRepository fmcostsetRepository,
                                                IFM_CostParamsSetDetailRepository fmcostsetDetailRepository,
                                                IFM_CostParamsSetExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _fmcostsetExtendRepository = fmcostsetExtendRepository;
            _identityService = identityService;
        }

        #region Handle
        public async Task<Result> Handle(FM_CostParamsSetModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                ErrorRecordBuilder errorRecordBuilder = ErrorRecordBuilder.Create();

                #region Business Code
                //业务验证
                //验证基础入参
                if (request == null)
                {
                    throw new ErrorCodeExecption(ErrorCode.NoContent);
                }
                if (errorRecordBuilder.Any())
                {
                    return errorRecordBuilder.MapToResult(result);
                }

                FmCostparamsset fmCostparamsset = new()
                {
                    NumericalOrder = request.NumericalOrder,
                    EnterpriseId = _identityService.EnterpriseId,
                    GenerationMode = request.GenerationMode,
                    TotalDepreciationMonths = request.TotalDepreciationMonths,
                    ResidualValueCalMethod = request.ResidualValueCalMethod,
                    ResidualValueRate = request.ResidualValueRate.HasValue ? request.ResidualValueRate.Value : 0,
                    ResidualValue = request.ResidualValue.HasValue ? request.ResidualValue.Value : 0,
                    OwnerId = _identityService.UserId,
                    Remarks = request.Remarks
                };
                //fmCostparamsset = request;

                _fmcostsetRepository.Update(fmCostparamsset);

                var details = request.Details.Select(_ => new FmCostparamssetdetail()
                {
                    RecordId=_.RecordId,
                    NumericalOrder = request.NumericalOrder,//_.NumericalOrder,
                    PigFarmId = string.IsNullOrEmpty(_.PigFarmId) ? "0" : _.PigFarmId,
                    BeginPeriod = _.BeginPeriod,
                    BeginDate = _.BeginDate,
                    EnablePeriod = _.EnablePeriod,
                    EnableDate = _.EnableDate,
                    CreatedDate = _.CreatedDate
                }).ToList();
                //删明细
                //foreach (var item in request.PMUnifiedPlanDetails)
                //{
                //    _pmunifiedPlanDetailRepository.Delete(item.RecordID);
                //}

                var extends = request.Extends.Select(_ => new FmCostparamssetextend()
                {
                    RecordId = _.RecordId,
                    NumericalOrder = request.NumericalOrder,//_.NumericalOrder,
                    ExtendTypeId = string.IsNullOrEmpty(_.ExtendTypeId) ? "0" : _.ExtendTypeId,
                    SourceTypeId = _.SourceTypeId,
                    CreatedTypeId = _.CreatedTypeId,
                    TotalDepreciationMonths = _.TotalDepreciationMonths,
                    CreatedDate = _.CreatedDate
                }).ToList();

                var extends1 = request.DepreciationExtends.Select(_ => new FmCostparamssetextend()
                {
                    RecordId = _.RecordId,
                    NumericalOrder = _.NumericalOrder,//_.NumericalOrder,
                    ExtendTypeId = string.IsNullOrEmpty(_.ExtendTypeId) ? "0" : _.ExtendTypeId,
                    SourceTypeId = _.SourceTypeId,
                    CreatedTypeId = _.CreatedTypeId,
                    TotalDepreciationMonths = _.TotalDepreciationMonths,
                    CreatedDate = _.CreatedDate
                }).ToList();
                extends.AddRange(extends1);

                details.ForEach(a => {
                    _fmcostsetDetailRepository.Update(a);
                });
                extends.ForEach(a => {
                    _fmcostsetExtendRepository.Update(a);
                });
                await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();
                await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();
                await _fmcostsetRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "修改成功！";
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                result.code = ErrorCode.ServerBusy.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
        #endregion
    }
}
