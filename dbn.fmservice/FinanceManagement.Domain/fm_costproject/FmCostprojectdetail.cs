﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FinanceManagement.Domain
{
    public partial class FmCostprojectdetail : EntityBase
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
        /// <summary>
        /// 关联ID
        /// </summary>
        public string RelatedId { get; set; }
        /// <summary>
        /// 辅助核算
        /// </summary>
        public string SubsidiaryAccounting { get; set; }
        /// <summary>
        /// 取数公式
        /// </summary>
        public string DataFormula { get; set; }

        public List<FmCostprojectExtend> ExtendDetails { get; set; } = new List<FmCostprojectExtend>();
    }
}