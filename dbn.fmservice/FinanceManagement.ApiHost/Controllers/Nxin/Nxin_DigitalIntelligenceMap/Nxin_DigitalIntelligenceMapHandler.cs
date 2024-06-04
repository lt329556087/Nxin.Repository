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

namespace FinanceManagement.ApiHost.Controllers.Nxin_DigitalIntelligenceMap
{
    public class Nxin_DigitalIntelligenceMapAddHandler : IRequestHandler<Nxin_DigitalIntelligenceMapAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        INxin_DigitalIntelligenceMapRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public Nxin_DigitalIntelligenceMapAddHandler(IIdentityService identityService,
                                            INxin_DigitalIntelligenceMapRepository repository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }
        public async Task<Result> Handle(Nxin_DigitalIntelligenceMapAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request==null)
                {
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "参数不能为空！";
                }
                if (string.IsNullOrEmpty(request.BlockList))
                {
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "配置样式不能为空！";
                }
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.Nxin_DigitalIntelligenceMap()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    GroupID = _identityService.GroupId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    MapType = request.MapType,
                    BackgroundValue = request.BackgroundValue,
                    Remarks = request.Remarks,
                    BlockList = request.BlockList,
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
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
    }

    public class Nxin_DigitalIntelligenceMapDeleteHandler : IRequestHandler<Nxin_DigitalIntelligenceMapDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        INxin_DigitalIntelligenceMapRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public Nxin_DigitalIntelligenceMapDeleteHandler(IIdentityService identityService,
                                            INxin_DigitalIntelligenceMapRepository repository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(Nxin_DigitalIntelligenceMapDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null)
                {
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "参数不能为空！";
                }
                if (string.IsNullOrEmpty(request.NumericalOrder))
                {
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "流水号不能为空！";
                }
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    var nums = request.NumericalOrder.Split(',').ToList();
                    List<string> newNums = nums.Where((x, i) => nums.FindIndex(z => z == x) == i).ToList();
                    foreach (var item in newNums)
                    {
                        await _repository.RemoveRangeAsync(o => o.NumericalOrder == item);
                    }
                    await _repository.UnitOfWork.SaveChangesAsync();
                    result.code = ErrorCode.Success.GetIntValue();
                    result.msg = "删除成功!";
                }
                else
                {
                    result.code = ErrorCode.Delete.GetIntValue();
                    result.msg = "流水号不能为空!";
                }
            }
            catch (ErrorCodeExecption errorCodeEx)
            {
                errorCodeEx.MapToResult(result).AsAggregate();
            }
            catch (Exception ex)
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "保存失败,请联系管理员！";
            }

            return result;
        }
    }

    public class Nxin_DigitalIntelligenceMapModifyHandler : IRequestHandler<Nxin_DigitalIntelligenceMapModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        INxin_DigitalIntelligenceMapRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public Nxin_DigitalIntelligenceMapModifyHandler(IIdentityService identityService,
                                            INxin_DigitalIntelligenceMapRepository repository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(Nxin_DigitalIntelligenceMapModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null)
                {
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "参数不能为空！";
                }
                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.MapType, request.BackgroundValue, request.BlockList, request.Remarks);
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
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }
    }

}
