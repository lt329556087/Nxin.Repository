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

namespace FinanceManagement.ApiHost.Controllers.FM_CostProject
{
    public class FM_CostProjectHandler : IRequestHandler<FM_CostProjectCommand, Result>
    {
        ILogger<FM_CostProjectHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmCostprojectRepository _fmcostsetRepository;
        IFmCostprojectDetailRepository _fmcostsetDetailRepository;
        IIdentityService _identityService;
        FM_CostProjectODataProvider _queryProvider;
        public FM_CostProjectHandler(ILogger<FM_CostProjectHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmCostprojectRepository fmcostsetRepository,
                                                IFmCostprojectDetailRepository fmcostsetDetailRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_CostProjectODataProvider queryProvider)
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
        public async Task<Result> Handle(FM_CostProjectCommand request, CancellationToken cancellationToken)
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
                FM_CostProjectEntity main = _queryProvider.GetSingleData(request.CostProjectId);//request.EnterpriseId
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

    public class FM_CostProjectSaveHandler : IRequestHandler<FM_CostProjectAddCommand, Result>
    {
        ILogger<FM_CostProjectSaveHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmCostprojectRepository _fmcostsetRepository;
        IFmCostprojectDetailRepository _fmcostsetDetailRepository;
        IFmCostprojectExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        FM_CostProjectODataProvider _queryProvider;
        public FM_CostProjectSaveHandler(ILogger<FM_CostProjectSaveHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmCostprojectRepository fmcostsetRepository,
                                                IFmCostprojectDetailRepository fmcostsetDetailRepository,
                                                IFmCostprojectExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_CostProjectODataProvider queryProvider)
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
        }

        #region Handle
        public async Task<Result> Handle(FM_CostProjectAddCommand request, CancellationToken cancellationToken)
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
                request.CostProjectId = numericalOrder.ToString();
                //[yyyyMMdd+4位数字]，共计12位
                //var number = _numberCreator.Create<PM_UnifiedPlan>(o => o.DataDate, o => o.Number, request.DataDate, o => o.EnterpriseID == _identityService.EnterpriseId && o.GroupID == _identityService.GroupId);
                //request.Number = number.ToString();

                #region 赋值
                FmCostproject fmCostparamsset = new()
                {
                    CostProjectId = numericalOrder.ToString(),
                    EnterpriseId = _identityService.EnterpriseId,
                    DataDate = request.DataDate,
                    CostProjectName = request.CostProjectName,
                    CostProjectTypeId = request.CostProjectTypeId,
                    IsUse = request.IsUse.HasValue ? request.IsUse.Value : 0,
                    OrderNumber = request.OrderNumber.HasValue ? request.OrderNumber.Value : 0,
                    OwnerId = _identityService.UserId,
                    Remarks = request.Remarks,
                    CreatedDate=request.CreatedDate,
                    ModifiedDate=request.ModifiedDate,
                    CostProjectCode=request.CostProjectCode,
                    CollectionType=request.CollectionType,
                    AllocationType=request.AllocationType,
                    DataSource=request.DataSource,
                    ModifiedOwnerID=_identityService.UserId,
                    PresetItem=request.PresetItem
                };
                //fmCostparamsset=request;
                #endregion

                #region 验证code是否使用
                var projectList = _queryProvider.GetDatas();
                var existsObj = projectList.Where(_ => _.CostProjectCode == request.CostProjectCode).FirstOrDefault();
                if (existsObj != null)
                {
                    result.code = 1;
                    result.msg = "项目编码已被使用，不能重复！";
                    return result;
                }
                #endregion

                var details = request.Details?.Select(_ => new FmCostprojectdetail()
                {
                    //RecordID="0",
                    CostProjectId = request.CostProjectId,//_.NumericalOrder,
                    RelatedType = string.IsNullOrEmpty(_.RelatedType) ? "0" : _.RelatedType,
                    RelatedId = _.RelatedId,
                    SubsidiaryAccounting = _.SubsidiaryAccounting,
                    DataFormula = _.DataFormula,
                    ExtendDetails=_.ExtendDetails
                }).ToList();

                _fmcostsetRepository.Add(fmCostparamsset);
                if (details != null && details.Count > 0)
                {
                    _fmcostsetDetailRepository.AddRange(details);
                    await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();

                    bool existsExtends = false;
                    details.ForEach(_ => {
                        var extendlist = _.ExtendDetails?.Select(a => { a.DetailID = _.RecordId; return a; }).ToList();
                        if (extendlist != null && extendlist.Count > 0)
                        {
                            existsExtends = true;
                            _fmcostsetExtendRepository.AddRange(extendlist);
                        }
                    });
                    if (existsExtends)
                    {
                        await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();
                    }
                }
               
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

    public class FM_CostProjectDelHandler : IRequestHandler<FM_CostProjectDeleteCommand, Result>
    {
        ILogger<FM_CostProjectDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmCostprojectRepository _fmcostsetRepository;
        IFmCostprojectDetailRepository _fmcostsetDetailRepository;
        IFmCostprojectExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        FM_CostProjectODataProvider _queryProvider;
        public FM_CostProjectDelHandler(ILogger<FM_CostProjectDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmCostprojectRepository fmcostsetRepository,
                                                IFmCostprojectDetailRepository fmcostsetDetailRepository,
                                                IFmCostprojectExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_CostProjectODataProvider queryProvider)
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
        }

        #region Handle
        public async Task<Result> Handle(FM_CostProjectDeleteCommand request, CancellationToken cancellationToken)
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
                var vdata = _queryProvider.GetDataExistsUse(request.CostProjectId).ToList();
                if (vdata != null)
                {
                    if (vdata.Count > 0)
                    {
                        result.code = ErrorCode.ServerBusy.GetIntValue();
                        vdata.ForEach(_ =>
                        {
                            result.msg += $"当前费用项目【{_.CostProjectName}({_.CostProjectCode})】已被引用，不能删除！";
                        });
                        return result;
                    }
                }
                #endregion

                _fmcostsetRepository.Delete(request.CostProjectId);
                var existDetails = _fmcostsetDetailRepository.ODataQuery(_ => _.CostProjectId == request.CostProjectId).Select(_ => _.RecordId).ToList();
                await _fmcostsetExtendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.DetailID));
                await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.CostProjectId == request.CostProjectId);
                
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

    public class FM_CostProjectUpdateHandler : IRequestHandler<FM_CostProjectModifyCommand, Result>
    {
        ILogger<FM_CostProjectUpdateHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmCostprojectRepository _fmcostsetRepository;
        IFmCostprojectDetailRepository _fmcostsetDetailRepository;
        IFmCostprojectExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        FM_CostProjectODataProvider _queryProvider;
        public FM_CostProjectUpdateHandler(ILogger<FM_CostProjectUpdateHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmCostprojectRepository fmcostsetRepository,
                                                IFmCostprojectDetailRepository fmcostsetDetailRepository,
                                                IFmCostprojectExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_CostProjectODataProvider queryProvider)
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
        }

        #region Handle
        public async Task<Result> Handle(FM_CostProjectModifyCommand request, CancellationToken cancellationToken)
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
                var projectList = _queryProvider.GetDatas();
                var existsObj = projectList.Where(_ => _.CostProjectCode == request.CostProjectCode&&_.CostProjectId!=request.CostProjectId).FirstOrDefault();
                if (existsObj != null)
                {
                    result.code = 1;
                    result.msg = "项目编码已被使用，不能重复！";
                    return result;
                }

                var currentObj = projectList.Where(_ => _.CostProjectId == request.CostProjectId).FirstOrDefault();
                if (currentObj.IsUse != request.IsUse.Value && request.IsUse.Value == 0)
                {//禁用操作
                    var vdata = _queryProvider.GetDataExistsUse(request.CostProjectId).ToList();
                    if (vdata != null)
                    {
                        if (vdata.Count > 0)
                        {
                            result.code = 1;
                            vdata.ForEach(_ =>
                            {
                                result.msg += $"当前费用项目【{_.CostProjectName}({_.CostProjectCode})】已被引用，不能禁用！";
                            });
                            return result;
                        }
                    }
                }
                #endregion

                FmCostproject fmCostparamsset = new()
                {
                    CostProjectId = request.CostProjectId.ToString(),
                    EnterpriseId = _identityService.EnterpriseId,
                    DataDate = request.DataDate,
                    CostProjectName = request.CostProjectName,
                    CostProjectTypeId = request.CostProjectTypeId,
                    IsUse = request.IsUse.HasValue ? request.IsUse.Value : 0,
                    OrderNumber = request.OrderNumber.HasValue ? request.OrderNumber.Value : 0,
                    OwnerId = request.OwnerId,
                    Remarks = request.Remarks,
                    CreatedDate = request.CreatedDate,
                    ModifiedDate = DateTime.Now,
                    CostProjectCode = request.CostProjectCode,
                    CollectionType = request.CollectionType,
                    AllocationType = request.AllocationType,
                    DataSource = request.DataSource,
                    ModifiedOwnerID= _identityService.UserId,
                    PresetItem = request.PresetItem
                };

                _fmcostsetRepository.Update(fmCostparamsset);

                var details = request.Details?.Select(_ => new FmCostprojectdetail()
                {
                    //RecordId=_.RecordId,
                    CostProjectId = request.CostProjectId.ToString(),//_.NumericalOrder,
                    RelatedType = string.IsNullOrEmpty(_.RelatedType) ? "0" : _.RelatedType,
                    RelatedId = _.RelatedId,
                    SubsidiaryAccounting = _.SubsidiaryAccounting,
                    DataFormula = _.DataFormula,
                    ExtendDetails = _.ExtendDetails
                }).ToList();

                var existDetails = _fmcostsetDetailRepository.ODataQuery(_ => _.CostProjectId == request.CostProjectId).Select(_ => _.RecordId).ToList();
                await _fmcostsetExtendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordId));
                await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.CostProjectId == request.CostProjectId);

                if (details != null && details.Count > 0)
                {
                    await _fmcostsetDetailRepository.AddRangeAsync(details);
                }
                await _fmcostsetDetailRepository.UnitOfWork.SaveChangesAsync();

                bool existsExtends = false;
                details?.ForEach(a => {
                    var extendlist = a.ExtendDetails?.Select(_ => { _.DetailID = a.RecordId; return _; }).ToList();
                    if (extendlist != null && extendlist.Count > 0)
                    {
                        existsExtends = true;
                        _fmcostsetExtendRepository.AddRange(extendlist);
                    }
                });
                await _fmcostsetExtendRepository.UnitOfWork.SaveChangesAsync();

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

    public class FM_CostProjectBatchDelHandler : IRequestHandler<FM_CostProjectBatchDelCommand, Result>
    {
        ILogger<FM_CostProjectDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmCostprojectRepository _fmcostsetRepository;
        IFmCostprojectDetailRepository _fmcostsetDetailRepository;
        IFmCostprojectExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        FM_CostProjectODataProvider _queryProvider;
        public FM_CostProjectBatchDelHandler(ILogger<FM_CostProjectDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmCostprojectRepository fmcostsetRepository,
                                                IFmCostprojectDetailRepository fmcostsetDetailRepository,
                                                IFmCostprojectExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_CostProjectODataProvider queryProvider)
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
        }

        #region Handle
        public async Task<Result> Handle(FM_CostProjectBatchDelCommand request, CancellationToken cancellationToken)
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
                string costprojectids = string.Join(',', request);
                var vdata = _queryProvider.GetDataExistsUse(costprojectids).ToList();
                if (vdata != null)
                {
                    if (vdata.Count > 0)
                    {
                        result.code = ErrorCode.ServerBusy.GetIntValue();
                        string errMsg = "";
                        vdata.ForEach(_ =>
                        {
                            errMsg += $"【{_.CostProjectName}({_.CostProjectCode})】,";
                        });

                        result.msg += $"当前费用项目{errMsg.Substring(0,errMsg.Length-1)}已被引用，不能删除！";
                        return result;
                    }
                }
                #endregion

                foreach (var item in request)
                {
                    await _fmcostsetRepository.DeleteAsync(item);
                    var existDetails = _fmcostsetDetailRepository.ODataQuery(_ => _.CostProjectId == item).Select(_ => _.RecordId).ToList();
                    await _fmcostsetExtendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordId));
                    await _fmcostsetDetailRepository.RemoveRangeAsync(_ => _.CostProjectId == item);
                }
                
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

    public class FM_CostProjectBatchAddHandler : IRequestHandler<FM_CostProjectBatchAddCommand, Result>
    {
        ILogger<FM_CostProjectBatchAddHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IFmCostprojectRepository _fmcostsetRepository;
        IFmCostprojectDetailRepository _fmcostsetDetailRepository;
        IFmCostprojectExtendRepository _fmcostsetExtendRepository;
        IIdentityService _identityService;
        FM_CostProjectODataProvider _queryProvider;
        public FM_CostProjectBatchAddHandler(ILogger<FM_CostProjectBatchAddHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                IFmCostprojectRepository fmcostsetRepository,
                                                IFmCostprojectDetailRepository fmcostsetDetailRepository,
                                                IFmCostprojectExtendRepository fmcostsetExtendRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_CostProjectODataProvider queryProvider)
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
        }

        #region Handle
        public async Task<Result> Handle(FM_CostProjectBatchAddCommand request, CancellationToken cancellationToken)
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
                //validate
                #endregion

                if (!string.IsNullOrEmpty(request.EnterpriseId))
                {//建猪场调用时使用，传单位EnterpriseId和制单人OwnerId
                    string[] enterIds = request.EnterpriseId.Split(',');

                    foreach (string enterId in enterIds)
                    {
                        var projects = _queryProvider.GetDatasByEnterPriseID(enterId);
                        var existModels = projects.Where(_ => _.CostProjectCode == "1001" || _.CostProjectCode == "1002" || _.CostProjectCode == "1003" || _.CostProjectCode == "1004");
                        if (!existModels.Any())
                        {
                            var numericalOrders = _numericalOrderCreator.CreateAsync(4).Result.ToArray();
                            List<FmCostproject> models = new List<FmCostproject>() {
                            new FmCostproject() {
                                                            CostProjectId = numericalOrders[0].ToString(),
                                                            EnterpriseId = enterId,
                                                            DataDate = request.DataDate,
                                                            CostProjectName = "直接人工",
                                                            CostProjectTypeId = "201904121023082104",
                                                            IsUse = 1,
                                                            OrderNumber = 0,
                                                            OwnerId = string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            Remarks = "预置项",
                                                            CreatedDate=DateTime.Now,
                                                            ModifiedDate=DateTime.Now,
                                                            CostProjectCode="1001",
                                                            CollectionType="202202111355001104",
                                                            AllocationType="202202111355001202",
                                                            DataSource="202202111355001303",
                                                            ModifiedOwnerID=string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            PresetItem="2022021710002"} ,
                            new FmCostproject() {
                            CostProjectId = numericalOrders[1].ToString(),
                                                            EnterpriseId = enterId,
                                                            DataDate = request.DataDate,
                                                            CostProjectName = "间接人工",
                                                            CostProjectTypeId = "201904121023082104",
                                                            IsUse = 1,
                                                            OrderNumber = 0,
                                                            OwnerId = string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            Remarks = "预置项",
                                                            CreatedDate=DateTime.Now,
                                                            ModifiedDate=DateTime.Now,
                                                            CostProjectCode="1002",
                                                            CollectionType="202202111355001104",
                                                            AllocationType="202202111355001202",
                                                            DataSource="202202111355001303",
                                                            ModifiedOwnerID=string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            PresetItem="2022021710003"
                            },
                            new FmCostproject() {
                            CostProjectId = numericalOrders[2].ToString(),
                                                            EnterpriseId = enterId,
                                                            DataDate = request.DataDate,
                                                            CostProjectName = "固定资产",
                                                            CostProjectTypeId = "201904121023082104",
                                                            IsUse = 1,
                                                            OrderNumber = 0,
                                                            OwnerId = string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            Remarks = "预置项",
                                                            CreatedDate=DateTime.Now,
                                                            ModifiedDate=DateTime.Now,
                                                            CostProjectCode="1003",
                                                            CollectionType="202202111355001104",
                                                            AllocationType="202202111355001202",
                                                            DataSource="202202111355001303",
                                                            ModifiedOwnerID=string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            PresetItem="2022021710004"
                            },
                            new FmCostproject() {
                            CostProjectId = numericalOrders[3].ToString(),
                                                            EnterpriseId = enterId,
                                                            DataDate = request.DataDate,
                                                            CostProjectName = "其他费用",
                                                            CostProjectTypeId = "201904121023082104",
                                                            IsUse = 1,
                                                            OrderNumber = 0,
                                                            OwnerId = string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            Remarks = "预置项",
                                                            CreatedDate=DateTime.Now,
                                                            ModifiedDate=DateTime.Now,
                                                            CostProjectCode="1004",
                                                            CollectionType="202202111355001104",
                                                            AllocationType="202202111355001202",
                                                            DataSource="202202111355001303",
                                                            ModifiedOwnerID=string.IsNullOrEmpty(request.OwnerId)?"-1":request.OwnerId,
                                                            PresetItem="2022021710001"
                            }
                        };

                            await _fmcostsetRepository.AddRangeAsync(models);
                        }
                    }

                    await _fmcostsetRepository.UnitOfWork.SaveChangesAsync();
                }
                else
                {//初始化费用项目数据，只初始化一次
                    var enters = _queryProvider.GetAllPigFarmID();

                    foreach (var enter in enters)
                    {
                        var projects = _queryProvider.GetDatasByEnterPriseID(enter.EnterpriseID);
                        var existModels = projects.Where(_ => _.CostProjectCode == "1001" || _.CostProjectCode == "1002" || _.CostProjectCode == "1003" || _.CostProjectCode == "1004");
                        if (!existModels.Any())
                        {
                            var numericalOrders = _numericalOrderCreator.CreateAsync(4).Result.ToArray();
                            List<FmCostproject> models = new List<FmCostproject>() {
                                    new FmCostproject() {
                                                                    CostProjectId = numericalOrders[0].ToString(),
                                                                    EnterpriseId = enter.EnterpriseID,
                                                                    DataDate = request.DataDate,
                                                                    CostProjectName = "直接人工",
                                                                    CostProjectTypeId = "201904121023082104",
                                                                    IsUse = 1,
                                                                    OrderNumber = 0,
                                                                    OwnerId = string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    Remarks = "预置项",
                                                                    CreatedDate=DateTime.Now,
                                                                    ModifiedDate=DateTime.Now,
                                                                    CostProjectCode="1001",
                                                                    CollectionType="202202111355001104",
                                                                    AllocationType="202202111355001202",
                                                                    DataSource="202202111355001303",
                                                                    ModifiedOwnerID=string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    PresetItem="2022021710002"} ,
                                    new FmCostproject() {
                                    CostProjectId = numericalOrders[1].ToString(),
                                                                    EnterpriseId = enter.EnterpriseID,
                                                                    DataDate = request.DataDate,
                                                                    CostProjectName = "间接人工",
                                                                    CostProjectTypeId = "201904121023082104",
                                                                    IsUse = 1,
                                                                    OrderNumber = 0,
                                                                    OwnerId = string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    Remarks = "预置项",
                                                                    CreatedDate=DateTime.Now,
                                                                    ModifiedDate=DateTime.Now,
                                                                    CostProjectCode="1002",
                                                                    CollectionType="202202111355001104",
                                                                    AllocationType="202202111355001202",
                                                                    DataSource="202202111355001303",
                                                                    ModifiedOwnerID=string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    PresetItem="2022021710003"
                                    },
                                    new FmCostproject() {
                                    CostProjectId = numericalOrders[2].ToString(),
                                                                    EnterpriseId = enter.EnterpriseID,
                                                                    DataDate = request.DataDate,
                                                                    CostProjectName = "固定资产",
                                                                    CostProjectTypeId = "201904121023082104",
                                                                    IsUse = 1,
                                                                    OrderNumber = 0,
                                                                    OwnerId = string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    Remarks = "预置项",
                                                                    CreatedDate=DateTime.Now,
                                                                    ModifiedDate=DateTime.Now,
                                                                    CostProjectCode="1003",
                                                                    CollectionType="202202111355001104",
                                                                    AllocationType="202202111355001202",
                                                                    DataSource="202202111355001303",
                                                                    ModifiedOwnerID=string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    PresetItem="2022021710004"
                                    },
                                    new FmCostproject() {
                                    CostProjectId = numericalOrders[3].ToString(),
                                                                    EnterpriseId = enter.EnterpriseID,
                                                                    DataDate = request.DataDate,
                                                                    CostProjectName = "其他费用",
                                                                    CostProjectTypeId = "201904121023082104",
                                                                    IsUse = 1,
                                                                    OrderNumber = 0,
                                                                    OwnerId = string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    Remarks = "预置项",
                                                                    CreatedDate=DateTime.Now,
                                                                    ModifiedDate=DateTime.Now,
                                                                    CostProjectCode="1004",
                                                                    CollectionType="202202111355001104",
                                                                    AllocationType="202202111355001202",
                                                                    DataSource="202202111355001303",
                                                                    ModifiedOwnerID=string.IsNullOrEmpty(enter.OwnerID)?"-1":enter.OwnerID,
                                                                    PresetItem="2022021710001"
                                    }
                                };

                            await _fmcostsetRepository.AddRangeAsync(models);
                        }
                    }

                    await _fmcostsetRepository.UnitOfWork.SaveChangesAsync();
                }
                
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "批量插入成功！";
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
