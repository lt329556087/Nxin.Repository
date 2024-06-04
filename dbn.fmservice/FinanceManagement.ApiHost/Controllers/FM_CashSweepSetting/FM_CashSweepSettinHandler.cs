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
using Architecture.Common.Application.Commands;
using FinanceManagement.ApiHost.Applications.Queries;

namespace FinanceManagement.ApiHost.Controllers.FM_CashSweepSetting
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FM_CashSweepSettingAddHandler : IRequestHandler<FM_CashSweepSettingAddCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CashSweepSettingRepository _repository;
        IFM_CashSweepSettingDetailRepository _detailRepository;
        IFM_CashSweepSettingExtRepository _extendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FM_CashSweepSettingAddHandler(IIdentityService identityService, IFM_CashSweepSettingRepository repository, IFM_CashSweepSettingDetailRepository detailRepository, IFM_CashSweepSettingExtRepository extendRepository, IBiz_ReviewRepository biz_ReviewRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_CashSweepSettingAddCommand request, CancellationToken cancellationToken)
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
                var domain = new Domain.FM_CashSweepSetting()
                {
                    Remarks=request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    EnterpriseID = request.EnterpriseID
                };
                var extList = new List<FM_CashSweepSettingExt>();
                var detailList = new List<FM_CashSweepSettingDetail>();
                foreach (var item in request.Lines)
                {
                    var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                    detailList.Add(new FM_CashSweepSettingDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = detailNumericalOrder,
                        SweepDirection = item.SweepDirection,
                        AccountTransferAbstract = item.AccountTransferAbstract,
                        Remarks = item.Remarks,
                        OwnerID=item.OwnerID,
                        ModifiedDate = DateTime.Now
                    });
                    item.Extends?.ForEach(o => {
                        extList.Add(new FM_CashSweepSettingExt()
                        {
                            NumericalOrder = numericalOrder,
                            NumericalOrderDetail = detailNumericalOrder,
                            BusiType = o.BusiType,
                            TicketedPointID = o.TicketedPointID,
                            ReceiptAbstractID = o.ReceiptAbstractID,
                            AccoSubjectID = o.AccoSubjectID,
                            OrganizationSortID =string.IsNullOrEmpty( o.OrganizationSortID)?"0": o.OrganizationSortID,
                            OwnerID = o.OwnerID,
                            ModifiedDate = DateTime.Now
                        });
                    });
                }
             
                await _repository.AddAsync(domain);
                await _detailRepository.AddRangeAsync(detailList);
                await _extendRepository.AddRangeAsync(extList);
                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
               
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
    public class FM_CashSweepSettingDeleteHandler : IRequestHandler<FM_CashSweepSettingDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CashSweepSettingRepository _repository;
        IFM_CashSweepSettingDetailRepository _detailRepository;
        IFM_CashSweepSettingExtRepository _extendRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        FM_CashSweepSettingODataProvider _provider;

        public FM_CashSweepSettingDeleteHandler(IIdentityService identityService, IFM_CashSweepSettingRepository repository, IFM_CashSweepSettingDetailRepository detailRepository, IFM_CashSweepSettingExtRepository extendRepository, IBiz_ReviewRepository biz_ReviewRepository
            , FM_CashSweepSettingODataProvider provider)
        {
            _provider = provider;
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FM_CashSweepSettingDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
              
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                var data = _provider.GetDataByEnterID(request.EnterpriseID);
                if (data?.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == item.NumericalOrder);
                        await _extendRepository.RemoveRangeAsync(o => o.NumericalOrder == item.NumericalOrder);
                        await _repository.RemoveRangeAsync(o => o.EnterpriseID == request.EnterpriseID && o.NumericalOrder == item.NumericalOrder);
                    }
                    await _repository.UnitOfWork.SaveChangesAsync();
                }
                
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

    public class FM_CashSweepSettingModifyHandler : IRequestHandler<FM_CashSweepSettingModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFM_CashSweepSettingRepository _repository;
        IFM_CashSweepSettingDetailRepository _detailRepository;
        IFM_CashSweepSettingExtRepository _extendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FM_CashSweepSettingModifyHandler(IIdentityService identityService, IFM_CashSweepSettingRepository repository, IFM_CashSweepSettingDetailRepository detailRepository, IFM_CashSweepSettingExtRepository extendRepository
            , NumericalOrderCreator numericalOrderCreator
            )
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _numericalOrderCreator = numericalOrderCreator;
        }

        public async Task<Result> Handle(FM_CashSweepSettingModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (domain == null) { result.code = ErrorCode.Update.GetIntValue(); result.msg = "NumericalOrder查询空"; return result; }
                domain.Update(request.Remarks);

                await _extendRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);

                var extList = new List<FM_CashSweepSettingExt>();
                foreach (var item in request.Lines)
                {
                    var detailNumericalOrder = await _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FM_CashSweepSettingDetail()
                    {
                        NumericalOrder = request.NumericalOrder,
                        NumericalOrderDetail = detailNumericalOrder,
                        SweepDirection = item.SweepDirection,
                        AccountTransferAbstract = item.AccountTransferAbstract,
                        Remarks = item.Remarks,
                        OwnerID = item.OwnerID,
                        ModifiedDate = DateTime.Now
                    });
                    item.Extends?.ForEach(o => {
                        extList.Add(new FM_CashSweepSettingExt()
                        {
                            NumericalOrder = request.NumericalOrder,
                            NumericalOrderDetail = detailNumericalOrder,
                            BusiType = o.BusiType,
                            TicketedPointID = o.TicketedPointID,
                            ReceiptAbstractID = o.ReceiptAbstractID,
                            AccoSubjectID = o.AccoSubjectID,
                            OrganizationSortID = o.OrganizationSortID,
                            OwnerID = o.OwnerID,
                            ModifiedDate = DateTime.Now
                        });
                    });
                }

                await _extendRepository.AddRangeAsync(extList);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
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
