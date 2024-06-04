﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Architecture.Seedwork.Domain;
#nullable disable

namespace FinanceManagement.Domain
{
    [Table("fm_pigoriginalassets")]
    public partial class FmPigoriginalassets : EntityBase
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [Key]
        [Column(TypeName = "bigint(20)")]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        [Column(TypeName = "bigint(20)")]
        public string Number { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        [Column(TypeName = "date")]
        public string DataDate { get; set; }
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
        /// 来源类型
        /// </summary>
        [Column(TypeName = "bigint(20)")]
        public string SourceType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1024)]
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        [Column("OwnerID", TypeName = "bigint(20)")]
        public string OwnerId { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [Column("ModifiedOwnerID", TypeName = "bigint(20)")]
        public string ModifiedOwnerId { get; set; }
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