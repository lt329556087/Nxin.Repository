﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Domain
{
    public partial class fd_auxiliaryproject : EntityBase
    {
        [Key]
        public string ProjectId { get; set; }
        public string ProjectType { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public long? Level { get; set; }
        public string Pid { get; set; }
        public string OwnerID { get; set; }
        public string GroupId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 是否启用 1=启用,0=禁用
        /// </summary>
        public int IsUse { get; set; }
    }
}