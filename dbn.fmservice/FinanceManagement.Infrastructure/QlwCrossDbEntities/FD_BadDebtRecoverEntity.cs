using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BadDebtRecoverODataEntity : OneWithManyQueryEntity<FD_BadDebtRecoverDetailODataEntity>
    {
        public FD_BadDebtRecoverODataEntity()
        {
            Lines1 = new List<FD_BadDebtRecoverDetailODataEntity>();
            Lines2 = new List<FD_BadDebtRecoverDetailODataEntity>();
            BusinessType = "201611160104402101";
        }

        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }

        /// <summary>
        /// 收款单流水号
        /// </summary>
        public string NumericalOrderReceive { get; set; }

        /// <summary>
        /// 收款单单据号
        /// </summary>
        public string NumberReceive { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public string DataDate { get; set; }

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

        //贷方科目
        public string CAccoSubjectID { get; set; }


        public string CAccoSubjectName { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID1 { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectName1 { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID2 { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectName2 { get; set; }

        public string NumericalOrderSetting { get; set; }

        [NotMapped]
        public List<FD_BadDebtRecoverDetailODataEntity> Lines1 { get; set; }

        [NotMapped]
        public List<FD_BadDebtRecoverDetailODataEntity> Lines2 { get; set; }

    }

    public class FD_BadDebtRecoverDetailODataEntity
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


        public string MarketName { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string PersonID { get; set; }

        public string PersonName { get; set; }

        /// <summary>
        /// 本次坏账收回金额
        /// </summary>
        public decimal CurrentRecoverAmount { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }
    }


    public class FD_BadDebtRecoverListOnly
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }


        public string CreateDate { get; set; }

        /// <summary>
        ///客户ID
        /// </summary>
        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public string OwnerName { get; set; }

        public string OwnerID { get; set; }

        public string ReviewName { get; set; }

        public string ReviewID { get; set; }

        public decimal CurrentRecoverAmount { get; set; }
    }


    public class FD_PaymentReceivablesODataEntity
    {
        [Key]
        public string NumericalOrder { get; set; }

        public string Number { get; set; }

        public string CustomerID { get; set; }

        public string CustomerName { get; set; }


        public string ReceiptAbstractName { get; set; }

        public decimal Amount { get; set; }

        public string DataDate { get; set; }

        public string PersonID { get; set; }

        public string PersonName { get; set; }

        public string MarketID { get; set; }

        public string MarketName { get; set; }

        public string AccoSubjectCode { get; set; }

    }
}
