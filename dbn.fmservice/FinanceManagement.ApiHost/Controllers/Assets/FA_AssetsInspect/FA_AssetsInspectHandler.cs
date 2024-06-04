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

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsInspect
{
    public class FA_AssetsInspectAddHandler : IRequestHandler<FA_AssetsInspectAddCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsInspectRepository _repository;
        IFA_AssetsInspectDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;


        IFA_AssetsApplyDetailRepository _fA_AssetsApplyDetailRepository;
        IFA_AssetsContractDetailRepository _fA_AssetsContractDetailRepository;
        FA_PurchaseSettingsODataProvider _provider;
        public FA_AssetsInspectAddHandler(IIdentityService identityService,
                                            IFA_AssetsInspectRepository repository,
                                            IFA_AssetsInspectDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository,
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
            _fA_AssetsApplyDetailRepository = fA_AssetsApplyDetailRepository;
            _fA_AssetsContractDetailRepository = fA_AssetsContractDetailRepository;
            _provider = provider;
        }
        public async Task<Result> Handle(FA_AssetsInspectAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                List<object> list = VerificationMethod(request);
                if (list.Count > 0)
                {
                    result.data = list;
                    result.code = ErrorCode.Create.GetIntValue();
                    result.msg = "验证失败";
                    return result;
                }
                long number = _numberCreator.Create<Domain.FA_AssetsInspect>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.EnterpriseId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.FA_AssetsInspect()
                {
                    NumericalOrder = numericalOrder,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = request.EnterpriseID,
                    TicketedPointID = request.TicketedPointID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    MarketID = request.MarketID,
                    DataDate = request.DataDate,
                    Remarks = request.Remarks,
                    PersonID = request.PersonID,
                    Number = number.ToString()
                };

                for (int i = 0; i < request.Lines.Count; i++)
                {
                    var o = request.Lines[i];
                    var numericalOrderDetail = _numericalOrderCreator.CreateAsync();
                    if (!string.IsNullOrEmpty(request.ApplyForms) && !string.IsNullOrEmpty(o.NumericalOrderInput))
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205161654530000309",//资产验收单Appid
                            ParentValue = numericalOrderDetail.Result,
                            ChildType = "2204151105080000109",//购置申请单appid
                            ChildValue = o.NumericalOrderInput,
                            //ParentValueDetail = numericalOrder
                        });
                    }
                    if (!string.IsNullOrEmpty(request.ContractForms) && !string.IsNullOrEmpty(o.NumericalOrderInput))
                    {
                        relateds.Add(new BIZ_Related()
                        {
                            RelatedType = "201610210104402102",
                            ParentType = "2205161654530000309",//资产验收单Appid
                            ParentValue = numericalOrderDetail.Result,
                            ChildType = "2204251425130000109",//购置合同appid
                            ChildValue = o.NumericalOrderInput,
                            // ParentValueDetail = numericalOrder
                        });
                    }
                    domain.AddDetail(new FA_AssetsInspectDetail()
                    {
                        NumericalOrder = numericalOrder,
                        NumericalOrderDetail = numericalOrderDetail.Result,
                        AssetsName = o.AssetsName,
                        AssetsTypeID = o.AssetsTypeID,
                        Specification = o.Specification,
                        Brand = o.Brand,
                        MeasureUnit = o.MeasureUnit,
                        Quantity = o.Quantity,
                        UnitPrice = o.UnitPrice,
                        Amount = o.Amount,
                        SupplierID = o.SupplierID ?? "0",
                        ProjectID = o.ProjectID??"0",
                        Remarks = o.Remarks,
                        AssetsNatureId = o.AssetsNatureId ?? "0",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                    });
                }
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _repository.AddAsync(domain);
                Biz_Review review = new Biz_Review(numericalOrder, "2205161654530000309", _identityService.UserId).SetMaking();
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
                result.data = ex;
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存失败,请联系管理员！";
            }
            return result;
        }

        public List<object> VerificationMethod(FA_AssetsInspectAddCommand main)
        {
            List<object> list = new List<object>();
            FA_PurchaseSettingsODataEntity setting = _provider.GetDataAsync(0).Result;
            if (setting==null)
            {
                return list;
            }
            string json = JsonConvert.SerializeObject(setting);

            for (int i = 0; i < main.Lines.Count; i++)
            {
                var detail = main.Lines[i];
                if (!string.IsNullOrEmpty(detail.NumericalOrderInput))
                {
                    decimal oldAmount = 0;
                    if (!string.IsNullOrEmpty(main.ApplyForms))
                    {
                        var domain = _fA_AssetsApplyDetailRepository.GetAsync(detail.NumericalOrderInput).Result;
                        oldAmount = domain.Amount;
                    }
                    if (!string.IsNullOrEmpty(main.ContractForms))
                    {
                        var domain = _fA_AssetsContractDetailRepository.GetAsync(detail.NumericalOrderInput).Result;
                        oldAmount = domain.Amount;
                    }
                    var result = VerificationHandler.VerificationMethod(JsonConvert.DeserializeObject<FA_PurchaseSettingsODataEntity>(json), detail, oldAmount, i + 1);
                    if (result != null)
                    {
                        list.Add(result);
                    }
                }
            }
            return list;
        }
    }

    public class FA_AssetsInspectDeleteHandler : IRequestHandler<FA_AssetsInspectDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsInspectRepository _repository;
        IFA_AssetsInspectDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;

        public FA_AssetsInspectDeleteHandler(IIdentityService identityService,
                                            IFA_AssetsInspectRepository repository,
                                            IFA_AssetsInspectDetailRepository detailRepository,
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

        public async Task<Result> Handle(FA_AssetsInspectDeleteCommand request, CancellationToken cancellationToken)
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
                        await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == item);
                        await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205161654530000309"&& domain.Details.Select(s => s.NumericalOrderDetail).Contains(o.ParentValue));
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

    public class FA_AssetsInspectModifyHandler : IRequestHandler<FA_AssetsInspectModifyCommand, Result>
    {
        IIdentityService _identityService;
        IBiz_Related _biz_RelatedRepository;
        IFA_AssetsInspectRepository _repository;
        IFA_AssetsInspectDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IFA_AssetsApplyDetailRepository _fA_AssetsApplyDetailRepository;
        IFA_AssetsContractDetailRepository _fA_AssetsContractDetailRepository;
        FA_PurchaseSettingsODataProvider _provider;

        public FA_AssetsInspectModifyHandler(IIdentityService identityService,
                                            IFA_AssetsInspectRepository repository,
                                            IFA_AssetsInspectDetailRepository detailRepository,
                                            NumericalOrderCreator numericalOrderCreator,
                                            NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
                                            IBiz_ReviewRepository biz_ReviewRepository,
                                            IBiz_Related biz_RelatedRepository,
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
            _fA_AssetsApplyDetailRepository = fA_AssetsApplyDetailRepository;
            _fA_AssetsContractDetailRepository = fA_AssetsContractDetailRepository;
            _provider = provider;
        }

        public async Task<Result> Handle(FA_AssetsInspectModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                List<object> list = VerificationMethod(request);
                if (list.Count > 0)
                {
                    result.data = list;
                    result.code = ErrorCode.Update.GetIntValue();
                    result.msg = "验证失败";
                    return result;
                }

                var domain = await _repository.GetAsync(request.NumericalOrder);
                domain.Update(request.EnterpriseID, request.DataDate, request.TicketedPointID, request.MarketID, request.PersonID, request.Remarks);
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        #region 保存中间关系
                        if (!string.IsNullOrEmpty(request.ApplyForms) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205161654530000309",//资产验收单Appid
                                ParentValue = numericalOrderDetail,
                                ChildType = "2204151105080000109",//购置申请单appid
                                ChildValue = item.NumericalOrderInput,
                                //ParentValueDetail = domain.NumericalOrder
                            });
                        }
                        if (!string.IsNullOrEmpty(request.ContractForms) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205161654530000309",//资产验收单Appid
                                ParentValue = numericalOrderDetail,
                                ChildType = "2204251425130000109",//购置合同appid
                                ChildValue = item.NumericalOrderInput,
                                // ParentValueDetail = domain.NumericalOrder
                            });
                        }
                        #endregion
                        domain.AddDetail(new FA_AssetsInspectDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            NumericalOrderDetail = numericalOrderDetail,
                            AssetsName = item.AssetsName,
                            AssetsTypeID = item.AssetsTypeID,
                            Specification = item.Specification,
                            Brand = item.Brand,
                            MeasureUnit = item.MeasureUnit,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            Amount = item.Amount,
                            SupplierID = item.SupplierID,
                            ProjectID = item.ProjectID ?? "0",
                            Remarks = item.Remarks,
                            AssetsNatureId = item.AssetsNatureId,
                            ModifiedDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details?.Find(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        obj.Update(item.AssetsName, item.AssetsTypeID, item.Specification, item.Brand, item.MeasureUnit, item.AssetsNatureId, item.Quantity, item.UnitPrice, item.Amount, item.SupplierID, item.ProjectID, item.Remarks);
                        #region 保存中间关系
                        if (!string.IsNullOrEmpty(request.ApplyForms) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205161654530000309" && o.ParentValue == obj.NumericalOrderDetail && o.ChildType == "2204151105080000109");
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205161654530000309",//资产验收单Appid
                                ParentValue = obj.NumericalOrderDetail,
                                ChildType = "2204151105080000109",//购置申请单appid
                                ChildValue = item.NumericalOrderInput,
                                //ParentValueDetail = domain.NumericalOrder
                            });
                        }
                        if (!string.IsNullOrEmpty(request.ContractForms) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205161654530000309" && o.ParentValue == obj.NumericalOrderDetail && o.ChildType == "2204251425130000109");
                            relateds.Add(new BIZ_Related()
                            {
                                RelatedType = "201610210104402102",
                                ParentType = "2205161654530000309",//资产验收单Appid
                                ParentValue = obj.NumericalOrderDetail,
                                ChildType = "2204251425130000109",//购置合同appid
                                ChildValue = item.NumericalOrderInput,
                                //ParentValueDetail = domain.NumericalOrder
                            });
                        }
                        #endregion
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.NumericalOrderDetail == item.NumericalOrderDetail);
                        if (!string.IsNullOrEmpty(request.ApplyForms) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205161654530000309" && o.ParentValue == item.NumericalOrderDetail && o.ChildType == "2204151105080000109");
                        }
                        if (!string.IsNullOrEmpty(request.ContractForms) && !string.IsNullOrEmpty(item.NumericalOrderInput))
                        {
                            await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2205161654530000309" && o.ParentValue == item.NumericalOrderDetail && o.ChildType == "2204251425130000109");
                        }
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
        public List<object> VerificationMethod(FA_AssetsInspectModifyCommand main)
        {
            List<object> list = new List<object>();
            FA_PurchaseSettingsODataEntity setting = _provider.GetDataAsync(0).Result;
            if (setting == null)
            {
                return list;
            }
            string json = JsonConvert.SerializeObject(setting);
            for (int i = 0; i < main.Lines.Count; i++)
            {
                var detail = main.Lines[i];
                if (!string.IsNullOrEmpty(detail.NumericalOrderInput) && (detail.IsCreate || detail.IsUpdate))
                {
                    decimal oldAmount = 0;
                    if (!string.IsNullOrEmpty(main.ApplyForms))
                    {
                        var domain = _fA_AssetsApplyDetailRepository.GetAsync(detail.NumericalOrderInput).Result;
                        oldAmount = domain.Amount;
                    }
                    if (!string.IsNullOrEmpty(main.ContractForms))
                    {
                        var domain = _fA_AssetsContractDetailRepository.GetAsync(detail.NumericalOrderInput).Result;
                        oldAmount = domain.Amount;
                    }
                    var result = VerificationHandler.VerificationMethod(JsonConvert.DeserializeObject<FA_PurchaseSettingsODataEntity>(json), detail, oldAmount, i + 1);
                    if (result != null)
                    {
                        list.Add(result);
                    }
                }
            }
            return list;
        }
    }

    public static class VerificationHandler
    {
        public static object VerificationMethod(FA_PurchaseSettingsODataEntity setting, FA_AssetsInspectDetailCommand detail, decimal oldAmount, int index)
        {
            setting.Lines?.ForEach(o =>
            {
                o.AssetsTypeID = string.Join(',', o.AssetsTypeList?.Select(s => s.AssetsTypeID));
            });
            var settingHaves = setting.Lines?.Where(o => o.AssetsTypeID.Contains(detail.AssetsTypeID) && o.BeginRange < oldAmount && o.EndRange >= oldAmount).ToList();
            if (settingHaves?.Count > 0)
            {
                settingHaves.ForEach(_ =>
                {
                    if (_.FloatingTypeID == "2003031357470000120")//浮动类型=百分比
                    {
                        _.MaxFloatingValue = Math.Round((oldAmount * _.MaxFloatingValue), 2);
                    }
                });
                //var settingHave = settingHaves.Where(nn => nn.MaxFloatingValue == settingHaves.Max(s1 => s1.MaxFloatingValue)).FirstOrDefault();
                var settingHave = settingHaves.FirstOrDefault();
                decimal changeAmount = detail.Amount - oldAmount;
                if (settingHave?.FloatingDirectionID == "2003031357470000110" && (changeAmount < (-settingHave?.MaxFloatingValue) || changeAmount > 0))//向下浮动
                {
                    return new { Number = index, AssetsName = detail.AssetsName, Content = "资产购置验收单金额不能小于购置申请金额，请确认！" };
                }
                if (settingHave?.FloatingDirectionID == "2003031357470000111" && (changeAmount > settingHave?.MaxFloatingValue || changeAmount < 0))//向上浮动
                {
                    return new { Number = index, AssetsName = detail.AssetsName, Content = "资产购置验收单金额不能大于购置申请金额，请确认！" };
                }
                if (settingHave?.FloatingDirectionID == "2003031357470000112" && (changeAmount < (-settingHave?.MaxFloatingValue) || changeAmount > settingHave?.MaxFloatingValue))//上下浮动
                {
                    if (changeAmount < (-settingHave?.MaxFloatingValue))//超过下限
                    {
                        return new { Number = index, AssetsName = detail.AssetsName, Content = "资产购置验收单金额不能小于购置申请金额，请确认！" };
                    }
                    if (changeAmount > settingHave?.MaxFloatingValue)//超过上限
                    {
                        return new { Number = index, AssetsName = detail.AssetsName, Content = "资产购置验收单金额不能大于购置申请金额，请确认！" };
                    }

                }
            }
            return null;
        }
    }
}
