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

namespace FinanceManagement.ApiHost.Controllers.FA_PurchaseSettings
{
    public class FA_PurchaseSettingsAddHandler : IRequestHandler<FA_PurchaseSettingsAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PurchaseSettingsRepository _repository;
        IFA_PurchaseSettingsDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_PurchaseSettingsAddHandler(IIdentityService identityService,
                                            IFA_PurchaseSettingsRepository repository,
                                            IFA_PurchaseSettingsDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(FA_PurchaseSettingsAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var modifyFieldID = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_PurchaseSettings()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = this._identityService.EnterpriseId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifyFieldID = modifyFieldID
                };
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                if (request.ModifyFieldList?.Count() > 0)
                {
                    request.ModifyFieldList.ForEach(s =>
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205121429180000109",//资产购置设置Appid
                            ParentValue = modifyFieldID,
                            ChildType = "2202271716580000115",//可修改字段pid
                            ChildValue = s.DataDictID,
                        });
                    });
                }
                else
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "可修改字段不可为空";
                    return result;
                }

                request.Lines?.ForEach(o =>
                {
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    var assetsTypeID = _numericalOrderCreator.CreateAsync();
                    domain.AddDetail(new FA_PurchaseSettingsDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AssetsTypeID = assetsTypeID.Result,
                        BeginRange = o.BeginRange,
                        EndRange = o.EndRange,
                        FloatingDirectionID = o.FloatingDirectionID,
                        FloatingTypeID = o.FloatingTypeID,
                        MaxFloatingValue = o.MaxFloatingValue,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                    });

                    o.AssetsTypeList?.ForEach(s =>
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205121429180000109",//资产购置设置Appid
                            ParentValue = assetsTypeID.Result,
                            ChildType = "1712071843500000101",//资产类别appid
                            ChildValue = s.AssetsTypeID,
                        });
                    });
                });
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _repository.AddAsync(domain);
                Biz_Review review = new Biz_Review(numericalOrder, "2205121429180000109", _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
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

    public class FA_PurchaseSettingsDeleteHandler : IRequestHandler<FA_PurchaseSettingsDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PurchaseSettingsRepository _repository;
        IFA_PurchaseSettingsDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_PurchaseSettingsDeleteHandler(IIdentityService identityService,
                                            IFA_PurchaseSettingsRepository repository,
                                            IFA_PurchaseSettingsDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(FA_PurchaseSettingsDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.EnterpriseId && o.NumericalOrder == request.NumericalOrder);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205121429180000109" && o.ParentValue == request.ModifyFieldID && o.ChildType == "2202271716580000115");
                    await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205121429180000109" && request.AssetsTypeIDList.Contains(o.ParentValue) && o.ChildType == "1712071843500000101");//因为表体多行，所以用Contains
                }
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = "删除成功!";
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

    public class FA_PurchaseSettingsModifyHandler : IRequestHandler<FA_PurchaseSettingsModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PurchaseSettingsRepository _repository;
        IFA_PurchaseSettingsDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_PurchaseSettingsModifyHandler(IIdentityService identityService,
                                            IFA_PurchaseSettingsRepository repository,
                                            IFA_PurchaseSettingsDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(FA_PurchaseSettingsModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205121429180000109" && o.ParentValue == domain.ModifyFieldID && o.ChildType == "2202271716580000115");
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                if (request.ModifyFieldList?.Count() > 0)
                {
                    request.ModifyFieldList.ForEach(s =>
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205121429180000109",//资产购置设置Appid
                            ParentValue = domain.ModifyFieldID,
                            ChildType = "2202271716580000115",
                            ChildValue = s.DataDictID,
                        });
                    });
                }
                else
                {
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    result.msg = "可修改字段不可为空";
                    return result;
                }
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        var assetsTypeID = await this._numericalOrderCreator.CreateAsync();
                        domain.AddDetail(new FA_PurchaseSettingsDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            AssetsTypeID = assetsTypeID,
                            BeginRange = item.BeginRange,
                            EndRange = item.EndRange,
                            FloatingDirectionID = item.FloatingDirectionID,
                            FloatingTypeID = item.FloatingTypeID,
                            MaxFloatingValue = item.MaxFloatingValue,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                        });
                        item.AssetsTypeList?.ForEach(s =>
                        {
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205121429180000109",//资产购置设置Appid
                                ParentValue = assetsTypeID,
                                ChildType = "1712071843500000101",//资产类别appid
                                ChildValue = s.AssetsTypeID,
                            });
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {

                        var obj = domain.Details.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj.Update(item.AssetsTypeID, item.BeginRange, item.EndRange, item.FloatingDirectionID, item.FloatingTypeID, item.MaxFloatingValue, DateTime.Now);

                        await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205121429180000109" && o.ParentValue == obj.AssetsTypeID && o.ChildType == "1712071843500000101");
                        item.AssetsTypeList?.ForEach(s =>
                        {
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205121429180000109",//资产购置设置Appid
                                ParentValue = obj.AssetsTypeID,
                                ChildType = "1712071843500000101",//资产类别appid
                                ChildValue = s.AssetsTypeID,
                            });
                        });
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205121429180000109" && o.ParentValue == item.AssetsTypeID && o.ChildType == "1712071843500000101");
                        continue;
                    }
                }
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
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
