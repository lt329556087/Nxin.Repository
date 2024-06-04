using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FmCostprojectExtend : EntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        /// <summary>
        /// 费用项目明细ID
        /// </summary>
        public int DetailID { get; set; }
        /// <summary>
        /// 关联类型
        /// </summary>
        public string RelatedType { get; set; }
        /// <summary>
        /// 关联ID
        /// </summary>
        public string RelatedId { get; set; }
    }
}
