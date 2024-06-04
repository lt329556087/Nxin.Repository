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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_PigAssetsReset
{
    public class FA_PigAssetsResetAddHandler : IRequestHandler<FA_PigAssetsResetAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PigAssetsResetRepository _repository;
        IFA_PigAssetsResetDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        IFA_AssetsInspectDetailRepository _fA_AssetsInspectDetailRepository;
        IFA_AssetsApplyDetailRepository _fA_AssetsApplyDetailRepository;
        IFA_AssetsContractDetailRepository _fA_AssetsContractDetailRepository;
        FA_PurchaseSettingsODataProvider _provider;
        public FA_PigAssetsResetAddHandler(IIdentityService identityService,
                                            IFA_PigAssetsResetRepository repository,
                                            IFA_PigAssetsResetDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository,
                                            IFA_AssetsInspectDetailRepository fA_AssetsInspectDetailRepository,
                                            IFA_AssetsApplyDetailRepository fA_AssetsApplyDetailRepository,
                                            IFA_AssetsContractDetailRepository fA_AssetsContractDetailRepository,
                                            FA_PurchaseSettingsODataProvider provider)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _fA_AssetsInspectDetailRepository = fA_AssetsInspectDetailRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _fA_AssetsApplyDetailRepository = fA_AssetsApplyDetailRepository;
            _fA_AssetsContractDetailRepository = fA_AssetsContractDetailRepository;
            _provider = provider;
        }
        public async Task<Result> Handle(FA_PigAssetsResetAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                long number = _numberCreator.Create<Domain.FA_PigAssetsReset>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_PigAssetsReset()
                {
                    NumericalOrder = numericalOrder,
                    EnterpriseID = _identityService.EnterpriseId,
                    DataDate = request.DataDate,
                    Number = number.ToString(),
                    PigfarmNatureID = request.PigfarmNatureID,
                    PigfarmID = request.PigfarmID,
                    PigNumberTypeId = request.PigNumberTypeId,
                    PigNumber = request.PigNumber,
                    PigPrice = request.PigPrice,
                    PigOriginalValue = request.PigOriginalValue,
                    BeginAccountPeriodDate = request.BeginAccountPeriodDate,
                    EndAccountPeriodDate =  request.EndAccountPeriodDate,
                    ResetOriginalValueDate = request.ResetOriginalValueDate,
                    ResetOriginalValueType = request.ResetOriginalValueType,
                    EquipmentProportion = request.EquipmentProportion,
                    HouseProportion = request.HouseProportion,
                    OwnerID = this._identityService.UserId,
                    Remarks = request.Remarks,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TicketedPointID= request.TicketedPointID,
                };
                for (int i = 0; i < request.Lines.Count; i++)
                {
                    var o = request.Lines[i];
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    if (!string.IsNullOrEmpty(o.AssetsCode) && !string.IsNullOrEmpty(o.NumericalOrderInput))
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205261535030000109",//猪场资产重置Appid
                            ParentValue = numericalOrderDetail.Result,
                            ChildType =  "1712201440170000101",//固定资产卡片appid
                            ChildValue = o.NumericalOrderInput,
                        });
                    }
                    if (!string.IsNullOrEmpty(o.InspectNumber) && !string.IsNullOrEmpty(o.NumericalOrderInput))
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205261535030000109",//猪场资产重置Appid
                            ParentValue = numericalOrderDetail.Result,
                            ChildType = "2205161654530000309",//资产验收appid
                            ChildValue = o.NumericalOrderInput,
                        });
                        var Inspect=  _fA_AssetsInspectDetailRepository.Get(o.NumericalOrderInput);
                        Inspect.Amount = o.ResetOriginalValue;
                        Inspect.UnitPrice = o.ResetOriginalValue / Inspect.Quantity;
                        _fA_AssetsInspectDetailRepository.Update(Inspect);
                    }
                    domain.AddDetail(new FA_PigAssetsResetDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AssetsCode = o.AssetsCode,
                        InspectNumber = o.InspectNumber,
                        AssetsName = o.AssetsName,
                        AssetsTypeID = o.AssetsTypeID,
                        Specification = o.Specification,
                        Brand = o.Brand,
                        MeasureUnit = o.MeasureUnit ?? "0",
                        MarketID = o.MarketID,
                        OriginalValue = o.OriginalValue,
                        NetValue = o.NetValue,
                        OriginalUseYear = o.OriginalUseYear,
                        ResetBase = o.ResetBase,
                        ResetUseYear = o.ResetUseYear,
                        ResetOriginalValue = o.ResetOriginalValue,
                        ContentType = o.ContentType,
                        Remarks = o.Remarks,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                    });
                }
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _repository.AddAsync(domain);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _fA_AssetsInspectDetailRepository.UnitOfWork.SaveChangesAsync();
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

    public class FA_PigAssetsResetDeleteHandler : IRequestHandler<FA_PigAssetsResetDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PigAssetsResetRepository _repository;
        IFA_PigAssetsResetDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_PigAssetsResetDeleteHandler(IIdentityService identityService,
                                            IFA_PigAssetsResetRepository repository,
                                            IFA_PigAssetsResetDetailRepository detailRepository,
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

        public async Task<Result> Handle(FA_PigAssetsResetDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    var nums = request.NumericalOrder.Split(',').ToList();
                    List<string> newNums = nums.Where((x, i) => nums.FindIndex(z => z == x) == i).ToList();
                    foreach (var item in newNums)
                    {
                        var domain = await _repository.GetAsync(item);
                        await _repository.RemoveRangeAsync(o => o.NumericalOrder == item);
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == item);
                        await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205261535030000109" && domain.Details.Select(s => s.NumericalOrderDetail).Contains(o.ParentValue));
                    }
                    await _repository.UnitOfWork.SaveChangesAsync();
                    await _detailRepository.UnitOfWork.SaveChangesAsync();
                    await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
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

    public class FA_PigAssetsResetModifyHandler : IRequestHandler<FA_PigAssetsResetModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_PigAssetsResetRepository _repository;
        IFA_PigAssetsResetDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IFA_AssetsApplyDetailRepository _fA_AssetsApplyDetailRepository;
        IFA_AssetsContractDetailRepository _fA_AssetsContractDetailRepository;
        FA_PurchaseSettingsODataProvider _provider;
        IFA_AssetsInspectDetailRepository _fA_AssetsInspectDetailRepository;
        public FA_PigAssetsResetModifyHandler(IIdentityService identityService,
                                            IFA_PigAssetsResetRepository repository,
                                            IFA_PigAssetsResetDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository,
                                            IFA_AssetsInspectDetailRepository fA_AssetsInspectDetailRepository,
                                            IFA_AssetsApplyDetailRepository fA_AssetsApplyDetailRepository,
                                            IFA_AssetsContractDetailRepository fA_AssetsContractDetailRepository,
                                            FA_PurchaseSettingsODataProvider provider)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
            _fA_AssetsInspectDetailRepository = fA_AssetsInspectDetailRepository;
            _fA_AssetsApplyDetailRepository = fA_AssetsApplyDetailRepository;
            _fA_AssetsContractDetailRepository = fA_AssetsContractDetailRepository;
            _provider = provider;
        }

        public async Task<Result> Handle(FA_PigAssetsResetModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                var domain = await _repository.GetAsync(request.NumericalOrder);
                if (request.IsUpdateEndAccountPeriodDate)
                {
                    domain.UpdateEndAccountPeriodDate(request.EndAccountPeriodDate);
                    await _repository.UnitOfWork.SaveChangesAsync();
                    result.data = new { NumericalOrder = request.NumericalOrder };
                    result.code = ErrorCode.Success.GetIntValue();
                    return result;
                }
                domain.Update(_identityService.EnterpriseId, request.DataDate, request.Number, request.PigfarmNatureID, request.PigfarmID, request.PigNumberTypeId,
            request.PigNumber, request.PigPrice, request.PigOriginalValue, request.BeginAccountPeriodDate, request.ResetOriginalValueDate, request.ResetOriginalValueType,
            request.EquipmentProportion, request.HouseProportion, request.OwnerID, request.Remarks,request.TicketedPointID,request.EndAccountPeriodDate);
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        if (!string.IsNullOrEmpty(item.AssetsCode) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205261535030000109",//猪场资产重置Appid
                                ParentValue = numericalOrderDetail,
                                ChildType = "1712201440170000101",//固定资产卡片appid
                                ChildValue = item.NumericalOrderInput,
                            });
                        }
                        if (!string.IsNullOrEmpty(item.InspectNumber) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205261535030000109",//猪场资产重置Appid
                                ParentValue = numericalOrderDetail,
                                ChildType = "2205161654530000309",//资产验收appid
                                ChildValue = item.NumericalOrderInput,
                            });
                            var Inspect = _fA_AssetsInspectDetailRepository.Get(item.NumericalOrderInput);
                            Inspect.Amount = item.ResetOriginalValue;
                            Inspect.UnitPrice = item.ResetOriginalValue / Inspect.Quantity;
                            _fA_AssetsInspectDetailRepository.Update(Inspect);
                        }
                        domain.AddDetail(new FA_PigAssetsResetDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            AssetsCode = item.AssetsCode,
                            InspectNumber = item.InspectNumber,
                            AssetsName = item.AssetsName,
                            AssetsTypeID = item.AssetsTypeID,
                            Specification = item.Specification,
                            Brand = item.Brand,
                            MeasureUnit = item.MeasureUnit,
                            MarketID = item.MarketID,
                            OriginalValue = item.OriginalValue,
                            NetValue = item.NetValue,
                            OriginalUseYear = item.OriginalUseYear,
                            ResetBase = item.ResetBase,
                            ResetUseYear = item.ResetUseYear,
                            ResetOriginalValue = item.ResetOriginalValue,
                            ContentType = item.ContentType,
                            Remarks = item.Remarks,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details?.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj.Update(item.AssetsCode, item.InspectNumber, item.AssetsName, item.AssetsTypeID, item.Specification, item.Brand, item.MeasureUnit, item.MarketID, item.OriginalValue, item.NetValue, item.OriginalUseYear, item.ResetBase
                        , item.ResetUseYear, item.ResetOriginalValue, item.ContentType, item.Remarks);
                        #region 保存中间关系
                        if (!string.IsNullOrEmpty(item.AssetsCode) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205261535030000109" && o.ParentValue == obj.NumericalOrderDetail && o.ChildType == "1712201440170000101");
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205261535030000109",//猪场资产重置Appid
                                ParentValue = obj.NumericalOrderDetail,
                                ChildType = "1712201440170000101",//资产卡片
                                ChildValue = item.NumericalOrderInput,
                            });
                        }
                        if (!string.IsNullOrEmpty(item.InspectNumber) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205261535030000109" && o.ParentValue == obj.NumericalOrderDetail && o.ChildType == "2205161654530000309");
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205261535030000109",//猪场资产重置Appid
                                ParentValue = obj.NumericalOrderDetail,
                                ChildType = "2205161654530000309",//资产验收单Appid
                                ChildValue = item.NumericalOrderInput,
                            });
                            var Inspect = _fA_AssetsInspectDetailRepository.Get(item.NumericalOrderInput);
                            Inspect.Amount = item.ResetOriginalValue;
                            Inspect.UnitPrice = item.ResetOriginalValue / Inspect.Quantity;
                            _fA_AssetsInspectDetailRepository.Update(Inspect);
                        }
                        #endregion
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        if (!string.IsNullOrEmpty(item.AssetsCode) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205261535030000109" && o.ParentValue == item.NumericalOrderDetail && o.ChildType == "1712201440170000101");
                        }
                        if (!string.IsNullOrEmpty(item.InspectNumber) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205261535030000109" && o.ParentValue == item.NumericalOrderDetail && o.ChildType == "2205161654530000309");
                        }
                        continue;
                    }
                }
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
                await _fA_AssetsInspectDetailRepository.UnitOfWork.SaveChangesAsync();
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
