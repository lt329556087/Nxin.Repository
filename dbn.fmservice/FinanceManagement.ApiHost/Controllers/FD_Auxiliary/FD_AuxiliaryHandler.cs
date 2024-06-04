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
using FinanceManagement.Common;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using FinanceManagement.ApiHost.Applications.Queries;
using NPOI.SS.Formula.Functions;

namespace FinanceManagement.ApiHost.Controllers.FD_Auxiliary
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_AuxiliaryAddHandler : IRequestHandler<FD_AuxiliaryAddCommand, Result>
    {
        IBiz_Related _biz_RelatedRepository;
        IIdentityService _identityService;
        Ifd_auxiliaryprojectRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        HostConfiguration _hostCongfiguration;

        public FD_AuxiliaryAddHandler(IIdentityService identityService, Ifd_auxiliaryprojectRepository repository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,IBiz_Related biz_RelatedRepository, HostConfiguration hostCongfiguration)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_RelatedRepository = biz_RelatedRepository;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result> Handle(FD_AuxiliaryAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                //赋值初始化数据
                var data = new fd_auxiliaryproject()
                {
                    CreatedDate = DateTime.Now,
                    GroupId = _identityService.GroupId,
                    Level = request.Level,
                    ModifiedDate = DateTime.Now,
                    OwnerID = _identityService.UserId,
                    Pid = request.Pid,
                    ProjectCode = request.ProjectCode,
                    ProjectId = numericalOrder,
                    ProjectName = request.ProjectName,
                    ProjectType = request.ProjectType,
                    Remarks = request.Remarks,
                    IsUse = request.IsUse
                };
                if (_repository.IsExistCode(data).Count >= 1)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "项目编码重复";
                    result.data = data;
                    return result;
                }
                await _repository.AddAsync(data);
                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = data;
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
                Serilog.Log.Error("FD_AuxiliaryADD:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));

            }

            return result;
        }
    }
    public class FD_AuxiliaryTypeAddHandler : IRequestHandler<FD_AuxiliaryTypeAddCommand, Result>
    {
        IBiz_Related _biz_RelatedRepository;
        IIdentityService _identityService;
        Ifd_auxiliarytypeRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        HostConfiguration _hostCongfiguration;

        public FD_AuxiliaryTypeAddHandler(IIdentityService identityService, Ifd_auxiliarytypeRepository repository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_Related biz_RelatedRepository, HostConfiguration hostCongfiguration)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_RelatedRepository = biz_RelatedRepository;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result> Handle(FD_AuxiliaryTypeAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                //赋值初始化数据
                var data = new fd_auxiliarytype()
                {
                    CreatedDate = DateTime.Now,
                    GroupId = _identityService.GroupId,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = _identityService.UserId,
                    Remarks = request.Remarks,
                    TypeCode = request.TypeCode,
                    TypeName = request.TypeName,
                    TypeTag = request.TypeTag,
                    IsCom = request.IsCom
                };
                await _repository.AddAsync(data);
                if (_repository.IsExistCode(data).Count >= 1)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "类型项目编码重复";
                    result.data = data;
                    return result;
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { AccountID = numericalOrder };
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
                Serilog.Log.Error("FD_AuxiliarTypeADD:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            return result;
        }
    }
    /// <summary>
    /// 删除
    /// </summary>
    public class FD_AuxiliaryDeleteHandler : IRequestHandler<FD_AuxiliaryDeleteCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_auxiliaryprojectRepository _repository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;
        FD_AuxiliaryODataProvider _fD_AuxiliaryODataProvider;
        public FD_AuxiliaryDeleteHandler(IIdentityService identityService, Ifd_auxiliaryprojectRepository repository, IBiz_ReviewRepository biz_ReviewRepository, IBiz_Related biz_RelatedRepository, FD_AuxiliaryODataProvider fD_AuxiliaryODataProvider)
        {
            _identityService = identityService;
            _repository = repository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _fD_AuxiliaryODataProvider = fD_AuxiliaryODataProvider;
        }

        public async Task<Result> Handle(FD_AuxiliaryDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.ProjectId))
                {
                    list = request.ProjectId.Split(',').ToList();
                }

                foreach (var num in list)
                {
                    //暂时取消校验，太影响性能 2023-12-15 17:12:27
                    //暂时恢复 2024-1-2 10:06:35 
                    var data = _fD_AuxiliaryODataProvider.IsExistSettle(new fd_auxiliaryproject() { GroupId = _identityService.GroupId, ProjectId = num });
                    if (data.Count >= 1)
                    {
                        result.data = data;
                        result.msg = "当前数据已被会计凭证使用";
                        result.code = ErrorCode.RequestArgumentError.GetIntValue();
                        return result;
                    }
                    await _repository.RemoveRangeAsync(o => o.GroupId == _identityService.GroupId && o.ProjectId == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { AccountID = request.ProjectId };
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
                Serilog.Log.Error("FD_AuxiliaryDelete:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }

            return result;
        }
    }
    public class FD_AuxiliaryTypeDeleteHandler : IRequestHandler<FD_AuxiliaryTypeDeleteCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_auxiliarytypeRepository _repository;
        public FD_AuxiliaryTypeDeleteHandler(IIdentityService identityService, Ifd_auxiliarytypeRepository repository)
        {
            _identityService = identityService;
            _repository = repository;
        }

        public async Task<Result> Handle(FD_AuxiliaryTypeDeleteCommand request, CancellationToken cancellationToken)
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
                    var data = _repository.IsExistData(new fd_auxiliaryproject() { GroupId = _identityService.GroupId, ProjectType = num });
                    if (data.Count >= 1)
                    {
                        result.data = num;
                        result.code = ErrorCode.RequestArgumentError.GetIntValue();
                        result.msg = @$"当前类型已被 【{data.FirstOrDefault()?.ProjectName}】使用 不可删除 ";
                        return result; ;
                    }
                    await _repository.RemoveRangeAsync(o => o.GroupId == _identityService.GroupId && o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                result.data = new { AccountID = request.NumericalOrder };
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
                Serilog.Log.Error("FD_AuxiliaryDelete:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }

            return result;
        }
    }

    public class FD_AuxiliaryModifyHandler : IRequestHandler<FD_AuxiliaryModifyCommand, Result>
    {
        IBiz_Related _biz_RelatedRepository;
        Applications.Queries.BIZ_RelatedODataProvider _bizRelatedProvider;
        IIdentityService _identityService;
        Ifd_auxiliaryprojectRepository _repository;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FD_AuxiliaryModifyHandler> _logger;
        FD_AuxiliaryODataProvider _fD_AuxiliaryODataProvider;
        public FD_AuxiliaryModifyHandler(IBiz_Related biz_RelatedRepository, BIZ_RelatedODataProvider bizRelatedProvider, IIdentityService identityService, Ifd_auxiliaryprojectRepository repository, HostConfiguration hostCongfiguration, ILogger<FD_AuxiliaryModifyHandler> logger, FD_AuxiliaryODataProvider fD_AuxiliaryODataProvider)
        {
            _biz_RelatedRepository = biz_RelatedRepository;
            _bizRelatedProvider = bizRelatedProvider;
            _identityService = identityService;
            _repository = repository;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _fD_AuxiliaryODataProvider = fD_AuxiliaryODataProvider;
        }

        public async Task<Result> Handle(FD_AuxiliaryModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var data = new fd_auxiliaryproject()
                {
                    CreatedDate = DateTime.Now,
                    GroupId = _identityService.GroupId,
                    Level = request.Level,
                    ModifiedDate = DateTime.Now,
                    OwnerID = _identityService.UserId,
                    Pid = request.Pid,
                    ProjectCode = request.ProjectCode,
                    ProjectId = request.ProjectId,
                    ProjectName = request.ProjectName,
                    ProjectType = request.ProjectType,
                    Remarks = request.Remarks,
                    IsUse = request.IsUse,
                };
                if (_repository.IsExistCode(data).Count >= 1)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "项目编码重复";
                    result.data = data;
                    return result;
                }
                var IsExit = _fD_AuxiliaryODataProvider.IsExistSettle(new fd_auxiliaryproject() { GroupId = _identityService.GroupId, ProjectId = request.ProjectId });
                if (IsExit.Count >= 1)
                {
                    result.data = data;
                    result.msg = "当前数据已被会计凭证使用";
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                _repository.Update(data);
                await _repository.UnitOfWork.SaveChangesAsync();
              
                result.data = data;
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
                Serilog.Log.Information("FD_AuxiliaryModify:errorCodeEx="+ errorCodeEx.ToString()+ ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
                Serilog.Log.Information("FD_AuxiliaryModify:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            return result;
        }
    }
    public class FD_AuxiliaryTypeModifyHandler : IRequestHandler<FD_AuxiliaryTypeModifyCommand, Result>
    {
        IIdentityService _identityService;
        Ifd_auxiliarytypeRepository _repository;
        private readonly ILogger<FD_AuxiliaryModifyHandler> _logger;

        public FD_AuxiliaryTypeModifyHandler(IIdentityService identityService, Ifd_auxiliarytypeRepository repository, ILogger<FD_AuxiliaryModifyHandler> logger)
        {
            _identityService = identityService;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> Handle(FD_AuxiliaryTypeModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var data = new fd_auxiliarytype()
                {
                    CreatedDate = DateTime.Now,
                    GroupId = _identityService.GroupId,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = request.NumericalOrder,
                    OwnerID = _identityService.UserId,
                    Remarks = request.Remarks,
                    TypeCode = request.TypeCode,
                    TypeName = request.TypeName,
                    TypeTag = request.TypeTag,
                    IsCom = request.IsCom
                };
                if (_repository.IsExistCode(data).Count >= 1)
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "类型项目编码重复";
                    result.data = data;
                    return result;
                }
                _repository.Update(data);
                await _repository.UnitOfWork.SaveChangesAsync();

                result.data = new { request.NumericalOrder };
                result.code = ErrorCode.Success.GetIntValue();
                return result;
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
                _logger.LogInformation("FD_AuxiliaryTypeModify:errorCodeEx=" + errorCodeEx.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
                _logger.LogInformation("FD_AuxiliaryTypeModify:ex=" + ex.ToString() + ";request=" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            }
            return result;
        }
    }
}
