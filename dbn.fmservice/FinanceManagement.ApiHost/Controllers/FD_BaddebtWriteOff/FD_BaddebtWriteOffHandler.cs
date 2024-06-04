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

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtWriteOff
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_BaddebtWriteOffAddHandler : IRequestHandler<FD_BaddebtWriteOffAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtWriteOffRepository _repository;
        IFD_BaddebtWriteOffDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_BaddebtWriteOffAddHandler(IIdentityService identityService, IFD_BaddebtWriteOffRepository repository, IFD_BaddebtWriteOffDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_BaddebtWriteOffAddCommand request, CancellationToken cancellationToken)
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
                long number = _numberCreator.Create<Domain.FD_BaddebtWriteOff>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == request.EnterpriseID); 
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_BaddebtWriteOff()
                {
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID,
                    TicketedPointID = request.TicketedPointID,
                    Remarks = request.Remarks,
                    Number=number.ToString()
                };
                foreach (var item in request.Lines)
                {
                    var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                    var detial = new FD_BaddebtWriteOffDetail()
                    {
                        NumericalOrder = numericalOrder,
                        BusiType = item.BusiType,
                        BusinessType = item.BusinessType,
                        CurrentUnit = item.CurrentUnit,
                        Amount = item.Amount,
                        WriteOffAmount=item.WriteOffAmount,
                        ModifiedDate = DateTime.Now
                    };
                   
                    domain.AddDetail(detial);
                }
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking();
                await _repository.AddAsync(domain);
                await _biz_ReviewRepository.AddAsync(review);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
               
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
    public class FD_BaddebtWriteOffDeleteHandler : IRequestHandler<FD_BaddebtWriteOffDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtWriteOffRepository _repository;
        IFD_BaddebtWriteOffDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FD_BaddebtWriteOffDeleteHandler(IIdentityService identityService, IFD_BaddebtWriteOffRepository repository, IFD_BaddebtWriteOffDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_BaddebtWriteOffDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = request.NumericalOrder.Split(',').ToList<string>();
                }

                foreach (var num in list)
                {
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == num);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
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

    /// <summary>
    /// 删除
    /// </summary>
    public class FD_BaddebtWriteOffDeleteDetailsHandler : IRequestHandler<FD_BaddebtWriteOffDeleteDetailCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtWriteOffRepository _repository;
        IFD_BaddebtWriteOffDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FD_BaddebtWriteOffDeleteDetailsHandler(IIdentityService identityService, IFD_BaddebtWriteOffRepository repository, IFD_BaddebtWriteOffDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }
        /// <summary>
        /// 列表删除按详情删除-可批量删除
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(FD_BaddebtWriteOffDeleteDetailCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (request == null)
                {
                    result.msg = "参数空";
                    return result;
                }
                var reqList = request.Details;
                if (reqList == null || reqList.Count == 0)
                {
                    result.msg = "参数空";
                    return result;
                }
                foreach (var groupItem in reqList.GroupBy(p => p.NumericalOrder))
                {
                    if (groupItem == null || groupItem.Count() == 0 || string.IsNullOrEmpty(groupItem.Key)) continue;
                    var numericalOrder = groupItem.Key;
                    var domain = await _repository.GetAsync(numericalOrder);
                    if (domain == null || domain.Lines.Count == 0) continue;
                    var detailList = domain.Lines;
                    foreach (var item in groupItem)
                    {
                        detailList?.RemoveAll(p => p.RecordID == item.RecordID);
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                    }
                    if (detailList?.Count == 0)
                    {
                        await _repository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                        await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                    }
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

    public class FD_BaddebtWriteOffModifyHandler : IRequestHandler<FD_BaddebtWriteOffModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_BaddebtWriteOffRepository _repository;
        IFD_BaddebtWriteOffDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_BaddebtWriteOffModifyHandler(IIdentityService identityService, IFD_BaddebtWriteOffRepository repository, IFD_BaddebtWriteOffDetailRepository detailRepository,
            NumericalOrderCreator numericalOrderCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;

        }

        public async Task<Result> Handle(FD_BaddebtWriteOffModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (string.IsNullOrEmpty(_identityService.EnterpriseId) || _identityService.EnterpriseId == "0")
                {
                    result.msg = "登录单位空";
                    result.code = ErrorCode.Update.GetIntValue();
                    return result;
                }

                if (domain == null)
                {
                    result.msg = "未查询到数据";
                    result.code = ErrorCode.NoContent.GetIntValue();
                    return result;
                }
                domain?.Update(request.DataDate, request.TicketedPointID, request.Remarks);

                foreach (var item in request.Lines)
                {
                    if (item.RowStatus=="A" || item.IsCreate)
                    {
                        var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                        var detial = new FD_BaddebtWriteOffDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            BusiType = item.BusiType,
                            BusinessType = item.BusinessType,
                            CurrentUnit = item.CurrentUnit,
                            Amount = item.Amount,
                            WriteOffAmount=item.WriteOffAmount,
                            ModifiedDate = DateTime.Now
                        };
                       
                        _detailRepository.Add(detial);
                        domain.AddDetail(detial);                        
                        continue;
                    }
                    if (item.RowStatus == "D" || item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                    if (item.RowStatus == "M" || item.IsUpdate)
                    {
                        var obj = domain.Lines.Find(o => o.RecordID == item.RecordID);
                        obj?.Update(item.BusiType,  item.BusinessType, item.CurrentUnit, item.Amount,item.WriteOffAmount);                        
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
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存异常";
            }
            return result;
        }
    }
}
