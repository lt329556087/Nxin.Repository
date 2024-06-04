using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FD_BadDebtOccur : EntityBase
    {
        public FD_BadDebtOccur()
        {
            Lines = new List<FD_BadDebtOccurDetail>();
        }

        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        public DateTime DataDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string TicketedPointID { get; set; }
        public string EnterpriseID { get; set; }
        public string CustomerID { get; set; }

        /// <summary>
        /// 坏账科目1
        /// </summary>
        public string CAccoSubjectID { get; set; }

        /// <summary>
        /// 坏账科目1
        /// </summary>
        public string AccoSubjectID1 { get; set; }

        /// <summary>
        /// 坏账科目2
        /// </summary>
        public string AccoSubjectID2 { get; set; }

        public string NumericalOrderSetting { get; set; }

        public string BusinessType { get; set; }

        public string PersonID { get; set; }

        [ForeignKey(nameof(NumericalOrder))]
        public List<FD_BadDebtOccurDetail > Lines { get; set; }
    }

    public class FD_BadDebtOccurDetail : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string MarketID { get; set; }
        public string PersonID { get; set; }

        /// <summary>
        /// 本次坏账发生金额
        /// </summary>
        public decimal CurrentOccurAmount { get; set; }

        /// <summary>
        /// 期末余额
        /// </summary>
        public decimal Amount { get; set; }

    }
}
