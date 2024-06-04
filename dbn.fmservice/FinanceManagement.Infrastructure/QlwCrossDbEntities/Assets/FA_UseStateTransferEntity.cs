using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_UseStateTransferODataEntity : OneWithManyQueryEntity<FA_UseStateTransferDetailODataEntity>
    {    
        [Key]
        public string Guid { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        private DateTime _CreatedDate;
        public string CreatedDate
        {
            get { return _CreatedDate.ToString(); }
            set { _CreatedDate = Convert.ToDateTime(value); }
        }


        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }
        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
        #region 列表查询 
        //审核人
        public string CheckedByName { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>		
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>		
        public string AssetsCode { get; set; }

        /// <summary>
        /// 变动前使用状态名称
        /// </summary>		
        public string BeforeUseStateName { get; set; }
        /// <summary>
        /// 变动后使用状态名称
        /// </summary>		
        public string AfterUseStateName { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public string AuditStatus{ get; set; }
        public string AuditStatusName { get; set; }
        /// <summary>
        /// 变动原因 表体
        /// </summary>
        //public string Remarks { get; set; }
        #endregion
    }

    public class FA_UseStateTransferDetailODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }


        /// <summary>
        /// 卡片ID
        /// </summary>		
        public string CardID { get; set; }


        /// <summary>
        /// 变动前使用状态
        /// </summary>		
        public string BeforeUseStateID { get; set; }
        /// <summary>
        /// 变动后使用状态
        /// </summary>		
        public string AfterUseStateID { get; set; }
        public string Remarks { get; set; }

        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }
        /// <summary>
        /// 卡片编码
        /// </summary>		
        public string CardCode { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>		
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>		
        public string AssetsCode { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>		
        public string Specification { get; set; }
        /// <summary>
        /// 开始使用时间
        /// </summary>		
        public string StartDate { get; set; }


        /// <summary>
        /// 变动前使用状态名称
        /// </summary>		
        public string BeforeUseStateName { get; set; }
        /// <summary>
        /// 变动后使用状态名称
        /// </summary>		
        public string AfterUseStateName { get; set; }
    }
    //public class FA_UseStateTransferListODataEntity
    //{
    //    [Key]
    //    /// <summary>
    //    /// NumericalOrder
    //    /// </summary>		
    //    public string NumericalOrder { get; set; }
    //    /// <summary>
    //    /// 日期
    //    /// </summary>
    //    public string DataDate { get; set; }
    //    /// <summary>
    //    /// 单据号
    //    /// </summary>
    //    public string Number { get; set; }

    //    /// <summary>
    //    /// OwnerID
    //    /// </summary>		
    //    public string OwnerID { get; set; }

    //    /// <summary>
    //    /// EnterpriseID
    //    /// </summary>		
    //    public string EnterpriseID { get; set; }
    //    public string OwnerName { get; set; }
       
    //    public string EnterpriseName { get; set; }
    //    //审核人
    //    public string CheckedByName { get; set; }       
    //    /// <summary>
    //    /// 资产名称
    //    /// </summary>		
    //    public string AssetsName { get; set; }
    //    /// <summary>
    //    /// 资产编码
    //    /// </summary>		
    //    public string AssetsCode { get; set; }
     
    //    /// <summary>
    //    /// 变动前使用状态名称
    //    /// </summary>		
    //    public string BeforeUseStateName { get; set; }
    //    /// <summary>
    //    /// 变动后使用状态名称
    //    /// </summary>		
    //    public string AfterUseStateName { get; set; }
    //    /// <summary>
    //    /// 变动原因 表体
    //    /// </summary>
    //    public string Remarks { get; set; }
    //}
}
