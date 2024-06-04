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

namespace FinanceManagement.ApiHost.Controllers
{
    public class MS_FormulaAddHandler : IRequestHandler<MS_FormulaAddCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaRepository _repository;
        IMS_FormulaDetailRepository _detailRepository;
        IMS_FormulaExtendRepository _extendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;

        public MS_FormulaAddHandler(IIdentityService identityService, IBiz_Related biz_RelatedRepository, IMS_FormulaRepository repository, IMS_FormulaDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IMS_FormulaExtendRepository extendRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extendRepository = extendRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(MS_FormulaAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                long number = _numberCreator.Create<Domain.MS_Formula>(o => o.DataDate, o => o.Number, Convert.ToDateTime(request.DataDate), o => o.EnterpriseID == _identityService.GroupId);
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var enterNumericalOrder = await _numericalOrderCreator.CreateAsync();
                var productNumericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.MS_Formula()
                {
                    NumericalOrder = numericalOrder,
                    Guid = Guid.NewGuid(),
                    DataDate = request.DataDate,
                    TicketedPointID = "0",
                    Number = number.ToString(),
                    FormulaName = request.FormulaName,
                    IsUse = true,
                    BaseQuantity = request.BaseQuantity,
                    Remarks = request.Remarks,
                    PackageRemarks = request.PackageRemarks,
                    OwnerID = this._identityService.UserId,
                    EnterpriseID = this._identityService.GroupId,
                    EarlyWarning = 0,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    EffectiveBeginDate = request.EffectiveBeginDate != null ? Convert.ToDateTime(request.EffectiveBeginDate) : Convert.ToDateTime("1000-01-01"),
                    EffectiveEndDate = request.EffectiveEndDate != null ? Convert.ToDateTime(request.EffectiveEndDate) : Convert.ToDateTime("9999-12-31"),
                    UseEnterprise = enterNumericalOrder,
                    UseProduct = productNumericalOrder,
                    IsGroup = 1
                };
                //使用单位关系
                request.UseEnterpriseList?.ForEach(s =>
                {
                    relateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "2004141510170000102",
                        ParentValue = enterNumericalOrder,
                        ChildType = "1709301424110000101",
                        ChildValue = s.Id,
                    });
                });
                //使用商品关系
                request.UseProductList?.ForEach(s =>
                {
                    relateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "2004141510170000102",
                        ParentValue = productNumericalOrder,
                        ChildType = "1708231814060000101",
                        ChildValue = s.Id,
                    });
                });
                int rounum = 1;
                foreach (var o in request.Lines)
                {
                    domain.AddDetail(new MS_FormulaDetail()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        ProductID = o.ProductID,
                        StockType = "0",
                        FormulaTypeID = "0",
                        ProportionQuantity = o.ProportionQuantity,
                        Quantity = o.Quantity,
                        ModifiedDate = DateTime.Now,
                        RowNum = o.RowNum != 0 ? o.RowNum : rounum,
                        Cost = o.Cost,
                        UnitCost = o.UnitCost
                    });
                    rounum++;
                }
                rounum = 1;
                foreach (var o in request.Extends)
                {
                    domain.AddExtend(new MS_FormulaExtend()
                    {
                        NumericalOrder = numericalOrder,
                        Guid = Guid.NewGuid(),
                        ProductID = o.ProductID,
                        PackingID = o.PackingID ?? "0",
                        Quantity = o.Quantity,
                        IsUse = o.IsUse,
                        RowNum = o.RowNum != 0 ? o.RowNum : rounum,
                        ModifiedDate = DateTime.Now,
                    });
                    rounum++;
                }
                await _repository.AddAsync(domain);
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, _identityService.UserId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
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

    public class MS_FormulaDeleteHandler : IRequestHandler<MS_FormulaDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaRepository _repository;
        IMS_FormulaDetailRepository _detailRepository;
        IMS_FormulaExtendRepository _extendRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IBiz_Related _biz_RelatedRepository;

        public MS_FormulaDeleteHandler(IIdentityService identityService, IBiz_ReviewRepository biz_ReviewRepository, IMS_FormulaRepository repository, IMS_FormulaDetailRepository detailRepository, IMS_FormulaExtendRepository extendRepository, IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(MS_FormulaDeleteCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                await _repository.RemoveRangeAsync(o => o.EnterpriseID == _identityService.GroupId && o.NumericalOrder == request.NumericalOrder);
                await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                await _extendRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2004141510170000102" && o.ChildType == "1709301424110000101" && o.ParentValue == request.UseEnterprise);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2004141510170000102" && o.ChildType == "1708231814060000101" && o.ParentValue == request.UseProduct);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
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

    public class MS_FormulaModifyHandler : IRequestHandler<MS_FormulaModifyCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaRepository _repository;
        IMS_FormulaDetailRepository _detailRepository;
        IMS_FormulaExtendRepository _extendRepository;
        NumericalOrderCreator _numericalOrderCreator;
        IBiz_Related _biz_RelatedRepository;
        public MS_FormulaModifyHandler(IIdentityService identityService, IMS_FormulaRepository repository, NumericalOrderCreator numericalOrderCreator, IMS_FormulaExtendRepository extendRepository, IMS_FormulaDetailRepository detailRepository, IBiz_Related biz_RelatedRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extendRepository = extendRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _biz_RelatedRepository = biz_RelatedRepository;
        }

        public async Task<Result> Handle(MS_FormulaModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                List<BIZ_Related> relateds = new List<BIZ_Related>();
                var domain = await _repository.GetAsync(request.NumericalOrder);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2004141510170000102" && o.ChildType == "1709301424110000101" && o.ParentValue == domain.UseEnterprise);
                await _biz_RelatedRepository.RemoveRangeAsync(o => o.RelatedType == "201610210104402102" && o.ParentType == "2004141510170000102" && o.ChildType == "1708231814060000101" && o.ParentValue == domain.UseProduct);

                domain.Update(request.DataDate, request.FormulaName, request.BaseQuantity, request.Remarks, request.PackageRemarks,
                    request.EffectiveBeginDate != null ? Convert.ToDateTime(request.EffectiveBeginDate) : Convert.ToDateTime("1000-01-01"),
                    request.EffectiveEndDate != null ? Convert.ToDateTime(request.EffectiveEndDate) : Convert.ToDateTime("9999-12-31"));

                //使用单位关系
                request.UseEnterpriseList?.ForEach(s =>
                {
                    relateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "2004141510170000102",
                        ParentValue = domain.UseEnterprise,
                        ChildType = "1709301424110000101",
                        ChildValue = s.Id,
                    });
                });
                //使用商品关系
                request.UseProductList?.ForEach(s =>
                {
                    relateds.Add(new BIZ_Related()
                    {
                        RelatedType = "201610210104402102",
                        ParentType = "2004141510170000102",
                        ParentValue = domain.UseProduct,
                        ChildType = "1708231814060000101",
                        ChildValue = s.Id,
                    });
                });
                //1增 2改 3删
                foreach (var item in request.Lines)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        domain.AddDetail(new MS_FormulaDetail()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            Guid = Guid.NewGuid(),
                            ProductID = item.ProductID,
                            StockType = "0",
                            FormulaTypeID = "0",
                            ProportionQuantity = item.ProportionQuantity,
                            Quantity = item.Quantity,
                            ModifiedDate = DateTime.Now,
                            RowNum = item.RowNum,
                            Cost = item.Cost,
                            UnitCost = item.UnitCost
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Details.Find(o => o.RecordID == item.RecordID);
                        obj.Update(item.ProductID, item.ProportionQuantity, item.Quantity, item.RowNum, item.UnitCost, item.Cost);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _detailRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                }
                foreach (var item in request.Extends)
                {
                    if (item.IsCreate)
                    {
                        var numericalOrderDetail = await this._numericalOrderCreator.CreateAsync();
                        domain.AddExtend(new MS_FormulaExtend()
                        {
                            NumericalOrder = domain.NumericalOrder,
                            Guid = Guid.NewGuid(),
                            ProductID = item.ProductID,
                            PackingID = item.PackingID ?? "0",
                            Quantity = item.Quantity,
                            IsUse = item.IsUse,
                            RowNum = item.RowNum,
                            ModifiedDate = DateTime.Now,
                        });
                        continue;
                    }
                    if (item.IsUpdate)
                    {
                        var obj = domain.Extends.Find(o => o.RecordID == item.RecordID);
                        obj.Update(item.ProductID, item.PackingID, item.Quantity, item.RowNum, item.IsUse);
                        continue;
                    }
                    if (item.IsDelete)
                    {
                        await _extendRepository.RemoveRangeAsync(o => o.RecordID == item.RecordID);
                        continue;
                    }
                }
                await _biz_RelatedRepository.AddRangeAsync(relateds);
                await _repository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _extendRepository.UnitOfWork.SaveChangesAsync();
                await _biz_RelatedRepository.UnitOfWork.SaveChangesAsync();
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
