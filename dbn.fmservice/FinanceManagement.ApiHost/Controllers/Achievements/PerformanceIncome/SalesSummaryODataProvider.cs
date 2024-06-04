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
    public class SalesSummaryODataProvider
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

        public SalesSummaryODataProvider(IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
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
            //追加 金融数据
            SetFinanceData(search, MaxRank, list);
            var enSort = EnterSortInfo(); //单位组织
            var maSort = MarketSortInfo();//创业单元
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
            var strColumn = @"new { 
                it.FirstOrDefault().KeyUUID, 
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
                it.Sum(m=>m.Quantity) AS Quantity,
                it.Sum(m=>m.Amount) AS UnitPriceTax,
                it.Sum(m => m.Amount) AS Amount,
                it.Sum(m => m.Discount) AS Discount,
                it.Sum(m=>m.DerateAmount) AS DerateAmount,
                it.Sum(m=>m.AmountTotal) AS AmountTotal,
                it.Sum(m => m.PurchaseUnitCost) AS PurchaseUnitCost,
                it.Sum(m => m.PurchaseUnitCostTaxSum) AS PurchaseUnitCostTaxSum,
                it.FirstOrDefault().PurchaseTaxRate,
                it.Sum(m => m.OperatingRevenue) AS OperatingRevenue,
                it.Sum(m => m.NetSalesExcludingTax) AS NetSalesExcludingTax,
                it.Sum(m => m.PurchaseCostExcludingTax) AS PurchaseCostExcludingTax,
                it.FirstOrDefault().ClassificationName,
                it.FirstOrDefault().pro_ifi1,                
                it.FirstOrDefault().pro_ifi2,
                it.FirstOrDefault().pro_ifi3,
                it.FirstOrDefault().AreaName,
                it.FirstOrDefault().area_name1,
                it.FirstOrDefault().area_name2,
                it.FirstOrDefault().area_name3,
                it.FirstOrDefault().DataMonth,
                it.FirstOrDefault().CustomerID, it.FirstOrDefault().CustomerName,
                it.FirstOrDefault().MarketName,
                it.FirstOrDefault().market_1,
                it.FirstOrDefault().market_2,
                it.FirstOrDefault().market_3,
                it.Sum(m=>m.Packages) AS Packages,
                it.FirstOrDefault().IsSum,
                it.FirstOrDefault().ProductID,
                it.FirstOrDefault().KeyCount,    
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
                it.FirstOrDefault().DataType,
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
                    //算均价
                    item.UnitPriceTax = item.Amount / (item.Quantity == 0 ? 1 : item.Quantity);
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
                        it.FirstOrDefault().KeyUUID, 
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
                        it.Sum(m=>m.Quantity) AS Quantity,
                        it.Sum(m=>m.Amount) AS UnitPriceTax,
                        it.Sum(m => m.Amount) AS Amount,
                        it.Sum(m => m.Discount) AS Discount,
                        it.Sum(m=>m.DerateAmount) AS DerateAmount,
                        it.Sum(m=>m.AmountTotal) AS AmountTotal,
                        it.Sum(m => m.PurchaseUnitCost) AS PurchaseUnitCost,
                        it.Sum(m => m.PurchaseUnitCostTaxSum) AS PurchaseUnitCostTaxSum,
                        it.FirstOrDefault().PurchaseTaxRate,
                        it.Sum(m => m.OperatingRevenue) AS OperatingRevenue,
                        it.Sum(m => m.NetSalesExcludingTax) AS NetSalesExcludingTax,
                        it.Sum(m => m.PurchaseCostExcludingTax) AS PurchaseCostExcludingTax,
                        it.FirstOrDefault().ClassificationName,
                        it.FirstOrDefault().pro_ifi1,                
                        it.FirstOrDefault().pro_ifi2,
                        it.FirstOrDefault().pro_ifi3,
                        it.FirstOrDefault().AreaName,
                        it.FirstOrDefault().area_name1,
                        it.FirstOrDefault().area_name2,
                        it.FirstOrDefault().area_name3,
                        it.FirstOrDefault().DataMonth,
                        it.FirstOrDefault().CustomerID, it.FirstOrDefault().CustomerName,
                        it.FirstOrDefault().MarketName,
                        it.FirstOrDefault().market_1,
                        it.FirstOrDefault().market_2,
                        it.FirstOrDefault().market_3,
                        it.Sum(m=>m.Packages) AS Packages,
                        it.FirstOrDefault().IsSum,                        
                        it.FirstOrDefault().ProductID,
                        it.LastOrDefault().KeyCount,    
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
                        it.FirstOrDefault().DataType,

                        }").OrderBy(string.Join(',', search.SummaryType.Select(m => m))).ToDynamicList();
                        var tempstr = JsonConvert.SerializeObject(tempResult);
                        if (!string.IsNullOrEmpty(tempstr))
                        {
                            var tempResultList = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(tempstr);
                            foreach (var item in tempResultList)
                            {
                                item.GetType().GetProperty(search.SummaryType[search.SummaryType.Count - 1 - i]).SetValue(item, "小计");
                                //算均价
                                item.UnitPriceTax = item.Amount / (item.Quantity == 0 ? 1 : item.Quantity);
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

        private void SetFinanceData(SalesSummarySearch search, string MaxRank, List<SalesSummaryEntity> list)
        {
            var finance = GetSalesFinanceList(search);
            foreach (var item in finance)
            {
                list.Add(new SalesSummaryEntity()
                {
                    Amount = item.Amount,
                    AmountTotal = item.Amount,
                    DerateAmount = 0,
                    Discount = 0,
                    IsSum = 0,
                    NetSalesExcludingTax = item.Amount / (1 + item.TaxRate),// 金额 / 1+ (税率)
                    OperatingRevenue = item.Amount / (1 + item.TaxRate) - item.GuarantorCost - item.CapitalCost,// 金额 / 1+ (税率) - 担保费 - 资金成本
                    Packages = 0,
                    PurchaseCostExcludingTax = item.GuarantorCost + item.CapitalCost,//担保费 + 资金成本
                    PurchaseUnitCost = 0,
                    PurchaseUnitCostTaxSum = 0,
                    Quantity = 0,
                    Rank = Convert.ToInt32(string.IsNullOrEmpty(MaxRank) ? "0" : MaxRank),
                    UnitPriceTax = 0,
                    TaxRate = item.TaxRate,
                    AreaName = item.AreaName,
                    area_name1 = item.Area_name1,
                    area_name2 = item.Area_name2,
                    area_name3 = item.Area_name3,
                    ClassificationName = item.ClassificationName,
                    CustomerID = item.CustomerID,
                    CustomerName = item.CustomerName,
                    DataDate = item.DataDate,
                    EnterpriseID = item.EnterpriseID,
                    DataMonth = item.DataMonth,
                    Name = "",
                    KeyUUID = item.KeyUUID,
                    OrganizationSortID = item.OrganizationSortID,
                    ProductGroupName = item.ProductGroupName,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    pro_ifi1 = item.Pro_ifi1,
                    pro_ifi2 = item.Pro_ifi2,
                    pro_ifi3 = item.Pro_ifi3,
                    SalesAbstract = item.BusinessAbstract,
                    SortId = "0",
                    SortName = "",
                    AreaId = item.AreaId,
                    ClassificationID = item.ClassificationID,
                    ProductGroupID = item.ProductID,
                    SalesAbstractName = item.AbstractName,
                    SalesmanID = item.SalesmanID,
                    cAxis = string.IsNullOrEmpty(item.cAxis) ? "" : item.cAxis,
                    bs_en1 = "",
                    bs_en2 = "",
                    bs_en3 = "",
                    bs_en4 = "",
                    bs_en5 = "",
                    bs_encAxis = "",
                    bs_enid1 = "",
                    bs_enid2 = "",
                    bs_enid3 = "",
                    bs_enid4 = "",
                    bs_enid5 = "",
                    EnterpriseName = "",
                    org_en = "",
                    org_en1 = "",
                    org_en2 = "",
                    org_en3 = "",
                    org_en4 = "",
                    org_encAxis = "",
                    org_enid1 = "",
                    org_enid2 = "",
                    org_enid3 = "",
                    org_enid4 = "",
                    market_1 = "",
                    market_2 = "",
                    market_3 = "",
                    MarketId = "",
                    MarketName = "",
                    PurchaseTaxRate = 0,
                    DataType = "金融",
                    Number = "",
                    NumericalOrder = item.NumericalOrder,
                });
            }
        }

        private void SetFinanceInnerPerformanceData(SalesSummarySearch search, string MaxRank, List<SalesSummaryEntity> list)
        {
            var finance = GetSalesInnerPerformanceFinanceList(search);
            foreach (var item in finance)
            {
                list.Add(new SalesSummaryEntity()
                {
                    Amount = item.Amount,
                    AmountTotal = item.Amount,
                    DerateAmount = 0,
                    Discount = 0,
                    IsSum = 0,
                    NetSalesExcludingTax = item.Amount / (1 + item.TaxRate),// 金额 / 1+ (税率)
                    OperatingRevenue = item.Amount / (1 + item.TaxRate) - item.GuarantorCost - item.CapitalCost,// 金额 / 1+ (税率) - 担保费 - 资金成本
                    Packages = 0,
                    PurchaseCostExcludingTax = item.GuarantorCost + item.CapitalCost,//担保费 + 资金成本
                    PurchaseUnitCost = 0,
                    PurchaseUnitCostTaxSum = 0,
                    Quantity = 0,
                    Rank = Convert.ToInt32(string.IsNullOrEmpty(MaxRank) ? "0" : MaxRank),
                    UnitPriceTax = 0,
                    TaxRate = item.TaxRate,
                    AreaName = item.AreaName,
                    area_name1 = item.Area_name1,
                    area_name2 = item.Area_name2,
                    area_name3 = item.Area_name3,
                    ClassificationName = item.ClassificationName,
                    CustomerID = item.CustomerID,
                    CustomerName = item.CustomerName,
                    DataDate = item.DataDate,
                    EnterpriseID = item.EnterpriseID,
                    DataMonth = item.DataMonth,
                    Name = "",
                    KeyUUID = item.KeyUUID,
                    OrganizationSortID = item.OrganizationSortID,
                    ProductGroupName = item.ProductGroupName,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    pro_ifi1 = item.Pro_ifi1,
                    pro_ifi2 = item.Pro_ifi2,
                    pro_ifi3 = item.Pro_ifi3,
                    SalesAbstract = item.BusinessAbstract,
                    SortId = "0",
                    SortName = "",
                    AreaId = item.AreaId,
                    ClassificationID = item.ClassificationID,
                    ProductGroupID = item.ProductID,
                    SalesAbstractName = item.AbstractName,
                    SalesmanID = item.SalesmanID,
                    cAxis = string.IsNullOrEmpty(item.cAxis) ? "" : item.cAxis,
                    bs_en1 = "",
                    bs_en2 = "",
                    bs_en3 = "",
                    bs_en4 = "",
                    bs_en5 = "",
                    bs_encAxis = "",
                    bs_enid1 = "",
                    bs_enid2 = "",
                    bs_enid3 = "",
                    bs_enid4 = "",
                    bs_enid5 = "",
                    EnterpriseName = "",
                    org_en = "",
                    org_en1 = "",
                    org_en2 = "",
                    org_en3 = "",
                    org_en4 = "",
                    org_encAxis = "",
                    org_enid1 = "",
                    org_enid2 = "",
                    org_enid3 = "",
                    org_enid4 = "",
                    market_1 = "",
                    market_2 = "",
                    market_3 = "",
                    MarketId = "",
                    MarketName = "",
                    PurchaseTaxRate = 0,
                    DataType = "金融",
                    Number = "",
                    NumericalOrder = item.NumericalOrder,
                });
            }
        }
        /// <summary>
        /// 获取销售汇总表数据-计算
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public dynamic GetSalesSummaryDataTest(SalesSummarySearch search)
        {
            if (string.IsNullOrEmpty(search.EnterpriseIds))
            {
                search.EnterpriseIds = "0";
            }
            //search.EnterpriseIds = "1564745,1564841,1579429,1635482,1649643,1649644,1649645,1649647,1649654,1649656,1649660,1649661,1649662,1649665,1649666,1649667,1649670,1649674,1649677,1649680,1649708,1664009,1687948,1715599,1731809,1736811,1770816,1786474,1786475,1798658,1812364,1812365,1812368,1862934,1869220,1887556,1891652,1908209,2119007,2390850,2488488,2529825,1345508302914,13768274246665,16436252259676,69827436510096,73083846959190,94159000791321,110393479681401,160888034113316,392765588122251,439884140970006,474220486710506,563211166749839,689770436088619,693675032631813,768774039576964,866472692097073,885514206174553,634086739144001641";

            //获取集团最大级次+2  页面跳转需要用到这个数据
            var MaxRankData = _context.GetNumbers.FromSqlRaw($@"SELECT IFNULL(CONCAT(MAX(Rank)+2),0) AS MaxNumber FROM qlw_nxin_com.BIZ_ProductGroupClassification WHERE enterpriseid = {_identityservice.GroupId}").FirstOrDefault();
            var MaxRank = MaxRankData == null ? "1" : MaxRankData.MaxNumber;
            if (string.IsNullOrEmpty(MaxRank))
            {
                MaxRank = "1";
            }
            string sql = GetSalesSQL(search, MaxRank);

            var list = _context.SalesSummaryEntitiesDataSet.FromSqlRaw(sql).ToList();
            //追加 金融数据
            SetFinanceData(search, MaxRank, list);
            var enSort = EnterSortInfo(); //单位组织 
            var maSort = MarketSortInfo();//创业单元
            foreach (var item in list)
            {
                SetSortNameOrganizationName(enSort, maSort, item);
            }
            //数据过滤
            var tempList = OrgEnterFilter(search, list);
            list = tempList;
            //绩效分配开始z
            //客户id 逗号分隔
            var cusArry = list.Select(m => string.IsNullOrEmpty(m.CustomerID) ? "0" : m.CustomerID).Distinct().ToList();
            List<CustomreService> CusPersons = new List<CustomreService>();
            //mysql in 语句 1000条限制
            if (cusArry.Count >999)
            {
                var cusCount = cusArry.Count / 1000 + 1;
                for (int i = 0; i < cusCount; i++)
                {
                    var cus = string.Join(',', cusArry.Skip(i * 1000).Take(1000));
                    //获取业务员数据
                    CusPersons.AddRange(CustomreServicesInfo(cus, search.EnterpriseIds));
                }
                CusPersons = CusPersons.Distinct().ToList();
            }
            else
            {
                var cus = string.Join(',', cusArry);
                //获取业务员数据
                CusPersons = CustomreServicesInfo(cus, search.EnterpriseIds);
            }
            //获取业务员 详细信息 （创业单元）
            var PersonDetails = ServicePersonSortsInfo();
            //获取分配方案
            var Solution = PerformanceSetInfo(search.EnterpriseIds, search.BeginDate, search.EndDate);
            //重构数据 
            var RefactorList = new List<SalesSummaryEntity>();
            foreach (var item in list)
            {
                var temp = CusPersons.Where(m => m.NumericalOrder.ToString() == item.CustomerID);
                //判断是否走 绩效分配方法
                //临时存储
                var TempList = new List<SalesSummaryEntity>();

                foreach (var pitem in temp)
                {
                    //获取最终的分配方案公式  只能命中一条方案
                    var percentage = Solution.FirstOrDefault(m => m.PersonnelType.ToString() == pitem.RoleType && m.SalesAbstract.ToString() == item.SalesAbstract && item.cAxis.Contains(m.ProductInfo.ToString()));
                    if (percentage != null)
                    {
                        // 使用正则表达式提取数字部分
                        string pattern = @"\d+(\.\d+)?"; // 匹配整数部分和可选的小数部分
                        Match match = Regex.Match(percentage.AllocationFormula, pattern);
                        if (match.Success)
                        {
                            string numberString = match.Value;
                            double number = double.Parse(numberString);
                            var pDetail = PersonDetails.FirstOrDefault(m => m.PersonId == pitem.PersonID);
                            if (pDetail == null)
                            {
                                pDetail = new ServicePersonSort();
                            }
                            //设置组织名称
                            item.OrganizationSortID = pDetail.SortID.ToString();
                            SetSortNameOrganizationName(enSort, maSort, item);
                            //同类型角色总数
                            int RoleCount = temp.Count(m => m.RoleType == pitem.RoleType);
                            if (RoleCount == 0)
                            {
                                RoleCount = 1;
                            }
                            TempList.Add(new SalesSummaryEntity()
                            {
                                Amount = item.Amount * number / RoleCount,
                                AmountTotal = item.AmountTotal * number / RoleCount,
                                DerateAmount = item.DerateAmount * number / RoleCount,
                                Discount = item.Discount * number / RoleCount,
                                IsSum = 0,
                                KeyCount = item.KeyCount,
                                NetSalesExcludingTax = item.NetSalesExcludingTax * number / RoleCount,
                                OperatingRevenue = item.OperatingRevenue * number / RoleCount,
                                Packages = item.Packages * number / RoleCount,
                                PurchaseCostExcludingTax = item.PurchaseCostExcludingTax * number / RoleCount,
                                PurchaseTaxRate = item.PurchaseTaxRate * number / RoleCount,
                                PurchaseUnitCost = item.PurchaseUnitCost * number / RoleCount,
                                PurchaseUnitCostTaxSum = item.PurchaseUnitCostTaxSum * number / RoleCount,
                                Quantity = item.Quantity * number / RoleCount,
                                Rank = item.Rank,
                                UnitPriceTax = item.UnitPriceTax,
                                TaxRate = item.TaxRate * number / RoleCount,
                                AreaName = item.AreaName,
                                area_name1 = item.area_name1,
                                area_name2 = item.area_name2,
                                area_name3 = item.area_name3,
                                cAxis = string.IsNullOrEmpty(item.cAxis) ? "" : item.cAxis,
                                bs_en1 = item.bs_en1,
                                bs_en2 = item.bs_en2,
                                bs_en3 = item.bs_en3,
                                bs_en4 = item.bs_en4,
                                bs_en5 = item.bs_en5,
                                ClassificationName = item.ClassificationName,
                                CustomerID = item.CustomerID,
                                CustomerName = item.CustomerName,
                                DataDate = item.DataDate,
                                EnterpriseID = item.EnterpriseID,
                                DataMonth = item.DataMonth,
                                EnterpriseName = item.EnterpriseName,
                                org_en = item.org_en,
                                KeyUUID = item.KeyUUID,
                                MarketName = item.MarketName,
                                market_1 = item.market_1,
                                market_2 = item.market_2,
                                market_3 = item.market_3,
                                Name = pitem.Name,
                                OrganizationSortID = item.OrganizationSortID,
                                org_en1 = item.org_en1,
                                org_en2 = item.org_en2,
                                org_en3 = item.org_en3,
                                org_en4 = item.org_en4,
                                ProductGroupName = item.ProductGroupName,
                                ProductID = item.ProductID,
                                ProductName = item.ProductName,
                                pro_ifi1 = item.pro_ifi1,
                                pro_ifi2 = item.pro_ifi2,
                                pro_ifi3 = item.pro_ifi3,
                                SalesAbstract = item.SalesAbstract,
                                SortId = item.SortId,
                                SortName = item.SortName,
                                MarketId = item.MarketId,
                                AreaId = item.AreaId,
                                ClassificationID = item.ClassificationID,
                                ProductGroupID = item.ProductID,
                                SalesAbstractName = item.SalesAbstractName,
                                SalesmanID = item.SalesmanID,
                                bs_encAxis = item.bs_encAxis,
                                bs_enid1 = item.bs_enid1,
                                bs_enid2 = item.bs_enid2,
                                bs_enid3 = item.bs_enid3,
                                bs_enid4 = item.bs_enid4,
                                bs_enid5 = item.bs_enid5,
                                org_encAxis = item.org_encAxis,
                                org_enid1 = item.org_enid1,
                                org_enid2 = item.org_enid2,
                                org_enid3 = item.org_enid3,
                                org_enid4 = item.org_enid4,
                                NumericalOrder = item.NumericalOrder,
                                Number = item.Number,
                                DataType = item.DataType,
                                SummaryType2Name = item.SummaryType2Name,
                            });
                        }
                    }
                }
                //不符合 分配 直接将初始数据 填充进去
                if (TempList.Count == 0)
                {
                    SetSortNameOrganizationName(enSort, maSort, item);
                    RefactorList.Add(item);
                }
                else
                {
                    if (item.AmountTotal != TempList.Sum(m => m.AmountTotal)
                        || item.Amount != TempList.Sum(m => m.Amount)
                        || item.DerateAmount != TempList.Sum(m => m.DerateAmount)
                        || item.Discount != TempList.Sum(m => m.Discount)
                        || item.NetSalesExcludingTax != TempList.Sum(m => m.NetSalesExcludingTax)
                        || item.OperatingRevenue != TempList.Sum(m => m.OperatingRevenue)
                        || item.Packages != TempList.Sum(m => m.Packages)
                        || item.PurchaseCostExcludingTax != TempList.Sum(m => m.PurchaseCostExcludingTax)
                        || item.PurchaseTaxRate != TempList.Sum(m => m.PurchaseTaxRate)
                        || item.PurchaseUnitCost != TempList.Sum(m => m.PurchaseUnitCost)
                        || item.PurchaseUnitCostTaxSum != TempList.Sum(m => m.PurchaseUnitCostTaxSum)
                        || item.Quantity != TempList.Sum(m => m.Quantity)
                        || item.TaxRate != TempList.Sum(m => m.TaxRate)
                        )
                    {
                        TempList.Add(new SalesSummaryEntity()
                        {
                            Amount = item.Amount - TempList.Sum(m => m.Amount),
                            AmountTotal = item.AmountTotal - TempList.Sum(m => m.AmountTotal),
                            DerateAmount = item.DerateAmount - TempList.Sum(m => m.DerateAmount),
                            Discount = item.Discount - TempList.Sum(m => m.Discount),
                            IsSum = 0,
                            KeyCount = item.KeyCount,
                            NetSalesExcludingTax = item.NetSalesExcludingTax - TempList.Sum(m => m.NetSalesExcludingTax),
                            OperatingRevenue = item.OperatingRevenue - TempList.Sum(m => m.OperatingRevenue),
                            Packages = item.Packages - TempList.Sum(m => m.Packages),
                            PurchaseCostExcludingTax = item.PurchaseCostExcludingTax - TempList.Sum(m => m.PurchaseCostExcludingTax),
                            PurchaseTaxRate = item.PurchaseTaxRate - TempList.Sum(m => m.PurchaseTaxRate),
                            PurchaseUnitCost = item.PurchaseUnitCost - TempList.Sum(m => m.PurchaseUnitCost),
                            PurchaseUnitCostTaxSum = item.PurchaseUnitCostTaxSum - TempList.Sum(m => m.PurchaseUnitCostTaxSum),
                            Quantity = item.Quantity - TempList.Sum(m => m.Quantity),
                            Rank = item.Rank,
                            UnitPriceTax = item.UnitPriceTax,
                            TaxRate = item.TaxRate - TempList.Sum(m => m.TaxRate),
                            AreaName = item.AreaName,
                            area_name1 = item.area_name1,
                            area_name2 = item.area_name2,
                            area_name3 = item.area_name3,
                            cAxis = string.IsNullOrEmpty(item.cAxis) ? "" : item.cAxis,
                            bs_en1 = item.bs_en1,
                            bs_en2 = item.bs_en2,
                            bs_en3 = item.bs_en3,
                            bs_en4 = item.bs_en4,
                            bs_en5 = item.bs_en5,
                            ClassificationName = item.ClassificationName,
                            CustomerID = item.CustomerID,
                            CustomerName = item.CustomerName,
                            DataDate = item.DataDate,
                            EnterpriseID = item.EnterpriseID,
                            DataMonth = item.DataMonth,
                            EnterpriseName = item.EnterpriseName,
                            org_en = item.org_en,
                            KeyUUID = item.KeyUUID,
                            MarketName = item.MarketName,
                            market_1 = item.market_1,
                            market_2 = item.market_2,
                            market_3 = item.market_3,
                            Name = "",
                            OrganizationSortID = "0",
                            org_en1 = item.org_en1,
                            org_en2 = item.org_en2,
                            org_en3 = item.org_en3,
                            org_en4 = item.org_en4,
                            ProductGroupName = item.ProductGroupName,
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            pro_ifi1 = item.pro_ifi1,
                            pro_ifi2 = item.pro_ifi2,
                            pro_ifi3 = item.pro_ifi3,
                            SalesAbstract = item.SalesAbstract,
                            SortId = item.SortId,
                            SortName = "",
                            MarketId = item.MarketId,
                            AreaId = item.AreaId,
                            ClassificationID = item.ClassificationID,
                            ProductGroupID = item.ProductID,
                            SalesAbstractName = item.SalesAbstractName,
                            SalesmanID = item.SalesmanID,
                            bs_encAxis = item.bs_encAxis,
                            bs_enid1 = item.bs_enid1,
                            bs_enid2 = item.bs_enid2,
                            bs_enid3 = item.bs_enid3,
                            bs_enid4 = item.bs_enid4,
                            bs_enid5 = item.bs_enid5,
                            org_encAxis = item.org_encAxis,
                            org_enid1 = item.org_enid1,
                            org_enid2 = item.org_enid2,
                            org_enid3 = item.org_enid3,
                            org_enid4 = item.org_enid4,
                            DataType = item.DataType,
                            Number  = item.Number,
                            SummaryType2Name = item.SummaryType2Name,
                            NumericalOrder = item.NumericalOrder,
                        });
                    }
                    if (item.AmountTotal != TempList.Sum(m => m.AmountTotal))
                    {
                        _logger.LogInformation($"item:{item.AmountTotal},TempList:{TempList.Sum(m => m.AmountTotal)}--------一次性流水:{item.KeyUUID}");
                    }
                    RefactorList.AddRange(TempList);
                }
            };
            list = RefactorList.OrderBy(m=>m.KeyCount).ToList();

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
            var strColumn = @"new { 
                it.FirstOrDefault().KeyUUID, 
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
                it.Sum(m=>m.Quantity) AS Quantity,
                it.Sum(m=>m.Amount) AS UnitPriceTax,
                it.Sum(m => m.Amount) AS Amount,
                it.Sum(m => m.Discount) AS Discount,
                it.Sum(m=>m.DerateAmount) AS DerateAmount,
                it.Sum(m=>m.AmountTotal) AS AmountTotal,
                it.Sum(m => m.PurchaseUnitCost) AS PurchaseUnitCost,
                it.Sum(m => m.PurchaseUnitCostTaxSum) AS PurchaseUnitCostTaxSum,
                it.FirstOrDefault().PurchaseTaxRate,
                it.Sum(m => m.OperatingRevenue) AS OperatingRevenue,
                it.Sum(m => m.NetSalesExcludingTax) AS NetSalesExcludingTax,
                it.Sum(m => m.PurchaseCostExcludingTax) AS PurchaseCostExcludingTax,
                it.FirstOrDefault().ClassificationName,
                it.FirstOrDefault().pro_ifi1,                
                it.FirstOrDefault().pro_ifi2,
                it.FirstOrDefault().pro_ifi3,
                it.FirstOrDefault().AreaName,
                it.FirstOrDefault().area_name1,
                it.FirstOrDefault().area_name2,
                it.FirstOrDefault().area_name3,
                it.FirstOrDefault().DataMonth,
                it.FirstOrDefault().CustomerID, it.FirstOrDefault().CustomerName,
                it.FirstOrDefault().MarketName,
                it.FirstOrDefault().market_1,
                it.FirstOrDefault().market_2,
                it.FirstOrDefault().market_3,
                it.Sum(m=>m.Packages) AS Packages,
                it.FirstOrDefault().IsSum,
                it.FirstOrDefault().ProductID,
                it.FirstOrDefault().KeyCount,    
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
                it.FirstOrDefault().DataType,
                }";
            var result = query.GroupBy($"new ({groupByExpression})", "it").Select(strColumn).OrderBy(string.Join(',', search.SummaryType.Select(m => m)));
            //获取所有 客户 准备关联 客户档案 重组数据结构
            var customers = string.Join(",", result.ToDynamicList().Select(m => m.CustomerID));

            var str = JsonConvert.SerializeObject(result);
            if (!string.IsNullOrEmpty(str))
            {
                var resultList = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(str);
                int KeyCount = 1;
                foreach (var item in resultList)
                {
                    item.KeyCount = KeyCount;
                    //算均价
                    item.UnitPriceTax = item.Amount / (item.Quantity == 0 ? 1 : item.Quantity);
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
                        it.FirstOrDefault().KeyUUID, 
                        it.FirstOrDefault().Name,
                        it.FirstOrDefault().EnterpriseID,
                        it.FirstOrDefault().EnterpriseName,
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
                        it.Sum(m=>m.Quantity) AS Quantity,
                        it.Sum(m=>m.Amount) AS UnitPriceTax,
                        it.Sum(m => m.Amount) AS Amount,
                        it.Sum(m => m.Discount) AS Discount,
                        it.Sum(m=>m.DerateAmount) AS DerateAmount,
                        it.Sum(m=>m.AmountTotal) AS AmountTotal,
                        it.Sum(m => m.PurchaseUnitCost) AS PurchaseUnitCost,
                        it.Sum(m => m.PurchaseUnitCostTaxSum) AS PurchaseUnitCostTaxSum,
                        it.FirstOrDefault().PurchaseTaxRate,
                        it.Sum(m => m.OperatingRevenue) AS OperatingRevenue,
                        it.Sum(m => m.NetSalesExcludingTax) AS NetSalesExcludingTax,
                        it.Sum(m => m.PurchaseCostExcludingTax) AS PurchaseCostExcludingTax,
                        it.FirstOrDefault().ClassificationName,
                        it.FirstOrDefault().pro_ifi1,                
                        it.FirstOrDefault().pro_ifi2,
                        it.FirstOrDefault().pro_ifi3,
                        it.FirstOrDefault().AreaName,
                        it.FirstOrDefault().area_name1,
                        it.FirstOrDefault().area_name2,
                        it.FirstOrDefault().area_name3,
                        it.FirstOrDefault().DataMonth,
                        it.FirstOrDefault().CustomerID, it.FirstOrDefault().CustomerName,
                        it.FirstOrDefault().MarketName,
                        it.FirstOrDefault().market_1,
                        it.FirstOrDefault().market_2,
                        it.FirstOrDefault().market_3,
                        it.Sum(m=>m.Packages) AS Packages,
                        it.FirstOrDefault().IsSum,                        
                        it.FirstOrDefault().ProductID,
                        it.LastOrDefault().KeyCount,    
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
                        it.FirstOrDefault().DataType,
                        }").OrderBy(string.Join(',', search.SummaryType.Select(m => m))).ToDynamicList();
                        var tempstr = JsonConvert.SerializeObject(tempResult);
                        if (!string.IsNullOrEmpty(tempstr))
                        {
                            var tempResultList = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(tempstr);
                            foreach (var item in tempResultList)
                            {
                                item.GetType().GetProperty(search.SummaryType[search.SummaryType.Count - 1 - i]).SetValue(item, "小计");
                                //算均价
                                item.UnitPriceTax = item.Amount / (item.Quantity == 0 ? 1 : item.Quantity);
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

            var sortdata = maSort.FirstOrDefault(m => m.SortId.ToString() == item.OrganizationSortID);
            item.SortName = sortdata?.cfullname;
            item.bs_encAxis = sortdata?.cAxis;
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
        /// 获取销售汇总表 SQL
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<SalesFinanceEntity> GetSalesFinanceList(SalesSummarySearch search)
        {
            string sql = $@"SELECT DISTINCT CONCAT(s.NumericalOrder) NumericalOrder,'' SummaryType2Name, UUID() KeyUUID, CONCAT(s.EnterpriseID) AS 'EnterpriseID',
            DATE_FORMAT(DataDate,'%Y-%m-%d') AS 'DataDate',DATE_FORMAT(s.DataDate, '%Y-%m') AS 'DataMonth',CONCAT(BusinessAbstract) BusinessAbstract,bd.cDictName AS 'AbstractName',CONCAT(sc.CustomerID) CustomerID,sc.CustomerName AS 'CustomerName',
            CONCAT(s.SalesmanID) SalesmanID,IF(hp.Name IS NULL, '', hp.Name) AS 'SalesmanName',bpclass.cAxis,
            CONCAT(cusarea.AreaId) AreaId,
            areas.cFullName AS AreaName,
            SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 1), '/', -1) AS Area_name1 ,
            SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 2), '/', -1) AS Area_name2 ,
            SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 3), '/', -1) AS Area_name3 ,
            CONCAT(bs.SortID) AS 'OrganizationSortID',CONCAT(bp.ProductID) ProductID,CONCAT(bpg.ProductGroupID) ProductGroupID,bpg.ProductGroupName,bp.ProductName AS 'ProductName',
            CONCAT( bpg.ClassificationID) ClassificationID,
            bpclass.cFullClassName ClassificationName,
            SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 1), '/', -1) AS Pro_ifi1 , 
            SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 2), '/', -1) AS Pro_ifi2 ,
            SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 3), '/', -1) AS Pro_ifi3 ,
            ss.TaxRate AS 'TaxRate',ss.Amount AS 'Amount',ss.GuarantorCost AS GuarantorCost,ss.CapitalCost AS CapitalCost

            FROM  nxin_qlw_business.sa_financialbusiness s 
            INNER JOIN nxin_qlw_business.sa_financialbusinessdetail ss ON s.NumericalOrder=ss.NumericalOrder
            LEFT JOIN qlw_nxin_com.biz_enterprise AS be ON be.EnterpriseID = s.EnterpriseID
            LEFT JOIN qlw_nxin_com.biz_product AS bp ON bp.ProductID = ss.ProductID 
            LEFT JOIN qlw_nxin_com.biz_productgroup bpg ON bpg.ProductGroupID = bp.ProductGroupID
            LEFT JOIN qlw_nxin_com.biz_productgroupclassification bpclass ON bpg.ClassificationID= bpclass.ClassificationID 
            LEFT JOIN nxin_qlw_business.sa_customer AS sc ON s.EnterpriseID = sc.EnterpriseID AND s.CustomerID = sc.CustomerID
            INNER JOIN qlw_nxin_com.biz_customer scc ON sc.CustomerID = scc.CustomerID
            LEFT JOIN qlw_nxin_com.biz_contacts cusarea ON cusarea.ContactsID = scc.ContactsID
            LEFT JOIN qlw_nxin_com.bsarea areas ON areas.AreaId = cusarea.AreaId
            LEFT JOIN qlw_nxin_com.bsdatadict AS bd ON bd.DictID = s.BusinessAbstract
            LEFT JOIN nxin_qlw_business.hr_person AS hp ON s.SalesmanID = hp.PersonID
            LEFT JOIN qlw_nxin_com.bsorganizationsort AS bs ON bs.SortId = s.OrganizationSortID
            WHERE  s.EnterpriseID IN ({search.EnterpriseIds})
            {((!string.IsNullOrEmpty(search.BeginDate) && !string.IsNullOrEmpty(search.EndDate)) ? @$"and  s.DataDate BETWEEN '{search.BeginDate}' AND '{search.EndDate}' " : "")}
            {(!string.IsNullOrEmpty(search.SalesAbstracts) ? $@" and s.BusinessAbstract IN ({search.SalesAbstracts})" : "")}
            {(!string.IsNullOrEmpty(search.ProductIds) ? $@" and bp.ProductID IN ({search.ProductIds})" : "")}
            {(!string.IsNullOrEmpty(search.ProductGroupIds) ? $@" and bpg.ProductGroupID IN ({search.ProductGroupIds})" : "")}
            {(!string.IsNullOrEmpty(search.ProductGroupClassificationIds) ? $@" and bpg.ClassificationID IN ({search.ProductGroupClassificationIds})" : "")}
            {(!string.IsNullOrEmpty(search.CustomerIds) ? $@" and s.CustomerID IN ({search.CustomerIds})" : "")}
            {(!string.IsNullOrEmpty(search.AreaId) ? $@" and cusarea.AreaId IN ({search.AreaId})" : "")}
            {(!string.IsNullOrEmpty(search.SalesmanID) ? $@" and s.SalesmanID IN ({search.SalesmanID})" : "")}
            ";
            return _context.SalesFinanceEntityDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 获取绩效分配
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<SalesFinanceEntity> GetSalesInnerPerformanceFinanceList(SalesSummarySearch search)
        {
            string sql = $@"SELECT DISTINCT CONCAT(s.NumericalOrder) NumericalOrder,'' SummaryType2Name, UUID() KeyUUID, CONCAT(s.EnterpriseID) AS 'EnterpriseID',
            DATE_FORMAT(DataDate,'%Y-%m-%d') AS 'DataDate',DATE_FORMAT(s.DataDate, '%Y-%m') AS 'DataMonth',CONCAT(BusinessAbstract) BusinessAbstract,bd.cDictName AS 'AbstractName',CONCAT(sc.CustomerID) CustomerID,sc.CustomerName AS 'CustomerName',
            CONCAT(s.SalesmanID) SalesmanID,IF(hp.Name IS NULL, '', hp.Name) AS 'SalesmanName',bpclass.cAxis,
            CONCAT(cusarea.AreaId) AreaId,
            areas.cFullName AS AreaName,
            SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 1), '/', -1) AS Area_name1 ,
            SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 2), '/', -1) AS Area_name2 ,
            SUBSTRING_INDEX(SUBSTRING_INDEX(areas.cFullName, '/', 3), '/', -1) AS Area_name3 ,
            CONCAT(bs.SortID) AS 'OrganizationSortID',CONCAT(bp.ProductID) ProductID,CONCAT(bpg.ProductGroupID) ProductGroupID,bpg.ProductGroupName,bp.ProductName AS 'ProductName',
            CONCAT( bpg.ClassificationID) ClassificationID,
            bpclass.cFullClassName ClassificationName,
            SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 1), '/', -1) AS Pro_ifi1 , 
            SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 2), '/', -1) AS Pro_ifi2 ,
            SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 3), '/', -1) AS Pro_ifi3 ,
            ss.TaxRate AS 'TaxRate',ss.Amount AS 'Amount',ss.GuarantorCost AS GuarantorCost,ss.CapitalCost AS CapitalCost

            FROM  nxin_qlw_business.sa_financialbusiness s 
            INNER JOIN nxin_qlw_business.sa_financialbusinessdetail ss ON s.NumericalOrder=ss.NumericalOrder
            LEFT JOIN qlw_nxin_com.biz_enterprise AS be ON be.EnterpriseID = s.EnterpriseID
            LEFT JOIN qlw_nxin_com.biz_product AS bp ON bp.ProductID = ss.ProductID 
            LEFT JOIN qlw_nxin_com.biz_productgroup bpg ON bpg.ProductGroupID = bp.ProductGroupID
            LEFT JOIN qlw_nxin_com.biz_productgroupclassification bpclass ON bpg.ClassificationID= bpclass.ClassificationID 
            LEFT JOIN nxin_qlw_business.sa_customer AS sc ON s.EnterpriseID = sc.EnterpriseID AND s.CustomerID = sc.CustomerID
            INNER JOIN qlw_nxin_com.biz_customer scc ON sc.CustomerID = scc.CustomerID
            LEFT JOIN qlw_nxin_com.biz_contacts cusarea ON cusarea.ContactsID = scc.ContactsID
            LEFT JOIN qlw_nxin_com.bsarea areas ON areas.AreaId = cusarea.AreaId
            LEFT JOIN qlw_nxin_com.bsdatadict AS bd ON bd.DictID = s.BusinessAbstract
            LEFT JOIN nxin_qlw_business.hr_person AS hp ON s.SalesmanID = hp.PersonID
            LEFT JOIN qlw_nxin_com.bsorganizationsort AS bs ON bs.SortId = s.OrganizationSortID
            WHERE  s.EnterpriseID IN ({search.EnterpriseIds})
            {((!string.IsNullOrEmpty(search.BeginDate) && !string.IsNullOrEmpty(search.EndDate)) ? @$"and  s.DataDate BETWEEN '{search.BeginDate}' AND '{search.EndDate}' " : "")}
            {(!string.IsNullOrEmpty(search.SalesAbstracts) ? $@" and s.BusinessAbstract IN ({search.SalesAbstracts})" : "")}
            {(!string.IsNullOrEmpty(search.ProductIds) ? $@" and bp.ProductID IN ({search.ProductIds})" : "")}
            {(!string.IsNullOrEmpty(search.ProductGroupIds) ? $@" and bpg.ProductGroupID IN ({search.ProductGroupIds})" : "")}
            {(!string.IsNullOrEmpty(search.ProductGroupClassificationIds) ? $@" and bpg.ClassificationID IN ({search.ProductGroupClassificationIds})" : "")}
            {(!string.IsNullOrEmpty(search.CustomerIds) ? $@" and s.CustomerID IN ({search.CustomerIds})" : "")}
            {(!string.IsNullOrEmpty(search.AreaId) ? $@" and cusarea.AreaId IN ({search.AreaId})" : "")}
            {(!string.IsNullOrEmpty(search.SalesmanID) ? $@" and s.SalesmanID IN ({search.SalesmanID})" : "")}
            ";
            return _context.SalesFinanceEntityDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 获取单位组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> EnterSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
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
        /// 获取部门组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> MarketSortInfo(string GroupId = "")
        {
            string sql = $@"SELECT DISTINCT org.SortId,org.`cFullName`,0 RelatedID,org.cAxis
            FROM qlw_nxin_com.`bsorganizationsort` org
            INNER JOIN qlw_nxin_com.`bsorganizationsortmarket` org_mar_rel ON org.`SortId` = org_mar_rel.`SortId`
            INNER JOIN qlw_nxin_com.biz_market mar ON org_mar_rel.MarketID = mar.`MarketID`
            INNER JOIN qlw_nxin_com.biz_versionsetting ver ON mar.`VersionID` = ver.`VersionID`
            WHERE org.isdel = 0 AND org.enterid = {(string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId)}
            AND ver.`dBegin` <= NOW() AND ver.`dEnd` > NOW()";
            return _context.SortInfoDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 绩效分配 客户关系
        /// </summary>
        /// <param name="cus"></param>
        /// <param name="entes"></param>
        /// <returns></returns>
        public List<CustomreService> CustomreServicesInfo(string cus = "0",string entes = "0")
        {
            if (string.IsNullOrEmpty(entes))
            {
                entes = "0";
            }
            if (string.IsNullOrEmpty(cus))
            {
                cus = "0";
            }
            string sql = $@"SELECT IFNULL(C.PersonID,0) AS PersonID,A.NumericalOrder,A.`RecordID`,ifnull(B.`RecordID`,0) AS cRecordId,
            CAST( A.`RoleType` AS CHAR(20)) AS RoleType, 
            CAST( A.`MarketID` AS CHAR(20)) AS MarketID,
            CAST( A.`EntityID` AS CHAR(20)) AS EntityID ,
            CAST( B.`dStart` AS CHAR(20))  AS dStart,
            A.IsDefault,
            D.MarketName,C.Name,IFNULL(OB.Name,O.Name) EditorName,A.ModifiedDate, IFNULL(BT.`cDictName`, T.`TagName`) RoleTypeName
            FROM nxin_qlw_business.`biz_managementrolebusiness` A 
            LEFT JOIN qlw_nxin_com.`biz_customerservice` B ON A.`EnterpriseID`=B.`EnterpriseID`
            AND A.`NumericalOrder`=B.`CustomerID` AND A.`RoleType`=B.`RoleType` AND A.`EntityID`=B.`EntityID`
            AND A.`MarketID`=B.`MarketID` AND A.`EntityType`=B.`EntityType` AND B.`dEnd` IS NULL AND B.`IsUse`=1
            LEFT JOIN qlw_nxin_com.`biz_market` D ON D.`MarketID`=A.`MarketID`
            LEFT JOIN nxin_qlw_business.`hr_person` C ON A.`EntityID`=C.`BO_ID`
            LEFT JOIN nxin_qlw_business.`hr_person` O ON A.`OwnerID`=O.`BO_ID`
            LEFT JOIN nxin_qlw_business.`hr_person` OB ON B.`OwnerID`=OB.`BO_ID`
            LEFT JOIN qlw_nxin_com.`biz_tag` T ON A.`RoleType`=T.`TagID`
            LEFT JOIN qlw_nxin_com.`bsdatadict` BT ON A.`RoleType`=BT.`DictID`
            WHERE A.`ManagementType`=201512170164262065 AND A.`NumericalOrder` IN ({cus}) -- AND A.`EnterpriseID` IN ({entes})
             GROUP BY A.`NumericalOrder`,A.`EntityID`,A.`RoleType` ";

            return _context.CustomreServiceDataSet.FromSqlRaw(sql).ToList(); 
        }
        /// <summary>
        /// 绩效分配 人员关系
        /// </summary>
        /// <returns></returns>
        public List<ServicePersonSort> ServicePersonSortsInfo()
        {
            string sql = @"SELECT hp.PersonId,hp.Name,sort_org.SortId,sort_org.cFullName AS FullName FROM `nxin_qlw_business`.hr_person hp
            INNER JOIN qlw_nxin_com.hr_postinformation hp1 ON hp.PersonID=hp1.PersonID AND hp1.IsUse=1
            INNER JOIN qlw_nxin_com.biz_market bm ON hp1.MarketID=bm.MarketID
            INNER JOIN 
             (
                SELECT sort_org.InheritanceId, sort_org.cFullName, m_org.MarketID,m_org.SortID
                FROM qlw_nxin_com.bsorganizationsortmarket m_org 
                LEFT JOIN qlw_nxin_com.BSOrganizationSort sort_org ON m_org.SortId=sort_org.SortId 
                WHERE sort_org.isDel = 0 AND  sort_org.InheritanceId = 636181287875447931  AND sort_org.isMaster=1
             ) sort_org ON bm.MarketID = sort_org.MarketID";
            return _context.ServicePersonSortDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 获取绩效分配方案
        /// </summary>
        /// <param name="entes"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<PerformanceSet> PerformanceSetInfo(string entes,string beginDate,string endDate)
        {
            string sql = $@"SELECT  spd.`RecordID`,sp.StartDate,sp.EndDate,sp.SalesAbstract,sp.ProductInfo,sp.AssignmentType,spd.AllocationFormula,spd.PersonnelType FROM `nxin_qlw_business`.sa_performanceset AS sp
            INNER JOIN `nxin_qlw_business`.sa_performancesetdetail  AS spd ON sp.NumericalOrder=spd.NumericalOrder
            WHERE sp.EnterpriseID IN ({entes}) and sp.StartDate <= '{beginDate}' and sp.EndDate >= '{endDate}'";
            return _context.PerformanceSetDataSet.FromSqlRaw(sql).ToList();
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
            if (_identityservice.GroupId == "957088881000000")
            {
                sql = $@"    SELECT '销售' DataType,CONCAT(IFNULL(s.NumericalOrder,0)) NumericalOrder,CONCAT(IFNULL(s.Number,0)) Number,'' SummaryType2Name,{MaxRank}  Rank,0 KeyCount,'' SummaryStr,0 IsSum,CONCAT(s.CustomerID) CustomerID,sc.CustomerName,'0' SortId,'' SortName,'' bs_en1,'' bs_en2,'' bs_en3,'' bs_en4,'' bs_en5,'' org_en,'' org_en1,'' org_en2,'' org_en3,UUID() KeyUUID,ss.RecordId,CONCAT(s.EnterpriseId) AS EnterpriseID,en.EnterpriseName,CONCAT(s.OrganizationSortID) OrganizationSortID,DATE_FORMAT(s.DataDate, '%Y-%m-%d') AS 'DataDate',
                   DATE_FORMAT(s.DataDate, '%Y-%m') AS 'DataMonth',CONCAT(ss.ProductId) ProductID,bp.ProductName,bgp.ProductGroupName,bpclass.cFullClassName AS ClassificationName,IFNULL(bpclass.cAxis,'') cAxis,CONCAT(s.SalesAbstract) SalesAbstract,bd.cDictName SalesAbstractName,
                   CONCAT(bgp.ProductGroupID) ProductGroupID,CONCAT( bgp.ClassificationID) ClassificationID,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 1), '/', -1) AS pro_ifi1 , 
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 2), '/', -1) AS pro_ifi2 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 3), '/', -1) AS pro_ifi3 ,
                   IF(hp.Name IS NULL, '', hp.Name) Name,
                   CONCAT(s.SalesmanID) SalesmanID,
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
                   IF(ss.Quantity=0,1,ss.Quantity)                                                        AS 'Quantity',
                   ss.Packages                                                        AS 'Packages',
                   ss.UnitPriceTax                                                    AS 'UnitPriceTax',
                   ss.Amount                                                          AS 'Amount',
                   ROUND(ss.UnitDiscount * ss.Quantity, 2)                                                    AS 'Discount',
                   s.DerateAmount                                                     AS 'DerateAmount',
                   ss.AmountTotal                                                     AS 'AmountTotal',
                   IFNULL(ssde.SalesUnitCost, tt1.UnitPrice)                          AS 'PurchaseUnitCost',
                   IFNULL(ssde.SalesCost, 0) + IFNULL(ss.Quantity * tt1.UnitPrice, 0) AS 'PurchaseUnitCostTaxSum',
                   IFNULL(tt1.TaxRate, 0)                                             AS 'PurchaseTaxRate',
                   ss.AmountTotal/(1+ ss.TaxRate)- (IFNULL(ssde.SalesCost, 0) + IFNULL(ss.Quantity * tt1.UnitPrice, 0)) / (1 + IFNULL(tt1.TaxRate, ss.TaxRate))  AS 'OperatingRevenue',
                    ss.AmountTotal/(1+ ss.TaxRate)                                   AS 'NetSalesExcludingTax',
                   (IFNULL(ssde.SalesCost, 0) + IFNULL(ss.Quantity * tt1.UnitPrice, 0)) / (1 + IFNULL(tt1.TaxRate, ss.TaxRate))  AS PurchaseCostExcludingTax
                    FROM nxin_qlw_business.sa_sales s
                    INNER JOIN nxin_qlw_business.sa_salesdetail ss ON s.numericalorder = ss.numericalorder
                    INNER JOIN qlw_nxin_com.biz_enterprise en ON en.enterpriseid = s.enterpriseid
                    INNER JOIN qlw_nxin_com.biz_product bp ON bp.productid = ss.productid
                    INNER JOIN qlw_nxin_com.biz_productgroup bgp ON bgp.ProductGroupID = bp.ProductGroupID
                    INNER JOIN qlw_nxin_com.biz_customer sc ON s.CustomerID = sc.CustomerID
                    INNER JOIN qlw_nxin_com.bsdatadict AS bd ON bd.DictID = s.SalesAbstract
                    LEFT JOIN qlw_nxin_com.biz_market market ON market.MarketId = s.MarketId
                    LEFT JOIN qlw_nxin_com.biz_productgroupclassification bpclass ON bgp.ClassificationID= bpclass.ClassificationID 
                    LEFT JOIN qlw_nxin_com.biz_contacts cusarea ON cusarea.ContactsID = sc.ContactsID
                    LEFT JOIN qlw_nxin_com.bsarea areas ON areas.AreaId = cusarea.AreaId
                    LEFT JOIN nxin_qlw_business.hr_person AS hp ON s.SalesmanID = hp.PersonID
                    LEFT JOIN nxin_qlw_business.SA_SalesDetailExtend SSDE
                    ON ss.NumericalOrder = SSDE.NumericalOrder AND ss.ProductType = SSDE.NumericalOrderDetail AND
                    SSDE.ProductID = ss.ProductID
                    LEFT JOIN nxin_qlw_business.BIZ_Related AS BR
                    ON BR.ChildValue = s.NumericalOrder AND BR.RelatedType = 201610210104402122 AND
                    BR.ParentType = 1612072106280000101 AND BR.ChildType = 1907161719230000102
                    LEFT JOIN nxin_qlw_business.PM_Supplier SP
                    ON Br.ParentValue = SP.SupplierID AND SP.EnterpriseID = s.EnterpriseID

                    LEFT JOIN nxin_qlw_business.BIZ_Related AS SBR
                    ON SBR.ChildValue = s.NumericalOrder AND SBR.RelatedType = 201610210104402122 AND
                    SBR.ParentType = 1909021648180000101 AND SBR.ChildType = 1907161719230000102
                    LEFT JOIN nxin_qlw_business.BIZ_Related AS PBR
                    ON PBR.RelatedType = 201610210104402122 AND PBR.ParentType = 1909021648390000101 AND
                    PBR.ChildType = 1907161719230000102 AND PBR.ChildValue = s.NumericalOrder
                    LEFT JOIN nxin_qlw_business.BIZ_Related AS BR11
                    ON BR11.ChildValue = s.NumericalOrder AND BR11.RelatedType = 201610210104402122 AND
                    BR11.ParentType = 1903061646040000101 AND BR11.ChildType = 1903061646210000101
                    LEFT JOIN nxin_qlw_business.pm_purchasedetail tt1
                    ON BR11.ParentValue = tt1.NumericalOrder AND tt1.ProductID = ss.ProductID
                    LEFT JOIN nxin_qlw_business.pm_purchase tt2
                    ON tt2.NumericalOrder = tt1.NumericalOrder
                    LEFT JOIN nxin_qlw_business.PM_Supplier SP1
                    ON tt2.SupplierID = SP1.SupplierID AND SP1.EnterpriseID = tt2.EnterpriseID
                    where s.enterpriseid in ({search.EnterpriseIds}) 
                    {((!string.IsNullOrEmpty(search.BeginDate) && !string.IsNullOrEmpty(search.EndDate)) ? @$"and  s.DataDate BETWEEN '{search.BeginDate}' AND '{search.EndDate}' " : "")}
                    {(!string.IsNullOrEmpty(search.SalesAbstracts) ? $@" and s.SalesAbstract IN ({search.SalesAbstracts})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductIds) ? $@" and ss.ProductID IN ({search.ProductIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupIds) ? $@" and bgp.ProductGroupID IN ({search.ProductGroupIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupClassificationIds) ? $@" and bgp.ClassificationID IN ({search.ProductGroupClassificationIds})" : "")}
                    {(!string.IsNullOrEmpty(search.CustomerIds) ? $@" and s.CustomerID IN ({search.CustomerIds})" : "")}
                    {(!string.IsNullOrEmpty(search.AreaId) ? $@" and cusarea.AreaId IN ({search.AreaId})" : "")}
                    {(!string.IsNullOrEmpty(search.MarketId) ? $@" and s.MarketId IN ({search.MarketId})" : "")}
                    {(!string.IsNullOrEmpty(search.SalesmanID) ? $@" and s.SalesmanID IN ({search.SalesmanID})" : "")}

                    {(search.IsGift != null ? $" and ss.Gift = {search.IsGift}" : "")}
                    ";
            }
            else
            {
                sql = $@"    SELECT '销售' DataType,CONCAT(IFNULL(s.NumericalOrder,0)) NumericalOrder,CONCAT(IFNULL(s.Number,0)) Number,'' SummaryType2Name,{MaxRank}  Rank,0 KeyCount,'' SummaryStr,0 IsSum,CONCAT(s.CustomerID) CustomerID,sc.CustomerName,'0' SortId,'' SortName,'' bs_en1,'' bs_en2,'' bs_en3,'' bs_en4,'' bs_en5,'' org_en,'' org_en1,'' org_en2,'' org_en3,UUID() KeyUUID,ss.RecordId,CONCAT(s.EnterpriseId) AS EnterpriseID,en.EnterpriseName,CONCAT(s.OrganizationSortID) OrganizationSortID,DATE_FORMAT(s.DataDate, '%Y-%m-%d') AS 'DataDate',
                   DATE_FORMAT(s.DataDate, '%Y-%m') AS 'DataMonth',CONCAT(ss.ProductId) ProductID,bp.ProductName,bgp.ProductGroupName,bpclass.cFullClassName AS ClassificationName,IFNULL(bpclass.cAxis,'') cAxis,CONCAT(s.SalesAbstract) SalesAbstract,bd.cDictName SalesAbstractName,
                   CONCAT(bgp.ProductGroupID) ProductGroupID,CONCAT( bgp.ClassificationID) ClassificationID,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 1), '/', -1) AS pro_ifi1 , 
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 2), '/', -1) AS pro_ifi2 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 3), '/', -1) AS pro_ifi3 ,
                   IF(hp.Name IS NULL, '', hp.Name) NAME,
                   CONCAT(s.SalesmanID) SalesmanID,
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
                   IF(ss.Quantity=0,1,ss.Quantity)                                                        AS 'Quantity',
                   ss.Packages                                                        AS 'Packages',
                   ss.UnitPriceTax                                                    AS 'UnitPriceTax',
                   ss.Amount                                                          AS 'Amount',
                   ROUND(ss.UnitDiscount * ss.Quantity, 2)                                                    AS 'Discount',
                   s.DerateAmount                                                     AS 'DerateAmount',
                   ss.AmountTotal                                                     AS 'AmountTotal',
                   0.00                          AS 'PurchaseUnitCost',
                   IFNULL(ss.Quantity, 0.00) AS 'PurchaseUnitCostTaxSum',
                   0.00                                             AS 'PurchaseTaxRate',
                   ss.AmountTotal/(1+ ss.TaxRate) + IFNULL(ss.Quantity, 0.00) / (1 + ss.TaxRate)  AS 'OperatingRevenue',
                    ss.AmountTotal/(1+ ss.TaxRate)                                   AS 'NetSalesExcludingTax',
                    IFNULL(ss.Quantity, 0.00) / (1 + ss.TaxRate)  AS PurchaseCostExcludingTax
                    FROM nxin_qlw_business.sa_sales s
                    INNER JOIN nxin_qlw_business.sa_salesdetail ss ON s.numericalorder = ss.numericalorder
                    INNER JOIN qlw_nxin_com.biz_enterprise en ON en.enterpriseid = s.enterpriseid
                    INNER JOIN qlw_nxin_com.biz_product bp ON bp.productid = ss.productid
                    INNER JOIN qlw_nxin_com.biz_productgroup bgp ON bgp.ProductGroupID = bp.ProductGroupID
                    INNER JOIN qlw_nxin_com.biz_customer sc ON s.CustomerID = sc.CustomerID
                    INNER JOIN qlw_nxin_com.bsdatadict AS bd ON bd.DictID = s.SalesAbstract
                    LEFT JOIN qlw_nxin_com.biz_market market ON market.MarketId = s.MarketId
                    LEFT JOIN qlw_nxin_com.biz_productgroupclassification bpclass ON bgp.ClassificationID= bpclass.ClassificationID 
                    LEFT JOIN qlw_nxin_com.biz_contacts cusarea ON cusarea.ContactsID = sc.ContactsID
                    LEFT JOIN qlw_nxin_com.bsarea areas ON areas.AreaId = cusarea.AreaId
                    LEFT JOIN nxin_qlw_business.hr_person AS hp ON s.SalesmanID = hp.PersonID
                    where s.enterpriseid in ({search.EnterpriseIds}) 
                    {((!string.IsNullOrEmpty(search.BeginDate) && !string.IsNullOrEmpty(search.EndDate)) ? @$"and  s.DataDate BETWEEN '{search.BeginDate}' AND '{search.EndDate}' " : "")}
                    {(!string.IsNullOrEmpty(search.SalesAbstracts) ? $@" and s.SalesAbstract IN ({search.SalesAbstracts})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductIds) ? $@" and ss.ProductID IN ({search.ProductIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupIds) ? $@" and bgp.ProductGroupID IN ({search.ProductGroupIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupClassificationIds) ? $@" and bgp.ClassificationID IN ({search.ProductGroupClassificationIds})" : "")}
                    {(!string.IsNullOrEmpty(search.CustomerIds) ? $@" and s.CustomerID IN ({search.CustomerIds})" : "")}
                    {(!string.IsNullOrEmpty(search.AreaId) ? $@" and cusarea.AreaId IN ({search.AreaId})" : "")}
                    {(!string.IsNullOrEmpty(search.MarketId) ? $@" and s.MarketId IN ({search.MarketId})" : "")}
                    {(!string.IsNullOrEmpty(search.SalesmanID) ? $@" and s.SalesmanID IN ({search.SalesmanID})" : "")}

                    {(search.IsGift != null ? $" and ss.Gift = {search.IsGift}" : "")}
                    ";
            }
            return sql;
        }
        /// <summary>
        /// 获取销售单汇总数据-绩效汇总表 专用
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        
        public dynamic GetSalesInnerPerformanceData(SalesSummarySearch search)
        {
            if (string.IsNullOrEmpty(search.EnterpriseIds))
            {
                search.EnterpriseIds = "0";
            }
            //search.EnterpriseIds = "1564745,1564841,1579429,1635482,1649643,1649644,1649645,1649647,1649654,1649656,1649660,1649661,1649662,1649665,1649666,1649667,1649670,1649674,1649677,1649680,1649708,1664009,1687948,1715599,1731809,1736811,1770816,1786474,1786475,1798658,1812364,1812365,1812368,1862934,1869220,1887556,1891652,1908209,2119007,2390850,2488488,2529825,1345508302914,13768274246665,16436252259676,69827436510096,73083846959190,94159000791321,110393479681401,160888034113316,392765588122251,439884140970006,474220486710506,563211166749839,689770436088619,693675032631813,768774039576964,866472692097073,885514206174553,634086739144001641";
            var MaxRankData = _context.GetNumbers.FromSqlRaw($@"SELECT IFNULL(CONCAT(MAX(Rank)+2),0) AS MaxNumber FROM qlw_nxin_com.BIZ_ProductGroupClassification WHERE enterpriseid = {search.GroupId}").FirstOrDefault();
            var MaxRank = MaxRankData == null ? "1" : MaxRankData.MaxNumber;
            if (string.IsNullOrEmpty(MaxRank))
            {
                MaxRank = "1";
            }
            string sql = $@"    SELECT '销售' DataType,CONCAT(IFNULL(s.NumericalOrder,0)) NumericalOrder,CONCAT(IFNULL(s.Number,0)) Number,per.SummaryType2Name,{MaxRank}  Rank,0 KeyCount,'' SummaryStr,0 IsSum,CONCAT(s.CustomerID) CustomerID,sc.CustomerName,'0' SortId,'' SortName,'' bs_en1,'' bs_en2,'' bs_en3,'' bs_en4,'' bs_en5,'' org_en,'' org_en1,'' org_en2,'' org_en3,UUID() KeyUUID,ss.RecordId,CONCAT(s.EnterpriseId) AS EnterpriseID,en.EnterpriseName,CONCAT(s.OrganizationSortID) OrganizationSortID,DATE_FORMAT(s.DataDate, '%Y-%m-%d') AS 'DataDate',
                   DATE_FORMAT(s.DataDate, '%Y-%m') AS 'DataMonth',CONCAT(ss.ProductId) ProductID,bp.ProductName,bgp.ProductGroupName,bpclass.cFullClassName AS ClassificationName,IFNULL(bpclass.cAxis,'') cAxis,CONCAT(s.SalesAbstract) SalesAbstract,bd.cDictName SalesAbstractName,
                   CONCAT(bgp.ProductGroupID) ProductGroupID,CONCAT( bgp.ClassificationID) ClassificationID,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 1), '/', -1) AS pro_ifi1 , 
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 2), '/', -1) AS pro_ifi2 ,
                   SUBSTRING_INDEX(SUBSTRING_INDEX(bpclass.cFullClassName, '/', 3), '/', -1) AS pro_ifi3 ,
                   IF(hp.Name IS NULL, '', hp.Name) Name,
                   CONCAT(s.SalesmanID) SalesmanID,
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
                   IF(ss.Quantity=0,1,ss.Quantity)                                                        AS 'Quantity',
                   ss.Packages                                                        AS 'Packages',
                   ss.UnitPriceTax                                                    AS 'UnitPriceTax',
                   ss.Amount                                                          AS 'Amount',
                   ROUND(ss.UnitDiscount * ss.Quantity, 2)                                                    AS 'Discount',
                   s.DerateAmount                                                     AS 'DerateAmount',
                   ss.AmountTotal                                                     AS 'AmountTotal',
                   IFNULL(ssde.SalesUnitCost, tt1.UnitPrice)                          AS 'PurchaseUnitCost',
                   IFNULL(ssde.SalesCost, 0) + IFNULL(ss.Quantity * tt1.UnitPrice, 0) AS 'PurchaseUnitCostTaxSum',
                   IFNULL(tt1.TaxRate, 0)                                             AS 'PurchaseTaxRate',
                   ss.AmountTotal/(1+ ss.TaxRate)- (IFNULL(ssde.SalesCost, 0) + IFNULL(ss.Quantity * tt1.UnitPrice, 0)) / (1 + IFNULL(tt1.TaxRate, ss.TaxRate))  AS 'OperatingRevenue',
                    ss.AmountTotal/(1+ ss.TaxRate)                                   AS 'NetSalesExcludingTax',
                   (IFNULL(ssde.SalesCost, 0) + IFNULL(ss.Quantity * tt1.UnitPrice, 0)) / (1 + IFNULL(tt1.TaxRate, ss.TaxRate))  AS PurchaseCostExcludingTax
                    FROM nxin_qlw_business.sa_sales s
                    INNER JOIN nxin_qlw_business.sa_salesdetail ss ON s.numericalorder = ss.numericalorder
                    INNER JOIN qlw_nxin_com.biz_enterprise en ON en.enterpriseid = s.enterpriseid
                    INNER JOIN nxin_qlw_business.biz_reviwe AS owe ON s.NumericalOrder = owe.NumericalOrder AND owe.Level = 1
                    INNER JOIN qlw_nxin_com.biz_product bp ON bp.productid = ss.productid
                    INNER JOIN qlw_nxin_com.biz_productgroup bgp ON bgp.ProductGroupID = bp.ProductGroupID
                    INNER JOIN `nxin_qlw_business`.fm_performanceincome per ON per.ProductGroupID = bp.ProductGroupID
                    INNER JOIN qlw_nxin_com.biz_customer sc ON s.CustomerID = sc.CustomerID
                    INNER JOIN qlw_nxin_com.bsdatadict AS bd ON bd.DictID = s.SalesAbstract
                    LEFT JOIN qlw_nxin_com.biz_market market ON market.MarketId = s.MarketId
                    LEFT JOIN qlw_nxin_com.biz_productgroupclassification bpclass ON bgp.ClassificationID= bpclass.ClassificationID 
                    LEFT JOIN qlw_nxin_com.biz_contacts cusarea ON cusarea.ContactsID = sc.ContactsID
                    LEFT JOIN qlw_nxin_com.bsarea areas ON areas.AreaId = cusarea.AreaId
                    LEFT JOIN nxin_qlw_business.hr_person AS hp ON s.SalesmanID = hp.PersonID
                    LEFT JOIN nxin_qlw_business.SA_SalesDetailExtend SSDE
                    ON ss.NumericalOrder = SSDE.NumericalOrder AND ss.ProductType = SSDE.NumericalOrderDetail AND
                    SSDE.ProductID = ss.ProductID
                    LEFT JOIN nxin_qlw_business.BIZ_Related AS BR
                    ON BR.ChildValue = s.NumericalOrder AND BR.RelatedType = 201610210104402122 AND
                    BR.ParentType = 1612072106280000101 AND BR.ChildType = 1907161719230000102
                    LEFT JOIN nxin_qlw_business.PM_Supplier SP
                    ON Br.ParentValue = SP.SupplierID AND SP.EnterpriseID = s.EnterpriseID

                    LEFT JOIN nxin_qlw_business.BIZ_Related AS SBR
                    ON SBR.ChildValue = s.NumericalOrder AND SBR.RelatedType = 201610210104402122 AND
                    SBR.ParentType = 1909021648180000101 AND SBR.ChildType = 1907161719230000102
                    LEFT JOIN nxin_qlw_business.BIZ_Related AS PBR
                    ON PBR.RelatedType = 201610210104402122 AND PBR.ParentType = 1909021648390000101 AND
                    PBR.ChildType = 1907161719230000102 AND PBR.ChildValue = s.NumericalOrder
                    LEFT JOIN nxin_qlw_business.BIZ_Related AS BR11
                    ON BR11.ChildValue = s.NumericalOrder AND BR11.RelatedType = 201610210104402122 AND
                    BR11.ParentType = 1903061646040000101 AND BR11.ChildType = 1903061646210000101
                    LEFT JOIN nxin_qlw_business.pm_purchasedetail tt1
                    ON BR11.ParentValue = tt1.NumericalOrder AND tt1.ProductID = ss.ProductID
                    LEFT JOIN nxin_qlw_business.pm_purchase tt2
                    ON tt2.NumericalOrder = tt1.NumericalOrder
                    LEFT JOIN nxin_qlw_business.PM_Supplier SP1
                    ON tt2.SupplierID = SP1.SupplierID AND SP1.EnterpriseID = tt2.EnterpriseID
                    where s.enterpriseid in ({search.EnterpriseIds}) 
                    {((!string.IsNullOrEmpty(search.BeginDate) && !string.IsNullOrEmpty(search.EndDate)) ? @$"and  s.DataDate BETWEEN '{search.BeginDate}' AND '{search.EndDate}' " : "")}
                    {(!string.IsNullOrEmpty(search.SalesAbstracts) ? $@" and s.SalesAbstract IN ({search.SalesAbstracts})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductIds) ? $@" and ss.ProductID IN ({search.ProductIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupIds) ? $@" and bgp.ProductGroupID IN ({search.ProductGroupIds})" : "")}
                    {(!string.IsNullOrEmpty(search.ProductGroupClassificationIds) ? $@" and bgp.ClassificationID IN ({search.ProductGroupClassificationIds})" : "")}
                    {(!string.IsNullOrEmpty(search.CustomerIds) ? $@" and s.CustomerID IN ({search.CustomerIds})" : "")}
                    {(!string.IsNullOrEmpty(search.AreaId) ? $@" and cusarea.AreaId IN ({search.AreaId})" : "")}
                    {(!string.IsNullOrEmpty(search.MarketId) ? $@" and s.MarketId IN ({search.MarketId})" : "")}
                    {(!string.IsNullOrEmpty(search.SalesmanID) ? $@" and s.SalesmanID IN ({search.SalesmanID})" : "")}

                    {(search.IsGift != null ? $" and ss.Gift = {search.IsGift}" : "")}";

            var list = _context.SalesSummaryEntitiesDataSet.FromSqlRaw(sql).ToList();
            //追加 金融数据
            SetFinanceInnerPerformanceData(search, MaxRank, list);
            var enSort = EnterSortInfo(search.GroupId); //单位组织
            var maSort = MarketSortInfo(search.GroupId);//创业单元
            foreach (var item in list)
            {
                SetSortNameOrganizationName(enSort, maSort, item);
            }
            //数据过滤
            var tempList = OrgEnterFilter(search, list);
            list = tempList;

            var query = list.Select(m=> new 
            {
                ym = m.DataMonth,
                L3_id = m.bs_enid3,
                L4_id = m.bs_enid4,
                L5_id = m.bs_enid5,
                l3_name = m.bs_en3, 
                l4_name = m.bs_en4,
                l5_name = m.bs_en5,
                person_id = m.SalesmanID,
                bo_name = m.Name,
                summaryType1Name = "营业收入",
                summaryType2Name = string.IsNullOrEmpty(m.SummaryType2Name) ? "金融服务收入" : m.SummaryType2Name,
                iorder = 1,
                periodYear = m.OperatingRevenue
            });
            try
            {
                _logger.LogInformation("销售汇总绩效参数：" + JsonConvert.SerializeObject(search));
                _logger.LogInformation("中南大区 数智服务收入明细：" + query.Where(m => m.summaryType2Name == "数智服务收入" && m.L3_id == "2002191347100000116").Sum(m => m.periodYear));
                _logger.LogInformation("中南大区 交易服务收入明细：" + query.Where(m => m.summaryType2Name == "交易服务收入" && m.L3_id == "2002191347100000116").Sum(m => m.periodYear));
                _logger.LogInformation("中南大区 金融服务收入明细：" + query.Where(m => m.summaryType2Name == "金融服务收入" && m.L3_id == "2002191347100000116").Sum(m => m.periodYear));
                _logger.LogInformation("总金额 ：" + query.Sum(m => m.periodYear));
            }
            catch (Exception e)
            {
                _logger.LogInformation("日志记录异常："+e.ToString());
            }
            return query;
        }
    }
}
