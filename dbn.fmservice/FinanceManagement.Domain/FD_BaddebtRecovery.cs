using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace FinanceManagement.Domain
{
    public class FD_BaddebtRecovery : EntityBase
    {
        public FD_BaddebtRecovery()
        {
            DataDate = CreatedDate = ModifiedDate = DateTime.Now;
            EnterpriseID = Number = NumericalOrder = TicketedPointID = OwnerID = "0";
            Lines = new List<FD_BaddebtRecoveryDetail>();
        }

        public void AddDetail(FD_BaddebtRecoveryDetail detail)
        {
            Lines.Add(detail);
        }


        public void Update(DateTime DataDate, string TicketedPointID, string Remarks)
        {
            this.DataDate = DataDate;
            this.TicketedPointID = TicketedPointID;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
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

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public List<FD_BaddebtRecoveryDetail> Lines { get; set; }
    }

    public class FD_BaddebtRecoveryDetail : EntityBase
    {
        public FD_BaddebtRecoveryDetail()
        {
            NumericalOrder = BusinessType = CurrentUnit = "0";
            Amount = 0;
        }

        public void Update(int BusiType,  string BusinessType, string CurrentUnit, decimal Amount,decimal RecoveryAmount)
        {
            this.BusiType = BusiType;
            this.Amount = Amount;
            this.BusinessType = BusinessType;
            this.CurrentUnit = CurrentUnit;
            this.RecoveryAmount = RecoveryAmount;
            this.ModifiedDate = DateTime.Now;
        }

        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int BusiType { get; set; }       
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

        public decimal RecoveryAmount { get; set; }
        public DateTime ModifiedDate { get; set; }

        public FD_BaddebtRecovery FD_BaddebtRecovery { get; set; }
    }
}
