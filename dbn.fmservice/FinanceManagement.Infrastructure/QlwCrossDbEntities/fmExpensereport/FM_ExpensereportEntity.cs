using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_ExpensereportEntity
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 填报期间
        /// </summary>
        public string ReportPeriod { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 费用总金额
        /// </summary>
        public decimal? ExpenseAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 是否归集（0：未归集 1：已归集）
        /// </summary>
        public int IsCollect { get; set; }

        /// <summary>
        /// 是否有错误报告（0：无 1：有）
        /// </summary>
        public int IsErrorLogs { get; set; }

        /// <summary>
        /// 明细集合
        /// </summary>
        [NotMapped]
        public List<fm_expensereportdetailEntity> DetailList { get; set; } = new List<fm_expensereportdetailEntity>();
    }

    public class fm_expensereportdetailEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 费用项目ID
        /// </summary>
        public string CostProjectID { get; set; }
        public string CostProjectCode { get; set; }
        public string CostProjectName { get; set; }
        /// <summary>
        /// 归集类型
        /// </summary>
        public string CollectionType { get; set; }
        public string CollectionTypeName { get; set; }
        /// <summary>
        /// 取数来源
        /// </summary>
        public string DataSource { get; set; }
        public string DataSourceName { get; set; }

        /// <summary>
        /// 分摊方式
        /// </summary>
        public string AllocationType { get; set; }
        public string AllocationTypeName { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        public decimal ExpenseAmount { get; set; }

        /// <summary>
        /// 是否归集
        /// </summary>
        public int IsCollect { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 取数集合
        /// </summary>
        [NotMapped]
        public List<fm_expensereportextendEntity> ExtendList { get; set; } = new List<fm_expensereportextendEntity>();

        /// <summary>
        /// 归集明细集合
        /// </summary>
        [NotMapped]
        public List<fm_expensereportextendlistEntity> ExtendDetailList { get; set; } = new List<fm_expensereportextendlistEntity>();

        /// <summary>
        /// 取数日志集合
        /// </summary>
        [NotMapped]
        public List<fm_expensereportdetaillogEntity> DetailLogList { get; set; } = new List<fm_expensereportdetaillogEntity>();
    }

    public partial class fm_expensereportdetaillogEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }
        /// <summary>
        /// 关联明细ID
        /// </summary>
        public int DetailID { get; set; }
        /// <summary>
        /// 辅助项
        /// </summary>
        public string SubsidiaryOption { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal OccuredAmount { get; set; }
        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }

    public partial class fm_expensereportextendEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }
        /// <summary>
        /// 关联明细ID
        /// </summary>
        public int DetailID { get; set; }
        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmID { get; set; }
        public string PigFarmName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptOrOthersID { get; set; }
        public string DeptOrOthersName { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        public decimal ExpenseAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }

    public class fm_expensereportextendlistEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }
        /// <summary>
        /// 关联明细ID
        /// </summary>
        public int DetailID { get; set; }
        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmID { get; set; }
        public string PigFarmName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptOrOthersID { get; set; }
        public string DeptOrOthersName { get; set; }
        ///// <summary>
        ///// 分摊方式
        ///// </summary>
        //public string AllocationType { get; set; }
        //public string AllocationTypeName { get; set; }
        /// <summary>
        /// 分摊值
        /// </summary>
        public decimal ExpenseValue { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        public decimal ExpenseAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }
}
