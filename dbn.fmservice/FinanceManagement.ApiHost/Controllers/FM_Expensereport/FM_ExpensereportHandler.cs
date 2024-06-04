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

namespace FinanceManagement.ApiHost.Controllers.FM_Expensereport
{
    public class FM_ExpensereportHandler : IRequestHandler<FM_ExpensereportCommand, Result>
    {
        ILogger<FM_ExpensereportHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        public FM_ExpensereportHandler(ILogger<FM_ExpensereportHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportCommand request, CancellationToken cancellationToken)
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
                if (request.EnterpriseID == "0" || string.IsNullOrEmpty(request.EnterpriseID))
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
                FM_ExpensereportEntity main = _queryProvider.GetSingleData(request.NumericalOrder);//request.EnterpriseId
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

    public class FM_ExpensereportSaveHandler : IRequestHandler<FM_ExpensereportAddCommand, Result>
    {
        ILogger<FM_ExpensereportSaveHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        FM_ExpenseCalFactory _calProvider;
        public FM_ExpensereportSaveHandler(ILogger<FM_ExpensereportSaveHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider,
                                                FM_ExpenseCalFactory calProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
            _calProvider = calProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportAddCommand request, CancellationToken cancellationToken)
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
                //request.NumericalOrder = numericalOrder.ToString();
                //[yyyyMMdd+4位数字]，共计12位
                //var number = _numberCreator.Create<PM_UnifiedPlan>(o => o.DataDate, o => o.Number, request.DataDate, o => o.EnterpriseID == _identityService.EnterpriseId && o.GroupID == _identityService.GroupId);
                //request.Number = number.ToString();

                #region 赋值
                Dictionary<string, string> dicEnter = _calProvider.PackageEnterprisePeriod(request.ReportPeriod, _identityService.EnterpriseId);
                fm_expensereport fmCostparamsset = new()
                {
                    NumericalOrder = string.IsNullOrEmpty(request.NumericalOrder)?numericalOrder.ToString(): request.NumericalOrder.ToString(),
                    EnterpriseID = _identityService.EnterpriseId,
                    DataDate = DateTime.Parse(dicEnter["EndDate"]),//request.DataDate,
                    ReportPeriod = request.ReportPeriod,
                    ExpenseAmount = request.ExpenseAmount.HasValue?request.ExpenseAmount.Value:0,
                    Remarks = request.Remarks,
                    OwnerID = string.IsNullOrEmpty(request.OwnerID)?_identityService.UserId:request.OwnerID
                };
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

                var details = request.DetailList?.Select(_ => new fm_expensereportdetail()
                {
                    RecordID= string.IsNullOrEmpty(request.NumericalOrder) ? 0 : _.RecordID,
                    NumericalOrder = fmCostparamsset.NumericalOrder,//_.NumericalOrder,
                    CostProjectID = string.IsNullOrEmpty(_.CostProjectID) ? "0" : _.CostProjectID,
                    CollectionType = _.CollectionType,
                    DataSource = _.DataSource,
                    AllocationType = _.AllocationType,
                    ExpenseAmount = _.ExpenseAmount,
                    CreatedDate=_.CreatedDate,
                    ModifiedDate=_.ModifiedDate,
                    ExtendList=_.ExtendList,
                    ExtendDetailList=_.ExtendDetailList,
                    DetailLogList=_.DetailLogList
                }).ToList();

                if (string.IsNullOrEmpty(request.NumericalOrder))
                {
                    _fmexpensereportRepository.Add(fmCostparamsset);
                }
                
                if (details != null && details.Count > 0)
                {
                    if (string.IsNullOrEmpty(request.NumericalOrder))
                    {//新增
                        _fmexpensereportDetailRepository.AddRange(details);
                        //await _fmexpensereportDetailRepository.AddAsync(details[0]);
                        await _fmexpensereportDetailRepository.UnitOfWork.SaveChangesAsync();
                    }

                    var existsAmount = details.Where(_ => _.DataSource == "202202111355001302").Sum(_ => _.ExpenseAmount);
                    fmCostparamsset.ExpenseAmount -= existsAmount;

                    foreach (var _ in details)
                    {
                        if (_.DataSource == "202202111355001302")
                        { //取数来源=总账系统
                            #region 取数逻辑
                            _calProvider.repoertDetail = _;
                            _calProvider.ReportPeriod = fmCostparamsset.ReportPeriod;
                            List<FM_ExpenseCalEntity> data = _calProvider.GetFDSettlereceiptData();

                            //调用猪联网接口拼接数据
                            //if (_.CollectionType == "202202111355001102")
                            //{ //部门

                            //}

                            //填报明细
                            var extendlist = data?.Select(a => new fm_expensereportextend { DetailID = _.RecordID, PigFarmID = a.PigFarmID, DeptOrOthersID = a.MarketID, ExpenseAmount = a.Amount }).ToList();
                            await _fmexpensereportextendRepository.RemoveRangeAsync(a => a.DetailID == _.RecordID);
                            if (extendlist != null && extendlist.Count > 0)
                            {//部门
                                _fmexpensereportextendRepository.AddRange(extendlist);
                            }

                            var amount = data?.Sum(_ => _.Amount);
                            fmCostparamsset.ExpenseAmount += amount;
                            _.ExpenseAmount = amount.HasValue ? amount.Value : 0;

                            //取数日志
                            if (_.CollectionType == "202202111355001102")
                            {
                                var detaillogs = extendlist?.Where(_ => _.DeptOrOthersID == "0").ToList();
                                await _fmexpensereportdetaillogRepository.RemoveRangeAsync(a => a.DetailID == _.RecordID);
                                if (detaillogs != null && detaillogs.Count > 0)
                                {
                                    var logs = detaillogs.Select(_ => new fm_expensereportdetaillog()
                                    {
                                        DetailID = _.DetailID,
                                        SubsidiaryOption = "-",
                                        OccuredAmount = _.ExpenseAmount,
                                        ErrorCode = "1001",
                                        ErrorMsg = "凭证未填写辅助核算。"
                                    });
                                    await _fmexpensereportdetaillogRepository.AddRangeAsync(logs);
                                }
                            }
                            #endregion
                        }
                    }

                    foreach (var item in details)
                    {
                        await _fmexpensereportDetailRepository.UpdateAsync(item);
                    }
                    await _fmexpensereportRepository.UpdateAsync(fmCostparamsset);

                    #region Old Code
                    //details.ForEach(_ => {
                    //    #region Old Code
                    //    ////取数明细
                    //    //var extendlist = _.ExtendList?.Select(a => { a.DetailID = _.RecordID; return a; }).ToList();
                    //    //if (extendlist != null && extendlist.Count > 0)
                    //    //{
                    //    //    existsExtends = true;
                    //    //    _fmexpensereportextendRepository.AddRange(extendlist);
                    //    //}

                    //    ////归集明细
                    //    //var extenddetaillist = _.ExtendDetailList?.Select(a => { a.DetailID = _.RecordID; return a; }).ToList();
                    //    //if (extenddetaillist != null && extenddetaillist.Count > 0)
                    //    //{
                    //    //    existsExtends = true;
                    //    //    _fmexpensereportextendlistRepository.AddRange(extenddetaillist);
                    //    //}
                    //    #endregion

                    //    #region 取数逻辑
                    //    _calProvider.repoertDetail = _;
                    //    _calProvider.ReportPeriod = fmCostparamsset.ReportPeriod;
                    //    List<FM_ExpenseCalEntity> data =  _calProvider.GetFDSettlereceiptData();

                    //    //调用猪联网接口拼接数据
                    //    //if (_.CollectionType == "202202111355001102")
                    //    //{ //部门

                    //    //}

                    //    //填报明细
                    //    var extendlist = data?.Select(a => new fm_expensereportextend{ DetailID = _.RecordID, PigFarmID=a.PigFarmID, DeptOrOthersID=a.MarketID, ExpenseAmount=a.Amount }).ToList();
                    //    if (extendlist != null && extendlist.Count > 0)
                    //    {//部门
                    //        _fmexpensereportextendRepository.AddRange(extendlist);
                    //    }
                    //    fmCostparamsset.ExpenseAmount += data?.Sum(_=>_.Amount);

                    //    //取数日志
                    //    if (_.CollectionType == "202202111355001102")
                    //    {
                    //        var detaillogs = extendlist?.Where(_ => _.DeptOrOthersID == "0").ToList();
                    //        _fmexpensereportdetaillogRepository.RemoveRangeAsync(a=>a.DetailID==_.RecordID);
                    //        if (detaillogs != null && detaillogs.Count > 0)
                    //        {
                    //            var logs = detaillogs.Select(_ => new fm_expensereportdetaillog() { DetailID = _.DetailID, SubsidiaryOption = "-", OccuredAmount=_.ExpenseAmount, ErrorCode="1001",
                    //                ErrorMsg= "凭证未填写辅助核算。"
                    //            });
                    //            _fmexpensereportdetaillogRepository.AddRangeAsync(logs);
                    //            //_fmexpensereportdetaillogRepository.UnitOfWork.SaveChangesAsync();
                    //        }
                    //    }
                    //    #endregion
                    //});
                    #endregion
                }

                await _fmexpensereportRepository.UnitOfWork.SaveChangesAsync();
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

    public class FM_ExpensereportDelHandler : IRequestHandler<FM_ExpensereportDeleteCommand, Result>
    {
        ILogger<FM_ExpensereportDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        public FM_ExpensereportDelHandler(ILogger<FM_ExpensereportDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportDeleteCommand request, CancellationToken cancellationToken)
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

                _fmexpensereportRepository.Delete(request.NumericalOrder);
                var existDetails = _fmexpensereportDetailRepository.ODataQuery(_ => _.NumericalOrder == request.NumericalOrder).Select(_ => _.RecordID).ToList();
                await _fmexpensereportextendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.DetailID));
                await _fmexpensereportextendlistRepository.RemoveRangeAsync(_ => existDetails.Contains(_.DetailID));
                await _fmexpensereportdetaillogRepository.RemoveRangeAsync(_ => existDetails.Contains(_.DetailID));
                await _fmexpensereportDetailRepository.RemoveRangeAsync(_ => _.NumericalOrder == request.NumericalOrder);

                await _fmexpensereportextendRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportextendlistRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportdetaillogRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportDetailRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportRepository.UnitOfWork.SaveChangesAsync();
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

    public class FM_ExpensereportUpdateHandler : IRequestHandler<FM_ExpensereportModifyCommand, Result>
    {
        ILogger<FM_ExpensereportUpdateHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        FM_ExpenseCalFactory _calProvider;
        public FM_ExpensereportUpdateHandler(ILogger<FM_ExpensereportUpdateHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider,
                                                FM_ExpenseCalFactory calProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
            _calProvider = calProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportModifyCommand request, CancellationToken cancellationToken)
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
                if (string.IsNullOrEmpty(request.NumericalOrder))
                {
                    result.code = 1;
                    result.msg = "请先操作取值，然后再归集！";
                    return result;
                }
                #endregion

                #region Old Code
                //fm_expensereport fmCostparamsset = new()
                //{
                //    NumericalOrder = request.NumericalOrder.ToString(),
                //    EnterpriseID = _identityService.EnterpriseId,
                //    DataDate = request.DataDate,
                //    ReportPeriod = request.ReportPeriod,
                //    ExpenseAmount = request.ExpenseAmount.HasValue ? request.ExpenseAmount.Value : 0,
                //    Remarks = request.Remarks,
                //    OwnerID = request.OwnerID
                //};

                //_fmexpensereportRepository.Update(fmCostparamsset);

                //var details = request.DetailList?.Select(_ => new fm_expensereportdetail()
                //{
                //    RecordID=_.RecordID,
                //    NumericalOrder = _.NumericalOrder,//_.NumericalOrder,
                //    CostProjectID = string.IsNullOrEmpty(_.CostProjectID) ? "0" : _.CostProjectID,
                //    CollectionType = _.CollectionType,
                //    DataSource = _.DataSource,
                //    AllocationType = _.AllocationType,
                //    ExpenseAmount = _.ExpenseAmount,
                //    ExtendList = _.ExtendList,
                //    ExtendDetailList = _.ExtendDetailList,
                //    DetailLogList = _.DetailLogList
                //}).ToList();

                //var existDetails = _fmexpensereportDetailRepository.ODataQuery(_ => _.NumericalOrder == request.NumericalOrder).Select(_ => _.RecordID).ToList();
                //await _fmexpensereportextendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordID));
                //await _fmexpensereportextendlistRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordID));
                //await _fmexpensereportdetaillogRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordID));
                ////await _fmexpensereportDetailRepository.RemoveRangeAsync(_ => _.NumericalOrder == request.NumericalOrder);

                //if (details != null && details.Count > 0)
                //{
                //    details.ForEach(_=> {
                //        _fmexpensereportDetailRepository.Update(_);
                //    });
                //}

                //await _fmexpensereportextendRepository.UnitOfWork.SaveChangesAsync();
                //await _fmexpensereportextendlistRepository.UnitOfWork.SaveChangesAsync();
                //await _fmexpensereportdetaillogRepository.UnitOfWork.SaveChangesAsync();
                //await _fmexpensereportDetailRepository.UnitOfWork.SaveChangesAsync();

                //await _fmexpensereportRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                #region 归集操作
                List<fm_expensereportextendlist> recordList = new List<fm_expensereportextendlist>();
                request.DetailList?.ForEach(_=> {
                    _calProvider.repoertDetail = _;
                    _calProvider.ReportPeriod = request.ReportPeriod;
                    _.IsCollect = 1;

                    if (_.ExtendList != null && _.ExtendList.Count > 0 && _.DataSource== "202202111355001302")
                    {
                        //获取猪联网数据，计算
                        List<FM_PigGroupDataEntity> groupData = _calProvider.GetPigGroupData(_.CollectionType,_.AllocationType);

                        if (_.CollectionType == "202202111355001102")//部门
                        {
                            //business code
                            //_.ExtendList.Join(groupData, extend => extend.PigFarmID, detail => detail.PigFarmID, (extend, detail) => new fm_expensereportextend(){ DeptOrOthersID = extend.DeptOrOthersID });
                            //var data = from a in _.ExtendList
                            //           join b in groupData on a.PigFarmID equals b.PigFarmID into temp
                            //           select new fm_expensereportextendlist(){ DetailID=a.DetailID,DeptOrOthersID=a.DeptOrOthersID,PigFarmID=a.PigFarmID };
                            var depts = _.ExtendList.Select(a => a.DeptOrOthersID).Distinct().ToList();
                            depts.ForEach(e=> {
                                var extendList = _.ExtendList.Where(a => a.DeptOrOthersID == e).ToList();
                                if (extendList.Count == 1)
                                {//部门，猪场1对1
                                    var extendObj = extendList.FirstOrDefault();
                                    if (extendObj.PigFarmID == "0")
                                    { //没有猪场，则分摊到所有猪场下
                                        var totalRate = groupData.Sum(a => a.RateDays);
                                        var totalAmount = extendObj.ExpenseAmount;
                                        groupData.ForEach(a =>
                                        {
                                            a.ExpenseAmount = totalRate == 0 ? 0 : Math.Round((a.RateDays / totalRate) * totalAmount,2,MidpointRounding.AwayFromZero);
                                        });

                                        #region 差异对比调整
                                        var expenseAmount = Math.Round(groupData.Sum(_ => _.ExpenseAmount), 2, MidpointRounding.AwayFromZero);
                                        var diff = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero) - expenseAmount;
                                        if (diff != 0)
                                        {
                                            var adjustModel = groupData.OrderByDescending(_ => _.ExpenseAmount).FirstOrDefault();
                                            if (adjustModel != null)
                                            {
                                                adjustModel.ExpenseAmount += diff;
                                            }
                                        }
                                        #endregion

                                        var extenddetailList = groupData.Select(a => new fm_expensereportextendlist()
                                        {
                                            DetailID = extendObj.DetailID,
                                            PigFarmID = a.PigFarmID,
                                            DeptOrOthersID = extendObj.DeptOrOthersID,
                                            ExpenseValue = a.RateDays,
                                            ExpenseAmount = Math.Round(a.ExpenseAmount,2,MidpointRounding.AwayFromZero)
                                        }).ToList();

                                        recordList.AddRange(extenddetailList);
                                    }
                                    else
                                    { //有猪场信息，则按对应的猪场信息分摊(1对1分摊)
                                        var groupData1 = groupData.Where(a => a.PigFarmID == extendObj.PigFarmID).ToList();
                                        if (groupData1 != null)
                                        {//有对应的猪场分摊系数
                                            var extenddetailList = groupData1.Select(a => new fm_expensereportextendlist()
                                            {
                                                DetailID = extendObj.DetailID,
                                                PigFarmID = a.PigFarmID,
                                                DeptOrOthersID = extendObj.DeptOrOthersID,
                                                ExpenseValue = a.RateDays,
                                                ExpenseAmount = extendObj.ExpenseAmount
                                            }).ToList();

                                            recordList.AddRange(extenddetailList);
                                        }
                                    }
                                }
                                else
                                { //非1对1
                                    var extendObj = extendList.FirstOrDefault();
                                    var groupData1 = groupData.Where(a => extendList.Any(b=>b.PigFarmID==a.PigFarmID)).ToList();
                                    if (groupData1 != null && groupData1.Count > 1)
                                    {//有对应的猪场分摊系数
                                        var totalRate = groupData1.Sum(a => a.RateDays);
                                        var totalAmount = extendObj.ExpenseAmount;
                                        groupData1.ForEach(a =>
                                        {
                                            a.ExpenseAmount = totalRate == 0 ? 0 : Math.Round((a.RateDays / totalRate) * totalAmount,2,MidpointRounding.AwayFromZero);
                                        });

                                        #region 差异对比调整
                                        var expenseAmount = Math.Round(groupData1.Sum(_ => _.ExpenseAmount), 2, MidpointRounding.AwayFromZero);
                                        var diff = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero) - expenseAmount;
                                        if (diff != 0)
                                        {
                                            var adjustModel = groupData1.OrderByDescending(_ => _.ExpenseAmount).FirstOrDefault();
                                            if (adjustModel != null)
                                            {
                                                adjustModel.ExpenseAmount += diff;
                                            }
                                        }
                                        #endregion

                                        var extenddetailList = groupData1.Select(a => new fm_expensereportextendlist()
                                        {
                                            DetailID = extendObj.DetailID,
                                            PigFarmID = a.PigFarmID,
                                            DeptOrOthersID = extendObj.DeptOrOthersID,
                                            ExpenseValue = a.RateDays,
                                            ExpenseAmount = a.ExpenseAmount
                                        }).ToList();

                                        recordList.AddRange(extenddetailList);
                                    }
                                }
                            });
                        }
                        else {//ExtendList只一条数据
                            var totalRate = groupData.Sum(a=>a.RateDays);
                            var totalAmount = _.ExtendList.Sum(b => b.ExpenseAmount);
                            groupData.ForEach(a=> {
                                a.ExpenseAmount = totalRate == 0 ? 0 : Math.Round((a.RateDays / totalRate) * totalAmount,2,MidpointRounding.AwayFromZero);
                            });

                            #region 差异对比调整
                            var expenseAmount = Math.Round(groupData.Sum(_=>_.ExpenseAmount),2,MidpointRounding.AwayFromZero);
                            var diff = Math.Round(totalAmount,2,MidpointRounding.AwayFromZero) - expenseAmount;
                            if (diff != 0)
                            {
                                var adjustModel = groupData.OrderByDescending(_ => _.ExpenseAmount).FirstOrDefault();
                                if (adjustModel != null)
                                {
                                    adjustModel.ExpenseAmount += diff;
                                }
                            }
                            #endregion

                            var extenddetailList = groupData.Select(a=>new fm_expensereportextendlist() { DetailID=_.RecordID, PigFarmID=a.PigFarmID, DeptOrOthersID=a.GroupID,
                                ExpenseValue=a.RateDays,
                                ExpenseAmount=a.ExpenseAmount
                            }).ToList();

                            recordList.AddRange(extenddetailList);
                        }
                    }
                });

                if (recordList != null && recordList.Count > 0)
                {
                    //await _fmexpensereportextendlistRepository.RemoveRangeAsync(a => request.DetailList.Any(_=>_.RecordID==a.DetailID));
                    var detailIds = request.DetailList.Where(_=>_.DataSource== "202202111355001302").Select(_ => _.RecordID).ToList();
                    await _fmexpensereportextendlistRepository.RemoveRangeAsync(a => detailIds.Contains(a.DetailID));
                    await _fmexpensereportextendlistRepository.AddRangeAsync(recordList);
                    //await _fmexpensereportextendlistRepository.UnitOfWork.SaveChangesAsync();
                }
                foreach (var item in request.DetailList)
                {
                    await _fmexpensereportDetailRepository.UpdateAsync(item);
                }
                await _fmexpensereportDetailRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "归集成功！";
                #endregion
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

    public class FM_ExpensereportBatchDelHandler : IRequestHandler<FM_ExpensereportBatchDelCommand, Result>
    {
        ILogger<FM_ExpensereportDelHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        public FM_ExpensereportBatchDelHandler(ILogger<FM_ExpensereportDelHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportBatchDelCommand request, CancellationToken cancellationToken)
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
                    await _fmexpensereportRepository.DeleteAsync(item);
                    var existDetails = _fmexpensereportDetailRepository.ODataQuery(_ => _.NumericalOrder == item).Select(_ => _.RecordID).ToList();
                    await _fmexpensereportextendRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordID));
                    await _fmexpensereportextendlistRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordID));
                    await _fmexpensereportdetaillogRepository.RemoveRangeAsync(_ => existDetails.Contains(_.RecordID));
                    await _fmexpensereportDetailRepository.RemoveRangeAsync(_=>_.NumericalOrder==item);
                }

                await _fmexpensereportextendRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportextendlistRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportdetaillogRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportDetailRepository.UnitOfWork.SaveChangesAsync();
                await _fmexpensereportRepository.UnitOfWork.SaveChangesAsync();
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

    public class FM_ExpensereportSummaryHandler : IRequestHandler<FM_ExpensereportSummaryCommand, Result>
    {
        ILogger<FM_ExpensereportSummaryHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        public FM_ExpensereportSummaryHandler(ILogger<FM_ExpensereportSummaryHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportSummaryCommand request, CancellationToken cancellationToken)
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
                    throw new ErrorCodeExecption(ErrorCode.NoContent, "流水号不能为空或0！");
                }
                if (errorRecordBuilder.Any())
                {
                    return errorRecordBuilder.MapToResult(result);
                }

                #region Business Code
                List<FM_ExpenseSummaryReportEntity> main = _queryProvider.GetExpenseSummaryReport(request.NumericalOrder);
                List<FM_ExpenseSummaryReportEntity> resultReport = new List<FM_ExpenseSummaryReportEntity>();
                List<string> pigfarms = main.Select(_ => _.PigFarmName).Distinct().ToList();
                var groupData = main.GroupBy(_ => new {_.CostProjectID, _.CostProjectCode, _.CollectionTypeName, _.CostProjectName });
                foreach (var item in groupData)
                {
                    FM_ExpenseSummaryReportEntity entity = new FM_ExpenseSummaryReportEntity() {CostProjectID=item.Key.CostProjectID,
                    CostProjectCode=item.Key.CostProjectCode,
                    CollectionTypeName=item.Key.CollectionTypeName,
                    CostProjectName=item.Key.CostProjectName,
                    TotalAmount=item.Sum(_=>_.ExpenseAmount),
                    PigFarmNames=pigfarms,
                    ExpenseAmounts=new List<decimal>(new decimal[pigfarms.Count])};

                    foreach (var obj in item)
                    {
                        int index = pigfarms.FindIndex(_=>_==obj.PigFarmName);
                        entity.ExpenseAmounts[index] += obj.ExpenseAmount;
                    }

                    resultReport.Add(entity);
                }
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.data = resultReport;
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

    public class FM_ExpensereportLogsHandler : IRequestHandler<FM_ExpensereportLogsCommand, Result>
    {
        ILogger<FM_ExpensereportLogsHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        public FM_ExpensereportLogsHandler(ILogger<FM_ExpensereportLogsHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportLogsCommand request, CancellationToken cancellationToken)
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
                    throw new ErrorCodeExecption(ErrorCode.NoContent, "流水号不能为空或0！");
                }
                if (errorRecordBuilder.Any())
                {
                    return errorRecordBuilder.MapToResult(result);
                }

                #region Business Code
                List<FM_ExpenseReportLogsEntity> main = _queryProvider.GetExpenseReportLogs(request.NumericalOrder);
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

    /// <summary>
    /// 手工录入-归集明细
    /// </summary>
    public class FM_ExpensereportFillHandler : IRequestHandler<FM_ExpensereportFillCommand, Result>
    {
        ILogger<FM_ExpensereportFillHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        FM_ExpenseCalFactory _calProvider;
        public FM_ExpensereportFillHandler(ILogger<FM_ExpensereportFillHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider,
                                                FM_ExpenseCalFactory calProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
            _calProvider = calProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportFillCommand request, CancellationToken cancellationToken)
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
                //if (string.IsNullOrEmpty(request.NumericalOrder))
                //{
                //    result.code = 1;
                //    result.msg = "请先操作取值，然后再归集！";
                //    return result;
                //}
                #endregion

                #region Business Code
                var detail = new fm_expensereportdetail()
                {
                    RecordID = request.RecordID,
                    NumericalOrder = request.NumericalOrder,//_.NumericalOrder,
                    CostProjectID = request.CostProjectID,
                    CollectionType = request.CollectionType,
                    DataSource = request.DataSource,
                    AllocationType = request.AllocationType,
                    ExpenseAmount = request.ExpenseAmount,
                    CreatedDate = request.CreatedDate,
                    ModifiedDate = request.ModifiedDate,
                    ExtendList = request.ExtendList,
                    ExtendDetailList = request.ExtendDetailList,
                    DetailLogList = request.DetailLogList
                };

                var details = request.ExtendDetailList?.Select(_ => new fm_expensereportextendlist()
                {
                    RecordID = _.RecordID,
                    DetailID = _.DetailID>0?_.DetailID:request.RecordID,//_.NumericalOrder,
                    PigFarmID = string.IsNullOrEmpty(_.PigFarmID) ? "0" : _.PigFarmID,
                    DeptOrOthersID = _.DeptOrOthersID,
                    ExpenseValue = 0,
                    ExpenseAmount = _.ExpenseAmount,
                    Remarks = _.Remarks
                }).ToList();

                decimal newAmount = details.Sum(_ => _.ExpenseAmount);
                decimal diffAmount = detail.ExpenseAmount - newAmount;

                detail.ExpenseAmount = newAmount;
                await _fmexpensereportDetailRepository.UpdateAsync(detail);

                var main = _fmexpensereportRepository.Get(detail.NumericalOrder);
                main.ExpenseAmount -= diffAmount;
                await _fmexpensereportRepository.UpdateAsync(main);

                await _fmexpensereportextendlistRepository.RemoveRangeAsync(_ => _.DetailID==request.RecordID);
                await _fmexpensereportextendlistRepository.AddRangeAsync(details);

                await _fmexpensereportextendlistRepository.UnitOfWork.SaveChangesAsync();
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "手动归集成功！";
                #endregion
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

    /// <summary>
    /// 问题追溯
    /// </summary>
    public class FM_ExpensereportDetailLogsHandler : IRequestHandler<FM_ExpensereportDetailLogsCommand, Result>
    {
        ILogger<FM_ExpensereportDetailLogsHandler> _logger;
        Nxin_Qlw_BusinessContext _nxin_Qlw_BusContext;

        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        Ifm_expensereportRepository _fmexpensereportRepository;
        Ifm_expensereportdetailRepository _fmexpensereportDetailRepository;
        Ifm_expensereportdetaillogRepository _fmexpensereportdetaillogRepository;
        Ifm_expensereportextendRepository _fmexpensereportextendRepository;
        Ifm_expensereportextendlistRepository _fmexpensereportextendlistRepository;
        IIdentityService _identityService;
        FM_ExpensereportODataProvider _queryProvider;
        FM_ExpenseCalFactory _calProvider;
        public FM_ExpensereportDetailLogsHandler(ILogger<FM_ExpensereportDetailLogsHandler> logger,
                                                Nxin_Qlw_BusinessContext Nxin_Qlw_BusinessContext,
                                                NumericalOrderCreator numericalOrderCreator,
                                                Ifm_expensereportRepository fmexpensereportRepository,
                                                Ifm_expensereportdetailRepository fmexpensereportDetailRepository,
                                                Ifm_expensereportdetaillogRepository fmexpensereportdetaillogRepository,
                                                Ifm_expensereportextendRepository fmexpensereportextendRepository,
                                                Ifm_expensereportextendlistRepository fmexpensereportextendlistRepository,
                                                NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                                IIdentityService identityService,
                                                FM_ExpensereportODataProvider queryProvider,
                                                FM_ExpenseCalFactory calProvider)
        {
            _logger = logger;
            _nxin_Qlw_BusContext = Nxin_Qlw_BusinessContext;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _fmexpensereportRepository = fmexpensereportRepository;
            _fmexpensereportDetailRepository = fmexpensereportDetailRepository;
            _fmexpensereportdetaillogRepository = fmexpensereportdetaillogRepository;
            _fmexpensereportextendRepository = fmexpensereportextendRepository;
            _fmexpensereportextendlistRepository = fmexpensereportextendlistRepository;
            _identityService = identityService;
            _queryProvider = queryProvider;
            _calProvider = calProvider;
        }

        #region Handle
        public async Task<Result> Handle(FM_ExpensereportDetailLogsCommand request, CancellationToken cancellationToken)
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
                    throw new ErrorCodeExecption(ErrorCode.NoContent, "流水号不能为空或0！");
                }
                if (errorRecordBuilder.Any())
                {
                    return errorRecordBuilder.MapToResult(result);
                }

                #region Business Code
                fm_expensereportdetail detail = new fm_expensereportdetail() {
                    RecordID = request.RecordID,
                    NumericalOrder = request.NumericalOrder,//_.NumericalOrder,
                    CostProjectID = string.IsNullOrEmpty(request.CostProjectID) ? "0" : request.CostProjectID,
                    CollectionType = request.CollectionType,
                    //DataSource = request.DataSource,
                    AllocationType = request.AllocationType,
                    ExpenseAmount =request.OccuredAmount
                };
                _calProvider.repoertDetail = detail;
                _calProvider.ReportPeriod = request.ReportPeriod;
                List<FM_ExpenseReportDetailLogsEntity> data = _calProvider.GetFDSettlereceiptErrorData();
                #endregion

                result.code = ErrorCode.Success.GetIntValue();
                result.data = data;
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
