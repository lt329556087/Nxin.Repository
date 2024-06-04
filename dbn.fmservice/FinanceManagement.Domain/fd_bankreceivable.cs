﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;

namespace FinanceManagement.Domain
{
    public partial class fd_bankreceivable :EntityBase
    {
        public string NumericalOrder { get; set; }
        public string transIndex { get; set; }
        public string dataSource { get; set; }
        public string bankSerial { get; set; }
        public decimal? amount { get; set; }
        public DateTime? receiveDay { get; set; }
        public string entId { get; set; }
        public string entName { get; set; }
        public string acctIndex { get; set; }
        public string acctNo { get; set; }
        public string otherSideName { get; set; }
        public string otherSideAcctIndex { get; set; }
        public string otherSideAcct { get; set; }
        public decimal fee { get; set; }
        public string msgCode { get; set; }
        public string msg { get; set; }
        public string custList { get; set; }
        /// <summary>
        /// 1:已生成,2:未生成,3:自动生成
        /// </summary>
        public int? IsGenerate { get; set; }
        public string Remarks { get; set; }
        public string SourceNum { get; set; }
        public DateTime CreateTime { get; set; }
    }
}