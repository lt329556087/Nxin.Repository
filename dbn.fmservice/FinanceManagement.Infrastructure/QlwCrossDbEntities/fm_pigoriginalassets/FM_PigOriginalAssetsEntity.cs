using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_PigOriginalAssetsEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmId { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 来源类型
        /// </summary>
        public string SourceType { get; set; }
        public string SourceTypeName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedOwnerId { get; set; }
        public string ModifiedOwnerName { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckedByName { get; set; }
        public DateTime CheckedDate { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        #region 明细属性
        /// <summary>
        /// 耳号
        /// </summary>
        public string EarNumber { get; set; }
        public string EarNumberName { get; set; }
        /// <summary>
        /// 猪只类型
        /// </summary>
        public string PigType { get; set; }
        public string PigTypeName { get; set; }
        /// <summary>
        /// 原值
        /// </summary>
        public decimal? OriginalValue { get; set; }
        /// <summary>
        /// 折旧年限（月）
        /// </summary>
        public int? DepreciationUseMonth { get; set; }
        /// <summary>
        /// 净残值率（%）
        /// </summary>
        public decimal? ResidualValueRate { get; set; }
        /// <summary>
        /// 净残值
        /// </summary>
        public decimal? ResidualValue { get; set; }
        /// <summary>
        /// 开始使用日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 已计提月份
        /// </summary>
        public int? AccruedMonth { get; set; }
        /// <summary>
        /// 累计折旧
        /// </summary>
        public decimal? DepreciationAccumulated { get; set; }
        #endregion 

        [NotMapped]
        public List<FM_PigOriginalAssetsdetailEntity> Details { get; set; }
    }

    public class FM_PigOriginalAssetsdetailEntity
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 耳号
        /// </summary>
        [Key]
        public string EarNumber { get; set; }
        public string EarNumberName { get; set; }
        /// <summary>
        /// 猪只类型
        /// </summary>
        public string PigType { get; set; }
        public string PigTypeName { get; set; }
        /// <summary>
        /// 原值
        /// </summary>
        public decimal? OriginalValue { get; set; }
        /// <summary>
        /// 折旧年限（月）
        /// </summary>
        public int? DepreciationUseMonth { get; set; }
        /// <summary>
        /// 净残值率（%）
        /// </summary>
        public decimal? ResidualValueRate { get; set; }
        /// <summary>
        /// 净残值
        /// </summary>
        public decimal? ResidualValue { get; set; }
        /// <summary>
        /// 开始使用日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 已计提月份
        /// </summary>
        public int? AccruedMonth { get; set; }
        /// <summary>
        /// 累计折旧
        /// </summary>
        public decimal? DepreciationAccumulated { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }

    public class FM_PigOriginalAssetsdetaillistEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int RecordId { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 耳号
        /// </summary>
        public string EarNumber { get; set; }
        /// <summary>
        /// 猪只类型
        /// </summary>
        public string PigType { get; set; }
        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmId { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 原值
        /// </summary>
        public decimal? OriginalValue { get; set; }
        /// <summary>
        /// 折旧年限（月）
        /// </summary>
        public int? DepreciationUseMonth { get; set; }
        /// <summary>
        /// 净残值率（%）
        /// </summary>
        public decimal? ResidualValueRate { get; set; }
        /// <summary>
        /// 净残值
        /// </summary>
        public decimal? ResidualValue { get; set; }
        /// <summary>
        /// 开始使用日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 已计提月份
        /// </summary>
        public int? AccruedMonth { get; set; }
        /// <summary>
        /// 累计折旧
        /// </summary>
        public decimal? DepreciationAccumulated { get; set; }
        /// <summary>
        /// 月折旧额
        /// </summary>
        public decimal? DepreciationMonthAmount { get; set; }
        /// <summary>
        /// 月折旧率
        /// </summary>
        public decimal? DepreciationMonthRate { get; set; }
        /// <summary>
        /// 净值
        /// </summary>
        public decimal? NetValue { get; set; }
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
