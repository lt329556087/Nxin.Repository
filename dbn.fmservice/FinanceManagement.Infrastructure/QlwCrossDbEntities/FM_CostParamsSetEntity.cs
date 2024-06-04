using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_CostParamsSetEntity
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 卡片生成方式
        /// </summary>
        public string GenerationMode { get; set; }
        /// <summary>
        /// 总折旧月数
        /// </summary>
        public int TotalDepreciationMonths { get; set; }
        /// <summary>
        /// 残值计算方法
        /// </summary>
        public string ResidualValueCalMethod { get; set; }
        /// <summary>
        /// 残值率(%)
        /// </summary>
        public decimal? ResidualValueRate { get; set; }

        /// <summary>
        /// 固定残值
        /// </summary>
        public decimal? ResidualValue { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        public List<FM_CostParamsSetDetailEntity> Details { get; set; } = new List<FM_CostParamsSetDetailEntity>();

        [NotMapped]
        public List<FM_CostParamsSetExtendEntity> Extends { get; set; } = new List<FM_CostParamsSetExtendEntity>();

        [NotMapped]
        public List<FM_CostParamsSetExtendEntity> DepreciationExtends { get; set; } = new List<FM_CostParamsSetExtendEntity>();
    }

    public class FM_CostParamsSetDetailEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 猪场ID
        /// </summary>
        public string PigFarmId { get; set; }

        /// <summary>
        /// 猪场名称
        /// </summary>
        public string PigFarmName { get; set; }

        /// <summary>
        /// 猪场建账期间
        /// </summary>
        public string BeginPeriod { get; set; }
        /// <summary>
        /// 猪场建账日期
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 成本启用期间
        /// </summary>
        public string EnablePeriod { get; set; }
        /// <summary>
        /// 成本计算开始日期
        /// </summary>
        public DateTime EnableDate { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }

    public class FM_CostParamsSetExtendEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 扩展类型ID（生成方式/总折旧月数）
        /// </summary>
        public string ExtendTypeId { get; set; }

        public string ExtendTypeName { get; set; }

        /// <summary>
        /// 来源类型ID
        /// </summary>
        public string SourceTypeId { get; set; }

        public string SourceTypeName { get; set; }
        /// <summary>
        /// 生成方式/获取方式
        /// </summary>
        public string CreatedTypeId { get; set; }
        public string CreatedTypeName { get; set; }
        /// <summary>
        /// 总折旧月数
        /// </summary>
        public int TotalDepreciationMonths { get; set; }
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
