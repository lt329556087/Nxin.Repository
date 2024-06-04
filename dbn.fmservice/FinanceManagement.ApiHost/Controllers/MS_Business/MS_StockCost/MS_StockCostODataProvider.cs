using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Util;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class MS_StockCostODataProvider
    {
        ILogger<MS_StockCostODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        FMBaseCommon _baseUnit;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;

        public MS_StockCostODataProvider(IIdentityService identityservice, QlwCrossDbContext context, TreeModelODataProvider treeModel, ILogger<MS_StockCostODataProvider> logger, FMBaseCommon baseUnit, HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
            _treeModel = treeModel;
            _baseUnit = baseUnit;
            _httpClientUtil1 = httpClientUtil1;
            _hostCongfiguration = hostCongfiguration;
        }
        public List<MS_StockCostResultModel> GetReportData(MS_StockCostSearch model)
        {
            var option = _baseUnit.OptionConfigValue("1820290943410000114", _identityservice.EnterpriseId);
            string notinWhere = string.Empty;
            if (option != "0" && !string.IsNullOrEmpty(option))
            {
                notinWhere = "201610140104402102, 201610140104402103,201610140104402104,201610140104402105";
            }
            else
            {
                notinWhere = "201611180104402301, 201611180104402302";
            }
            model.LastBeginDate = Convert.ToDateTime(model.BeginDate).AddMonths(-1).ToString("yyyy-MM-dd");
            model.LastEndDate = Convert.ToDateTime(model.EndDate).AddMonths(-1).ToString("yyyy-MM-dd");
            string sqlWhere = string.Empty;
            if (string.IsNullOrEmpty(model.EnterpriseIds))
            {
                model.EnterpriseIds = _baseUnit.GetAuthorEnterpise(_identityservice.EnterpriseId, _identityservice.UserId);
            }
            if (!string.IsNullOrEmpty(model.StockTypes))
            {
                sqlWhere += @$" and pd.StockType in ({model.StockTypes}) ";
            }
            if (!string.IsNullOrEmpty(model.ProductIds))
            {
                sqlWhere += @$" and pd.ProductID in ({model.ProductIds}) ";
            }
            if (!string.IsNullOrEmpty(model.ProductGroupIds))
            {
                sqlWhere += @$" and pn.ProductGroupID in ({model.ProductGroupIds}) ";
            }
            if (!string.IsNullOrEmpty(model.ProductClassIds))
            {
                sqlWhere += @$" and pc.ClassificationID in ({model.ProductClassIds}) ";
            }
            string qcSql = $@"-- 1、从成本汇总取本期期初
                            SELECT t1.EnterPriseID,ent.EnterPriseName,pc.cAxis,pc.`cFullClassName`,pn.ProductGroupID,pn.ProductGroupName,t2.ProductID,pd1.ProductName,
                            SUM(t2.Quantity) qcQuantity,ROUND(t2.UnitPrice,4)  AS  qcUnitPrice,SUM(t2.Amount) AS qcAmount
                            FROM nxin_qlw_business.`ms_cost` t1
                            INNER JOIN nxin_qlw_business.`ms_costdetail` t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                            INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterPriseID= ent.`EnterpriseID`
                            LEFT JOIN nxin_qlw_business.BIZ_ProductDetail pd ON pd.ProductID = t2.ProductID AND t1.EnterpriseID = pd.EnterpriseID
                            LEFT JOIN qlw_nxin_com.BIZ_Product pd1 ON  pd1.ProductID = t2.ProductID AND pd1.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroup pn ON pd1.ProductGroupID=pn.ProductGroupID AND pn.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroupclassification pc ON pn.ClassificationID=pc.ClassificationID AND pc.EnterpriseID={_identityservice.GroupId}
                            WHERE t1.EnterpriseID in ({model.EnterpriseIds}) AND DATE_FORMAT(t1.DataDate, '%Y-%m')= DATE_FORMAT('{model.LastBeginDate}', '%Y-%m') AND  t2.CostProject=201705250104402106 
                            {sqlWhere}
                            GROUP BY t1.`EnterpriseID`,t2.`ProductID` ";
            var qcList = _context.DynamicSqlQuery(qcSql);
            string fsSql = $@"-- 2、从采购单取本期采购入库
                            SELECT t1.EnterPriseID,ent.EnterPriseName,pc.cAxis,pc.`cFullClassName`,pn.ProductGroupID,pn.ProductGroupName,t2.ProductID,pd1.ProductName,
                            SUM(t2.`SettlementQuantity`) AS inQuantity,ROUND( SUM(t2.`SettlementQuantity`)/SUM(t2.AmountTotal),4) AS inUnitPrice,SUM(t2.AmountTotal) AS inAmount
                            FROM nxin_qlw_business.`pm_purchase` t1
                            INNER JOIN nxin_qlw_business.`pm_purchasedetail` t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                            INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterPriseID= ent.`EnterpriseID`
                            LEFT JOIN nxin_qlw_business.BIZ_ProductDetail pd ON pd.ProductID = t2.ProductID AND t1.EnterpriseID = pd.EnterpriseID
                            LEFT JOIN qlw_nxin_com.BIZ_Product pd1 ON  pd1.ProductID = t2.ProductID AND pd1.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroup pn ON pd1.ProductGroupID=pn.ProductGroupID AND pn.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroupclassification pc ON pn.ClassificationID=pc.ClassificationID AND pc.EnterpriseID={_identityservice.GroupId}
                            WHERE t1.EnterpriseID in ({model.EnterpriseIds})  AND t1.`DataDate` BETWEEN '{model.BeginDate}' AND '{model.EndDate}'   
                            {sqlWhere}
                            GROUP BY t1.`EnterpriseID`,t2.`ProductID`  ";
            var fsList = _context.DynamicSqlQuery(fsSql);

            string qmSql = $@"
                                -- 3.1从存货中间表取上月期初数量
                                SELECT a.EnterPriseID,ent.EnterPriseName,pc.cAxis,pc.`cFullClassName`,pn.ProductGroupID,pn.ProductGroupName,a.ProductID,pd1.ProductName,SUM(a.Quantity) AS qmQuantity
                                FROM NXin_Qlw_Business.WM_MidWarehouseBalance a
                                INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON a.EnterPriseID= ent.`EnterpriseID`
                                LEFT JOIN nxin_qlw_business.BIZ_ProductDetail pd ON pd.ProductID = a.ProductID AND a.EnterpriseID = pd.EnterpriseID
                                LEFT JOIN qlw_nxin_com.BIZ_Product pd1 ON  pd1.ProductID = a.ProductID AND pd1.EnterpriseID={model.GroupId}
                                LEFT JOIN qlw_nxin_com.biz_productgroup pn ON pd1.ProductGroupID=pn.ProductGroupID AND pn.EnterpriseID={model.GroupId}
                                LEFT JOIN qlw_nxin_com.biz_productgroupclassification pc ON pn.ClassificationID=pc.ClassificationID AND pc.EnterpriseID={model.GroupId}
                                WHERE a.EnterpriseID IN ({model.EnterpriseIds}) AND DATE_FORMAT(a.DataDate, '%Y-%m')= DATE_FORMAT('{model.LastEndDate}', '%Y-%m') {sqlWhere}
                                GROUP BY a.EnterpriseID,a.ProductID 
                                UNION ALL 
                                -- 3.2本期出入库数量
                                SELECT a.EnterPriseID,ent.EnterPriseName,pc.cAxis,pc.`cFullClassName`,pn.ProductGroupID,pn.ProductGroupName,b.ProductID,pd1.ProductName,
                                SUM(CASE WHEN c.`cDictCode`=1 THEN b.Quantity WHEN c.`cDictCode`=2 THEN -b.`Quantity` ELSE 0 END) AS qmQuantity
                                FROM NXin_Qlw_Business.WM_WarehouseStock a
                                INNER JOIN NXin_Qlw_Business.WM_WarehouseStockDetail b ON a.NumericalOrder = b.NumericalOrder
                                LEFT JOIN qlw_nxin_com.BSDataDict c ON a.WarehouseStockAbstract = c.DictID
                                INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON a.EnterPriseID= ent.`EnterpriseID`
                                LEFT JOIN nxin_qlw_business.BIZ_ProductDetail pd ON pd.ProductID = b.ProductID AND a.EnterpriseID = pd.EnterpriseID
                                LEFT JOIN qlw_nxin_com.BIZ_Product pd1 ON  pd1.ProductID = b.ProductID AND pd1.EnterpriseID={model.GroupId}
                                LEFT JOIN qlw_nxin_com.biz_productgroup pn ON pd1.ProductGroupID=pn.ProductGroupID AND pn.EnterpriseID={model.GroupId}
                                LEFT JOIN qlw_nxin_com.biz_productgroupclassification pc ON pn.ClassificationID=pc.ClassificationID AND pc.EnterpriseID={model.GroupId}
                                WHERE a.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' {sqlWhere}
                                AND a.EnterpriseID IN ({model.EnterpriseIds})
                                AND a.WarehouseStockType NOT IN ({notinWhere}) 
                                GROUP BY a.EnterpriseID,b.ProductID
                                ";
            var qmList = _context.DynamicSqlQuery(qmSql);
            //期初  发生 期末数据整合
            List<MS_StockCostResultModel> result = GetMS_StockCostResultModel(qcList, fsList, qmList);
            //获取分组属性
            ViewModel viewModel = new ViewModel();
            GroupModel groupModel = SetGroupModel(model.SummaryType, viewModel);
            //数据根据进行汇总方式进行分组
            result = result.GroupBy(s => groupModel.GroupBy(s)).Select(s => new MS_StockCostResultModel()
            {
                EnterPriseID = s.Key.EnterPriseID,
                EnterPriseName = result.Where(n => n.EnterPriseID == s.Key.EnterPriseID)?.FirstOrDefault()?.EnterPriseName,
                ProductGroupID = s.Key.ProductGroupID,
                ProductGroupName = result.Where(n => n.ProductGroupID == s.Key.ProductGroupID)?.FirstOrDefault()?.ProductGroupName,
                ProductID = s.Key.ProductID,
                ProductName = result.Where(n => n.ProductID == s.Key.ProductID)?.FirstOrDefault()?.ProductName,
                ClassificationID1 = s.Key.ClassificationID1,
                ClassificationID2 = s.Key.ClassificationID2,
                ClassificationID3 = s.Key.ClassificationID3,
                ClassificationID4 = s.Key.ClassificationID4,
                ClassificationID5 = s.Key.ClassificationID5,
                ClassificationID6 = s.Key.ClassificationID6,
                ClassificationName1 = result.Where(n => n.ClassificationID1 == s.Key.ClassificationID1)?.FirstOrDefault()?.ClassificationName1,
                ClassificationName2 = result.Where(n => n.ClassificationID2 == s.Key.ClassificationID2)?.FirstOrDefault()?.ClassificationName2,
                ClassificationName3 = result.Where(n => n.ClassificationID3 == s.Key.ClassificationID3)?.FirstOrDefault()?.ClassificationName3,
                ClassificationName4 = result.Where(n => n.ClassificationID4 == s.Key.ClassificationID4)?.FirstOrDefault()?.ClassificationName4,
                ClassificationName5 = result.Where(n => n.ClassificationID5 == s.Key.ClassificationID5)?.FirstOrDefault()?.ClassificationName5,
                ClassificationName6 = result.Where(n => n.ClassificationID6 == s.Key.ClassificationID6)?.FirstOrDefault()?.ClassificationName6,
                qcAmount = s.Sum(n => n.qcAmount),
                qcQuantity = s.Sum(n => n.qcQuantity),
                inAmount = s.Sum(n => n.inAmount),
                inQuantity = s.Sum(n => n.inQuantity),
                qmQuantity = s.Sum(n => n.qmQuantity),
            }).ToList();

            //数据处理
            foreach (var item in result)
            {
                string summaryType = "";
                string summaryTypeName = "";
                foreach (var summary in model.SummaryType)
                {
                    switch (summary)
                    {
                        case "EnterPriseID":
                            summaryType += item.EnterPriseID + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.EnterPriseName) ? item.EnterPriseName : " ") + "~";
                            break;
                        case "ProductID":
                            summaryType += item.ProductID + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ProductName) ? item.ProductName : " ") + "~";
                            break;
                        case "ProductGroupID":
                            summaryType += item.ProductGroupID + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ProductGroupName) ? item.ProductGroupName : " ") + "~";
                            break;
                        case "ClassificationID1":
                            summaryType += item.ClassificationID1 + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ClassificationName1) ? item.ClassificationName1 : " ") + "~";
                            break;
                        case "ClassificationID2":
                            summaryType += item.ClassificationID2 + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ClassificationName2) ? item.ClassificationName2 : " ") + "~";
                            break;
                        case "ClassificationID3":
                            summaryType += item.ClassificationID3 + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ClassificationName3) ? item.ClassificationName3 : " ") + "~";
                            break;
                        case "ClassificationID4":
                            summaryType += item.ClassificationID4 + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ClassificationName4) ? item.ClassificationName4 : " ") + "~";
                            break;
                        case "ClassificationID5":
                            summaryType += item.ClassificationID5 + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ClassificationName5) ? item.ClassificationName5 : " ") + "~";
                            break;
                        case "ClassificationID6":
                            summaryType += item.ClassificationID6 + "~";
                            summaryTypeName += (!string.IsNullOrEmpty(item.ClassificationName6) ? item.ClassificationName6 : " ") + "~";
                            break;
                        default:
                            break;
                    }
                }
                item.SummaryType = summaryType.TrimEnd('~');
                item.SummaryTypeName = summaryTypeName.TrimEnd('~');
                //item.qmUnitPrice = (item.qcAmount + item.inAmount) == 0 || (item.qcQuantity + item.inQuantity) == 0 ? 0 : Math.Round((item.qcAmount + item.inAmount) / (item.qcQuantity + item.inQuantity), 4);
                //item.qmAmount = Math.Round(item.qmQuantity * item.qmUnitPrice, 4);
            }
            //获取小计和合计
            result = MakeSubTotal(result, viewModel);
            //计算平均单据
            foreach (var item in result)
            {
                item.qcUnitPrice = item.qcAmount == 0 || item.qcQuantity == 0 ? 0 : Math.Round(item.qcAmount / item.qcQuantity, 4);
                item.inUnitPrice = item.inAmount == 0 || item.inQuantity == 0 ? 0 : Math.Round(item.inAmount / item.inQuantity, 4);
                item.qmUnitPrice = (item.qcAmount + item.inAmount) == 0 || (item.qcQuantity + item.inQuantity) == 0 ? 0 : Math.Round((item.qcAmount + item.inAmount) / (item.qcQuantity + item.inQuantity), 4);
                item.qmAmount = Math.Round(item.qmQuantity * item.qmUnitPrice, 2);
            }
            result.RemoveAll(s => s.qcAmount == 0 && s.qcQuantity == 0 && s.qcUnitPrice == 0 && s.inQuantity == 0 && s.inUnitPrice == 0 && s.inAmount == 0 && s.qmAmount == 0 && s.qmQuantity == 0 && s.qmUnitPrice == 0);
            return result;
        }
        /// <summary>
        /// 获取分组值
        /// </summary>
        /// <param name="SummaryType"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private GroupModel SetGroupModel(List<string> SummaryType, ViewModel viewModel)
        {
            GroupModel groupModel = new GroupModel();
            foreach (var item in SummaryType)
            {
                switch (item)
                {
                    case "EnterPriseID":
                        groupModel.EnterPriseID = true;
                        viewModel.SummaryTypeName += "单位=";
                        break;
                    case "ProductID":
                        groupModel.ProductID = true;
                        viewModel.SummaryTypeName += "商品代号=";
                        break;
                    case "ProductGroupID":
                        groupModel.ProductGroupID = true;
                        viewModel.SummaryTypeName += "商品名称=";
                        break;
                    case "ClassificationID1":
                        groupModel.ClassificationID1 = true;
                        viewModel.SummaryTypeName += "商品分类一级=";
                        break;
                    case "ClassificationID2":
                        groupModel.ClassificationID2 = true;
                        viewModel.SummaryTypeName += "商品分类二级=";
                        break;
                    case "ClassificationID3":
                        groupModel.ClassificationID3 = true;
                        viewModel.SummaryTypeName += "商品分类三级=";
                        break;
                    case "ClassificationID4":
                        groupModel.ClassificationID4 = true;
                        viewModel.SummaryTypeName += "商品分类四级=";
                        break;
                    case "ClassificationID5":
                        groupModel.ClassificationID5 = true;
                        viewModel.SummaryTypeName += "商品分类五级=";
                        break;
                    case "ClassificationID6":
                        groupModel.ClassificationID6 = true;
                        viewModel.SummaryTypeName += "商品分类六级=";
                        break;
                    default:
                        break;
                }
            }
            viewModel.SummaryTypeName = viewModel.SummaryTypeName.TrimEnd('=');
            return groupModel;
        }
        /// <summary>
        /// 数据合并
        /// </summary>
        /// <param name="qcList"></param>
        /// <param name="fsList"></param>
        /// <param name="qmList"></param>
        /// <returns></returns>
        public  List<MS_StockCostResultModel> GetMS_StockCostResultModel(List<dynamic> qcList, List<dynamic> fsList, List<dynamic> qmList)
        {
            List<MS_StockCostResultModel> result = new List<MS_StockCostResultModel>();
            foreach (var item in qcList)
            {
                MS_StockCostResultModel domain = new MS_StockCostResultModel()
                {
                    EnterPriseID = item.EnterPriseID,
                    EnterPriseName = item.EnterPriseName,
                    cAxis = item.cAxis,
                    cFullClassName = item.cFullClassName,
                    ProductGroupID = item.ProductGroupID,
                    ProductGroupName = item.ProductGroupName,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    qcAmount = string.IsNullOrEmpty(item.qcAmount + "") ? 0 : Math.Round((decimal)item.qcAmount, 2),
                    qcUnitPrice = string.IsNullOrEmpty(item.qcUnitPrice + "") ? 0 : Math.Round((decimal)item.qcUnitPrice, 4),
                    qcQuantity = string.IsNullOrEmpty(item.qcQuantity + "") ? 0 : Math.Round((decimal)item.qcQuantity, 2),
                };
                SplitClass(domain.cAxis, domain);
                SplitClass(domain.cFullClassName, domain);
                result.Add(domain);
            }
            foreach (var item in fsList)
            {
                MS_StockCostResultModel domain = new MS_StockCostResultModel()
                {
                    EnterPriseID = item.EnterPriseID,
                    EnterPriseName = item.EnterPriseName,
                    cAxis = item.cAxis,
                    cFullClassName = item.cFullClassName,
                    ProductGroupID = item.ProductGroupID,
                    ProductGroupName = item.ProductGroupName,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    inAmount = string.IsNullOrEmpty(item.inAmount + "") ? 0 : Math.Round((decimal)item.inAmount, 2),
                    inQuantity = string.IsNullOrEmpty(item.inQuantity + "") ? 0 : Math.Round((decimal)item.inQuantity, 2),
                    inUnitPrice = string.IsNullOrEmpty(item.inUnitPrice + "") ? 0 : Math.Round((decimal)item.inUnitPrice, 4),
                };
                SplitClass(domain.cAxis, domain);
                SplitClass(domain.cFullClassName, domain);
                result.Add(domain);
            }
            foreach (var item in qmList)
            {
                MS_StockCostResultModel domain = new MS_StockCostResultModel()
                {
                    EnterPriseID = item.EnterPriseID,
                    EnterPriseName = item.EnterPriseName,
                    cAxis = item.cAxis,
                    cFullClassName = item.cFullClassName,
                    ProductGroupID = item.ProductGroupID,
                    ProductGroupName = item.ProductGroupName,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    qmQuantity = string.IsNullOrEmpty(item.qmQuantity + "") ? 0 : Math.Round((decimal)item.qmQuantity, 2),
                };
                SplitClass(domain.cAxis, domain);
                SplitClass(domain.cFullClassName, domain);
                result.Add(domain);
            }
            return result;
        }
        /// <summary>
        /// 拆分商品分类
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="model"></param>
        private void SplitClass(string axis, MS_StockCostResultModel model)
        {
            List<string> axisList = new List<string>();
            if (axis.Contains('$'))
            {
                model.cAxis = axis;
                axisList = axis.Split('$').ToList();
            }
            if (axis.Contains('/'))
            {
                model.cFullClassName = axis;
                axisList = axis.Split('/').ToList();
            }
            var classList = axisList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            for (int i = 0; i < classList.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(classList[i])) continue;
                if (axis.Contains('$'))
                {
                    switch (i)
                    {
                        case 0:
                            model.ClassificationID1 = classList[i];
                            break;
                        case 1:
                            model.ClassificationID2 = classList[i];
                            break;
                        case 2:
                            model.ClassificationID3 = classList[i];
                            break;
                        case 3:
                            model.ClassificationID4 = classList[i];
                            break;
                        case 4:
                            model.ClassificationID5 = classList[i];
                            break;
                        case 5:
                            model.ClassificationID6 = classList[i];
                            break;
                    }
                }
                if (axis.Contains('/'))
                {
                    switch (i)
                    {
                        case 0:

                            model.ClassificationName1 = classList[i];
                            break;
                        case 1:
                            model.ClassificationName2 = classList[i];
                            break;
                        case 2:
                            model.ClassificationName3 = classList[i];
                            break;
                        case 3:
                            model.ClassificationName4 = classList[i];
                            break;
                        case 4:
                            model.ClassificationName5 = classList[i];
                            break;
                        case 5:
                            model.ClassificationName6 = classList[i];
                            break;
                    }
                }

            }
        }
        /// <summary>
        /// 获取小计
        /// </summary>
        /// <param name="data"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public List<MS_StockCostResultModel> MakeSubTotal(List<MS_StockCostResultModel> data, ViewModel viewModel)
        {
            if (data == null || data.Count == 0)
            {
                return data;
            }
            data = data.AddSubTotal((subList) =>
            {
                var obj = new MS_StockCostResultModel()
                {
                    qcQuantity = subList.Sum(n => n.qcQuantity),
                    qcAmount = subList.Sum(n => n.qcAmount),
                    inQuantity = subList.Sum(n => n.inQuantity),
                    inAmount = subList.Sum(n => n.inAmount),
                    qmQuantity = subList.Sum(n => n.qmQuantity),
                    qmAmount = subList.Sum(n => n.qmAmount),
                };
                return obj;
            }, viewModel).ToList();
            return data;
        }
        /// <summary>
        /// 获取汇总方式
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<ReportSummary> GetReportSummary(AuthenticationHeaderValue token)
        {
            List<ReportSummary> list = new List<ReportSummary>()
            {
                new ReportSummary(){Name="单位",Value="EnterPriseID",DataValue="EnterPriseID"},
                new ReportSummary(){Name="商品代号",Value="ProductID",DataValue="ProductID"},
                new ReportSummary(){Name="商品名称",Value="ProductGroupID",DataValue="ProductGroupID"},
            };
            var result = _httpClientUtil1.PostJsonAsync<List<TreeModelODataEntity>>(_hostCongfiguration._wgUrl + "/dbn/fm/api/FMBase/GetTreeModelAsync", new { SortID = 1001, GroupID = _identityservice.GroupId }, (a) => { a.Authorization = token; }).Result;
            if (result == null)
            {
                return list;
            }
            int maxRank = result.Max(s => s.Rank);
            for (int i = 0; i < maxRank; i++)
            {
                list.Add(new ReportSummary() { Name = $"商品分类{i + 1}级", Value = $"ClassificationID{i + 1}", DataValue = $"ClassificationName{i + 1}" });
            }
            return list;
        }
        /// <summary>
        /// 配方成本对比表，生产成本分析表 专用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<MS_StockCostResultModel> GetReportByQMAmount(dynamic model)
        {
            if (model.DataDate == null)
            {
                model.DataDate = model.BeginDate.ToString();
            }
            else
            {
                model.BeginDate = model.DataDate.ToString();
                model.EndDate = model.DataDate.ToString();
            }
            if (model.EnterSortIds == null)
            {
                model.EnterSortIds = "0";
            }
            string qcSql = $@"-- 1、从成本汇总取本期期初
                            SELECT t1.EnterPriseID,ent.EnterPriseName,pc.cAxis,pc.`cFullClassName`,pn.ProductGroupID,pn.ProductGroupName,t2.ProductID,pd1.ProductName,
                            SUM(t2.Quantity) qcQuantity,ROUND(t2.UnitPrice,4)  AS  qcUnitPrice,SUM(t2.Amount) AS qcAmount
                            FROM nxin_qlw_business.`ms_cost` t1
                            INNER JOIN nxin_qlw_business.`ms_costdetail` t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                            INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterPriseID= ent.`EnterpriseID`
                            LEFT JOIN nxin_qlw_business.BIZ_ProductDetail pd ON pd.ProductID = t2.ProductID AND t1.EnterpriseID = pd.EnterpriseID
                            LEFT JOIN qlw_nxin_com.BIZ_Product pd1 ON  pd1.ProductID = t2.ProductID AND pd1.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroup pn ON pd1.ProductGroupID=pn.ProductGroupID AND pn.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroupclassification pc ON pn.ClassificationID=pc.ClassificationID AND pc.EnterpriseID={_identityservice.GroupId}
                            WHERE t1.EnterpriseID in ({model.EnterSortIds}) AND DATE_FORMAT(t1.DataDate, '%Y-%m')= DATE_FORMAT('{((DateTime)Convert.ToDateTime(model.DataDate)).AddMonths(-1).ToString("yyyy-MM-dd")}', '%Y-%m') AND  t2.CostProject=201705250104402106 
                            GROUP BY t1.`EnterpriseID`,t2.`ProductID` ";
            var qcList = _context.DynamicSqlQuery(qcSql);
            string fsSql = $@"-- 2、从采购单取本期采购入库
                            SELECT t1.EnterPriseID,ent.EnterPriseName,pc.cAxis,pc.`cFullClassName`,pn.ProductGroupID,pn.ProductGroupName,t2.ProductID,pd1.ProductName,
                            SUM(t2.`SettlementQuantity`) AS inQuantity,ROUND( SUM(t2.`SettlementQuantity`)/SUM(t2.AmountTotal),4) AS inUnitPrice,SUM(t2.AmountTotal) AS inAmount
                            FROM nxin_qlw_business.`pm_purchase` t1
                            INNER JOIN nxin_qlw_business.`pm_purchasedetail` t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                            INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterPriseID= ent.`EnterpriseID`
                            LEFT JOIN nxin_qlw_business.BIZ_ProductDetail pd ON pd.ProductID = t2.ProductID AND t1.EnterpriseID = pd.EnterpriseID
                            LEFT JOIN qlw_nxin_com.BIZ_Product pd1 ON  pd1.ProductID = t2.ProductID AND pd1.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroup pn ON pd1.ProductGroupID=pn.ProductGroupID AND pn.EnterpriseID={_identityservice.GroupId}
                            LEFT JOIN qlw_nxin_com.biz_productgroupclassification pc ON pn.ClassificationID=pc.ClassificationID AND pc.EnterpriseID={_identityservice.GroupId}
                            WHERE t1.EnterpriseID in ({model.EnterSortIds})  AND t1.`DataDate` BETWEEN '{model.BeginDate}' AND '{model.EndDate}'   
                            GROUP BY t1.`EnterpriseID`,t2.`ProductID`  ";
            var fsList = _context.DynamicSqlQuery(fsSql);
            var result = GetMS_StockCostResultModel(qcList,fsList,new List<dynamic>());
            GroupModel groupModel = SetGroupModel(new List<string>() { "EnterPriseID", "ProductID" }, new ViewModel() { });
            //数据根据进行汇总方式进行分组
            result = result.GroupBy(s => groupModel.GroupBy(s)).Select(s => new MS_StockCostResultModel()
            {
                EnterPriseID = s.Key.EnterPriseID,
                EnterPriseName = result.Where(n => n.EnterPriseID == s.Key.EnterPriseID)?.FirstOrDefault()?.EnterPriseName,
                ProductGroupID = s.Key.ProductGroupID,
                ProductGroupName = result.Where(n => n.ProductGroupID == s.Key.ProductGroupID)?.FirstOrDefault()?.ProductGroupName,
                ProductID = s.Key.ProductID,
                ProductName = result.Where(n => n.ProductID == s.Key.ProductID)?.FirstOrDefault()?.ProductName,
                ClassificationID1 = s.Key.ClassificationID1,
                ClassificationID2 = s.Key.ClassificationID2,
                ClassificationID3 = s.Key.ClassificationID3,
                ClassificationID4 = s.Key.ClassificationID4,
                ClassificationID5 = s.Key.ClassificationID5,
                ClassificationID6 = s.Key.ClassificationID6,
                ClassificationName1 = result.Where(n => n.ClassificationID1 == s.Key.ClassificationID1)?.FirstOrDefault()?.ClassificationName1,
                ClassificationName2 = result.Where(n => n.ClassificationID2 == s.Key.ClassificationID2)?.FirstOrDefault()?.ClassificationName2,
                ClassificationName3 = result.Where(n => n.ClassificationID3 == s.Key.ClassificationID3)?.FirstOrDefault()?.ClassificationName3,
                ClassificationName4 = result.Where(n => n.ClassificationID4 == s.Key.ClassificationID4)?.FirstOrDefault()?.ClassificationName4,
                ClassificationName5 = result.Where(n => n.ClassificationID5 == s.Key.ClassificationID5)?.FirstOrDefault()?.ClassificationName5,
                ClassificationName6 = result.Where(n => n.ClassificationID6 == s.Key.ClassificationID6)?.FirstOrDefault()?.ClassificationName6,
                qcAmount = s.Sum(n => n.qcAmount),
                qcQuantity = s.Sum(n => n.qcQuantity),
                inAmount = s.Sum(n => n.inAmount),
                inQuantity = s.Sum(n => n.inQuantity),
                qmQuantity = s.Sum(n => n.qmQuantity),
            }).ToList();
            //计算平均单据
            foreach (var item in result)
            {
                item.qmUnitPrice = (item.qcAmount + item.inAmount) == 0 || (item.qcQuantity + item.inQuantity) == 0 ? 0 : Math.Round((item.qcAmount + item.inAmount) / (item.qcQuantity + item.inQuantity), 4);
            }
            return result;
        }
    }
}
