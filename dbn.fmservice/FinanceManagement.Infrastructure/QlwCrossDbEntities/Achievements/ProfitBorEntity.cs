using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class ProfitSearch
    {
        /// <summary>
        /// 年月日 固定 yyyy-MM-25
        /// </summary>
        public string DataDate { get; set; }
        public string SortId { get; set; }
        public List<EnterpriseByPower> EnterpriseByPowers { get; set; }
        public List<ProfitTemplate> TemplateDatas { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
    /// <summary>
    /// 利润表数据
    /// </summary>
    public class ProfitData 
    {
        public string FinancialStatementID { get; set; }
        public string ClassificationID { get; set; }
        public double Amount { get; set; }
        public double YearAmount { get; set; }
        public int ResultType { get; set; }
        public string OrganizationSortID { get; set; }
        public string OrganizationSortName { get; set; }
        public string FinancialStatementCode { get; set; }
    }
    /// <summary>
    /// 利润表模板
    /// </summary>
    public class ProfitTemplate
    {
        public string FinancialStatementID { get; set; }
        public string FinancialStatementCode { get; set; }
        public string FinancialStatementName { get; set; }
        public string FinancialStatementSort { get; set; }
        public int Level { get; set; }
        public List<dynamic> Orgs { get; set; }
        /// <summary>
        /// 是否部门利润表展示
        /// </summary>
        public bool IsMarketProfit { get; set; }
    }
    /// <summary>
    /// 利润表汇总数据
    /// </summary>
    public class ProfitSummary
    {
        public List<ProfitTemplate> Template { get; set; }
        public List<ProfitSummaryData> Porfits { get; set; } = new List<ProfitSummaryData>();
        /// <summary>
        /// 前端专用 选中的部门组织
        /// </summary>
        public List<dynamic> OrgList { get; set; } = new List<dynamic>();
    }
    /// <summary>
    /// 利润表数据
    /// </summary>
    public class ProfitSummaryData
    {
        public string EnterpriseName { get; set; }
        public List<ProfitData> Data { get; set; }
    }
    /// <summary>
    /// 费用类（会计辅助账）
    /// </summary>
    public class CostData
    {
        [Key]
        public Guid KeyUUID { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 科目编码
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 借方金额
        /// </summary>
        public double Amount { get; set; }
        [NotMapped]
        /// <summary>
        /// 部门组织/创业单元  根据部门转换
        /// </summary>
        public string OrganizationSortID { get; set; } = "";
        [NotMapped]
        public string OrganizationSortName { get; set; } = "";
        [NotMapped]
        public string cAxis { get; set; } = "";
    }
    public class SalesSummary
    {
        [Key]
        public string KeyUUID { get; set; }
        public string EnterpriseID { get; set; }
        public string MarketID { get; set; }
        public double ForecastUnitCost { get; set; }
        public double AmountNet { get; set; }
        [NotMapped]
        /// <summary>
        /// 部门组织/创业单元  根据部门转换
        /// </summary>
        public string OrganizationSortID { get; set; } = "";
        [NotMapped]
        public string OrganizationSortName { get; set; } = "";
        [NotMapped]
        public string cAxis { get; set; } = "";
    }
}
