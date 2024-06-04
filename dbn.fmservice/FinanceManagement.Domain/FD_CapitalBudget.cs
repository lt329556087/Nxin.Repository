using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace FinanceManagement.Domain
{
    public class FD_CapitalBudget : EntityBase
    {
        public FD_CapitalBudget()
        {
            DataDate = DateTime.Now;
            CapitalBudgetAbstract = "201612130104402203";
            Details = new List<FD_CapitalBudgetDetail>();
        }

        public void AddDetail(FD_CapitalBudgetDetail detail)
        {
            Details.Add(detail);
        }


        public void Update(DateTime DataDate, string TicketedPointID, string Number, string CapitalBudgetAbstract, DateTime StartDate, DateTime EndDate, string Remarks, DateTime ModifiedDate, string MarketID)
        {
            this.DataDate = DataDate;
            this.TicketedPointID = TicketedPointID;
            this.Number = Number;
            this.CapitalBudgetAbstract = CapitalBudgetAbstract;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.Remarks = Remarks;
            this.ModifiedDate = ModifiedDate;
            this.MarketID = MarketID;
        }
        [Key]
        public string NumericalOrder { get; set; }
        public int RecordID { get; set; }

        public Guid Guid { get; set; }
        public DateTime DataDate { get; set; }
        public string TicketedPointID { get; set; }
        public string Number { get; set; }
        public string CapitalBudgetType { get; set; }
        public string CapitalBudgetAbstract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }

        public string MarketID { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal? Amount { get; set; }

        public List<FD_CapitalBudgetDetail> Details { get; set; }
    }

    public class FD_CapitalBudgetDetail : EntityBase
    {
        public void Update(string ReceiptAbstractID, string PaymentObjectId, decimal PayAmount, decimal ReceiptAmount, string Remarks)
        {
            this.ReceiptAbstractID = ReceiptAbstractID;
            this.PaymentObjectID = PaymentObjectId;
            this.PayAmount = PayAmount;
            this.ReceiptAmount = ReceiptAmount;
            this.Remarks = Remarks;
        }

        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string PaymentObject { get; set; }
        public decimal Amount { get; set; }

        public decimal PayAmount { get; set; }

        public decimal ReceiptAmount { get; set; }

        public string Remarks { get; set; }

        public string PaymentObjectID { get; set; }


        public FD_CapitalBudget FD_CapitalBudget { get; set; }
    }
}
