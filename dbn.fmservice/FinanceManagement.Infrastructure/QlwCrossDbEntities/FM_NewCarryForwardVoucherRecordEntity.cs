using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_NewCarryForwardVoucherRecordODataEntity 
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderCarry { get; set; }
        public string NumericalOrderSettl { get; set; }
        public string TransferAccountsType { get; set; }
        public string TransferAccountsName { get; set; }
        public string CarryName { get; set; }
        public string DataSource { get; set; }
        public string DataSourceName { get; set; }
        public string Number { get; set; }
        public string DataDate { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string ImplementResult { get; set; }
        public bool ResultState { get; set; }
        public string CreatedDate { get; set; }
        public string TransBeginDate { get; set; }
        public string TransEndDate { get; set; }
        public string TransferAccountsAbstract { get; set; }
        public string TransSummary { get; set; }
        public string TransSummaryName { get; set; }
        public string TransWhereList { get; set; }
        public string TransferAccountsSort { get; set; }
        public string Remarks { get; set; }
    }
    public class JXCModel
    {
        [Key]
        public string Guid { get; set; }
        public decimal Amount { get; set; }
    }
}
