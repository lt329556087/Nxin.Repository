
using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
namespace FinanceManagement.Domain
{
    public class FD_Payextend : EntityBase
    {
        public FD_Payextend()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
        }

        public void Update(string PayCode, string PayTypeName, DateTime? PayTime, int PayStatus, string Remarks)
        {
            if (!string.IsNullOrEmpty(PayCode))
            {
                this.PayCode = PayCode;
            }
            if (!string.IsNullOrEmpty(PayTypeName))
            {
                this.PayTypeName = PayTypeName;
            }
            if (PayTime!=null)
            {
                this.PayTime = PayTime;
            }
            if (!string.IsNullOrEmpty(Remarks))
            {
                this.Remarks = Remarks;
            }
            this.PayStatus = PayStatus;
            this.ModifiedDate = DateTime.Now;
        }
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
        public DateTime? PayTime { get; set; }
        //public int DPayStatus { get; set; }
        public int? PayStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Purpose { get; set; }
        public string BankRoute { get; set; }
        public int AuditLevel { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 批次号（用于安全认证）
        /// </summary>
        public string BatchNo { get; set; }
    }
}
