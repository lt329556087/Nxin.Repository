using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class PerformanceIncomeEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseName { get; set; }
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string PersonName { get; set; }
        public string PersonID { get; set; }
        /// <summary>
        /// 组织ID
        /// </summary>
        public string SortID { get; set; }
        public string cAxis { get; set; }
        public string ax1SortId { get; set; }
        public string ax2SortId { get; set; }
        public string ax3SortId { get; set; }
        public string ax1SortName { get; set; }
        public string ax2SortName { get; set; }
        public string ax3SortName { get; set; }
        public double bigDataIncomeTax { get; set; }
        public double bigDataIncome { get; set; }
        public double extensionIncomeTax { get; set; }
        public double extensionIncome { get; set; }
        public double hardwareIncomeTax { get; set; }
        public double hardwareIncome { get; set; }
        public double memberIncomeTax { get; set; }
        public double memberIncome { get; set; }
        public double nongDaiIncomeTax { get; set; }
        public double nongDaiIncome { get; set; }
        public double nongDataIncomeTax { get; set; }
        public double nongDataIncome { get; set; }
        public double nongYinIncomeTax { get; set; }
        public double nongYinIncome { get; set; }
        public double otherIncomeTax { get; set; }
        public double otherIncome { get; set; }
        public double saasIncomeTax { get; set; }
        public double saasIncome { get; set; }
        public double scienceIncomeTax { get; set; }
        public double scienceIncome { get; set; }
        public double softwareIncomeTax { get; set; }
        public double softwareIncome { get; set; }
        public double straightIncomeTax { get; set; }
        public double straightIncome { get; set; }

        [NotMapped]
        public double numericalRevenueTax { get; set; }
        [NotMapped]
        public double numericalRevenue { get; set; }
        [NotMapped]
        public double transactionIncomeTax { get; set; }
        [NotMapped]
        public double transactionIncome { get; set; }
        [NotMapped]
        public double financialIncomeTax { get; set; }
        [NotMapped]
        public double financialIncome { get; set; }
        [NotMapped]
        public double incomeTotalTax { get; set; }
        [NotMapped]
        public double incomeTotal { get; set; }

    }
    /// <summary>
    /// 销售汇总表-新
    /// </summary>
    public class SalesSummaryEntity
    {
        [Key]
        public string KeyUUID { get; set; }
        /// <summary>
        /// 提供给闫 的专属
        /// </summary>
        public string SummaryType2Name { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 单位组织
        /// </summary>
        /// 
        [NotMapped]
        public string org_enid1 { get; set; }
        [NotMapped]
        public string org_enid2 { get; set; }
        [NotMapped]
        public string org_enid3 { get; set; }
        [NotMapped]
        public string org_enid4 { get; set; }

        public string org_en { get; set; }
        [NotMapped]
        public string org_encAxis { get; set; } = "";
        /// <summary>
        /// 单位组织切割1级
        /// </summary>
        public string org_en1 { get; set; }
        /// <summary>
        /// 单位组织切割2级
        /// </summary>
        public string org_en2 { get; set; }
        /// <summary>
        /// 单位组织切割3级
        /// </summary>
        public string org_en3 { get; set; }
        [NotMapped]
        public string org_en4 { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 业务员ID
        /// </summary>
        public string SalesmanID { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取集团商品分类最大级次
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// 业务单元ID
        /// </summary>
        public string SortId { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string SortName { get; set; }
        [NotMapped]
        public string bs_enid1 { get; set; }
        [NotMapped]
        public string bs_enid2 { get; set; }
        [NotMapped]
        public string bs_enid3 { get; set; }
        [NotMapped]
        public string bs_enid4 { get; set; }
        [NotMapped]
        public string bs_enid5 { get; set; }
        [NotMapped]
        public string bs_encAxis { get; set; } = "";


        /// <summary>
        /// 创业单元切割1级
        /// </summary>
        public string bs_en1 { get; set; }
        /// <summary>
        /// 创业单元切割2级
        /// </summary>
        public string bs_en2 { get; set; }
        /// <summary>
        /// 创业单元切割3级
        /// </summary>
        public string bs_en3 { get; set; }
        /// <summary>
        /// 创业单元切割4级
        /// </summary>
        public string bs_en4 { get; set; }
        /// <summary>
        /// 创业单元切割5级
        /// </summary>
        public string bs_en5 { get; set; }
        /// <summary>
        /// 商品代号ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 商品代号
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public double? TaxRate { get; set; }
        /// <summary>
        /// 销售数量
        /// </summary>
        public double? Quantity { get; set; }
        /// <summary>
        /// 销售件数
        /// </summary>
        public double? Packages { get; set; }
        /// <summary>
        /// 销售单价
        /// </summary>
        public double? UnitPriceTax { get; set; }
        /// <summary>
        /// 含税销售收入
        /// </summary>
        public double? Amount { get; set; }
        /// <summary>
        /// 现场折扣
        /// </summary>
        public double? Discount { get; set; }
        /// <summary>
        /// 抹零
        /// </summary>
        public double? DerateAmount { get; set; }
        /// <summary>
        /// 含税销售净额
        /// </summary>
        public double? AmountTotal { get; set; }
        /// <summary>
        /// 采购单位成本
        /// </summary>
        public double? PurchaseUnitCost { get; set; }
        /// <summary>
        /// 采购含税成本总额
        /// </summary>
        public double? PurchaseUnitCostTaxSum { get; set; }
        /// <summary>
        /// 采购税率
        /// </summary>
        public double? PurchaseTaxRate { get; set; }
        /// <summary>
        /// 营业收入
        /// </summary>
        public double? OperatingRevenue { get; set; }
        /// <summary>
        /// 不含税销售净额
        /// </summary>
        public double? NetSalesExcludingTax { get; set; }
        /// <summary>
        /// 不含税采购成本
        /// </summary>
        public double? PurchaseCostExcludingTax { get; set; }
        /// <summary>
        /// 商品名称ID
        /// </summary>
        public string ProductGroupID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductGroupName { get; set; }
        /// <summary>
        /// 商品分类ID
        /// </summary>
        public string ClassificationID { get; set; }
        /// <summary>
        /// 商品分类
        /// </summary>
        public string ClassificationName { get; set; }
        /// <summary>
        /// 商品分类轴
        /// </summary>
        public string cAxis { get; set; }
        /// <summary>
        /// 商品分类 1级
        /// </summary>
        public string pro_ifi1 { get; set; }
        /// <summary>
        /// 商品分类 2级
        /// </summary>
        public string pro_ifi2 { get; set; }
        /// <summary>
        /// 商品分类 3级
        /// </summary>
        public string pro_ifi3 { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 省市县
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string area_name1 { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string area_name2 { get; set; }
        /// <summary>
        /// 县
        /// </summary>
        public string area_name3 { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string DataMonth { get; set; }
        /// <summary>
        /// 部门组织
        /// </summary>
        public string OrganizationSortID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 部门名称1级
        /// </summary>
        public string market_1 { get; set; }
        /// <summary>
        /// 部门名称2级
        /// </summary>
        public string market_2 { get; set; }
        /// <summary>
        /// 部门名称3级
        /// </summary>
        public string market_3 { get; set; }
        /// <summary>
        /// 是否小计
        /// </summary>
        public int? IsSum { get; set; }
        /// <summary>
        /// 用于排序
        /// </summary>
        public long? KeyCount { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string SalesAbstract { get; set; }
        /// <summary>
        /// 摘要名称
        /// </summary>
        public string SalesAbstractName { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 数据类型（金融/销售 标识）
        /// </summary>
        public string DataType { get; set; }
    }
    /// <summary>
    /// 销售汇总表-金融数据
    /// </summary>
    public class SalesFinanceEntity
    {
        [Key]
        // 编号
        /// <summary>
        /// 编号
        /// </summary>
        public string KeyUUID { get; set; }
        /// <summary>
        /// 提供给闫 的专属
        /// </summary>
        public string SummaryType2Name { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 数据日期
        /// </summary>
        public string DataDate { get; set; }

        /// <summary>
        /// 业务摘要
        /// </summary>
        public string BusinessAbstract { get; set; }

        /// <summary>
        /// 摘要名称
        /// </summary>
        public string AbstractName { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 销售员ID
        /// </summary>
        public string SalesmanID { get; set; }

        /// <summary>
        /// 销售员名称
        /// </summary>
        public string SalesmanName { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 区域名称1
        /// </summary>
        public string Area_name1 { get; set; }

        /// <summary>
        /// 区域名称2
        /// </summary>
        public string Area_name2 { get; set; }

        /// <summary>
        /// 区域名称3
        /// </summary>
        public string Area_name3 { get; set; }

        /// <summary>
        /// 组织排序ID
        /// </summary>
        public string OrganizationSortID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品组ID
        /// </summary>
        public string ProductGroupID { get; set; }

        /// <summary>
        /// 产品组名称
        /// </summary>
        public string ProductGroupName { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public string ClassificationID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string ClassificationName { get; set; }

        /// <summary>
        /// 产品信息1
        /// </summary>
        public string Pro_ifi1 { get; set; }

        /// <summary>
        /// 产品信息2
        /// </summary>
        public string Pro_ifi2 { get; set; }

        /// <summary>
        /// 产品信息3
        /// </summary>
        public string Pro_ifi3 { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public double? TaxRate { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public double? Amount { get; set; }

        /// <summary>
        /// 担保费用
        /// </summary>
        public double? GuarantorCost { get; set; }

        /// <summary>
        /// 资金成本
        /// </summary>
        public double? CapitalCost { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string DataMonth { get; set; }
        /// <summary>
        /// 商品分类轴
        /// </summary>
        public string cAxis { get; set; }
        public string NumericalOrder { get; set; }
    }

    public class SortInfo
    {
        [Key]
        /// <summary>
        /// 组织ID
        /// </summary>
        public long SortId { get; set; }
        [NotMapped]
        public string sortName { get; set; }
        /// <summary>
        /// 组织全称
        /// </summary>
        public string cfullname { get; set; }
        /// <summary>
        /// 关系ID  单位ID/部门ID
        /// </summary>
        public long RelatedID { get; set; }
        public string cAxis { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        [NotMapped]
        public string Id { get; set; }
        [NotMapped]
        public int sortRank { get; set; }
    }
    /// <summary>
    /// 通过客户ID 获取 客户档案中的业务员
    /// </summary>
    public class CustomreService
    {
        [Key]
        public long RecordID { get; set; }
        public long cRecordId { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public long NumericalOrder { get; set; }
        /// <summary>
        /// 业务员ID
        /// </summary>
        public long PersonID { get; set; }
        /// <summary>
        /// 业务员类型ID
        /// </summary>
        public string RoleType { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 业务员BO_ID
        /// </summary>
        public string EntityID { get; set; }
        public string dStart { get; set; }
        public bool? IsDefault { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditorName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// 业务员类型
        /// </summary>
        public string RoleTypeName { get; set; }
    }
    public class ServicePersonSort
    {
        /// <summary>
        /// 业务人员ID
        /// </summary>
        [Key]
        public long PersonId { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创业单元ID
        /// </summary>
        public long SortID { get; set; }
        /// <summary>
        /// 创业单元全称
        /// </summary>
        public string FullName { get; set; }
    }
    public class PerformanceSet
    {
        [Key]
        public long RecordID { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 销售摘要
        /// </summary>
        public long SalesAbstract { get; set; }
        /// <summary>
        /// 商品分类
        /// </summary>
        public long? ProductInfo { get; set; }
        /// <summary>
        /// 分配依据
        /// </summary>
        public long AssignmentType { get; set; }
        /// <summary>
        /// 公式
        /// </summary>
        public string AllocationFormula { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        public long? PersonnelType { get; set; }
    }
}
