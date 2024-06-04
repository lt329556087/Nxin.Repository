using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FD_Expense : EntityBase
    {
        public FD_Expense()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID = "0";
        }

        public void Update(DateTime DataDate,  string Remarks, string EnterpriseID)
        {
            this.DataDate = DataDate;
            this.Remarks = Remarks;
            this.EnterpriseID = EnterpriseID;
            this.ModifiedDate = DateTime.Now;
        }
        public void UpdateApply(DateTime? PayDate, string DraweeID)
        {
            this.PayDate = PayDate;
            this.DraweeID = DraweeID;
        }
        public void UpdateDetail(DateTime DataDate, string Remarks, string ExpenseAbstract, string ExpenseSort, string PersonID, DateTime? HouldPayDate, DateTime? PayDate, DateTime? StartDate,
            DateTime? EndDate, string DraweeID, decimal CurrentVerificationAmount, int Pressing,string TicketedPointID)
        {
            this.DataDate = DataDate;
            this.Remarks = Remarks;
            this.ExpenseAbstract = ExpenseAbstract;
            this.ExpenseSort = ExpenseSort;
            this.PersonID = PersonID;
            this.HouldPayDate = HouldPayDate;
            this.PayDate = PayDate;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.DraweeID = DraweeID;
            this.CurrentVerificationAmount = CurrentVerificationAmount;
            this.Pressing = Pressing;
            this.TicketedPointID = TicketedPointID;
            this.ModifiedDate = DateTime.Now;
        }
        /// <summary>
        /// auto_increment
        /// </summary>
        //[Key]/
        //public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>        
        [Key]
        public string NumericalOrder { get; set; }    
        //
        public DateTime? DataDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public DateTime? PayDate { get; set; }
        public string DraweeID { get; set; }
        
        public Guid Guid { get; set; }
        public string ExpenseType { get; set; }
        public string ExpenseAbstract { get; set; }
        public string ExpenseSort { get; set; }
        public string PersonID { get; set; }
        public DateTime? HouldPayDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal CurrentVerificationAmount { get; set; }
        public int Pressing { get; set; }
        public string Number { get; set; }
        public string TicketedPointID { get; set; }
    }

    public class FD_ExpenseDetail : EntityBase
    {

        public void Update(string ReceiptAbstractID, string BusinessType,string SettleBusinessType)
        {
            this.ReceiptAbstractID = ReceiptAbstractID;
            this.BusinessType = BusinessType;
            this.SettleBusinessType= SettleBusinessType;
        }
        [Key]
        /// <summary>
        /// 
        /// </summary>		
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string BusinessType { get; set; }
        public string SettleBusinessType { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }
        public string ProjectID { get; set; }
        public string SettlePayerID { get; set; }
        public decimal Amount { get; set; }
        public string Content { get; set; }
        public string AccountInformation { get; set; }
        public string ReceiptAbstractDetail { get; set; }
    }

    public class FD_ExpenseExt : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string CollectionId { get; set; }
        public string PersonId { get; set; }
        public string AccountName { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
    }

    #region
    //public class fd_expenseextend
    //{
    //    public int RecordID { get; set; }
    //    public long NumericalOrder { get; set; }
    //    public long NumericalOrderDetail { get; set; }
    //    public Guid Guid { get; set; }
    //    public long ExpenseType { get; set; }
    //    public decimal Amount { get; set; }
    //    public string Remarks { get; set; }
    //}
    //public class fd_expenselist
    //{
    //    public int RecordID { get; set; }
    //    public long NumericalOrder { get; set; }
    //    public string Guid { get; set; }
    //    public string Code { get; set; }
    //    public string Number { get; set; }
    //    public string HashCode { get; set; }
    //    public string Remarks { get; set; }
    //    public int SerialNumber { get; set; }
    //    public string FilePath { get; set; }
    //    public string FileName { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //}
    #endregion
}
