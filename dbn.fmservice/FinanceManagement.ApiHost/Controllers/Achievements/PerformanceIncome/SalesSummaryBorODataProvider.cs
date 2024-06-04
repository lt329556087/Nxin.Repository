using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.PerformanceIncome;
using FinanceManagement.Common;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using MP.MiddlePlatform.Integration.Integaration;
using System.Reflection.Emit;
using Newtonsoft.Json;
using FinanceManagement.Common.MakeVoucherCommon;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Architecture.Seedwork.Core;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 波尔莱特 - 定制化需求
    /// </summary>
    public class SalesSummaryBorODataProvider
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        BIZ_DataDictODataProvider _dictProvider;
        FMBaseCommon _baseUnit;
        IHttpContextAccessor _httpContextAccessor;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        private readonly ILogger<PerformanceIncomeEntity> _logger;

        public SalesSummaryBorODataProvider(IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _identityservice = identityservice;
            _context = context;
            _treeModel = treeModel;
            _dictProvider = dictProvider;
            _httpContextAccessor = httpContextAccessor;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
            _logger = logger;
        }
        public string DynamicSql()
        {
            return @"it.FirstOrDefault().KeyUUID, 
                it.FirstOrDefault().Name,
                it.FirstOrDefault().EnterpriseID,
                it.FirstOrDefault().EnterpriseName,
                it.FirstOrDefault().org_en,
                it.FirstOrDefault().org_en1,
                it.FirstOrDefault().org_en2,
                it.FirstOrDefault().org_en3,
                it.FirstOrDefault().org_en4,
                it.FirstOrDefault().org_enid1,
                it.FirstOrDefault().org_enid2,
                it.FirstOrDefault().org_enid3,
                it.FirstOrDefault().org_enid4,
                it.FirstOrDefault().SortName,
                it.FirstOrDefault().bs_en1,
                it.FirstOrDefault().bs_en2,
                it.FirstOrDefault().bs_en3,
                it.FirstOrDefault().bs_en4,
                it.FirstOrDefault().bs_en5,
                it.FirstOrDefault().bs_enid1,
                it.FirstOrDefault().bs_enid2,
                it.FirstOrDefault().bs_enid3,
                it.FirstOrDefault().bs_enid4,
                it.FirstOrDefault().bs_enid5,
                it.FirstOrDefault().OrganizationSortID,
                it.FirstOrDefault().DataDate,
                it.FirstOrDefault().SortId,
                it.FirstOrDefault().ProductName,
                it.FirstOrDefault().ProductGroupName,
                it.FirstOrDefault().TaxRate,
                
                it.FirstOrDefault().ClassificationName,
                it.FirstOrDefault().pro_ifi1,                
                it.FirstOrDefault().pro_ifi2,
                it.FirstOrDefault().pro_ifi3,
                it.FirstOrDefault().AreaName,
                it.FirstOrDefault().area_name1,
                it.FirstOrDefault().area_name2,
                it.FirstOrDefault().area_name3,
                it.FirstOrDefault().DataMonth,
                it.FirstOrDefault().CustomerID, 
                it.FirstOrDefault().CustomerName,
                it.FirstOrDefault().MarketName,
                it.FirstOrDefault().market_1,
                it.FirstOrDefault().market_2,
                it.FirstOrDefault().market_3,
                it.FirstOrDefault().IsSum,
                it.FirstOrDefault().ProductID,
                it.FirstOrDefault().Rank,    
                it.FirstOrDefault().SalesAbstract,
                it.FirstOrDefault().SalesAbstractName,
                it.FirstOrDefault().ProductGroupID,
                it.FirstOrDefault().ClassificationID,
                it.FirstOrDefault().MarketId,
                it.FirstOrDefault().AreaId,
                it.FirstOrDefault().SalesmanID,
                it.FirstOrDefault().cAxis,
                it.FirstOrDefault().org_encAxis,
                it.FirstOrDefault().bs_encAxis,
                it.FirstOrDefault().NumericalOrder,
                it.FirstOrDefault().Number,
                it.FirstOrDefault().DataType,";
        }
        /// <summary>
        /// 获取销售汇总表数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public dynamic GetSalesSummaryData(SalesSummarySearch search)
        {
            if (string.IsNullOrEmpty(search.EnterpriseIds))
            {
                search.EnterpriseIds = "0";
            }
            //search.EnterpriseIds = "1564745,1564841,1579429,1635482,1649643,1649644,1649645,1649647,1649654,1649656,1649660,1649661,1649662,1649665,1649666,1649667,1649670,1649674,1649677,1649680,1649708,1664009,1687948,1715599,1731809,1736811,1770816,1786474,1786475,1798658,1812364,1812365,1812368,1862934,1869220,1887556,1891652,1908209,2119007,2390850,2488488,2529825,1345508302914,13768274246665,16436252259676,69827436510096,73083846959190,94159000791321,110393479681401,160888034113316,392765588122251,439884140970006,474220486710506,563211166749839,689770436088619,693675032631813,768774039576964,866472692097073,885514206174553,634086739144001641";
            var MaxRankData = _context.GetNumbers.FromSqlRaw($@"SELECT IFNULL(CONCAT(MAX(Rank)+2),0) AS MaxNumber FROM qlw_nxin_com.BIZ_ProductGroupClassification WHERE enterpriseid = {_identityservice.GroupId}").FirstOrDefault();
            var MaxRank = MaxRankData == null ? "1" : MaxRankData.MaxNumber;
            if (string.IsNullOrEmpty(MaxRank))
            {
                MaxRank = "1";
            }
            string sql = GetSalesSQL(search, MaxRank);

            var list = _context.SalesSummaryEntitiesDataSet.FromSqlRaw(sql).ToList();
            var enSort = EnterSortInfo(); //单位组织
            var maSort = MarketSortInfo("",string.Join(',',list.Select(m=>m.MarketId).Distinct()));//创业单元
            foreach (var item in list)
            {
                SetSortNameOrganizationName(enSort, maSort, item);
            }
            //数据过滤
            var tempList = OrgEnterFilter(search, list);
            list = tempList;
            var query = list.AsQueryable();

            // 构建分组条件
            var groupByFields = new List<string>();
            foreach (var item in search.SummaryType)
            {
                groupByFields.Add(item);
            }
            if (groupByFields.Count == 0)
            {
                groupByFields.Add("KeyUUID");
                search.SummaryType.Add("KeyUUID");
            }
            // 构建查询字符串
            var groupByExpression = string.Join(", ", groupByFields);
            #region 属性说明
            //AS 'Quantity', --销售数量
            //AS 'Packages', --销售件数
            //AS 'Discount', --现场折扣
            //AS 'DerateAmount', --销售折扣
            //AS 'PurchaseUnitCost',--含税销售净额
            //AS 'AmountTotal',--销售净额
            //AS 'UnitPriceTax',--预测销售成本
            //AS 'PurchaseTaxRate', --预测销售毛利
            //AS PurchaseCostExcludingTax, --实际销售成本
            //AS 'OperatingRevenue', --实际销售毛利
            #endregion
            var strColumn = @"new { 
                "+ DynamicSql() + @"
                it.FirstOrDefault().KeyCount,    
                it.Sum(m=>m.Quantity) AS Quantity,
                it.Sum(m=>m.UnitPriceTax) AS UnitPriceTax,
                it.Sum(m => m.Amount) AS Amount,
                it.Sum(m => m.Discount) AS Discount,
                it.Sum(m=>m.DerateAmount) AS DerateAmount,
                it.Sum(m=>m.AmountTotal) AS AmountTotal,
                it.Sum(m => m.PurchaseUnitCost) AS PurchaseUnitCost,
                it.Sum(m => m.PurchaseUnitCostTaxSum) AS PurchaseUnitCostTaxSum,
                it.Sum(m => m.OperatingRevenue) AS OperatingRevenue,
                it.Sum(m => m.NetSalesExcludingTax) AS NetSalesExcludingTax,
                it.Sum(m => m.PurchaseCostExcludingTax) AS PurchaseCostExcludingTax,
                it.Sum(m=>m.Packages) AS Packages,
                it.Sum(m=>m.PurchaseTaxRate) AS PurchaseTaxRate,
                }";

            // 使用 Dynamic Linq 库执行查询
            var result = query.GroupBy($"new ({groupByExpression})", "it").Select(strColumn).OrderBy(string.Join(',', search.SummaryType.Select(m => m)));

            var str = JsonConvert.SerializeObject(result);
            if (!string.IsNullOrEmpty(str))
            {
                var resultList = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(str);
                int KeyCount = 1;
                foreach (var item in resultList)
                {
                    item.KeyCount = KeyCount;
                    KeyCount++;
                }
                str = JsonConvert.SerializeObject(resultList);
                if (search.SummaryType.Count > 1)
                {
                    var tempSummary = "";
                    for (int i = 0; i < search.SummaryType.Count - 1; i++)
                    {
                        if (string.IsNullOrEmpty(tempSummary))
                        {
                            //获取倒数第二个汇总方式，永远不取倒数第一的数据
                            foreach (var item in search.SummaryType)
                            {
                                if (item == search.SummaryType.LastOrDefault())
                                {
                                    break;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tempSummary))
                                    {
                                        tempSummary = item;
                                    }
                                    else
                                    {
                                        tempSummary += "," + item;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var tempArry = tempSummary.Split(',');
                            tempSummary = "";
                            foreach (var item in tempArry)
                            {
                                if (item == tempArry.LastOrDefault())
                                {
                                    break;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tempSummary))
                                    {
                                        tempSummary = item;
                                    }
                                    else
                                    {
                                        tempSummary += "," + item;
                                    }
                                }
                            }
                        }
                        var tempstrColumn = strColumn;
                        //tempstrColumn = tempstrColumn.Replace("it.FirstOrDefault()." + search.SummaryType.LastOrDefault(), $"{search.SummaryType.LastOrDefault()} = 小计 ");
                        var tempResult = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(str).AsQueryable().GroupBy($"new ({tempSummary})", "it").Select(@"new { 
                        "+ DynamicSql() + @"
                        it.Sum(m=>m.Quantity) AS Quantity,
                        it.Sum(m=>m.UnitPriceTax) AS UnitPriceTax,
                        it.Sum(m => m.Amount) AS Amount,
                        it.Sum(m => m.Discount) AS Discount,
                        it.Sum(m=>m.DerateAmount) AS DerateAmount,
                        it.Sum(m=>m.AmountTotal) AS AmountTotal,
                        it.Sum(m => m.PurchaseUnitCost) AS PurchaseUnitCost,
                        it.Sum(m => m.PurchaseUnitCostTaxSum) AS PurchaseUnitCostTaxSum,
                        it.Sum(m => m.OperatingRevenue) AS OperatingRevenue,
                        it.Sum(m => m.NetSalesExcludingTax) AS NetSalesExcludingTax,
                        it.Sum(m => m.PurchaseCostExcludingTax) AS PurchaseCostExcludingTax,
                        it.Sum(m=>m.Packages) AS Packages,
                        it.LastOrDefault().KeyCount,    
                        it.Sum(m=>m.PurchaseTaxRate) AS PurchaseTaxRate,
                        }").OrderBy(string.Join(',', search.SummaryType.Select(m => m))).ToDynamicList();
                        var tempstr = JsonConvert.SerializeObject(tempResult);
                        if (!string.IsNullOrEmpty(tempstr))
                        {
                            var tempResultList = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(tempstr);
                            foreach (var item in tempResultList)
                            {
                                item.GetType().GetProperty(search.SummaryType[search.SummaryType.Count - 1 - i]).SetValue(item, "小计");
                                item.IsSum = 1;
                            }
                            resultList.AddRange(tempResultList);
                            //search.SummaryType.RemoveAt(search.SummaryType.Count - 1);
                        }
                    }
                }
                return resultList.OrderBy(m => m.KeyCount);
            }
            return result;
        }

        /// <summary>
        /// 单位组织，创业单元过滤
        /// </summary>
        /// <param name="search"></param>
        /// <param name="tempList"></param>
        /// <returns></returns>
        private static List<SalesSummaryEntity> OrgEnterFilter(SalesSummarySearch search, List<SalesSummaryEntity> tempList)
        {
            if (!string.IsNullOrEmpty(search.EnterSortIds))
            {
                var eSortWhere = search.EnterSortIds.Split(',').Select(e => Convert.ToInt64(e)).ToList();
                var TempArry = new List<SalesSummaryEntity>();
                foreach (var item in eSortWhere)
                {
                    TempArry.AddRange(tempList.Where(m => m.org_encAxis.Contains(item.ToString())).ToList());
                }
                tempList = TempArry;
            }

            if (!string.IsNullOrEmpty(search.MarketSortIds))
            {
                var mSortWhere = search.MarketSortIds.Split(',').Select(e => Convert.ToInt64(e)).ToList();
                var TempArry = new List<SalesSummaryEntity>();
                foreach (var item in mSortWhere)
                {
                    TempArry.AddRange(tempList.Where(m => m.bs_encAxis.Contains(item.ToString())).ToList());
                }
                tempList = TempArry;
            }

            return tempList;
        }

        /// <summary>
        /// 设置单位组织名称，创业单元
        /// </summary>
        /// <param name="enSort">单位组织</param>
        /// <param name="maSort">创业单元</param>
        /// <param name="item">要处理的数据</param>
        private void SetSortNameOrganizationName(List<SortInfo> enSort, List<SortInfo> maSort, SalesSummaryEntity item)
        {
            var endata = enSort.FirstOrDefault(m => m.SortId.ToString() == item.EnterpriseID);
            item.org_en = endata?.cfullname;
            item.org_encAxis = endata?.cAxis;
            if (string.IsNullOrEmpty(item.org_encAxis))
            {
                item.org_encAxis = "";
            }
            if (!string.IsNullOrEmpty(item.org_en) && item.org_en.Contains('/'))
            {
                var esplit = item.org_en.Split('/');
                if (!string.IsNullOrEmpty(endata.cAxis) && endata.cAxis.Substring(0,1) == "$")
                {
                    endata.cAxis = endata.cAxis.Remove(0,1);
                    endata.cAxis = endata.cAxis.Remove(endata.cAxis.Length - 1,1);
                }
                var esplitid = endata.cAxis.Split('$');
                for (int i = 0, n = Math.Min(esplit.Length, 4); i < n; i++)
                {
                    switch (i)
                    {
                        case 0:
                            item.org_en1 = esplit[i];
                            item.org_enid1 = esplitid[i];
                            break;
                        case 1:
                            item.org_en2 = esplit[i];
                            item.org_enid2 = esplitid[i];
                            break;
                        case 2:
                            item.org_en3 = esplit[i];
                            item.org_enid3 = esplitid[i];
                            break;
                        case 3:
                            item.org_en4 = esplit[i];
                            item.org_enid4 = esplitid[i];
                            break;
                    }
                }
            }

            var sortdata = maSort.FirstOrDefault(m => m.Id.ToString() == item.MarketId);
            item.SortName = sortdata?.cfullname;
            item.bs_encAxis = sortdata?.cAxis;
            item.OrganizationSortID = sortdata?.SortId.ToString();
            if (string.IsNullOrEmpty(item.bs_encAxis))
            {
                item.bs_encAxis = "";
            }
            if (!string.IsNullOrEmpty(item.SortName) && item.SortName.Contains('/'))
            {
                var msplit = item.SortName.Split('/');
                if (!string.IsNullOrEmpty(sortdata.cAxis) && sortdata.cAxis.Substring(0, 1) == "$")
                {
                    sortdata.cAxis = sortdata.cAxis.Remove(0,1);
                    sortdata.cAxis = sortdata.cAxis.Remove(sortdata.cAxis.Length - 1, 1);
                }
                var msplitid = sortdata.cAxis.Split('$');
                for (int i = 0, n = Math.Min(msplit.Length, 5); i < n; i++)
                {
                    switch (i)
                    {
                        case 0:
                            item.bs_en1 = msplit[i];
                            item.bs_enid1 = msplitid[i];
                            break;
                        case 1:
                            item.bs_en2 = msplit[i];
                            item.bs_enid2 = msplitid[i];
                            break;
                        case 2:
                            item.bs_en3 = msplit[i];
                            item.bs_enid3 = msplitid[i];
                            break;
                        case 3:
                            item.bs_en4 = msplit[i];
                            item.bs_enid4 = msplitid[i];
                            break;
                        case 4:
                            item.bs_en5 = msplit[i];
                            item.bs_enid5 = msplitid[i];
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 获取单位组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> EnterSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuju",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterpriseId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    InheritanceId = "636346361736274263",
                    ScopeData = new int[] { 1,2},
                    IsUse = 1,
                    IsTop = 1,
                    IsMaster = 0,
                    IsExpand = 1
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 获取部门组织/创业单元
        /// 波尔定制化
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> OrgSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuju",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterpriseId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    InheritanceId = "636181287875447931",
                    IsUse = 1,
                    IsTop = 0,
                    IsMaster = 1,
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 获取部门组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="MarketIds">部门id找组织</param>
        /// <returns></returns>
        public List<SortInfo> MarketSortInfo(string GroupId = "", string MarketIds = "")
        {
            if (string.IsNullOrEmpty(MarketIds) || MarketIds == "0")
            {
                MarketIds = "10";
            }
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.getorgusedinfo/1.0?open_req_src=nxin_shuju",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    Inheritance = "636181287875447931",
                    Id = MarketIds,
                    IsUse = 1,
                    IsMaster = 1,
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 获取销售汇总表 SQL
        /// 农信集团定制化SQL
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string GetSalesSQL(SalesSummarySearch search,string MaxRank)
        {
            string sql = "";
            {
                sql = $@"        SELECT '销售' DataType,CONCAT(IFNULL(s.NumericalOrder,0)) NumericalOrder,'' Number,'' SummaryType2Name,0  Rank,0 KeyCount,'' SummaryStr,0 IsSum,CONCAT(ss.CustomerID) CustomerID,sc.CustomerName,'0' SortId,'' SortName,'' bs_en1,'' bs_en2,'' bs_en3,'' bs_en4,'' bs_en5,'' org_en,'' org_en1,'' org_en2,'' org_en3,UUID() KeyUUID,ss.RecordId,CONCAT(s.EnterpriseId) AS EnterpriseID,en.EnterpriseName,CONCAT(ss.OrganizationSortID) OrganizationSortID,DATE_FORMAT(s.DataDate, '%Y-%m-%d') AS 'DataDate',
                   DATE_FORMAT(s.DataDate, '%Y-%m') AS 'DataMonth',CONCAT(ss.ProductId) ProductID,bp.ProductName,bgp.ProductGroupName,bpclass.cFullClassName AS ClassificationName,IFNULL(bpclass.cAxis,'') cAxis,CONCAT(ss.SalesAbstract) SalesAbstract,bd.cDictName SalesAbstractName,
                   CONCAT(bgp.ProductGroupID) ProductGroupID,CONCAT( bgp.ClassificationID) ClassificationID,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 1), '/', -1) AS pro_ifi1 , 
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 2), '/', -1) AS pro_ifi2 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 3), '/', -1) AS pro_ifi3 ,
                   IF(hp.Name IS NULL, '', hp.Name) NAME,
                   CONCAT(ss.SalesmanID) SalesmanID,
                   CONCAT(market.MarketId) MarketId,
                   market.cFullName MarketName,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(market.cFullName, '/', 1), '/', -1) AS market_1 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(market.cFullName, '/', 2), '/', -1) AS market_2 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(market.cFullName, '/', 3), '/', -1) AS market_3 ,
                   CONCAT(cusarea.AreaId) AreaId,
                   areas.cFullName AS AreaName,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 1), '/', -1) AS area_name1 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 2), '/', -1) AS area_name2 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 3), '/', -1) AS area_name3 ,
                   ss.TaxRate                                                         AS 'TaxRate',
                   ss.Amount                                                          AS 'Amount',
                   IFNULL(ss.Quantity,0)                                              AS 'Quantity', -- 销售数量
                   ss.Packages                                                        AS 'Packages', -- 销售件数
                   ROUND(ss.Discount, 2)                                              AS 'Discount', -- 现场折扣
                   ss.MonthDiscount + ss.YearDiscount		                          AS 'DerateAmount', -- 销售折扣
                   ss.AmountTotal - ss.MonthDiscount - ss.YearDiscount                AS 'PurchaseUnitCost',-- 含税销售净额
                   ss.AmountNet                                                       AS 'AmountTotal',-- 销售净额
                   ROUND(ss.Quantity * IFNULL(fcd.ForecastUnitCost,0),2)                                AS 'UnitPriceTax',-- 预测销售成本
                   ss.AmountNet - (ss.Quantity * IFNULL(fcd.ForecastUnitCost,0))               AS 'PurchaseTaxRate', -- 预测销售毛利
                   IFNULL(ss.SalesCost, 0.00)                                         AS PurchaseCostExcludingTax, -- 实际销售成本
                   ss.SalesProfit                                                     AS 'OperatingRevenue', -- 实际销售毛利
                   IFNULL(ss.Quantity, 0.00) AS 'PurchaseUnitCostTaxSum',
                    ss.AmountNet/(1+ ss.TaxRate)                                   AS 'NetSalesExcludingTax'
                    FROM nxin_qlw_business.sa_salessummary s
                    INNER JOIN nxin_qlw_business.sa_salessummarydetail ss ON s.numericalorder = ss.numericalorder
                    INNER JOIN qlw_nxin_com.biz_enterprise en ON en.enterpriseid = s.enterpriseid
                    INNER JOIN qlw_nxin_com.biz_product bp ON bp.productid = ss.productid
                    INNER JOIN qlw_nxin_com.biz_productgroup bgp ON bgp.ProductGroupID = bp.ProductGroupID
                    INNER JOIN qlw_nxin_com.biz_customer sc ON ss.CustomerID = sc.CustomerID
                    INNER JOIN qlw_nxin_com.bsdatadict AS bd ON bd.DictID = ss.SalesAbstract
                    LEFT JOIN qlw_nxin_com.biz_market market ON market.MarketId = ss.MarketId
                    LEFT JOIN qlw_nxin_com.biz_productgroupclassification bpclass ON bgp.ClassificationID= bpclass.ClassificationID 
                    LEFT JOIN qlw_nxin_com.biz_contacts cusarea ON cusarea.ContactsID = sc.ContactsID
                    LEFT JOIN qlw_nxin_com.bsarea areas ON areas.AreaId = cusarea.AreaId
                    LEFT JOIN nxin_qlw_business.hr_person AS hp ON ss.SalesmanID = hp.PersonID
                    left join nxin_qlw_business.FD_MarketingProductCostSetting fc on fc.AccountingEnterpriseID = s.Enterpriseid and LEFT(fc.DataDate,7) = LEFT(s.DataDate,7) 
                    left join nxin_qlw_business.FD_MarketingProductCostSettingDetail fcd on fcd.ProductID = ss.productid and fc.NumericalOrder = fcd.NumericalOrder
                    where s.enterpriseid in ({search.EnterpriseIds}) 
                    {((!string.IsNullOrEmpty(search.BeginDate) && !string.IsNullOrEmpty(search.EndDate)) ? @$"and  s.DataDate BETWEEN '{search.BeginDate}' AND '{search.EndDate}' " : "")}
                    {(!string.IsNullOrEmpty(search.SalesAbstracts) ? $@" and ss.SalesAbstract IN ({search.SalesAbstracts})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductIds) ? $@" and ss.ProductID IN ({search.ProductIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupIds) ? $@" and bgp.ProductGroupID IN ({search.ProductGroupIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupClassificationIds) ? $@" and bgp.ClassificationID IN ({search.ProductGroupClassificationIds})" : "")}
                    {(!string.IsNullOrEmpty(search.CustomerIds) ? $@" and ss.CustomerID IN ({search.CustomerIds})" : "")}
                    {(!string.IsNullOrEmpty(search.AreaId) ? $@" and cusarea.AreaId IN ({search.AreaId})" : "")}
                    {(!string.IsNullOrEmpty(search.MarketId) ? $@" and ss.MarketId IN ({search.MarketId})" : "")}
                    {(!string.IsNullOrEmpty(search.SalesmanID) ? $@" and ss.SalesmanID IN ({search.SalesmanID})" : "")}

                    {(search.IsGift != null ? $" and ss.Gift = {search.IsGift}" : "")}
                    ";
            }
            return sql;
        }
    }
}
