using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FD_AccountTransfer : EntityBase
    {
        public FD_AccountTransfer()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            DataDate = DateTime.Now;
            AccountTransferType = "0";
            AccountTransferAbstract = "0";
            NumericalOrder = "0";
            OwnerID = EnterpriseID = "0";
            Details = new List<FD_AccountTransferDetail>();
        }

        public void Update(string AccountTransferType, string AccountTransferAbstract, string DataDate, string Remarks, string OwnerID, string EnterpriseID)
        {
            this.AccountTransferType = AccountTransferType;
            this.AccountTransferAbstract = AccountTransferAbstract;
            this.DataDate = Convert.ToDateTime(DataDate);
            this.Remarks = Remarks;
            this.OwnerID = OwnerID;
            this.EnterpriseID = EnterpriseID;
        }

        public FD_AccountTransferDetail AddDetail(FD_AccountTransferDetail detail)
        {
            Details.Add(detail);
            return detail;
        }

        #region 业务说明
        //业务类似会计凭证：一条表头，两条表体（调入、调出）；生成调拨单时写入两条明细，调出账户、金额、日期生成付款单是写入；表头Remarks:事由， 表体Remarks：备注；
        #endregion
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public Guid Guid { get; set; }

        /// <summary>
        /// AccountTransferType 调拨类型
        /// </summary>		
        public string AccountTransferType { get; set; }

        /// <summary>
        /// AccountTransferAbstract 调拨类别
        /// </summary>		
        public string AccountTransferAbstract { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>	

        public DateTime DataDate { get; set; }

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
        private DateTime _CreatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// 上传地址
        /// </summary>
        public string UploadUrl { get; set; }


        public List<FD_AccountTransferDetail> Details { get; set; }

    }

    public class FD_AccountTransferDetail : EntityBase
    {

        public void Update(string EnterpriseID, string PaymentTypeID, string AccountID, decimal Amount, bool IsIn, string DataDateTime, string Remarks)
        {
            this.EnterpriseID = EnterpriseID;
            this.PaymentTypeID = PaymentTypeID;
            this.AccountID = AccountID;
            this.Amount = Amount;
            this.IsIn = IsIn;
            this.DataDateTime = Convert.ToDateTime(DataDateTime);
            this.Remarks = Remarks;
        }


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
        /// Guid
        /// </summary>		
        public Guid Guid { get; set; }

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
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// IsIn
        /// </summary>		
        public bool IsIn { get; set; }

        /// <summary>
        /// DataDateTime 归还/调出时间
        /// </summary>		
        public DateTime DataDateTime { get; set; }


        /// <summary>
        /// Remarks 事由
        /// </summary>		
        public string Remarks { get; set; }

        public DateTime ModifiedDate { get; set; }


        [ForeignKey("NumericalOrder")]
        public FD_AccountTransfer FD_AccountTransfer { get; set; }
    }
}
