using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Dtos;
using FinanceManagement.ApiHost.Extension;
using FinanceManagement.Common;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.EntityConfigurations.MarketingProductCostSettingManagement;
using Microsoft.EntityFrameworkCore;
using static FinanceManagement.ApiHost.Applications.Queries.FM_CashSweepODataProvider;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement
{
    public class FD_MarketingProductCostSettingODataProvider
    {
        private readonly QlwCrossDbContext _qlwCrossDbContext;
        private readonly IIdentityService _identityService;
        private readonly EnterprisePeriodUtil _enterprisePeriodUtil;
        public FD_MarketingProductCostSettingODataProvider(QlwCrossDbContext qlwCrossDbContext, IIdentityService identityService, EnterprisePeriodUtil enterprisePeriodUtil)
        {
            _qlwCrossDbContext = qlwCrossDbContext;
            _identityService = identityService;
            _enterprisePeriodUtil = enterprisePeriodUtil;
        }

        /// <summary>
        /// 列表数据(OData)
        /// </summary>
        /// <returns></returns>
        public IQueryable<FD_MarketingProductCostSettingListQueryModel> GetList()
        {
            var groupId = _identityService.GroupId;
            var query = _qlwCrossDbContext.FD_MarketingProductCostSettingListQueryModel.FromSqlInterpolated(@$"
SELECT
    CAST(mpcs.NumericalOrder AS CHAR) AS NumericalOrder,
    mpcs.DataDate AS DataDate,
    CAST(mpcs.Number AS CHAR) AS Number,
    CAST(mpcs.AccountingEnterpriseID AS CHAR) AS AccountingEnterpriseID,
    e.EnterpriseName AS AccountingEnterpriseName,
    CAST(mpcs.OwnerID AS CHAR) AS OwnerID,
    op.Name AS OwnerName,
    CAST(rev.CheckedByID AS CHAR) AS AuditUserID,
    rp.Name AS AuditUserName,
    rev.CreatedDate AS AuditDate,
    IF(rev.NumericalOrder>0,1,0) AS AuditStatus
FROM nxin_qlw_business.fd_marketingproductcostsetting AS mpcs
INNER JOIN qlw_nxin_com.biz_enterprise AS e ON e.EnterpriseID=mpcs.AccountingEnterpriseID
LEFT JOIN nxin_qlw_business.hr_person AS op ON mpcs.OwnerID=op.BO_ID
LEFT JOIN nxin_qlw_business.biz_reviwe AS rev ON rev.NumericalOrder=mpcs.NumericalOrder AND rev.CheckMark=16
LEFT JOIN nxin_qlw_business.hr_person AS rp ON rp.BO_ID=rev.CheckedByID
WHERE e.PID={groupId}").OrderByDescending(c => c.DataDate);

            return query;
        }

        /// <summary>
        /// 根据流水号获取营销商品成本设定
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        public async Task<FD_MarketingProductCostSettingOutputDto> GetByNumericalOrder(string numericalOrder)
        {
            if (!numericalOrder.IsValidNumer())
            {
                return null;
            }

            var groupId = _identityService.GroupId;
            var queryResult = await GetQuery(groupId).Where(c => c.NumericalOrder == numericalOrder).ToListAsync();

            if (!queryResult.Any())
            {
                return null;
            }

            var first = queryResult[0];
            var outputDto = new FD_MarketingProductCostSettingOutputDto()
            {
                AccountingEnterpriseID = first.AccountingEnterpriseID,
                AccountingEnterpriseName = first.AccountingEnterpriseName,
                DataDate = first.DataDate,
                Number = first.Number,
                NumericalOrder = first.NumericalOrder,
                OwnerID = first.OwnerID,
                OwnerName = first.OwnerName,
                Remarks = first.Remarks
            };

            outputDto.Details = queryResult.Select(c => new FD_MarketingProductCostSettingDetailOutputDto()
            {
                CurrentCalcCost = c.CurrentCalcCost,
                ForecastAndCurrentDiff = c.ForecastAndCurrentDiff,
                ForecastUnitCost = c.ForecastUnitCost,
                MeasureUnitName = c.MeasureUnitNanme,
                NumericalOrder = c.NumericalOrder,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                RecordID = c.RecordID,
            }).ToList();

            return outputDto;
        }

        /// <summary>
        /// 根据账务单位和日期获取信息表体信息
        /// </summary>
        /// <param name="accountingEnterpriseID"></param>
        /// <param name="dataDate"></param>
        /// <returns></returns>
        public async Task<Result> Load(string accountingEnterpriseID, DateTime dataDate)
        {
            var result = new Result()
            {
                code = ErrorCode.ServerBusy.GetIntValue(),
                msg = null
            };
            var groupId = _identityService.GroupId;

            if (!accountingEnterpriseID.IsValidNumer() || dataDate == default)
            {
                return null;
            }

            DateTime? startDate = null;
            DateTime? endDate = null;
            var enterprisePeriod = _enterprisePeriodUtil.GetEnterperisePeriod(accountingEnterpriseID, dataDate.Year, dataDate.Month);

            if (enterprisePeriod != null)
            {
                startDate = enterprisePeriod.StartDate;
                endDate = enterprisePeriod.EndDate;
            }
            else
            {
                startDate = new DateTime(dataDate.Year, dataDate.Month, 1);
                endDate = dataDate;
            }

            var existsQuery = GetQuery(groupId);
            var isExists = await existsQuery.Where(predicate: c => c.AccountingEnterpriseID == accountingEnterpriseID && c.DataDate >= startDate.Value && c.DataDate <= endDate.Value).AnyAsync();

            if (isExists)
            {
                result.msg = "当前会计期间已存在营销成本设定数据";

                return result;
            }

            result.code = 0;
            var queryList = await GetSaleSummaryCostInfoListAsync(groupId, accountingEnterpriseID, startDate.Value, endDate.Value);

            if (!queryList.Any())
            {
                return result;
            }

            var resultList = MakeFromSaleSummary(queryList);
            result.data = resultList;

            return result;
        }

        /// <summary>
        /// 根据流水号重新获取重新获取信息表体信息
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        public async Task<Result> Reload(string numericalOrder)
        {
            var result = new Result()
            {
                code = ErrorCode.ServerBusy.GetIntValue(),
                msg = null
            };
            var groupId = _identityService.GroupId;

            if (!numericalOrder.IsValidNumer())
            {
                return null;
            }

            var existsQuery = GetQuery(groupId).Where(c => c.NumericalOrder == numericalOrder);
            var existsList = await existsQuery.ToListAsync();

            if (!existsList.Any())
            {
                return null;
            }

            var first = existsList.First();
            DateTime? startDate = null;
            DateTime? endDate = null;

            var enterprisePeriod = _enterprisePeriodUtil.GetEnterperisePeriod(first.AccountingEnterpriseID, first.DataDate.Year, first.DataDate.Month);

            if (enterprisePeriod != null)
            {
                startDate = enterprisePeriod.StartDate;
                endDate = enterprisePeriod.EndDate;
            }
            else
            {
                startDate = new DateTime(first.DataDate.Year, first.DataDate.Month, 1);
                endDate = first.DataDate;
            }

            var existsDetailList = existsList.Select(c => new FD_MarketingProductCostSettingDetailOutputDto()
            {
                CurrentCalcCost = c.CurrentCalcCost,
                ForecastAndCurrentDiff = c.ForecastAndCurrentDiff,
                ForecastUnitCost = c.ForecastUnitCost,
                MeasureUnitName = c.MeasureUnitNanme,
                NumericalOrder = c.NumericalOrder,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                RecordID = c.RecordID
            }).ToList();

            result.code = 0;
            var saleSummaryQueryList = await GetSaleSummaryCostInfoListAsync(groupId, first.AccountingEnterpriseID, startDate.Value, endDate.Value);

            if (saleSummaryQueryList == null || !saleSummaryQueryList.Any())
            {
                result.data = existsDetailList;

                return result;
            }

            var resultList = new List<FD_MarketingProductCostSettingDetailOutputDto>();
            var saleSummaryList = MakeFromSaleSummary(saleSummaryQueryList);
            var existsProductIds = existsDetailList.Select(c => c.ProductId).Distinct().ToList();
            var saleSummaryProdutIds = saleSummaryList.Select(c => c.ProductId).Distinct().ToList();
            //新的没有的
            var deleteProductIds = existsProductIds.Except(saleSummaryProdutIds).ToList();
            //老的没有的
            var newProductIds = saleSummaryProdutIds.Except(existsProductIds).ToList();
            //都有的
            var intersectProductIds = existsProductIds.Intersect(saleSummaryProdutIds).ToList();

            //上面三个在重新构建返回数据时判断哪些数据在重新获取的销售汇总表中被删除，哪些新增的和哪些是两个都存在的（修改）的

            foreach (var productId in intersectProductIds)
            {
                var existsItem = existsDetailList.FirstOrDefault(c => c.ProductId == productId);
                var newItem = saleSummaryList.FirstOrDefault(c => c.ProductId == productId);

                if (existsItem == null || newItem == null)
                {
                    continue;
                }

                resultList.Add(new FD_MarketingProductCostSettingDetailOutputDto()
                {
                    CurrentCalcCost = newItem.CurrentCalcCost,
                    ForecastAndCurrentDiff = newItem.ForecastAndCurrentDiff,
                    ForecastUnitCost = newItem.ForecastUnitCost,
                    MeasureUnitName = existsItem.MeasureUnitName,
                    NumericalOrder = existsItem.NumericalOrder,
                    ProductId = productId,
                    ProductName = existsItem.ProductName,
                    RecordID = existsItem.RecordID
                });
            }

            resultList.AddRange(saleSummaryList.Where(c => newProductIds.Contains(c.ProductId)));
            result.data = resultList;

            return result;
        }

        /// <summary>
        /// 获取当前集团下所有的营销商品成本设定数据
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        public async Task<List<FD_MarketingProductCostSettingQueryModel>> GetFullList(string numericalOrder)
        {
            var groupId = _identityService.GroupId;
            var query = GetQuery(groupId);

            if (!string.IsNullOrEmpty(numericalOrder))
            {
                query = query.Where(c => c.NumericalOrder == numericalOrder);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 将销售汇总表数据封装为营销商品成本设定详情的格式
        /// </summary>
        /// <param name="list">销售成本汇总信息</param>
        /// <returns></returns>
        private List<FD_MarketingProductCostSettingDetailOutputDto> MakeFromSaleSummary(List<SalesSummaryCostInfoQueryModel> list)
        {
            if (list == null)
            {
                return null;
            }

            var groupedQueryList = list.GroupBy(c => c.ProductId);
            var resultList = new List<FD_MarketingProductCostSettingDetailOutputDto>();

            foreach (var item in groupedQueryList)
            {
                var key = item.Key;
                var childList = item.ToList();
                var fist = childList.FirstOrDefault();

                if (fist == null)
                {
                    continue;
                }

                var totalSalesCost = childList.Where(c => c.SalesCost.HasValue).Sum(c => c.SalesCost.Value);
                var totalQuantity = childList.Sum(c => c.Quantity);
                var currentCalcCost = totalQuantity > 0 ? totalSalesCost / totalQuantity : 0;

                resultList.Add(new FD_MarketingProductCostSettingDetailOutputDto()
                {
                    ProductId = key,
                    ProductName = fist.ProductName,
                    RecordID = 0,
                    CurrentCalcCost = currentCalcCost,
                    MeasureUnitName = fist.MeasureUnitName,
                    ForecastUnitCost = currentCalcCost,
                    ForecastAndCurrentDiff = 0
                });
            }

            return resultList;
        }

        /// <summary>
        /// 获取指定账务单位和指定时间区间的销售汇总表中成本相关的数据
        /// </summary>
        /// <param name="groupId">集团ID</param>
        /// <param name="accountingEnterpriseID">账务单位</param>
        /// <param name="periodStartDate">开始时间</param>
        /// <param name="periodEndDate">结束时间</param>
        /// <returns></returns>
        private async Task<List<SalesSummaryCostInfoQueryModel>> GetSaleSummaryCostInfoListAsync(string groupId, string accountingEnterpriseID, DateTime periodStartDate, DateTime periodEndDate)
        {

            var query = GetSaleSummaryCostInfoQuery(groupId, periodBeginDate: periodStartDate, periodEndDate: periodEndDate);
            var queryList = await query.Where(c => c.EnterpriseID == accountingEnterpriseID).ToListAsync();

            return queryList;
        }

        /// <summary>
        /// 获取集团下营销商品成本设定详情信息
        /// </summary>
        /// <param name="groupId">集团ID</param>
        /// <returns></returns>
        private IQueryable<FD_MarketingProductCostSettingQueryModel> GetQuery(string groupId)
        {
            var query = _qlwCrossDbContext.FD_MarketingProductCostSettingQueryModel.FromSqlInterpolated($@"
SELECT
    CAST(mpcs.NumericalOrder AS CHAR) AS NumericalOrder,
    mpcs.DataDate,
    CAST(mpcs.Number AS CHAR) AS Number,
    CAST(mpcs.AccountingEnterpriseID AS CHAR) AS AccountingEnterpriseID,
    e.EnterpriseName AS AccountingEnterpriseName,
    mpcsd.RecordID,
    CAST(mpcsd.ProductId AS CHAR) AS ProductId,
    pd.ProductName,
    mpcsd.MeasureUnitNanme,
    mpcsd.ForecastUnitCost,
    mpcsd.CurrentCalcCost,
    mpcsd.ForecastAndCurrentDiff,
    CAST(mpcs.OwnerID AS CHAR) AS OwnerID,
    op.Name AS OwnerName,
    CAST(rp.BO_ID AS CHAR) AS AuditUserID,
    rp.Name AS AuditUserName,
    rev.CreatedDate AS AuditDate,
    mpcs.CreatedDate,
    mpcs.ModifiedDate,
    mpcs.Remarks
FROM nxin_qlw_business.fd_marketingproductcostsetting AS mpcs
INNER JOIN nxin_qlw_business.fd_marketingproductcostsettingdetail AS mpcsd ON mpcs.NumericalOrder=mpcsd.NumericalOrder
INNER JOIN qlw_nxin_com.biz_enterprise AS e ON e.EnterpriseID=mpcs.AccountingEnterpriseID
LEFT JOIN nxin_qlw_business.biz_productdetail AS pd ON pd.ProductID=mpcsd.ProductId AND pd.EnterpriseID=mpcs.AccountingEnterpriseID
LEFT JOIN nxin_qlw_business.hr_person AS op ON mpcs.OwnerID=op.BO_ID
LEFT JOIN nxin_qlw_business.biz_reviwe AS rev ON rev.NumericalOrder=mpcs.NumericalOrder AND rev.CheckMark=16
LEFT JOIN nxin_qlw_business.hr_person AS rp ON rp.BO_ID=rev.CheckedByID
WHERE e.PID={groupId}");

            return query;
        }

        /// <summary>
        /// 获取集团下指定时间区间的销售汇总表中成本相关的数据
        /// </summary>
        /// <param name="groupId">集团ID</param>
        /// <param name="periodBeginDate">开始时间</param>
        /// <param name="periodEndDate">结束时间</param>
        /// <returns></returns>
        private IQueryable<SalesSummaryCostInfoQueryModel> GetSaleSummaryCostInfoQuery(string groupId, DateTime periodBeginDate, DateTime periodEndDate)
        {
            var query = _qlwCrossDbContext.SalesSummaryCostInfoQueryModel.FromSqlInterpolated(@$"
SELECT
     ss.DataDate,CAST(ssd.ProductID AS CHAR) AS ProductId,
     pd.ProductName,pd.MeasureUnitName,ssd.SalesCost,
     ssd.Quantity,
     CAST(ss.EnterpriseID AS CHAR) AS EnterpriseID
FROM nxin_qlw_business.sa_salessummary AS ss
INNER JOIN nxin_qlw_business.sa_salessummarydetail AS ssd ON ss.NumericalOrder=ssd.NumericalOrder
INNER JOIN qlw_nxin_com.biz_enterprise AS e ON ss.EnterpriseID=e.EnterpriseID
LEFT JOIN nxin_qlw_business.biz_productdetail AS pd ON pd.ProductID=ssd.ProductID
WHERE e.PID={groupId} AND ss.DataDate>={periodBeginDate} AND ss.DataDate<={periodEndDate}
");

            return query;
        }
    }
}
