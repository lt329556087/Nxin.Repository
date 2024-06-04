using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    /// <summary>
    /// 坏账计提准备
    /// </summary>
    public class FD_BadDebtProvisionODataEntity : OneWithManyQueryEntity<FD_BadDebtProvisionDetailODataEntity>
    {
        public FD_BadDebtProvisionODataEntity()
        {
            Lines1 = new List<FD_BadDebtProvisionDetailODataEntity>();
            Lines2 = new List<FD_BadDebtProvisionDetailODataEntity>();
        }

        /// <summary>
        /// 流水号
        /// </summary>
        /// 

        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointID { get; set; }

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

        [NotMapped]
        public decimal SumProvisionAmount1 { get; set; }

        [NotMapped]
        public decimal SumProvisionAmount2 { get; set; }

        /// <summary>
        /// 已计提总金额1
        /// </summary>
        public decimal HaveProvisionAmount1 { get; set; }


        /// <summary>
        /// 已计提总金额2
        /// </summary>
        public decimal HaveProvisionAmount2 { get; set; }

        [NotMapped]
        /// <summary>
        /// 计提差额1
        /// </summary>
        public decimal DiffAmount1 { get; set; }


        [NotMapped]
        /// <summary>
        /// 计提差额2
        /// </summary>
        public decimal DiffAmount2 { get; set; }

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
        public List<FD_BadDebtProvisionDetailODataEntity> Lines1 { get; set; }


        [NotMapped]
        public List<FD_BadDebtProvisionDetailODataEntity> Lines2 { get; set; }
    }

    /// <summary>
    /// 坏账计提准备-表体
    /// </summary>
    public class FD_BadDebtProvisionDetailODataEntity
    {
        public FD_BadDebtProvisionDetailODataEntity()
        {
            AgingList = new List<FD_BadDebtProvisionExtODataEntity>();
        }

        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }

        [Key]

        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// 科目id
        /// </summary>
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string AccoSubjectName { get; set; }

        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerName { get; set; }

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

        public string BusinessType { get; set; }

        public string ProvisionType { get; set; }

        public string ProvisionTypeName { get; set; }

        public decimal ReclassAmount { get; set; }

        public decimal EndAmount { get; set; }



        [NotMapped]
        public List<FD_BadDebtProvisionExtODataEntity> AgingList { get; set; }
    }

    /// <summary>
    /// 扩展表
    /// </summary>
    public class FD_BadDebtProvisionExtODataEntity
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
        /// 账龄名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        public decimal Ratio { get; set; } = 1;
    }


    public class FD_BadDebtProvisionListOnlyODataEntity
    {
        [Key]
        public string NumericalOrder { get; set; }


        public string DataDate { get; set; }

        public string Number { get; set; }

        public decimal ProvisionAmount { get; set; }

        public string OwnerName { get; set; }

        public string OwnerID { get; set; }

        public string ReviewName { get; set; }

        public string ReviewID { get; set; }
    }
}
