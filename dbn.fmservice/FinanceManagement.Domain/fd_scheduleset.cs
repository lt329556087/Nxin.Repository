﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public partial class fd_scheduleset : EntityBase
    {
        [Key]
        public long RecordId { get; set; }
        public string GroupId { get; set; }
        public int Level { get; set; }
        public int? StayDay { get; set; }
        public long OwnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}