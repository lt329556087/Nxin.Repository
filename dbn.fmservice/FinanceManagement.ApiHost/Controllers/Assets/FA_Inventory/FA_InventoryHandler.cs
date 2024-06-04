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
using Newtonsoft.Json;
using FinanceManagement.Common;
using Microsoft.Extensions.Logging;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;

namespace FinanceManagement.ApiHost.Controllers.FA_Inventory
{
    /// <summary>
    /// 增加
    /// </summary>
    public class FA_InventoryAddHandler : IRequestHandler<FA_InventoryAddCommand, Result>
    {
        IIdentityService _identityService;
        IFA_InventoryRepository _repository;
        IFA_InventoryDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IbsfileRepository _ibsfileRepository;
        FA_InventoryODataProvider _inventoryProdiver;
        private readonly ILogger<FA_InventoryAddHandler> _logger;
        private string _AppID = "2306081612060000150";//资产盘点菜单
        public FA_InventoryAddHandler(IIdentityService identityService, IFA_InventoryRepository repository, IFA_InventoryDetailRepository detailRepository, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator, IBiz_ReviewRepository biz_ReviewRepository
            ,IbsfileRepository ibsfileRepository, ILogger<FA_InventoryAddHandler> logger, FA_InventoryODataProvider inventoryProdiver)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _biz_ReviewRepository = biz_ReviewRepository;
            _ibsfileRepository = ibsfileRepository;
            _logger = logger;
            _inventoryProdiver= inventoryProdiver;
        }

        public async Task<Result> Handle(FA_InventoryAddCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    request.EnterpriseID = _identityService.EnterpriseId;
                }
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    result.msg = "EnterpriseID不能为空";
                    result.code = ErrorCode.RequestArgumentError.GetIntValue();
                    return result;
                }
                var existCardName = "";
                if (request.Lines?.Count > 0)
                {
                    var existCardIDList = _inventoryProdiver.GetInventoryDetailByDate(new Infrastructure.QlwCrossDbEntities.AssetscardSearch { EnterpriseID = request.EnterpriseID, DateDate = request.DataDate.ToString() });
                    if(existCardIDList?.Count > 0)
                    {
                        foreach(var item in request.Lines)
                        {
                            var filterList=existCardIDList.Where(x => x.id==item.CardID).ToList();
                            if (filterList?.Count > 0)
                            {
                                existCardName += item.AssetsName+",";
                            }
                        }
                        if (!string.IsNullOrEmpty(existCardName)) { result.msg = existCardName.TrimEnd(',') + "当天已做过盘点单";return result; }
                    }
                }
                if (string.IsNullOrEmpty(request.OwnerID))
                {
                    request.OwnerID = _identityService.UserId;
                }
                var number = _numberCreator.Create<Domain.FA_Inventory>(request.DataDate, o =>o.Number, o => o.Number.StartsWith(request.DataDate.ToString("yyyyMMdd")), o => o.EnterpriseID == request.EnterpriseID); 
                var numericalOrder = await _numericalOrderCreator.CreateAsync();
                var maxNum = 512;
                if (!string.IsNullOrEmpty(request.Remarks) && request.Remarks.Length > maxNum)
                {
                    request.Remarks=request.Remarks.Substring(0,maxNum);
                }
                var domain = new Domain.FA_Inventory()
                {
                    NumericalOrder = numericalOrder,
                    EnterpriseID = request.EnterpriseID,
                    DataDate = Convert.ToDateTime(request.DataDate),
                    Number = number.ToString(),
                    FAPlaceID = request.FAPlaceID,
                    UseStateID = request.UseStateID,
                    Remarks = request.Remarks,
                    OwnerID =string.IsNullOrEmpty(request.OwnerID)?"0": request.OwnerID,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                };
                if (request.Lines?.Count > 0)
                {
                    foreach (var item in request.Lines)
                    {
                        if (item.FileModels == null)
                        {
                            item.FileModels = new List<FMFileModel>();
                        }
                        var detial = new FA_InventoryDetail()
                        {
                            NumericalOrder = numericalOrder,
                            CardID = string.IsNullOrEmpty(item.CardID) ? "0" : item.CardID,
                            Quantity = item.Quantity,
                            InventoryQuantity = item.InventoryQuantity,
                            Remarks = item.Remarks,
                            FileName = string.Join(',', item.FileModels?.Select(s => s.FileName)),
                            PathUrl = string.Join(',', item.FileModels?.Select(s => s.PathUrl)),
                            ModifiedDate = DateTime.Now
                        };
                        domain.AddDetail(detial);
                    }
                }
                Biz_Review review = new Biz_Review(numericalOrder, _AppID, request.OwnerID).SetMaking();
                #region 附件old
                //List<FD_PaymentReceivables.UploadInfo> up = null;
                //if (!string.IsNullOrEmpty(request.UploadInfo))
                //{
                //    up = JsonConvert.DeserializeObject<List<FD_PaymentReceivables.UploadInfo>>(request.UploadInfo);
                //}
                //if (up != null)
                //{
                //    foreach (var item in up)
                //    {
                //        await _ibsfileRepository.AddAsync(new bsfile()
                //        {
                //            Guid = Guid.NewGuid(),
                //            EnterId = request.EnterpriseID,
                //            NumericalOrder = numericalOrder,
                //            Type = 2,
                //            FileName = item.FileName,
                //            PathUrl = item.PathUrl,
                //            OwnerID = request.OwnerID,
                //            Remarks = request.UploadInfo
                //        });
                //    }
                //    await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                //}
                #endregion
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
                _logger.LogError($"FA_InventoryAdd/Handle:异常{ex.ToString()}\n param={JsonConvert.SerializeObject(request)}");
            }
            return result;
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class FA_InventoryDeleteHandler : IRequestHandler<FA_InventoryDeleteCommand, Result>
    {
        IIdentityService _identityService;
        IFA_InventoryRepository _repository;
        IFA_InventoryDetailRepository _detailRepository;
        IBiz_ReviewRepository _biz_ReviewRepository;
        IbsfileRepository _ibsfileRepository;
        public FA_InventoryDeleteHandler(IIdentityService identityService, IFA_InventoryRepository repository, IFA_InventoryDetailRepository detailRepository, IBiz_ReviewRepository biz_ReviewRepository, IbsfileRepository ibsfileRepository)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _biz_ReviewRepository = biz_ReviewRepository;
            _ibsfileRepository = ibsfileRepository;
        }

        public async Task<Result> Handle(FA_InventoryDeleteCommand request, CancellationToken cancellationToken)
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
                    await _repository.RemoveRangeAsync(o => o.NumericalOrder == num && o.EnterpriseID == _identityService.EnterpriseId);
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    //await _ibsfileRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                    await _biz_ReviewRepository.RemoveRangeAsync(o => o.NumericalOrder == num);
                }

                await _repository.UnitOfWork.SaveChangesAsync();
                //await _ibsfileRepository.UnitOfWork.SaveChangesAsync();
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _biz_ReviewRepository.UnitOfWork.SaveChangesAsync();
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

    public class FA_InventoryModifyHandler : IRequestHandler<FA_InventoryModifyCommand, Result>
    {
        IIdentityService _identityService;
        IFA_InventoryRepository _repository;
        IFA_InventoryDetailRepository _detailRepository;
        NumericalOrderCreator _numericalOrderCreator;
        IbsfileRepository _ibsfileRepository;
        FA_InventoryODataProvider _inventoryProdiver;
        public FA_InventoryModifyHandler(IIdentityService identityService, IFA_InventoryRepository repository, IFA_InventoryDetailRepository detailRepository,
            NumericalOrderCreator numericalOrderCreator,
             IbsfileRepository ibsfileRepository, FA_InventoryODataProvider inventoryProdiver)
        {
            _identityService = identityService;
            _repository = repository;
            _detailRepository = detailRepository;
            _numericalOrderCreator = numericalOrderCreator;
            _ibsfileRepository = ibsfileRepository;
            _inventoryProdiver = inventoryProdiver;
        }

        public async Task<Result> Handle(FA_InventoryModifyCommand request, CancellationToken cancellationToken)
        {
            Result result = new Result();
            try
            {
                var domain = await _repository.GetAsync(request.NumericalOrder);
                
                if (domain == null)
                {
                    result.msg = "未查询到数据";
                    result.code = ErrorCode.NoContent.GetIntValue();
                    return result;
                }
                var existCardName = "";
                if (request.Lines?.Count > 0)
                {
                    var existCardIDList = _inventoryProdiver.GetInventoryDetailByDate(new Infrastructure.QlwCrossDbEntities.AssetscardSearch { EnterpriseID = request.EnterpriseID, DateDate = request.DataDate.ToString(), NumericalOrder = request.NumericalOrder });
                    if (existCardIDList?.Count > 0)
                    {
                        foreach (var item in request.Lines)
                        {
                            var filterList = existCardIDList.Where(x => x.id == item.CardID).ToList();
                            if (filterList?.Count > 0)
                            {
                                existCardName += item.AssetsName + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(existCardName)) { result.msg = existCardName.TrimEnd(',') + "当天已做过盘点单"; return result; }
                    }
                }
                var maxNum = 512;
                if (!string.IsNullOrEmpty(request.Remarks) && request.Remarks.Length > maxNum)
                {
                    request.Remarks = request.Remarks.Substring(0, maxNum);
                }
                domain.Update(request.DataDate, request.FAPlaceID, request.UseStateID, request.Remarks);
                var detailList = new List<FA_InventoryDetail>();                
                if (request.Lines?.Count > 0)
                {
                    //先删后增
                    await _detailRepository.RemoveRangeAsync(o => o.NumericalOrder == request.NumericalOrder);
                    foreach (var item in request.Lines)
                    {
                        if (item.FileModels == null)
                        {
                            item.FileModels = new List<FMFileModel>();
                        }
                        var detial = new FA_InventoryDetail()
                        {
                            NumericalOrder = request.NumericalOrder,
                            CardID = string.IsNullOrEmpty(item.CardID) ? "0" : item.CardID,
                            Quantity = item.Quantity,
                            InventoryQuantity = item.InventoryQuantity,
                            Remarks = item.Remarks,
                            FileName = string.Join(',', item.FileModels?.Select(s => s.FileName)),
                            PathUrl = string.Join(',', item.FileModels?.Select(s => s.PathUrl)),
                            ModifiedDate = DateTime.Now
                        };
                        detailList.Add(detial);
                    }
                    await _detailRepository.AddRangeAsync(detailList);
                }
               
               
                await _detailRepository.UnitOfWork.SaveChangesAsync();
                await _repository.UnitOfWork.SaveChangesAsync();
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
