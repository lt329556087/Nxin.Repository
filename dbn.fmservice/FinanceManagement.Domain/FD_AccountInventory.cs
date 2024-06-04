using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FD_AccountInventory : EntityBase
    {
        public FD_AccountInventory()
        {
            Details = new List<FD_AccountInventoryDetail>();
        }

        public FD_AccountInventoryDetail AddDetail(FD_AccountInventoryDetail detail)
        {
            Details.Add(detail);
            return detail;
        }

        public void Update(DateTime DataDate, string TicketedPointID, string ResponsiblePerson, string Remarks)
        {
            this.DataDate = DataDate;
            this.TicketedPointID = TicketedPointID;
            this.ResponsiblePerson = ResponsiblePerson;
            this.Remarks = Remarks;
        }

        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public Guid Guid { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>		
        public DateTime DataDate { get; set; }

        /// <summary>
        /// TicketedPointID
        /// </summary>		
        public string TicketedPointID { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// ResponsiblePerson
        /// </summary>		
        public string ResponsiblePerson { get; set; }

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

        public List<FD_AccountInventoryDetail> Details { get; set; }
    }

    public class FD_AccountInventoryDetail : EntityBase
    {

        public void Add() { }

        public void Update(string AccountID, string AccoSubjectID, string AccoSubjectCode, decimal FlowAmount, decimal DepositAmount, decimal FrozeAmount, decimal FuturesBond, decimal OtherBond, decimal BankFrozen, decimal OtherAmount, decimal BookAmount)
        {
            this.AccountID = AccountID;
            this.AccoSubjectID = AccoSubjectID;
            this.AccoSubjectCode = AccoSubjectCode;
            this.FlowAmount = FlowAmount;
            this.DepositAmount = DepositAmount;
            this.FrozeAmount = FrozeAmount;
            this.FuturesBond = FuturesBond;
            this.OtherBond = OtherBond;
            this.OtherAmount = OtherAmount;
            this.BankFrozen = BankFrozen;
            this.BookAmount = BookAmount;
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
        /// Guid
        /// </summary>		
        public Guid Guid { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// AccoSubjectID
        /// </summary>		
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// AccoSubjectCode
        /// </summary>		
        public string AccoSubjectCode { get; set; }

        /// <summary>
        /// FlowAmount
        /// </summary>		
        public decimal FlowAmount { get; set; }

        /// <summary>
        /// DepositAmount
        /// </summary>		
        public decimal DepositAmount { get; set; }

        /// <summary>
        /// 不可用资金
        /// </summary>		
        public decimal FrozeAmount { get; set; }

        /// <summary>
        /// 期货保证金
        /// </summary>
        public decimal FuturesBond { get; set; }

        /// <summary>
        /// 其他保证金
        /// </summary>
        public decimal OtherBond { get; set; }

        /// <summary>
        /// 银行冻结
        /// </summary>
        public decimal BankFrozen { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public decimal OtherAmount { get; set; }

        /// <summary>
        /// 账面金额
        /// </summary>
        public decimal BookAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }


        public FD_AccountInventory FD_AccountInventory { get; set; }

    }
}
