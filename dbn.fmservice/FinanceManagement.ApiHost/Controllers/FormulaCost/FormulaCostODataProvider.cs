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
using Architecture.Common.Util;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 波尔莱特 - 定制化需求
    /// </summary>
    public class FormulaCostODataProvider
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

        public FormulaCostODataProvider(MS_StockCostODataProvider msCostProvider, IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
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
        }
        /// <summary>
        /// https://confluence.nxin.com/pages/viewpage.action?pageId=74679524
        /// </summary>
        /// <param name="model"></param>
        /// model.EnterSortIds = 单位id
        /// <returns></returns>
        public dynamic GetFormulaCostData(dynamic model)
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
            if (model.IsGroup == null)
            {
                model.IsGroup = "";
            }
            if (model.FormulaName == null)
            {
                model.FormulaName = "";
            }
            if (Convert.ToInt32(model.IsTons) == 2)
            {
                MeasureUnit = 1000;
            }
            #region 获取每条产品名称 最新数据
            string NewProduce = $@"SELECT MAX(t3.NumericalOrder) NumericalOrder from nxin_qlw_business.ms_formulaextend  t3
            inner join nxin_qlw_business.ms_formula t1 on t1.NumericalOrder = t3.NumericalOrder
            where t1.IsUse = 1 AND EnterpriseID IN ({model.EnterSortIds}) AND t1.DataDate <= '{model.DataDate}' and t3.PackingID <> 0
            {(!string.IsNullOrEmpty(model.FormulaName.ToString()) ? $@" AND t1.FormulaName LIKE  '%{model.FormulaName.ToString()}%'" : "")}
            GROUP BY t3.ProductId";
            var ProduceList = _context.DynamicSqlQuery(NewProduce);
            var ProduceNums = string.Join(',', ProduceList.Select(m=>m.NumericalOrder));
            if (string.IsNullOrEmpty(ProduceNums))
            {
                ProduceNums = "0";
            }
            #endregion
            #region SQL数据源
            string finlaySql = $@"
            SELECT t1.DataDate,LEFT(t1.DataDate,7) Month,t1.EnterpriseID,en.EnterpriseName,t1.FormulaName -- 配方名称
            ,t1.BaseQuantity,-- 配方基数
            t2.ProductID,bp.ProductName, -- 原料名称
            t2.ProportionQuantity, -- 耗用用量
            t3.ProductID as ProduceID, -- 产品名称ID
            bp2.ProductName as ProduceName -- 产品名称
            ,t3.PackingID, -- 包装ID
            bp3.ProductName as PackingName, -- 包装名称
            0.00 ProductMarketPrice
            from nxin_qlw_business.ms_formula t1
            INNER JOIN nxin_qlw_business.MS_FormulaDetail t2 ON t1.NumericalOrder=t2.NumericalOrder
            LEFT JOIN nxin_qlw_business.ms_formulaextend t3 ON t2.NumericalOrder=t3.NumericalOrder and t3.IsUse = true
            LEFT JOIN qlw_nxin_com.biz_product bp on bp.ProductId = t2.ProductId -- 原料名称
            LEFT JOIN qlw_nxin_com.biz_product bp2 on bp2.ProductId = t3.ProductId -- 产品名称
            LEFT JOIN qlw_nxin_com.biz_product bp3 on bp3.ProductId = t3.PackingID -- 包装名称
            LEFT JOIN qlw_nxin_com.biz_enterprise en on en.EnterpriseId = t1.EnterpriseId  -- 单位
            WHERE t1.IsUse = 1 AND t1.NumericalOrder IN ({ProduceNums})
            {(!string.IsNullOrEmpty(model.IsGroup.ToString()) ? $@" AND t1.IsGroup = {model.IsGroup.ToString()} " : "")}
            {(!string.IsNullOrEmpty(model.ProductIds.ToString()) ? $@" AND t3.ProductId IN ({model.ProductIds.ToString()})" : "")}
            ORDER BY t1.NumericalOrder desc 
            ";
            //配方商品价格设定
            string PriceSql = $@"select ff.DataDate,ffd.ProductID,ffd.MarketPrice from nxin_qlw_business.ms_formulaproductprice ff
            inner join nxin_qlw_business.ms_formulaproductpricedetail ffd on ffd.NumericalOrder = ff.NumericalOrder
            left join nxin_qlw_business.ms_formulaproductpriceext ffe on ffe.NumericalOrder = ffe.NumericalOrder
            where ff.GroupID = {_identityservice.GroupId} and DataDate <= '{model.DataDate}'";
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
            //只算原料
            for (int i = 0; i < Data.Count; i++)
            {
                var item = Data[i];
                //原料库存数据
                var MsTempData = MSData.Where(m => m.ProductID == item.ProductID && m.EnterPriseID == item.EnterpriseID).FirstOrDefault();
                if (MsTempData == null)
                {
                    MsTempData = new MS_StockCostResultModel() { qmUnitPrice  = 0};
                }
                //【单位耗用量】= 原料耗用数量 / 配方基数 * 1000
                double UnitConsumption = Convert.ToDouble(Data[i].ProportionQuantity / (Data[i].BaseQuantity == 0 ? 1: Data[i].BaseQuantity) * MeasureUnit);
                //【实际单位成本】=【原料库存实时成本】的期末单位成本 * 1000  （备注：取自 另一个报表【原料库存实时成本】）
                double ActualUnitCost = Convert.ToDouble(MsTempData.qmUnitPrice);
                //【实际成本】= 单位耗用量 * 实际单位成本   
                double ActualCost = UnitConsumption * ActualUnitCost;
                //【设定单位成本】= 【配方商品价格设定】市场价*1000  （备注：价格设定 日期  <= 日期，取自 【配方商品价格设定】 ）
                double SetUnitCost = item.ProductMarketPrice;
                //【设定成本】= 单位耗用量 * 设定单位成本
                double SetCost = UnitConsumption * SetUnitCost;
                //【单位成本差异】= 实际单位成本 - 设定单位成本
                double UnitCostVariance = ActualUnitCost - SetUnitCost;
                //【成本差异】 = 实际成本 - 设定成本
                double CostVariance = ActualCost - SetCost;


                //是否小计
                _context.AddProperty(Data[i], "IsSum", 0);
                //排序
                _context.AddProperty(Data[i], "OrderNum", 0.00);
                //单位耗用量
                _context.AddProperty(Data[i], "UnitConsumption", UnitConsumption);
                //实际单位成本
                _context.AddProperty(Data[i], "ActualUnitCost", ActualUnitCost);
                //实际成本
                _context.AddProperty(Data[i], "ActualCost", ActualCost);
                //设定单位成本 
                _context.AddProperty(Data[i], "SetUnitCost", SetUnitCost);
                //设定成本
                _context.AddProperty(Data[i], "SetCost", SetCost);
                //单位成本差异
                _context.AddProperty(Data[i], "UnitCostVariance", UnitCostVariance);
                //成本差异
                _context.AddProperty(Data[i], "CostVariance", CostVariance);
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
            #region 行汇总数据项
            //行汇总数据项
            var RowColumnList = new List<dynamic>();
            var RowColumnSummary = GetColumnSummaryHandler(model.RowColumnSummary.ToString());
            string rowColunm = GetNameBySummary(model.RowColumnSummary.ToString());
            // 使用 Dynamic Linq 库执行查询
            if (!string.IsNullOrEmpty(model.RowColumnSummary.ToString()))
            {
                string order = model.RowColumnSummary.ToString();
                RowColumnList = Data.AsQueryable().GroupBy($"{RowColumnSummary.ToString()}", "it").Select(rowColunm).OrderBy(order).ToDynamicList();
            }
            #endregion
            #region 数据
            //存在行汇总方式时
            List<dynamic> List = new List<dynamic>();
            //获取行汇总项目 所选项
            foreach (object item in RowColumnList)
            {
                //获取行名称
                string propName = model.RowColumnSummary.ToString();
                var value = item.GetType().GetProperty(propName).GetValue(item);
                //填充到数据集标识头
                dynamic temp = new { RowName = value, Details = new List<dynamic>() };
                string order = model.RowColumnSummary.ToString();
                //获取列项目字符串
                string colSummary = model.ColumnSummary.ToString();
                //获取行项目字符串
                string rowColSummary = model.ColumnSummary.ToString();
                //where 过滤 行项，dynamic LINQ 分组 列数据 得到最终数据
                var Details = Data.AsQueryable().GroupBy($"{ColumnSummary.ToString()}", "it").ToDynamicList();
                foreach (var group in Details)
                {
                    // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                    dynamic key = group.Key;
                    IEnumerable<dynamic> elements = group;
                    if (elements.FirstOrDefault().FormulaName == value.ToString())
                    {
                        var SetUnitCost = elements.Sum(m => (decimal)m.SetUnitCost);
                        var SetCost = elements.Sum(m => (decimal)m.SetCost);
                        var Gtemp = elements.FirstOrDefault();
                        Gtemp.SetUnitCost = SetUnitCost;
                        Gtemp.SetCost = SetCost;
                        temp.Details.Add(Gtemp);
                    }
                    else
                    {
                        IDictionary<string, object> keyValuePairs = (IDictionary<string, object>)elements.FirstOrDefault();
                        dynamic newData = new ExpandoObject();
                        //补充 剩余属性 保持一致性
                        //是否赋值
                        foreach (var keyValue in keyValuePairs)
                        {
                            var pitem = keyValue.Key;
                            if (!(keyValue.Value is string))
                            {
                                _context.AddProperty(newData, pitem, 0.00);
                            }
                            else
                            {
                                _context.AddProperty(newData, pitem, keyValue.Value);
                            }
                        }
                        temp.Details.Add(newData);
                    }
                }
                List.Add(temp);
            }
            if (RowColumnList.Count == 0)
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
                    var UnitConsumption = elements.Sum(m => (decimal)m.UnitConsumption);
                    var ActualUnitCost = elements.Sum(m => (decimal)m.ActualUnitCost);
                    var ActualCost = elements.Sum(m => (decimal)m.ActualCost);
                    var SetUnitCost = elements.Sum(m => (decimal)m.SetUnitCost);
                    var SetCost = elements.Sum(m => (decimal)m.SetCost);
                    var UnitCostVariance = elements.Sum(m => (decimal)m.UnitCostVariance);
                    var CostVariance = elements.Sum(m => (decimal)m.CostVariance);
                    //重构数据结构 赋值
                    var Gtemp = elements.FirstOrDefault();
                    Gtemp.UnitConsumption = UnitConsumption;
                    Gtemp.ActualUnitCost = ActualUnitCost;
                    Gtemp.ActualCost = ActualCost;
                    Gtemp.SetUnitCost = SetUnitCost;
                    Gtemp.SetCost = SetCost;
                    Gtemp.UnitCostVariance = UnitCostVariance;
                    Gtemp.CostVariance = CostVariance;
                    temp.Details.Add(Gtemp);
                }
                List.Add(temp);
            }
            #endregion
            //列汇总项赋值
            result.ColumnList = ColumnList.OrderBy(m => m.OrderNum);
            //行汇总项赋值
            result.RowColumnList = RowColumnList;
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
                        if (Details.Count != ColumnList.Count)
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
                                    if (valueToSet is decimal || valueToSet is double)
                                    {
                                        _context.AddProperty(newData, pitem, 0.00);
                                    }
                                    else if (pitem == "OrderNum")
                                    {
                                        _context.AddProperty(newData, pitem, 0);
                                    }
                                    else
                                    {
                                        _context.AddProperty(newData, pitem, "");
                                    }
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
                        var tempResult = Details.Where(m=>!string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").AsQueryable().GroupBy($"{tempGroupBySummary}", "it").ToDynamicList();
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
                                        if (groupItem.OrderNum == 0 || groupItem.OrderNum.ToString() == "")
                                        {
                                            groupItem.OrderNum = Convert.ToDouble(Details.Max(m => m.OrderNum) + 1);
                                        }
                                    }
                                    #endregion
                                    // 使用 dynamic 类型计算 字段的总和
                                    var UnitConsumption = elements.Sum(m => (decimal)m.UnitConsumption);
                                    var ActualCost = elements.Sum(m => (decimal)m.ActualCost);
                                    var ActualUnitCost = UnitConsumption == 0 ? 0 : ActualCost / UnitConsumption;
                                    var SetUnitCost = elements.Sum(m => (decimal)m.SetUnitCost);
                                    var SetCost = elements.Sum(m => (decimal)m.SetCost);
                                    var UnitCostVariance = elements.Sum(m => (decimal)m.UnitCostVariance);
                                    var CostVariance = elements.Sum(m => (decimal)m.CostVariance);
                                    //重构数据结构 赋值
                                    var Gtemp = elements.FirstOrDefault();
                                    jObj.UnitConsumption = UnitConsumption;
                                    jObj.ActualUnitCost = ActualUnitCost;
                                    jObj.ActualCost = ActualCost;
                                    jObj.SetUnitCost = SetUnitCost;
                                    jObj.SetCost = SetCost;
                                    jObj.UnitCostVariance = UnitCostVariance;
                                    jObj.CostVariance = CostVariance;
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
                    //排序
                    ResultDataList.Add(new { RowName = result.RowColumnList.Count == 0 ? "" : item.RowName, SumData, Details = ((List<dynamic>)item.Details).OrderBy(m => m.OrderNum).ToDynamicList() });
                }
                else
                {
                    string order = model.ColumnSummary.ToString();
                    dynamic SumData = new ExpandoObject();
                    GetSumData(item, SumData);
                    ResultDataList.Add(new { RowName = result.RowColumnList.Count == 0 ? "" : item.RowName, SumData, Details = ((List<dynamic>)item.Details).AsQueryable().OrderBy(order).ToDynamicList() });
                }
            }
            if (ResultDataList.Count > 0)
            {
                result.DataList = ResultDataList;
            }
            #endregion
            return result;
        }
        /// <summary>
        /// 行汇总 列小计统计
        /// </summary>
        /// <param name="item"></param>
        /// <param name="SumData"></param>
        private void GetSumData(dynamic item, dynamic SumData)
        {
            SumData.UnitConsumption = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.UnitConsumption);
            SumData.ActualUnitCost = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.ActualUnitCost);
            SumData.ActualCost = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.ActualCost);
            SumData.SetUnitCost = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.SetUnitCost);
            SumData.SetCost = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.SetCost);
            SumData.UnitCostVariance = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.UnitCostVariance);
            SumData.CostVariance = ((List<dynamic>)item.Details).Where(m => !string.IsNullOrEmpty(m.IsSum.ToString()) && m.IsSum.ToString() == "0").Sum(m => (decimal)m.CostVariance);
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
