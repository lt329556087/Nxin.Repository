using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_CarryForwardVoucherRecord : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderCarry { get; set; }
        public string NumericalOrderSettl { get; set; }
        public string TransferAccountsType { get; set; }
        public string CarryName { get; set; }
        public string TransferAccountsAbstract { get; set; }
        public string TicketedPointID { get; set; }
        public string DataSource { get; set; }
        public string OwnerID { get; set; }
        public string ImplementResult { get; set; }
        public bool ResultState { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime TransBeginDate { get; set; }
        public DateTime TransEndDate { get; set; }
        public string TransSummary { get; set; }
        public string TransSummaryName { get; set; }
        public string TransWhereList { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

}
