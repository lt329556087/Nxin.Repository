﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Domain
{
    public partial class biz_relateddetail : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        public long RelatedID { get; set; }
        public long? RelatedDetailID { get; set; }
        public long? RelatedDetailType { get; set; }
        public decimal Payable { get; set; }
        public decimal Paid { get; set; }
        public decimal Payment { get; set; }
        public string Remarks { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long? OwnerID { get; set; }
    }
}