using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_CostProjectEntity
    {
        /// <summary>
        /// 费用项目ID
        /// </summary>
        [Key]
        public string CostProjectId { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseId { get; set; }
        
        public string CostProjectName { get; set; }
        /// <summary>
        /// 项目分类
        /// </summary>
        public string? CostProjectTypeId { get; set; }
        public string CostProjectTypeName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public ulong? IsUse { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int? OrderNumber { get; set; }
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
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// 费用编码
        /// </summary>
        public string CostProjectCode { get; set; }
        /// <summary>
        /// 归集类型
        /// </summary>
        public string CollectionType { get; set; }
        public string CollectionTypeName { get; set; }
        /// <summary>
        /// 分摊方式
        /// </summary>
        public string AllocationType { get; set; }
        public string AllocationTypeName { get; set; }
        /// <summary>
        /// 取数来源
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedOwnerID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedOwnerName { get; set; }

        /// <summary>
        /// 绑定预置项
        /// </summary>
        public string PresetItem { get; set; }

        /// <summary>
        /// 绑定预置项名称
        /// </summary>
        public string PresetItemName { get; set; }

        [NotMapped]
        public List<FM_CostProjectDetailEntity> Details { get; set; }
    }

    public class FM_CostProjectDetailEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        /// <summary>
        /// 费用项目ID
        /// </summary>
        public string CostProjectId { get; set; }
        /// <summary>
        /// 关联类型
        /// </summary>
        public string RelatedType { get; set; }
        public string RelatedTypeName { get; set; }
        /// <summary>
        /// 关联ID
        /// </summary>
        public string RelatedId { get; set; }
        public string RelatedName { get; set; }
        /// <summary>
        /// 辅助核算
        /// </summary>
        public string SubsidiaryAccounting { get; set; }
        /// <summary>
        /// 取数公式
        /// </summary>
        public string DataFormula { get; set; }
        public string DataFormulaStr { get; set; }

        [NotMapped]
        public List<FmCostprojectExtendEntity> ExtendDetails { get; set; }
    }

    public class FmCostprojectExtendEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        /// <summary>
        /// 费用项目明细ID
        /// </summary>
        public string DetailID { get; set; }
        /// <summary>
        /// 关联类型
        /// </summary>
        public string RelatedType { get; set; }
        /// <summary>
        /// 关联ID
        /// </summary>
        public string RelatedId { get; set; }

        public string RelatedName { get; set; }
    }

    public class FM_CostProjectValidateEntity
    {
        /// <summary>
        /// 费用项目ID
        /// </summary>
        [Key]
        public string CostProjectID { get; set; }
        
        public string CostProjectName { get; set; }
        
        /// <summary>
        /// 费用编码
        /// </summary>
        public string CostProjectCode { get; set; }
    }

    public class FM_EnterpriseEntity
    {
        /// <summary>
        /// 单位ID
        /// </summary>
        [Key]
        public string EnterpriseID { get; set; }

        public string OwnerID { get; set; }
    }
}
