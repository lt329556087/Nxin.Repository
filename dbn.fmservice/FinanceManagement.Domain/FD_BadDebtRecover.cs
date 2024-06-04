using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FD_BadDebtRecover : EntityBase
    {
        public FD_BadDebtRecover()
        {
            Lines = new List<FD_BadDebtRecoverDetail>();
        }


        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }

        /// <summary>
        /// 收款单流水号
        /// </summary>
        public string NumericalOrderReceive { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTime DataDate { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointID { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        ///客户ID
        /// </summary>
        public string CustomerID { get; set; }

        public string PersonID { get; set; }

        public string BusinessType { get; set; }

        public string CAccoSubjectID { get; set; }


        public string AccoSubjectID1 { get; set; }

        public string AccoSubjectID2 { get; set; }

        public string NumericalOrderSetting { get; set; }

        [ForeignKey(nameof(NumericalOrder))]
        public List<FD_BadDebtRecoverDetail> Lines { get; set; }
    }

    public class FD_BadDebtRecoverDetail : EntityBase
    {

        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string PersonID { get; set; }

        /// <summary>
        /// 本次坏账收回金额
        /// </summary>
        public decimal CurrentRecoverAmount { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

    }
}
