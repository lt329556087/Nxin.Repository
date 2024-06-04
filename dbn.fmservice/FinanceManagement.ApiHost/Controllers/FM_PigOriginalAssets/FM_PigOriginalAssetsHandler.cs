using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
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

namespace FinanceManagement.ApiHost.Controllers.FM_PigOriginalAssets
{
    public class FM_PigOriginalAssetsHandler : IRequestHandler<FM_PigOriginalAssetsCommand, Result>
    {
        ILogger<FM_PigOriginalAssetsHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmPigoriginalassetsRepository _fmcostsetRepository;
        IFmPigoriginalassetsdetailRepository _fmcostsetDetailRepository;
        IIdentityService _identityService;
        FM_PigOriginalAssetsODataProvider _queryProvider;
        public FM_PigOriginalAssetsHandler(ILogger<FM_PigOriginalAssetsHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmPigoriginalassetsRepository fmcostsetRepository,
                                                IFmPigoriginalassetsdetailRepository fmcostsetDetailRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_PigOriginalAssetsODataProvider queryProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_PigOriginalAssetsCommand request, CancellationToken cancellationToken)
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
                if (request.NumericalOrder == "0" || string.IsNullOrEmpty(request.NumericalOrder))
                {
                    throw new ErrorCodeExecption(ErrorCode.NoContent, "单据流水号不能为空或0！");
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
                FM_PigOriginalAssetsEntity main = _queryProvider.GetSingleData(request.NumericalOrder);//request.EnterpriseId
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

    public class FM_PigOriginalAssetsSaveHandler : IRequestHandler<FM_PigOriginalAssetsAddCommand, Result>
    {
        ILogger<FM_PigOriginalAssetsSaveHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmPigoriginalassetsRepository _fmcostsetRepository;
        IFmPigoriginalassetsdetailRepository _fmcostsetDetailRepository;
        IFmPigoriginalassetsdetaillistRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FM_PigOriginalAssetsODataProvider _queryProvider;
        private EnterprisePeriodUtil _enterpriseperiodUtil;
        public FM_PigOriginalAssetsSaveHandler(ILogger<FM_PigOriginalAssetsSaveHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmPigoriginalassetsRepository fmcostsetRepository,
                                                IFmPigoriginalassetsdetailRepository fmcostsetDetailRepository,
                                                IFmPigoriginalassetsdetaillistRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                IBiz_ReviewRepository biz_ReviewRepository,
                                                EnterprisePeriodUtil enterpriseperiodUtil,
                                                FM_PigOriginalAssetsODataProvider queryProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _fmcostsetExtendRepository = fmcostsetExtendRepository;
            _identityService = identityService;
            _biz_ReviewRepository = biz_ReviewRepository;
            _queryProvider = queryProvider;
            _enterpriseperiodUtil = enterpriseperiodUtil;
        }

        #region Handle
        public async Task<Result> Handle(FM_PigOriginalAssetsAddCommand request, CancellationToken cancellationToken)
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
                var number = _numberCreator.Create<FmPigoriginalassets>(o => Convert.ToDateTime(o.DataDate), o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseId == _identityService.EnterpriseId);
                request.Number = number.ToString();

                #region 赋值
                FmPigoriginalassets fmCostparamsset = new()
                {
                    NumericalOrder = numericalOrder.ToString(),
                    Number=number.ToString(),
                    EnterpriseId = _identityService.EnterpriseId,
                    DataDate = request.DataDate,
                    PigFarmId = request.PigFarmId,
                    SourceType = request.SourceType,
                    Remarks=request.Remarks,
                    OwnerId=string.IsNullOrEmpty(request.OwnerId)?_identityService.UserId:request.OwnerId,
                    //ModifiedOwnerId = string.IsNullOrEmpty(request.ModifiedOwnerId) ? _identityService.UserId : request.OwnerId,
                    CreatedDate = request.CreatedDate,
                    ModifiedDate = request.ModifiedDate
                };
                //fmCostparamsset=request;
                #endregion

                #region 验证code是否使用
                //var projectList = _queryProvider.GetDatas();
                //var existsObj = projectList.Where(_ => _.CostProjectCode == request.CostProjectCode).FirstOrDefault();
                //if (existsObj != null)
                //{
                //    result.code = 1;
                //    result.msg = "项目编码已被使用，不能重复！";
                //    return result;
                //}
                #endregion

                var details = request.Details?.Select(_ => new FmPigoriginalassetsdetail()
                {
                    NumericalOrder=numericalOrder.ToString(),
                    EarNumber = _.EarNumber,
                    PigType = string.IsNullOrEmpty(_.PigType) ? "0" : _.PigType,
                    OriginalValue = _.OriginalValue.HasValue?_.OriginalValue.Value:0,
                    DepreciationUseMonth = _.DepreciationUseMonth.HasValue?_.DepreciationUseMonth.Value:0,
                    ResidualValueRate = _.ResidualValueRate.HasValue?_.ResidualValueRate.Value:0,
                    ResidualValue = _.ResidualValue.HasValue?_.ResidualValue.Value:0,
                    StartDate=_.StartDate,
                    AccruedMonth=_.AccruedMonth.HasValue?_.AccruedMonth.Value:0,
                    DepreciationAccumulated=_.DepreciationAccumulated.HasValue?_.DepreciationAccumulated.Value:0
                }).ToList();

                #region 会计期间处理
                DateTime dt_data = DateTime.Parse(request.DataDate);
                DateTime dt = new DateTime(dt_data.Year, dt_data.Month, 1);
                var periods = _enterpriseperiodUtil.GetEnterperisePeriodList(_identityService.EnterpriseId, dt.Year, -1);//获取所有会计期间
                if (periods != null && periods.Count > 0)
                {
                    var period = periods.Where(_ => dt_data >= _.StartDate && dt_data <= _.EndDate).FirstOrDefault();
                    if (period != null)
                    {
                        dt = period.StartDate;
                    }
                }
                #endregion

                var detaillists =request.Details?.Select(_ => new FmPigoriginalassetsdetaillist()
                {
                    EarNumber = _.EarNumber,
                    DataDate=dt.ToString("yyyy-MM-dd"),//request.DataDate,
                    PigType = string.IsNullOrEmpty(_.PigType) ? "0" : _.PigType,
                    PigFarmId=request.PigFarmId,
                    EnterpriseId=request.EnterpriseId,
                    OriginalValue = _.OriginalValue.HasValue ? _.OriginalValue.Value : 0,
                    DepreciationUseMonth = _.DepreciationUseMonth.HasValue ? _.DepreciationUseMonth.Value : 0,
                    ResidualValueRate = _.ResidualValueRate.HasValue ? _.ResidualValueRate.Value : 0,
                    ResidualValue = _.ResidualValue.HasValue ? _.ResidualValue.Value : 0,
                    StartDate = _.StartDate,
                    AccruedMonth = _.AccruedMonth.HasValue ? _.AccruedMonth.Value : 0,
                    DepreciationAccumulated = _.DepreciationAccumulated.HasValue ? _.DepreciationAccumulated.Value : 0,
                    DepreciationMonthAmount=0,
                    DepreciationMonthRate=0,
                    NetValue=0
                }).ToList();

                _fmcostsetRepository.Add(fmCostparamsset);
                if (details != null && details.Count > 0)
                {
                    _fmcostsetDetailRepository.AddRange(details);
                    _fmcostsetExtendRepository.AddRange(detaillists);
                }

                //制单信息灌入
                Biz_Review review = new Biz_Review(fmCostparamsset.NumericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);

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

    public class FM_PigOriginalAssetsDelHandler : IRequestHandler<FM_PigOriginalAssetsDeleteCommand, Result>
    {
        ILogger<FM_PigOriginalAssetsDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmPigoriginalassetsRepository _fmcostsetRepository;
        IFmPigoriginalassetsdetailRepository _fmcostsetDetailRepository;
        IFmPigoriginalassetsdetaillistRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        public FM_PigOriginalAssetsDelHandler(ILogger<FM_PigOriginalAssetsDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmPigoriginalassetsRepository fmcostsetRepository,
                                                IFmPigoriginalassetsdetailRepository fmcostsetDetailRepository,
                                                IFmPigoriginalassetsdetaillistRepository fmcostsetExtendRepository,
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
        public async Task<Result> Handle(FM_PigOriginalAssetsDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result() { code = 0 };
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
                var existDetails = _fmcostsetDetailRepository.ODataQuery(_ => _.NumericalOrder == request.NumericalOrder).Select(_ => _.EarNumber).ToList();
                await _fmcostsetExtendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.EarNumber));
                await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.NumericalOrder == request.NumericalOrder);

                //await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();
                //await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();
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

    public class FM_PigOriginalAssetsUpdateHandler : IRequestHandler<FM_PigOriginalAssetsModifyCommand, Result>
    {
        ILogger<FM_PigOriginalAssetsUpdateHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmPigoriginalassetsRepository _fmcostsetRepository;
        IFmPigoriginalassetsdetailRepository _fmcostsetDetailRepository;
        IFmPigoriginalassetsdetaillistRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        FM_PigOriginalAssetsODataProvider _queryProvider;
        private EnterprisePeriodUtil _enterpriseperiodUtil;
        public FM_PigOriginalAssetsUpdateHandler(ILogger<FM_PigOriginalAssetsUpdateHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmPigoriginalassetsRepository fmcostsetRepository,
                                                IFmPigoriginalassetsdetailRepository fmcostsetDetailRepository,
                                                IFmPigoriginalassetsdetaillistRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_PigOriginalAssetsODataProvider queryProvider,
                                                EnterprisePeriodUtil enterpriseperiodUtil)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmcostsetRepository = fmcostsetRepository;
            _fmcostsetDetailRepository = fmcostsetDetailRepository;
            _fmcostsetExtendRepository = fmcostsetExtendRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
            _enterpriseperiodUtil = enterpriseperiodUtil;
        }

        #region Handle
        public async Task<Result> Handle(FM_PigOriginalAssetsModifyCommand request, CancellationToken cancellationToken)
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

                #region 验证code是否使用
                //var projectList = _queryProvider.GetDatas();
                //var existsObj = projectList.Where(_ => _.CostProjectCode == request.CostProjectCode && _.CostProjectId != request.CostProjectId).FirstOrDefault();
                //if (existsObj != null)
                //{
                //    result.code = 1;
                //    result.msg = "项目编码已被使用，不能重复！";
                //    return result;
                //}
                #endregion

                FmPigoriginalassets fmCostparamsset = new()
                {
                    NumericalOrder = request.NumericalOrder,
                    Number = request.Number,
                    EnterpriseId = _identityService.EnterpriseId,
                    DataDate = request.DataDate,
                    PigFarmId = request.PigFarmId,
                    SourceType = request.SourceType,
                    Remarks = request.Remarks,
                    //OwnerId = string.IsNullOrEmpty(request.OwnerId) ? _identityService.UserId : request.OwnerId,
                    ModifiedOwnerId = string.IsNullOrEmpty(request.ModifiedOwnerId) ? _identityService.UserId : request.OwnerId
                };

                _fmcostsetRepository.Update(fmCostparamsset);

                var details = request.Details?.Select(_ => new FmPigoriginalassetsdetail()
                {
                    NumericalOrder = _.NumericalOrder,
                    EarNumber = _.EarNumber,
                    PigType = string.IsNullOrEmpty(_.PigType) ? "0" : _.PigType,
                    OriginalValue = _.OriginalValue.HasValue ? _.OriginalValue.Value : 0,
                    DepreciationUseMonth = _.DepreciationUseMonth.HasValue ? _.DepreciationUseMonth.Value : 0,
                    ResidualValueRate = _.ResidualValueRate.HasValue ? _.ResidualValueRate.Value : 0,
                    ResidualValue = _.ResidualValue.HasValue ? _.ResidualValue.Value : 0,
                    StartDate = _.StartDate,
                    AccruedMonth = _.AccruedMonth.HasValue ? _.AccruedMonth.Value : 0,
                    DepreciationAccumulated = _.DepreciationAccumulated.HasValue ? _.DepreciationAccumulated.Value : 0
                }).ToList();

                #region 会计期间处理
                DateTime dt_data = DateTime.Parse(request.DataDate);
                DateTime dt = new DateTime(dt_data.Year, dt_data.Month, 1);
                var periods = _enterpriseperiodUtil.GetEnterperisePeriodList(_identityService.EnterpriseId, dt.Year, -1);//获取所有会计期间
                if (periods != null && periods.Count > 0)
                {
                    var period = periods.Where(_ => dt_data >= _.StartDate && dt_data <= _.EndDate).FirstOrDefault();
                    if (period != null)
                    {
                        dt = period.StartDate;
                    }
                }
                #endregion

                var detaillists = request.Details?.Select(_ => new FmPigoriginalassetsdetaillist()
                {
                    EarNumber = _.EarNumber,
                    DataDate = dt.ToString("yyyy-MM-dd"),//request.DataDate,
                    PigType = string.IsNullOrEmpty(_.PigType) ? "0" : _.PigType,
                    PigFarmId = request.PigFarmId,
                    EnterpriseId = request.EnterpriseId,
                    OriginalValue = _.OriginalValue.HasValue ? _.OriginalValue.Value : 0,
                    DepreciationUseMonth = _.DepreciationUseMonth.HasValue ? _.DepreciationUseMonth.Value : 0,
                    ResidualValueRate = _.ResidualValueRate.HasValue ? _.ResidualValueRate.Value : 0,
                    ResidualValue = _.ResidualValue.HasValue ? _.ResidualValue.Value : 0,
                    StartDate = _.StartDate,
                    AccruedMonth = _.AccruedMonth.HasValue ? _.AccruedMonth.Value : 0,
                    DepreciationAccumulated = _.DepreciationAccumulated.HasValue ? _.DepreciationAccumulated.Value : 0,
                    DepreciationMonthAmount = 0,
                    DepreciationMonthRate = 0,
                    NetValue = 0
                }).ToList();

                var existDetails = _fmcostsetDetailRepository.ODataQuery(_ => _.NumericalOrder == request.NumericalOrder).Select(_ => _.EarNumber).ToList();
                await _fmcostsetExtendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.EarNumber));
                await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.NumericalOrder == request.NumericalOrder);

                if (details != null && details.Count > 0)
                {
                    await _fmcostsetDetailRepository.AddRangeAsync(details);
                    await _fmcostsetExtendRepository.AddRangeAsync(detaillists);
                }

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

    public class FM_PigOriginalAssetsBatchDelHandler : IRequestHandler<FM_PigOriginalAssetsBatchDelCommand, Result>
    {
        ILogger<FM_PigOriginalAssetsDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmPigoriginalassetsRepository _fmcostsetRepository;
        IFmPigoriginalassetsdetailRepository _fmcostsetDetailRepository;
        IFmPigoriginalassetsdetaillistRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        public FM_PigOriginalAssetsBatchDelHandler(ILogger<FM_PigOriginalAssetsDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmPigoriginalassetsRepository fmcostsetRepository,
                                                IFmPigoriginalassetsdetailRepository fmcostsetDetailRepository,
                                                IFmPigoriginalassetsdetaillistRepository fmcostsetExtendRepository,
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
        public async Task<Result> Handle(FM_PigOriginalAssetsBatchDelCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result() { code = 0 };
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

                foreach (var item in request)
                {
                    await _fmcostsetRepository.DeleteAsync(item);
                    var existDetails = _fmcostsetDetailRepository.ODataQuery(_ => _.NumericalOrder == item).Select(_ => _.EarNumber).ToList();
                    await _fmcostsetExtendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.EarNumber));
                    await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.NumericalOrder == item);
                }

                //await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();
                //await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();
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
}
