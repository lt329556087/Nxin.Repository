using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BaddebtWriteOffODataEntity : OneWithManyQueryEntity<FD_BaddebtWriteOffDetailODataEntity>
    {

        [Key]
        public string Guid { get; set; }        
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

        public string TicketedPointName { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// 审核状态  0： '未审核' 1： '已审核'
        /// </summary>
        public int? CheckState { get; set; }
        public string CheckStateName { get; set; }
        public int? RecordID { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款） 科目
        /// </summary>
        public int? BusiType { get; set; }
        public string BusiTypeName { get; set; }
        public decimal Amount { get; set; }
        public decimal WriteOffAmount { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CurrentUnit { get; set; }
        /// <summary>
        /// 往来类型名称
        /// </summary>
        public string BusinessTypeName { get; set; }
        public string CurrentUnitName { get; set; }
    }
    public class FD_BaddebtWriteOffDetailODataEntity
    {
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
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int? BusiType { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CurrentUnit { get; set; }
       
        /// <summary>
        /// 期末余额
        /// </summary>

        public decimal Amount { get; set; }
        /// <summary>
        /// 坏账核销金额金额
        /// </summary>
        public decimal WriteOffAmount { get; set; }

        public string ModifiedDate { get; set; }
        /// <summary>
        /// 往来类型名称
        /// </summary>
        public string BusinessTypeName { get; set; }
        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CurrentUnitName { get; set; }
    }

}
