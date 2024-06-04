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

namespace FinanceManagement.ApiHost.Controllers
{
    public class PerformanceIncomeODataProvider
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

        public PerformanceIncomeODataProvider(IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
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
        public List<PerformanceIncomeEntity> GetList(PerformanceIncomeSearch model)
        {
            string selectStr = getSelectMethod(model);
            string whereStr = getWhereMethod(model);
            string groupStr = getGroupMethod(model);
            string sql = $@"SELECT CONCAT(UUID()) AS PrimaryKey,
                                {selectStr.TrimEnd(',')}
                                FROM (
                                SELECT be.EnterpriseName                                                  AS EnterpriseName,
                                       CONCAT(s.EnterpriseID)                                                     AS EnterpriseID,
                                       DATE_FORMAT(s.DataDate, '%Y-%m')                                   AS DataDate,
                                       IF(hp.Name IS NULL, '', hp.Name)                                   AS PersonName,
                                       CONCAT(ifnull(hp.personID,''))                                             AS PersonID,
                                       CONCAT(s.OrganizationSortID)                                               AS SortID,
                                       se1.cAxis, 
                                       CASE WHEN SUBSTRING_INDEX(CONCAT(se1.cFullName,'/'),'/', 3) =CONCAT(se1.cFullName,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(se1.cFullName,'/', 3),'/',-1) END as ax1SortId,
                                       SUBSTRING_INDEX(SUBSTRING_INDEX(se1.cAxis, '$', 4),'$',-1)  ax1SortName,
                                       CASE WHEN SUBSTRING_INDEX(CONCAT(se1.cFullName,'/'),'/', 4) =CONCAT(se1.cFullName,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(se1.cFullName,'/', 4),'/',-1) END as ax2SortId,
                                       SUBSTRING_INDEX(SUBSTRING_INDEX(se1.cAxis, '$', 5),'$',-1)  ax2SortName,
                                       CASE WHEN SUBSTRING_INDEX(CONCAT(se1.cFullName,'/'),'/', 5) =CONCAT(se1.cFullName,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(se1.cFullName,'/', 5),'/',-1) END as ax3SortId,
                                       SUBSTRING_INDEX(SUBSTRING_INDEX(se1.cAxis, '$', 6),'$',-1)  ax3SortName,
                                       ss.AmountTotal                                                     AS SaleAmountTotal,
                                       IFNULL(ssde.salesCost,0) +IFNULL(ROUND(ss.Quantity *tt1.UnitPrice,4),0) 	AS PurchaseAmount,
                                       CONCAT(bpgroup.ProductGroupID)                                             AS ProductGroupID,
                                       bpgroup.ProductGroupName                                           AS ProductGroupName
                                         FROM nxin_qlw_business.sa_sales AS s
                                         INNER JOIN nxin_qlw_business.sa_salesdetail AS ss ON ss.NumericalOrder = s.NumericalOrder
                                         INNER JOIN qlw_nxin_com.biz_enterprise AS be ON be.EnterpriseID = s.EnterpriseID
                                         INNER JOIN qlw_nxin_com.biz_product AS bp2 ON bp2.ProductID = ss.ProductID
                                         LEFT JOIN qlw_nxin_com.biz_productgroup bpgroup ON bp2.ProductGroupID = bpgroup.ProductGroupID
                                         LEFT JOIN nxin_qlw_business.SA_SalesDetailExtend SSDE
                                                   ON ss.NumericalOrder = SSDE.NumericalOrder AND ss.ProductType = SSDE.NumericalOrderDetail AND
                                                      SSDE.ProductID = ss.ProductID
                                         LEFT JOIN nxin_qlw_business.hr_person AS hp ON s.SalesmanID = hp.PersonID
                                         LEFT JOIN qlw_nxin_com.bsorganizationsort AS se1 ON se1.SortId = s.OrganizationSortID and se1.EnterID={model.GroupID}
                                         LEFT JOIN (
                                                SELECT tt2.EnterpriseID, tt2.SupplierID ,IFNULL	(tt1.UnitPrice,0) AS UnitPrice,tt1.TaxRate ,t2.RecordID
                                                FROM nxin_qlw_business.sa_sales t1 INNER JOIN  nxin_qlw_business.sa_salesdetail t2
                                                INNER  JOIN nxin_qlw_business.BIZ_Related AS BR11
                                                                ON BR11.ChildValue = t1.NumericalOrder AND BR11.RelatedType = 201610210104402122 AND
                                                                    BR11.ParentType = 1903061646040000101 AND BR11.ChildType = 1903061646210000101
                                                        INNER  JOIN nxin_qlw_business.pm_purchasedetail tt1
                                                                ON BR11.ParentValue = tt1.NumericalOrder AND tt1.ProductID = t2.ProductID
                                                        INNER  JOIN nxin_qlw_business.pm_purchase tt2
                                                                ON tt2.NumericalOrder = tt1.NumericalOrder
                                                where t1.enterpriseid in (select enterpriseid from qlw_nxin_com.biz_enterprise where  PID = {model.GroupID})
                                                and t1.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}')   tt1 ON tt1.RecordID=ss.RecordID
                                         LEFT JOIN nxin_qlw_business.PM_Supplier SP1 ON tt1.SupplierID = SP1.SupplierID AND SP1.EnterpriseID = tt1.EnterpriseID
                                WHERE s.enterpriseid in (select enterpriseid from qlw_nxin_com.biz_enterprise where  PID = {model.GroupID})
                                  and s.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}'  {whereStr}
                                ) temp  {groupStr}";
            _logger.LogInformation("绩效收入sql" + sql);
            var list = _context.PerformanceIncomeEntityDataSet.FromSqlRaw(sql).ToList();
            list.ForEach(s =>
            {
                s.numericalRevenue = s.saasIncome + s.softwareIncome + s.hardwareIncome + s.bigDataIncome + s.memberIncome + s.scienceIncome;
                s.numericalRevenueTax = s.saasIncomeTax + s.softwareIncomeTax + s.hardwareIncomeTax + s.bigDataIncomeTax + s.memberIncomeTax + s.scienceIncomeTax;
                s.transactionIncome = s.straightIncome + s.extensionIncome;
                s.transactionIncomeTax = s.straightIncomeTax + s.extensionIncomeTax;
                s.financialIncome = s.nongYinIncome + s.nongDaiIncome + s.nongDataIncome;
                s.financialIncomeTax = s.nongYinIncomeTax + s.nongDaiIncomeTax + s.nongDataIncomeTax;
                s.incomeTotal = s.numericalRevenue + s.transactionIncome + s.financialIncome + s.otherIncome;
                s.incomeTotalTax = s.numericalRevenueTax + s.transactionIncomeTax + s.financialIncomeTax + s.otherIncomeTax;
            });
            return list;
        }

        public List<FM_PerformanceIncomeEntity> GetPerformanceIncomeProperty(string groupid)
        {
            string sql = $@"SELECT RecordID,GROUP_CONCAT(ProductGroupID) AS ProductGroupID,PropertyName FROM nxin_qlw_business.fm_PerformanceIncome
                            WHERE EnterpriseID={groupid}
                            GROUP BY PropertyName";
            var list = _context.FM_PerformanceIncomeEntityDataSet.FromSqlRaw(sql).ToList();
            return list;
        }
        private string getSelectMethod(PerformanceIncomeSearch model)
        {
            var propertyList = GetPerformanceIncomeProperty(model.GroupID);
            string selectStr = $@"EnterpriseName,
                                  EnterpriseID,
                                  DataDate,
                                  PersonName,
                                  PersonID,
                                  SortID,
                                  cAxis,
                                  ax1SortId,
                                  ax2SortId,
                                  ax3SortId,
                                  ax1SortName,
                                  ax2SortName,
                                  ax3SortName,";
            foreach (var property in propertyList)
            {
                selectStr += (" SUM(CASE WHEN ProductGroupID IN (" + property.ProductGroupID + ") THEN SaleAmountTotal ELSE 0 END) AS " + property.PropertyName + ",");
                selectStr += (" SUM(CASE WHEN ProductGroupID IN (" + property.ProductGroupID + ") THEN SaleAmountTotal-PurchaseAmount ELSE 0 END) AS " + property.PropertyName + "Tax,");
            }
            return selectStr;
        }
        private string getWhereMethod(PerformanceIncomeSearch model)
        {
            #region
            string whereStr = string.Empty;

            if (!string.IsNullOrEmpty(model.PersonID))
            {
                whereStr += " and hp.personID=" + model.PersonID;
            }
            if (model.StortList?.Count > 0)
            {
                whereStr = " and (";
                for (int i = 0; i < model.StortList.Count; i++)
                {
                    if (i != 0) whereStr += " or ";
                    whereStr += $" se1.cAxis like '%{model.StortList[i]}%' ";
                }
                whereStr += ") ";
            }
            #endregion
            return whereStr;
        }
        private string getGroupMethod(PerformanceIncomeSearch model)
        {
            #region groupby 
            string groupStr = "GROUP BY ";
            foreach (var item in model.SummaryType)
            {
                switch (item)
                {
                    case "DataDate":
                        groupStr += "temp.DataDate,";
                        break;
                    case "PersonID":
                        groupStr += "temp.PersonID,";
                        break;
                    case "ax1SortId":
                        groupStr += "temp.ax1SortId,";
                        break;
                    case "ax2SortId":
                        groupStr += "temp.ax2SortId,";
                        break;
                    case "ax3SortId":
                        groupStr += "temp.ax3SortId,";
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(groupStr))
            {
                groupStr = "GROUP BY temp.DataDate,temp.PersonID,temp.ax1SortId,temp.ax2SortId,temp.ax3SortId";
            }
            else
            {
                groupStr = groupStr.TrimEnd(',');
            }
            #endregion
            return groupStr;
        }

        #region 业务战报
        /// <summary>
        /// 销售收入  净收入  销售数量  庞
        /// </summary>
        /// <returns></returns>
        public List<dynamic> GetSaleAmount(BusinessSearch model)
        {
            string LastBeginDate = Convert.ToDateTime(model.BeginDate).AddMonths(-1).ToString("yyyy-MM-dd");
            DateTime endDate = Convert.ToDateTime(model.EndDate);
            string LastEndDate = new DateTime(endDate.Year, endDate.Month, 1).AddDays(-1).ToString("yyyy-MM-dd");
            string sqlWhere = string.Empty;
            string sqlGourp = "group by ";
            string sqlOrder = "order by ";
            #region 筛选
            //人员筛选
            if (!string.IsNullOrEmpty(model.Boids))
            {
                sqlWhere += @$" AND hr.BO_ID in ({model.Boids}) ";
            }
            //单位筛选
            if (!string.IsNullOrEmpty(model.EnterpriseID))
            {
                sqlWhere += @$" AND a.EnterpriseID in ({model.EnterpriseID}) ";
            }
            //部门组织筛选
            if (!string.IsNullOrEmpty(model.SortId))
            {
                sqlWhere += @$" AND o.SortId in ({model.SortId}) ";
            }
            //商品分类筛选
            if (!string.IsNullOrEmpty(model.ClassificationName))
            {
                sqlWhere += @$" and  pd.cFullClassName like '%{model.ClassificationName}%'  ";
            }
            //部门组织排名筛选
            if (!string.IsNullOrEmpty(model.OrgSortId))
            {
                sqlWhere += @$" AND oi.PID in ({model.OrgSortId}) ";
            }
            //部门组织排名筛选
            if (!string.IsNullOrEmpty(model.OrgSortName))
            {
                sqlWhere += @$" and  oi.cFullName like '%{model.OrgSortName}%'  ";
            }
            //点击客户名称，钻取弹层到【商品代号，数值】
            if (!string.IsNullOrEmpty(model.CustomerName))
            {
                sqlWhere += @$" and  cus.CustomerName like '%{model.CustomerName}%'  ";
            }
            #endregion
            #region 汇总方式
            //集团商品分类 汇总方式
            if (model.IsIncomeRank)
            {
                if (!string.IsNullOrEmpty(model.ClassificationName) && model.ClassificationRank > 0)
                {
                    sqlGourp += $" CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cAxis,'$', {model.ClassificationRank + 1}),'$',-1) =pd.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cAxis,'$', {model.ClassificationRank + 1 }),'$',-1) END,";
                }
                else
                {
                    sqlGourp += " CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cAxis,'$', 2),'$',-1) =pd.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cAxis,'$', 2),'$',-1) END,";
                }
            }
            //人员汇总方式
            if (model.IsGroupRank)
            {
                sqlGourp += " hr.`BO_ID`,";
            }
            //商品汇总方式
            if (model.IsProductRank)
            {
                sqlGourp += " b.`ProductID`,";
            }
            //客户汇总方式
            if (model.IsCustomerRank)
            {
                sqlGourp += " a.`CustomerId`,";
            }
            //单位汇总方式
            if (model.IsEntentRank)
            {
                sqlGourp += " a.EnterpriseID,";
            }
            //部门汇总方式
            if (model.IsMarketRank)
            {
                sqlGourp += " CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) =m.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) END,";
            }
            //部门组织汇总方式
            if (model.IsOrgRank)
            {
                if (!string.IsNullOrEmpty(model.OrgSortName) && model.OrgRank > 0)
                {
                    sqlGourp += $" CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'$', {model.OrgRank + 1}),'$',-1) =oi.cFullName THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'$', {model.OrgRank + 1}),'$',-1) END,";
                }
                else
                {
                    sqlGourp += " CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', 2),'/',-1) =oi.cFullName THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', 2),'/',-1) END,";
                }
            }
            #endregion
            #region 排序
            //金额排序
            if (model.IsOrderIncome)
            {
                sqlOrder += " SUM(b.Amount) desc ";
            }
            //销量排序
            if (model.IsOrderQuantity)
            {
                sqlOrder += " SUM(b.Quantity) desc ";
            }
            //净收入排序
            if (model.IsOrderNetIncome)
            {
                sqlOrder += " SUM(ROUND((b.AmountTotal - b.AmountAdjust) / (1 + b.TaxRate), 2)) desc ";
            }
            #endregion
            //格式化groupby
            if (sqlGourp != "group by ")
            {
                sqlGourp = sqlGourp.TrimEnd(',');
            }
            else
            {
                sqlGourp = "";
            }
            #region 获取最大分类级次
            int crank = GetMaxClassificationRank();
            string ClassificationSql = "";
            //部门 级次 SQL 切割
            for (int i = 1; i <= crank; i++)
            {

                ClassificationSql += $@" CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cAxis,'$', {i+1}),'$',-1) =pd.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cAxis,'$', {i+1}),'$',-1) END AS ClassificationID{i},
                                     CASE WHEN SUBSTRING_INDEX(CONCAT(pd.cFullClassName,'/'),'/', {i}) =CONCAT(pd.cFullClassName,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(pd.cFullClassName,'/', {i}),'/',-1) END AS ClassificationName{i}, ";
            }
            #endregion
            #region 获取最大部门组织级次
            int orank = GetMaxOrgRank();
            string OrgSql = "";
            //部门 级次 SQL 切割
            for (int i = 1; i <= orank; i++)
            {

                OrgSql += $@" CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', {i}),'/',-1) =oi.cFullName THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', {i}),'/',-1) END  SortName{i}, ";
            }
            #endregion
            string sql = $@"SELECT a.EnterpriseID,ent.EnterpriseName,SortRank,
                            {ClassificationSql} {OrgSql} 
                            CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) =m.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) END AS MarketID,
                            CASE WHEN SUBSTRING_INDEX(CONCAT(m.cfullname,'/'),'/', 1) =CONCAT(m.cfullname,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(m.cfullname,'/', 1),'/',-1) END  AS MarketName,
                            hr.`BO_ID`,hr.`Name`,
                            p.ProductName,cus.CustomerName,
                            SUM(b.Packages)                                                   AS Packages,
                            SUM(b.AmountAdjust)                                               AS AmountAdjust,
                            SUM(b.Amount - b.AmountTotal)                                     AS UnitDiscount,
                            SUM(ROUND((b.AmountTotal - b.AmountAdjust) / (1 + b.TaxRate), 2)) AS NetSales,
                            SUM(b.Amount)                                                     AS Amount,
                            SUM(b.Quantity)                                                   AS Quantity,
                            IFNULL(SUM(b.Quantity * mscost.UnitPrice),0) AS CostAmount
                            FROM NXin_Qlw_Business.SA_Sales a
                            INNER JOIN NXin_Qlw_Business.SA_SalesDetail b ON a.NumericalOrder = b.NumericalOrder
                            
                            left join qlw_nxin_com.biz_market m on a.MarketID=m.MarketID
                            left JOIN qlw_nxin_com.bsorganizationsortmarket o ON o.MarketID = m.MarketID 
                            LEFT JOIN qlw_nxin_com.bsorganizationsort oi on oi.SortId = o.SortId 
                            INNER JOIN NXin_Qlw_Business.BIZ_ProductDetail bpd2
                                                ON a.EnterpriseID = bpd2.EnterpriseID AND b.ProductID = bpd2.ProductID
                                     LEFT JOIN qlw_nxin_com.BIZ_EnterprisePeriod ep
                                               ON a.EnterpriseID = ep.EnterpriseID AND a.DataDate BETWEEN ep.StartDate AND ep.EndDate
LEFT JOIN  ( 
	SELECT a.AccountingEnterpriseID,b.ProductId,SUM(ForecastUnitCost) AS UnitPrice FROM `nxin_qlw_business`.`fd_marketingproductcostsetting` a
LEFT JOIN `nxin_qlw_business`.`fd_marketingproductcostsettingdetail` b ON a.NumericalOrder=b.NumericalOrder
LEFT JOIN qlw_nxin_com.biz_enterprise  ent ON a.AccountingEnterpriseID=ent.EnterpriseID
WHERE ent.pid = {model.GroupID} AND DataDate BETWEEN '{LastBeginDate}' AND '{LastEndDate}'
GROUP BY a.AccountingEnterpriseID,b.ProductId
) mscost ON a.EnterpriseID = mscost.AccountingEnterpriseID AND b.ProductID = mscost.ProductID
                            LEFT JOIN qlw_nxin_com.biz_enterprise  ent ON a.EnterpriseID=ent.EnterpriseID
                            LEFT JOIN qlw_nxin_com.BIZ_Product p ON p.ProductID = b.ProductID
                            LEFT JOIN qlw_nxin_com.`biz_productgroup` pg ON p.`ProductGroupID`=pg.`ProductGroupID`
                            LEFT JOIN qlw_nxin_com.`biz_productgroupclassification` pd ON pg.`ClassificationID`=pd.`ClassificationID`
                            LEFT JOIN `nxin_qlw_business`.hr_person hr ON hr.`PersonID`=a.SalesmanID
                            LEFT JOIN qlw_nxin_com.biz_customer cus on cus.CustomerId = a.CustomerId
                            WHERE a.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' and a.SalesAbstract not in (201610140104402503,201610140104402505)  AND ent.Pid IN ({model.GroupID}) {sqlWhere}  {sqlGourp }  {sqlOrder} ";
            return _context.DynamicSqlQuery(sql);
        }
        /// <summary>
        /// 获取费用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<dynamic> GetCostAmount(BusinessSearch model)
        {
            string sqlWhere = string.Empty;
            string sqlGourp = "group by ";
            #region 筛选
            //人员筛选
            if (!string.IsNullOrEmpty(model.Boids))
            {
                sqlWhere += @$" AND hr.BO_ID in ({model.Boids}) ";
            }
            //单位筛选
            if (!string.IsNullOrEmpty(model.EnterpriseID))
            {
                sqlWhere += @$" AND v.EnterpriseID in ({model.EnterpriseID}) ";
            }
            //部门组织筛选
            if (!string.IsNullOrEmpty(model.SortId))
            {
                sqlWhere += @$" AND o.SortId in ({model.SortId}) ";
            }
            //部门组织排名筛选
            if (!string.IsNullOrEmpty(model.OrgSortId))
            {
                sqlWhere += @$" AND oi.PID in ({model.OrgSortId}) ";
            }
            //部门组织排名筛选
            if (!string.IsNullOrEmpty(model.OrgSortName))
            {
                sqlWhere += @$" and  oi.cFullName like '%{model.OrgSortName}%'  ";
            }
            //点击客户名称，钻取弹层到【商品代号，数值】
            if (!string.IsNullOrEmpty(model.CustomerName))
            {
                sqlWhere += @$" and  cus.CustomerName like '%{model.CustomerName}%'  ";
            }
            #endregion
            #region 汇总方式
            //集团商品分类 汇总方式
            if (model.IsIncomeRank)
            {
                sqlGourp += " CASE WHEN SUBSTRING_INDEX(CONCAT(acc.AccoSubjectFullName,'/'),'/', 1) =CONCAT(acc.AccoSubjectFullName,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(acc.AccoSubjectFullName,'/', 1),'/',-1) END ,";
            }
            //人员汇总方式
            if (model.IsGroupRank)
            {
                sqlGourp += " hr.`BO_ID`,";
            }
            //商品汇总方式
            if (model.IsProductRank)
            {
                sqlGourp += " vd.`ProductID`,";
            }
            //客户汇总方式
            if (model.IsCustomerRank)
            {
                sqlGourp += " vd.`CustomerId`,";
            }
            //单位汇总方式
            if (model.IsEntentRank)
            {
                sqlGourp += " v.EnterpriseID,";
            }
            //部门汇总方式
            if (model.IsMarketRank)
            {
                sqlGourp += " CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) =m.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) END,";
            }
            //部门组织汇总方式
            if (model.IsOrgRank)
            {
                if (!string.IsNullOrEmpty(model.OrgSortName) && model.OrgRank > 0)
                {
                    sqlGourp += $" CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'$', {model.OrgRank + 1}),'$',-1) =oi.cFullName THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'$', {model.OrgRank + 1}),'$',-1) END,";
                }
                else
                {
                    sqlGourp += " CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', 2),'/',-1) =oi.cFullName THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', 2),'/',-1) END,";
                }
            }
            #endregion
            //格式化groupby
            if (sqlGourp != "group by ")
            {
                sqlGourp = sqlGourp.TrimEnd(',');
            }
            else
            {
                sqlGourp = "";
            }
            #region 获取最大部门组织级次
            int orank = GetMaxOrgRank();
            string OrgSql = "";
            //部门 级次 SQL 切割
            for (int i = 1; i <= orank; i++)
            {

                OrgSql += $@" CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', {i}),'/',-1) =oi.cFullName THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(oi.cFullName,'/', {i}),'/',-1) END  SortName{i}, ";
            }
            #endregion
            string sql = @$"SELECT 
                            v.EnterpriseID,c.EnterpriseName,
                            {OrgSql}
                            CASE WHEN SUBSTRING_INDEX(CONCAT(acc.AccoSubjectFullName,'/'),'/', 1) =CONCAT(acc.AccoSubjectFullName,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(acc.AccoSubjectFullName,'/', 1),'/',-1) END  AS AccoSubjectName,
                            CASE WHEN SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) =m.cAxis THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis,'$', 2),'$',-1) END AS MarketID,
                            CASE WHEN SUBSTRING_INDEX(CONCAT(m.cfullname,'/'),'/', 1) =CONCAT(m.cfullname,'/') THEN '' ELSE SUBSTRING_INDEX(SUBSTRING_INDEX(m.cfullname,'/', 1),'/',-1) END  AS MarketName,
                             hr.`BO_ID`,hr.`Name`,p.ProductName,
                            SUM(vd.Debit) AS Debit 
                            FROM nxin_qlw_business.fd_settlereceipt v
                            INNER JOIN nxin_qlw_business.fd_settlereceiptdetail vd ON vd.numericalorder = v.numericalorder
                            LEFT JOIN qlw_nxin_com.BIZ_Product p ON p.ProductID = vd.ProductID
                            LEFT JOIN qlw_nxin_com.`biz_productgroup` pg ON p.`ProductGroupID`=pg.`ProductGroupID`
                            LEFT JOIN qlw_nxin_com.`biz_productgroupclassification` pd ON pg.`ClassificationID`=pd.`ClassificationID`
                            INNER JOIN qlw_nxin_com.`biz_enterprise` c ON v.EnterpriseID=c.EnterpriseID 
                            LEFT JOIN qlw_nxin_com.biz_accosubject acc ON acc.AccoSubjectID = vd.AccoSubjectID -- and acc.EnterpriseID = v.EnterpriseID
                            LEFT JOIN qlw_nxin_com.`biz_settlesummary` bs ON vd.ReceiptAbstractID=bs.SettleSummaryID
                            LEFT JOIN qlw_nxin_com.`biz_settlesummarygroup` bsg ON bs.SettleSummaryGroupID=bsg.SettleSummaryGroupID
                            LEFT JOIN nxin_qlw_business.HR_Person hr ON hr.personid = vd.personid 
                            LEFT JOIN qlw_nxin_com.biz_market m ON m.MarketID = vd.MarketID AND m.EnterpriseID = v.EnterpriseID
                            LEFT JOIN qlw_nxin_com.bsorganizationsortmarket o ON o.MarketID = m.MarketID 
                            LEFT JOIN qlw_nxin_com.bsorganizationsort oi on oi.SortId = o.SortId 
                            LEFT JOIN qlw_nxin_com.biz_customer cus on cus.CustomerId = vd.CustomerId
                            WHERE   LEFT(acc.AccoSubjectCode,4) IN (6601,6602,6603,5101) 
                            AND v.datadate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' AND c.Pid IN ({model.GroupID}) 
                            {sqlWhere}  {sqlGourp} order by SUM(vd.Debit) desc ";
            return _context.DynamicSqlQuery(sql);
        }
        /// <summary>
        /// 获取组织下相关人员
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<dynamic> GetBoidBySort(BusinessSearch model)
        {
            string sql = $@"SELECT hr.Bo_Id,hr.Name FROM nxin_qlw_business.hr_person hr 
                            INNER JOIN qlw_nxin_com.hr_postinformation hrp ON hrp.PersonID=hr.PersonID AND hrp.isuse = 1
                            INNER JOIN  qlw_nxin_com.hr_persondetail hrd ON  hr.personID = hrd.personID AND hrd.PersonType = 201610220104402102 AND hrd.PersonState <> 201610290104402103
                            INNER JOIN qlw_nxin_com.biz_market m ON m.MarketID = hrp.MarketID 
                            INNER JOIN qlw_nxin_com.bsorganizationsortmarket o ON o.MarketID = m.MarketID 
                            WHERE o.SortId IN  (
                            SELECT o.SortId  FROM nxin_qlw_business.hr_person hr 
                            INNER JOIN qlw_nxin_com.hr_postinformation hrp ON hrp.PersonID=hr.PersonID AND hrp.isuse = 1
                            INNER JOIN  qlw_nxin_com.hr_persondetail hrd ON  hr.personID = hrd.personID AND hrd.PersonType = 201610220104402102 AND hrd.PersonState <> 201610290104402103
                            INNER JOIN qlw_nxin_com.biz_market m ON m.MarketID = hrp.MarketID 
                            INNER JOIN qlw_nxin_com.bsorganizationsortmarket o ON o.MarketID = m.MarketID 
                            WHERE hr.Bo_Id={model.Boids}
                            GROUP BY o.SortId 
                            ) ";
            return _context.DynamicSqlQuery(sql);
        }
        /// <summary>
        /// 获取最大商品分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public dynamic GetMaxClassificationRank()
        {
            #region 获取最大部门级次 MarketSql
            string MaxRankMarket = $@"SELECT MAX(Rank) Rank from qlw_nxin_com.biz_productgroupclassification where EnterpriseID IN ({_identityservice.GroupId})";
            var MarketRank = _context.DynamicSqlQuery(MaxRankMarket).FirstOrDefault().Rank;
            return MarketRank;
            #endregion
        }
        /// <summary>
        /// 获取最大部门组织级次
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public dynamic GetMaxOrgRank()
        {
            #region 获取最大部门级次 MarketSql
            string MaxRankMarket = $@"SELECT MAX(SortRank) Rank from qlw_nxin_com.bsorganizationsortmarket o 
                                    INNER JOIN qlw_nxin_com.bsorganizationsort oi on oi.SortId = o.SortId 
                                    where oi.EnterId  IN ({_identityservice.GroupId})";
            var MarketRank = _context.DynamicSqlQuery(MaxRankMarket).FirstOrDefault().Rank;
            return MarketRank;
            #endregion
        }
        #endregion
    }
}
