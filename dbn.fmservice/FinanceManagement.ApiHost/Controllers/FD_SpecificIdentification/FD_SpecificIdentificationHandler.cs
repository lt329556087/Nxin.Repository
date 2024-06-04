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

namespace FinanceManagement.ApiHost.Controllers.FD_SpecificIdentification
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FD_SpecificIdentificationAddHandler : IRequestHandler<FD_SpecificIdentificationAddCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SpecificIdentificationRepository _repository;
        IFD_SpecificIdentificationDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_SpecificIdentificationAddHandler(IIdentityService identityService, IFD_SpecificIdentificationRepository repository, IFD_SpecificIdentificationDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_SpecificIdentificationAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(_identityService.EnterpriseId) || _identityService.EnterpriseId == "0")
                {
                    result.msg = "登录单位空";
                    result.code = ErrorCode.Create.GetIntValue();
                    return result;
                }
                long number = _numberCreator.Create<Domain.FD_SpecificIdentification>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FD_SpecificIdentification()
                {
                    Number = number.ToString(),
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = _identityService.UserId,
                    EnterpriseID = _identityService.EnterpriseId,
                    AccoSubjectID1 = request.AccoSubjectID1,
                    AccoSubjectID2 = request.AccoSubjectID2,
                    BusinessType = request.BusinessType,
                    NumericalOrderSetting = request.NumericalOrderSetting
                };



                if (request.Lines1.Count == 0)
                {
                    request.Lines1 = new List<FD_SpecificIdentificationDetailCommand>();
                }

                request.Lines1.AddRange(request.Lines2);
                request.Lines1.RemoveAll(o => o.CustomerID == null);
                request.Lines1.RemoveAll(o => o.ProvisionType == null);
                request.Lines1?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync().Result;
                    var detail = new FD_SpecificIdentificationDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail,
                        ProvisionType = o.ProvisionType,
                        CustomerID = o.CustomerID,
                        AccoSubjectID = o.AccoSubjectID,
                        Amount = o.Amount,
                        AccoAmount = o.AccoAmount,
                        Remarks = o.Remarks,
                        ModifiedDate = DateTime.Now
                    };

                    o?.AgingList?.ForEach(o =>
                    {
                        var ext = new FD_SpecificIdentificationExt()
                        {
                            NumericalOrderDetail = numericalOrderDetail,
                            Name = o.Name,
                            Amount = o.Amount
                        };
                        detail.AgingList.Add(ext);
                    });
                    domain.AddDetail(detail);
                });

                //Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                //await _biz_ReviewRepository.AddAsync(review);
                await _repository.AddAsync(domain);
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
    public class FD_SpecificIdentificationDeleteHandler : IRequestHandler<FD_SpecificIdentificationDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SpecificIdentificationRepository _repository;
        IFD_SpecificIdentificationDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        public FD_SpecificIdentificationDeleteHandler(IIdentityService identityService, IFD_SpecificIdentificationRepository repository, IFD_SpecificIdentificationDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(FD_SpecificIdentificationDeleteCommand request, CancellationToken cancellationToken)
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

    public class FD_SpecificIdentificationModifyHandler : IRequestHandler<FD_SpecificIdentificationModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFD_SpecificIdentificationRepository _repository;
        IFD_SpecificIdentificationDetailRepository _detailRepository;
        IFD_SpecificIdentificationExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        public FD_SpecificIdentificationModifyHandler(IIdentityService identityService, IFD_SpecificIdentificationRepository repository, IFD_SpecificIdentificationDetailRepository detailRepository,
            NumericalOrderCreator numericalOrderCreator,
            IFD_SpecificIdentificationExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _extRepository = extRepository;

        }

        public async Task<Result> Handle(FD_SpecificIdentificationModifyCommand request, CancellationToken cancellationToken)
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
                if (string.IsNullOrEmpty(request.DataDate))
                {
                    result.msg = "单据日期不能为空！";
                    result.code = ErrorCode.Update.GetIntValue();
                    return result;
                }

                domain.Update(Convert.ToDateTime(request.DataDate),
                    request.Number);

                request.Lines1.AddRange(request.Lines2);
                foreach (var item in request.Lines1)
                {
                    var obj = domain.Lines.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = _numericalOrderCreator.CreateAsync().Result;
                        var detail = new FD_SpecificIdentificationDetail()
                        {

                            NumericalOrderDetail = numericalOrderDetail,
                            NumericalOrder = domain.NumericalOrder,
                            ProvisionType = item.ProvisionType,
                            CustomerID = item.CustomerID,
                            AccoSubjectID = item.AccoSubjectID,
                            Amount = item.Amount,
                            AccoAmount = item.AccoAmount,
                            Remarks = item.Remarks,
                            ModifiedDate = DateTime.Now
                        };

                        item.AgingList.ForEach(o =>
                        {
                            var ext = new FD_SpecificIdentificationExt()
                            {
                                NumericalOrderDetail = numericalOrderDetail,
                                Amount = o.Amount,
                                Name = o.Name
                            };
                            ///detail.AgingList.Add(ext);
                            _extRepository.Add(ext);
                        });

                        _detailRepository.Add(detail);
                        //domain.AddDetail(detail);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        obj.Update(item.ProvisionType, item.CustomerID, item.Amount, item.AccoAmount, item.Remarks);
                        await _extRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);

                        var list = new List<FD_SpecificIdentificationExt>();
                        obj.AgingList.ForEach(o =>
                       {
                           var ext = new FD_SpecificIdentificationExt()
                           {
                               Amount = o.Amount,
                               Name = o.Name,
                               NumericalOrderDetail = o.NumericalOrderDetail
                           };
                           list.Add(ext);
                       });
                        _extRepository.AddRange(list);
                        continue;
                    }
                }
                await _detailRepository.UnitOfWork.SaveChangesAsync();
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
