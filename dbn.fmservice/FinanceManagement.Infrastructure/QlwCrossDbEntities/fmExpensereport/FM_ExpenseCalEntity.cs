using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_ExpenseCalEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketID { get; set; }

        public string PigFarmID { get; set; }
        ///// <summary>
        ///// 借方金额
        ///// </summary>
        //public decimal nDebitAmount { get; set; }
        ///// <summary>
        ///// 贷方金额
        ///// </summary>
        //public decimal nCreditAmount { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class FM_PigGroupDataEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 汇总方式ID
        /// </summary>
        public string GroupID { get; set; }

        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmID { get; set; }
        /// <summary>
        /// 日龄
        /// </summary>
        [NotMapped]
        public decimal Days { get; set; }
        /// <summary>
        /// 日龄*系数
        /// </summary>
        public decimal RateDays { get; set; }

        /// <summary>
        /// 分摊金额
        /// </summary>
        [NotMapped]
        public decimal ExpenseAmount { get; set; }
    }

    public class FM_ExpenseSummaryReportEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string CostProjectCode { get; set; }

        public string PigFarmID { get; set; }
        public string PigFarmName { get; set; }
        /// <summary>
        /// 归集类型
        /// </summary>
        public string CollectionType { get; set; }
        public string CollectionTypeName { get; set; }
        public string CostProjectID { get; set; }
        public string CostProjectName { get; set; }
        public decimal ExpenseValue { get; set; }
        /// <summary>
        /// 分摊金额
        /// </summary>
        public decimal ExpenseAmount { get; set; }
        /// <summary>
        /// 合计金额
        /// </summary>
        [NotMapped]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 猪场集合
        /// </summary>
        [NotMapped]
        public List<string> PigFarmNames { get; set; } = new List<string>();
        /// <summary>
        /// 分摊金额集合
        /// </summary>
        [NotMapped]
        public List<decimal> ExpenseAmounts { get; set; } = new List<decimal>();
    }

    public class FM_ExpenseReportLogsEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        public string NumericalOrder { get; set; }
        public int RecordID { get; set; }
        public string AllocationType { get; set; }
        public string ReportPeriod { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string CostProjectCode { get; set; }

        /// <summary>
        /// 归集类型
        /// </summary>
        public string CollectionType { get; set; }
        public string CollectionTypeName { get; set; }
        public string CostProjectID { get; set; }
        public string CostProjectName { get; set; }
        /// <summary>
        /// 辅助项
        /// </summary>
        public string SubsidiaryOption { get; set; }
        public decimal OccuredAmount { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class FM_ExpenseReportDetailLogsEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        public string OwnerID { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 科目全称
        /// </summary>
        public string AccoSubjectFullName { get; set; }
        public string SubsidiaryOption { get; set; }
        public decimal Amount { get; set; }
    }
}
