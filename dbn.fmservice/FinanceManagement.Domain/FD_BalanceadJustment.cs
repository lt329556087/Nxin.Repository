using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FD_BalanceadJustment : EntityBase
    {
        public FD_BalanceadJustment()
        {
            Details = new List<FD_BalanceadJustmentDetail>();
        }

        public FD_BalanceadJustmentDetail AddDetail(FD_BalanceadJustmentDetail detail)
        {
            Details.Add(detail);
            return detail;
        }

        public void Update(DateTime DataDate, string AccountID, string Remarks)
        {
            this.DataDate = DataDate;
            this.AccountID = AccountID;
            this.Remarks = Remarks;
        }

        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }
        /// <summary>
        /// DataDate
        /// </summary>		
        public DateTime DataDate { get; set; }

        /// <summary>
        ///资金账户
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FD_BalanceadJustmentDetail> Details { get; set; }
    }

    public class FD_BalanceadJustmentDetail : EntityBase
    {

        public void Add() { }

        public void Update(string EnterProjectID, decimal EnterProjectAmount, string BankProjectID, decimal BankProjectAmount)
        {
            this.EnterProjectID = EnterProjectID;
            this.EnterProjectAmount = EnterProjectAmount;
            this.BankProjectID = BankProjectID;
            this.BankProjectAmount = BankProjectAmount;
        }
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// EnterProjectID
        /// </summary>		
        public string EnterProjectID { get; set; }

        /// <summary>
        /// EnterProjectAmount
        /// </summary>		
        public decimal EnterProjectAmount { get; set; }

        /// <summary>
        /// BankProjectID
        /// </summary>		
        public string BankProjectID { get; set; }

        /// <summary>
        /// BankProjectAmount
        /// </summary>		
        public decimal BankProjectAmount { get; set; }
        public FD_BalanceadJustment FD_BalanceadJustment { get; set; }

    }
}
