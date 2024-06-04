﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Architecture.Seedwork.Domain;
#nullable disable

namespace FinanceManagement.Domain
{
    [Table("fm_pigoriginalassetsdetaillist")]
    //[Index(nameof(EarNumber), nameof(DataDate), Name = "Num_Date")]
    public partial class FmPigoriginalassetsdetaillist : EntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RecordID", TypeName = "int(11)")]
        public int RecordId { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        [Column(TypeName = "date")]
        public string DataDate { get; set; }
        /// <summary>
        /// 耳号
        /// </summary>
        [Column(TypeName = "bigint(20)")]
        public string EarNumber { get; set; }
        /// <summary>
        /// 猪只类型
        /// </summary>
        [Column(TypeName = "bigint(20)")]
        public string PigType { get; set; }
        /// <summary>
        /// 猪场ID
        /// </summary>
        [Column("PigFarmID", TypeName = "bigint(20)")]
        public string PigFarmId { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        [Column("EnterpriseID", TypeName = "bigint(20)")]
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 原值
        /// </summary>
        public decimal? OriginalValue { get; set; }
        /// <summary>
        /// 折旧年限（月）
        /// </summary>
        [Column(TypeName = "int(11)")]
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
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 已计提月份
        /// </summary>
        [Column(TypeName = "int(11)")]
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
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [Column(TypeName = "timestamp")]
        public DateTime ModifiedDate { get; set; }
    }
}