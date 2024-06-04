using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_AccountTransferODataEntity : OneWithManyQueryEntity<FD_AccountTransferDetailODataEntity>
    {
        #region 业务说明
        //业务类似会计凭证：一条表头，两条表体（调入、调出）；生成调拨单时写入两条明细，调出账户、金额、日期生成付款单是写入；表头Remarks:事由， 表体Remarks：备注；
        #endregion

        // A.EnterpriseID,  C.EnterpriseID OutEnterpriseID, B.Amount, C.Amount RAmount


        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// AccountTransferType 调拨类型
        /// </summary>		
        public string AccountTransferType { get; set; }
        public string AccountTransferTypeName { get; set; }

        /// <summary>
        /// AccountTransferAbstract 调拨类别
        /// </summary>		
        public string AccountTransferAbstract { get; set; }

        /// <summary>
        /// 调拨类别名称
        /// </summary>
        public string AccountTransferAbstractName { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>	
        private DateTime _DateDate;

        public string DataDate
        {
            get { return _DateDate.ToString("yyyy-MM-dd"); }
            set { _DateDate = Convert.ToDateTime(value); }
        }

        /// <summary>
        /// Remarks 备注
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

        public decimal Amount { get; set; }



        public decimal RAmount { get; set; }
        public string OwnerName { get; set; }

        //public string Results { get; set; }
        //public string ResultsName { get; set; }
        //调出单位
        public string OutEnterpriseID { get; set; }
        public string OutEnterpriseName { get; set; }

        public int ApprovalState { get; set; }

        public string ApprovalStateName { get; set; }

        public string UploadUrl { get; set; }
        //public bool IsCashSweep { get; set; }
        /// <summary>
        /// 资金归集流水号
        /// </summary>
        public string NumericalOrderForCashSweep { get; set; }

    }

    public class FD_AccountTransferDetailODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public string RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// EnterpriseID 调入/出单位
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// Amount
        /// </summary>		
        public decimal Amount { get; set; }

        public string AmountUpper { get; set; }

        public string PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// IsIn
        /// </summary>		
        public bool IsIn { get; set; }

        /// <summary>
        /// DataDateTime 归还/调出时间
        /// </summary>		
        private DateTime _DataDateTime;
        public string DataDateTime
        {
            get { return _DataDateTime.ToString("yyyy-MM-dd"); }
            set { _DataDateTime = Convert.ToDateTime(value); }
        }

        /// <summary>
        /// Remarks 事由
        /// </summary>		
        public string Remarks { get; set; }

        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }
        public string AccountName { get; set; }
        public string DepositBank { get; set; }
        public string AccountNumber { get; set; }
        public string EnterpriseName { get; set; }
    }
}
