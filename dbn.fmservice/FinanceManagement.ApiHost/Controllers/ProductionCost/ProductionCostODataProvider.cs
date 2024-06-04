using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;
using System.Dynamic;
using System.Reflection;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 波尔莱特 - 定制化需求
    /// </summary>
    public class ProductionCostODataProvider
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
        MS_StockCostODataProvider _msCostProvider;
        MS_FormulaODataProvider _msFormulaODataProvider;

        public ProductionCostODataProvider(MS_FormulaODataProvider msFormulaODataProvider, MS_StockCostODataProvider msCostProvider, IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
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
            _msCostProvider = msCostProvider;
            _msFormulaODataProvider = msFormulaODataProvider;
        }
        /// <summary>
        /// https://confluence.nxin.com/pages/viewpage.action?pageId=74679524
        /// </summary>
        /// <param name="model"></param>
        /// model.EnterSortIds = 单位id
        /// <returns></returns>
        public dynamic GetProductionCostData(dynamic model)
        {
            dynamic result = new ExpandoObject();
            double MeasureUnit = 1;
            if (model.IsTons == null)
            {
                model.IsTons = 1;
            }
            if (model.ProductIds == null)
            {
                model.ProductIds = "";
            }
            if (model.BeginDate == null)
            {
                model.BeginDate = "2000-01-01";
            }
            if (model.EndDate == null)
            {
                model.EndDate = "2099-12-31";
            }
            #region SQL数据源
            string finlaySql = $@"
            SELECT 0.00 AS ProductMarketPrice,t1.*,
            IFNULL(t2.Quantity,0.00) Quantity
            FROM
            (
	            SELECT a.EnterpriseID,
                a.EnterpriseName,
                a.DataDate,
                a.ProductID AS ProduceID, -- 产品名称ID
                a.ProductName as ProduceName, -- 产品名称
                a.FormulaName, -- 配方名称
                a.BaseQuantity, -- 配方基数
                c.ylProductID AS ProductID, -- 原料名称ID
                c.ProductName, -- 原料名称
	            SUM(a.Quantity_sc) AS Quantity_sc , -- 生产入库数量
                0.00 AS Quantity, -- 间接耗用数量
                SUM(IFNULL(c.PlanExpend, 0)) AS PlanExpend, -- 计划耗用
                SUM(IFNULL(c.StandardExpend, 0)) AS StandardExpend,-- 单位配方标准耗用 
	            SUM(IFNULL(c.ProductionExpend, 0)) AS ProductionExpend ,-- 生产耗用
                SUM(IFNULL(c.Loss, 0)) AS Loss , -- 损耗
                SUM(IFNULL(c.bzQuantity, 0)) AS bzQuantity , -- 包装耗用
                SUM(IFNULL(c.ylQuantity, 0)) AS ylQuantity   -- 原料耗用
	            FROM
	            (
		            -- 生产入库
		            SELECT t4.BatchID,t1.DataDate,t1.EnterpriseID,en.EnterpriseName, t7.FormulaName,t7.BaseQuantity, t4.BatchNumber, t2.ProductID,abp.ProductName, t1.MarketID,IFNULL(SUM(t2.Quantity),0.00) AS Quantity_sc
		            FROM NXin_Qlw_Business.MS_ProductionStockIn t1
		            INNER JOIN NXin_Qlw_Business.MS_ProductionStockInDetail t2 ON t1.NumericalOrder = t2.NumericalOrder
		            LEFT JOIN NXin_Qlw_Business.BIZ_Related t3 ON t3.RelatedType = 201610210104402122 AND t3.ParentType = 1710311318280000101 AND t2.NumericalOrderDetail = t3.ChildValue AND t3.`ChildType` = 1611121402400000101
		            LEFT JOIN NXin_Qlw_Business.BIZ_Batch t4 ON t3.ParentValue = t4.BatchID
		            LEFT JOIN  NXin_Qlw_Business.BIZ_Related t5 ON t5.RelatedType=201610210104402122 AND t5.ParentType= 1710311318280000101 AND t5.ChildType=1612141408050000101 AND t5.ParentValue=t4.BatchID
		            LEFT JOIN NXin_Qlw_Business.MS_ProductionPlanDetail t6 ON t6.NumericalOrderDetail=t5.ChildValue
		            LEFT JOIN nxin_qlw_business.ms_formula  t7 ON t6.FormulaID=t7.NumericalOrder
                    LEFT JOIN qlw_nxin_com.biz_product abp on abp.ProductId = t2.ProductId
                    LEFT JOIN qlw_nxin_com.biz_enterprise en on en.enterpriseid = t1.enterpriseid
		            WHERE t1.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' AND t1.EnterpriseID IN({model.EnterSortIds}) AND t1.StockAbstract IN (201611220104402201) AND t7.ISUSE = TRUE 
                    {(!string.IsNullOrEmpty(model.ProductIds.ToString()) ? $@" AND t2.ProductID IN ({model.ProductIds.ToString()})" : "")}
		            GROUP BY t1.EnterpriseID, t4.BatchID,  t2.ProductID, t1.MarketID
	            ) a

	            LEFT JOIN
	            (
		            -- 材料领用(直接材料单，间接材料单：取数来自成本计算后分摊到表体2）
		            SELECT t4.BatchID,t1.EnterpriseID,t4.BatchNumber,t1.ProductID,t2.`ProductID` AS ylproductID,abp.ProductName,t1.MarketID,SUM(t2.PlanExpend) AS PlanExpend,(t2.StandardExpend * t1.Quantity) AS StandardExpend,
		            SUM(t2.ProductionExpend) AS ProductionExpend,SUM(t2.Loss) AS Loss,
		            SUM(CASE WHEN t13.FormulaClassify = 201706140104402102 THEN t2.ProductionExpend + t2.Loss ELSE 0 END) AS bzQuantity,
		            SUM(CASE WHEN t13.FormulaClassify != 201706140104402102 THEN t2.ProductionExpend + t2.Loss ELSE 0 END) AS ylQuantity
		            FROM NXin_Qlw_Business.MS_MaterialsExpend t1
		            INNER JOIN NXin_Qlw_Business.MS_MaterialsExpendDetail t2 ON t1.NumericalOrder = t2.NumericalOrder
		            LEFT JOIN NXin_Qlw_Business.BIZ_Related t3 ON t3.RelatedType = 201610210104402122 AND t3.ParentType = 1710311318280000101 AND t2.NumericalOrder = t3.ChildValue
		            LEFT JOIN NXin_Qlw_Business.BIZ_Batch t4 ON t3.ParentValue = t4.BatchID
		            LEFT JOIN NXin_Qlw_Business.BIZ_ProductDetail pd ON t2.ProductID = pd.ProductID AND t1.EnterpriseID = pd.EnterpriseID
		            LEFT JOIN qlw_nxin_com.BSDataDict t11 ON pd.StockType = t11.DictID
		            LEFT JOIN NXin_Qlw_Business.BIZ_Related t12 ON t12.RelatedType = 201610210104402122 AND t12.ParentType = 2016141408050000101 AND t12.ChildType = 1611121417390000101 AND t12.ChildValue = t1.NumericalOrder
		            LEFT JOIN nxin_qlw_business.ms_formulatype t13 ON t12.ParentValue = t13.FormulaTypeID
                    LEFT JOIN qlw_nxin_com.biz_product abp on abp.ProductId = t2.ProductId
		            WHERE MaterialsExpendType = 201611220104402101 AND t1.DataDate BETWEEN '{model.BeginDate}' AND DATE_ADD('{model.EndDate}', INTERVAL 2 MONTH) AND t1.EnterpriseID IN({model.EnterSortIds}) AND t11.DictID IN (1811021018180000130, 2016030706305420212, 2016030706691391315, 2016030706946772264, 2016030708495161860,2016030710282051216)
                    GROUP BY t1.EnterpriseID,  t1.ProductID,t2.`ProductID`, t1.MarketID,t4.BatchID
	            ) c ON a.ProductID = c.ProductID AND a.MarketID = c.MarketID and a.EnterpriseId = c.EnterpriseId and a.BatchID = c.BatchID
	            GROUP BY a.EnterpriseID,a.ProductID,c.ylProductID
            ) t1

            LEFT JOIN
            (
	            -- 间接材料单
	            SELECT a.EnterpriseID,a.productID,a.ProductID1 AS ylProductID,IFNULL(SUM(a.Quantity),0.00) AS Quantity-- 间接耗用数量
	            FROM
	            (
		            SELECT t1.ProductionAbstract,t1.WarehouseID,t1.EnterpriseID,t2.ProductID,t2.ProductID1,t2.Quantity,tb.BatchNumber,t1.MarketID,t1.DataDate
		            FROM NXin_Qlw_Business.MS_MaterialsExpend t1
		            INNER JOIN NXin_Qlw_Business.MS_MaterialsExpendList t2 ON t1.NumericalOrder = t2.NumericalOrder
		            LEFT JOIN NXin_Qlw_Business.BIZ_Related tt ON tt.RelatedType = 201610210104402122 AND tt.ParentType = 1710311318280000101 AND tt.ChildValue = t1.NumericalOrder
		            LEFT JOIN NXin_Qlw_Business.BIZ_Batch tb ON tt.ParentValue = tb.BatchID
		            LEFT JOIN NXin_Qlw_Business.BIZ_ProductDetail pd ON t2.ProductID1 = pd.ProductID AND t1.EnterpriseID = pd.EnterpriseID
		            LEFT JOIN qlw_nxin_com.BSDataDict t11 ON pd.StockType = t11.DictID
		            WHERE t1.ProductionAbstract IN (201612090104402104) AND t1.EnterpriseID IN({model.EnterSortIds}) AND t1.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'
		            AND t11.DictID IN (1811021018180000130, 2016030706305420212, 2016030706691391315, 2016030706946772264, 2016030708495161860,2016030710282051216)
	            ) a
	            LEFT JOIN qlw_nxin_com.BIZ_Product d ON a.ProductID = d.ProductID
	            LEFT JOIN NXin_Qlw_Business.BIZ_ProductDetail t9 ON a.EnterpriseID = t9.EnterpriseID AND d.ProductID = t9.ProductID
	            LEFT JOIN qlw_nxin_com.BIZ_Product pd ON a.ProductID1 = pd.ProductID
	            LEFT JOIN qlw_nxin_com.UnitMeasurement um ON t9.MeasureUnit = um.UnitID
	            GROUP BY a.EnterpriseID,a.productID,a.ProductID1
            ) t2 ON t1.EnterpriseID=t2.EnterpriseID AND t1.ProductID=t2.ProductID AND t1.ProductID=t2.ylProductID

            WHERE t1.EnterpriseID IN ({model.EnterSortIds}) AND t1.DataDate Between '{model.BeginDate}' AND '{model.EndDate}'
            ";
            //配方商品价格设定
            string PriceSql = $@"select ff.DataDate,ffd.ProductID,ffd.MarketPrice from nxin_qlw_business.ms_formulaproductprice ff
            inner join nxin_qlw_business.ms_formulaproductpricedetail ffd on ffd.NumericalOrder = ff.NumericalOrder
            left join nxin_qlw_business.ms_formulaproductpriceext ffe on ffe.NumericalOrder = ffe.NumericalOrder
            where ff.GroupID = {_identityservice.GroupId} and DataDate <= '{model.DataDate}'
            ORDER BY DataDate desc 
            ";
            #endregion
            var PriceData = _context.DynamicSqlQuery(PriceSql);
            //主体数据 涵盖计算逻辑 公斤 吨 换算 最后在处理
            var Data = _context.DynamicSqlQuery(finlaySql);
            #region 数据融合 价格设定
            foreach (var item in PriceData)
            {
                //原料
                var tempProduct = Data.Where(m => m.ProductID == item.ProductID).ToList();
                foreach (var product in tempProduct)
                {
                    product.ProductMarketPrice = item.MarketPrice;
                }
            }
            #endregion
            #region 原料库存实时成本 数据源
            //https://docs.qq.com/doc/DQW5walBFWWlDWUho
            var MSData = (List<MS_StockCostResultModel>)_msCostProvider.GetReportByQMAmount(model);
            #endregion
            #region 标准配方单
            var FormulaData = _msFormulaODataProvider.GetFormulaByDate(new MS_FormulaSearch() 
            {
                BeginDate = model.BeginDate ,EndDate = model.EndDate,GroupID = _identityservice.GroupId ,
                UseEnterpriseIds = model.EnterSortIds,NumericalOrder = "0"
            });
            foreach (var item in FormulaData)
            {
                var T = Data.FirstOrDefault(m =>  item.UseProductIds.Contains(m.ProduceID.ToString()) && item.ProductID == m.ProductID && item.UseEnterpriseIds.Contains(m.EnterpriseID)) ;
                if (T == null)
                {
                    //单位
                    int indexE = 0;
                    foreach (var x in ((string)item.UseEnterpriseIds).Split(','))
                    {
                        int index = 0;
                        //产品
                        foreach (var produce in ((string)item.UseProductIds).Split(','))
                        {
                            dynamic valuePairs = new ExpandoObject();
                            var expandoObj = (ExpandoObject)Data.FirstOrDefault();
                            var expandoDict = (IDictionary<string, object>)expandoObj;
                            if (expandoObj == null)
                            {
                                continue;
                            }
                            foreach (var items in expandoDict)
                            {
                                if (items.Key == "EnterpriseID")
                                {
                                    _context.AddProperty(valuePairs, items.Key, x);
                                }
                                else if (items.Key == "EnterpriseName")
                                {
                                    _context.AddProperty(valuePairs, items.Key, ((string)item.UseEnterpriseNames).Split(',')[indexE]);
                                }
                                else if (items.Key == "ProduceID")
                                {
                                    _context.AddProperty(valuePairs, items.Key, produce);
                                }
                                else if(items.Key == "FormulaName")
                                {
                                    _context.AddProperty(valuePairs, items.Key, item.FormulaName);
                                }
                                else if (items.Key == "ProductID")
                                {
                                    _context.AddProperty(valuePairs, items.Key, item.ProductID);
                                }
                                else if (items.Key == "ProduceName")
                                {
                                    _context.AddProperty(valuePairs, items.Key, ((string)item.UseProductNames).Split(',')[index]);
                                }
                                else if (items.Key == "ProductName")
                                {
                                    _context.AddProperty(valuePairs, items.Key, item.ProductName);
                                }
                                else if(items.Key == "Quantity_sc")
                                {
                                    var temp = Data.FirstOrDefault(m => m.ProduceID == produce && m.EnterpriseID == x);
                                    double Quantity_sc = 0;
                                    if (temp != null)
                                    {
                                        Quantity_sc = (double)temp.Quantity_sc;
                                    }
                                    else
                                    {

                                    }
                                    _context.AddProperty(valuePairs, items.Key, Quantity_sc);
                                }
                                else
                                {
                                    if (items.Value is double)
                                    {
                                        _context.AddProperty(valuePairs, items.Key, (double)0.00);
                                    }
                                    else
                                    {
                                        _context.AddProperty(valuePairs, items.Key, "");
                                    }
                                }
                            }
                            Data.Add(valuePairs);
                            index++;
                        }
                        indexE++;
                    }
                }
            }
            #endregion
            //只算原料
            for (int i = 0; i < Data.Count; i++)
            {
                var item = Data[i];
                if (item.BaseQuantity.ToString() == "")
                {
                    item.BaseQuantity = 1;
                }
                var MsTempData = MSData.Where(m => m.ProductID == item.ProductID && m.EnterPriseID == item.EnterpriseID).FirstOrDefault();
                if (MsTempData == null)
                {
                    MsTempData = new MS_StockCostResultModel() { qmUnitPrice = 0 };
                }
                var TempFormulaData = FormulaData.Where(m =>m.UseProductIds.Contains(item.ProduceID) && m.ProductID == item.ProductID && m.UseEnterpriseIds.Contains(item.EnterpriseID)).FirstOrDefault();
                if (TempFormulaData != null && item.ProduceID == "1803261003130000202" && item.ProductID == "1803251542580000208")
                {
                    Console.WriteLine();
                }
                //【标准配方耗用量】= 【标准配方单】耗用用量/配方基数*实际入库数量
                //实际入库数量 = 产品名称汇总数量
                double StandardConsumption = (TempFormulaData == null || TempFormulaData.BaseQuantity == 0) ? 0 : (TempFormulaData?.ProportionQuantity == null ? 0 : TempFormulaData.ProportionQuantity) / TempFormulaData.BaseQuantity * (double)item.Quantity_sc;

                //【理论用量】=【单位配方单】耗用用量/配方基数*实际入库数量
                double TheoreticalDosage = item.StandardExpend;

                //【实际用量】= 实际用量=生产耗用+损耗+间接耗用
                double ActualUsage = item.ProductionExpend + item.Loss + item.Quantity;//todo

                //【标准配方成本】= 标准配方耗用量 * 实时单位成本
                double StandardFormulaCost = StandardConsumption * Convert.ToDouble(MsTempData.qmUnitPrice);

                //【理论成本】= 理论用量 * 实时单位成本
                double TheoreticalCost = TheoreticalDosage * Convert.ToDouble(MsTempData.qmUnitPrice);

                //【实际成本】= 实际用量 * 实时单位成本
                double ActualCost = ActualUsage * Convert.ToDouble(MsTempData.qmUnitPrice);

                //【实时单位成本】 = 原料实时库存成本的单位成本
                double RealUnitCost = Convert.ToDouble(MsTempData.qmUnitPrice);


                //是否小计
                _context.AddProperty(Data[i], "IsSum", 0);
                //排序
                _context.AddProperty(Data[i], "OrderNum", 0.00);
                //标准配方耗用量
                _context.AddProperty(Data[i], "StandardConsumption", StandardConsumption);
                //理论用量
                _context.AddProperty(Data[i], "TheoreticalDosage", TheoreticalDosage);
                //实际用量
                _context.AddProperty(Data[i], "ActualUsage", ActualUsage);
                //标准配方成本 
                _context.AddProperty(Data[i], "StandardFormulaCost", StandardFormulaCost);
                //理论成本
                _context.AddProperty(Data[i], "TheoreticalCost", TheoreticalCost);
                //实际成本
                _context.AddProperty(Data[i], "ActualCost", ActualCost);
                //实时单位成本
                _context.AddProperty(Data[i], "RealUnitCost", RealUnitCost);
            }
            #region 列汇总数据项
            string colunm = GetNameBySummary(model.ColumnSummary.ToString() + ",OrderNum");
            //列汇总数据项
            var ColumnList = new List<dynamic>();
            //Dynamic Linq 重构列汇总方式参数
            var ColumnSummary = GetColumnSummaryHandler(model.ColumnSummary.ToString());
            // 使用 Dynamic Linq 库执行查询

            if (!string.IsNullOrEmpty(model.ColumnSummary.ToString()))
            {
                string order = model.ColumnSummary.ToString();

                ColumnList = Data.AsQueryable().GroupBy($"{ColumnSummary.ToString()}", "it").Select(colunm).OrderBy(order).ToDynamicList();
            }
            else
            {
                //无列汇总方式则全显
                ColumnList = Data;
            }
            #endregion            
            #region 数据
            //存在行汇总方式时
            List<dynamic> List = new List<dynamic>();
            {
                //dynamic LINQ 分组 列数据 得到最终数据
                dynamic temp = new { Details = new List<dynamic>() };
                var Details = Data.AsQueryable().GroupBy($"{ColumnSummary.ToString()}", "it").ToDynamicList();
                //获取列项目字符串
                string colSummary = model.ColumnSummary.ToString();
                //数据金额汇总
                foreach (var group in Details)
                {
                    // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                    dynamic key = group.Key;
                    IEnumerable<dynamic> elements = group;

                    // 使用 dynamic 类型计算 字段的总和
                    var StandardConsumption = elements.Sum(m => (decimal)m.StandardConsumption);
                    var TheoreticalDosage = elements.Sum(m => (decimal)m.TheoreticalDosage);
                    var ActualUsage = elements.Sum(m => (decimal)m.ActualUsage);
                    var StandardFormulaCost = elements.Sum(m => (decimal)m.StandardFormulaCost);
                    var TheoreticalCost = elements.Sum(m => (decimal)m.TheoreticalCost);
                    var ActualCost = elements.Sum(m => (decimal)m.ActualCost);
                    var RealUnitCost = elements.Sum(m => (decimal)m.RealUnitCost);
                    //重构数据结构 赋值
                    var Gtemp = elements.FirstOrDefault();
                    Gtemp.StandardConsumption = StandardConsumption;
                    Gtemp.TheoreticalDosage = TheoreticalDosage;
                    Gtemp.ActualUsage = ActualUsage;
                    Gtemp.StandardFormulaCost = StandardFormulaCost;
                    Gtemp.TheoreticalCost = TheoreticalCost;
                    Gtemp.ActualCost = ActualCost;
                    Gtemp.RealUnitCost = ActualUsage == 0 ? 0 :  (ActualCost / ActualUsage);
                    temp.Details.Add(Gtemp);
                }
                string order = model.ColumnSummary.ToString();
                var tempList = ((List<dynamic>)temp.Details).AsQueryable().OrderBy(order).ToList();
                dynamic tempR = new { Details = tempList };
                List.Add(tempR);
            }
            #endregion
            //列汇总项赋值
            result.ColumnList = ColumnList.OrderBy(m => m.OrderNum);
            //数据赋值
            result.DataList = List;
            string[] ColumnSummaryList = model.ColumnSummary.ToString().Split(",");
            //最终数据 排序好的数据 替换原始数据
            List<dynamic> ResultDataList = new List<dynamic>();
            #region 补全数据
            //补全数据
            foreach (var item in result.DataList)
            {
                //补全数据
                List<dynamic> Details = item.Details;
                if (Details.Count != ColumnList.Count && Details.Count > 0)
                {
                    foreach (var items in ColumnList)
                    {
                        if (Details.Where(m => m.OrderNum == items.OrderNum).Count() == 0)
                        {
                            //反射
                            object TempItems = items;
                            IDictionary<string, object> keyValuePairs = (IDictionary<string, object>)Details.FirstOrDefault();
                            dynamic newData = new ExpandoObject();
                            //补充 剩余属性 保持一致性
                            //是否赋值
                            foreach (var pitem in keyValuePairs.Keys)
                            {
                                bool IsBreak = false;
                                //过滤掉列汇总属性
                                for (int i = 0; i < TempItems.GetType().GetProperties().Count(); i++)
                                {
                                    var xitem = TempItems.GetType().GetProperties()[i];
                                    //如果当前属性跟汇总方式一样，则赋值汇总方式的值
                                    if (xitem.Name == pitem)
                                    {
                                        var xvalueToSet = TempItems.GetType().GetProperty(xitem.Name).GetValue(TempItems);
                                        _context.AddProperty(newData, pitem, xvalueToSet);
                                        //阻止二次覆盖
                                        IsBreak = true;
                                        break;
                                    }
                                }
                                if (!IsBreak)
                                {
                                    var valueToSet = keyValuePairs[pitem];
                                    _context.AddProperty(newData, pitem, valueToSet);
                                }
                            }
                            Details.Add(newData);
                        }
                    }
                }
                //生成小计
                if (ColumnSummaryList.Count() > 1 && Details.Count > 0)
                {
                    var tempSummary = "";
                    //存储小计 算完小计 在追加到 数据中
                    List<dynamic> jList = new List<dynamic>();
                    for (int i = 0; i < ColumnSummaryList.Count() - 1; i++)
                    {
                        if (string.IsNullOrEmpty(tempSummary))
                        {
                            //获取倒数第二个汇总方式，永远不取倒数第一的数据
                            foreach (var citem in ColumnSummaryList)
                            {
                                if (citem == ColumnSummaryList.LastOrDefault())
                                {
                                    break;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tempSummary))
                                    {
                                        tempSummary = citem;
                                    }
                                    else
                                    {
                                        tempSummary += "," + citem;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var tempArry = tempSummary.Split(',');
                            tempSummary = "";
                            foreach (var citem in tempArry)
                            {
                                if (citem == tempArry.LastOrDefault())
                                {
                                    break;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tempSummary))
                                    {
                                        tempSummary = citem;
                                    }
                                    else
                                    {
                                        tempSummary += "," + citem;
                                    }
                                }
                            }
                        }
                        string tempstrColumn = GetPropertyName(Details.FirstOrDefault());
                        string tempGroupBySummary = GetColumnSummaryHandler(tempSummary);
                        //tempstrColumn = tempstrColumn.Replace("it.FirstOrDefault()." + ColumnSummaryList.LastOrDefault(), $"{ColumnSummaryList.LastOrDefault()} = 小计 ");
                        var tempResult = Details.AsQueryable().GroupBy($"{tempGroupBySummary}", "it").ToDynamicList();
                        if (tempResult.Count > 0)
                        {
                            //小计专用小数点位数
                            foreach (var group in tempResult)
                            {
                                int index = 1;
                                // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                                dynamic key = group.Key;
                                IEnumerable<dynamic> elements = group;
                                var jObj = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(elements.FirstOrDefault()));
                                if (jObj != null)
                                {
                                    #region 重构OrerNum排序 以便于小计排序
                                    foreach (var groupItem in group)
                                    {
                                        if (groupItem.OrderNum == 0)
                                        {
                                            groupItem.OrderNum = Convert.ToDouble(Details.Max(m => m.OrderNum) + 1);
                                        }
                                    }
                                    #endregion
                                    // 使用 dynamic 类型计算 字段的总和
                                    var StandardConsumption  = elements.Sum(m => (decimal)m.StandardConsumption);
                                    var TheoreticalDosage      = elements.Sum(m => (decimal)m.TheoreticalDosage);
                                    var ActualUsage                =      elements.Sum(m => (decimal)m.ActualUsage);
                                    var StandardFormulaCost            = elements.Sum(m => (decimal)m.StandardFormulaCost);
                                    var TheoreticalCost                   = elements.Sum(m => (decimal)m.TheoreticalCost);
                                    var ActualCost           = elements.Sum(m => (decimal)m.ActualCost);
                                    var RealUnitCost             = elements.Sum(m => (decimal)m.RealUnitCost);
                                    //重构数据结构 赋值
                                    var Gtemp = elements.FirstOrDefault();
                                    jObj.StandardConsumption = StandardConsumption;
                                    jObj.TheoreticalDosage = TheoreticalDosage;
                                    jObj.ActualUsage = ActualUsage;
                                    jObj.StandardFormulaCost = StandardFormulaCost;
                                    jObj.TheoreticalCost = TheoreticalCost;
                                    jObj.ActualCost = ActualCost;
                                    jObj.RealUnitCost = RealUnitCost;
                                    _context.SetProperty(jObj, ColumnSummaryList[ColumnSummaryList.Count() - 1 - i], "小计");
                                    jObj.IsSum = 1;
                                    jObj.OrderNum = Convert.ToDouble(((IGrouping<dynamic, dynamic>)group).LastOrDefault().OrderNum + "." + index);
                                    jList.Add(jObj);
                                    index++;
                                }
                            }
                        }
                    }
                    //将已经算好的小计 追加到数据中
                    Details.AddRange(jList);
                    dynamic SumData = new ExpandoObject();
                    GetSumData(item, SumData);
                    ResultDataList.Add(new
                    {
                        Details = ((List<dynamic>)item.Details).OrderBy(m => m.OrderNum).ToDynamicList(),
                        SumData
                    }) ;
                }
                else
                {
                    dynamic SumData = new ExpandoObject();
                    GetSumData(item, SumData);
                    ResultDataList.Add(new
                    {
                        Details = ((List<dynamic>)item.Details).OrderBy(m => m.OrderNum).ToDynamicList(),
                        SumData
                    });
                }
            }
            #endregion
            if (ResultDataList.Count > 0)
            {
                result.DataList = ResultDataList;
            }
            return result;
        }
        /// <summary>
        /// 行汇总 列小计统计
        /// </summary>
        /// <param name="item"></param>
        /// <param name="SumData"></param>
        private void GetSumData(dynamic item, dynamic SumData)
        {
            SumData.StandardConsumption       = ((List<dynamic>)item.Details).Sum(m => (decimal)m.StandardConsumption);
            SumData.TheoreticalDosage         = ((List<dynamic>)item.Details).Sum(m => (decimal)m.TheoreticalDosage);
            SumData.ActualUsage             = ((List<dynamic>)item.Details).Sum(m => (decimal)m.ActualUsage);
            SumData.StandardFormulaCost    = ((List<dynamic>)item.Details).Sum(m => (decimal)m.StandardFormulaCost);
            SumData.TheoreticalCost         = ((List<dynamic>)item.Details).Sum(m => (decimal)m.TheoreticalCost);
            SumData.ActualCost          = ((List<dynamic>)item.Details).Sum(m => (decimal)m.ActualCost);
            SumData.RealUnitCost          = ((List<dynamic>)item.Details).Sum(m => (decimal)m.RealUnitCost);
        }
        /// <summary>
        /// 读取动态属性 用于 dynamicLINQ 查询列
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetPropertyName(object model)
        {
            IDictionary<string, object> dictionary = (IDictionary<string, object>)model;
            string str = "new ( ";
            foreach (var item in dictionary.Keys)
            {
                //手动计算 dynamic LINQ 对 dynamic 集合进行 sum 会抛出异常（也可能是写法有问题）
                if (item == "Credit" || item == "Debit")
                {
                    //str += $"it.Sum(x => x.{item}) AS {item}" + ",\n";
                }
                else
                {
                    str += "it.FirstOrDefault()." + item + " AS " + item + ",\n";
                }
            }
            str += " )";
            return str;
        }
        /// <summary>
        /// 通过汇总方式返回对应的属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetNameBySummary(string model)
        {
            string str = "new ( ";
            foreach (var item in model.Split(","))
            {
                str += "it.FirstOrDefault()." + item + " AS " + item + ",\n";
            }
            str += " )";
            return str;
        }
        /// <summary>
        /// 列汇总方式 处理
        /// 用于 dynamic LINQ
        /// 例子：org_en1,org_en2,org_en3 = new (org_en1 as org_en1,org_en2 as org_en2,org_en3 as org_en3)
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetColumnSummaryHandler(string column)
        {
            if (!string.IsNullOrEmpty(column))
            {
                string str = "new ( ";
                foreach (var item in column.Split(','))
                {
                    str += item + " as " + item + " ,\n";
                }
                str += " ) ";
                return str;
            }
            return " new (org_en3 as org_en3) ";
        }
    }
}
