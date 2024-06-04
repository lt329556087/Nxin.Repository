using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace FinanceManagement.Domain
{
    /// <summary>
    /// 坏账计提准备
    /// </summary>
    public class FD_BadDebtProvision : EntityBase
    {
        public FD_BadDebtProvision()
        {
            Lines = new List<FD_BadDebtProvisionDetail>();
        }

        [Key]
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointID { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        public DateTime CreateDate { get; set; }

        public string AccoSubjectID1 { get; set; }
        public string AccoSubjectID2 { get; set; }

        public string NumericalOrderSetting { get; set; }

        public decimal HaveProvisionAmount1 { get; set; }
        public decimal HaveProvisionAmount2 { get; set; }

        [ForeignKey(nameof(NumericalOrder))]
        public List<FD_BadDebtProvisionDetail> Lines { get; set; }
    }

    /// <summary>
    /// 坏账计提准备-表体
    /// </summary>
    public class FD_BadDebtProvisionDetail : EntityBase
    {
        public FD_BadDebtProvisionDetail()
        {
            AgingList = new List<FD_BadDebtProvisionExt>();
        }

        [Key]
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 科目id
        /// </summary>
        public string AccoSubjectID { get; set; }


        public string NumericalOrder { get; set; }


        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 未收回金额
        /// </summary>
        public decimal NoReceiveAmount { get; set; }
        /// <summary>
        /// 本期坏账计提准备金额
        /// </summary>
        public decimal CurrentDebtPrepareAmount { get; set; }
        /// <summary>
        /// 上期坏账计提准备金额
        /// </summary>
        public decimal LastDebtPrepareAmount { get; set; }

        /// <summary>
        /// 调整金额
        /// </summary>
        public decimal TransferAmount { get; set; }

        /// <summary>
        /// 本期计提金额
        /// </summary>
        public decimal ProvisionAmount { get; set; }

        public string NumericalOrderSpecific { get; set; }


        public string ProvisionType { get; set; }

        public string BusinessType { get; set; }


        public decimal ReclassAmount { get; set; }

        public decimal EndAmount { get; set; }




        [ForeignKey(nameof(NumericalOrderDetail))]

        public List<FD_BadDebtProvisionExt> AgingList { get; set; }
    }

    /// <summary>
    /// 扩展表
    /// </summary>
    public class FD_BadDebtProvisionExt : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 计提表体ID
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 账龄ID
        /// </summary>
        public int AgingID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        public decimal  Ratio { get; set; }

        public string Name { get; set; }
    }
}
