﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Domain
{
    public partial class fd_settletypeset: EntityBase
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string SettleReceipType { get; set; }
        public string GroupId { get; set; }
        public string Remarks { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}