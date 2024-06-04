using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_PayextendODataEntity
    {        
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string BusinessType { get; set; }
        public string OrderNo { get; set; }
        public string PayNO { get; set; }
        public string ScOrderNo { get; set; }
        public string PayeeID { get; set; }
        public string PayerID { get; set; }
        public string PayCardNo { get; set; }
        public string ReceCardNo { get; set; }
        public string PayCode { get; set; }
        public string PayTypeName { get; set; }
        public string PayTime { get; set; }
        //public int DPayStatus { get; set; }
        public int? PayStatus { get; set; }
        public string Remarks { get; set; }
        public string CreatedDate { get; set; }
        public string Purpose { get; set; }
        public string BankRoute { get; set; }
        public int AuditLevel { get; set; }
        public string ModifiedDate { get; set; }
        public decimal Amount { get; set; }

    }


}
