using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FA_UseStateTransfer : EntityBase
    {
        public FA_UseStateTransfer()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID = "0";
            Details = new List<FA_UseStateTransferDetail>();
        }

        public void Update(DateTime DataDate,  string Remarks, string EnterpriseID)
        {
            this.DataDate = DataDate;
            this.Remarks = Remarks;
            this.EnterpriseID = EnterpriseID;
            this.ModifiedDate = DateTime.Now;
        }

        public FA_UseStateTransferDetail AddDetail(FA_UseStateTransferDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
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

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }


        public List<FA_UseStateTransferDetail> Details { get; set; }

    }

    public class FA_UseStateTransferDetail : EntityBase
    {

        public void Update(string CardID, string BeforeUseStateID,string AfterUseStateID,string Remarks)
        {
            this.CardID = CardID;
            this.BeforeUseStateID = BeforeUseStateID;
            this.AfterUseStateID = AfterUseStateID;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
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

        public DateTime ModifiedDate { get; set; }


        [ForeignKey("NumericalOrder")]
        public FA_UseStateTransfer FA_UseStateTransfer { get; set; }
    }

    public class FA_UseStateTransferHandle
    {
        public string DataDate { get; set; }
        /// <summary>
        /// 卡片ID
        /// </summary>		
        public string CardID { get; set; }


        /// <summary>
        /// 使用状态
        /// </summary>		
        public string UseStateID { get; set; }

    }
    public class FA_AssetsCardInfos
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperateType { get; set; }

        /// <summary>
        /// 卡片集合
        /// </summary>
        public List<FA_UseStateTransferHandle> lstCardDetailInfo { get; set; }
    }
}
