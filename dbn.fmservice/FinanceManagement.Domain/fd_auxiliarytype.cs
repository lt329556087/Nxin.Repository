﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Domain
{
    public partial class fd_auxiliarytype : EntityBase
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string TypeName { get; set; }
        public string TypeTag { get; set; }
        public string TypeCode { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string GroupId { get; set; }
        /// <summary>
        /// 是否往来
        /// </summary>
        public int IsCom { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}