﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public partial class fd_paymentreceivablesvoucher : EntityBase
    {
        public fd_paymentreceivablesvoucher()
        {
            details = new List<fd_paymentreceivablesvoucherdetail>();
        }
        [Key]
        public string NumericalOrder { get; set; }
        public Guid? Guid { get; set; }
        public string SettleReceipType { get; set; }
        public DateTime DataDate { get; set; }
        public string TicketedPointID { get; set; }
        public string Number { get; set; }
        public string AccountNo { get; set; }
        public string AttachmentNum { get; set; }
        public string Remarks { get; set; }
        public string EnterpriseID { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        [NotMapped]
        public List<fd_paymentreceivablesvoucherdetail> details { get; set; }
    }
}