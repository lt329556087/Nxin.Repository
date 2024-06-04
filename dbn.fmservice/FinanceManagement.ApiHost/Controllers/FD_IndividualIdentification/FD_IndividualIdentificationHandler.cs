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

namespace FinanceManagement.ApiHost.Controllers.FD_IndividualIdentification
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_IndividualIdentificationAddHandler : IRequestHandler<FD_IndividualIdentificationAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_IndividualIdentificationRepository _repository;
        IFD_IndividualIdentificationDetailRepository _detailRepository;
        IFD_IndividualIdentificationExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_IndividualIdentificationAddHandler(IIdentityService identityService, IFD_IndividualIdentificationRepository repository, IFD_IndividualIdentificationDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IFD_IndividualIdentificationExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extRepository = extRepository;
        }

        public async Task<Result> Handle(FD_IndividualIdentificationAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {                
                //if (string.IsNullOrEmpty(_identityService.EnterpriseId) || _identityService.EnterpriseId == "0")
                //{
                //    result.msg = "登录单位空";
                //    result.code = ErrorCode.Create.GetIntValue();
                //    return result;
                //}
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                long number = _numberCreator.Create<Domain.FD_IndividualIdentification>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == request.EnterpriseID);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_IndividualIdentification()
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
                var extList = new List<FD_IndividualIdentificationExt>();
                foreach (var item in request.Lines)
                {
                    var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                    var detial = new FD_IndividualIdentificationDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = detailNumericalOrder,
                        BusiType = item.BusiType,
                        IdentificationType = item.IdentificationType,
                        BusinessType = item.BusinessType,
                        CurrentUnit = item.CurrentUnit,
                        Amount = item.Amount,
                        AccrualAmount=item.AccrualAmount,
                        DataSourceType = item.DataSourceType,
                        ModifiedDate = DateTime.Now
                    };
                    item.AgingList?.ForEach(o =>
                    {
                        var ext = new FD_IndividualIdentificationExt()
                        {
                            NumericalOrderDetail = detailNumericalOrder,
                            NumericalOrder=numericalOrder,
                            BusiType = o.BusiType,
                            Name = o.Name,
                            Amount = o.Amount,
                            ModifiedDate = DateTime.Now
                        };
                        extList.Add(ext);
                    });
                    domain.AddDetail(detial);
                }
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking();
                await _repository.AddAsync(domain);
                await _extRepository.AddRangeAsync(extList);
                await _biz_ReviewRepository.AddAsync(review);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                await _extRepository.UnitOfWork.SaveChangesAsync();
               
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
    public class FD_IndividualIdentificationDeleteHandler : IRequestHandler<FD_IndividualIdentificationDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_IndividualIdentificationRepository _repository;
        IFD_IndividualIdentificationDetailRepository _detailRepository;
        IFD_IndividualIdentificationExtRepository _extRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FD_IndividualIdentificationDeleteHandler(IIdentityService identityService, IFD_IndividualIdentificationRepository repository, IFD_IndividualIdentificationDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository, IFD_IndividualIdentificationExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extRepository = extRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_IndividualIdentificationDeleteCommand request, CancellationToken cancellationToken)
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
                    await _extRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
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

    public class FD_IndividualIdentificationModifyHandler : IRequestHandler<FD_IndividualIdentificationModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_IndividualIdentificationRepository _repository;
        IFD_IndividualIdentificationDetailRepository _detailRepository;
        IFD_IndividualIdentificationExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_IndividualIdentificationModifyHandler(IIdentityService identityService, IFD_IndividualIdentificationRepository repository, IFD_IndividualIdentificationDetailRepository detailRepository,
            NumericalOrderCreator numericalOrderCreator,
            IFD_IndividualIdentificationExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _extRepository = extRepository;

        }

        public async Task<Result> Handle(FD_IndividualIdentificationModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (domain == null) { result.code = ErrorCode.Update.GetIntValue(); result.msg = "NumericalOrder查询空"; return result; }
                domain?.Update(request.DataDate, request.TicketedPointID, request.Remarks);

                foreach (var item in request.Lines)
                {
                    if (item.RowStatus=="A" || item.IsCreate)
                    {
                        var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                        var detial = new FD_IndividualIdentificationDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            NumericalOrderDetail = detailNumericalOrder,
                            BusiType = item.BusiType,
                            IdentificationType = item.IdentificationType,
                            BusinessType = item.BusinessType,
                            CurrentUnit = item.CurrentUnit,
                            Amount = item.Amount,
                            AccrualAmount=item.AccrualAmount,
                            DataSourceType = item.DataSourceType,
                            ModifiedDate = DateTime.Now
                        };
                        item.AgingList?.ForEach(o =>
                        {
                            var ext = new FD_IndividualIdentificationExt()
                            {
                                NumericalOrderDetail = detailNumericalOrder,
                                NumericalOrder = request.NumericalOrder,
                                BusiType = o.BusiType,
                                Name = o.Name,
                                Amount = o.Amount,
                                ModifiedDate = DateTime.Now
                            };
                            detial.AgingList.Add(ext);
                        });
                        _extRepository.AddRange(detial.AgingList);
                        _detailRepository.Add(detial);
                        domain.AddDetail(detial);                        
                        continue;
                    }
                    if (item.RowStatus == "D" || item.IsDelete)
                    {
                        await _extRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);                        
                        continue;
                    }
                    if (item.RowStatus == "M" || item.IsUpdate)
                    {
                        var obj = domain.Lines.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj?.Update(item.BusiType, item.IdentificationType, item.BusinessType, item.CurrentUnit, item.Amount,item.DataSourceType,item.AccrualAmount);
                        //var detailData = await _extRepository.GetExtAsync(item.NumericalOrder, item.NumericalOrderDetail, item.BusiType);
                        //if (detailData?.Count == 0)
                        //{
                        //    var list = new List<FD_IndividualIdentificationExt>();
                        //    item.AgingList?.ForEach(o =>
                        //    {
                        //        var ext = new FD_IndividualIdentificationExt()
                        //        {
                        //            NumericalOrderDetail = item.NumericalOrderDetail,
                        //            NumericalOrder = request.NumericalOrder,
                        //            BusiType = o.BusiType,
                        //            Name = o.Name,
                        //            Amount = o.Amount,
                        //            ModifiedDate = DateTime.Now
                        //        };
                        //        list.Add(ext);
                        //    });
                        //    _extRepository.AddRange(list);
                        //}
                        //else
                        //{
                        //    detailData?.ForEach(p =>
                        //    {
                        //        p?.Update(p.BusiType, p.Name, p.Amount);
                        //    });
                        //}

                        await _extRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);

                        var list = new List<FD_IndividualIdentificationExt>();
                        item.AgingList?.ForEach(o =>
                        {
                            var ext = new FD_IndividualIdentificationExt()
                            {
                                NumericalOrderDetail = item.NumericalOrderDetail,
                                NumericalOrder = request.NumericalOrder,
                                BusiType = o.BusiType,
                                Name = o.Name,
                                Amount = o.Amount,
                                ModifiedDate = DateTime.Now
                            };
                            list.Add(ext);
                        });
                        // _extRepository.AddRange(list);
                        await _extRepository.AddRangeAsync(list);
                    }
                }
               
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extRepository.UnitOfWork.SaveChangesAsync();
                result.data = domain.NumericalOrder;
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
