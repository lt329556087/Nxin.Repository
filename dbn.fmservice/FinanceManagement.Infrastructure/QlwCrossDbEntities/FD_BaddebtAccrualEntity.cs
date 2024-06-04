using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BaddebtAccrualODataEntity : OneWithManyQueryEntity<FD_BaddebtAccrualDetailODataEntity>
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
        public string NumericalOrderSetting { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

        public string TicketedPointName { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// 审核状态  0： '未审核' 1： '已审核'
        /// </summary>
        public int CheckState { get; set; }
        public string CheckStateName { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款） 科目
        /// </summary>
        public int? BusiType { get; set; }
        public string BusiTypeName { get; set; }
        public decimal Amount { get; set; }
        public decimal AccrualAmount { get; set; }
        [NotMapped]
        public string NumericalOrderDetail { get; set; }
    }
    public class FD_BaddebtAccrualDetailODataEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int? BusiType { get; set; }
        /// <summary>
        /// 计提类型
        /// </summary>
        public string TypeID { get; set; }
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
        /// 坏账准备金额
        /// </summary>
        public decimal AccrualAmount { get; set; }

        public string ModifiedDate { get; set; }
        /// <summary>
        /// 往来类型名称
        /// </summary>
        public string BusinessTypeName { get; set; }
        public string CurrentUnitName { get; set; }
        public string TypeName { get; set; }
        //public bool IsProvision { get; set; }
        [NotMapped]
        public List<FD_BaddebtAccrualExtODataEntity> AgingList { get; set; }
       
        //[NotMapped]
        //public string AccoSubjectID { get; set; }
        //[NotMapped]
        //public string AccoSubjectCode { get; set; }
    }

    /// <summary>
    /// 扩展表
    /// </summary>
    public class FD_BaddebtAccrualExtODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：坏账准备 1：账龄区间）
        /// </summary>
        public int? BusiType { get; set; }
        /// <summary>
        /// 区间金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 区间名称
        /// </summary>
        public string Name { get; set; }
        public string ModifiedDate { get; set; }
    }

}
