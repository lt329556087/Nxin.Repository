using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_VoucherAmortizationRecordODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderVoucher { get; set; }
        public string NumericalOrderSettl { get; set; }
        public string AmortizationName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string ImplementResult { get; set; }
        public bool ResultState { get; set; }
        public string ResultStateName { get; set; }
        public string EnterpriseID { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string Number { get; set; }
    }
}
