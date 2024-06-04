using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Commands;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MP.MiddlePlatform.Integration.Integaration;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FinanceManagement.ApiHost.Extension;
using NPOI.SS.Formula.Functions;

namespace FinanceManagement.ApiHost.Controllers.MS_FormulaProductPrice
{
    /// <summary>
    /// 增加
    /// </summary>
    public class MS_FormulaProductPriceAddHandler : IRequestHandler<MS_FormulaProductPriceAddCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaProductPriceRepository _repository;
        IMS_FormulaProductPriceDetailRepository _detailRepository;
        IMS_FormulaProductPriceExtRepository _extRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        HandleData _handleData = new HandleData();

        public MS_FormulaProductPriceAddHandler(IIdentityService identityService, IMS_FormulaProductPriceRepository repository, IMS_FormulaProductPriceDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IMS_FormulaProductPriceExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extRepository = extRepository;
        }

        public async Task<Result> Handle(MS_FormulaProductPriceAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(_identityService.GroupId) || _identityService.GroupId == "0")
                {
                    result.msg = "登录GroupId空";
                    result.code = ErrorCode.Create.GetIntValue();
                    return result;
                }
                result = _handleData.ValidData(request, true);
                if (result.code != 0)
                {
                    result.msg = "保存失败," + result.msg;
                    return result;
                }
                if (string.IsNullOrEmpty(request.GroupID))
                {
                    request.GroupID = _identityService.GroupId;
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                var number = _numberCreator.Create<Domain.MS_FormulaProductPrice>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.GroupID == request.GroupID);

                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var domain = new Domain.MS_FormulaProductPrice()
                {
                    DataDate = Convert.ToDateTime(request.DataDate),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    NumericalOrder = numericalOrder,
                    OwnerID = request.OwnerID,
                    GroupID = request.GroupID,
                    Number = number.ToString()
                };
                request.NumericalOrder = numericalOrder;
                domain.Lines = _handleData.GetDetailList(request);
                domain.ExtList = _handleData.GetExtList(request);
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
    public class MS_FormulaProductPriceDeleteHandler : IRequestHandler<MS_FormulaProductPriceDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaProductPriceRepository _repository;
        IMS_FormulaProductPriceDetailRepository _detailRepository;
        IMS_FormulaProductPriceExtRepository _extRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        
        public MS_FormulaProductPriceDeleteHandler(IIdentityService identityService, IMS_FormulaProductPriceRepository repository, IMS_FormulaProductPriceDetailRepository detailRepository,  IBiz_ReviewRepository biz_ReviewRepository, IMS_FormulaProductPriceExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _extRepository = extRepository;
        }
        public async Task<Result> Handle(MS_FormulaProductPriceDeleteCommand request, CancellationToken cancellationToken)
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
                    await _repository.RemoveRangeAsync(o => o.GroupID == _identityService.GroupId && o.NumericalOrder == num);
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
    public class MS_FormulaProductPriceModifyHandler : IRequestHandler<MS_FormulaProductPriceModifyCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaProductPriceRepository _repository;
        IMS_FormulaProductPriceDetailRepository _detailRepository;
        IMS_FormulaProductPriceExtRepository _extRepository;
        HandleData _handleData = new HandleData();

        public MS_FormulaProductPriceModifyHandler(IIdentityService identityService, IMS_FormulaProductPriceRepository repository, IMS_FormulaProductPriceDetailRepository detailRepository, IMS_FormulaProductPriceExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _extRepository = extRepository;
        }
        public async Task<Result> Handle(MS_FormulaProductPriceModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                result = _handleData.ValidData(request, true);
                if (result.code != 0)
                {
                    return result;
                }
                var numericalOrder = request.NumericalOrder;
                var domain = await _repository.GetAsync(numericalOrder);
                if (domain == null) { result.code = ErrorCode.Update.GetIntValue(); result.msg = "未查询到数据"; return result; }
                domain?.Update(request.DataDate);

                var detailList = _handleData.GetDetailList(request);
                var extList = _handleData.GetExtList(request);
                //先删后增
                await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
                await _extRepository.RemoveRangeAsync(o => o.NumericalOrder == numericalOrder);
               

                await _detailRepository.AddRangeAsync(detailList);
                await _extRepository.AddRangeAsync(extList);
                await _repository.UnitOfWork.SaveChangesAsync();
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
    public class MS_FormulaProductPriceListHandler : IRequestHandler<MS_FormulaProductPriceListCommand, Result>
    {
        IIdentityService _identityService;
        IMS_FormulaProductPriceRepository _repository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        HandleData _handleData = new HandleData();

        public MS_FormulaProductPriceListHandler(IIdentityService identityService, IMS_FormulaProductPriceRepository repository, IMS_FormulaProductPriceDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository, IMS_FormulaProductPriceExtRepository extRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        public async Task<Result> Handle(MS_FormulaProductPriceListCommand cmd, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if(cmd == null||cmd.List==null|| cmd.List.Count == 0) { result.msg = "无数据";return result;}
                var domainlist = new List<Domain.MS_FormulaProductPrice>();
                var reviewList = new List<Biz_Review>();
                foreach(var request in cmd.List)
                {
                    var number = _numberCreator.Create<Domain.MS_FormulaProductPrice>(request.DataDate, o => o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.GroupID == request.GroupID);

                    var numericalOrder = await _numericalOrderCreator.CreateAsync();                    
                    request.Number = number.ToString();
                    request.NumericalOrder = numericalOrder;
                    request.Lines.ForEach(p=>p.NumericalOrder=numericalOrder);
                    request.ExtList.ForEach(p=>p.NumericalOrder=numericalOrder);
                    domainlist.Add(request);
                    reviewList.Add( new Biz_Review(numericalOrder, _identityService.AppId, request.OwnerID).SetMaking());                    
                }

                await _repository.AddRangeAsync(domainlist);
                await _biz_ReviewRepository.AddRangeAsync(reviewList);

                await _repository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();

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
    
    public class HandleData
    {    
        #region 数据处理
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public Result ValidData(MS_FormulaProductPriceBaseCommand request, bool isAdd = true)
        {
            var errorCode = isAdd ? ErrorCode.Create.GetIntValue() : ErrorCode.Update.GetIntValue();
            var result = new Result()
            {
                code = errorCode
            };

            if (request == null)
            {
                result.msg = "参数空";
                return result;
            }
            if(!isAdd&&string.IsNullOrEmpty(request.NumericalOrder)) {
                result.msg = "NumericalOrder空";
                return result;
            }
            if (request.EnterpriseList == null || request.EnterpriseList.Count == 0)
            {
                result.msg = "EnterpriseList空";
                return result;
            }

            if (request.Lines == null || !request.Lines.Any())
            {
                result.msg = "没有表体数据";

                return result;
            }

            var details = request.Lines;

            if (details.Any(c => !c.ProductID.IsValidNumer()))
            {
                result.msg = "商品代号不能为空";

                return result;
            }

            if (details.Select(c => c.ProductID).Distinct().Count() != details.Count)
            {
                result.msg = "商品代号不能重复";

                return result;
            }

            result.code = 0;

            return result;
        }
        public List<MS_FormulaProductPriceDetail> GetDetailList(MS_FormulaProductPriceBaseCommand request)
        {
            var detailList = new List<MS_FormulaProductPriceDetail>();
            foreach (var item in request.Lines)
            {
                var detail = new MS_FormulaProductPriceDetail()
                {
                    NumericalOrder = request.NumericalOrder,
                    ProductID = item.ProductID,
                    Specification = item.Specification,
                    StandardPack = item.StandardPack,
                    MeasureUnit = item.MeasureUnit,
                    MarketPrice = item.MarketPrice,
                    Remarks = item.Remarks,
                    ModifiedDate = DateTime.Now
                };
                detailList.Add(detail);
            }
            return detailList;
        }
        public List<MS_FormulaProductPriceExt> GetExtList(MS_FormulaProductPriceBaseCommand request)
        {
            var extList = new List<MS_FormulaProductPriceExt>();
            foreach (var item in request.EnterpriseList)
            {
                var ext = new MS_FormulaProductPriceExt()
                {
                    NumericalOrder = request.NumericalOrder,
                    EnterpriseID = item,
                    ModifiedDate = DateTime.Now
                };
                extList.Add(ext);
            }
            return extList;
        }
        #endregion
    }
}
