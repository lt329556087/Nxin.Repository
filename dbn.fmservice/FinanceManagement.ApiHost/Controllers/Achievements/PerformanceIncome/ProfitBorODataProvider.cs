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
using Architecture.Common.Util;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 波尔莱特 - 定制化需求
    /// 部门预测利润表
    /// </summary>
    public class ProfitBorODataProvider
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
        SalesSummaryBorODataProvider _SalesSummaryBorODataProvider;

        public ProfitBorODataProvider(SalesSummaryBorODataProvider salesSummaryBorODataProvider,IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
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
            _SalesSummaryBorODataProvider = salesSummaryBorODataProvider;
        }
        public dynamic GetProfit(ProfitSearch model)
        {
            ProfitSummary finaly = new ProfitSummary();
            //获取权限单位数据
            var enterprises = _httpClientUtil.GetJsonAsync<ResultModel<EnterpriseByPower>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.permission.enterlistbymenuids.list/2.0?open_req_src=nxin_shuju&menuid=0&enterpriseid={_identityservice.EnterpriseId}&boid={_identityservice.UserId}").Result;
            model.EnterpriseByPowers = enterprises?.Data;
            //获取模板
            var template = _httpClientUtil.GetJsonAsync<Result<dynamic>>(_hostCongfiguration._rdUrl + $"/api/BIZFinancialStatement/Get/{_identityservice.GroupId}/201703100104402002/{model.DataDate}").Result;
            finaly.Template = JsonConvert.DeserializeObject<List<ProfitTemplate>>(JsonConvert.SerializeObject(template?.data));
            finaly.Template = finaly.Template.Where(m => m.IsMarketProfit).ToList();
            model.TemplateDatas = finaly.Template;
            //获取最终数据
            finaly = ProfitCalc(model);

            foreach (var item in finaly.Template)
            {
                foreach (var items in finaly.Porfits)
                {
                    if (item.Orgs == null)
                    {
                        item.Orgs = new List<dynamic>();
                    }
                    var temp = items.Data.Where(m => m.FinancialStatementID == item.FinancialStatementID).FirstOrDefault();
                    if (temp != null)
                    {
                        item.Orgs.Add(temp);
                    }
                }
            }
            finaly.Porfits.Clear();
            return finaly;
        }
        /// <summary>
        /// 定制化获取 销售汇总表数据（波尔莱特）（预算数据）
        /// </summary>
        public List<SalesSummaryEntity> GetSalesData(ProfitSearch model)
        {
            string json = JsonConvert.SerializeObject(_SalesSummaryBorODataProvider.GetSalesSummaryData(new SalesSummarySearch()
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                EnterpriseId = string.Join(',', model.EnterpriseByPowers.Select(m => m.EnterpriseID)),
                GroupId = _identityservice.GroupId,
                EnterpriseIds = string.Join(',', model.EnterpriseByPowers.Select(m => m.EnterpriseID)),
                MarketSortIds = model.SortId,
            }));
            if (string.IsNullOrEmpty(json))
            {
                json = "[]";
            }
            var data = JsonConvert.DeserializeObject<List<SalesSummaryEntity>>(json);
            return data;
        }
        /// <summary>
        /// 获取费用数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CostData> GetCostDatas(ProfitSearch model)
        {
            string sql = $@"-- 取费用
            SELECT uuid() KeyUUID,CONCAT(s3.EnterpriseID) AS EnterpriseId, s3.EnterpriseName AS EnterpriseName,Concat(s2.`MarketID`) MarketID,LEFT(s8.`AccoSubjectCode`,4) AS AccoSubjectCode,SUM(s2.Debit) AS Amount -- ,s2.Credit as 贷方金额
            FROM nxin_qlw_business.fd_settlereceipt AS s1
            INNER JOIN nxin_qlw_business.fd_settlereceiptdetail AS s2 ON s1.NumericalOrder = s2.NumericalOrder
            INNER JOIN qlw_nxin_com.biz_enterprise AS s3 ON s1.EnterpriseID = s3.EnterpriseID

            LEFT JOIN qlw_nxin_com.biz_accosubject AS s8 ON s2.AccoSubjectID=s8.AccoSubjectID

            --  left JOIN qlw_nxin_com.biz_market AS s10 on s2.MarketID = s10.MarketID
            WHERE s3.PID = {_identityservice.GroupId} AND LEFT(s8.`AccoSubjectCode`,4) IN (6601,6602,6603,6403,6402)
            -- AND s1.SettleReceipType in(201610220104402201,201610220104402202,201610220104402203)
            AND s1.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' 
            GROUP BY s3.EnterpriseName ,s2.`MarketID`,LEFT(s8.`AccoSubjectCode`,4);";
            var data = _context.CostDataSet.FromSqlRaw(sql).AsNoTracking().ToList();
            var maSort = _SalesSummaryBorODataProvider.MarketSortInfo("",string.Join(',',data.Select(m=>m.MarketID).Distinct()));//部门组织/创业单元
            foreach (var item in data)
            {
                //部门找组织
                var sortdata = maSort.FirstOrDefault(m => m.Id == item.MarketID);
                if (sortdata != null)
                {
                    item.OrganizationSortID = sortdata.SortId.ToString();
                    item.OrganizationSortName = sortdata.sortName;
                    item.cAxis = sortdata.cAxis;
                }
            }
            return data;
        }
        public List<SalesSummary> GetSalesSummarieDatas(ProfitSearch model)
        {
            string sql = $@"SELECT UUID() KeyUUID,CONCAT(t1.`EnterpriseID`) EnterpriseID,CONCAT(t2.`MarketID`) MarketID,IFNULL(SUM(t2.`Quantity`* fcd.ForecastUnitCost),0.00) AS ForecastUnitCost,SUM(t2.`AmountNet`) AS AmountNet
            FROM nxin_qlw_business.`sa_salessummary` t1
            INNER JOIN nxin_qlw_business.`sa_salessummarydetail` t2 ON t1.NumericalOrder = t2.NumericalOrder
            left JOIN nxin_qlw_business.FD_MarketingProductCostSetting t3 ON t1.`EnterpriseID`=t3.AccountingEnterpriseID
            left JOIN nxin_qlw_business.FD_MarketingProductCostSettingDetail fcd on fcd.ProductID = t2.productid and t3.NumericalOrder = fcd.NumericalOrder
            WHERE t1.`DataDate` BETWEEN '{model.BeginDate}' AND '{model.EndDate}' AND t1.EnterpriseID IN ({string.Join(',', model.EnterpriseByPowers.Select(m=>m.EnterpriseID))})
            GROUP BY t2.`MarketID`;";
            var data = _context.SalesSummaryDataSet.FromSqlRaw(sql).ToList();
            var maSort = _SalesSummaryBorODataProvider.MarketSortInfo("", string.Join(',', data.Select(m => m.MarketID).Distinct()));//部门组织/创业单元
            foreach (var item in data)
            {
                //部门找组织
                var sortdata = maSort.FirstOrDefault(m => m.Id == item.MarketID);
                if (sortdata != null)
                {
                    item.OrganizationSortID = sortdata.SortId.ToString();
                    item.OrganizationSortName = sortdata.sortName;
                    item.cAxis = sortdata.cAxis;
                }
            }
            return data;
        }
        /// <summary>
        /// 根据部门组织筛选进行数据合并计算
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProfitSummary ProfitCalc(ProfitSearch model)
        {
            var costData = GetCostDatas(model);//获取费用
            var salesData = GetSalesSummarieDatas(model);//获取销售汇总预测表
            model.BeginDate = Convert.ToDateTime(model.DataDate).ToString("yyyy-01-01");
            //获取本年累计
            var costDataCount = GetCostDatas(model);//获取费用
            var salesDataCount = GetSalesSummarieDatas(model);//获取销售汇总预测表
            var sort = _SalesSummaryBorODataProvider.OrgSortInfo();//部门组织/创业单元
            #region 部门组织/创业单元 为空 默认当前集团全部
            if (string.IsNullOrEmpty(model.SortId))
            {
                model.SortId = string.Join(',', sort.Select(m => m.SortId).Distinct());
            }
            #endregion
            #region 部门组织/创业单元 数据过滤
            {
                var mSortWhere = model.SortId.Split(',').Select(e => Convert.ToInt64(e)).ToList();
                #region 销售汇总数据过滤
                var TempArry = new List<SalesSummary>();
                var TempArryCount = new List<SalesSummary>();
                foreach (var item in mSortWhere)
                {
                    TempArry.AddRange(salesData.Where(m => m.cAxis.Contains(item.ToString())).ToList());
                    TempArryCount.AddRange(salesDataCount.Where(m => m.cAxis.Contains(item.ToString())).ToList());
                }
                salesData = TempArry.Distinct().ToList();
                salesDataCount = TempArryCount.Distinct().ToList();
                #endregion
                #region 费用数据过滤
                var TempCost = new List<CostData>();
                var TempCostCount = new List<CostData>();
                foreach (var item in mSortWhere)
                {
                    TempCost.AddRange(costData.Where(m => m.cAxis.Contains(item.ToString())).ToList());
                    TempCostCount.AddRange(costDataCount.Where(m => m.cAxis.Contains(item.ToString())).ToList());
                }
                costData = TempCost.Distinct().ToList();
                costDataCount = TempCostCount.Distinct().ToList();
                #endregion
            }
            #endregion
            //初始化数据结果
            ProfitSummary finaly = new ProfitSummary() { Template = model.TemplateDatas,Porfits = new List<ProfitSummaryData>() };
            if (costData.Count == 0 & salesData.Count ==0 && costDataCount.Count == 0 && salesDataCount.Count == 0)
            {
                return finaly;
            }
            #region 计算逻辑
            foreach (var item in model.SortId.Split(',').ToArray())
            {
                var profits = new List<ProfitData>();
                foreach (var items in model.TemplateDatas.OrderBy(m=>m.FinancialStatementCode))
                {
                    var t = new ProfitData()
                    {
                        FinancialStatementID = items.FinancialStatementID,
                        OrganizationSortID = item,
                        OrganizationSortName = sort.Where(m => m.SortId == Convert.ToInt64(item)).FirstOrDefault().sortName,
                        FinancialStatementCode = items.FinancialStatementCode,
                    };
                    if (items.FinancialStatementCode == "2001")
                    {
                        t.Amount = (double)salesData.Where(m=> m.cAxis.Contains(item.ToString())).Sum(m => m.AmountNet) + costData.Where(m=>m.AccoSubjectCode == "6051" && m.cAxis.Contains(item.ToString())).Sum(m=>m.Amount);
                        t.YearAmount = (double)salesDataCount.Where(m => m.cAxis.Contains(item.ToString())).Sum(m => m.AmountNet) + costDataCount.Where(m => m.AccoSubjectCode == "6051" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                    }
                    else if(items.FinancialStatementCode == "2002")
                    {
                        t.Amount = (double)salesData.Where(m=> m.cAxis.Contains(item.ToString())).Sum(m => m.AmountNet) ;
                        t.YearAmount = (double)salesDataCount.Where(m => m.cAxis.Contains(item.ToString())).Sum(m => m.AmountNet);
                    }
                    else if (items.FinancialStatementCode == "2003")
                    {
                        t.Amount = (double)salesData.Where(m=> m.cAxis.Contains(item.ToString())).Sum(m => m.ForecastUnitCost) + costData.Where(m => m.AccoSubjectCode == "6402" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                        t.YearAmount = (double)salesDataCount.Where(m => m.cAxis.Contains(item.ToString())).Sum(m => m.ForecastUnitCost) + costDataCount.Where(m => m.AccoSubjectCode == "6402" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                    }
                    else if (items.FinancialStatementCode == "2004")
                    {
                        t.Amount = (double)salesData.Where(m=> m.cAxis.Contains(item.ToString())).Sum(m => m.ForecastUnitCost);
                        t.YearAmount = (double)salesDataCount.Where(m => m.cAxis.Contains(item.ToString())).Sum(m => m.ForecastUnitCost);
                    }
                    else if (items.FinancialStatementCode == "2005")
                    {
                        t.Amount = costData.Where(m => m.AccoSubjectCode == "6403").Sum(m => m.Amount);
                        t.YearAmount = costDataCount.Where(m => m.AccoSubjectCode == "6403" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                    }
                    else if (items.FinancialStatementCode == "2006")
                    {
                        t.Amount = costData.Where(m => m.AccoSubjectCode == "6601" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                        t.YearAmount = costDataCount.Where(m => m.AccoSubjectCode == "6601" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                    }
                    else if (items.FinancialStatementCode == "2007")
                    {
                        t.Amount = costData.Where(m => m.AccoSubjectCode == "6602" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                        t.YearAmount = costDataCount.Where(m => m.AccoSubjectCode == "6602" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                    }
                    else if (items.FinancialStatementCode == "2009")
                    {
                        t.Amount = costData.Where(m => m.AccoSubjectCode == "6603" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                        t.YearAmount = costDataCount.Where(m => m.AccoSubjectCode == "6603" && m.cAxis.Contains(item.ToString())).Sum(m => m.Amount);
                    }
                    else if (items.FinancialStatementCode == "2018")
                    {
                        t.Amount = profits.Where(m => m.FinancialStatementCode == "2001" && m.OrganizationSortID == item).Sum(m => m.Amount) -
                            profits.Where(m => m.FinancialStatementCode == "2003" && m.OrganizationSortID == item).Sum(m => m.Amount) -
                            profits.Where(m => m.FinancialStatementCode == "2005" && m.OrganizationSortID == item).Sum(m => m.Amount) -
                            profits.Where(m => m.FinancialStatementCode == "2006" && m.OrganizationSortID == item).Sum(m => m.Amount) -
                            profits.Where(m => m.FinancialStatementCode == "2007" && m.OrganizationSortID == item).Sum(m => m.Amount) -
                            profits.Where(m => m.FinancialStatementCode == "2009" && m.OrganizationSortID == item).Sum(m => m.Amount);
                        t.YearAmount = profits.Where(m => m.FinancialStatementCode == "2001" && m.OrganizationSortID == item).Sum(m => m.YearAmount) -
                            profits.Where(m => m.FinancialStatementCode == "2003" && m.OrganizationSortID == item).Sum(m => m.YearAmount) -
                            profits.Where(m => m.FinancialStatementCode == "2005" && m.OrganizationSortID == item).Sum(m => m.YearAmount) -
                            profits.Where(m => m.FinancialStatementCode == "2006" && m.OrganizationSortID == item).Sum(m => m.YearAmount) -
                            profits.Where(m => m.FinancialStatementCode == "2007" && m.OrganizationSortID == item).Sum(m => m.YearAmount) -
                            profits.Where(m => m.FinancialStatementCode == "2009" && m.OrganizationSortID == item).Sum(m => m.YearAmount);
                    }
                    profits.Add(t);
                }
                finaly.Porfits.Add(new ProfitSummaryData()
                {
                    EnterpriseName = sort.Where(m => m.SortId == Convert.ToInt64(item)).FirstOrDefault()?.sortName,
                    Data = profits
                });
                finaly.OrgList.Add(new { OrganizationSortName = sort.Where(m => m.SortId == Convert.ToInt64(item)).FirstOrDefault()?.sortName, OrganizationSortID = item });
            }
            return finaly;
            #endregion
        }
    }
}
