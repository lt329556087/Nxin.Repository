using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Commands;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Dtos;
using FinanceManagement.ApiHost.Extension;
using FinanceManagement.Domain;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement
{
    public class FD_MarketingProductCostSettingHandler :
        IRequestHandler<FD_MarketingProductCostSettingCreateCommand, Result>,
        IRequestHandler<FD_MarketingProductCostSettingUpdateCommand, Result>,
        IRequestHandler<FD_MarketingProductCostSettingDeleteCommand, Result>
    {

        private readonly IFD_MarketingProductCostSettingRepository _marketingProductCostSettingRepository;
        private readonly IFD_MarketingProductCostSettingDetailRepository _marketingProductCostSettingDetailRepository;
        private readonly IIdentityService _identityService;
        private readonly NumericalOrderCreator _numericalOrderCreator;
        private readonly NumberCreator<Nxin_Qlw_BusinessContext> _numberCreator;
        private readonly Nxin_Qlw_BusinessContext _qlw_BusinessContext;
        private readonly IBiz_ReviewRepository _biz_ReviewRepository;

        public FD_MarketingProductCostSettingHandler(IFD_MarketingProductCostSettingRepository marketingProductCostSettingRepository,
            IFD_MarketingProductCostSettingDetailRepository marketingProductCostSettingDetailRepository,
            IIdentityService identityService, NumericalOrderCreator numericalOrderCreator, NumberCreator<Nxin_Qlw_BusinessContext> numberCreator,
            Nxin_Qlw_BusinessContext qlw_BusinessContext, IBiz_ReviewRepository biz_ReviewRepository)
        {
            _marketingProductCostSettingRepository = marketingProductCostSettingRepository;
            _marketingProductCostSettingDetailRepository = marketingProductCostSettingDetailRepository;
            _identityService = identityService;
            _numericalOrderCreator = numericalOrderCreator;
            _numberCreator = numberCreator;
            _qlw_BusinessContext = qlw_BusinessContext;
            _biz_ReviewRepository = biz_ReviewRepository;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(FD_MarketingProductCostSettingCreateCommand request, CancellationToken cancellationToken)
        {
            var result = new Result()
            {
                code = 0
            };

            result = CreateOrUpdateCommonChecker(request, true);

            if (result.code != 0)
            {
                return result;
            }

            var userId = _identityService.UserId;
            var nowDate = DateTime.Now;
            var numericalOrder = await _numericalOrderCreator.CreateAsync();

            if (!numericalOrder.IsValidNumer())
            {
                result.code = ErrorCode.Create.GetIntValue();
                result.msg = "保存失败，流水号创建失败";

                return result;
            }

            var number = _numberCreator.Create<FD_MarketingProductCostSetting>(
                request.DataDate,
                o => o.Number,
                o => EF.Functions.Like(o.Number.ToString(), $"{request.DataDate:yyyyMMdd}%"),
                o => o.AccountingEnterpriseID == request.AccountingEnterpriseID);

            var entity = new FD_MarketingProductCostSetting()
            {
                AccountingEnterpriseID = request.AccountingEnterpriseID,
                CreatedDate = nowDate,
                DataDate = request.DataDate,
                ModifiedDate = nowDate,
                Number = number,
                NumericalOrder = numericalOrder,
                OwnerID = userId,
                Remarks = string.IsNullOrEmpty(request.Remarks) ? string.Empty : request.Remarks,
            };

            var detailList = request.Details.Select(c => BindDetailEntityPropertyValues(new FD_MarketingProductCostSettingDetail()
            {
                NumericalOrder = numericalOrder
            }, c)).ToList();

            //制单人
            Biz_Review review = new Biz_Review(numericalOrder, _identityService.AppId, userId).SetMaking();
            await _biz_ReviewRepository.AddAsync(review);
            await _marketingProductCostSettingRepository.AddAsync(entity);
            await _marketingProductCostSettingDetailRepository.AddRangeAsync(detailList);
            await _marketingProductCostSettingRepository.UnitOfWork.SaveChangesAsync();

            result.data = numericalOrder;

            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(FD_MarketingProductCostSettingUpdateCommand request, CancellationToken cancellationToken)
        {
            var result = new Result()
            {
                code = 0
            };

            if (!request.NumericalOrder.IsValidNumer())
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存失败，提供的流水号错误";

                return result;
            }

            result = CreateOrUpdateCommonChecker(request, false);

            if (result.code != 0)
            {
                return result;
            }

            var entity = await _marketingProductCostSettingRepository.GetAsync(request.NumericalOrder, cancellationToken);
            var detailEntities = await _marketingProductCostSettingDetailRepository.GetByNumericalOrderListAsync(request.NumericalOrder, cancellationToken);

            if (entity == null || detailEntities == null || !detailEntities.Any())
            {
                result.code = ErrorCode.Update.GetIntValue();
                result.msg = "保存失败，数据或已被删除";
            }

            entity.Remarks = string.IsNullOrEmpty(request.Remarks) ? string.Empty : request.Remarks;

            var rqeuestDetails = request.Details;
            var requestProductIds = rqeuestDetails.Select(c => c.ProductId).ToList();
            var existsProductIds = detailEntities.Select(c => c.ProductId).ToList();
            var intersectProductIds = existsProductIds.Intersect(requestProductIds).ToList();
            var deleteProductIds = existsProductIds.Except(requestProductIds).ToList();
            var addProductIds = requestProductIds.Except(existsProductIds).ToList();
            var addList = rqeuestDetails.Where(c => addProductIds.Contains(c.ProductId))
                .Select(c => BindDetailEntityPropertyValues(new FD_MarketingProductCostSettingDetail()
                {
                    NumericalOrder = entity.NumericalOrder,
                }, c)).ToList();
            var deleteList = detailEntities.Where(c => deleteProductIds.Contains(c.ProductId)).ToList();
            var updateList = new List<FD_MarketingProductCostSettingDetail>();

            foreach (var productId in intersectProductIds)
            {
                var existsDetailEntity = detailEntities.FirstOrDefault(c => c.ProductId == productId);
                var rquestDetailItem = rqeuestDetails.FirstOrDefault(c => c.ProductId == productId);

                if (existsDetailEntity == null || rquestDetailItem == null)
                {
                    continue;
                }

                BindDetailEntityPropertyValues(existsDetailEntity, rquestDetailItem);
                updateList.Add(existsDetailEntity);
            }

            bool hasChange = false;

            _marketingProductCostSettingRepository.Update(entity);

            if (addList.Any())
            {
                await _marketingProductCostSettingDetailRepository.AddRangeAsync(addList);
                hasChange = true;
            }

            if (updateList.Any())
            {
                _marketingProductCostSettingDetailRepository.UpdateRange(updateList);
                hasChange = true;
            }

            if (deleteList.Any())
            {
                _marketingProductCostSettingDetailRepository.RemoveRange(deleteList);
                hasChange = true;
            }

            if (hasChange)
            {
                await _marketingProductCostSettingRepository.UnitOfWork.SaveChangesAsync();
            }

            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(FD_MarketingProductCostSettingDeleteCommand request, CancellationToken cancellationToken)
        {
            var result = new Result()
            {
                code = 0
            };

            if (!request.NumericalOrder.IsValidNumer())
            {
                result.code = ErrorCode.Delete.GetIntValue();
                result.msg = "删除失败，提供的流水号错误";

                return result;
            }

            var entity = await _marketingProductCostSettingRepository.GetAsync(request.NumericalOrder, cancellationToken);
            var detailEntities = await _marketingProductCostSettingDetailRepository.GetByNumericalOrderListAsync(request.NumericalOrder, cancellationToken);

            if (entity == null)
            {
                return result;
            }

            _marketingProductCostSettingRepository.Remove(entity);

            if (detailEntities != null && detailEntities.Any())
            {
                _marketingProductCostSettingDetailRepository.RemoveRange(detailEntities);
            }

            await _marketingProductCostSettingRepository.UnitOfWork.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// 添加和修改公共验证
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        private Result CreateOrUpdateCommonChecker(FD_MarketingProductCostSettingCreateOrUpdateCommand cmd, bool isAdd = true)
        {
            var errorCode = isAdd ? ErrorCode.Create.GetIntValue() : ErrorCode.Update.GetIntValue();
            var result = new Result()
            {
                code = errorCode
            };

            if (cmd == null)
            {
                result.msg = "保存失败，无效的数据";

                return result;
            }

            if (!cmd.AccountingEnterpriseID.IsValidNumer())
            {
                result.msg = "保存失败，账务单位不能为空";

                return result;
            }

            if (cmd.DataDate == default)
            {
                result.msg = "保存失败，日期格式不能为空";

                return result;
            }

            if (cmd.Details == null || !cmd.Details.Any())
            {
                result.msg = "保存失败，没有表体数据";

                return result;
            }

            var details = cmd.Details;

            if (details.Any(c => !c.ProductId.IsValidNumer()))
            {
                result.msg = "保存失败，商品代号不能为空";

                return result;
            }

            if (details.Select(c => c.ProductId).Distinct().Count() != details.Count)
            {
                result.msg = "保存失败，商品代号不能重复";

                return result;
            }

            result.code = 0;

            return result;
        }

        private FD_MarketingProductCostSettingDetail BindDetailEntityPropertyValues(FD_MarketingProductCostSettingDetail detailEntity, FD_MarketingProductCostSettingDetailInputDto detailInputDto)
        {
            if (detailEntity == null || detailInputDto == null)
            {
                return null;
            }

            detailEntity.CurrentCalcCost = detailInputDto.CurrentCalcCost;
            detailEntity.ForecastUnitCost = detailInputDto.ForecastUnitCost;
            detailEntity.ProductId = detailInputDto.ProductId;
            detailEntity.MeasureUnitNanme = detailInputDto.MeasureUnitName;
            detailEntity.ForecastAndCurrentDiff = detailInputDto.ForecastUnitCost - detailEntity.CurrentCalcCost;

            return detailEntity;
        }
    }
}
